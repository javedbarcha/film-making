using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Snowglow.Editor {
 public static class ExhaustRiseAuthoring {
 const string Logs="Logs/ExhaustSmoke/Rise",Request="Logs/ExhaustRise.request";static int checks;
 static void Check(bool ok,string message){if(!ok)throw new InvalidOperationException(message);checks++;}
 public static void ConfigureMotion(ParticleSystem ps){
  ps.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
  var main=ps.main;main.startSpeed=0;
  var v=ps.velocityOverLifetime;v.enabled=true;v.space=ParticleSystemSimulationSpace.World;
  // First 42% of life is a vertical column: 0.46-0.63 m over the saved lifetime range.
  var rise=new AnimationCurve(new Keyframe(0,.65f),new Keyframe(.42f,.65f),new Keyframe(1,.08f));
  // Unity requires matching curve modes across all three velocity axes.
  v.y=new ParticleSystem.MinMaxCurve(1,rise,rise);
  v.x=new ParticleSystem.MinMaxCurve(1,Drift(-.14f),Drift(.14f));
  v.z=new ParticleSystem.MinMaxCurve(1,Drift(-.14f),Drift(.14f));
  EditorUtility.SetDirty(ps);
 }
 static AnimationCurve Drift(float end)=>new AnimationCurve(new Keyframe(0,0),new Keyframe(.42f,0),new Keyframe(1,end));
 [InitializeOnLoadMethod]static void Register(){EditorApplication.update+=Pending;}
 static void Pending(){if(!File.Exists(Request)){EditorApplication.update-=Pending;return;}if(EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode)return;EditorApplication.update-=Pending;File.Delete(Request);try{Run();}catch(Exception e){Directory.CreateDirectory(Logs);File.WriteAllText(Logs+"/Validation.txt","FAIL: "+e);Debug.LogException(e);}}
 static T[] All<T>(Scene s)where T:Component=>s.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<T>(true)).ToArray();
 static void Validate(GameObject root){
  var e=root.GetComponentInChildren<ExhaustSmokeEffect>(true);var ps=e.particles;
  Check(Vector3.Distance(e.transform.localPosition,ExhaustSmokeAuthoring.OutletPosition(root,root.GetComponent<MachineController>().definition.loader))<.001f,"outlet changed");
  Check(ps.main.startSpeed.constant==0&&ps.velocityOverLifetime.space==ParticleSystemSimulationSpace.World,"saved rise settings");
  Check(ps.velocityOverLifetime.enabled&&Mathf.Abs(ps.velocityOverLifetime.y.Evaluate(.1f)-.65f)<.001f,"saved upward velocity");
  Check(ps.main.maxParticles>=Mathf.CeilToInt(9.6f*ps.main.startLifetime.constantMax),"particle capacity");
  var copy=new GameObject("Exhaust trajectory check");copy.AddComponent<ParticleSystem>();try{
   var effect=copy.GetComponent<ExhaustSmokeEffect>();if(effect)UnityEngine.Object.DestroyImmediate(effect);
   var test=copy.GetComponent<ParticleSystem>();EditorUtility.CopySerialized(ps,test);ConfigureMotion(test);var em=test.emission;em.enabled=false;var testMain=test.main;testMain.cullingMode=ParticleSystemCullingMode.AlwaysSimulate;
   test.useAutoRandomSeed=false;test.randomSeed=41;
   var particles=new ParticleSystem.Particle[24];float drift=0;
   foreach(float life in new[]{1.7f,2f,2.3f})for(uint seed=1;seed<=8;seed++){
    test.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
    test.Simulate(0,true,true,false);test.Play(true);test.Emit(new ParticleSystem.EmitParams{position=copy.transform.position,velocity=Vector3.zero,startLifetime=life,randomSeed=seed,applyShapeToPosition=false},1);
    for(int step=0;step<42;step++)test.Simulate(life*.01f,true,false,false);Check(test.GetParticles(particles)==1,"rise particle missing at life="+life+" seed="+seed+" count="+test.particleCount);
    var delta=particles[0].position-copy.transform.position;
    Check(delta.y>.44f&&delta.y<.65f,"rise height "+delta.y+" remaining="+particles[0].remainingLifetime+" speed="+test.main.simulationSpeed+" active="+test.gameObject.activeInHierarchy+" velocity="+particles[0].totalVelocity+" enabled="+test.velocityOverLifetime.enabled+" y="+test.velocityOverLifetime.y.Evaluate(.1f)+" modifier="+test.velocityOverLifetime.speedModifier.Evaluate(.1f)+" limit="+test.limitVelocityOverLifetime.enabled+" force="+test.forceOverLifetime.enabled+" scale="+copy.transform.lossyScale+" custom="+test.main.customSimulationSpace);
    Check(new Vector2(delta.x,delta.z).magnitude<.005f,"drift started before rise");
    // Integrate later motion in small steps, as in moving gameplay.
    for(int n=0;n<40;n++)test.Simulate(life*.01f,true,false,false);
    Check(test.GetParticles(particles)==1,"drift particle missing");delta=particles[0].position-copy.transform.position;
    drift+=new Vector2(delta.x,delta.z).magnitude;Check(delta.y>.5f,"vertical momentum lost");
   }
   Check(drift>.05f,"no lateral drift after rise");
  }finally{UnityEngine.Object.DestroyImmediate(copy);}
 }
 [MenuItem("Snowglow/Effects/Apply doubled smoke and vertical rise")]
 public static void Run(){
  checks=0;Directory.CreateDirectory(Logs);var scene=SceneManager.GetActiveScene();Check(scene.path==SceneValidation.ScenePath&&!EditorApplication.isPlayingOrWillChangePlaymode,"open Pinefall with Play stopped");
  EditorSceneManager.SaveScene(scene,Logs+"/Pinefall-before.unity",true);
  var session=All<PrototypeSession>(scene).Single();string seed=AssetDatabase.GetAssetPath(session.Snow.seed);var hash=AssetDatabase.GetAssetDependencyHash(seed);
  var poses=All<CatalogPlacement>(scene).ToDictionary(p=>GlobalObjectId.GetGlobalObjectIdSlow(p).ToString(),p=>p.transform.localToWorldMatrix.ToString("R"));var fleet=session.Fleet.Select(m=>m.transform.localToWorldMatrix.ToString("R")).ToArray();
  foreach(var path in new[]{StarterPlowAuthoring.PrefabPath,"Assets/Snowglow/Catalog/Prefabs/ImportedSnowLoader.prefab"}){
   if(!File.Exists(Logs+"/"+Path.GetFileName(path)))File.Copy(path,Logs+"/"+Path.GetFileName(path));
   var root=PrefabUtility.LoadPrefabContents(path);try{ConfigureMotion(root.GetComponentInChildren<ExhaustSmokeEffect>(true).particles);Validate(root);PrefabUtility.SaveAsPrefabAsset(root,path);}finally{PrefabUtility.UnloadPrefabContents(root);}
   Validate(AssetDatabase.LoadAssetAtPath<GameObject>(path));
  }
  foreach(var m in session.Fleet){var ps=m.GetComponentInChildren<ExhaustSmokeEffect>(true).particles;ConfigureMotion(ps);PrefabUtility.RecordPrefabInstancePropertyModifications(ps);}
  for(int t=0;t<=10;t++)for(int l=0;l<=10;l++){float old=Mathf.Min(4.8f,.5f+2.7f*t/10+1.6f*l/10);Check(Mathf.Abs(ExhaustSmokeEffect.CalculateEmissionRate(true,true,true,10,100,t/10f,l/10f)-old*2)<.00001f,"rate not doubled");}
  Check(ExhaustSmokeEffect.CalculateEmissionRate(false,true,true,10,100,1,1)==0&&ExhaustSmokeEffect.CalculateEmissionRate(true,false,true,10,100,1,1)==0&&ExhaustSmokeEffect.CalculateEmissionRate(true,true,false,10,100,1,1)==0&&ExhaustSmokeEffect.CalculateEmissionRate(true,true,true,0,100,1,1)==0&&ExhaustSmokeEffect.CalculateEmissionRate(true,true,true,10,0,1,1)==0,"state gates");
  AssetDatabase.SaveAssets();EditorSceneManager.MarkSceneDirty(scene);Check(EditorSceneManager.SaveScene(scene),"scene save failed");scene=EditorSceneManager.OpenScene(SceneValidation.ScenePath);
  SceneValidation.Validate(scene);MechanicsChecks.Run();session=All<PrototypeSession>(scene).Single();foreach(var m in session.Fleet)Validate(m.gameObject);
  Check(seed==AssetDatabase.GetAssetPath(session.Snow.seed)&&hash==AssetDatabase.GetAssetDependencyHash(seed),"snow changed");Check(fleet.SequenceEqual(session.Fleet.Select(m=>m.transform.localToWorldMatrix.ToString("R"))),"fleet moved");foreach(var p in All<CatalogPlacement>(scene))Check(poses[GlobalObjectId.GetGlobalObjectIdSlow(p).ToString()]==p.transform.localToWorldMatrix.ToString("R"),"scenery moved");
  File.WriteAllText(Logs+"/Validation.txt","PASS: Unity "+Application.unityVersion+" / "+EditorUserBuildSettings.activeBuildTarget+"; "+checks+" assertions. Quantity doubled again: idle 1/s, maximum 9.6/s. Native particles rise 0.46-0.63 m before lateral drift; 24 seeds/lifetimes tested per prefab/scene instance. Both prefabs saved/reloaded and Pinefall saved/reopened. Scene validation and 19 snow checks passed; snow hash and fleet/catalog poses unchanged. Existing size, lifetime, color, material, mesh and source outlet positions retained. No player-save writes. Live driving, Android packaging and Galaxy A07 performance untested.");Debug.Log(File.ReadAllText(Logs+"/Validation.txt"));
 }
 }
}

