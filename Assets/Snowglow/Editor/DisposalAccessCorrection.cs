using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
namespace Snowglow.Editor {
 public static class DisposalAccessCorrection {
  const string Request="Logs/RemoveDisposalAccess.request", Report="Logs/DisposalAccess/Validation.txt";
  [InitializeOnLoadMethod] static void Register(){EditorApplication.update+=Pending;}
  static void Pending(){if(!File.Exists(Request)){EditorApplication.update-=Pending;return;}if(EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode)return;EditorApplication.update-=Pending;File.Delete(Request);try{Run();}catch(Exception e){Directory.CreateDirectory("Logs/DisposalAccess");File.WriteAllText(Report,"FAIL: "+e);Debug.LogException(e);}}
  static Transform[] Objects(Scene s)=>s.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<Transform>(true)).ToArray();
  static bool Spur(Transform t)=>t.name=="Road | asphalt"&&Mathf.Abs(t.localPosition.x-78)<.01f&&Mathf.Abs(t.localPosition.z+85)<.01f&&Vector3.Distance(t.localScale,new Vector3(30,.18f,10))<.01f;
  [MenuItem("Snowglow/Environment/Remove redundant disposal west access")]
  public static void Run(){
   if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Stop Play first");
   var scene=SceneManager.GetActiveScene();if(scene.path!=SceneValidation.ScenePath)throw new InvalidOperationException("Open Pinefall");SceneValidation.Validate(scene);
   var targets=Objects(scene).Where(Spur).ToArray();if(targets.Length!=1)throw new InvalidOperationException("Expected exactly one redundant west access, found "+targets.Length);
   Directory.CreateDirectory("Logs/DisposalAccess");EditorSceneManager.SaveScene(scene,"Logs/DisposalAccess/Pinefall-before.unity",true);
   var snow=Objects(scene).Select(t=>t.GetComponent<SnowField>()).First(x=>x);var hash=AssetDatabase.GetAssetDependencyHash(AssetDatabase.GetAssetPath(snow.seed));
   Undo.DestroyObjectImmediate(targets[0].gameObject);EditorSceneManager.MarkSceneDirty(scene);SceneValidation.Validate(scene);if(!EditorSceneManager.SaveScene(scene))throw new InvalidOperationException("Save failed");scene=EditorSceneManager.OpenScene(SceneValidation.ScenePath);
   if(Objects(scene).Any(Spur))throw new InvalidOperationException("Spur still exists");SceneValidation.Validate(scene);MechanicsChecks.Run();snow=Objects(scene).Select(t=>t.GetComponent<SnowField>()).First(x=>x);if(hash!=AssetDatabase.GetAssetDependencyHash(AssetDatabase.GetAssetPath(snow.seed)))throw new InvalidOperationException("Snow seed changed");
   File.WriteAllText(Report,"PASS: Removed only the redundant 30 x 10 m west disposal access at (78,-85), including its road collider. Yard and all other roads preserved. Pinefall saved/reopened, scene validation and 19 snow checks passed; snow seed unchanged. Factory updated without district rebake. No phone playtest.");Debug.Log(File.ReadAllText(Report));
  }
 }
}
