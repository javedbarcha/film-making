using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Snowglow.Editor {
 public static class AngularCloudsIntake {
  const string Source=CatalogPreparation.Root+"/Sources/AngularClouds/",Logs="Logs/AngularClouds",Request="Logs/ImportAngularClouds.request";
  const string Hash="8AE32B6DE44F38348DB0D38BD745FA0C274D657D6E0E7B7D0A6D3318BEE681E9";
  [Serializable]public class Export{public Cloud[] clouds;}
  [Serializable]public class Cloud{public string index,name,model;public int triangles;public float[] size;}
  static T[] All<T>(Scene s)where T:Component=>s.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<T>(true)).ToArray();
  static bool IsCloud(CatalogPlacement p)=>(p.placementId??"").StartsWith("pinefall-cloud-");
  static void Require(bool value,string message){if(!value)throw new InvalidOperationException("Cloud intake: "+message);}
  [InitializeOnLoadMethod]static void Register(){EditorApplication.update+=Pending;}
  static void Pending(){
   if(!File.Exists(Request)){EditorApplication.update-=Pending;return;}
   if(EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode)return;
   EditorApplication.update-=Pending;File.Delete(Request);try{Install();}catch(Exception e){Directory.CreateDirectory(Logs);File.WriteAllText(Logs+"/UnityIntake.txt","FAIL: "+e);Debug.LogException(e);}
  }
  static Material Material(){
   AssetDatabase.ImportAsset(Source+"Palette.png",ImportAssetOptions.ForceSynchronousImport);
   var importer=(TextureImporter)AssetImporter.GetAtPath(Source+"Palette.png");importer.textureType=TextureImporterType.Default;importer.sRGBTexture=true;importer.mipmapEnabled=false;importer.filterMode=FilterMode.Point;importer.wrapMode=TextureWrapMode.Clamp;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.npotScale=TextureImporterNPOTScale.None;importer.SaveAndReimport();
   var m=AssetDatabase.LoadAssetAtPath<Material>(Source+"CloudPalette.mat");if(!m){m=new Material(Shader.Find("Universal Render Pipeline/Unlit"));AssetDatabase.CreateAsset(m,Source+"CloudPalette.mat");}
   m.name="Angular clouds | opaque white ice and underside palette";m.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>(Source+"Palette.png"));m.SetColor("_BaseColor",Color.white);m.SetFloat("_Surface",0);m.SetFloat("_ZWrite",1);m.SetFloat("_Cull",2);m.renderQueue=2000;m.enableInstancing=true;EditorUtility.SetDirty(m);return m;
  }
  static CatalogAsset Prepare(Cloud c,Material material){
   string path=Source+c.model+".fbx",id="angular-cloud-"+c.index;
   AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);var importer=(ModelImporter)AssetImporter.GetAtPath(path);
   importer.importCameras=false;importer.importLights=false;importer.importAnimation=false;importer.globalScale=1;importer.isReadable=false;importer.materialImportMode=ModelImporterMaterialImportMode.ImportStandard;
   importer.AddRemap(new AssetImporter.SourceAssetIdentifier(typeof(Material),"AngularCloudPalette"),material);importer.SaveAndReimport();
   var model=AssetDatabase.LoadAssetAtPath<GameObject>(path);var bounds=CatalogValidation.BoundsOf(model);Require((bounds.size-new Vector3(c.size[0],c.size[1],c.size[2])).magnitude<.03f,"metres or axis mismatch "+id);
   var a=CatalogPreparation.Library().Find(id);
   if(!a){var draft=CatalogPreparation.Draft(c.name,CatalogKind.Landscape,AssetSource.ImportedModel,null,"User supplied Eight_Angular_Low_Poly_Clouds.blend; object "+c.model+"; SHA256 "+Hash+". Blender autoexecution disabled. No embedded text exists.");draft.id=id;draft.creationSourcePath="SourceAssets/AngularClouds/Eight_Angular_Low_Poly_Clouds.blend";a=CatalogPreparation.Prepare(model,CatalogKind.Landscape,c.name,1,Vector3.zero,AssetSource.ImportedModel,draft.provenance,draft);}
   var root=PrefabUtility.LoadPrefabContents(AssetDatabase.GetAssetPath(a.prefab));
   try{foreach(var collider in root.GetComponentsInChildren<Collider>(true))UnityEngine.Object.DestroyImmediate(collider.gameObject);
    foreach(var r in root.GetComponentsInChildren<Renderer>(true)){r.sharedMaterial=material;r.shadowCastingMode=ShadowCastingMode.Off;r.receiveShadows=false;r.lightProbeUsage=LightProbeUsage.Off;r.reflectionProbeUsage=ReflectionProbeUsage.Off;}
    PrefabUtility.SaveAsPrefabAsset(root,AssetDatabase.GetAssetPath(a.prefab));
   }finally{PrefabUtility.UnloadPrefabContents(root);}
   a.blocksSnow=false;a.minimumScale=5;a.maximumScale=40;a.tags=new[]{"cloud","angular-cloud","sky","opaque","visual-only"};a.status=PreparationStatus.NeedsReview;a.visualReviewed=false;
   a.assumptions="Static Editor-authored sky decoration. One flat-shaded mesh and shared opaque unlit three-color palette. No colliders, Rigidbody, shadows, lights, wind or runtime spawning. Source proportions preserved with uniform placement scale. Day/night material response is future work.";
   a.reviewNotes="User-authorized sky scatter; needs final spherical sky/background composition review and A07 profiling before Ready publication.";CatalogPreparation.RecordMetrics(a);EditorUtility.SetDirty(a);
   Require(a.triangles==c.triangles&&a.renderers==1&&a.materialSlots==1,"geometry metrics differ "+id);Require(CatalogValidation.Check(a).Ok,"catalog validation "+id);return a;
  }
  static Bounds WorldBounds(CatalogAsset a,Vector3 position,Quaternion rotation,float scale){
   var b=a.localBounds;var result=new Bounds(position+rotation*(b.center*scale),Vector3.zero);
   for(int i=0;i<8;i++)result.Encapsulate(position+rotation*((b.center+Vector3.Scale(b.extents,new Vector3((i&1)==0?-1:1,(i&2)==0?-1:1,(i&4)==0?-1:1)))*scale));return result;
  }
  static void Scatter(Scene scene,CatalogAsset[] entries){
   var existing=All<CatalogPlacement>(scene).Where(IsCloud).ToArray();if(existing.Length>0){Require(existing.Length==40,"partial cloud scatter; preserve and inspect before adding");return;}
   var group=new GameObject("Sky clouds | 40 angular low-poly clouds");SceneManager.MoveGameObjectToScene(group,scene);
   var rng=new System.Random(311103);var variants=Enumerable.Range(0,40).Select(i=>i%8).ToArray();
   for(int i=variants.Length-1;i>0;i--){int j=rng.Next(i+1);int swap=variants[i];variants[i]=variants[j];variants[j]=swap;}
   var mountainBounds=All<CatalogPlacement>(scene).Where(p=>(p.assetId??"").StartsWith("alpine-mountain-")).SelectMany(p=>p.GetComponentsInChildren<Renderer>(true)).Select(r=>r.bounds).ToArray();
   var used=new System.Collections.Generic.List<Bounds>();
   for(int i=0;i<40;i++){
    var a=entries[variants[i]];Vector3 position=Vector3.zero;Quaternion rotation=Quaternion.identity;float scale=1;Bounds bounds=default;bool found=false;
    for(int attempt=0;attempt<500&&!found;attempt++){
     float angle=(float)((i+.1+rng.NextDouble()*.8)*Math.PI*2/40),radius=260+(float)rng.NextDouble()*360;
     position=new Vector3(Mathf.Cos(angle)*radius,195+(float)rng.NextDouble()*65,Mathf.Sin(angle)*radius);
     rotation=Quaternion.Euler(0,(float)rng.NextDouble()*360,0);scale=(45+(float)rng.NextDouble()*40)/a.localBounds.size.x;bounds=WorldBounds(a,position,rotation,scale);var clearance=bounds;clearance.Expand(20);
     found=!mountainBounds.Any(b=>clearance.Intersects(b))&&!used.Any(b=>clearance.Intersects(b));
    }
    Require(found,"could not place cloud without intersection "+i);used.Add(bounds);
    var go=(GameObject)PrefabUtility.InstantiatePrefab(a.prefab,scene);go.transform.SetParent(group.transform,false);go.transform.SetPositionAndRotation(position,rotation);go.transform.localScale=Vector3.one*scale;go.GetComponent<CatalogPlacement>().placementId="pinefall-cloud-"+(i+1).ToString("D3");
   }
  }
  [MenuItem("Snowglow/Asset intake/Import and scatter eight angular clouds")]
  public static void Install(){
   Require(!EditorApplication.isPlayingOrWillChangePlaymode,"stop Play before importing clouds");var scene=SceneManager.GetActiveScene();Require(scene.path==SceneValidation.ScenePath,"open Pinefall first");Directory.CreateDirectory(Logs);
   if(!File.Exists(Logs+"/Pinefall-before.unity"))File.Copy(scene.path,Logs+"/Pinefall-before.unity");
   if(scene.isDirty)Require(EditorSceneManager.SaveScene(scene,Logs+"/Pinefall-unsaved-before.unity",true),"backup unsaved work");
   var poses=All<CatalogPlacement>(scene).Where(p=>!IsCloud(p)&&!string.IsNullOrEmpty(p.placementId)).ToDictionary(p=>p.placementId,p=>p.assetId+"|"+p.transform.localToWorldMatrix.ToString("R"));
   var session=All<PrototypeSession>(scene).Single();
   var hashes=All<Terrain>(scene).Select(t=>AssetDatabase.GetAssetPath(t.terrainData)).Concat(All<MeshFilter>(scene).Select(m=>AssetDatabase.GetAssetPath(m.sharedMesh))).Concat(new[]{AssetDatabase.GetAssetPath(session.Snow.seed)}).Where(p=>p.StartsWith("Assets/",StringComparison.Ordinal)).Distinct().ToDictionary(p=>p,p=>AssetDatabase.GetAssetDependencyHash(p).ToString());
   var data=JsonUtility.FromJson<Export>(File.ReadAllText(Source+"ExportMetrics.json"));Require(data.clouds.Length==8,"expected eight variants");var material=Material();var entries=data.clouds.Select(c=>Prepare(c,material)).ToArray();Scatter(scene,entries);
   AssetDatabase.SaveAssets();SceneValidation.Validate(scene);EditorSceneManager.MarkSceneDirty(scene);Require(EditorSceneManager.SaveScene(scene),"scene save failed");scene=EditorSceneManager.OpenScene(SceneValidation.ScenePath);SceneValidation.Validate(scene);
   foreach(var p in All<CatalogPlacement>(scene).Where(p=>!IsCloud(p)&&!string.IsNullOrEmpty(p.placementId)))Require(poses[p.placementId]==p.assetId+"|"+p.transform.localToWorldMatrix.ToString("R"),"existing placement changed "+p.placementId);
   foreach(var h in hashes)Require(AssetDatabase.GetAssetDependencyHash(h.Key).ToString()==h.Value,"protected mesh/Terrain/snow changed "+h.Key);
   var clouds=All<CatalogPlacement>(scene).Where(IsCloud).ToArray();Require(clouds.Length==40&&clouds.Select(p=>p.placementId).Distinct().Count()==40,"cloud identities missing");
   foreach(var a in entries)Require(clouds.Count(p=>p.assetId==a.id)==5,"variant distribution "+a.id);
   foreach(var p in clouds){Require(p.GetComponentsInChildren<Collider>(true).Length==0&&p.GetComponentsInChildren<Rigidbody>(true).Length==0,"cloud physics not allowed");Require(PrefabUtility.IsPartOfPrefabInstance(p),"saved cloud is not a prefab instance");Require(p.GetComponentsInChildren<Renderer>(true).All(r=>r.sharedMaterial==material&&r.shadowCastingMode==ShadowCastingMode.Off),"cloud renderer settings lost");}
   var report=new StringBuilder("PASS: Unity "+Application.unityVersion+" / "+EditorUserBuildSettings.activeBuildTarget+". Eight cloud catalog prefabs imported, 40 sky instances saved/reopened (five each), 13,280 triangles / 40 renderers / one shared opaque material.\n");
   foreach(var a in entries)report.AppendLine(a.id+" | "+a.displayName+" | "+a.triangles+" triangles | "+a.localBounds.size+" metres at source scale | Needs Review");
   report.AppendLine("Seed 311103; jittered angular sectors, radii 260-620m, base altitudes 195-260m, uniform size fitting to 45-85m width and random yaw. Clearance checked against other clouds and current mountain bounds. Repeated intake preserves cloud placements. No colliders, shadows, lights, wind or runtime factories. Existing catalog placement identities/transforms, all existing mesh/Terrain/snow dependency hashes preserved, including manually edited mountains. Scene validation passed. No player saves touched. Device performance and live all-camera visual comfort remain untested.");
   Preview(scene);EditorSceneManager.OpenScene(SceneValidation.ScenePath);File.WriteAllText(Logs+"/UnityIntake.txt",report.ToString());Debug.Log(report.ToString());
  }
  static void Preview(Scene scene){
   var camera=All<Camera>(scene).First();camera.orthographic=false;camera.fieldOfView=60;camera.aspect=16f/9f;
   foreach(int yaw in new[]{0,90,180,270}){
    camera.transform.SetPositionAndRotation(new Vector3(0,12,0),Quaternion.Euler(-12,yaw,0));var rt=new RenderTexture(1280,720,24);var previous=RenderTexture.active;Texture2D image=null;
    try{camera.targetTexture=rt;camera.Render();RenderTexture.active=rt;image=new Texture2D(1280,720,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1280,720),0,0);image.Apply();File.WriteAllBytes(Logs+"/Horizon-"+yaw+".png",image.EncodeToPNG());}
    finally{camera.targetTexture=null;RenderTexture.active=previous;rt.Release();UnityEngine.Object.DestroyImmediate(rt);if(image)UnityEngine.Object.DestroyImmediate(image);}
   }
  }
 }
}
