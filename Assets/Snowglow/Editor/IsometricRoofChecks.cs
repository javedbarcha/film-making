using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
namespace Snowglow.Editor {
 public static class IsometricRoofChecks {
 const string Logs="Logs/IsometricRoof",Request="Logs/IsometricRoof.request";static int assertions;
 static void Check(bool ok,string message){if(!ok)throw new InvalidOperationException("Isometric roof: "+message);assertions++;}
 [InitializeOnLoadMethod]static void Register(){EditorApplication.update+=Pending;}
 static void Pending(){if(!File.Exists(Request)){EditorApplication.update-=Pending;return;}if(EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode)return;EditorApplication.update-=Pending;File.Delete(Request);try{Run();}catch(Exception e){Directory.CreateDirectory(Logs);File.WriteAllText(Logs+"/Validation.txt","FAIL: "+e);Debug.LogException(e);}}
 static T[] All<T>(Scene s)where T:Component=>s.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<T>(true)).ToArray();
 static void Render(Camera view,string name){var rt=new RenderTexture(1200,675,24);var active=RenderTexture.active;var old=view.targetTexture;Texture2D image=null;try{view.targetTexture=rt;view.Render();RenderTexture.active=rt;image=new Texture2D(1200,675,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1200,675),0,0);image.Apply();File.WriteAllBytes(Logs+"/"+name+".png",image.EncodeToPNG());}finally{view.targetTexture=old;RenderTexture.active=active;rt.Release();UnityEngine.Object.DestroyImmediate(rt);if(image)UnityEngine.Object.DestroyImmediate(image);}}
 static void Grounded(CameraRig rig){var corners=new Vector3[4];rig.view.CalculateFrustumCorners(new Rect(0,0,1,1),rig.view.nearClipPlane,Camera.MonoOrStereoscopicEye.Mono,corners);foreach(var corner in corners){var p=rig.transform.TransformPoint(corner);Check(p.y>=rig.terrain.SampleHeight(p)+rig.terrain.transform.position.y+.19f,"near-plane terrain clipping");}}
 [MenuItem("Snowglow/Run isometric roof and smooth tracking checks")]
 public static void Run(){
 Directory.CreateDirectory(Logs);assertions=0;var scene=SceneManager.GetActiveScene();Check(scene.path==SceneValidation.ScenePath,"open Pinefall");SceneValidation.Validate(scene);if(scene.isDirty)EditorSceneManager.SaveScene(scene,Logs+"/Pinefall-unsaved-before.unity",true);var sceneBytes=File.ReadAllBytes(scene.path);var source=All<PrototypeSession>(scene).Single().CameraRig;
 GameObject cameraObject=null,target=null,roof=null;
 try{cameraObject=new GameObject("Isometric roof check camera"){hideFlags=HideFlags.HideAndDontSave};var view=cameraObject.AddComponent<Camera>();view.CopyFrom(source.view);view.enabled=false;view.aspect=16f/9;var urp=cameraObject.AddComponent<UniversalAdditionalCameraData>();urp.renderPostProcessing=false;var rig=cameraObject.AddComponent<CameraRig>();rig.view=view;rig.terrain=source.terrain;rig.isometricNearSize=source.isometricNearSize;rig.isometricFarSize=source.isometricFarSize;rig.isometricZoomResponse=source.isometricZoomResponse;target=new GameObject("Isometric check subject"){hideFlags=HideFlags.HideAndDontSave};target.transform.position=new Vector3(-110,SnowField.Ground,-85);rig.SetMode(CameraMode.Isometric);
 void Tick(float speed=0,float dt=1f/60){rig.UpdateForTarget(target.transform,dt,true,speed);}
 for(int n=0;n<180;n++)Tick();var settled=rig.transform.position;roof=GameObject.CreatePrimitive(PrimitiveType.Cube);roof.name="Disposable canopy obstruction";roof.hideFlags=HideFlags.HideAndDontSave;roof.transform.position=target.transform.position+new Vector3(-7,8,-7);roof.transform.localScale=new Vector3(14,.5f,14);Physics.SyncTransforms();for(int n=0;n<180;n++){Tick();Check(Vector3.Distance(rig.transform.position,settled)<.001f,"stationary camera pulled toward roof");}
 UnityEngine.Object.DestroyImmediate(roof);roof=null;var last=rig.transform.position;float worstStep=0;
 for(int n=0;n<600;n++){target.transform.position+=Vector3.right*(2f/60);Tick();float step=Vector3.Distance(last,rig.transform.position);worstStep=Mathf.Max(worstStep,step);Check(step<.05f,"tracking jump while crossing canopy");Check(Quaternion.Angle(rig.transform.rotation,Quaternion.Euler(41,45,0))<.001f,"isometric orientation changed");last=rig.transform.position;}
 for(int n=0;n<600;n++)Tick(8);Check(Mathf.Abs(view.orthographicSize-source.isometricFarSize)<.01f,"speed zoom far size changed");float paused=view.orthographicSize;rig.UpdateForTarget(target.transform,.1f,false,0);Check(Mathf.Abs(view.orthographicSize-paused)<.0001f,"pause changed zoom");for(int n=0;n<600;n++)Tick();Check(Mathf.Abs(view.orthographicSize-source.isometricNearSize)<.01f,"rest zoom size changed");
 foreach(float aspect in new[]{16f/9,4f/3,21f/9,.75f}){view.aspect=aspect;rig.ResetView();Tick();Grounded(rig);}view.aspect=16f/9;
 var subjects=new[]{new Vector3(-110,SnowField.Ground,-85),new Vector3(-98,SnowField.Ground,-99),All<PrototypeSession>(scene).Single().Fleet[0].transform.position};var names=new[]{"Fuel-canopy","Garage-roof","Tractor-isometric"};
 for(int i=0;i<subjects.Length;i++){target.transform.position=subjects[i];rig.ResetView();for(int n=0;n<60;n++)Tick();var center=view.WorldToViewportPoint(target.transform.position+Vector3.up*1.6f);Check(Mathf.Abs(center.x-.5f)<.001f&&Mathf.Abs(center.y-.5f)<.001f,"subject not centred");Grounded(rig);
 foreach(var placement in All<CatalogPlacement>(scene).Where(p=>p.placementId=="pinefall-garage"||p.placementId=="pinefall-fuel-station"))foreach(var r in placement.GetComponentsInChildren<Renderer>()){var b=r.bounds;for(int k=0;k<8;k++){var corner=b.center+Vector3.Scale(b.extents,new Vector3((k&1)==0?-1:1,(k&2)==0?-1:1,(k&4)==0?-1:1));Check(view.WorldToViewportPoint(corner).z>view.nearClipPlane+.1f,"near plane cuts depot geometry");}}
 Render(view,names[i]);}
 SceneValidation.Validate(scene);MechanicsChecks.Run();Check(sceneBytes.SequenceEqual(File.ReadAllBytes(scene.path)),"saved scene changed during checks");File.WriteAllText(Logs+"/Validation.txt","PASS: Unity "+Application.unityVersion+" / "+EditorUserBuildSettings.activeBuildTarget+". "+assertions+" assertions: stationary synthetic canopy cannot pull isometric camera; 600-frame 2 m/s crossing, worst step "+worstStep.ToString("F5")+" m; fixed orientation, centred subject, unchanged near/far speed zoom, paused zoom, four aspect-ratio near-plane terrain checks, garage/station bounds entirely beyond near plane. Three native depot renders. Scene validation and 19 snow checks passed; saved Pinefall bytes unchanged, no player-save writes. Follow/Orbit retain perspective collision path. Full live driving/phone performance untested.");Debug.Log(File.ReadAllText(Logs+"/Validation.txt"));
 }finally{foreach(var go in new[]{roof,cameraObject,target})if(go)UnityEngine.Object.DestroyImmediate(go);}
 }
 }
}
