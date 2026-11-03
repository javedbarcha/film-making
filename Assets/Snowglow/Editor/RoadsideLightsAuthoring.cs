using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Snowglow.Editor {
 public static class RoadsideLightsAuthoring {
  const string Request="Logs/ExpandRoadsideLights.request",Logs="Logs/RoadsideLights",Prefix="pinefall-road-light-";
  static T[] All<T>(Scene s)where T:Component=>s.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<T>(true)).ToArray();
  static bool Added(CatalogPlacement p)=>(p.placementId??"").StartsWith(Prefix,StringComparison.Ordinal);
  static bool Fixture(CatalogPlacement p)=>(p.assetId??"").StartsWith("alpine-street-light-",StringComparison.Ordinal);
  static void Require(bool c,string m){if(!c)throw new InvalidOperationException("Roadside lights: "+m);}
  static string SnowData(SnowSeed s)=>JsonUtility.ToJson(new SnowVolumeGrid{depth=s.grid.depth,credit=s.grid.credit,initialVolume=s.grid.initialVolume,disposed=s.grid.disposed,revision=s.grid.revision,blocked=new bool[0]});
  [InitializeOnLoadMethod]static void Register(){EditorApplication.update+=Pending;}
  static void Pending(){if(!File.Exists(Request)){EditorApplication.update-=Pending;return;}if(EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode)return;EditorApplication.update-=Pending;File.Delete(Request);try{Install();}catch(Exception e){Directory.CreateDirectory(Logs);File.WriteAllText(Logs+"/Validation.txt","FAIL: "+e);Debug.LogException(e);}}
  struct Candidate{public Vector3 position,toward;}
  static Bounds Proxy(CatalogAsset a,Vector3 p,Quaternion q){
   var box=a.prefab.GetComponentInChildren<BoxCollider>(true);Require(box,"missing pole proxy");var result=new Bounds(p+q*box.transform.TransformPoint(box.center),Vector3.zero);
   for(int i=0;i<8;i++)result.Encapsulate(p+q*box.transform.TransformPoint(box.center+Vector3.Scale(box.size*.5f,new Vector3((i&1)==0?-1:1,(i&2)==0?-1:1,(i&4)==0?-1:1))));return result;
  }
  static bool Clear(Bounds b,Bounds[] roads,CatalogPlacement self=null){
   var expanded=b;expanded.Expand(new Vector3(1.6f,0,1.6f));if(roads.Any(r=>expanded.min.x<r.max.x&&expanded.max.x>r.min.x&&expanded.min.z<r.max.z&&expanded.max.z>r.min.z))return false;
   return !Physics.OverlapBox(b.center,b.extents+new Vector3(.6f,0,.6f),Quaternion.identity,1<<0,QueryTriggerInteraction.Ignore).Any(c=>!(c is TerrainCollider)&&(!self||!c.transform.IsChildOf(self.transform)));
  }
  static List<Candidate> Candidates(Scene scene){
   var result=new List<Candidate>();var terrain=All<Terrain>(scene).Single();
   foreach(var renderer in All<Renderer>(scene).Where(r=>r.name=="Road | asphalt"&&Mathf.Max(r.bounds.size.x,r.bounds.size.z)>150)){
    var b=renderer.bounds;bool x=b.size.x>b.size.z;float low=x?b.min.x:b.min.z,high=x?b.max.x:b.max.z;
    for(float t=low+10;t<=high-10;t+=5)foreach(float side in new[]{-1f,1f}){var p=x?new Vector3(t,0,b.center.z+side*(b.extents.z+2.5f)):new Vector3(b.center.x+side*(b.extents.x+2.5f),0,t);p.y=terrain.SampleHeight(p)+terrain.transform.position.y-.025f;result.Add(new Candidate{position=p,toward=x?new Vector3(0,0,-side):new Vector3(-side,0,0)});}
   }return result;
  }
  static Quaternion Rotation(CatalogAsset a,Vector3 toward)=>Quaternion.Euler(0,Mathf.Atan2(-toward.z,toward.x)*Mathf.Rad2Deg+(a.localBounds.center.x<-.05f?180:0),0);
  static int RefreshMask(SnowField snow,CatalogPlacement[] added){
   Physics.SyncTransforms();var original=snow.seed;var mask=(bool[])original.grid.blocked.Clone();var bounds=added.SelectMany(p=>p.GetComponentsInChildren<Collider>()).Select(c=>c.bounds).ToArray();int changed=0;
   for(int i=0;i<mask.Length;i++){float x=original.grid.X(i),z=original.grid.Z(i);if(!bounds.Any(b=>x>=b.min.x-.5f&&x<=b.max.x+.5f&&z>=b.min.z-.5f&&z<=b.max.z+.5f))continue;bool value=Physics.CheckBox(new Vector3(x,SnowField.Ground+.85f,z),new Vector3(SnowVolumeGrid.Cell*.45f,.65f,SnowVolumeGrid.Cell*.45f),Quaternion.identity,1<<0,QueryTriggerInteraction.Ignore);if(value!=mask[i]){mask[i]=value;changed++;}}
   if(changed>0){var seed=UnityEngine.Object.Instantiate(original);seed.name="Pinefall | additional roadside light obstacles";seed.grid.blocked=mask;AssetDatabase.CreateAsset(seed,AssetDatabase.GenerateUniqueAssetPath(CatalogPreparation.Root+"/SceneData/RoadsideLightsSnowSeed.asset"));snow.seed=seed;EditorUtility.SetDirty(seed);EditorUtility.SetDirty(snow);}return changed;
  }
  [MenuItem("Snowglow/Environment/Add fourteen roadside street lights")]
  public static void Install(){
   Require(!EditorApplication.isPlayingOrWillChangePlaymode,"stop Play first");var scene=SceneManager.GetActiveScene();Require(scene.path==SceneValidation.ScenePath,"open Pinefall first");Directory.CreateDirectory(Logs);
   if(!File.Exists(Logs+"/Pinefall-before.unity"))File.Copy(scene.path,Logs+"/Pinefall-before.unity");if(scene.isDirty)Require(EditorSceneManager.SaveScene(scene,Logs+"/Pinefall-unsaved-before.unity",true),"unsaved-work backup failed");
   var poses=All<CatalogPlacement>(scene).Where(p=>!Added(p)&&!string.IsNullOrEmpty(p.placementId)).ToDictionary(p=>p.placementId,p=>p.assetId+"|"+p.transform.localToWorldMatrix.ToString("R"));var session=All<PrototypeSession>(scene).Single();string snowBefore=SnowData(session.Snow.seed);
   var paths=All<MeshFilter>(scene).Where(m=>m.sharedMesh).Select(m=>AssetDatabase.GetAssetPath(m.sharedMesh)).Concat(All<Terrain>(scene).Select(t=>AssetDatabase.GetAssetPath(t.terrainData))).Concat(new[]{AssetDatabase.GetAssetPath(session.Snow.seed)}).Where(p=>p.StartsWith("Assets/",StringComparison.Ordinal)).Distinct().ToDictionary(p=>p,p=>AssetDatabase.GetAssetDependencyHash(p));
   int enabledLights=All<Light>(scene).Count(l=>l.enabled&&l.gameObject.activeInHierarchy);var roads=All<Renderer>(scene).Where(r=>r.name=="Road | asphalt").Select(r=>r.bounds).ToArray();Physics.SyncTransforms();var added=All<CatalogPlacement>(scene).Where(Added).ToArray();
   if(added.Length==0){
    var points=Candidates(scene);Require(points.Count>0,"main roads not found");var occupied=All<CatalogPlacement>(scene).Where(Fixture).Select(p=>p.transform.position).ToList();var group=new GameObject("Additional roadside lights | 14 prepared fixtures");SceneManager.MoveGameObjectToScene(group,scene);int[] variants={9,4,10,3,7,9,10,5,4,9,10,3,8,11};
    for(int i=0;i<variants.Length;i++){
     var a=CatalogPreparation.Library().Find("alpine-street-light-"+variants[i].ToString("D2"));Require(a&&a.prefab,"prepared fixture missing");bool placed=false;
     var options=points.Select(p=>new{point=p,rotation=Rotation(a,p.toward),score=occupied.Min(o=>new Vector2(o.x-p.position.x,o.z-p.position.z).sqrMagnitude)}).OrderByDescending(p=>p.score);
     foreach(var option in options){if(option.score<18*18)continue;var bounds=Proxy(a,option.point.position,option.rotation);if(!Clear(bounds,roads))continue;var go=(GameObject)PrefabUtility.InstantiatePrefab(a.prefab,scene);go.transform.SetParent(group.transform,true);go.transform.SetPositionAndRotation(option.point.position,option.rotation);var p=go.GetComponent<CatalogPlacement>();p.placementId=Prefix+(i+1).ToString("D2");PrefabUtility.RecordPrefabInstancePropertyModifications(p);occupied.Add(option.point.position);Physics.SyncTransforms();placed=true;break;}Require(placed,"no clear roadside position for fixture "+(i+1));
    }
   }else Require(added.Length==14,"partial expansion found; preserve and inspect it");
   added=All<CatalogPlacement>(scene).Where(Added).ToArray();int maskChanges=RefreshMask(session.Snow,added);Require(SnowData(session.Snow.seed)==snowBefore,"snow volume/credit changed");SceneValidation.Validate(scene);AssetDatabase.SaveAssets();EditorSceneManager.MarkSceneDirty(scene);Require(EditorSceneManager.SaveScene(scene),"scene save failed");scene=EditorSceneManager.OpenScene(SceneValidation.ScenePath);SceneValidation.Validate(scene);
   added=All<CatalogPlacement>(scene).Where(Added).ToArray();Require(added.Length==14,"saved expansion count differs");Physics.SyncTransforms();foreach(var p in added){Require(PrefabUtility.IsPartOfPrefabInstance(p),"disconnected fixture");Require(p.GetComponentsInChildren<Light>().Length==0,"optional point light enabled");Require(Clear(p.GetComponentInChildren<Collider>().bounds,roads,p),"saved pole obstructs road/scenery "+p.placementId);}
   foreach(var p in All<CatalogPlacement>(scene).Where(p=>!Added(p)&&!string.IsNullOrEmpty(p.placementId)))Require(poses[p.placementId]==p.assetId+"|"+p.transform.localToWorldMatrix.ToString("R"),"existing placement changed "+p.placementId);foreach(var path in paths)Require(AssetDatabase.GetAssetDependencyHash(path.Key)==path.Value,"existing asset changed "+path.Key);
   Require(SnowData(All<PrototypeSession>(scene).Single().Snow.seed)==snowBefore,"saved snow volume/credit changed");Require(All<Light>(scene).Count(l=>l.enabled&&l.gameObject.activeInHierarchy)==enabledLights,"enabled light count changed");MechanicsChecks.Run();var library=CatalogPreparation.Library();int triangles=added.Sum(p=>library.Find(p.assetId).triangles);
   var report=new StringBuilder("PASS: Unity "+Application.unityVersion+". Added 14 roadside fixture prefab instances; total fixtures "+All<CatalogPlacement>(scene).Count(Fixture)+". Added "+triangles+" triangles / "+added.Sum(p=>p.GetComponentsInChildren<Renderer>().Length)+" renderers. Saved/reopened; road/scenery proxy clearance passed. Optional lights off; enabled light count unchanged.\n");foreach(var p in added)report.AppendLine(p.placementId+" | "+p.assetId+" | "+p.transform.position+" | yaw "+p.transform.eulerAngles.y);
   report.AppendLine("Largest-gap placement along six main-road shoulders, outreach aimed toward roads. Existing placement identities/transforms, Terrain, meshes and original seed hashes unchanged. Updated "+maskChanges+" local obstacle cells in a separate seed copy; all snow depth/credit/volume values retained. Scene validation and 19 snow-ledger checks passed. No district rebake, source/prefab replacement or player-save writes. Native district preview rendered. Continuous driving and device performance untested.");Preview(scene);EditorSceneManager.OpenScene(SceneValidation.ScenePath);File.WriteAllText(Logs+"/Validation.txt",report.ToString());Debug.Log(report.ToString());
  }
  static void Preview(Scene scene){
   var camera=All<Camera>(scene).First();camera.orthographic=true;camera.orthographicSize=145;camera.aspect=16f/9f;camera.transform.SetPositionAndRotation(new Vector3(0,245,-180),Quaternion.Euler(55,0,0));var rt=new RenderTexture(1600,900,24);var previous=RenderTexture.active;Texture2D image=null;
   try{camera.targetTexture=rt;camera.Render();RenderTexture.active=rt;image=new Texture2D(1600,900,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1600,900),0,0);image.Apply();File.WriteAllBytes(Logs+"/District.png",image.EncodeToPNG());}finally{camera.targetTexture=null;RenderTexture.active=previous;rt.Release();UnityEngine.Object.DestroyImmediate(rt);if(image)UnityEngine.Object.DestroyImmediate(image);}
  }
 }
}
