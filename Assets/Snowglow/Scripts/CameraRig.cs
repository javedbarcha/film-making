using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Snowglow {
 // Value 3 is retained only to migrate existing serialized scenes.
 public enum CameraMode { Isometric, Follow, Orbit, Scenic }
 public sealed class CameraRig : MonoBehaviour {
  public Camera view;public Terrain terrain;public CameraMode mode;
  [FormerlySerializedAs("Follow"),SerializeField]bool legacyFollow;
  public float groundClearance=1,collisionRadius=.4f;
  public float orbitDistance=15,orbitPitch=25;
  [HideInInspector]public float scenicDistance=40,scenicPitch=18,scenicSpeed=4;
  public float isometricNearSize=12f,isometricFarSize=19.2f,isometricFullZoomSpeed=8,isometricZoomResponse=1.1f;
  public float followDistance=13,followPoseResponse=2.5f,followFullTiltSpeed=4;
  float isometricSize,followPose,followPoseVelocity;
  Vector3 isometricFocus,isometricFocusVelocity;
  public float Yaw {get;private set;}public float Pitch {get;private set;}public float Distance {get;private set;}
  public bool Interactive=>mode==CameraMode.Orbit;
  public string ModeName=>mode.ToString().ToUpperInvariant();
  public bool Follow {get=>mode==CameraMode.Follow;set=>SetMode(value?CameraMode.Follow:CameraMode.Isometric);}
  Vector3 velocity;Transform lastTarget;bool initialized,resetRequested=true,mouseCaptured;
  float manualHold,pinchDistance;int pinchA=-1,pinchB=-1;
  readonly Dictionary<int,Vector2> fingers=new Dictionary<int,Vector2>();
  readonly List<int> released=new List<int>();readonly List<RaycastResult> uiHits=new List<RaycastResult>();
  readonly RaycastHit[] obstacles=new RaycastHit[32];PointerEventData pointer;EventSystem pointerEvents;
  const int WorldMask=(1<<0)|(1<<9);
  public void Toggle()=>SetMode((CameraMode)(((int)mode+1)%3));
  public void SetMode(CameraMode value){mode=value==CameraMode.Scenic?CameraMode.Isometric:value;legacyFollow=mode==CameraMode.Follow;resetRequested=true;velocity=Vector3.zero;ClearGesture();}
  public void ResetView(){resetRequested=true;manualHold=0;ClearGesture();}
  public void Zoom(float steps){if(!Interactive||steps==0)return;Distance=Mathf.Clamp(Distance*Mathf.Exp(-steps*.12f),7,42);manualHold=6;}
  public void Rotate(Vector2 degrees){if(!Interactive||degrees.sqrMagnitude==0)return;Yaw=Mathf.Repeat(Yaw+degrees.x,360);Pitch=Mathf.Clamp(Pitch+degrees.y,5,75);manualHold=6;}
  void Awake(){if(mode==CameraMode.Scenic)mode=CameraMode.Isometric;if(legacyFollow&&mode==CameraMode.Isometric)mode=CameraMode.Follow;}
  void OnDisable()=>ClearGesture();
  void OnApplicationFocus(bool focused){if(!focused)ClearGesture();}
  void ClearGesture(){mouseCaptured=false;fingers.Clear();pinchDistance=0;pinchA=pinchB=-1;}
  bool OverUI(Vector2 position){
   var events=EventSystem.current;if(!events)return false;
   if(pointer==null||pointerEvents!=events){pointer=new PointerEventData(events);pointerEvents=events;}
   pointer.position=position;uiHits.Clear();events.RaycastAll(pointer,uiHits);return uiHits.Count>0;
  }
  void ReadInput(float dt){
   var k=Keyboard.current;if(k!=null){
    Rotate(new Vector2((k.lKey.isPressed?1:0)-(k.jKey.isPressed?1:0),(k.kKey.isPressed?1:0)-(k.iKey.isPressed?1:0))*70*dt);
    float zoom=(k.equalsKey.isPressed||k.numpadPlusKey.isPressed?1:0)-(k.minusKey.isPressed||k.numpadMinusKey.isPressed?1:0);if(zoom!=0)Zoom(zoom*5*dt);
    if(k.homeKey.wasPressedThisFrame)ResetView();
   }
   // A finger beginning on a driving/HUD control never becomes a camera finger.
   var screen=Touchscreen.current;bool touching=false;int first=-1,second=-1;Vector2 a=default,b=default,previousA=default;bool aKnown=false;
   if(screen!=null){
    released.Clear();foreach(var pair in fingers)released.Add(pair.Key);
    foreach(var touch in screen.touches){if(!touch.press.isPressed)continue;touching=true;int id=touch.touchId.ReadValue();var position=touch.position.ReadValue();
     bool known=fingers.TryGetValue(id,out var previous);
     if(!known&&(touch.phase.ReadValue()!=UnityEngine.InputSystem.TouchPhase.Began||OverUI(position)))continue;
     fingers[id]=position;released.Remove(id);
     if(first<0){first=id;a=position;previousA=previous;aKnown=known;}else if(second<0){second=id;b=position;}
    }
    foreach(int id in released)fingers.Remove(id);
   }
   float pixels=Mathf.Max(240,Screen.height);
   if(second>=0){
    float span=Vector2.Distance(a,b);if(pinchA==first&&pinchB==second&&pinchDistance>10&&span>10)Zoom(Mathf.Log(span/pinchDistance)/.12f);
    pinchA=first;pinchB=second;pinchDistance=span;
   }else{
    if(first>=0&&aKnown&&pinchDistance==0){var delta=(a-previousA)/pixels;Rotate(new Vector2(delta.x*180,-delta.y*110));}
    pinchDistance=0;pinchA=pinchB=-1;
   }
   var mouse=Mouse.current;if(mouse==null||touching){mouseCaptured=false;return;}
   if(mouse.rightButton.wasPressedThisFrame)mouseCaptured=!OverUI(mouse.position.ReadValue());
   if(!mouse.rightButton.isPressed)mouseCaptured=false;
   if(mouseCaptured){var delta=mouse.delta.ReadValue()/pixels;Rotate(new Vector2(delta.x*180,-delta.y*110));}
   float scroll=mouse.scroll.ReadValue().y;
   if(InputSystem.settings.scrollDeltaBehavior==InputSettings.ScrollDeltaBehavior.KeepPlatformSpecificInputRange&&(Application.platform==RuntimePlatform.WindowsEditor||Application.platform==RuntimePlatform.WindowsPlayer))scroll/=120;
   if(scroll!=0&&!OverUI(mouse.position.ReadValue()))Zoom(scroll);
  }
  float Ground(Vector3 position){
   if(!terrain)terrain=Terrain.activeTerrain;
   return terrain?terrain.SampleHeight(position)+terrain.transform.position.y:SnowField.Ground;
  }
  Vector3 AvoidObstacles(Transform target,Vector3 focus,Vector3 position){
   var offset=position-focus;float distance=offset.magnitude;if(distance<.001f)return position;
   int count=Physics.SphereCastNonAlloc(focus,collisionRadius,offset/distance,obstacles,distance,WorldMask,QueryTriggerInteraction.Ignore);float nearest=distance;
   for(int i=0;i<count;i++)if(obstacles[i].collider&&!obstacles[i].collider.transform.IsChildOf(target))nearest=Mathf.Min(nearest,Mathf.Max(.75f,obstacles[i].distance-.15f));
   return focus+offset/distance*nearest;
  }
  Vector3 AboveGround(Vector3 position,Quaternion rotation){
   float lift=Ground(position)+Mathf.Max(.5f,groundClearance)-position.y;
   float halfY=view.nearClipPlane*Mathf.Tan(view.fieldOfView*Mathf.Deg2Rad*.5f),halfX=halfY*view.aspect;
   if(view.orthographic){halfY=view.orthographicSize;halfX=halfY*view.aspect;}
   // Protect the near-plane corners too, including on steep terrain and wide screens.
   for(int x=-1;x<=1;x+=2)for(int y=-1;y<=1;y+=2){var corner=position+rotation*new Vector3(x*halfX,y*halfY,view.nearClipPlane);lift=Mathf.Max(lift,Ground(corner)+.2f-corner.y);}
   if(lift>0)position.y+=lift;return position;
  }
  void LateUpdate(){var g=PrototypeSession.Instance;if(g==null||g.Active==null||!view)return;UpdateForTarget(g.Active.transform,Time.unscaledDeltaTime,g.IsRunning);}
  public void UpdateForTarget(Transform target,float dt,bool controls,float? signedSpeedOverride=null){
   if(!view||!target)return;
   if(mode==CameraMode.Scenic)SetMode(CameraMode.Isometric);
   var body=target.GetComponent<Rigidbody>();
   float signedSpeed=signedSpeedOverride??(body?Vector3.Dot(body.linearVelocity,target.forward):0);
   dt=Mathf.Clamp(dt,.0001f,.1f);bool recenter=resetRequested;bool switched=lastTarget!=target;lastTarget=target;
   if(resetRequested||switched){Yaw=Mathf.Repeat(target.eulerAngles.y,360);Pitch=Mathf.Clamp(orbitPitch,5,75);Distance=Mathf.Clamp(orbitDistance,7,42);manualHold=0;resetRequested=false;ClearGesture();if(isometricSize<=0)isometricSize=isometricNearSize;followPose=0;followPoseVelocity=0;}
   if(Interactive&&controls){ReadInput(dt);manualHold=Mathf.Max(0,manualHold-dt);}else ClearGesture();
   if(controls){
    float desiredSize=Mathf.Lerp(isometricNearSize,isometricFarSize,Mathf.Clamp01(Mathf.Abs(signedSpeed)/Mathf.Max(1,isometricFullZoomSpeed)));
    isometricSize=Mathf.Lerp(isometricSize,desiredSize,1-Mathf.Exp(-dt/Mathf.Max(.1f,isometricZoomResponse)));
    // Tilt grows continuously with actual speed, capped at the direction
    // limits. Tiny physics drift counts as stopped, with no yaw arc.
    float amount=Mathf.Clamp01((Mathf.Abs(signedSpeed)-.12f)/Mathf.Max(.1f,followFullTiltSpeed-.12f));
    float desiredPose=-Mathf.Sign(signedSpeed)*amount;
    followPose=Mathf.SmoothDamp(followPose,desiredPose,ref followPoseVelocity,Mathf.Max(.5f,followPoseResponse),1,dt);
   }
   view.orthographic=mode==CameraMode.Isometric;
   view.orthographicSize=isometricSize*Mathf.Max(1,(16f/9)/Mathf.Max(.6f,view.aspect));
   view.fieldOfView=mode==CameraMode.Orbit?52:60;
   Vector3 focus=target.position+Vector3.up*1.6f;
   if(mode==CameraMode.Isometric){
    // Orthographic size controls framing independently of camera distance. A
    // perspective collision pull-in moves its wide near plane through roofs.
    // Smooth the subject once; keep a fixed orientation and retreat along the
    // viewing axis for terrain safety without shifting the screen composition.
    isometricFocus=(!initialized||switched||recenter||(isometricFocus-focus).sqrMagnitude>160*160)?focus:Vector3.SmoothDamp(isometricFocus,focus,ref isometricFocusVelocity,.18f,1000,dt);
    if(!initialized||switched||recenter)isometricFocusVelocity=Vector3.zero;
    var isoRotation=Quaternion.Euler(41,45,0);var backwards=-(isoRotation*Vector3.forward);
    var isoPosition=isometricFocus+backwards*Mathf.Max(60,view.orthographicSize*2.5f);
    for(int pass=0;pass<3;pass++){float lift=AboveGround(isoPosition,isoRotation).y-isoPosition.y;if(lift<=.001f)break;isoPosition+=backwards*(lift/Mathf.Max(.1f,backwards.y));}
    transform.SetPositionAndRotation(isoPosition,isoRotation);velocity=Vector3.zero;initialized=true;return;
   }
   Vector3 wanted=mode==CameraMode.Isometric?target.position+new Vector3(-15,19,-15):mode==CameraMode.Follow?target.position+Quaternion.Euler(0,target.eulerAngles.y,0)*Vector3.back*followDistance+Vector3.up*(followDistance*Mathf.Tan(35*Mathf.Deg2Rad)+1.6f):focus+Quaternion.Euler(Pitch,Yaw,0)*Vector3.back*Distance;
   wanted=AvoidObstacles(target,focus,wanted);Quaternion looking=mode==CameraMode.Isometric?Quaternion.Euler(41,45,0):mode==CameraMode.Follow?Quaternion.Euler(35+(followPose>=0?10:20)*followPose,target.eulerAngles.y,0):Quaternion.LookRotation(focus-wanted);
   wanted=AboveGround(wanted,looking);
   Vector3 position=(!initialized||switched||(transform.position-wanted).sqrMagnitude>160*160)?wanted:Vector3.SmoothDamp(transform.position,wanted,ref velocity,.18f,1000,dt);
   position=AvoidObstacles(target,focus,position);looking=mode==CameraMode.Isometric?Quaternion.Euler(41,45,0):mode==CameraMode.Follow?Quaternion.Euler(35+(followPose>=0?10:20)*followPose,target.eulerAngles.y,0):Quaternion.LookRotation(focus-position);
   var rotation=initialized?Quaternion.Slerp(transform.rotation,looking,1-Mathf.Exp(-dt*9)):looking;
   for(int pass=0;pass<3;pass++)position=AboveGround(position,rotation);
   transform.SetPositionAndRotation(position,rotation);initialized=true;
  }
 }
}
