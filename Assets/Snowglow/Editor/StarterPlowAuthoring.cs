using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Snowglow.Editor {
 // Explicit prototype integration. Catalog source approval remains separate from
 // choosing a test machine for Pinefall; all assembly happens before Play.
 public static class StarterPlowAuthoring {
  public const string PrefabPath=CatalogPreparation.Root+"/Prefabs/GoldenTractor-Starter.prefab";
  const string DefinitionPath=CatalogPreparation.Root+"/Recipes/GoldenTractor-Starter-physics.asset";
  const string Request="Logs/InstallStarterPlow.request";
  [InitializeOnLoadMethod]static void Reload(){EditorApplication.update+=Pending;}
  static void Pending(){
   if(!File.Exists(Request)){EditorApplication.update-=Pending;return;}
   if(EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode)return;
   EditorApplication.update-=Pending;File.Delete(Request);
   try{Install();}catch(Exception e){File.WriteAllText("Logs/StarterPlowValidation.txt","FAIL: "+e);Debug.LogException(e);}
  }
  public static MachineController Create(Vector3 position,Quaternion rotation,Material snow){
   var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
   if(!prefab)prefab=Build(snow);
   var root=(GameObject)PrefabUtility.InstantiatePrefab(prefab);root.transform.SetPositionAndRotation(position,rotation);
   return root.GetComponent<MachineController>();
  }
  static GameObject Build(Material snow){
   var recipe=AssetDatabase.LoadAssetAtPath<AssemblyRecipe>(CatalogPreparation.Root+"/Recipes/GoldenTractor.asset");
   if(!recipe||!snow)throw new InvalidOperationException("Prepared GoldenTractor recipe and snow material are required.");
   var root=CatalogAssembly.Preview(recipe,false);
   try{
    root.name="Golden tractor | free starter plow";
    foreach(var t in root.GetComponentsInChildren<Transform>(true))t.gameObject.layer=8;
    var rig=root.GetComponent<PreparedVehicleRig>();
    var definition=UnityEngine.Object.Instantiate(recipe.vehicle.vehicle);
    // Preserve old starter saves without matching display names or fleet indices.
    definition.id="municipal-plow-01";definition.displayName="Golden snow-plowing tractor";definition.price=0;
    definition.mass+=5; // roof beacon; attachment mass is accounted separately
    AssetDatabase.CreateAsset(definition,DefinitionPath);
    var slot=root.GetComponentsInChildren<AssetSocket>().Single(s=>s.id=="front");
    var fitting=recipe.fittings.Single(f=>f.part.kind==CatalogKind.Attachment);
    var tool=root.AddComponent<AttachmentController>();tool.useAuthoredRig=true;tool.bladeRaiseDistance=.45f;
    tool.mount=slot.transform;tool.mouth=slot.GetComponentsInChildren<AssetAnchor>().Single(a=>a.role==AnchorRole.WorkingEdge).transform;
    var effects=new GameObject("Snow spray | authored starter effect");effects.layer=8;effects.transform.SetParent(tool.mouth,false);
    var ps=effects.AddComponent<ParticleSystem>();ps.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
    var main=ps.main;main.playOnAwake=false;main.startLifetime=.7f;main.startSize=.12f;main.startSpeed=2.5f;main.gravityModifier=1;main.maxParticles=150;main.simulationSpace=ParticleSystemSimulationSpace.World;
    var emission=ps.emission;emission.enabled=false;ps.GetComponent<ParticleSystemRenderer>().sharedMaterial=snow;tool.spray=ps;
    var machine=root.AddComponent<MachineController>();machine.definition=definition;machine.equipment=fitting.part.attachment;machine.Tool=tool;machine.wheels=rig.wheels;machine.wheelVisuals=rig.wheelVisuals;machine.Owned=true;machine.Fuel=definition.fuelCapacity;
    var rb=root.GetComponent<Rigidbody>();rb.mass=definition.mass+machine.equipment.mass;rb.centerOfMass=(definition.mass*definition.dryCenterOfMass+machine.equipment.mass*tool.mount.localPosition)/rb.mass;
    // This is an equipped output, not the unequipped catalog body.
    root.GetComponent<CatalogPlacement>().assetId="golden-tractor-starter";
    VehicleLightsAuthoring.Configure(root,"Tractor");
    var prefab=PrefabUtility.SaveAsPrefabAsset(root,PrefabPath);AssetDatabase.SaveAssets();return prefab;
   }finally{UnityEngine.Object.DestroyImmediate(root);}
  }
  [MenuItem("Snowglow/Asset intake/Use tractor as free starter plow")]
  public static void Install(){
   if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Stop Play before installing machinery.");
   var stage=PrefabStageUtility.GetCurrentPrefabStage();if(stage!=null){if(stage.scene.isDirty)throw new InvalidOperationException("Save the edited prefab before installing machinery.");StageUtility.GoToMainStage();}
   var current=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
   if(current.isDirty)throw new InvalidOperationException("Save the edited scene before installing machinery.");
   var scene=EditorSceneManager.OpenScene(SceneValidation.ScenePath);
   var session=scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<PrototypeSession>()).Single();
   var old=session.Fleet[0];var loader=session.Fleet[1];
   string loaderPrefab=PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(loader.gameObject);
   Vector3 position=new Vector3(old.transform.position.x,SnowField.Ground+.25f,old.transform.position.z);Quaternion rotation=old.transform.rotation;
   var snow=session.Snow.GetComponentInChildren<MeshRenderer>().sharedMaterial;
   var first=Create(position,rotation,snow);session.Fleet[0]=first;UnityEngine.Object.DestroyImmediate(old.gameObject);
   EditorUtility.SetDirty(session);EditorSceneManager.MarkSceneDirty(scene);
   SceneValidation.Validate(scene);
   if(!EditorSceneManager.SaveScene(scene))throw new IOException("Could not save starter replacement.");AssetDatabase.SaveAssets();
   // Check deserialized scene references, not just transient authoring objects.
   scene=EditorSceneManager.OpenScene(SceneValidation.ScenePath);SceneValidation.Validate(scene);
   session=scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<PrototypeSession>()).Single();first=session.Fleet[0];
   Require(first.definition.id=="municipal-plow-01"&&first.definition.price==0&&first.Owned,"free starter identity/ownership");
   Require(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(first.gameObject)==PrefabPath,"imported prefab is fleet slot zero");
   Require(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(session.Fleet[1].gameObject)==loaderPrefab,"loader preserved");
   Require(first.wheels.Take(2).All(w=>Mathf.Abs(w.radius-.578f)<.001f)&&first.wheels.Skip(2).All(w=>Mathf.Abs(w.radius-.888f)<.001f),"unequal wheel radii preserved");
   Require(first.equipment.Validate(first.definition)==null&&first.Tool.useAuthoredRig,"compatible authored plow rig");
   Require(first.GetComponentsInChildren<MachineController>().Length==1,"single machine controller");
   // Exercise controller initialization, wheel physics configuration and authored
   // rest transforms on an isolated instance; no session/save file is started.
   var probe=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath));
   try{var m=probe.GetComponent<MachineController>();m.Initialize();Require(m.enabled&&m.Body&&m.Tool.Working&&m.Tool.Load==0,"controller initialization");m.Tool.ToggleWork();Require(!m.Tool.Working&&m.Tool.Lift==1,"raise command");m.Tool.ToggleWork();Require(m.Tool.Working&&m.Tool.Lift==0,"lower command");m.Tool.CycleAngle();Require(m.Tool.Angle==1,"angle command");m.SetActiveMachine(true);Require(!m.Body.isKinematic,"drive activation");m.SetActiveMachine(false);Require(m.Body.isKinematic,"park activation");}finally{UnityEngine.Object.DestroyImmediate(probe);}
   MechanicsChecks.Run();
   File.WriteAllText("Logs/StarterPlowValidation.txt","PASS: Unity "+Application.unityVersion+" / "+EditorUserBuildSettings.activeBuildTarget+". Saved/reopened Pinefall; imported starter prefab in Fleet[0], free/owned, legacy save ID preserved, loader unchanged, unequal wheel radii, compatible plow, controller initialization, work/raise/angle and drive/park commands passed. All 19 snow checks passed. No player save was changed. Sustained driving, hydraulic linkage fidelity, Android build and Galaxy A07 profiling remain untested.");
   Selection.activeGameObject=first.gameObject;if(SceneView.lastActiveSceneView)SceneView.lastActiveSceneView.FrameSelected();
   Debug.Log("Snowglow imported tractor installed as free starter. See Logs/StarterPlowValidation.txt");
  }
  static void Require(bool condition,string label){if(!condition)throw new InvalidOperationException("Starter plow check failed: "+label);}
 }
}
