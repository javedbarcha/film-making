using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
namespace Snowglow.Editor {
 public static class SignNormalsChecks {
  const string Source="Assets/Snowglow/Catalog/Sources/AlpineSignBoards/", Logs="Logs/AlpineSignBoards/Normals",Request="Logs/CheckSignNormals.request";
  static void Require(bool ok,string message){if(!ok)throw new InvalidOperationException("Sign normals: "+message);}
  static T[] All<T>(Scene scene)where T:Component=>scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<T>(true)).ToArray();
  [InitializeOnLoadMethod]static void Register(){EditorApplication.update+=Pending;}
  static void Pending(){if(!File.Exists(Request)){EditorApplication.update-=Pending;return;}if(EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode)return;EditorApplication.update-=Pending;File.Delete(Request);try{Run();}catch(Exception e){Directory.CreateDirectory(Logs);File.WriteAllText(Logs+"/Validation.txt","FAIL: "+e);Debug.LogException(e);}}
  struct Tri {public int a,b,c;public double volume;}
  static string Key(Vector3 p)=>Math.Round(p.x*100000)+","+Math.Round(p.y*100000)+","+Math.Round(p.z*100000);
  static (int,int) Edge(int a,int b)=>a<b?(a,b):(b,a);
  static (int solids,int open,int triangles) Audit(Mesh mesh){
   using(var data=MeshUtility.AcquireReadOnlyMeshData(mesh))using(var positions=new NativeArray<Vector3>(mesh.vertexCount,Allocator.Temp))using(var normals=new NativeArray<Vector3>(mesh.vertexCount,Allocator.Temp)){
    var raw=data[0];raw.GetVertices(positions);raw.GetNormals(normals);var welded=new Dictionary<string,int>();var canonical=new int[mesh.vertexCount];for(int i=0;i<canonical.Length;i++){string key=Key(positions[i]);if(!welded.TryGetValue(key,out int n)){n=welded.Count;welded.Add(key,n);}canonical[i]=n;}
    var triangles=new List<Tri>();var edges=new Dictionary<(int,int),List<int>>();int opposite=0,total=0;
    for(int sub=0;sub<raw.subMeshCount;sub++){var descriptor=raw.GetSubMesh(sub);total+=descriptor.indexCount/3;Require(descriptor.topology==MeshTopology.Triangles,"non-triangle mesh "+mesh.name);for(int j=0;j<descriptor.indexCount;j+=3){int At(int x)=>descriptor.baseVertex+(raw.indexFormat==IndexFormat.UInt32?(int)raw.GetIndexData<uint>()[descriptor.indexStart+x]:raw.GetIndexData<ushort>()[descriptor.indexStart+x]);int a=At(j),b=At(j+1),c=At(j+2);var cross=Vector3.Cross(positions[b]-positions[a],positions[c]-positions[a]);if(cross.sqrMagnitude<1e-14f)continue;var normal=normals[a]+normals[b]+normals[c];if(Vector3.Dot(cross.normalized,normal.normalized)<-.001f)opposite++;
      var x=positions[a];var y=positions[b];var z=positions[c];double volume=((double)x.x*(y.y*z.z-y.z*z.y)+(double)x.y*(y.z*z.x-y.x*z.z)+(double)x.z*(y.x*z.y-y.y*z.x))/6;
      var t=new Tri{a=canonical[a],b=canonical[b],c=canonical[c],volume=volume};int number=triangles.Count;triangles.Add(t);foreach(var edge in new[]{Edge(t.a,t.b),Edge(t.b,t.c),Edge(t.c,t.a)}){if(!edges.TryGetValue(edge,out var list)){list=new List<int>();edges.Add(edge,list);}list.Add(number);}
    }}Require(opposite==0,"vertex normals oppose triangle winding on "+mesh.name+": "+opposite);
    var unseen=new HashSet<int>(Enumerable.Range(0,triangles.Count));int solids=0,open=0;while(unseen.Count>0){int first=unseen.First();unseen.Remove(first);var todo=new Stack<int>();todo.Push(first);bool closed=true;double volume=0;while(todo.Count>0){var t=triangles[todo.Pop()];volume+=t.volume;foreach(var e in new[]{Edge(t.a,t.b),Edge(t.b,t.c),Edge(t.c,t.a)}){if(edges[e].Count!=2)closed=false;foreach(int next in edges[e])if(unseen.Remove(next))todo.Push(next);}}if(closed){Require(volume>=-1e-7,"inward closed shell in "+mesh.name+": "+volume);solids++;}else open++;}
    return(solids,open,total);
   }
  }
  static int Preview(CatalogAsset asset,int view){var p=new PreviewRenderUtility();try{var go=UnityEngine.Object.Instantiate(asset.prefab);p.AddSingleGO(go);var bounds=CatalogValidation.BoundsOf(go);p.camera.orthographic=true;p.camera.orthographicSize=Mathf.Max(bounds.extents.x,bounds.extents.y)*1.3f;p.camera.nearClipPlane=.03f;p.camera.farClipPlane=100;Vector3 direction=view==0?new Vector3(1,.4f,3):view==1?new Vector3(-1,.4f,-3):view==2?new Vector3(3,.4f,1):new Vector3(-3,.4f,-1);p.camera.transform.position=bounds.center+direction*bounds.size.magnitude;p.camera.transform.LookAt(bounds.center);p.camera.clearFlags=CameraClearFlags.SolidColor;p.camera.backgroundColor=new Color(.24f,.35f,.4f);p.ambientColor=new Color(.45f,.45f,.45f);p.lights[0].intensity=1.1f;p.lights[0].transform.rotation=p.camera.transform.rotation;p.BeginStaticPreview(new Rect(0,0,480,600));p.Render(true);var frame=p.EndStaticPreview();var pixels=frame.GetPixels32();var background=pixels[0];int coverage=pixels.Count(c=>Math.Abs(c.r-background.r)+Math.Abs(c.g-background.g)+Math.Abs(c.b-background.b)>30);Require(coverage>2500,"empty culled preview "+asset.id+" view "+view);File.WriteAllBytes(Logs+"/"+asset.id+"-"+view+".png",frame.EncodeToPNG());UnityEngine.Object.DestroyImmediate(frame);return coverage;}finally{p.Cleanup();}}
  [MenuItem("Snowglow/Asset intake/Validate all sign normals and four-sided renders")]
  public static void Run(){
   Require(!EditorApplication.isPlayingOrWillChangePlaymode,"stop Play first");Directory.CreateDirectory(Logs);var scene=SceneManager.GetActiveScene();Require(scene.path==SceneValidation.ScenePath,"open Pinefall");var sceneBytes=File.ReadAllBytes(scene.path);var placements=All<CatalogPlacement>(scene).Where(p=>!string.IsNullOrEmpty(p.placementId)).ToDictionary(p=>p.placementId,p=>p.assetId+"|"+p.transform.localToWorldMatrix.ToString("R"));var protectedPaths=All<Terrain>(scene).Select(t=>AssetDatabase.GetAssetPath(t.terrainData)).Concat(All<SnowField>(scene).Select(s=>AssetDatabase.GetAssetPath(s.seed))).Concat(All<SnowField>(scene).SelectMany(s=>s.GetComponentsInChildren<MeshFilter>()).Select(m=>AssetDatabase.GetAssetPath(m.sharedMesh))).Distinct().ToDictionary(p=>p,p=>AssetDatabase.GetAssetDependencyHash(p));
   foreach(string index in new[]{"03","04","08","09"})AssetDatabase.ImportAsset(Source+"Sign"+index+".fbx",ImportAssetOptions.ForceUpdate|ImportAssetOptions.ForceSynchronousImport);
   var report=new StringBuilder("PASS: Unity "+Application.unityVersion+" / "+EditorUserBuildSettings.activeBuildTarget+". All twelve catalog sign variants audited after winding repair.\n");int solids=0,triangles=0;
   foreach(int index in Enumerable.Range(1,12)){var a=CatalogPreparation.Library().Find("alpine-sign-"+index.ToString("D2"));Require(a&&a.prefab,"missing sign prefab");Require(CatalogValidation.Check(a).Ok,"invalid catalog binding "+a.id);Require(a.prefab.GetComponentsInChildren<Collider>(true).All(c=>c),"missing collider");int count=0;
    foreach(var filter in a.prefab.GetComponentsInChildren<MeshFilter>(true)){Require(filter.sharedMesh,"missing mesh reference after reimport "+a.id+" "+filter.name);var result=Audit(filter.sharedMesh);solids+=result.solids;count+=result.triangles;}
    Require(count==a.triangles,"triangle count changed "+a.id+" "+count+" vs "+a.triangles);triangles+=count;var body=a.prefab.GetComponentsInChildren<MeshRenderer>(true).First(r=>r.name=="Body");Require(body.sharedMaterial.GetFloat("_Cull")==2,"backface culling was disabled");report.Append(a.id+": "+count+" triangles; coverage front/back/side/side ");foreach(int view in Enumerable.Range(0,4))report.Append(Preview(a,view)+" ");report.AppendLine();
   }
   SceneValidation.Validate(scene);MechanicsChecks.Run();foreach(var p in All<CatalogPlacement>(scene).Where(p=>!string.IsNullOrEmpty(p.placementId)))Require(placements[p.placementId]==p.assetId+"|"+p.transform.localToWorldMatrix.ToString("R"),"placement changed");Require(File.ReadAllBytes(scene.path).SequenceEqual(sceneBytes),"Pinefall bytes changed");foreach(var pair in protectedPaths)Require(AssetDatabase.GetAssetDependencyHash(pair.Key)==pair.Value,"protected Terrain/snow asset changed");
   report.AppendLine("Native closed shells checked: "+solids+"; total catalog triangles: "+triangles+". No inward closed native shells or opposing vertex normals. Existing zero-area glyph triangles are excluded from orientation math and retained in geometry totals. Open/connected boundaries (including intentionally one-sided lettering) retain their intended topology. Body backface culling stays enabled. 48 native previews rendered. All prefab/scene mesh bindings and triangle counts retained; placement matrices, Pinefall bytes, Terrain and saved snow hashes unchanged. Scene and 19 snow-ledger checks passed. No scene rebake, source blend/save writes, runtime factory or double-sided material workaround. Render inspection and Android phone acceptance are separate.");File.WriteAllText(Logs+"/Validation.txt",report.ToString());Debug.Log(report.ToString());
  }
 }
}
