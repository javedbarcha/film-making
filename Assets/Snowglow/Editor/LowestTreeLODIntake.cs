using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Snowglow.Editor {
 public static class LowestTreeLODIntake {
  public const string Source="Assets/Snowglow/Catalog/Sources/AlpineTreesLowestLOD/",ManifestPath=Source+"ExportMetrics.json";
  const string Logs="Logs/AlpineTreesLowestLOD",Request="Logs/UseLowestTreeLOD.request";
  const string SourceBlend="SourceAssets/AlpineTreesLowestLOD/Alpine_Trees_26_Variants_4_LODs.blend";
  [Serializable]public class Export{public Tree[] trees;public int lod;}
  [Serializable]public class Tree{public string index,name,model,sourceCollection;public int triangles;public float[] size;}
  static T[] All<T>(Scene s)where T:Component=>s.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<T>(true)).ToArray();
  static bool IsTree(CatalogPlacement p)=>(p.assetId??"").StartsWith("alpine-tree-",StringComparison.Ordinal);
  static void Require(bool condition,string message){if(!condition)throw new InvalidOperationException("Lowest tree LOD: "+message);}
  [InitializeOnLoadMethod]static void Register(){EditorApplication.update+=Pending;}
  static void Pending(){if(!File.Exists(Request)){EditorApplication.update-=Pending;return;}if(EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode)return;EditorApplication.update-=Pending;File.Delete(Request);try{Install();}catch(Exception e){Directory.CreateDirectory(Logs);File.WriteAllText(Logs+"/Validation.txt","FAIL: "+e);Debug.LogException(e);}}
  static long Triangles(Mesh m){long n=0;for(int i=0;i<m.subMeshCount;i++)n+=(long)m.GetIndexCount(i)/3;return n;}
  static long ForestTriangles(Scene s)=>All<CatalogPlacement>(s).Where(IsTree).Sum(p=>p.GetComponentsInChildren<MeshFilter>(true).Sum(m=>Triangles(m.sharedMesh)));
  static string PhysicalState(CatalogPlacement p){
   var text=new StringBuilder(p.assetId+"|"+p.transform.localToWorldMatrix.ToString("R"));var cull=p.GetComponent<DistanceVisibility>();text.Append("|cull="+(cull?cull.distance.ToString("R"):"none"));
   foreach(var c in p.GetComponentsInChildren<Collider>(true).OrderBy(c=>c.name)){text.Append("|"+c.GetType().Name+"|"+c.transform.localToWorldMatrix.ToString("R")+"|"+c.enabled+"|"+c.isTrigger);if(c is CapsuleCollider cap)text.Append("|"+cap.center.ToString("R")+"|"+cap.radius.ToString("R")+"|"+cap.height.ToString("R")+"|"+cap.direction);else if(c is BoxCollider box)text.Append("|"+box.center.ToString("R")+"|"+box.size.ToString("R"));}return text.ToString();
  }
  static void Prepare(Tree tree,Material bark,Material snow){
   string id="alpine-tree-"+tree.index,path=Source+tree.model+".fbx";var a=CatalogPreparation.Library().Find(id);Require(a&&a.prefab,"existing catalog entry missing "+id);
   AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);var importer=(ModelImporter)AssetImporter.GetAtPath(path);importer.importCameras=false;importer.importLights=false;importer.importAnimation=false;importer.globalScale=1;importer.isReadable=false;importer.materialImportMode=ModelImporterMaterialImportMode.ImportStandard;
   importer.AddRemap(new AssetImporter.SourceAssetIdentifier(typeof(Material),"AlpineTreePalette"),bark);importer.AddRemap(new AssetImporter.SourceAssetIdentifier(typeof(Material),"AlpineTreeSnow"),snow);importer.SaveAndReimport();
   var model=AssetDatabase.LoadAssetAtPath<GameObject>(path);var meshes=model.GetComponentsInChildren<MeshFilter>(true).ToDictionary(m=>m.name,m=>m.sharedMesh);Require(meshes.ContainsKey("TreeWood"),"missing body "+id);Require((CatalogValidation.BoundsOf(model).size-new Vector3(tree.size[0],tree.size[1],tree.size[2])).magnitude<.03f,"imported units/axes differ "+id);
   string prefabPath=AssetDatabase.GetAssetPath(a.prefab),backup=Logs+"/OriginalPrefabs/"+id+".prefab";Directory.CreateDirectory(Path.GetDirectoryName(backup));if(!File.Exists(backup))File.Copy(prefabPath,backup);
   var root=PrefabUtility.LoadPrefabContents(prefabPath);
   try{
    Require(root.GetComponentsInChildren<LODGroup>(true).Length==0,"unexpected existing LODGroup "+id);
    var filters=root.GetComponentsInChildren<MeshFilter>(true);Require(filters.Length==meshes.Count,"mesh groups differ "+id);
    foreach(var filter in filters){string key=filter.name.StartsWith("SnowCap",StringComparison.Ordinal)?"SnowCap":"TreeWood";Require(meshes.ContainsKey(key),"missing matching mesh "+id);filter.sharedMesh=meshes[key];filter.GetComponent<Renderer>().sharedMaterial=key=="SnowCap"?snow:bark;}
    Require((CatalogValidation.BoundsOf(root).size-new Vector3(tree.size[0],tree.size[1],tree.size[2])).magnitude<.04f,"mesh frame differs from existing prefab "+id);
    PrefabUtility.SaveAsPrefabAsset(root,prefabPath);
   }finally{PrefabUtility.UnloadPrefabContents(root);}
   a.sourcePath=path;a.sourceGuid=AssetDatabase.AssetPathToGUID(path);a.creationSourcePath=SourceBlend;a.sourceType=AssetSource.ImportedModel;a.tags=a.tags.Concat(new[]{"lod3-only","lowest-detail","single-detail"}).Distinct().ToArray();a.status=PreparationStatus.NeedsReview;a.visualReviewed=false;a.reviewedPrefabHash="";
   a.provenance="User supplied four-LOD Blender pack; only collection "+tree.sourceCollection+" exported. Original SHA256 895E555B6AE1371E639B207FFD301E5BD8B8D936E99C2FBFD0D4D3FB5425F94D. Embedded text is data and was not executed. LOD0/1/2 excluded. Stable catalog/prefab identity retained.";
   a.assumptions="Lowest supplied LOD3 only at every distance, no LODGroup or higher-detail renderers. Original mounting basis, materials, removable snow, colliders and DistanceVisibility retained. Original higher-detail sources preserved for reversibility.";
   a.reviewNotes="Authorized lowest-detail replacement; Needs Review for final visual and A07 acceptance.";CatalogPreparation.RecordMetrics(a);Require(a.triangles==tree.triangles,"native triangle count differs "+id+": "+a.triangles+" vs "+tree.triangles);Require(CatalogValidation.Check(a).Ok,"catalog validation "+id);EditorUtility.SetDirty(a);
  }
  [MenuItem("Snowglow/Asset intake/Use lowest LOD3 for all alpine trees and wood")]
  public static void Install(){
   var scene=SceneManager.GetActiveScene();Require(scene.path==SceneValidation.ScenePath&&!EditorApplication.isPlayingOrWillChangePlaymode,"open Pinefall with Play stopped");Directory.CreateDirectory(Logs);if(!File.Exists(Logs+"/Pinefall-before.unity"))File.Copy(scene.path,Logs+"/Pinefall-before.unity");if(scene.isDirty)Require(EditorSceneManager.SaveScene(scene,Logs+"/Pinefall-unsaved-before.unity",true),"unsaved-work backup failed");
   var snapshots=All<CatalogPlacement>(scene).Where(p=>!string.IsNullOrEmpty(p.placementId)).ToDictionary(p=>p.placementId,PhysicalState);int count=All<CatalogPlacement>(scene).Count(IsTree);long before=ForestTriangles(scene);Require(count>0,"no existing forest found");
   var session=All<PrototypeSession>(scene).Single();var protectedAssets=All<Terrain>(scene).Select(t=>AssetDatabase.GetAssetPath(t.terrainData)).Concat(session.Snow.GetComponentsInChildren<MeshFilter>(true).Select(m=>AssetDatabase.GetAssetPath(m.sharedMesh))).Concat(new[]{AssetDatabase.GetAssetPath(session.Snow.seed)}).Where(p=>!string.IsNullOrEmpty(p)).Distinct().ToDictionary(p=>p,p=>AssetDatabase.GetAssetDependencyHash(p));
   var data=JsonUtility.FromJson<Export>(File.ReadAllText(ManifestPath));Require(data.lod==3&&data.trees.Length==26,"expected only 26 LOD3 variants");var bark=AssetDatabase.LoadAssetAtPath<Material>("Assets/Snowglow/Catalog/Sources/AlpineTrees/TreePalette.mat");Require(bark,"existing tree palette missing");foreach(var tree in data.trees)Prepare(tree,bark,session.Snow.palette.snow);
   // Prefab edits propagate to existing instances; replace any explicit old-mesh overrides too.
   foreach(var p in All<CatalogPlacement>(scene).Where(IsTree)){
    var row=data.trees.Single(t=>p.assetId=="alpine-tree-"+t.index);var model=AssetDatabase.LoadAssetAtPath<GameObject>(Source+row.model+".fbx");var meshes=model.GetComponentsInChildren<MeshFilter>(true).ToDictionary(m=>m.name,m=>m.sharedMesh);
    foreach(var filter in p.GetComponentsInChildren<MeshFilter>(true)){var mesh=meshes[filter.name.StartsWith("SnowCap",StringComparison.Ordinal)?"SnowCap":"TreeWood"];if(filter.sharedMesh!=mesh){filter.sharedMesh=mesh;PrefabUtility.RecordPrefabInstancePropertyModifications(filter);}}
   }
   AssetDatabase.SaveAssets();SceneValidation.Validate(scene);EditorSceneManager.MarkSceneDirty(scene);Require(EditorSceneManager.SaveScene(scene),"scene save failed");scene=EditorSceneManager.OpenScene(SceneValidation.ScenePath);SceneValidation.Validate(scene);
   Require(All<CatalogPlacement>(scene).Count(IsTree)==count,"forest count changed");foreach(var p in All<CatalogPlacement>(scene).Where(p=>!string.IsNullOrEmpty(p.placementId)))Require(snapshots[p.placementId]==PhysicalState(p),"placement/collision/culling changed "+p.placementId);
   foreach(var pair in protectedAssets)Require(AssetDatabase.GetAssetDependencyHash(pair.Key)==pair.Value,"Terrain/snow asset changed "+pair.Key);
   foreach(var p in All<CatalogPlacement>(scene).Where(IsTree)){
    Require(PrefabUtility.IsPartOfPrefabInstance(p),"disconnected tree instance");Require(p.GetComponentsInChildren<LODGroup>(true).Length==0,"LOD switching was added");foreach(var mesh in p.GetComponentsInChildren<MeshFilter>(true))Require(AssetDatabase.GetAssetPath(mesh.sharedMesh).StartsWith(Source,StringComparison.Ordinal),"higher-detail scene mesh remains");
   }
   Require(!All<MeshFilter>(scene).Any(m=>m.sharedMesh&&AssetDatabase.GetAssetPath(m.sharedMesh).StartsWith("Assets/Snowglow/Catalog/Sources/AlpineTrees/",StringComparison.Ordinal)),"old forest mesh remains elsewhere in scene");
   var report=new StringBuilder("PASS: Unity "+Application.unityVersion+". All "+count+" existing tree/wood instances now reference supplied lowest LOD3 only. Forest triangles "+before+" -> "+ForestTriangles(scene)+"; "+All<CatalogPlacement>(scene).Where(IsTree).Sum(p=>p.GetComponentsInChildren<Renderer>(true).Length)+" renderers. No LODGroups, higher-detail scene meshes or LOD0/1/2 imports.\n");
   foreach(var row in data.trees){var a=CatalogPreparation.Library().Find("alpine-tree-"+row.index);Require(a&&a.triangles==row.triangles,"saved catalog metrics differ");report.AppendLine(a.id+" | "+a.displayName+" | "+a.triangles+" triangles | "+All<CatalogPlacement>(scene).Count(p=>p.assetId==a.id)+" placements | Needs Review");}
   report.AppendLine("Existing catalog IDs, prefab GUIDs, scene placement identities/transforms, collision proxy shapes and culling distances retained. Materials and detachable snow groups retained. Existing Terrain/snow seed/mesh hashes unchanged. Scene validation passed. Source Blender and original geometry preserved, no source execution, district rebake, snow-mask mutation or player-save writes. Native prefab/forest previews rendered. Live traversal and Android/device timing untested.");
   Preview(scene);EditorSceneManager.OpenScene(SceneValidation.ScenePath);File.WriteAllText(Logs+"/Validation.txt",report.ToString());Debug.Log(report.ToString());
  }
  static void Preview(Scene scene){
   var library=CatalogPreparation.Library();foreach(int index in new[]{1,9,14,21,25,26}){
    var a=library.Find("alpine-tree-"+index.ToString("D2"));var preview=new PreviewRenderUtility();try{var obj=UnityEngine.Object.Instantiate(a.prefab);preview.AddSingleGO(obj);var b=CatalogValidation.BoundsOf(obj);preview.camera.orthographic=true;preview.camera.aspect=1;preview.camera.orthographicSize=Mathf.Max(b.extents.y,Mathf.Max(b.extents.x,b.extents.z))*1.45f;preview.camera.nearClipPlane=.05f;preview.camera.farClipPlane=150;preview.camera.transform.position=b.center+new Vector3(-1,.5f,-1)*b.size.magnitude*2;preview.camera.transform.LookAt(b.center);preview.camera.clearFlags=CameraClearFlags.SolidColor;preview.camera.backgroundColor=new Color(.25f,.38f,.45f);preview.ambientColor=new Color(.5f,.5f,.5f);preview.lights[0].intensity=1;preview.lights[0].transform.rotation=preview.camera.transform.rotation;preview.BeginStaticPreview(new Rect(0,0,600,600));preview.Render(true);var image=preview.EndStaticPreview();File.WriteAllBytes(Logs+"/"+index.ToString("D2")+"-LOD3.png",image.EncodeToPNG());UnityEngine.Object.DestroyImmediate(image);}finally{preview.Cleanup();}
   }
   var tree=All<CatalogPlacement>(scene).Where(IsTree).OrderBy(p=>(p.transform.position-new Vector3(-140,0,-140)).sqrMagnitude).First();var camera=All<Camera>(scene).First();camera.orthographic=false;camera.fieldOfView=60;camera.aspect=16f/9f;camera.transform.position=tree.transform.position+new Vector3(-20,12,-24);camera.transform.LookAt(tree.transform.position+Vector3.up*3);var rt=new RenderTexture(1280,720,24);var previous=RenderTexture.active;Texture2D frame=null;
   try{camera.targetTexture=rt;camera.Render();RenderTexture.active=rt;frame=new Texture2D(1280,720,TextureFormat.RGB24,false);frame.ReadPixels(new Rect(0,0,1280,720),0,0);frame.Apply();File.WriteAllBytes(Logs+"/Forest.png",frame.EncodeToPNG());}finally{camera.targetTexture=null;RenderTexture.active=previous;rt.Release();UnityEngine.Object.DestroyImmediate(rt);if(frame)UnityEngine.Object.DestroyImmediate(frame);}
  }
 }
}
