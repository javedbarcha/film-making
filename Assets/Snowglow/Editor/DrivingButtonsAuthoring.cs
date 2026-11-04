using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace Snowglow.Editor {
 public static class DrivingButtonsAuthoring {
  const string Request="Logs/DrivingButtons.request";
  [InitializeOnLoadMethod]static void Reload(){EditorApplication.update+=Pending;}
  static void Pending(){
   if(!File.Exists(Request)){EditorApplication.update-=Pending;return;}
   if(EditorApplication.isCompiling||EditorApplication.isUpdating)return;
   if(EditorApplication.isPlaying){EditorApplication.isPlaying=false;return;}
   if(EditorApplication.isPlayingOrWillChangePlaymode)return;
   EditorApplication.update-=Pending;File.Delete(Request);
   try{Install();}catch(Exception e){Directory.CreateDirectory("Logs/DrivingButtons");File.WriteAllText("Logs/DrivingButtons/Validation.txt","FAIL: "+e);Debug.LogException(e);}
  }
  static void Require(bool value,string message){if(!value)throw new InvalidOperationException("Driving buttons: "+message);}
  static RectTransform Group(Transform parent,string name,float anchor){
   var found=parent.Find(name);var r=found?(RectTransform)found:new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();
   r.SetParent(parent,false);r.anchorMin=new Vector2(anchor,0);r.anchorMax=new Vector2(anchor,1);r.pivot=new Vector2(anchor,.5f);r.sizeDelta=Vector2.zero;r.anchoredPosition=Vector2.zero;return r;
  }
  static void Position(RectTransform r,Transform parent,float x,float y,float w,float h,bool top=false,bool right=false){
   r.SetParent(parent,false);r.anchorMin=r.anchorMax=new Vector2(right?1:0,top?1:0);r.pivot=new Vector2(right?1:0,top?1:0);r.anchoredPosition=new Vector2(x,top?-y:y);r.sizeDelta=new Vector2(w,h);
   var label=r.GetComponentInChildren<Text>(true);if(label){var t=label.rectTransform;t.anchorMin=Vector2.zero;t.anchorMax=Vector2.one;t.offsetMin=new Vector2(6,6);t.offsetMax=new Vector2(-6,-6);label.alignment=TextAnchor.MiddleCenter;}
  }
  static void Label(GameObject g,string title){var text=g.GetComponentInChildren<Text>(true);text.text=title;text.fontSize=17;}
  static void Bind(UICommand command){var button=command.GetComponent<Button>();button.onClick=new Button.ButtonClickedEvent();UnityEventTools.AddPersistentListener(button.onClick,command.Click);}
  public static void Apply(PrototypeHUD hud){
   var root=(RectTransform)hud.driving.transform;root.SetParent(hud.safeArea,false);root.anchorMin=Vector2.zero;root.anchorMax=Vector2.one;root.offsetMin=root.offsetMax=Vector2.zero;root.localScale=Vector3.one;
   foreach(var touch in root.GetComponentsInChildren<TouchControl>(true).Where(t=>t.channel=="lift"||t.channel=="tilt").ToArray())UnityEngine.Object.DestroyImmediate(touch.gameObject);
   foreach(var command in root.GetComponentsInChildren<UICommand>(true).Where(c=>c.command=="auto").ToArray())UnityEngine.Object.DestroyImmediate(command.gameObject);
   hud.leftControls=Group(root,"Left controls | 5 percent inset",.05f);hud.rightControls=Group(root,"Right controls | 5 percent inset",.95f);hud.centreControls=Group(root,"Equipment controls",.5f);
   var steer=root.GetComponentsInChildren<TouchControl>(true).FirstOrDefault(t=>t.channel=="steer"||t.channel=="steerLeft");Require(steer,"steering template missing");
   var right=root.GetComponentsInChildren<TouchControl>(true).FirstOrDefault(t=>t.channel=="steerRight");
   if(!right)right=UnityEngine.Object.Instantiate(steer.gameObject,hud.leftControls).GetComponent<TouchControl>();
   steer.channel="steerLeft";steer.value=1;steer.name="Steer left";right.channel="steerRight";right.value=1;right.name="Steer right";
   Position((RectTransform)steer.transform,hud.leftControls,0,36,104,118);Position((RectTransform)right.transform,hud.leftControls,140,36,104,118);Label(steer.gameObject,"LEFT");Label(right.gameObject,"RIGHT");
   foreach(var command in root.GetComponentsInChildren<UICommand>(true)){
    var rect=(RectTransform)command.transform;
    switch(command.command){
     case "camera":Position(rect,hud.leftControls,0,110,150,48,true);break;
     case "open:garage":Position(rect,hud.leftControls,0,168,150,48,true);break;
     case "pause":Position(rect,hud.rightControls,0,110,160,48,true,true);break;
     case "reverse":Position(rect,hud.rightControls,0,206,118,64,false,true);break;
     case "work":Position(rect,hud.centreControls,-235,98,150,50);break;
     case "dump":Position(rect,hud.centreControls,-75,98,150,50);break;
     case "angle":Position(rect,hud.centreControls,85,98,150,50);break;
     case "recover":case "recover:depot":command.command="recover:depot";Position(rect,hud.centreControls,-105,36,210,44);Label(command.gameObject,"RECOVER TO DEPOT");Bind(command);break;
    }
   }
   foreach(var touch in root.GetComponentsInChildren<TouchControl>(true)){
    if(touch.channel=="throttle")Position((RectTransform)touch.transform,hud.rightControls,0,36,118,120,false,true);
    if(touch.channel=="brake")Position((RectTransform)touch.transform,hud.rightControls,-138,36,110,104,false,true);
   }
   if(hud.cameraTools){var tools=(RectTransform)hud.cameraTools.transform;tools.SetParent(hud.leftControls,false);tools.anchorMin=Vector2.zero;tools.anchorMax=Vector2.one;tools.offsetMin=tools.offsetMax=Vector2.zero;}
   foreach(var text in hud.GetComponentsInChildren<Text>(true).Where(t=>t.text.StartsWith("DESKTOP: W/S",StringComparison.Ordinal)))text.text="DESKTOP: W/S drive/reverse | A/D steer | Space brake | E work/carry | Q blade angle\nB dump | C cycles 3 cameras | H recover to depot | Esc pause\nOrbit: right-drag or J/L + I/K | scroll or +/- zoom | Home resets";
   foreach(var zoom in hud.driving.GetComponentsInChildren<UICommand>(true).Where(c=>c.command=="camera:in"||c.command=="camera:out").ToArray())UnityEngine.Object.DestroyImmediate(zoom.gameObject);
   if(hud.cameraTools){var reset=hud.cameraTools.GetComponentsInChildren<UICommand>(true).Single(c=>c.command=="camera:reset");Position((RectTransform)reset.transform,hud.cameraTools.transform,0,230,150,42,true);}
   VehicleLightsAuthoring.ConfigureButton(hud);PSDUIIntake.ApplyPrepared(hud);EditorUtility.SetDirty(hud);
  }
  [MenuItem("Snowglow/UI/Install simplified driving buttons")]
  public static void Install(){
   var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();Require(scene.path==SceneValidation.ScenePath,"open Pinefall first");Directory.CreateDirectory("Logs/DrivingButtons");
   if(!File.Exists("Logs/DrivingButtons/Pinefall-before.unity"))File.Copy(scene.path,"Logs/DrivingButtons/Pinefall-before.unity");
   var hud=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<PrototypeHUD>(true)).Single();Apply(hud);SceneValidation.Validate(scene);EditorSceneManager.MarkSceneDirty(scene);Require(EditorSceneManager.SaveScene(scene),"save failed");
   scene=EditorSceneManager.OpenScene(SceneValidation.ScenePath);SceneValidation.Validate(scene);hud=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<PrototypeHUD>(true)).Single();
   Require(hud.leftControls.anchorMin.x==.05f&&hud.rightControls.anchorMin.x==.95f,"side anchors not saved");
   Require(!hud.driving.GetComponentsInChildren<TouchControl>(true).Any(t=>t.channel=="lift"||t.channel=="tilt"||t.channel=="steer"),"obsolete touch controls remain");
   Require(!hud.driving.GetComponentsInChildren<UICommand>(true).Any(c=>c.command=="auto"),"manual toggle remains");
   var recover=hud.driving.GetComponentsInChildren<UICommand>(true).Single(c=>c.command=="recover:depot");Require(recover.GetComponent<Button>().onClick.GetPersistentEventCount()==1,"recover listener missing");
   Checks();MechanicsChecks.Run();
   typeof(CompactHUDAuthoring).GetMethod("Preview",BindingFlags.Static|BindingFlags.NonPublic).Invoke(null,new object[]{hud});
   File.Copy("Logs/CompactHUD/Preview.png","Logs/DrivingButtons/Preview.png",true);EditorSceneManager.OpenScene(SceneValidation.ScenePath);
   File.WriteAllText("Logs/DrivingButtons/Validation.txt","PASS: Unity "+Application.unityVersion+"; saved/reopened left/right button steering, enlarged/separated F/R, explicit Recover-to-depot listener, removed manual controls and 5% safe-area side anchors. Touch hold/release fixture passed; automatic full bucket carry and depot recovery fixtures passed; 19 snow-ledger checks passed. Native UI preview rendered. Continuous driving, actual recovery click and phone ergonomics remain untested; no player save written by fixtures.");
   Debug.Log(File.ReadAllText("Logs/DrivingButtons/Validation.txt"));
  }
  static void Checks(){
   var field=typeof(PrototypeSession).GetField("<Instance>k__BackingField",BindingFlags.Static|BindingFlags.NonPublic);var old=PrototypeSession.Instance;var root=new GameObject("Disposable driving buttons checks");
   try{
    var game=root.AddComponent<PrototypeSession>();game.Input=root.AddComponent<PrototypeInput>();field.SetValue(null,game);
    var a=new GameObject("Left").AddComponent<TouchControl>();a.transform.SetParent(root.transform);a.channel="steerLeft";
    var b=new GameObject("Right").AddComponent<TouchControl>();b.transform.SetParent(root.transform);b.channel="steerRight";
    var ev=new PointerEventData(null){pointerId=1};var ev2=new PointerEventData(null){pointerId=2};a.OnPointerDown(ev);b.OnPointerDown(ev2);
    Require(game.Input.touchLeft==1&&game.Input.touchRight==1,"simultaneous steering missing");a.OnPointerUp(ev);Require(game.Input.touchLeft==0&&game.Input.touchRight==1,"release cancelled opposite button");b.OnPointerUp(ev2);Require(game.Input.touchRight==0,"released steering stuck");
    var machine=root.AddComponent<MachineController>();machine.definition=ScriptableObject.CreateInstance<VehicleDefinition>();machine.definition.loader=true;machine.equipment=ScriptableObject.CreateInstance<AttachmentDefinition>();machine.equipment.capacity=1.6f;
    var tool=root.AddComponent<AttachmentController>();tool.mount=root.transform;tool.mouth=root.transform;tool.Initialize(machine);tool.Auto=false;tool.Load=1.6f;
    typeof(AttachmentController).GetMethod("UpdateAutoCarry",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(tool,null);
    // Simulate is the public path that normalizes legacy Manual state; no snow work occurs while stationary.
    tool.Simulate(.02f,0);Require(tool.Auto&&!tool.Working&&tool.Lift>=.6f,"full bucket did not automatically lift");
    machine.Tool=tool;machine.ActiveMachine=true;
    typeof(MachineController).GetField("<Body>k__BackingField",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(machine,root.GetComponent<Rigidbody>());
    var depot=new Vector3(-95,6,-90);typeof(MachineController).GetField("parkingPosition",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(machine,depot);
    typeof(MachineController).GetField("parkingRotation",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(machine,Quaternion.identity);
    root.transform.position=Vector3.zero;machine.RecoverToDepot();Require(Vector3.Distance(root.transform.position,depot+Vector3.up*.3f)<.001f&&tool.Load==1.6f,"depot recovery failed or lost load");
    UnityEngine.Object.DestroyImmediate(machine.definition);UnityEngine.Object.DestroyImmediate(machine.equipment);
   }finally{field.SetValue(null,old);UnityEngine.Object.DestroyImmediate(root);}
  }
 }
}
