using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Snowglow.Editor {
 public static class SelectedMountainRefitChecks {
  const string Request="Logs/SelectedMountainRefit.request",Report="Logs/SelectedMountainRefit/Validation.txt";
  const string Folder="Assets/Snowglow/MountainRefitCheckFixtures";
  static int assertions;
  static void Check(bool ok,string message){assertions++;if(!ok)throw new InvalidOperationException(message);}
  [InitializeOnLoadMethod]static void Reload(){EditorApplication.update+=Pending;}
  static void Pending(){
   if(!File.Exists(Request)){EditorApplication.update-=Pending;return;}
   if(EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode)return;
   EditorApplication.update-=Pending;File.Delete(Request);
   try{Run();}catch(Exception e){Directory.CreateDirectory(Path.GetDirectoryName(Report));File.WriteAllText(Report,"FAIL: "+e);Debug.LogException(e);}
  }
  [MenuItem("Snowglow/Environment/Run selected mountain refit checks")]
  public static void Run(){
   Check(!EditorApplication.isPlayingOrWillChangePlaymode,"Stop Play first.");assertions=0;
   var active=SceneManager.GetActiveScene();var selection=Selection.objects;
   var previous=AssetDatabase.FindAssets("t:Mesh",new[]{CatalogPreparation.Root+"/SceneMeshes"}).Select(AssetDatabase.GUIDToAssetPath).ToHashSet();
   Scene fixture=default;bool created=false;var catalog=CatalogPreparation.Library();var entries=Enumerable.Range(1,5).Select(i=>catalog.Find("alpine-mountain-"+i.ToString("D2"))).ToArray();
   var hashes=entries.ToDictionary(e=>AssetDatabase.GetAssetPath(e.prefab),e=>AssetDatabase.GetAssetDependencyHash(AssetDatabase.GetAssetPath(e.prefab)));
   string mainHash=File.Exists(SceneValidation.ScenePath)?Convert.ToBase64String(System.Security.Cryptography.SHA256.Create().ComputeHash(File.ReadAllBytes(SceneValidation.ScenePath))):"";
   try{
    Check(!AssetDatabase.IsValidFolder(Folder),"Fixture path already exists; preserve it and investigate.");
    Directory.CreateDirectory(Folder);created=true;AssetDatabase.Refresh();fixture=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive);SceneManager.SetActiveScene(fixture);
    var data=new TerrainData{heightmapResolution=65,size=new Vector3(1000,100,1000)};var heights=new float[65,65];
    for(int z=0;z<65;z++)for(int x=0;x<65;x++)heights[z,x]=.2f+.08f*Mathf.Sin(x*.15f)+.05f*Mathf.Cos(z*.2f);
    data.SetHeights(0,0,heights);AssetDatabase.CreateAsset(data,Folder+"/Terrain.asset");var terrainObject=Terrain.CreateTerrainGameObject(data);terrainObject.transform.position=new Vector3(-500,0,-500);
    var terrain=terrainObject.GetComponent<Terrain>();
    var group=new GameObject("Mountain lot | fixture");group.transform.position=new Vector3(-250,-80,0);group.transform.rotation=Quaternion.Euler(0,29,0);group.transform.localScale=new Vector3(1.1f,1.2f,.9f);
    var mountains=entries.Select((entry,i)=>{
     var lot=new GameObject("Mountain lot | fixture-"+i);lot.transform.SetParent(group.transform,false);lot.transform.localPosition=new Vector3(i*85,0,0);
     var obj=(GameObject)PrefabUtility.InstantiatePrefab(entry.prefab,fixture);obj.transform.SetParent(lot.transform,false);obj.transform.localRotation=Quaternion.Euler(i==2?8:0,31+i*37,0);obj.transform.localScale=Vector3.one*(2+i*.2f);
     var p=obj.GetComponent<CatalogPlacement>();p.placementId="refit-fixture-"+i;return p;
    }).ToArray();
    var untouched=(GameObject)PrefabUtility.InstantiatePrefab(entries[0].prefab,fixture);untouched.transform.position=new Vector3(200,20,200);var untouchedMesh=untouched.GetComponentInChildren<MeshFilter>().sharedMesh;var untouchedPose=untouched.transform.localToWorldMatrix;
    Check(SelectedMountainRefit.Resolve(new[]{group,mountains[0].gameObject}).Length==5,"Parent group selection/deduplication failed.");
    Check(SelectedMountainRefit.Resolve(new[]{mountains[1].GetComponentInChildren<MeshFilter>().gameObject}).Single()==mountains[1],"Child selection failed.");
    Check(SelectedMountainRefit.Resolve(new[]{entries[0].prefab,terrainObject}).Length==0,"Project assets or unrelated objects accepted.");
    // Refit real single-mountain lot roots without moving their shared container.
    group.name="Fixture container";
    var scales=mountains.Select(p=>p.transform.localScale).ToArray();var rotations=mountains.Select(p=>p.transform.localRotation).ToArray();var xz=mountains.Select(p=>p.transform.position).ToArray();
    var initialFilters=mountains.SelectMany(p=>p.GetComponentsInChildren<MeshFilter>(true)).ToArray();var initialRefs=initialFilters.Select(f=>f.sharedMesh).ToArray();
    var boxes=mountains.SelectMany(p=>p.GetComponentsInChildren<BoxCollider>(true)).ToArray();var initialCenters=boxes.Select(b=>b.center).ToArray();var initialSizes=boxes.Select(b=>b.size).ToArray();
    SelectedMountainRefit.Refit(mountains);
    var filters=mountains.SelectMany(p=>p.GetComponentsInChildren<MeshFilter>(true)).ToArray();
    var first=filters.Select(f=>SelectedMountainRefit.Read(f.sharedMesh)).ToArray();
    for(int n=0;n<mountains.Length;n++){
     var p=mountains[n];Check(p.transform.localScale==scales[n]&&Quaternion.Angle(p.transform.localRotation,rotations[n])<.001f,"Scale/rotation changed.");
     Check(Mathf.Abs(p.transform.position.x-xz[n].x)<.001f&&Mathf.Abs(p.transform.position.z-xz[n].z)<.001f,"Horizontal position changed.");
     var entry=entries[n];
     foreach(var filter in p.GetComponentsInChildren<MeshFilter>(true)){
      var source=entry.prefab.GetComponentsInChildren<MeshFilter>(true).Single(f=>f.name==filter.name);var original=SelectedMountainRefit.Read(source.sharedMesh);var fitted=SelectedMountainRefit.Read(filter.sharedMesh);
      Check(original.Length==fitted.Length&&source.sharedMesh.GetIndexCount(0)==filter.sharedMesh.GetIndexCount(0),"Geometry count changed.");
      for(int i=0;i<fitted.Length;i++){
       var sourcePoint=entry.prefab.transform.InverseTransformPoint(source.transform.TransformPoint(original[i]));float t=(sourcePoint.y-entry.localBounds.min.y)/entry.localBounds.size.y;
       if(t>=.55f)Check((fitted[i]-original[i]).magnitude<.002f,"Upper peak deformed.");
       if(t<.0001f){var point=filter.transform.TransformPoint(fitted[i]);Check(point.y<terrain.SampleHeight(point)+terrain.transform.position.y-.5f,"Base floats.");}
      }
      var indices=filter.sharedMesh.triangles;
      for(int i=0;i<indices.Length;i+=3)for(int edge=0;edge<3;edge++){
       int a=indices[i+edge],b=indices[i+(edge+1)%3];
       float ay=entry.prefab.transform.InverseTransformPoint(source.transform.TransformPoint(original[a])).y;
       float by=entry.prefab.transform.InverseTransformPoint(source.transform.TransformPoint(original[b])).y;
       if(ay>entry.localBounds.min.y+.001f||by>entry.localBounds.min.y+.001f)continue;
       for(int sample=0;sample<=32;sample++){
        var point=filter.transform.TransformPoint(Vector3.Lerp(fitted[a],fitted[b],sample/32f));
        Check(point.y<=terrain.SampleHeight(point)+terrain.transform.position.y-.7f,"Base edge floats between vertices.");
       }
      }
     }
     Physics.SyncTransforms();foreach(var box in p.GetComponentsInChildren<BoxCollider>(true)){var b=box.bounds;b.Expand(.02f);foreach(var r in p.GetComponentsInChildren<MeshRenderer>(true))Check(b.Contains(r.bounds.min)&&b.Contains(r.bounds.max),"Collider coverage failed.");}
    }
    var refs=filters.Select(f=>f.sharedMesh).ToArray();SelectedMountainRefit.Refit(mountains);
    for(int f=0;f<filters.Length;f++){var second=SelectedMountainRefit.Read(filters[f].sharedMesh);for(int i=0;i<second.Length;i++)Check((second[i]-first[f][i]).magnitude<.002f,"Repeat fitting accumulated distortion.");}
    var redoRefs=filters.Select(f=>f.sharedMesh).ToArray();Undo.PerformUndo();Check(filters.Select(f=>f.sharedMesh).SequenceEqual(refs),"Undo did not restore mesh references.");Undo.PerformRedo();Check(filters.Select(f=>f.sharedMesh).SequenceEqual(redoRefs),"Redo failed.");
    Undo.PerformUndo();Undo.PerformUndo();Check(filters.Select(f=>f.sharedMesh).SequenceEqual(initialRefs),"Original mesh Undo failed.");
    for(int i=0;i<mountains.Length;i++)Check((mountains[i].transform.position-xz[i]).magnitude<.002f,"Undo seating failed.");
    for(int i=0;i<boxes.Length;i++)Check(boxes[i].center==initialCenters[i]&&boxes[i].size==initialSizes[i],"Undo collider failed.");
    Undo.PerformRedo();Undo.PerformRedo();Check(filters.Select(f=>f.sharedMesh).SequenceEqual(redoRefs),"Full Redo failed.");
    Check(untouched.GetComponentInChildren<MeshFilter>().sharedMesh==untouchedMesh&&untouched.transform.localToWorldMatrix==untouchedPose,"Unselected mountain changed.");
    string path=Folder+"/Check.unity";Check(EditorSceneManager.SaveScene(fixture,path),"Fixture save failed.");EditorSceneManager.CloseScene(fixture,true);fixture=EditorSceneManager.OpenScene(path,OpenSceneMode.Additive);
    var reopened=fixture.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<CatalogPlacement>()).Where(p=>(p.placementId??"").StartsWith("refit-fixture-",StringComparison.Ordinal)).ToArray();Check(reopened.Length==5,"Reopened mountains missing.");
    Check(reopened.SelectMany(p=>p.GetComponentsInChildren<MeshFilter>()).All(f=>f.sharedMesh&&AssetDatabase.Contains(f.sharedMesh)),"Persisted mesh references lost.");
    foreach(var pair in hashes)Check(AssetDatabase.GetAssetDependencyHash(pair.Key)==pair.Value,"Catalog/source dependency changed.");
    if(mainHash!="")Check(mainHash==Convert.ToBase64String(System.Security.Cryptography.SHA256.Create().ComputeHash(File.ReadAllBytes(SceneValidation.ScenePath))),"Pinefall scene modified.");
    Directory.CreateDirectory(Path.GetDirectoryName(Report));File.WriteAllText(Report,"PASS: Unity "+Application.unityVersion+" / "+EditorUserBuildSettings.activeBuildTarget+". "+assertions+" assertions. Five catalog variants on uneven Terrain, parent/child selection and deduplication, yaw/pitch/nonuniform ancestor scale, preserved X/Z/rotation/scale, buried base vertices, unchanged upper peaks and geometry counts, proxy coverage, repeat fitting, mesh Undo/Redo, unselected preservation, fixture scene save/reopen and source dependency hashes. Pinefall scene unchanged. No player-save writes. Rendered seam inspection, manual menu interaction and Android/device testing remain untested.");Debug.Log(File.ReadAllText(Report));
   }finally{
    if(fixture.IsValid()&&fixture.isLoaded)EditorSceneManager.CloseScene(fixture,true);
    if(active.IsValid()&&active.isLoaded)SceneManager.SetActiveScene(active);Selection.objects=selection;
    foreach(var guid in AssetDatabase.FindAssets("t:Mesh",new[]{CatalogPreparation.Root+"/SceneMeshes"})){var path=AssetDatabase.GUIDToAssetPath(guid);if(!previous.Contains(path))AssetDatabase.DeleteAsset(path);}
    if(created&&AssetDatabase.IsValidFolder(Folder))AssetDatabase.DeleteAsset(Folder);
   }
  }
 }
}
