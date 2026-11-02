using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
namespace Snowglow.Editor {
 public static class IntersectionObstacleAuthoring {
  const string Request="Logs/IntersectionObstacle.request";
  [InitializeOnLoadMethod]static void Reload(){EditorApplication.update+=Pending;}
  static void Pending(){
   if(!File.Exists(Request)){EditorApplication.update-=Pending;return;}
   if(EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode)return;
   EditorApplication.update-=Pending;string action=File.ReadAllText(Request).Trim();File.Delete(Request);
   try{if(action=="fix")Repair();else Audit();}catch(Exception e){Directory.CreateDirectory("Logs/IntersectionObstacle");File.WriteAllText("Logs/IntersectionObstacle/Audit.txt",e.ToString());Debug.LogException(e);}
  }
  static void Require(bool ok,string message){if(!ok)throw new InvalidOperationException(message);}
  static T[] All<T>(Scene scene)where T:Component=>scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<T>(true)).ToArray();
  static string Placements(Scene scene)=>string.Join(";",All<CatalogPlacement>(scene).Where(p=>!string.IsNullOrEmpty(p.placementId)).OrderBy(p=>p.placementId).Select(p=>p.placementId+"|"+p.assetId+"|"+p.transform.localToWorldMatrix.ToString("R")));
  static string SnowData(SnowSeed seed)=>JsonUtility.ToJson(new SnowVolumeGrid{depth=seed.grid.depth,credit=seed.grid.credit,initialVolume=seed.grid.initialVolume,disposed=seed.grid.disposed,revision=seed.grid.revision,blocked=new bool[0]});
  static void ClearIntersection(){
   Physics.SyncTransforms();
   var hits=Physics.OverlapBox(new Vector3(0,8,0),new Vector3(6,2.5f,6),Quaternion.identity,1<<0,QueryTriggerInteraction.Ignore);
   Require(hits.Length==0,"Intersection still blocked: "+string.Join(", ",hits.Select(c=>Hierarchy(c.transform))));
   foreach(var direction in new[]{Vector3.forward,Vector3.back,Vector3.left,Vector3.right}){
    bool blocked=Physics.BoxCast(new Vector3(0,6.6f,0)-direction*12,new Vector3(1,.7f,1.8f),direction,out var hit,Quaternion.LookRotation(direction),24,1<<0,QueryTriggerInteraction.Ignore);
    Require(!blocked,"Vehicle-sized intersection sweep blocked: "+(hit.collider?Hierarchy(hit.transform):"unknown"));
   }
  }
  [MenuItem("Snowglow/Environment/Repair central intersection mountain proxies")]
  public static void Repair(){
   var scene=SceneManager.GetActiveScene();Require(!EditorApplication.isPlayingOrWillChangePlaymode&&scene.path==SceneValidation.ScenePath&&!scene.isDirty,"Stop Play and save Pinefall before repair.");
   Directory.CreateDirectory("Logs/IntersectionObstacle");string backup="Logs/IntersectionObstacle/Pinefall-before.unity";if(!File.Exists(backup))File.Copy(scene.path,backup);
   var session=All<PrototypeSession>(scene).Single();string poses=Placements(scene),snow=SnowData(session.Snow.seed);
   var geometry=All<MeshFilter>(scene).Where(m=>m.sharedMesh).Select(m=>AssetDatabase.GetAssetPath(m.sharedMesh)).Concat(All<Terrain>(scene).Select(t=>AssetDatabase.GetAssetPath(t.terrainData))).Where(p=>!string.IsNullOrEmpty(p)).Distinct().ToDictionary(p=>p,p=>AssetDatabase.GetAssetDependencyHash(p));
   var mountains=All<CatalogPlacement>(scene).Where(p=>(p.placementId??"").StartsWith("pinefall-mountain-",StringComparison.Ordinal)).ToArray();Require(mountains.Length==12,"Expected twelve mountains.");
   Physics.SyncTransforms();var oldBounds=mountains.SelectMany(p=>p.GetComponentsInChildren<BoxCollider>(true)).Select(b=>b.bounds).ToArray();
   foreach(var mountain in mountains)AlpineTerrainFitting.RefitCollisionProxies(mountain);
   AlpineTerrainFitting.ValidateCollisionProxies(scene);ClearIntersection();
   var original=session.Snow.seed;var mask=(bool[])original.grid.blocked.Clone();int changed=0;
   for(int i=0;i<mask.Length;i++){
    float x=original.grid.X(i),z=original.grid.Z(i);if(!oldBounds.Any(b=>x>=b.min.x-.25f&&x<=b.max.x+.25f&&z>=b.min.z-.25f&&z<=b.max.z+.25f))continue;
    bool value=Physics.CheckBox(new Vector3(x,SnowField.Ground+.85f,z),new Vector3(SnowVolumeGrid.Cell*.45f,.65f,SnowVolumeGrid.Cell*.45f),Quaternion.identity,1<<0,QueryTriggerInteraction.Ignore);
    if(mask[i]!=value){mask[i]=value;changed++;}
   }
   if(changed>0){
    const string path="Assets/Snowglow/Catalog/SceneData/CentralIntersectionSnowSeed.asset";
    var seed=AssetDatabase.LoadAssetAtPath<SnowSeed>(path);
    if(!seed){seed=UnityEngine.Object.Instantiate(original);AssetDatabase.CreateAsset(seed,path);}else EditorUtility.CopySerialized(original,seed);
    seed.name="Pinefall | corrected central intersection obstacles";seed.grid.blocked=mask;session.Snow.seed=seed;EditorUtility.SetDirty(seed);EditorUtility.SetDirty(session.Snow);
   }
   Require(SnowData(session.Snow.seed)==snow,"Snow volume/credit changed.");Require(Placements(scene)==poses,"Scenery poses changed.");
   SceneValidation.Validate(scene);AssetDatabase.SaveAssets();EditorSceneManager.MarkSceneDirty(scene);Require(EditorSceneManager.SaveScene(scene),"Scene save failed.");
   scene=EditorSceneManager.OpenScene(SceneValidation.ScenePath);SceneValidation.Validate(scene);AlpineTerrainFitting.ValidateCollisionProxies(scene);ClearIntersection();
   session=All<PrototypeSession>(scene).Single();Require(SnowData(session.Snow.seed)==snow&&Placements(scene)==poses,"Saved scenery/snow changed.");
   foreach(var asset in geometry)Require(AssetDatabase.GetAssetDependencyHash(asset.Key)==asset.Value,"Terrain or mesh changed: "+asset.Key);
   MechanicsChecks.Run();Audit();
   File.WriteAllText("Logs/IntersectionObstacle/Validation.txt","PASS: Unity "+Application.unityVersion+" / "+EditorUserBuildSettings.activeBuildTarget+". Refit 12 misplaced mountain proxies to their fitted meshes; saved/reopened Pinefall. Central intersection overlap and four vehicle-sized directional sweeps clear. All mountain proxies cover their own mesh bounds outside the interactive field. Updated "+changed+" local obstacle flags, preserving every depth/credit/accounting value and original seed. All catalogue placement IDs/transforms, Terrain and mesh assets unchanged. Scene validation and 19 snow-ledger checks passed. No player-save writes. Full live driving and Android/device performance remain untested.");
  }
  static string Hierarchy(Transform t)=>t.parent?Hierarchy(t.parent)+"/"+t.name:t.name;
  [MenuItem("Snowglow/Environment/Audit central intersection obstacle")]
  public static void Audit(){
   var scene=SceneManager.GetActiveScene();if(scene.path!=SceneValidation.ScenePath)throw new InvalidOperationException("Open Pinefall.");
   Physics.SyncTransforms();var log=new StringBuilder("Intersection collider audit at X0/Z0\n");
   var area=new Bounds(new Vector3(0,9,0),new Vector3(14,8,14));
   foreach(var c in scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<Collider>(true)).Where(c=>c.enabled&&c.gameObject.activeInHierarchy&&c.bounds.Intersects(area))){
    log.AppendLine(c.GetType().Name+" | "+Hierarchy(c.transform)+" | bounds "+c.bounds.ToString("F3")+" | trigger "+c.isTrigger+" | layer "+c.gameObject.layer);
    foreach(var r in c.GetComponentsInChildren<Renderer>(true))log.AppendLine("  renderer "+r.name+" enabled "+r.enabled+" active "+r.gameObject.activeInHierarchy+" bounds "+r.bounds.ToString("F3"));
   }
   foreach(float x in new[]{-4f,0,4})foreach(float z in new[]{-4f,0,4})foreach(var h in Physics.RaycastAll(new Vector3(x,25,z),Vector3.down,25,~0,QueryTriggerInteraction.Ignore).OrderByDescending(h=>h.point.y))log.AppendLine("Ray "+x+","+z+" height "+h.point.y+" | "+Hierarchy(h.transform));
   Directory.CreateDirectory("Logs/IntersectionObstacle");File.WriteAllText("Logs/IntersectionObstacle/Audit.txt",log.ToString());Debug.Log(log.ToString());
  }
 }
}
