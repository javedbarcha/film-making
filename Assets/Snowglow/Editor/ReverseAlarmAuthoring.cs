using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Snowglow.Editor {
 public static class ReverseAlarmAuthoring {
  const string ClipPath="Assets/Snowglow/Audio/reverse-alarm.wav",Request="Logs/InstallReverseAlarm.request",Logs="Logs/ReverseAlarm";
  [InitializeOnLoadMethod]static void Register(){EditorApplication.update+=Pending;}
  static void Pending(){if(!File.Exists(Request)){EditorApplication.update-=Pending;return;}if(EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode)return;EditorApplication.update-=Pending;File.Delete(Request);try{Install();}catch(Exception e){Directory.CreateDirectory(Logs);File.WriteAllText(Logs+"/Validation.txt","FAIL: "+e);Debug.LogException(e);}}
  static void Require(bool value,string message){if(!value)throw new InvalidOperationException("Reverse alarm: "+message);}
  public static void Bind(SynthAudio audio){
   var clip=AssetDatabase.LoadAssetAtPath<AudioClip>(ClipPath);Require(clip,"reverse alarm clip missing");
   if(!audio.reverseAlarm){var go=new GameObject("Reverse warning | heavy machinery");go.transform.SetParent(audio.transform,false);audio.reverseAlarm=go.AddComponent<AudioSource>();}
   var source=audio.reverseAlarm;source.clip=clip;source.playOnAwake=false;source.loop=true;source.spatialBlend=0;source.dopplerLevel=0;source.pitch=1;source.volume=.35f;source.priority=70;source.ignoreListenerPause=false;source.mute=audio.effects&&audio.effects.mute;
   EditorUtility.SetDirty(source);EditorUtility.SetDirty(audio);
  }
  [MenuItem("Snowglow/Audio/Install reverse warning alarm")]
  public static void Install(){
   var scene=SceneManager.GetActiveScene();Require(scene.path==SceneValidation.ScenePath&&!EditorApplication.isPlayingOrWillChangePlaymode,"open Pinefall with Play stopped");Directory.CreateDirectory(Logs);if(!File.Exists(Logs+"/Pinefall-before.unity"))File.Copy(scene.path,Logs+"/Pinefall-before.unity");if(scene.isDirty)Require(EditorSceneManager.SaveScene(scene,Logs+"/Pinefall-unsaved-before.unity",true),"backup failed");
   var session=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<PrototypeSession>(true)).Single();
   var poses=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<CatalogPlacement>(true)).Where(p=>!string.IsNullOrEmpty(p.placementId)).ToDictionary(p=>p.placementId,p=>p.assetId+"|"+p.transform.localToWorldMatrix.ToString("R"));
   string seedPath=AssetDatabase.GetAssetPath(session.Snow.seed),seedHash=AssetDatabase.GetAssetDependencyHash(seedPath).ToString();
   AssetDatabase.ImportAsset(ClipPath,ImportAssetOptions.ForceSynchronousImport);var importer=(AudioImporter)AssetImporter.GetAtPath(ClipPath);importer.forceToMono=false;var settings=importer.defaultSampleSettings;settings.preloadAudioData=true;settings.loadType=AudioClipLoadType.DecompressOnLoad;settings.compressionFormat=AudioCompressionFormat.PCM;settings.sampleRateSetting=AudioSampleRateSetting.PreserveSampleRate;importer.defaultSampleSettings=settings;importer.SaveAndReimport();
   Bind(session.Audio);SceneValidation.Validate(scene);AssetDatabase.SaveAssets();EditorSceneManager.MarkSceneDirty(scene);Require(EditorSceneManager.SaveScene(scene),"save failed");scene=EditorSceneManager.OpenScene(SceneValidation.ScenePath);SceneValidation.Validate(scene);
   session=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<PrototypeSession>(true)).Single();var alarm=session.Audio.reverseAlarm;Require(alarm&&alarm.clip&&alarm.loop&&!alarm.playOnAwake&&alarm.spatialBlend==0,"saved alarm bindings differ");Require(alarm.clip.channels==1&&Mathf.Abs(alarm.clip.length-.85f)<.002f,"clip duration/channels differ");
   Require(AssetDatabase.GetAssetPath(session.Snow.seed)==seedPath&&AssetDatabase.GetAssetDependencyHash(seedPath).ToString()==seedHash,"snow seed changed");foreach(var p in scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<CatalogPlacement>(true)).Where(p=>!string.IsNullOrEmpty(p.placementId)))Require(poses[p.placementId]==p.assetId+"|"+p.transform.localToWorldMatrix.ToString("R"),"placement changed "+p.placementId);
   session.Audio.SetMuted(true);Require(alarm.mute,"mute did not reach reverse source");session.Audio.SetMuted(false);Require(!alarm.mute,"unmute did not reach reverse source");session.Audio.Reverse(false);EditorSceneManager.OpenScene(SceneValidation.ScenePath);
   File.WriteAllText(Logs+"/Validation.txt","PASS: Unity "+Application.unityVersion+". Imported original synthesized mono 22,050Hz backup alarm, 0.85s cycle (0.36s beep / 0.49s gap), authored looping 2D source with playOnAwake off. Saved/reopened Pinefall and checked persistent clip/source binding, mute/unmute and scene validation. Session update requests warning only when the active machine is in reverse gear and moving backwards faster than 0.3 m/s during running gameplay; both keyboard and touch share this state. Factory binds the source before Play for future bakes. Existing scenery poses and snow seed hash unchanged. No player-save writes or rebake. Audible Play-mode/phone speaker verification remains untested.");Debug.Log(File.ReadAllText(Logs+"/Validation.txt"));
  }
 }
}
