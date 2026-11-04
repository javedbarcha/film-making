using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Snowglow.Editor {
 public static class LoaderModelIntake {
  const string Source=CatalogPreparation.Root+"/Sources/LowPolySnowLoader/";
  public const string PrefabPath=CatalogPreparation.Root+"/Prefabs/ImportedSnowLoader.prefab";
  const string Request="Logs/InstallImportedLoader.request";
  const string Report="Logs/LoaderIntake/UnityValidation.txt";
  static readonly Vector3 BoomOrigin=new Vector3(0,1.74f,.53f),BucketOrigin=new Vector3(0,1.03f,2.81f);
  static readonly Vector3 BucketEdge=new Vector3(0,-.463f,1.2f),BucketLoad=new Vector3(0,-.13f,.64f);
  const float WorkingBoomPitch=9;
  [InitializeOnLoadMethod]static void Reload(){EditorApplication.update+=Pending;}
  static void Pending(){
   if(!File.Exists(Request)){EditorApplication.update-=Pending;return;}
   if(EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode)return;
   EditorApplication.update-=Pending;File.Delete(Request);
   try{Install();}catch(Exception e){Directory.CreateDirectory("Logs/LoaderIntake");File.WriteAllText(Report,"FAIL: "+e);Debug.LogException(e);}
  }
  static Material Palette(bool glass){
   string path=Source+(glass?"Glass.mat":"OpaquePalette.mat");var mat=AssetDatabase.LoadAssetAtPath<Material>(path);if(mat)return mat;
   mat=new Material(Shader.Find("Universal Render Pipeline/Simple Lit")){name=glass?"Loader glass":"Loader opaque palette",enableInstancing=true};mat.SetFloat("_Smoothness",.12f);
   if(glass)ToonWindowsAuthoring.Configure(mat);
   else{mat.SetColor("_BaseColor",Color.white);mat.SetTexture("_BaseMap",AssetDatabase.LoadAssetAtPath<Texture2D>(Source+"Palette.png"));}
   AssetDatabase.CreateAsset(mat,path);return mat;
  }
  static void Edit(CatalogAsset a,Action<GameObject> edit){
   string path=AssetDatabase.GetAssetPath(a.prefab);var root=PrefabUtility.LoadPrefabContents(path);
   try{
    foreach(var r in root.GetComponentsInChildren<Renderer>(true)){var mats=r.sharedMaterials;for(int i=0;i<mats.Length;i++)if(mats[i].name.StartsWith("Loader",StringComparison.Ordinal))mats[i]=Palette(mats[i].name.IndexOf("glass",StringComparison.OrdinalIgnoreCase)>=0);r.sharedMaterials=mats;}
    edit(root);PrefabUtility.SaveAsPrefabAsset(root,path);
   }finally{PrefabUtility.UnloadPrefabContents(root);}
   CatalogPreparation.RecordMetrics(a);EditorUtility.SetDirty(a);
  }
  static CatalogAsset Prepare(string id,string name,string model,CatalogKind kind){
   var existing=CatalogPreparation.Library().Find(id);if(existing)return existing;
   var draft=CatalogPreparation.Draft(name,kind,AssetSource.ImportedModel,null,"User supplied snow_loader_lp.blend; SHA256 365804EDC406BD3D021C8156956C984815D4567F30ABAA58D2F29ED993467327. Embedded source text was not executed.");
   draft.id=id;draft.creationSourcePath="SourceAssets/LowPolySnowLoader/snow_loader_lp.blend";
   return CatalogPreparation.Prepare(AssetDatabase.LoadAssetAtPath<GameObject>(Source+model+".fbx"),kind,name,1,Vector3.zero,AssetSource.ImportedModel,draft.provenance,draft);
  }
  static GameObject Build(Material snow){
   var texture=(TextureImporter)AssetImporter.GetAtPath(Source+"Palette.png");texture.filterMode=FilterMode.Point;texture.mipmapEnabled=false;texture.wrapMode=TextureWrapMode.Clamp;texture.textureCompression=TextureImporterCompression.Uncompressed;texture.sRGBTexture=true;texture.SaveAndReimport();
   var raw=AssetDatabase.LoadAssetAtPath<GameObject>(Source+"LoaderBody.fbx");
   Require(raw&&Vector3.Distance(raw.GetComponentsInChildren<Transform>().Single(t=>t.name=="wheel_fl").position,new Vector3(-.91f,.73f,1.03f))<.01f,"metre scale / forward / wheel conversion");
   var body=Prepare("imported-loader-body","Golden loader | body, wheels and boom","LoaderBody",CatalogKind.Vehicle);
   var bucket=Prepare("imported-loader-bucket","Golden loader | wide snow bucket","LoaderBucket",CatalogKind.Attachment);
   var def=body.vehicle;def.loader=true;def.mass=4200;def.wheelRadius=.73f;def.track=1.82f;def.wheelbase=2.38f;def.torque=2900;def.maxSpeed=7;def.mountId="imported-loader-quick-A";def.maxAttachmentMass=1200;def.maxWorkingWidth=3.2f;def.dryCenterOfMass=new Vector3(0,.9f,-.5f);def.price=1400;
   body.tags=new[]{"loader","yellow","four-wheels","palette-atlas"};body.assumptions="Metres, Y-up/Z-forward. Wheel centres measured from source. 4200 kg, torque, COM and suspension are prototype values. Front-wheel steering retained; chassis articulation and hydraulic strut constraints are not simulated. Prepared boom extends 0.8 m beyond the original pin, with 9 degree working pitch; bucket mounting counter-rotates it to retain its source floor angle.";
   Edit(body,root=>{
    var rig=root.GetComponent<PreparedVehicleRig>();Vector3 delta=new Vector3(-.91f,.73f,1.03f)-rig.wheels[0].transform.localPosition;
    foreach(Transform child in root.transform)child.localPosition+=delta;
    for(int i=0;i<4;i++){var w=rig.wheels[i];w.radius=.73f;w.mass=70;w.suspensionDistance=.24f;w.forceAppPointDistance=.1f;var spring=w.suspensionSpring;spring.spring=65000;spring.damper=5000;spring.targetPosition=.5f;w.suspensionSpring=spring;}
    var box=root.GetComponentInChildren<BoxCollider>();box.center=new Vector3(0,1,-.25f);box.size=new Vector3(1.25f,.75f,3.4f);
    var boomMesh=root.GetComponentsInChildren<Transform>().Single(t=>t.name=="boom");
    var pivot=new GameObject("Boom | authored lift pivot").transform;pivot.SetParent(root.transform,false);pivot.localPosition=BoomOrigin;boomMesh.SetParent(pivot,true);rig.boom=pivot;
    var front=root.GetComponentsInChildren<AssetSocket>().Single(s=>s.id=="front");front.transform.SetParent(pivot,false);front.transform.localPosition=BucketOrigin-BoomOrigin;front.transform.localRotation=Quaternion.Euler(-WorkingBoomPitch,0,0);front.interfaceId=def.mountId;front.maximumWidth=3.2f;front.maximumMass=1200;
    pivot.localRotation=Quaternion.Euler(WorkingBoomPitch,0,0);rig.maximumWheelRadius=.8f;rig.maximumWheelWidth=.65f;
   });
   bucket.interfaceId=def.mountId;bucket.attachment.id="loader-bucket-16";bucket.attachment.kind=AttachmentKind.Bucket;bucket.attachment.mass=550;bucket.attachment.capacity=1.6f;bucket.attachment.width=2.84f;bucket.attachment.mountId=def.mountId;
   bucket.tags=new[]{"bucket","snow","loader","palette-atlas"};bucket.assumptions="Prepared bucket pin X2.81/Z1.03 after 0.8 m clearance extension; intake/discharge at X4.01/Z0.567. Bucket-local geometry and anchors retain source dimensions. Width 2.84 m measured; retained prototype capacity 1.6 cubic metres and mass 550 kg are unmeasured. Preauthored carried-snow proxy is scaled by existing gameplay.";
   Edit(bucket,root=>{
    var mesh=root.GetComponentsInChildren<MeshFilter>().Single();Vector3 delta=root.transform.InverseTransformPoint(mesh.transform.TransformPoint(Vector3.zero));
    var anchors=root.GetComponentsInChildren<AssetAnchor>();anchors.Single(a=>a.role==AnchorRole.Connection).transform.localPosition=delta;
    anchors.Single(a=>a.role==AnchorRole.WorkingEdge).transform.localPosition=delta+BucketEdge;
    var load=GameObject.CreatePrimitive(PrimitiveType.Sphere);load.name="Carried snow | authored visual";UnityEngine.Object.DestroyImmediate(load.GetComponent<Collider>());load.transform.SetParent(root.transform,false);load.transform.localPosition=delta+BucketLoad;load.transform.localScale=new Vector3(2.2f,.4f,.9f);load.GetComponent<Renderer>().sharedMaterial=snow;var anchor=load.AddComponent<AssetAnchor>();anchor.id="load";anchor.role=AnchorRole.LoadVisual;load.SetActive(false);
   });
   foreach(var a in new[]{body,bucket}){a.reviewNotes="Prototype integration authorized. Review full movement, driving and Galaxy A07 performance before Ready publication.";EditorUtility.SetDirty(a);Require(CatalogValidation.Check(a).Ok,"prepared catalog structure: "+a.id);}
   EditorUtility.SetDirty(def);EditorUtility.SetDirty(bucket.attachment);AssetDatabase.SaveAssets();
   var fit=ScriptableObject.CreateInstance<FittingProfile>();fit.vehicle=body;fit.part=bucket;fit.socketId="front";fit.reviewNotes="Measured source pin alignment. Full motion review still required.";AssetDatabase.CreateAsset(fit,CatalogPreparation.Root+"/Recipes/ImportedLoaderBucketFit.asset");
   var recipe=ScriptableObject.CreateInstance<AssemblyRecipe>();recipe.id="imported-snow-loader";recipe.displayName="Golden snow loader";recipe.vehicle=body;recipe.fittings.Add(fit);AssetDatabase.CreateAsset(recipe,CatalogPreparation.Root+"/Recipes/ImportedLoader.asset");
   var assembled=CatalogAssembly.Preview(recipe,false);
   try{
    assembled.name="Golden snow loader | authored machinery";foreach(var t in assembled.GetComponentsInChildren<Transform>(true))t.gameObject.layer=8;
    var rig=assembled.GetComponent<PreparedVehicleRig>();var slot=assembled.GetComponentsInChildren<AssetSocket>().Single(s=>s.id=="front");
    var tool=assembled.AddComponent<AttachmentController>();tool.useAuthoredRig=true;tool.dumpMinimumLift=1;tool.mount=slot.transform;tool.boom=rig.boom;var part=slot.transform.GetChild(0);tool.bucket=new GameObject("Bucket | tilt pivot").transform;tool.bucket.SetParent(slot.transform,false);part.SetParent(tool.bucket,true);
    var anchors=slot.GetComponentsInChildren<AssetAnchor>(true);tool.mouth=anchors.Single(a=>a.role==AnchorRole.WorkingEdge).transform;tool.loadVisual=anchors.Single(a=>a.role==AnchorRole.LoadVisual).transform;
    var effect=new GameObject("Snow spray | preauthored loader effect");effect.transform.SetParent(tool.mouth,false);var ps=effect.AddComponent<ParticleSystem>();ps.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);var main=ps.main;main.playOnAwake=false;main.startLifetime=.7f;main.startSize=.12f;main.startSpeed=2.5f;main.maxParticles=150;main.simulationSpace=ParticleSystemSimulationSpace.World;var emission=ps.emission;emission.enabled=false;ps.GetComponent<ParticleSystemRenderer>().sharedMaterial=snow;tool.spray=ps;
    var physics=UnityEngine.Object.Instantiate(def);physics.id="wheeled-loader-01";physics.displayName="Golden snow loader";AssetDatabase.CreateAsset(physics,CatalogPreparation.Root+"/Recipes/ImportedLoader-physics.asset");
    var m=assembled.AddComponent<MachineController>();m.definition=physics;m.equipment=bucket.attachment;m.Tool=tool;m.wheels=rig.wheels;m.wheelVisuals=rig.wheelVisuals;m.Fuel=physics.fuelCapacity;m.Owned=false;
    var rb=assembled.GetComponent<Rigidbody>();rb.mass=physics.mass+bucket.attachment.mass;rb.centerOfMass=(physics.dryCenterOfMass*physics.mass+assembled.transform.InverseTransformPoint(tool.mount.position)*bucket.attachment.mass)/rb.mass;
    assembled.GetComponent<CatalogPlacement>().assetId=recipe.id;
    VehicleLightsAuthoring.Configure(assembled,"Loader");
    recipe.bakedPrefab=PrefabUtility.SaveAsPrefabAsset(assembled,PrefabPath);EditorUtility.SetDirty(recipe);AssetDatabase.SaveAssets();return recipe.bakedPrefab;
   }finally{UnityEngine.Object.DestroyImmediate(assembled);}
  }
  public static MachineController Create(Vector3 position,Quaternion rotation,Material snow){
   var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);if(!prefab)prefab=Build(snow);
   if(prefab.GetComponent<AttachmentController>().bucket.name!="Bucket | tilt pivot"){
    // Refit the early intake candidate in place, keeping all asset GUIDs.
    var entry=CatalogPreparation.Library().Find("imported-loader-bucket");Edit(entry,FixBucketAnchors);
    var contents=PrefabUtility.LoadPrefabContents(PrefabPath);try{
     var tool=contents.GetComponent<AttachmentController>();var part=tool.bucket;FixBucketAnchors(part.gameObject);
     var fit=AssetDatabase.LoadAssetAtPath<FittingProfile>(CatalogPreparation.Root+"/Recipes/ImportedLoaderBucketFit.asset");CatalogAssembly.Align(part.gameObject,tool.mount,fit);
     var pivot=new GameObject("Bucket | tilt pivot").transform;pivot.SetParent(tool.mount,false);part.SetParent(pivot,true);tool.bucket=pivot;tool.dumpMinimumLift=1;
     PrefabUtility.SaveAsPrefabAsset(contents,PrefabPath);
    }finally{PrefabUtility.UnloadPrefabContents(contents);}prefab=AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);AssetDatabase.SaveAssets();
   }
   if(Vector3.Distance(prefab.GetComponent<AttachmentController>().mount.localPosition,BucketOrigin-BoomOrigin)>.001f){
    var body=CatalogPreparation.Library().Find("imported-loader-body");Edit(body,FitClearance);
    body.assumptions="Boom extended 0.8 m in prepared export for tyre clearance; working pitch 9 degrees. Original source preserved. Hydraulics remain simplified.";EditorUtility.SetDirty(body);
    var contents=PrefabUtility.LoadPrefabContents(PrefabPath);try{FitClearance(contents);var m=contents.GetComponent<MachineController>();m.GetComponent<Rigidbody>().centerOfMass=(m.definition.mass*m.definition.dryCenterOfMass+m.equipment.mass*contents.transform.InverseTransformPoint(m.Tool.mount.position))/(m.definition.mass+m.equipment.mass);PrefabUtility.SaveAsPrefabAsset(contents,PrefabPath);}finally{PrefabUtility.UnloadPrefabContents(contents);}prefab=AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);AssetDatabase.SaveAssets();
   }
   var root=(GameObject)PrefabUtility.InstantiatePrefab(prefab);root.transform.SetPositionAndRotation(position,rotation);return root.GetComponent<MachineController>();
  }
  static void FitClearance(GameObject root){
   root.GetComponent<PreparedVehicleRig>().boom.localRotation=Quaternion.Euler(WorkingBoomPitch,0,0);
   var socket=root.GetComponentsInChildren<AssetSocket>().Single(s=>s.id=="front").transform;socket.localPosition=BucketOrigin-BoomOrigin;socket.localRotation=Quaternion.Euler(-WorkingBoomPitch,0,0);
  }
  static void FixBucketAnchors(GameObject part){
   var mesh=part.GetComponentsInChildren<MeshFilter>(true).Single(f=>f.name=="LoaderBucket");var delta=part.transform.InverseTransformPoint(mesh.transform.TransformPoint(Vector3.zero));
   var anchors=part.GetComponentsInChildren<AssetAnchor>(true);anchors.Single(a=>a.role==AnchorRole.Connection).transform.localPosition=delta;
   anchors.Single(a=>a.role==AnchorRole.WorkingEdge).transform.localPosition=delta+BucketEdge;
   anchors.Single(a=>a.role==AnchorRole.LoadVisual).transform.localPosition=delta+BucketLoad;
  }
  [MenuItem("Snowglow/Asset intake/Use imported snow loader")]
  public static void Install(){
   if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Stop Play first.");
   var stage=PrefabStageUtility.GetCurrentPrefabStage();if(stage!=null){Require(!stage.scene.isDirty,"save edited prefab first");StageUtility.GoToMainStage();}
   Require(!UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty,"save edited scene first");
   Directory.CreateDirectory("Logs/LoaderIntake");File.Copy(SceneValidation.ScenePath,"Logs/LoaderIntake/Pinefall-before.unity",true);
   var scene=EditorSceneManager.OpenScene(SceneValidation.ScenePath);var session=scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<PrototypeSession>()).Single();
   string firstPrefab=PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(session.Fleet[0].gameObject);
   var old=session.Fleet[1];var loader=Create(new Vector3(old.transform.position.x,SnowField.Ground+.25f,old.transform.position.z),old.transform.rotation,session.Snow.GetComponentInChildren<MeshRenderer>().sharedMaterial);
   session.Fleet[1]=loader;UnityEngine.Object.DestroyImmediate(old.gameObject);EditorUtility.SetDirty(session);EditorSceneManager.MarkSceneDirty(scene);SceneValidation.Validate(scene);
   Require(EditorSceneManager.SaveScene(scene),"save scene");AssetDatabase.SaveAssets();scene=EditorSceneManager.OpenScene(SceneValidation.ScenePath);SceneValidation.Validate(scene);
   session=scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<PrototypeSession>()).Single();loader=session.Fleet[1];
   Require(loader.definition.id=="wheeled-loader-01"&&loader.definition.price==1400&&!loader.Owned,"purchase/save identity retained");
   Require(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(session.Fleet[0].gameObject)==firstPrefab,"free starter unchanged");
   Require(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(loader.gameObject)==PrefabPath,"saved imported loader instance");
   var probe=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath));
   try{
    var m=probe.GetComponent<MachineController>();m.Initialize();Require(m.enabled&&m.equipment.Validate(m.definition)==null,"controller initialization and compatibility");m.Tool.SendMessage("Update");float workingHeight=m.Tool.mouth.position.y;
    var shell=m.Tool.bucket.GetComponentInChildren<MeshRenderer>();Require(shell.bounds.min.y>-.05f&&shell.bounds.min.y<.4f&&Vector3.Distance(shell.bounds.center,m.Tool.mouth.position)<2,"bucket shell and edge aligned at work height");
    m.Tool.ToggleWork();m.Tool.SendMessage("Update");Require(m.Tool.mouth.position.y>workingHeight+.3f,"carry raises bucket mouth");m.Tool.RequestDump();m.Tool.SendMessage("Update");Require(m.Tool.Tilt==1&&m.Tool.Lift==1&&m.Tool.mouth.position.y>workingHeight+.2f,"dump tips bucket above working edge height: work="+workingHeight+", dump="+m.Tool.mouth.position.y+", lift="+m.Tool.Lift+", boom="+m.Tool.boom.localEulerAngles+", bucket="+m.Tool.bucket.localEulerAngles);
    Require(shell.bounds.min.y>.1f,"dumped shell clears ground plane");m.Tool.Load=.8f;m.Tool.SendMessage("Update");Require(m.Tool.loadVisual.gameObject.activeSelf,"authored load visual responds");
    m.SetActiveMachine(true);Require(!m.Body.isKinematic,"drive activation");m.SetActiveMachine(false);Require(m.Body.isKinematic,"parking");
    foreach(var filter in m.GetComponentsInChildren<MeshFilter>(true))Require(filter.sharedMesh&&AssetDatabase.Contains(filter.sharedMesh),"persistent meshes");
    // Conservative clearance for all lift/tilt commands, including tyre steering.
    // A cylinder's maximum Z extent during yaw is bounded by radius/half-width.
    float tyreFront=float.NegativeInfinity;
    for(int n=0;n<2;n++){
     var w=m.wheels[n];var bounds=m.wheelVisuals[n].GetComponentInChildren<Renderer>().bounds;
     float halfWidth=Mathf.Max(Mathf.Abs(bounds.min.x-w.transform.position.x),Mathf.Abs(bounds.max.x-w.transform.position.x));
     tyreFront=Mathf.Max(tyreFront,w.transform.position.z+Mathf.Sqrt(w.radius*w.radius+halfWidth*halfWidth));
    }
    float minimumGap=float.PositiveInfinity;
    for(int lift=0;lift<=20;lift++)for(int tilt=-10;tilt<=10;tilt++){
     m.Tool.Lift=lift/20f;m.Tool.Tilt=tilt/10f;m.Tool.Load=0;m.Tool.SendMessage("Update");
     float gap=shell.bounds.min.z-tyreFront;minimumGap=Mathf.Min(minimumGap,gap);
     Require(gap>.08f,"bucket/tyre clearance at lift="+m.Tool.Lift+", tilt="+m.Tool.Tilt+", gap="+gap);
    }
    File.WriteAllText("Logs/LoaderIntake/ClearanceChecks.txt","PASS: 441 lift/tilt samples; bucket rear remains beyond conservative steered front tyre envelopes. Minimum gap "+minimumGap.ToString("F3")+" m. Work, carry and dump ground clearance also checked. Continuous poses, suspension/body contacts and dynamic driving still require playtesting.");
   }finally{UnityEngine.Object.DestroyImmediate(probe);}
   MechanicsChecks.Run();var report=new StringBuilder("PASS: Unity "+Application.unityVersion+" / "+EditorUserBuildSettings.activeBuildTarget+". Scene saved/reopened; imported loader, stable save ID, price/ownership, unchanged free tractor, controller initialization, carry/dump/load visual, drive/park and 19 snow checks passed.\n");
   Metrics(loader.gameObject,report);Metrics(session.Fleet[0].gameObject,report);
   var planes=GeometryUtility.CalculateFrustumPlanes(session.CameraRig.view);long candidateTriangles=0,snowTriangles=0;int candidateSlots=0;
   foreach(var r in scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<MeshRenderer>())){
    if(!r.enabled||!GeometryUtility.TestPlanesAABB(planes,r.bounds))continue;var filter=r.GetComponent<MeshFilter>();if(!filter||!filter.sharedMesh)continue;var mesh=filter.sharedMesh;
    long triangles=0;for(int i=0;i<mesh.subMeshCount;i++)triangles+=(long)mesh.GetIndexCount(i)/3;candidateTriangles+=triangles;candidateSlots+=r.sharedMaterials.Length;if(r.transform.IsChildOf(session.Snow.transform))snowTriangles+=triangles;
   }
   report.AppendLine("Saved camera frustum candidate inventory: "+candidateTriangles+" mesh triangles ("+snowTriangles+" snow), "+candidateSlots+" material slots. Conservative CPU bounds estimate; excludes terrain/UI/particles/shadow passes and does not account for batching/occlusion. Not actual GPU draw statistics.");
   report.AppendLine("Hydraulic linkage/chassis articulation are approximated. Live snow/driving, physical capacity, Android build and Galaxy A07 frame timings remain untested. Mesh counts are geometry inventory, not measured rendered frame counts.");File.WriteAllText(Report,report.ToString());
   Selection.activeGameObject=loader.gameObject;if(SceneView.lastActiveSceneView)SceneView.lastActiveSceneView.LookAt(loader.transform.position+new Vector3(0,1.5f,.4f),Quaternion.Euler(25,215,0),6,true,true);Debug.Log(report.ToString());
  }
  static void Metrics(GameObject root,StringBuilder report){
   int vertices=0,triangles=0,slots=0,renderers=0;var materials=new System.Collections.Generic.HashSet<Material>();
   foreach(var r in root.GetComponentsInChildren<Renderer>(true)){
    if(r is ParticleSystemRenderer)continue;renderers++;slots+=r.sharedMaterials.Length;foreach(var mat in r.sharedMaterials)materials.Add(mat);
    var filter=r.GetComponent<MeshFilter>();var mesh=filter?filter.sharedMesh:null;if(mesh){vertices+=mesh.vertexCount;for(int i=0;i<mesh.subMeshCount;i++)triangles+=(int)mesh.GetIndexCount(i)/3;}
   }report.AppendLine(root.name+": "+vertices+" imported vertices, "+triangles+" triangles, "+renderers+" renderers, "+slots+" material slots, "+materials.Count+" distinct materials (includes inactive carried load, excludes particles).");
  }
  static void Require(bool condition,string label){if(!condition)throw new InvalidOperationException("Loader check: "+label);}
 }
}
// Model intake keeps source files and saved player profiles intact.
