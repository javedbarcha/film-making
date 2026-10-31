using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Snowglow.Editor {
 public static class RecoveryPhysicsChecks {
  const string Request="Logs/RecoveryPhysics.request";
  [InitializeOnLoadMethod]static void Reload(){EditorApplication.update+=Pending;}
  static void Pending(){if(!File.Exists(Request)){EditorApplication.update-=Pending;return;}if(EditorApplication.isCompiling||EditorApplication.isUpdating)return;if(EditorApplication.isPlaying){EditorApplication.isPlaying=false;return;}if(EditorApplication.isPlayingOrWillChangePlaymode)return;EditorApplication.update-=Pending;File.Delete(Request);try{Run();}catch(Exception e){Directory.CreateDirectory("Logs/RecoveryPhysics");File.WriteAllText("Logs/RecoveryPhysics/Validation.txt","FAIL: "+e);Debug.LogException(e);}}
  static void Require(bool condition,string message){if(!condition)throw new InvalidOperationException("Recovery: "+message);}
  [MenuItem("Snowglow/Run recovery physics checks")]
  public static void Run(){
   Directory.CreateDirectory("Logs/RecoveryPhysics");var main=SceneManager.GetActiveScene();var session=main.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<PrototypeSession>(true)).Single();
   var fixture=EditorSceneManager.NewPreviewScene();var physics=fixture.GetPhysicsScene();bool sync=Physics.autoSyncTransforms;int assertions=0;
   try{
    Require(physics.IsValid()&&!physics.Equals(Physics.defaultPhysicsScene),"preview scene must isolate physics");
    Physics.autoSyncTransforms=false;
    foreach(var source in session.Fleet){
     var clone=UnityEngine.Object.Instantiate(source.gameObject);SceneManager.MoveGameObjectToScene(clone,fixture);var machine=clone.GetComponent<MachineController>();machine.Initialize();machine.Body.useGravity=false;
     var depot=machine.transform.position+Vector3.up*.3f;float load=machine.definition.loader ? .8f : 0;machine.Tool.Load=load;
     foreach(bool active in new[]{true,false}){
      machine.SetActiveMachine(true);machine.Body.position=depot+new Vector3(35,2,20);machine.Body.rotation=Quaternion.Euler(0,120,0);machine.Body.linearVelocity=new Vector3(5,0,3);machine.Body.angularVelocity=Vector3.up;physics.Simulate(.02f);
      machine.SetActiveMachine(active);machine.RecoverToDepot();
      Require(Vector3.Distance(machine.Body.position,depot)<.001f,source.name+" physics pose did not teleport");assertions++;
      Require(machine.Body.isKinematic==!active&&machine.Body.interpolation==RigidbodyInterpolation.Interpolate,"activity/interpolation changed");assertions++;
      Require(machine.Tool.Load==load,"bucket load changed");assertions++;
      for(int i=0;i<120;i++){physics.Simulate(.02f);Require(Vector3.Distance(machine.Body.position,depot)<.02f,"snapped back after physics tick "+i);assertions++;}
      machine.SetActiveMachine(true);physics.Simulate(.02f);Require(machine.Body.linearVelocity.sqrMagnitude<.001f&&Vector3.Distance(machine.Body.position,depot)<.02f,"stale momentum after activation");assertions++;
     }
     UnityEngine.Object.DestroyImmediate(clone);
    }
    SceneValidation.Validate(main);MechanicsChecks.Run();File.WriteAllText("Logs/RecoveryPhysics/Validation.txt","PASS: Unity "+Application.unityVersion+"; "+assertions+" assertions on actual tractor/loader clones in isolated 3D physics. Active/inactive recovery, interpolated Rigidbody, autoSyncTransforms=false, loaded bucket retained, 120 post-recovery steps per case and reactivation without stale momentum. Main scene validation and 19 snow ledger checks passed. No scene assets or player saves written. Full live driving/recovery click and device validation remain untested.");Debug.Log(File.ReadAllText("Logs/RecoveryPhysics/Validation.txt"));
   }finally{Physics.autoSyncTransforms=sync;EditorSceneManager.ClosePreviewScene(fixture);SceneManager.SetActiveScene(main);}
  }
 }
}
