using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Snowglow.Editor {
 // Scene-only mesh revisions keep Undo references valid without modifying shared assets.
 public static class SelectedMountainRefit {
  public const string Menu="Snowglow/Environment/Refit selected mountains to terrain";
  const string Folder=CatalogPreparation.Root+"/SceneMeshes/MountainRefits";
  static void Require(bool ok,string message){if(!ok)throw new InvalidOperationException("Mountain refit: "+message);}
  static bool IsMountain(CatalogPlacement p)=>p&&(p.assetId??"").StartsWith("alpine-mountain-",StringComparison.Ordinal);
  public static CatalogPlacement[] Resolve(IEnumerable<GameObject> objects){
   var result=new HashSet<CatalogPlacement>();
   foreach(var obj in objects){
    if(!obj||EditorUtility.IsPersistent(obj)||!obj.scene.IsValid())continue;
    var ancestor=obj.GetComponentInParent<CatalogPlacement>(true);
    if(IsMountain(ancestor))result.Add(ancestor);
    foreach(var child in obj.GetComponentsInChildren<CatalogPlacement>(true))if(IsMountain(child))result.Add(child);
   }
   return result.OrderBy(p=>p.placementId,StringComparer.Ordinal).ToArray();
  }
  [MenuItem(Menu,true)]static bool Available()=>!EditorApplication.isPlayingOrWillChangePlaymode&&Resolve(Selection.gameObjects).Length>0;
  [MenuItem(Menu)]public static void Run(){
   try{var count=Refit(Resolve(Selection.gameObjects));Debug.Log("Refitted "+count+" selected mountain(s). Save the scene to keep the result. Undo is available.");}
   catch(Exception e){Debug.LogException(e);EditorUtility.DisplayDialog("Mountain refit",e.Message,"OK");}
  }
  sealed class Part {public MeshFilter target;public Mesh mesh;}
  sealed class Plan {public CatalogPlacement placement;public Transform root;public Vector3 position;public List<Part> parts=new List<Part>();}
  static void FitColliders(CatalogPlacement placement){
   // PhysX boxes cannot shear. Use their actual rotation/lossy scale frame,
   // rather than a sheared local matrix, and conservatively enclose render bounds.
   foreach(var box in placement.GetComponentsInChildren<BoxCollider>(true)){
    bool found=false;var bounds=new Bounds();var inverse=Quaternion.Inverse(box.transform.rotation);
    foreach(var renderer in placement.GetComponentsInChildren<MeshRenderer>(true)){
     var b=renderer.bounds;
     for(int i=0;i<8;i++){
      var world=b.center+Vector3.Scale(b.extents,new Vector3((i&1)==0?-1:1,(i&2)==0?-1:1,(i&4)==0?-1:1));
      var point=inverse*(world-box.transform.position);
      if(!found){bounds=new Bounds(point,Vector3.zero);found=true;}else bounds.Encapsulate(point);
     }
    }
    Require(found,"no renderer for collider.");
    box.center=box.transform.InverseTransformPoint(box.transform.position+box.transform.rotation*bounds.center);
    var scale=box.transform.lossyScale;box.size=new Vector3(bounds.size.x/Mathf.Abs(scale.x),bounds.size.y/Mathf.Abs(scale.y),bounds.size.z/Mathf.Abs(scale.z));
    PrefabUtility.RecordPrefabInstancePropertyModifications(box);EditorUtility.SetDirty(box);
   }
  }
  internal static Vector3[] Read(Mesh mesh){
   using(var data=MeshUtility.AcquireReadOnlyMeshData(mesh))
   using(var values=new NativeArray<Vector3>(data[0].vertexCount,Allocator.Temp)){data[0].GetVertices(values);return values.ToArray();}
  }
  static float Ground(Terrain terrain,Vector3 point){
   var local=terrain.transform.InverseTransformPoint(point);var size=terrain.terrainData.size;
   Require(local.x>=0&&local.z>=0&&local.x<=size.x&&local.z<=size.z,"entire mountain must be inside its Terrain.");
   return terrain.SampleHeight(point)+terrain.transform.position.y;
  }
  static Plan Prepare(CatalogPlacement p){
   Require(!EditorUtility.IsPersistent(p)&&!PrefabStageUtility.GetPrefabStage(p.gameObject),"use a placed scene mountain, outside Prefab Mode.");
   var entry=CatalogPreparation.Library().Find(p.assetId);Require(entry&&entry.prefab,"missing original catalog mountain: "+p.assetId);
   var terrain=p.gameObject.scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<Terrain>()).SingleOrDefault();
   Require(terrain&&terrain.terrainData,"scene must contain one Terrain.");
   var parent=p.transform.parent;
   var root=parent&&parent.name.StartsWith("Mountain lot",StringComparison.Ordinal)&&parent.GetComponentsInChildren<CatalogPlacement>(true).Count(IsMountain)==1?parent:p.transform;
   Require(p.transform.lossyScale.x>0&&p.transform.lossyScale.y>0&&p.transform.lossyScale.z>0,"use positive, nonzero scales.");
   // Sample the full rotated footprint. Keep the summit shape above the highest ground.
   var targets=p.GetComponentsInChildren<MeshFilter>(true);Require(targets.Length>0,"no mountain meshes.");
   var sources=entry.prefab.GetComponentsInChildren<MeshFilter>(true);
   var originals=targets.Select(t=>{
    var source=sources.SingleOrDefault(s=>s.name==t.name);Require(source&&source.sharedMesh,"unrecognized part: "+t.name);
    return Read(source.sharedMesh).Select(v=>t.transform.TransformPoint(v)).ToArray();
   }).ToArray();
   var world=originals.SelectMany(v=>v).ToArray();var bounds=new Bounds(world[0],Vector3.zero);foreach(var v in world)bounds.Encapsulate(v);
   float reference=float.NegativeInfinity;
   for(int x=0;x<=16;x++)for(int z=0;z<=16;z++)reference=Mathf.Max(reference,Ground(terrain,new Vector3(Mathf.Lerp(bounds.min.x,bounds.max.x,x/16f),0,Mathf.Lerp(bounds.min.z,bounds.max.z,z/16f))));
   float shift=reference-bounds.min.y;var plan=new Plan{placement=p,root=root,position=root.position+Vector3.up*shift};
   // Do not introduce obstacles in the authoritative snow field (its mask is not changed here).
   var snow=p.gameObject.scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<SnowField>()).FirstOrDefault();
   if(snow)Require(bounds.max.x<-128||bounds.min.x>128||bounds.max.z<-128||bounds.min.z>128,"keep background mountains outside the interactive snow field.");
   float height=entry.localBounds.size.y;Require(height>.001f,"invalid source bounds.");
   float embed=2;
   // Base edges must be buried between vertices too, including terrain hollows.
   for(int part=0;part<targets.Length;part++){
    var source=sources.Single(s=>s.name==targets[part].name);var vertices=Read(source.sharedMesh);
    var low=vertices.Select(v=>entry.prefab.transform.InverseTransformPoint(source.transform.TransformPoint(v)).y<=entry.localBounds.min.y+.001f).ToArray();
    using(var data=MeshUtility.AcquireReadOnlyMeshData(source.sharedMesh))for(int sub=0;sub<data[0].subMeshCount;sub++)
    using(var indices=new NativeArray<int>(data[0].GetSubMesh(sub).indexCount,Allocator.Temp)){
     data[0].GetIndices(indices,sub);
     for(int i=0;i<indices.Length;i+=3)for(int edge=0;edge<3;edge++){
      int a=indices[i+edge],b=indices[i+(edge+1)%3];if(!low[a]||!low[b])continue;
      float start=Ground(terrain,originals[part][a]),end=Ground(terrain,originals[part][b]);
      for(int sample=0;sample<=32;sample++){float t=sample/32f;embed=Mathf.Max(embed,Mathf.Lerp(start,end,t)-Ground(terrain,Vector3.Lerp(originals[part][a],originals[part][b],t))+.75f);}
     }
    }
   }
   try{
    for(int part=0;part<targets.Length;part++){
     var target=targets[part];var source=sources.Single(s=>s.name==target.name);var vertices=Read(source.sharedMesh);
     for(int i=0;i<vertices.Length;i++){
      // Height in the catalog root works for yaw, pitch and nonuniform scaling.
      var sourceLocal=entry.prefab.transform.InverseTransformPoint(source.transform.TransformPoint(vertices[i]));
      float t=Mathf.Clamp01((sourceLocal.y-entry.localBounds.min.y)/height);
      float weight=1-Mathf.SmoothStep(0,1,t/.55f);
      var at=originals[part][i]+Vector3.up*shift;
      var baseLocal=sourceLocal;baseLocal.y=entry.localBounds.min.y;
      var baseVertex=source.transform.InverseTransformPoint(entry.prefab.transform.TransformPoint(baseLocal));
      float baseHeight=target.transform.TransformPoint(baseVertex).y+shift;
      at.y+=(Ground(terrain,at)-embed-baseHeight)*weight;
      // Transform is still at its old position while planning, so subtract the root shift.
      vertices[i]=target.transform.InverseTransformPoint(at-Vector3.up*shift);
      Require(float.IsFinite(vertices[i].x)&&float.IsFinite(vertices[i].y)&&float.IsFinite(vertices[i].z),"nonfinite fitted vertex.");
     }
     var mesh=new Mesh{name=p.placementId+"-"+target.name+"-selected-refit",indexFormat=source.sharedMesh.indexFormat};
     using(var data=MeshUtility.AcquireReadOnlyMeshData(source.sharedMesh))
     using(var uv=new NativeArray<Vector2>(data[0].vertexCount,Allocator.Temp)){
      data[0].GetUVs(0,uv);mesh.vertices=vertices;mesh.uv=uv.ToArray();mesh.subMeshCount=data[0].subMeshCount;
      for(int sub=0;sub<data[0].subMeshCount;sub++)using(var indices=new NativeArray<int>(data[0].GetSubMesh(sub).indexCount,Allocator.Temp)){data[0].GetIndices(indices,sub);mesh.SetTriangles(indices.ToArray(),sub);}
     }
     mesh.vertices=vertices;mesh.RecalculateNormals();mesh.RecalculateBounds();plan.parts.Add(new Part{target=target,mesh=mesh});
    }
    return plan;
   }catch{foreach(var part in plan.parts)UnityEngine.Object.DestroyImmediate(part.mesh);throw;}
  }
  public static int Refit(CatalogPlacement[] mountains){
   Require(!EditorApplication.isPlayingOrWillChangePlaymode,"stop Play first.");Require(mountains.Length>0,"select a mountain group or one of its parts.");
   var plans=new List<Plan>();var paths=new List<string>();int group=-1;
   try{
    // Preflight all selections before modifying any scene objects.
    foreach(var p in mountains.Distinct())plans.Add(Prepare(p));
    System.IO.Directory.CreateDirectory(Folder);AssetDatabase.Refresh();
    foreach(var plan in plans)foreach(var part in plan.parts){
     string path=AssetDatabase.GenerateUniqueAssetPath(Folder+"/"+part.mesh.name+".asset");AssetDatabase.CreateAsset(part.mesh,path);paths.Add(path);
    }
    Undo.IncrementCurrentGroup();group=Undo.GetCurrentGroup();Undo.SetCurrentGroupName("Refit selected mountains to terrain");
    foreach(var plan in plans){
     Undo.RecordObject(plan.root,"Seat mountain");plan.root.position=plan.position;PrefabUtility.RecordPrefabInstancePropertyModifications(plan.root);
     foreach(var part in plan.parts){Undo.RecordObject(part.target,"Fit mountain mesh");part.target.sharedMesh=part.mesh;PrefabUtility.RecordPrefabInstancePropertyModifications(part.target);}
     foreach(var box in plan.placement.GetComponentsInChildren<BoxCollider>(true))Undo.RecordObject(box,"Fit mountain collider");
     FitColliders(plan.placement);
     Physics.SyncTransforms();
     foreach(var box in plan.placement.GetComponentsInChildren<BoxCollider>(true)){
      var bounds=box.bounds;bounds.Expand(.05f);
      foreach(var renderer in plan.placement.GetComponentsInChildren<MeshRenderer>(true))Require(bounds.Contains(renderer.bounds.min)&&bounds.Contains(renderer.bounds.max),"collider does not cover the fitted mountain.");
      if(plan.placement.gameObject.scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<SnowField>()).Any())Require(bounds.max.x<-128||bounds.min.x>128||bounds.max.z<-128||bounds.min.z>128,"fitted collider would enter the interactive snow field.");
     }
     EditorSceneManager.MarkSceneDirty(plan.placement.gameObject.scene);
    }
    Undo.FlushUndoRecordObjects();Undo.CollapseUndoOperations(group);AssetDatabase.SaveAssets();SceneView.RepaintAll();return plans.Count;
   }catch{
    if(group>=0)Undo.RevertAllDownToGroup(group);
    foreach(var path in paths)AssetDatabase.DeleteAsset(path);
    throw;
   }finally{foreach(var plan in plans)foreach(var part in plan.parts)if(part.mesh&&!AssetDatabase.Contains(part.mesh))UnityEngine.Object.DestroyImmediate(part.mesh);}
  }
 }
}
