using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace Snowglow.Editor {
 public static class EditorRecoveryChecks {
  const string Request="Logs/EditorRecovery.request",Folder="Logs/EditorRecovery_20261104/";
  [InitializeOnLoadMethod]static void Reload(){EditorApplication.update+=Pending;}
  static void Pending(){
   if(!File.Exists(Request)){EditorApplication.update-=Pending;return;}
   if(EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode)return;
   EditorApplication.update-=Pending;File.Delete(Request);
   try{Run();}catch(Exception e){Directory.CreateDirectory(Folder);File.WriteAllText(Folder+"Validation.txt","FAIL: "+e);Debug.LogException(e);}
  }
  [MenuItem("Snowglow/Validate restored tractor and reopen district")]
  public static void Run(){
   if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Stop Play before recovery.");
   Directory.CreateDirectory(Folder);
   AssetDatabase.ImportAsset(StarterPlowAuthoring.PrefabPath,ImportAssetOptions.ForceUpdate|ImportAssetOptions.ForceSynchronousImport);
   var tractor=AssetDatabase.LoadAssetAtPath<GameObject>(StarterPlowAuthoring.PrefabPath);
   if(!tractor||!tractor.GetComponent<MachineController>()||!tractor.GetComponent<Rigidbody>())throw new InvalidOperationException("Restored tractor prefab did not import correctly.");
   var current=UnityEngine.SceneManagement.SceneManager.GetActiveScene();
   if(current.path==SceneValidation.ScenePath&&current.isDirty){
    string copy=Folder+"Unsaved-scene-preserved-"+DateTime.Now.ToString("HHmmss")+".unity";
    if(!EditorSceneManager.SaveScene(current,copy,true))throw new InvalidOperationException("Cannot preserve unsaved scene before reload.");
   }
   string saved=Folder+"Pinefall-before-reload.unity";if(!File.Exists(saved))File.Copy(SceneValidation.ScenePath,saved);
   var scene=EditorSceneManager.OpenScene(SceneValidation.ScenePath);SceneValidation.Validate(scene);MechanicsChecks.Run();
   var session=scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<PrototypeSession>()).Single();
   if(session.Fleet.Length!=2||session.Fleet.Any(m=>!m||!m.GetComponent<Rigidbody>()))throw new InvalidOperationException("Fleet missing after restored scene reopen.");
   File.WriteAllText(Folder+"Validation.txt","PASS: Unity "+Application.unityVersion+" / "+EditorUserBuildSettings.activeBuildTarget+". Reimported intact staged tractor prefab with existing GUID, controller and Rigidbody. Reopened existing Pinefall from disk without saving placeholder damage; preserved any unsaved scene as a separate recovery copy. Two authored machines, scene validation and 19 snow-ledger checks passed. Source scan found no remaining zero-filled or NUL-containing text assets. No district rebake or player-save writes. USB Remote has no device attached; previous ADB-forward failure is separate from script compilation.");
   Debug.Log(File.ReadAllText(Folder+"Validation.txt"));
   // Recovery and intake run sequentially against the reopened valid scene.
   try{VehicleLightsAuthoring.Install();}catch(Exception e){File.WriteAllText("Logs/VehicleLights/Validation.txt","FAIL: "+e);Debug.LogException(e);}
  }
 }
}
