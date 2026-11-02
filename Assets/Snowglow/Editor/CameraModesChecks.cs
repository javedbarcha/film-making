using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using TouchPhase=UnityEngine.InputSystem.TouchPhase;

namespace Snowglow.Editor {
 public static class CameraModesChecks {
  static int checks,poses;
  [InitializeOnLoadMethod]static void QueueFollowChecks(){EditorApplication.update+=PendingFollowChecks;}
  static void PendingFollowChecks(){
   const string request="Logs/FollowGear.request";
   if(!File.Exists(request)){EditorApplication.update-=PendingFollowChecks;return;}
   if(EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode)return;
   EditorApplication.update-=PendingFollowChecks;File.Delete(request);
   if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty){File.WriteAllText("Logs/CameraModes/Validation.txt","NOT RUN: Save manual scene changes before camera checks.");return;}
   try{Run();}catch(Exception e){File.WriteAllText("Logs/CameraModes/Validation.txt","FAIL: "+e);Debug.LogException(e);}
  }
  static void Check(bool ok,string name){if(!ok)throw new InvalidOperationException("Camera checks: "+name);checks++;}
  static void Grounded(CameraRig rig){
   var camera=rig.view;var t=rig.terrain;float Ground(Vector3 p)=>t.SampleHeight(p)+t.transform.position.y;
   Check(rig.transform.position.y>=Ground(rig.transform.position)+rig.groundClearance-.002f,"camera centre below clearance");
   var corners=new Vector3[4];camera.CalculateFrustumCorners(new Rect(0,0,1,1),camera.nearClipPlane,Camera.MonoOrStereoscopicEye.Mono,corners);
   foreach(var corner in corners){var world=camera.transform.TransformPoint(corner);Check(world.y>=Ground(world)+.19f,"near plane below terrain");}poses++;
  }
  static void Render(Camera view,string name){
   var rt=new RenderTexture(960,540,24);var previous=RenderTexture.active;var old=view.targetTexture;Texture2D texture=null;
   try{view.targetTexture=rt;view.Render();RenderTexture.active=rt;texture=new Texture2D(960,540,TextureFormat.RGB24,false);texture.ReadPixels(new Rect(0,0,960,540),0,0);texture.Apply();File.WriteAllBytes("Logs/CameraModes/"+name+".png",texture.EncodeToPNG());}
   finally{view.targetTexture=old;RenderTexture.active=previous;rt.Release();UnityEngine.Object.DestroyImmediate(rt);if(texture)UnityEngine.Object.DestroyImmediate(texture);}
  }
  [MenuItem("Snowglow/Run camera orbit input and terrain checks")]
  public static void Run(){
   if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Stop Play first.");Directory.CreateDirectory("Logs/CameraModes");checks=poses=0;
   var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();SceneValidation.Validate(scene);var source=scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<PrototypeSession>()).Single();
   var field=typeof(PrototypeSession).GetField("<Instance>k__BackingField",BindingFlags.Static|BindingFlags.NonPublic);var previousSession=PrototypeSession.Instance;var previousEvents=EventSystem.current;
   var previousKeyboard=Keyboard.current;var previousMouse=Mouse.current;var previousTouch=Touchscreen.current;
   // Input System's Editor buffer restores its update counter after Update().
   // Use the package's own temporary player-update test hook for edge/gesture tests.
   var manager=typeof(InputSystem).GetField("s_Manager",BindingFlags.Static|BindingFlags.NonPublic).GetValue(null);
   var playerUpdates=manager.GetType().GetProperty("runPlayerUpdatesInEditMode",BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic);
   bool oldPlayerUpdates=(bool)playerUpdates.GetValue(manager);
   var raycasters=source.HUD.GetComponentsInChildren<GraphicRaycaster>(true);var raycasterStates=raycasters.Select(r=>r.enabled).ToArray();
   GameObject fixture=null,cameraObject=null,targetObject=null,canvasObject=null,uiCameraObject=null,wall=null;Keyboard keyboard=null;Mouse mouse=null;Touchscreen touchscreen=null;
   try{
    playerUpdates.SetValue(manager,true);
    foreach(var r in raycasters)r.enabled=false;
    fixture=new GameObject("Camera check session"){hideFlags=HideFlags.HideAndDontSave};var session=fixture.AddComponent<PrototypeSession>();session.Profile=new ProfileSave();session.Panel="";session.Input=fixture.AddComponent<PrototypeInput>();field.SetValue(null,session);var events=fixture.AddComponent<EventSystem>();events.SendMessage("OnEnable");EventSystem.current=events;
    targetObject=new GameObject("Camera check target"){hideFlags=HideFlags.HideAndDontSave};targetObject.transform.position=source.Fleet[0].transform.position;session.Fleet=new[]{targetObject.AddComponent<MachineController>()};
    cameraObject=new GameObject("Camera check rig"){hideFlags=HideFlags.HideAndDontSave};var view=cameraObject.AddComponent<Camera>();view.enabled=false;view.nearClipPlane=.2f;view.farClipPlane=1000;view.clearFlags=CameraClearFlags.SolidColor;view.backgroundColor=new Color(.66f,.76f,.83f);view.aspect=16f/9;cameraObject.AddComponent<UniversalAdditionalCameraData>();var rig=cameraObject.AddComponent<CameraRig>();rig.view=view;rig.terrain=source.CameraRig.terrain;session.CameraRig=rig;Check(rig.terrain,"authored terrain binding");
    keyboard=InputSystem.AddDevice<Keyboard>();mouse=InputSystem.AddDevice<Mouse>();touchscreen=InputSystem.AddDevice<Touchscreen>();
    void Tick(bool controls=true){rig.UpdateForTarget(targetObject.transform,.02f,controls);}
    void Keys(params Key[] keys){InputSystem.QueueStateEvent(keyboard,new KeyboardState(keys));InputSystem.Update();}
    void MouseStateAt(Vector2 position,Vector2 delta,bool down=false,float scroll=0){InputSystem.QueueStateEvent(mouse,new MouseState{position=position,delta=delta,scroll=new Vector2(0,scroll)}.WithButton(MouseButton.Right,down));InputSystem.Update();Tick();}
    void Touch(int id,TouchPhase phase,Vector2 position){InputSystem.QueueStateEvent(touchscreen,new TouchState{touchId=id,phase=phase,position=position});InputSystem.Update();Tick();}
    void Settle(){for(int n=0;n<50;n++)Tick(false);}
    Keys();_ = keyboard.cKey.wasPressedThisFrame;_ = keyboard.homeKey.wasPressedThisFrame;_ = mouse.rightButton.wasPressedThisFrame;
    rig.SetMode(CameraMode.Isometric);Tick(false);foreach(var expected in new[]{CameraMode.Follow,CameraMode.Orbit,CameraMode.Isometric}){Keys(Key.C);session.Input.SendMessage("Update");Check(rig.mode==expected,"C mode cycle "+expected+" actual "+rig.mode+" pressed "+keyboard.cKey.isPressed+" edge "+keyboard.cKey.wasPressedThisFrame);Keys();session.Input.SendMessage("Update");}
    rig.SetMode(CameraMode.Orbit);Tick(false);Keys(Key.W);session.Input.SendMessage("Update");Check(session.Input.Throttle==1&&!session.Input.Reverse,"W stays forward in Orbit");Keys(Key.S);session.Input.SendMessage("Update");Check(session.Input.Throttle==1&&session.Input.Reverse,"S stays reverse in Orbit");Keys(Key.A);session.Input.SendMessage("Update");Check(session.Input.Steer==-1,"A steering stays vehicle-relative");Keys();
    float yaw=rig.Yaw;Keys(Key.L);Tick();Check(rig.Yaw>yaw,"L orbits right");Keys(Key.I);Tick();Check(rig.Pitch<25,"I tilts toward horizon");Keys();rig.Rotate(new Vector2(1080,-999));Check(rig.Pitch==5&&rig.Yaw>=0&&rig.Yaw<360,"full yaw wrap and minimum pitch");rig.Rotate(new Vector2(0,999));Check(rig.Pitch==75,"maximum pitch");rig.Zoom(999);Check(rig.Distance==7,"near zoom clamp");rig.Zoom(-999);Check(rig.Distance==42,"far zoom clamp");
    uiCameraObject=new GameObject("Stationary camera for UI fixture"){hideFlags=HideFlags.HideAndDontSave};var uiView=uiCameraObject.AddComponent<Camera>();uiView.enabled=false;uiView.cullingMask=1<<5;uiView.nearClipPlane=.1f;uiView.farClipPlane=10;uiView.aspect=16f/9;uiCameraObject.AddComponent<UniversalAdditionalCameraData>();
    canvasObject=new GameObject("Camera UI exclusion fixture",typeof(RectTransform)){hideFlags=HideFlags.HideAndDontSave};var canvas=canvasObject.AddComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=uiView;canvas.planeDistance=2;canvas.sortingOrder=5000;canvasObject.layer=5;canvasObject.AddComponent<GraphicRaycaster>().SendMessage("OnEnable");var panel=new GameObject("Simulated drive button",typeof(RectTransform),typeof(Image));panel.transform.SetParent(canvasObject.transform,false);panel.layer=5;var rect=(RectTransform)panel.transform;rect.anchorMin=rect.anchorMax=Vector2.zero;rect.pivot=Vector2.zero;rect.anchoredPosition=new Vector2(20,20);rect.sizeDelta=new Vector2(180,180);Canvas.ForceUpdateCanvases();Render(uiView,"UI-input-fixture");Canvas.ForceUpdateCanvases();var uiPoint=RectTransformUtility.WorldToScreenPoint(uiView,rect.TransformPoint(rect.rect.center));var worldPoint=uiPoint+new Vector2(400,300);
    var debugHits=new System.Collections.Generic.List<RaycastResult>();var debugPointer=new PointerEventData(EventSystem.current){position=uiPoint};EventSystem.current.RaycastAll(debugPointer,debugHits);
    File.WriteAllText("Logs/CameraModes/UI-diagnostic.txt","point "+uiPoint+" contains "+RectTransformUtility.RectangleContainsScreenPoint(rect,uiPoint,uiView)+" hits "+debugHits.Count+" registered "+UnityEngine.UI.GraphicRegistry.GetRaycastableGraphicsForCanvas(canvas).Count+" culled "+panel.GetComponent<Image>().canvasRenderer.cull+" raycast "+panel.GetComponent<Image>().Raycast(uiPoint,uiView)+" depth "+panel.GetComponent<Image>().depth+" camera "+uiView.pixelRect+" screen "+Screen.width+" x "+Screen.height+" current events "+EventSystem.current.name);

    rig.ResetView();Tick(false);yaw=rig.Yaw;MouseStateAt(uiPoint,Vector2.zero,true);MouseStateAt(uiPoint+new Vector2(20,0),new Vector2(20,0),true);Check(rig.Yaw==yaw,"RMB beginning on UI excluded at "+uiPoint+" depth "+panel.GetComponent<Image>().depth+" rect "+rect.rect);MouseStateAt(worldPoint,Vector2.zero);MouseStateAt(worldPoint,Vector2.zero,true);MouseStateAt(worldPoint+new Vector2(60,0),new Vector2(60,0),true);Check(rig.Yaw!=yaw,"RMB world drag orbits");float distance=rig.Distance;MouseStateAt(worldPoint+new Vector2(60,0),Vector2.zero,false,InputSystem.settings.scrollDeltaBehavior==InputSettings.ScrollDeltaBehavior.UniformAcrossAllPlatforms?1:120);Check(rig.Distance<distance,"mouse wheel zooms");MouseStateAt(worldPoint+new Vector2(60,0),Vector2.zero);
    rig.ResetView();Tick(false);yaw=rig.Yaw;Touch(1,TouchPhase.Began,uiPoint);Touch(1,TouchPhase.Moved,worldPoint);Check(rig.Yaw==yaw,"touch starting on drive UI excluded after moving off");Touch(1,TouchPhase.Ended,worldPoint);Touch(2,TouchPhase.Began,worldPoint);Touch(2,TouchPhase.Moved,worldPoint+new Vector2(60,0));Check(rig.Yaw!=yaw,"one world finger orbits");Touch(3,TouchPhase.Began,worldPoint+new Vector2(300,0));distance=rig.Distance;Touch(3,TouchPhase.Moved,worldPoint+new Vector2(400,0));Check(rig.Distance<distance,"two world fingers pinch zoom");Touch(2,TouchPhase.Ended,worldPoint+new Vector2(60,0));Touch(3,TouchPhase.Ended,worldPoint+new Vector2(400,0));canvasObject.SetActive(false);
    rig.SetMode(CameraMode.Scenic);Tick(false);Check(rig.mode==CameraMode.Isometric,"legacy Scenic migrates out of gameplay");
    targetObject.transform.SetPositionAndRotation(new Vector3(0,7,30),Quaternion.identity);
    rig.SetMode(CameraMode.Isometric);
    for(int n=0;n<300;n++)rig.UpdateForTarget(targetObject.transform,.02f,true,0);
    float nearSize=view.orthographicSize;
    for(int n=0;n<300;n++)rig.UpdateForTarget(targetObject.transform,.02f,true,8);
    Check(view.orthographicSize>nearSize+2.5f,"Isometric zooms out at speed");
    for(int n=0;n<300;n++)rig.UpdateForTarget(targetObject.transform,.02f,true,-8);
    Check(view.orthographicSize>nearSize+2.5f,"Isometric zoom also follows reverse speed");
    float heldSize=view.orthographicSize;rig.UpdateForTarget(targetObject.transform,.1f,false,0);
    Check(view.orthographicSize==heldSize,"pause freezes speed zoom");
    for(int n=0;n<300;n++)rig.UpdateForTarget(targetObject.transform,.02f,true,0);
    Check(Mathf.Abs(view.orthographicSize-nearSize)<.1f,"Isometric closes as speed falls");
    rig.SetMode(CameraMode.Follow);
    void Travel(float speed,int frames=700){for(int n=0;n<frames;n++){rig.UpdateForTarget(targetObject.transform,.02f,true,speed);Grounded(rig);}}
    Travel(0);var resting=rig.transform.position;var restRotation=rig.transform.rotation;
    float Tilt()=>Mathf.Asin(-rig.transform.forward.y)*Mathf.Rad2Deg;
    Check(Mathf.Abs(Tilt()-35)<.1f,"resting pitch is 35 degrees");
    var gearButton=fixture.AddComponent<UICommand>();gearButton.command="reverse";gearButton.Click();Travel(0,100);
    Check(Vector3.Distance(resting,rig.transform.position)<.02f,"gear selection at rest keeps static rear pose");
    var beforeReverse=rig.transform.position;rig.UpdateForTarget(targetObject.transform,.02f,true,-4);
    Check(Vector3.Distance(beforeReverse,rig.transform.position)<.1f,"reverse tilt keeps position fixed");
    Travel(-4);var overhead=rig.transform.position;
    Check(Vector3.Distance(overhead,resting)<.03f,"reverse keeps fixed camera offset");
    Check(Vector3.Dot(overhead-targetObject.transform.position,Vector3.forward)<0,"reverse never swings in front");
    Check(Mathf.Abs(Tilt()-45)<.1f,"reverse pitch is 45 degrees");
    Travel(-2);Check(Mathf.Abs(Tilt()-39.85f)<.15f,"half reverse speed gives intermediate tilt");
    Travel(-12);Check(Mathf.Abs(Tilt()-45)<.1f,"reverse tilt clamps above full speed");
    var poseField=typeof(CameraRig).GetField("followPose",BindingFlags.Instance|BindingFlags.NonPublic);
    float held=(float)poseField.GetValue(rig);rig.UpdateForTarget(targetObject.transform,.1f,false,4);
    Check((float)poseField.GetValue(rig)==held,"pause holds tilt transition");
    Travel(0);Check(Vector3.Distance(resting,rig.transform.position)<.05f,"complete stop restores initial Follow pose");
    Check(Mathf.Abs(Tilt()-35)<.1f,"reverse stop returns tilt to 35 degrees");
    Travel(.03f);Check(Vector3.Distance(resting,rig.transform.position)<.05f,"tiny physics drift keeps resting pose");
    Travel(4);Check(Vector3.Distance(resting,rig.transform.position)<.03f,"forward keeps fixed camera offset");
    Check(Mathf.Abs(Tilt()-15)<.1f,"forward pitch is 15 degrees");
    Check(Vector3.Dot(rig.transform.position-targetObject.transform.position,Vector3.forward)<-10,"forward stays behind vehicle");
    Travel(2);Check(Mathf.Abs(Tilt()-25.31f)<.15f,"half forward speed gives intermediate tilt");
    Travel(12);Check(Mathf.Abs(Tilt()-15)<.1f,"forward tilt clamps above full speed");
    float movingTilt=Tilt();rig.UpdateForTarget(targetObject.transform,.02f,true,0);
    Check(Mathf.Abs(Tilt()-movingTilt)<.15f,"stopping returns tilt gently rather than snapping");
    Travel(0);Check(Vector3.Distance(resting,rig.transform.position)<.05f,"forward stop restores initial pose");
    Check(Mathf.Abs(Tilt()-35)<.1f,"stop restores 35 degree tilt");
    for(int n=0;n<100;n++){
     float previousTilt=Tilt();var before=rig.transform.position;rig.UpdateForTarget(targetObject.transform,.02f,true,n%2==0?-4:4);
     Check(Vector3.Distance(before,rig.transform.position)<.03f,"rapid reversals keep position fixed");
     Check(Mathf.Abs(Tilt()-previousTilt)<.15f,"rapid reversals tilt gently");
     Check(Vector3.Dot(rig.transform.position-targetObject.transform.position,Vector3.forward)<0,"rapid reversals never cross to front");
    }
    rig.SetMode(CameraMode.Orbit);Tick(false);rig.Rotate(new Vector2(60,0));
    Keys(Key.Home);Tick();Keys();Tick();Check(Mathf.Abs(Mathf.DeltaAngle(rig.Yaw,targetObject.transform.eulerAngles.y))<1,"Home recentres Orbit");
    foreach(var mode in new[]{CameraMode.Isometric,CameraMode.Follow,CameraMode.Orbit})foreach(float aspect in new[]{4f/3,20f/9})foreach(var xz in new[]{new Vector2(0,30),new Vector2(175,130),new Vector2(-190,-160)}){
     targetObject.transform.position=new Vector3(xz.x,rig.terrain.SampleHeight(new Vector3(xz.x,0,xz.y))+rig.terrain.transform.position.y+.5f,xz.y);rig.SetMode(mode);view.aspect=aspect;Tick(false);
     foreach(float pitch in new[]{5f,45,75})foreach(float angle in new[]{0f,90,180,270,359}){
      rig.Rotate(new Vector2(Mathf.DeltaAngle(rig.Yaw,angle),pitch-rig.Pitch));for(int step=0;step<6;step++){Tick(false);Grounded(rig);}
     }
    }
    // A real collider must pull the orbit inward; ignore our target's own colliders.
    targetObject.transform.SetPositionAndRotation(new Vector3(0,7,30),Quaternion.identity);rig.SetMode(CameraMode.Orbit);rig.orbitPitch=10;Tick(false);rig.Rotate(new Vector2(180,0));Settle();float openDistance=Vector3.Distance(rig.transform.position,targetObject.transform.position+Vector3.up*1.6f);
    wall=GameObject.CreatePrimitive(PrimitiveType.Cube);wall.hideFlags=HideFlags.HideAndDontSave;wall.transform.position=new Vector3(0,11,36);wall.transform.localScale=new Vector3(6,10,1);Physics.SyncTransforms();Settle();Check(Vector3.Distance(rig.transform.position,targetObject.transform.position+Vector3.up*1.6f)<openDistance-1,"obstruction pulls camera inward");UnityEngine.Object.DestroyImmediate(wall);wall=null;
    targetObject.transform.SetPositionAndRotation(source.Fleet[0].transform.position,source.Fleet[0].transform.rotation);view.aspect=16f/9;rig.orbitPitch=25;rig.SetMode(CameraMode.Orbit);Settle();Render(view,"Orbit-rear");rig.Rotate(new Vector2(180,0));Settle();Check(Vector3.Dot((rig.transform.position-targetObject.transform.position).normalized,targetObject.transform.forward)>.4f,"front of vehicle reachable");Render(view,"Orbit-front");rig.SetMode(CameraMode.Isometric);Settle();Render(view,"Isometric-close");
    MechanicsChecks.Run();File.WriteAllText("Logs/CameraModes/Validation.txt","PASS: Unity "+Application.unityVersion+" / "+EditorUserBuildSettings.activeBuildTarget+". Saved scene validated; "+checks+" assertions including "+poses+" terrain/near-plane poses, three-mode C cycle, W/S/A semantics, yaw/pitch/zoom limits, mouse drag/wheel, touch drag/pinch, UI exclusions, speed zoom, fixed rear Follow position, speed-proportional 35/45/15 degree tilt with partial-speed and limit checks, stop restoration, drift tolerance and rapid reversals, pause, Orbit reset, collision pull-in and front/rear views. Scene structure and 19 snow ledger checks passed. Three native scene renders saved. No runtime scene construction or player-save writes. Full live driving/gesture ergonomics, all obstacle cases, Android packaging and Galaxy A07 frame timings remain untested.\n");Debug.Log(File.ReadAllText("Logs/CameraModes/Validation.txt"));
   }finally{
    playerUpdates.SetValue(manager,oldPlayerUpdates);field.SetValue(null,previousSession);if(keyboard!=null)InputSystem.RemoveDevice(keyboard);if(mouse!=null)InputSystem.RemoveDevice(mouse);if(touchscreen!=null)InputSystem.RemoveDevice(touchscreen);if(previousKeyboard!=null)previousKeyboard.MakeCurrent();if(previousMouse!=null)previousMouse.MakeCurrent();if(previousTouch!=null)previousTouch.MakeCurrent();if(fixture)fixture.GetComponent<EventSystem>().SendMessage("OnDisable");EventSystem.current=previousEvents;
    if(canvasObject)canvasObject.GetComponent<GraphicRaycaster>().SendMessage("OnDisable");foreach(var go in new[]{wall,canvasObject,uiCameraObject,cameraObject,targetObject,fixture})if(go)UnityEngine.Object.DestroyImmediate(go);for(int i=0;i<raycasters.Length;i++)if(raycasters[i])raycasters[i].enabled=raycasterStates[i];
    EditorSceneManager.OpenScene(SceneValidation.ScenePath);
   }
  }
 }
}
