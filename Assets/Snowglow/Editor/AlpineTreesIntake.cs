using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Snowglow.Editor {
 public static class AlpineTreesIntake {
  const string Source=CatalogPreparation.Root+"/Sources/AlpineTrees/",Request="Logs/ImportAlpineTrees.request",Report="Logs/AlpineTrees/UnityIntake.txt";
  const string Hash="C8FA426AEAB4D38B8FFBCD378212D1376E139CA831F8437676D6F3720FBFE06F";
  [Serializable]public class Export {public Tree[] trees;}
  [Serializable]public class Point {public float[] xyz;}
  [Serializable]public class Tree {public string index,name,model;public int triangles;public float[] size;public bool snow,clutter;public float trunkRadius;public Point[] basePoints;[NonSerialized]public Vector3[] feet;}
  static T[] All<T>(Scene s) where T:Component=>s.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<T>(true)).ToArray();
  static CatalogPlacement[] Placed(Scene s)=>All<CatalogPlacement>(s).Where(p=>(p.placementId??"").StartsWith("pinefall-tree-",StringComparison.Ordinal)).ToArray();
  static void Require(bool ok,string message){if(!ok)throw new InvalidOperationException("Alpine trees: "+message);}
  static string Pose(Transform t)=>t.localToWorldMatrix.ToString("R");
  static float Ground(Terrain t,Vector3 p)=>t.SampleHeight(p)+t.transform.position.y;
  static Vector3 V(float[] a)=>new Vector3(a[0],a[1],a[2]);
  [InitializeOnLoadMethod]static void Reload(){EditorApplication.update+=Pending;}
  static void Pending(){
   if(!File.Exists(Request)){EditorApplication.update-=Pending;return;}
   if(EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode)return;
   EditorApplication.update-=Pending;File.Delete(Request);try{Install();}catch(Exception e){Directory.CreateDirectory("Logs/AlpineTrees");File.WriteAllText(Report,"FAIL: "+e);Debug.LogException(e);}
  }
  static Material Bark(){
   var m=AssetDatabase.LoadAssetAtPath<Material>(Source+"TreePalette.mat");if(!m){m=new Material(Shader.Find("Universal Render Pipeline/Simple Lit"));AssetDatabase.CreateAsset(m,Source+"TreePalette.mat");}
   m.name="Alpine trees | opaque bark needles and cut wood";m.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>(Source+"Palette.png"));m.SetColor("_BaseColor",Color.white);m.SetColor("_SpecColor",Color.black);m.SetFloat("_Smoothness",0);m.SetFloat("_Surface",0);m.SetFloat("_ZWrite",1);m.SetFloat("_Cull",2);m.renderQueue=2000;m.enableInstancing=true;EditorUtility.SetDirty(m);return m;
  }
  static Vector3[] Feet(Tree tree){
   if(tree.feet!=null)return tree.feet;var model=AssetDatabase.LoadAssetAtPath<GameObject>(Source+tree.model+".fbx");Require(model,"missing FBX");var frames=model.GetComponentsInChildren<Transform>(true);
   Vector3 Basis(string name)=>model.transform.InverseTransformPoint(frames.Single(t=>t.name==name).position);
   var bx=Basis("BasisX");var by=Basis("BasisY");var bz=Basis("BasisZ");Require(Mathf.Abs(bx.x)>.99f&&Mathf.Abs(by.z)>.99f&&Mathf.Abs(bz.y)>.99f,"FBX basis differs");
   tree.feet=tree.basePoints.Select(p=>bx*p.xyz[0]+by*p.xyz[1]+bz*p.xyz[2]).ToArray();Require(tree.feet.Length>0,"seating probes missing");return tree.feet;
  }
  static CatalogAsset Prepare(Tree tree,Material snow){
   string id="alpine-tree-"+tree.index;var a=CatalogPreparation.Library().Find(id);if(a&&a.tags.Contains("alpine-tree")){CatalogPreparation.RecordMetrics(a);ValidateEntry(a,tree);return a;}
   string path=Source+tree.model+".fbx";var importer=AssetImporter.GetAtPath(path) as ModelImporter;Require(importer!=null,"model not imported: "+path);
   importer.importCameras=false;importer.importLights=false;importer.importAnimation=false;importer.globalScale=1;importer.isReadable=false;importer.materialImportMode=ModelImporterMaterialImportMode.ImportStandard;
   importer.AddRemap(new AssetImporter.SourceAssetIdentifier(typeof(Material),"AlpineTreePalette"),Bark());importer.AddRemap(new AssetImporter.SourceAssetIdentifier(typeof(Material),"AlpineTreeSnow"),snow);importer.SaveAndReimport();
   var model=AssetDatabase.LoadAssetAtPath<GameObject>(path);Require((CatalogValidation.BoundsOf(model).size-V(tree.size)).magnitude<.03f,"metres/axes mismatch: "+id);Feet(tree);
   if(!a){var draft=CatalogPreparation.Draft(tree.name,CatalogKind.Landscape,AssetSource.ImportedModel,null,"User supplied Alpine_Trees_26_Variants.blend; collection "+tree.index+" "+tree.name+"; SHA256 "+Hash+". Embedded text was not executed.");draft.id=id;draft.creationSourcePath="SourceAssets/AlpineTrees/Alpine_Trees_26_Variants.blend";a=CatalogPreparation.Prepare(model,CatalogKind.Landscape,tree.name,1,Vector3.zero,AssetSource.ImportedModel,draft.provenance,draft);}
   var root=PrefabUtility.LoadPrefabContents(AssetDatabase.GetAssetPath(a.prefab));try{
    var visual=root.transform.Find("VisualRoot");visual.localPosition=Vector3.zero;var raw=visual.GetChild(0);if(PrefabUtility.IsPartOfPrefabInstance(raw))PrefabUtility.UnpackPrefabInstance(PrefabUtility.GetOutermostPrefabInstanceRoot(raw.gameObject),PrefabUnpackMode.Completely,InteractionMode.AutomatedAction);
    foreach(var c in root.GetComponentsInChildren<Collider>(true))UnityEngine.Object.DestroyImmediate(c.gameObject);
    foreach(var r in root.GetComponentsInChildren<MeshRenderer>(true)){bool cap=r.name=="SnowCap";r.sharedMaterial=cap?snow:Bark();if(cap)r.name="SnowCap | removable decorative snow";}
    foreach(var frame in root.GetComponentsInChildren<Transform>(true).Where(t=>t.name.StartsWith("Basis",StringComparison.Ordinal)).ToArray())UnityEngine.Object.DestroyImmediate(frame.gameObject);
    var proxy=new GameObject(tree.clutter?"Collision | stump or log envelope":"Collision | trunk only");proxy.transform.SetParent(root.transform,false);
    if(tree.clutter){var b=CatalogValidation.BoundsOf(root);var box=proxy.AddComponent<BoxCollider>();box.center=b.center;box.size=b.size;}
    else{float radius=Mathf.Clamp(tree.trunkRadius*.55f,.1f,.35f);var capsule=proxy.AddComponent<CapsuleCollider>();capsule.radius=radius;capsule.height=Mathf.Max(radius*2,tree.size[1]*.65f);capsule.center=Vector3.up*capsule.height*.5f;capsule.direction=1;}
    var cull=root.AddComponent<DistanceVisibility>();cull.distance=230;PrefabUtility.SaveAsPrefabAsset(root,AssetDatabase.GetAssetPath(a.prefab));
   }finally{PrefabUtility.UnloadPrefabContents(root);}
   int index=int.Parse(tree.index);a.tags=new[]{"alpine-tree",tree.snow?"removable-snow":"snow-free",tree.clutter?"forest-floor":index>=17?"deadwood":"evergreen",tree.model.ToLowerInvariant()};a.minimumScale=.75f;a.maximumScale=1.6f;a.status=PreparationStatus.NeedsReview;a.visualReviewed=false;
   a.assumptions="Metres, Y-up, original horizontal mounting origin and normalized ground floor. Source Tree or Wood rigid assembly stays one opaque palette mesh; snowy variants have independently removable decorative SnowCap. Snow-free source variants are kept snow-free. Two shared materials across the set; no transparent foliage cards, wind animation, runtime assembly or plowable decorative snow. Upright trees have a narrow trunk capsule, stumps/logs have a compact box; collision shapes are review proposals. Existing 230 m DistanceVisibility is retained. Placement follows Terrain at the base; stumps/logs align with local terrain normal without deforming shared meshes.";
   a.reviewNotes="Authorized prototype replacement. Needs visual collision/terrain review and A07 profiling before Ready publication.";CatalogPreparation.RecordMetrics(a);ValidateEntry(a,tree);EditorUtility.SetDirty(a);return a;
  }
  static void ValidateEntry(CatalogAsset a,Tree tree){
   var check=CatalogValidation.Check(a);Require(check.Ok,"catalog validation "+a.id+": "+check);Require(a.triangles==tree.triangles,"triangle mismatch "+a.id+": "+a.triangles+" vs "+tree.triangles);Require(a.renderers==(tree.snow?2:1)&&a.materialSlots==a.renderers,"mesh groups differ "+a.id);Require(a.prefab.GetComponentsInChildren<Camera>(true).Length==0&&a.prefab.GetComponentsInChildren<Light>(true).Length==0,"presentation object leaked");Require(a.prefab.GetComponent<DistanceVisibility>()&&a.prefab.GetComponent<DistanceVisibility>().distance==230,"culling missing");Require(a.prefab.GetComponentsInChildren<Transform>(true).Count(t=>t.name=="SnowCap | removable decorative snow")== (tree.snow?1:0),"snow separation differs");Require(a.prefab.GetComponentsInChildren<Collider>(true).Length==1,"single proxy required");
  }
  static float Seat(Terrain terrain,Tree tree,Vector3 position,Quaternion rotation,float scale){
   float height=float.PositiveInfinity;foreach(var foot in Feet(tree)){var relative=rotation*(foot*scale);var at=position+relative;float ground=Ground(terrain,at);height=Mathf.Min(height,ground-relative.y);}return height-.08f;
  }
  static void Place(Scene scene,Tree[] trees,CatalogAsset[] entries){
   var original=All<Transform>(scene).Where(t=>t.name=="Snow tree | cylinder + spheres").OrderBy(t=>t.position.x).ThenBy(t=>t.position.z).ToArray();if(original.Length==0){ValidatePlacements(scene,trees,entries);return;}
   Require(Placed(scene).Length==0&&original.Length>=26,"partial tree replacement requires review");var terrain=All<Terrain>(scene).Single();var random=new System.Random(301026);
   var bag=Enumerable.Range(0,26).ToList();var weighted=Enumerable.Range(8,8).Concat(Enumerable.Range(8,8)).Concat(Enumerable.Range(0,20)).ToArray();while(bag.Count<original.Length)bag.Add(weighted[random.Next(weighted.Length)]);
   for(int i=bag.Count-1;i>0;i--){int j=random.Next(i+1);int swap=bag[i];bag[i]=bag[j];bag[j]=swap;}
   var group=new GameObject("Alpine forest | prepared trees and deadwood").transform;group.SetParent(original[0].parent,false);
   for(int i=0;i<original.Length;i++){
    var old=original[i];var tree=trees[bag[i]];float scale=old.lossyScale.x;Require(scale>=entries[bag[i]].minimumScale&&scale<=entries[bag[i]].maximumScale,"original tree scale outside range");var position=old.position;float yaw=(float)random.NextDouble()*360;
    var normal=terrain.terrainData.GetInterpolatedNormal((position.x-terrain.transform.position.x)/terrain.terrainData.size.x,(position.z-terrain.transform.position.z)/terrain.terrainData.size.z);var rotation=(tree.clutter?Quaternion.FromToRotation(Vector3.up,normal):Quaternion.identity)*Quaternion.Euler(0,yaw,0);position.y=Seat(terrain,tree,position,rotation,scale);
    var go=(GameObject)PrefabUtility.InstantiatePrefab(entries[bag[i]].prefab,scene);go.transform.SetParent(group,true);go.transform.SetPositionAndRotation(position,rotation);go.transform.localScale=Vector3.one*scale;go.GetComponent<CatalogPlacement>().placementId="pinefall-tree-"+(i+1).ToString("D3");UnityEngine.Object.DestroyImmediate(old.gameObject);
   }
   ValidatePlacements(scene,trees,entries);
  }
  static Vector3[] Vertices(Mesh mesh){using(var data=MeshUtility.AcquireReadOnlyMeshData(mesh))using(var values=new NativeArray<Vector3>(data[0].vertexCount,Allocator.Temp)){data[0].GetVertices(values);return values.ToArray();}}
  static void ValidatePlacements(Scene scene,Tree[] trees,CatalogAsset[] entries){
   var placed=Placed(scene);Require(placed.Length>=26&&placed.Select(p=>p.placementId).Distinct().Count()==placed.Length,"saved unique tree IDs missing");Require(entries.All(a=>placed.Any(p=>p.assetId==a.id)),"every tree variant must be used");Require(!All<Transform>(scene).Any(t=>t.name=="Snow tree | cylinder + spheres"),"primitive tree remains");var terrain=All<Terrain>(scene).Single();var cache=new Dictionary<Mesh,Vector3[]>();
   foreach(var p in placed){
    var tree=trees.Single(t=>p.assetId=="alpine-tree-"+t.index);Require(PrefabUtility.IsPartOfPrefabInstance(p)&&p.GetComponent<DistanceVisibility>(),"tree lost prefab/culling");float largestFoot=float.NegativeInfinity;
    foreach(var foot in Feet(tree)){var at=p.transform.TransformPoint(foot);largestFoot=Mathf.Max(largestFoot,at.y-Ground(terrain,at));}Require(largestFoot<-.05f,"floating foot probe: "+p.placementId);
    float relief=float.NegativeInfinity;foreach(var m in p.GetComponentsInChildren<MeshFilter>()){Require(AssetDatabase.Contains(m.sharedMesh),"tree mesh is not persistent");if(!cache.TryGetValue(m.sharedMesh,out var vertices)){vertices=Vertices(m.sharedMesh);cache[m.sharedMesh]=vertices;}foreach(var v in vertices){var at=m.transform.TransformPoint(v);relief=Mathf.Max(relief,at.y-Ground(terrain,at));}}
    Require(relief>tree.size[1]*p.transform.lossyScale.y*(tree.clutter?.3f:.65f),"tree buried too deeply: "+p.placementId+" relief "+relief);
   }
  }
  public static void PlacePreparedTrees(Scene scene){
   if(!File.Exists(Source+"ExportMetrics.json"))return;var data=JsonUtility.FromJson<Export>(File.ReadAllText(Source+"ExportMetrics.json"));var entries=data.trees.Select(t=>CatalogPreparation.Library().Find("alpine-tree-"+t.index)).ToArray();if(entries.Length==26&&entries.All(a=>a&&a.prefab))Place(scene,data.trees,entries);
  }
  static string SnowData(SnowSeed seed)=>JsonUtility.ToJson(new SnowVolumeGrid{depth=seed.grid.depth,credit=seed.grid.credit,initialVolume=seed.grid.initialVolume,disposed=seed.grid.disposed,revision=seed.grid.revision,blocked=new bool[0]});
  static int RefreshNearbyObstacles(Scene scene,SnowField snow){
   Physics.SyncTransforms();var changed=new Dictionary<int,bool>();var candidates=new HashSet<int>();float start=SnowVolumeGrid.Origin,cell=SnowVolumeGrid.Cell,end=start+SnowVolumeGrid.Size*cell;
   foreach(var collider in Placed(scene).SelectMany(p=>p.GetComponentsInChildren<Collider>())){
    var b=collider.bounds;if(b.max.x<start||b.min.x>end||b.max.z<start||b.min.z>end)continue;
    int x0=Mathf.Clamp(Mathf.FloorToInt((b.min.x-start)/cell)-1,0,SnowVolumeGrid.Size-1),x1=Mathf.Clamp(Mathf.CeilToInt((b.max.x-start)/cell)+1,0,SnowVolumeGrid.Size-1),z0=Mathf.Clamp(Mathf.FloorToInt((b.min.z-start)/cell)-1,0,SnowVolumeGrid.Size-1),z1=Mathf.Clamp(Mathf.CeilToInt((b.max.z-start)/cell)+1,0,SnowVolumeGrid.Size-1);
    for(int z=z0;z<=z1;z++)for(int x=x0;x<=x1;x++)candidates.Add(z*SnowVolumeGrid.Size+x);
   }
   foreach(int i in candidates){bool blocked=Physics.CheckBox(new Vector3(snow.seed.grid.X(i),SnowField.Ground+.85f,snow.seed.grid.Z(i)),new Vector3(cell*.45f,.65f,cell*.45f),Quaternion.identity,1<<0,QueryTriggerInteraction.Ignore);if(blocked!=snow.seed.grid.blocked[i])changed[i]=blocked;}
   if(changed.Count==0)return 0;const string path=CatalogPreparation.Root+"/SceneData/AlpineTreesSnowSeed.asset";var seed=AssetDatabase.LoadAssetAtPath<SnowSeed>(path);if(!seed){seed=UnityEngine.Object.Instantiate(snow.seed);seed.name="Pinefall | tree trunk obstacle mask";AssetDatabase.CreateAsset(seed,path);}Require(SnowData(seed)==SnowData(snow.seed),"scene copy snow data differs");seed.grid.blocked=(bool[])snow.seed.grid.blocked.Clone();foreach(var item in changed)seed.grid.blocked[item.Key]=item.Value;snow.seed=seed;EditorUtility.SetDirty(seed);return changed.Count;
  }
  static void Preview(CatalogAsset a,string index){
   var p=new PreviewRenderUtility();try{var obj=UnityEngine.Object.Instantiate(a.prefab);p.AddSingleGO(obj);var b=CatalogValidation.BoundsOf(obj);p.camera.orthographic=true;p.camera.orthographicSize=Mathf.Max(b.extents.y,Mathf.Max(b.extents.x,b.extents.z)) *1.3f;p.camera.nearClipPlane=.05f;p.camera.farClipPlane=100;p.camera.transform.position=b.center+new Vector3(-1,.45f,-1)*Mathf.Max(2,b.size.magnitude)*2;p.camera.transform.LookAt(b.center);
    float width=0,height=0;for(int corner=0;corner<8;corner++){var local=p.camera.transform.InverseTransformPoint(b.center+Vector3.Scale(b.extents,new Vector3((corner&1)==0?-1:1,(corner&2)==0?-1:1,(corner&4)==0?-1:1)));width=Mathf.Max(width,Mathf.Abs(local.x));height=Mathf.Max(height,Mathf.Abs(local.y));}
    p.camera.aspect=480f/600;p.camera.orthographicSize=Mathf.Max(height,width/p.camera.aspect)*1.12f;
    p.camera.clearFlags=CameraClearFlags.SolidColor;p.camera.backgroundColor=new Color(.3f,.43f,.52f);p.ambientColor=new Color(.4f,.4f,.4f);p.lights[0].intensity=1.1f;p.lights[0].transform.rotation=p.camera.transform.rotation;p.lights[1].intensity=.4f;p.lights[1].transform.rotation=Quaternion.Euler(20,150,0);p.BeginStaticPreview(new Rect(0,0,480,600));p.Render(true);var image=p.EndStaticPreview();File.WriteAllBytes("Logs/AlpineTrees/"+index+"-unity.png",image.EncodeToPNG());UnityEngine.Object.DestroyImmediate(image);}finally{p.Cleanup();}
  }
  [MenuItem("Snowglow/Asset intake/Import twenty six alpine trees")]
  public static void Install(){
   var activeTree=CatalogPreparation.Library().Find("alpine-tree-01");if(activeTree&&activeTree.tags.Contains("lod3-only")){LowestTreeLODIntake.Install();return;}
   Require(!EditorApplication.isPlayingOrWillChangePlaymode,"stop Play first");var scene=SceneManager.GetActiveScene();Require(scene.path==SceneValidation.ScenePath&&!scene.isDirty,"open saved Pinefall first");Directory.CreateDirectory("Logs/AlpineTrees");if(!File.Exists("Logs/AlpineTrees/Pinefall-before.unity"))File.Copy(scene.path,"Logs/AlpineTrees/Pinefall-before.unity");
   var session=All<PrototypeSession>(scene).Single();var dataBefore=SnowData(session.Snow.seed);var fleet=session.Fleet.Select(m=>m.GetInstanceID()).ToArray();var snapshots=All<CatalogPlacement>(scene).Where(p=>!string.IsNullOrEmpty(p.placementId)).ToDictionary(p=>p.placementId,p=>p.assetId+"|"+Pose(p.transform));var protectedPaths=All<Terrain>(scene).Select(t=>AssetDatabase.GetAssetPath(t.terrainData)).Concat(session.Snow.GetComponentsInChildren<MeshFilter>().Select(m=>AssetDatabase.GetAssetPath(m.sharedMesh))).Concat(new[]{AssetDatabase.GetAssetPath(session.Snow.seed)}).Where(p=>!string.IsNullOrEmpty(p)&&!p.EndsWith("/AlpineTreesSnowSeed.asset",StringComparison.Ordinal)).Distinct().ToDictionary(p=>p,p=>AssetDatabase.GetAssetDependencyHash(p).ToString());
   var originals=All<Transform>(scene).Where(t=>t.name=="Snow tree | cylinder + spheres").OrderBy(t=>t.position.x).ThenBy(t=>t.position.z).ToArray();var originalSites=originals.Select(t=>new Vector3(t.position.x,t.lossyScale.x,t.position.z)).ToArray();int oldTriangles=originals.Sum(t=>t.GetComponentsInChildren<MeshFilter>().Sum(m=>m.sharedMesh? (int)m.sharedMesh.GetIndexCount(0)/3:0)),oldRenderers=originals.Sum(t=>t.GetComponentsInChildren<Renderer>().Length);
   var texture=AssetImporter.GetAtPath(Source+"Palette.png") as TextureImporter;Require(texture!=null,"palette missing");texture.filterMode=FilterMode.Point;texture.mipmapEnabled=false;texture.wrapMode=TextureWrapMode.Clamp;texture.textureCompression=TextureImporterCompression.Uncompressed;texture.sRGBTexture=true;texture.SaveAndReimport();var data=JsonUtility.FromJson<Export>(File.ReadAllText(Source+"ExportMetrics.json"));Require(data.trees.Length==26,"26 source variants required");var entries=data.trees.Select(t=>Prepare(t,session.Snow.palette.snow)).ToArray();AssetDatabase.SaveAssets();
   int changed;
   try{
    Place(scene,data.trees,entries);var placed=Placed(scene);Require(originalSites.Length==0||originalSites.Length==placed.Length,"tree count changed");foreach(var site in originalSites)Require(placed.Count(p=>Mathf.Abs(p.transform.position.x-site.x)<.001f&&Mathf.Abs(p.transform.position.z-site.z)<.001f&&Mathf.Abs(p.transform.lossyScale.x-site.y)<.001f)==1,"original tree location/scale missing");
    changed=RefreshNearbyObstacles(scene,session.Snow);Require(fleet.SequenceEqual(session.Fleet.Select(m=>m.GetInstanceID())),"fleet changed");SceneValidation.Validate(scene);
   }catch{EditorSceneManager.OpenScene(SceneValidation.ScenePath);throw;} // Only our unsaved placement pass is discarded.
   EditorSceneManager.MarkSceneDirty(scene);AssetDatabase.SaveAssets();Require(EditorSceneManager.SaveScene(scene),"save failed");scene=EditorSceneManager.OpenScene(SceneValidation.ScenePath);entries=data.trees.Select(t=>CatalogPreparation.Library().Find("alpine-tree-"+t.index)).ToArray();ValidatePlacements(scene,data.trees,entries);SceneValidation.Validate(scene);MechanicsChecks.Run();Require(dataBefore==SnowData(All<PrototypeSession>(scene).Single().Snow.seed),"snow data changed");
   var current=All<CatalogPlacement>(scene).Where(p=>!string.IsNullOrEmpty(p.placementId)).ToDictionary(p=>p.placementId,p=>p.assetId+"|"+Pose(p.transform));foreach(var item in snapshots)Require(current.TryGetValue(item.Key,out var state)&&state==item.Value,"existing placement moved: "+item.Key);foreach(var item in protectedPaths)Require(item.Value==AssetDatabase.GetAssetDependencyHash(item.Key).ToString(),"protected terrain/snow asset changed");
   var report=new StringBuilder("PASS: Unity "+Application.unityVersion+" / "+EditorUserBuildSettings.activeBuildTarget+". All 26 tree/wood variants prepared; "+Placed(scene).Length+" primitive tree locations replaced, saved and reopened.\n");
   foreach(var tree in data.trees){var a=entries.Single(e=>e.id=="alpine-tree-"+tree.index);ValidateEntry(a,tree);report.AppendLine(tree.index+" "+a.displayName+": "+a.triangles+" triangles / "+a.prefab.GetComponentsInChildren<MeshFilter>(true).Sum(m=>m.sharedMesh.vertexCount)+" imported vertices / "+a.renderers+" renderers; "+Placed(scene).Count(p=>p.assetId==a.id)+" placements; Needs Review.");Preview(a,tree.index);}
   report.AppendLine("Placed geometry: "+Placed(scene).Sum(p=>entries.Single(a=>a.id==p.assetId).triangles)+" triangles / "+Placed(scene).Sum(p=>entries.Single(a=>a.id==p.assetId).renderers)+" renderer/material slots. Two shared opaque materials across the set. Original primitive inventory this run: "+oldTriangles+" triangles / "+oldRenderers+" renderers (zero on repeat). No runtime assembly or added transparent foliage.");
   report.AppendLine("All variants used. Original horizontal positions and uniform scales preserved; deterministic yaw/mix, winter evergreens weighted, every deadwood type represented. Foot probes buried at least 5 cm; vertex relief checks keep tree/wood geometry above Terrain. Stumps/logs align with terrain normal; shared meshes remain unchanged. Existing 230 m distance culling retained. Snow obstacle cells changed: "+changed+"; all volume/credit/accounting unchanged. Homes, mountains, lights, fleet, Terrain and saved snow meshes preserved. Scene structural and 19 snow-ledger checks passed. Twenty-six native previews rendered. Full forest/collision walkthrough, Android rendering/build and Galaxy A07 performance remain untested.");
   File.WriteAllText(Report,report.ToString());Debug.Log(report.ToString());if(SceneView.lastActiveSceneView)SceneView.lastActiveSceneView.LookAt(Placed(scene).First(p=>p.assetId=="alpine-tree-09").transform.position+Vector3.up*4,Quaternion.Euler(20,135,0),13,true,true);
  }
 }
}
