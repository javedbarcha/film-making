using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Snowglow.Editor {
 public static class ExhaustSmokeAuthoring {
  const string Folder="Assets/Snowglow/Generated/ExhaustSmoke";
  const string MeshPath=Folder+"/LowPolySmokePuff.asset";
  const string MaterialPath=Folder+"/SemiTransparentGreySmoke.mat";
  const string Request="Logs/InstallExhaustSmoke.request",Logs="Logs/ExhaustSmoke";
  const string TractorPrefab=CatalogPreparation.Root+"/Prefabs/GoldenTractor-Starter.prefab";
  const string LoaderPrefab=CatalogPreparation.Root+"/Prefabs/ImportedSnowLoader.prefab";
  static int assertions;

  static void Check(bool condition,string message){if(!condition)throw new InvalidOperationException("Exhaust smoke: "+message);assertions++;}
  [InitializeOnLoadMethod]static void Register(){EditorApplication.update+=Pending;}
  static void Pending(){
   if(!File.Exists(Request)){EditorApplication.update-=Pending;return;}
   if(EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode)return;
   EditorApplication.update-=Pending;File.Delete(Request);
   try{Install();}catch(Exception e){Directory.CreateDirectory(Logs);File.WriteAllText(Logs+"/Validation.txt","FAIL: "+e);Debug.LogException(e);}
  }

  static void EnsureAssets(){
   if(!AssetDatabase.IsValidFolder(Folder)){Directory.CreateDirectory(Folder);AssetDatabase.Refresh();}
   var mesh=AssetDatabase.LoadAssetAtPath<Mesh>(MeshPath);
   if(!mesh){mesh=CreateLowPolyPuff();AssetDatabase.CreateAsset(mesh,MeshPath);}
   var shader=Shader.Find("Universal Render Pipeline/Particles/Unlit");
   Check(shader,"URP particle unlit shader is unavailable");
   var material=AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
   if(!material){material=new Material(shader){name="Semi-transparent grey exhaust smoke"};AssetDatabase.CreateAsset(material,MaterialPath);}
   material.shader=shader;material.SetColor("_BaseColor",Color.white);
   material.SetFloat("_Surface",1);material.SetFloat("_Blend",0);
   material.SetFloat("_SrcBlend",(float)BlendMode.SrcAlpha);material.SetFloat("_DstBlend",(float)BlendMode.OneMinusSrcAlpha);
   material.SetFloat("_SrcBlendAlpha",(float)BlendMode.One);material.SetFloat("_DstBlendAlpha",(float)BlendMode.OneMinusSrcAlpha);
   material.SetFloat("_ZWrite",0);material.SetFloat("_Cull",(float)CullMode.Back);
   material.SetOverrideTag("RenderType","Transparent");material.renderQueue=(int)RenderQueue.Transparent;
   material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");material.DisableKeyword("_ALPHAPREMULTIPLY_ON");material.DisableKeyword("_ALPHAMODULATE_ON");
   EditorUtility.SetDirty(material);AssetDatabase.SaveAssets();
   Check(mesh&&AssetDatabase.Contains(mesh)&&mesh.triangles.Length/3==80,"saved low-poly puff mesh");
   Check(material&&material.IsKeywordEnabled("_SURFACE_TYPE_TRANSPARENT")&&Mathf.Approximately(material.GetFloat("_ZWrite"),0),"saved transparent smoke material");
  }

  static Mesh CreateLowPolyPuff(){
   float t=(1+Mathf.Sqrt(5))/2;
   var vertices=new List<Vector3>{
    new Vector3(-1,t,0),new Vector3(1,t,0),new Vector3(-1,-t,0),new Vector3(1,-t,0),
    new Vector3(0,-1,t),new Vector3(0,1,t),new Vector3(0,-1,-t),new Vector3(0,1,-t),
    new Vector3(t,0,-1),new Vector3(t,0,1),new Vector3(-t,0,-1),new Vector3(-t,0,1)
   };
   int[] faces={
    0,11,5,0,5,1,0,1,7,0,7,10,0,10,11,1,5,9,5,11,4,11,10,2,10,7,6,7,1,8,
    3,9,4,3,4,2,3,2,6,3,6,8,3,8,9,4,9,5,2,4,11,6,2,10,8,6,7,9,8,1
   };
   var triangles=new List<int>();var midpoints=new Dictionary<long,int>();
   int Midpoint(int a,int b){
    int lo=Math.Min(a,b),hi=Math.Max(a,b);long key=((long)lo<<32)|(uint)hi;
    if(midpoints.TryGetValue(key,out int index))return index;
    index=vertices.Count;vertices.Add(((vertices[a]+vertices[b])*.5f).normalized);midpoints.Add(key,index);return index;
   }
   for(int i=0;i<faces.Length;i+=3){
    int a=faces[i],b=faces[i+1],c=faces[i+2],ab=Midpoint(a,b),bc=Midpoint(b,c),ca=Midpoint(c,a);
    triangles.AddRange(new[]{a,ab,ca,b,bc,ab,c,ca,bc,ab,bc,ca});
   }
   for(int i=0;i<vertices.Count;i++)vertices[i]=vertices[i].normalized;
   var mesh=new Mesh{name="Low-poly exhaust smoke puff"};mesh.SetVertices(vertices);mesh.SetTriangles(triangles,0);mesh.RecalculateNormals();mesh.RecalculateBounds();return mesh;
  }

  // Source +X-forward/Z-up maps to Unity (-sourceY, sourceZ, sourceX).
  // Resolve through the fitted body transform rather than assuming root space.
  public static Vector3 OutletPosition(GameObject root,bool loader){
   MeshFilter body=null;foreach(var candidate in root.GetComponentsInChildren<MeshFilter>(true))if(candidate.name=="body"){if(body)throw new InvalidOperationException("Multiple body meshes");body=candidate;}
   if(!body)throw new InvalidOperationException("Missing source body mesh");
   var sourceTip=loader?new Vector3(-.38f,2.75f,-1.3f):new Vector3(-.42f,2.795f,.46f);
   return root.transform.InverseTransformPoint(body.transform.TransformPoint(sourceTip+Vector3.up*.005f));
  }

  public static void Configure(GameObject root,bool loader){
   if(!root)throw new ArgumentNullException(nameof(root));
   EnsureAssets();
   var machine=root.GetComponent<MachineController>();
   Check(machine,"machine controller missing from "+root.name);
   var existing=root.GetComponentsInChildren<ExhaustSmokeEffect>(true);
   Check(existing.Length<=1,"duplicate smoke effects on "+root.name);
   var effect=existing.Length==1?existing[0]:new GameObject("Exhaust smoke | low-poly authored").AddComponent<ExhaustSmokeEffect>();
   effect.transform.SetParent(root.transform,false);
   effect.name="Exhaust smoke | low-poly authored";effect.gameObject.layer=root.layer;effect.gameObject.SetActive(true);
   // Outlet positions follow the supplied exhaust caps after Unity's axis conversion.
   effect.transform.localPosition=OutletPosition(root,loader);
   effect.transform.localRotation=Quaternion.identity;effect.transform.localScale=Vector3.one;
   effect.machine=machine;
   var system=effect.GetComponent<ParticleSystem>();if(!system)system=effect.gameObject.AddComponent<ParticleSystem>();
   effect.particles=system;system.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
   var main=system.main;main.playOnAwake=false;main.loop=true;main.duration=5;main.maxParticles=24;
   main.startLifetime=new ParticleSystem.MinMaxCurve(1.7f,2.3f);main.startSpeed=new ParticleSystem.MinMaxCurve(.08f,.24f);
   main.startSize=new ParticleSystem.MinMaxCurve(.09f,.14f);main.startColor=Color.white;
   main.gravityModifier=0;main.simulationSpace=ParticleSystemSimulationSpace.World;main.cullingMode=ParticleSystemCullingMode.Automatic;
   var emission=system.emission;emission.enabled=true;emission.rateOverTime=0;
   var shape=system.shape;shape.enabled=true;shape.shapeType=ParticleSystemShapeType.Cone;shape.angle=9;shape.radius=.025f;shape.radiusThickness=1;
   var color=system.colorOverLifetime;color.enabled=true;
   var gradient=new Gradient();
   gradient.SetKeys(
    new[]{new GradientColorKey(new Color(.34f,.35f,.37f),0),new GradientColorKey(new Color(.48f,.49f,.5f),1)},
    new[]{new GradientAlphaKey(.46f,0),new GradientAlphaKey(.28f,.45f),new GradientAlphaKey(0,1)});
   color.color=new ParticleSystem.MinMaxGradient(gradient);
   var size=system.sizeOverLifetime;size.enabled=true;size.size=new ParticleSystem.MinMaxCurve(1,new AnimationCurve(new Keyframe(0,.65f),new Keyframe(.45f,1.15f),new Keyframe(1,1.9f)));
   var velocity=system.velocityOverLifetime;velocity.enabled=true;velocity.space=ParticleSystemSimulationSpace.World;
   ExhaustRiseAuthoring.ConfigureMotion(system);
   var renderer=system.GetComponent<ParticleSystemRenderer>();renderer.renderMode=ParticleSystemRenderMode.Mesh;
   renderer.mesh=AssetDatabase.LoadAssetAtPath<Mesh>(MeshPath);renderer.sharedMaterial=AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
   renderer.alignment=ParticleSystemRenderSpace.Local;renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=false;
   renderer.lightProbeUsage=LightProbeUsage.Off;renderer.reflectionProbeUsage=ReflectionProbeUsage.Off;renderer.enableGPUInstancing=true;
   Check(renderer.mesh&&renderer.sharedMaterial,"persistent particle mesh/material assigned");
   EditorUtility.SetDirty(system);EditorUtility.SetDirty(renderer);EditorUtility.SetDirty(effect);EditorUtility.SetDirty(root);
  }

  static void AuthorPrefab(string path,bool loader){
   var root=PrefabUtility.LoadPrefabContents(path);
   try{Configure(root,loader);PrefabUtility.SaveAsPrefabAsset(root,path);}
   finally{PrefabUtility.UnloadPrefabContents(root);}
  }

  static void ValidatePrefab(string path,bool loader){
   var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(path);Check(prefab,"saved machinery prefab missing: "+path);
   var machine=prefab.GetComponent<MachineController>();var effect=prefab.GetComponentInChildren<ExhaustSmokeEffect>(true);
   Check(machine&&effect&&effect.machine==machine&&effect.particles,"saved exhaust component binding: "+path);
   var ps=effect.particles;var main=ps.main;var renderer=ps.GetComponent<ParticleSystemRenderer>();
   Check(!main.playOnAwake&&main.simulationSpace==ParticleSystemSimulationSpace.World&&main.maxParticles==24,"saved particle runtime settings: "+path);
   Check(renderer&&renderer.renderMode==ParticleSystemRenderMode.Mesh&&renderer.mesh==AssetDatabase.LoadAssetAtPath<Mesh>(MeshPath),"saved low-poly particle renderer: "+path);
   Check(renderer.sharedMaterial==AssetDatabase.LoadAssetAtPath<Material>(MaterialPath)&&renderer.sharedMaterial.IsKeywordEnabled("_SURFACE_TYPE_TRANSPARENT"),"saved smoke transparency: "+path);
   Check(Vector3.Distance(effect.transform.localPosition,OutletPosition(prefab,loader))<.001f,"smoke is at exhaust outlet: "+path);
   Check(prefab.GetComponentsInChildren<ExhaustSmokeEffect>(true).Length==1&&effect.GetComponentsInChildren<Collider>(true).Length==0,"single non-colliding exhaust effect: "+path);
  }

  [MenuItem("Snowglow/Effects/Install low-poly exhaust smoke")]
  public static void Install(){
   assertions=0;Check(!EditorApplication.isPlayingOrWillChangePlaymode,"stop Play before authoring");
   var scene=SceneManager.GetActiveScene();Check(scene.path==SceneValidation.ScenePath,"open Pinefall before authoring");
   Check(!scene.isDirty,"save or discard unrelated Pinefall edits before authoring");
   Directory.CreateDirectory(Logs);
   if(!File.Exists(Logs+"/Pinefall-before.unity"))EditorSceneManager.SaveScene(scene,Logs+"/Pinefall-before.unity",true);
   EnsureAssets();AuthorPrefab(TractorPrefab,false);AuthorPrefab(LoaderPrefab,true);AssetDatabase.SaveAssets();AssetDatabase.Refresh();
   ValidatePrefab(TractorPrefab,false);ValidatePrefab(LoaderPrefab,true);
   Check(ExhaustSmokeEffect.CalculateEmissionRate(true,true,true,10,100,0,0)>0,"idle smoke rate");
   Check(ExhaustSmokeEffect.CalculateEmissionRate(false,true,true,10,100,1,1)==0,"inactive machine smoke gate");
   Check(ExhaustSmokeEffect.CalculateEmissionRate(true,false,true,10,100,1,1)==0,"paused smoke gate");
   Check(ExhaustSmokeEffect.CalculateEmissionRate(true,true,false,10,100,1,1)==0,"unselected machine smoke gate");
   Check(ExhaustSmokeEffect.CalculateEmissionRate(true,true,true,0,100,1,1)==0&&ExhaustSmokeEffect.CalculateEmissionRate(true,true,true,10,0,1,1)==0,"unavailable machine smoke gate");
   float idle=ExhaustSmokeEffect.CalculateEmissionRate(true,true,true,10,100,0,0);
   float working=ExhaustSmokeEffect.CalculateEmissionRate(true,true,true,10,100,1,1);
   Check(working>idle&&working<=9.6f,"throttle/load raises bounded smoke rate");
   SceneValidation.Validate(scene);MechanicsChecks.Run();
   File.WriteAllText(Logs+"/Validation.txt","PASS: Unity "+Application.unityVersion+" / "+EditorUserBuildSettings.activeBuildTarget+". Both saved/reopened machinery prefabs have one preauthored non-colliding mesh-particle exhaust effect at their supplied exhaust-stack outlets. The 80-triangle puff uses a shared URP transparent grey material, alpha/size fade, upward drift and bounded particle count. Smoke emits at a low idle rate, increases with throttle/load, and clears for pause, parked/unselected machines, empty fuel or zero condition. Scene validation and 19 snow-ledger checks passed. No source model, snow data, machine physics, player saves or district layout changed; no runtime effect/material/mesh factory added. Rendered appearance, moving vehicle Play testing, Android packaging and Galaxy A07 cost remain untested.");
   Debug.Log(File.ReadAllText(Logs+"/Validation.txt"));
  }
 }
}
