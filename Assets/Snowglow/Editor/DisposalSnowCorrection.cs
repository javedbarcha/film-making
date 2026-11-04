using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
namespace Snowglow.Editor {
 public static class DisposalSnowCorrection {
 const string Request="Logs/RemoveDisposalSnow.request", Folder="Assets/Snowglow/Catalog/SceneData", Report="Logs/DisposalAccess/SnowValidation.txt";
 [InitializeOnLoadMethod]static void Register(){EditorApplication.update+=Pending;}
 static void Pending(){if(!File.Exists(Request)){EditorApplication.update-=Pending;return;}if(EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode)return;EditorApplication.update-=Pending;File.Delete(Request);try{Run();}catch(Exception e){File.WriteAllText(Report,"FAIL: "+e);Debug.LogException(e);}}
 static SnowField Snow(Scene s)=>s.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<SnowField>()).Single();
 public static void Run(){
 var scene=SceneManager.GetActiveScene();if(scene.path!=SceneValidation.ScenePath)throw new InvalidOperationException("Open Pinefall");SceneValidation.Validate(scene);Directory.CreateDirectory("Logs/DisposalAccess");EditorSceneManager.SaveScene(scene,"Logs/DisposalAccess/Pinefall-before-snow.unity",true);
 var snow=Snow(scene);var original=snow.seed;var revised=UnityEngine.Object.Instantiate(original);revised.name="Pinefall | redundant disposal snow strip removed";var grid=revised.grid;var ids=new HashSet<int>();double removed=0;int count=0;
 for(int i=0;i<grid.depth.Length;i++){float x=grid.X(i),z=grid.Z(i);if(x<=70||x>=95||Mathf.Abs(z+85)>=5||grid.depth[i]<=0)continue;removed+=grid.depth[i]*SnowVolumeGrid.Area;grid.depth[i]=0;grid.credit[i]=0;ids.Add((i/SnowVolumeGrid.Size/SnowField.Chunk)*SnowField.Across+(i%SnowVolumeGrid.Size/SnowField.Chunk));count++;}
 if(count==0)throw new InvalidOperationException("No seeded strip found");grid.initialVolume=original.grid.initialVolume-(float)removed;
 var flags=BindingFlags.NonPublic|BindingFlags.Instance;typeof(SnowField).GetField("cube",flags).SetValue(snow,snow.GetComponent<MeshFilter>().sharedMesh);var chunks=(Dictionary<int,MeshFilter>)typeof(SnowField).GetField("chunks",flags).GetValue(snow);chunks.Clear();snow.Grid=grid;
 string path=AssetDatabase.GenerateUniqueAssetPath(Folder+"/DisposalAccessSnowRevision.asset");AssetDatabase.CreateAsset(revised,path);
 foreach(Transform child in snow.transform){int id=int.Parse(child.name.Split('|')[0]);if(!ids.Contains(id))continue;var mf=child.GetComponent<MeshFilter>();var mc=child.GetComponent<MeshCollider>();mf.sharedMesh=UnityEngine.Object.Instantiate(mf.sharedMesh);mc.sharedMesh=null;chunks[id]=mf;typeof(SnowField).GetMethod("Rebuild",flags).Invoke(snow,new object[]{id});AssetDatabase.AddObjectToAsset(mf.sharedMesh,revised);if(mc.sharedMesh)AssetDatabase.AddObjectToAsset(mc.sharedMesh,revised);}
 snow.seed=revised;EditorUtility.SetDirty(revised);EditorUtility.SetDirty(snow);AssetDatabase.SaveAssets();SceneValidation.Validate(scene);EditorSceneManager.MarkSceneDirty(scene);if(!EditorSceneManager.SaveScene(scene))throw new InvalidOperationException("Save failed");scene=EditorSceneManager.OpenScene(SceneValidation.ScenePath);SceneValidation.Validate(scene);MechanicsChecks.Run();var after=Snow(scene).seed.grid;
 for(int i=0;i<after.depth.Length;i++){bool area=after.X(i)>70&&after.X(i)<95&&Mathf.Abs(after.Z(i)+85)<5;if(!area&&(after.depth[i]!=original.grid.depth[i]||after.credit[i]!=original.grid.credit[i]))throw new InvalidOperationException("Snow outside strip changed");if(area&&after.depth[i]!=0)throw new InvalidOperationException("Strip remains");}
 File.WriteAllText(Report,"PASS: Removed "+count+" seeded cells / "+removed.ToString("F3")+" m3 of authored starting snow from redundant disposal link only; revised initialVolume accordingly, no gameplay disposal. Rebuilt only "+ids.Count+" affected chunks with new mesh assets. Original seed/meshes retained, outside snow unchanged. Pinefall saved/reopened; scene validation and 19 snow checks passed. Player saves untouched; no phone playtest.");Debug.Log(File.ReadAllText(Report));
 }
 }
}
