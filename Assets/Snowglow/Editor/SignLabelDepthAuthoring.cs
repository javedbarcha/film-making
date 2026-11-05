using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
namespace Snowglow.Editor {
 public static class SignLabelDepthAuthoring {
  const string MaterialPath="Assets/Snowglow/Catalog/Sources/AlpineSignBoards/WorldSignText.mat", Request="Logs/FixSignLabelDepth.request", Logs="Logs/AlpineSignBoards/LabelDepth";
  static T[] All<T>(Scene scene)where T:Component=>scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<T>(true)).ToArray();
  static bool Sign(CatalogPlacement p)=>p&&(p.assetId??"").StartsWith("alpine-sign-",StringComparison.Ordinal);
  static void Require(bool ok,string message){if(!ok)throw new InvalidOperationException("Sign text depth: "+message);}
  [InitializeOnLoadMethod]static void Register(){EditorApplication.update+=Pending;}
  static void Pending(){if(!File.Exists(Request)){EditorApplication.update-=Pending;return;}if(EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode)return;EditorApplication.update-=Pending;File.Delete(Request);try{Run();}catch(Exception e){Directory.CreateDirectory(Logs);File.WriteAllText(Logs+"/Validation.txt","FAIL: "+e);Debug.LogException(e);}}
  public static void Apply(Scene scene){
   var shader=Shader.Find("Snowglow/World Sign Text");Require(shader&&shader.isSupported&&!ShaderUtil.ShaderHasError(shader),"text shader unavailable");var material=AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);if(!material){material=new Material(shader){name="Sign captions | depth tested front faces"};AssetDatabase.CreateAsset(material,MaterialPath);}material.shader=shader;material.renderQueue=3000;
   foreach(var text in All<CatalogPlacement>(scene).Where(Sign).SelectMany(p=>p.GetComponentsInChildren<TextMesh>())){var binding=text.GetComponent<WorldSignLabel>();if(!binding)binding=text.gameObject.AddComponent<WorldSignLabel>();binding.caption=text;binding.material=material;binding.Refresh();EditorUtility.SetDirty(binding);}EditorUtility.SetDirty(material);
  }
  static Color32[] Render(Camera camera,string path){var rt=new RenderTexture(960,720,24);var previous=RenderTexture.active;Texture2D frame=null;try{camera.targetTexture=rt;camera.Render();RenderTexture.active=rt;frame=new Texture2D(960,720,TextureFormat.RGB24,false);frame.ReadPixels(new Rect(0,0,960,720),0,0);frame.Apply();if(path!=null)File.WriteAllBytes(path,frame.EncodeToPNG());return frame.GetPixels32();}finally{camera.targetTexture=null;RenderTexture.active=previous;rt.Release();UnityEngine.Object.DestroyImmediate(rt);if(frame)UnityEngine.Object.DestroyImmediate(frame);}}
  static int Diff(Color32[] a,Color32[] b)=>a.Zip(b,(x,y)=>Math.Abs(x.r-y.r)+Math.Abs(x.g-y.g)+Math.Abs(x.b-y.b)>20?1:0).Sum();
  [MenuItem("Snowglow/Asset intake/Fix edited sign text depth and verify front-back")]
  public static void Run(){
   Require(!EditorApplication.isPlayingOrWillChangePlaymode,"stop Play");Directory.CreateDirectory(Logs);var scene=SceneManager.GetActiveScene();Require(scene.path==SceneValidation.ScenePath,"open Pinefall");SceneValidation.Validate(scene);if(scene.isDirty)Require(EditorSceneManager.SaveScene(scene,Logs+"/Pinefall-unsaved-before.unity",true),"unsaved backup failed");if(!File.Exists(Logs+"/Pinefall-before.unity"))File.Copy(scene.path,Logs+"/Pinefall-before.unity");
   var poses=All<CatalogPlacement>(scene).Where(p=>!string.IsNullOrEmpty(p.placementId)).ToDictionary(p=>p.placementId,p=>p.transform.localToWorldMatrix.ToString("R"));var snow=All<SnowField>(scene).Single();var seedHash=AssetDatabase.GetAssetDependencyHash(AssetDatabase.GetAssetPath(snow.seed));Apply(scene);AssetDatabase.SaveAssets();SceneValidation.Validate(scene);EditorSceneManager.MarkSceneDirty(scene);Require(EditorSceneManager.SaveScene(scene),"save failed");scene=EditorSceneManager.OpenScene(SceneValidation.ScenePath);SceneValidation.Validate(scene);MechanicsChecks.Run();
   var report=new StringBuilder("PASS: Unity "+Application.unityVersion+" / "+EditorUserBuildSettings.activeBuildTarget+". Edited world captions use ZTest LEqual and backface culling, with an authored shared material and dynamic-font atlas binding.\n");
   foreach(var sign in All<CatalogPlacement>(scene).Where(Sign).Where(p=>p.GetComponentsInChildren<TextMesh>().Length>0)){
    var preview=new PreviewRenderUtility();try{var copy=UnityEngine.Object.Instantiate(sign.gameObject);copy.transform.SetPositionAndRotation(Vector3.zero,Quaternion.identity);preview.AddSingleGO(copy);var texts=copy.GetComponentsInChildren<TextMesh>();foreach(var label in copy.GetComponentsInChildren<WorldSignLabel>())label.Refresh();var anchor=copy.GetComponentsInChildren<AssetAnchor>().Single(a=>a.id=="label-face").transform;var center=CatalogValidation.BoundsOf(copy).center;preview.camera.orthographic=true;preview.camera.orthographicSize=Mathf.Max(CatalogValidation.BoundsOf(copy).extents.x,CatalogValidation.BoundsOf(copy).extents.y)*1.3f;preview.camera.aspect=4f/3f;preview.camera.nearClipPlane=.03f;preview.camera.farClipPlane=100;preview.camera.clearFlags=CameraClearFlags.SolidColor;preview.camera.backgroundColor=new Color(.24f,.35f,.4f);preview.ambientColor=new Color(.5f,.5f,.5f);preview.lights[0].intensity=1.1f;int[] differences=new int[2];
     for(int side=0;side<2;side++){var direction=-anchor.forward*(side==0?1:-1);preview.camera.transform.position=center+direction*12+Vector3.up;preview.camera.transform.LookAt(center);preview.lights[0].transform.rotation=preview.camera.transform.rotation;foreach(var t in texts)t.GetComponent<MeshRenderer>().enabled=true;preview.BeginStaticPreview(new Rect(0,0,960,720));preview.Render(true);var on=preview.EndStaticPreview();File.WriteAllBytes(Logs+"/"+sign.placementId+(side==0?"-front.png":"-back.png"),on.EncodeToPNG());var a=on.GetPixels32();foreach(var t in texts)t.GetComponent<MeshRenderer>().enabled=false;preview.BeginStaticPreview(new Rect(0,0,960,720));preview.Render(true);var off=preview.EndStaticPreview();differences[side]=Diff(a,off.GetPixels32());UnityEngine.Object.DestroyImmediate(on);UnityEngine.Object.DestroyImmediate(off);}
     Require(differences[0]>100,"front caption invisible "+sign.placementId);Require(sign.transform.Find("Presentation | rear service caption")?differences[1]>100:differences[1]==0,"unexpected rear caption visibility "+sign.placementId);report.AppendLine(sign.placementId+": front caption pixels "+differences[0]+"; rear caption pixels "+differences[1]);
    }finally{preview.Cleanup();}
   }
   foreach(var p in All<CatalogPlacement>(scene).Where(p=>poses.ContainsKey(p.placementId??"")))Require(poses[p.placementId]==p.transform.localToWorldMatrix.ToString("R"),"pose changed");Require(seedHash==AssetDatabase.GetAssetDependencyHash(AssetDatabase.GetAssetPath(All<SnowField>(scene).Single().seed)),"snow seed changed");report.AppendLine("Scene saved/reopened; scene/19 snow checks passed. Board geometry, placement and snow seed unchanged. Font texture rebuilds update only the authored binding. Phone driving/performance untested.");File.WriteAllText(Logs+"/Validation.txt",report.ToString());Debug.Log(report.ToString());
  }
 }
}
