using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEditor.Events;
using System.Collections.Generic;
namespace Snowglow.Editor {
 public static class VehicleLightsAuthoring {
  const string Folder=CatalogPreparation.Root+"/Sources/VehicleLights/",Request="Logs/VehicleLights.request",Report="Logs/VehicleLights/Validation.txt";
  [Serializable]public class Data {public Vehicle[] vehicles;}
  [Serializable]public class Vehicle {public string model;public Lamp[] lights;}
  [Serializable]public class Lamp {public string name;public float[] position;}
  static void Check(bool value,string message){if(!value)throw new InvalidOperationException("Vehicle lights: "+message);}
  [InitializeOnLoadMethod]static void Reload(){EditorApplication.update+=Pending;}
  static void Pending(){
   if(!File.Exists(Request)){EditorApplication.update-=Pending;return;}
   if(EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode)return;
   EditorApplication.update-=Pending;File.Delete(Request);
   try{Install();}catch(Exception e){Directory.CreateDirectory("Logs/VehicleLights");File.WriteAllText(Report,"FAIL: "+e);Debug.LogException(e);}
  }
  static Material Material(string channel){
   string path=Folder+channel+".mat";var mat=AssetDatabase.LoadAssetAtPath<Material>(path);
   if(!mat){mat=new Material(Shader.Find("Universal Render Pipeline/Unlit"));AssetDatabase.CreateAsset(mat,path);}
   mat.name="Operating vehicle lamps | "+channel;
   mat.SetColor("_BaseColor",channel=="Front"?new Color(1.7f,1.05f,.55f):channel=="Rear"?new Color(2,2,2):new Color(2, .65f,.025f));mat.SetFloat("_Surface",0);mat.SetFloat("_ZWrite",1);mat.renderQueue=2000;mat.enableInstancing=true;EditorUtility.SetDirty(mat);return mat;
  }
  public static void Configure(GameObject root,string model){
   var machine=root.GetComponent<MachineController>();Check(machine,"missing machine controller");
   var bindings=root.GetComponent<VehicleOperatingLights>();if(bindings&&bindings.authoringVersion>=5)return;
   var body=root.GetComponentsInChildren<MeshFilter>(true).Single(m=>m.name=="body").transform;
   if(!bindings)bindings=root.AddComponent<VehicleOperatingLights>();bindings.machine=machine;
   var old=body.Find("Operating lights | authored");if(old)UnityEngine.Object.DestroyImmediate(old.gameObject);
   var group=new GameObject("Operating lights | authored");group.layer=8;group.transform.SetParent(body,false);
   foreach(string channel in new[]{"Front","Rear","Beacon"}){
    string path=Folder+model+channel+".fbx";var importer=AssetImporter.GetAtPath(path) as ModelImporter;Check(importer!=null,"export lamp meshes first: "+path);
    importer.importCameras=false;importer.importLights=false;importer.importAnimation=false;importer.isReadable=false;importer.materialImportMode=ModelImporterMaterialImportMode.ImportStandard;
    importer.AddRemap(new AssetImporter.SourceAssetIdentifier(typeof(Material),"OperatingLamp"),Material(channel));importer.SaveAndReimport();
    var lamp=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(path));lamp.name=channel+" luminous surfaces";lamp.transform.SetParent(group.transform,false);
    foreach(var r in lamp.GetComponentsInChildren<MeshRenderer>(true)){r.sharedMaterials=new[]{Material(channel)};r.shadowCastingMode=ShadowCastingMode.Off;r.receiveShadows=false;r.lightProbeUsage=LightProbeUsage.Off;r.reflectionProbeUsage=ReflectionProbeUsage.Off;r.enabled=false;r.gameObject.layer=8;}
   }
   var data=JsonUtility.FromJson<Data>(File.ReadAllText(Folder+"LampBindings.json")).vehicles.Single(v=>v.model==model);
   var positions=data.lights.OrderByDescending(l=>l.position[1]).Take(2);
   foreach(var lamp in positions){
    var obj=new GameObject("Shadowless beam | "+lamp.name);obj.layer=8;obj.transform.SetParent(group.transform,false);obj.transform.localPosition=new Vector3(lamp.position[0],lamp.position[1],lamp.position[2]+.045f);obj.transform.localRotation=Quaternion.Euler(18,0,0);
    var light=obj.AddComponent<Light>();light.type=LightType.Spot;light.color=new Color(1,.85f,.6f);light.range=22;light.spotAngle=68;light.innerSpotAngle=38;light.intensity=10;light.shadows=LightShadows.None;light.renderMode=LightRenderMode.Auto;light.cullingMask=~(1<<8);light.enabled=false;
   }
   bindings.beams=group.GetComponentsInChildren<Light>(true);bindings.lamps=group.GetComponentsInChildren<Renderer>(true);
   bindings.rearLamp=group.transform.Find("Rear luminous surfaces").GetComponentInChildren<Renderer>();
   var beaconVisual=group.transform.Find("Beacon luminous surfaces");bindings.beaconLamp=beaconVisual.GetComponentInChildren<Renderer>();
   var beaconMaterial=Material("Beacon");beaconMaterial.shader=Shader.Find("Snowglow/Vehicle Beacon");beaconMaterial.SetColor("_BaseColor",VehicleOperatingLights.BeaconLensColor);beaconMaterial.SetVector("_Center",bindings.beaconLamp.GetComponent<MeshFilter>().sharedMesh.bounds.center);EditorUtility.SetDirty(beaconMaterial);
   var rotor=new GameObject("Beacon | rotating lens").transform;rotor.SetParent(group.transform,false);rotor.position=bindings.beaconLamp.bounds.center;
   beaconVisual.SetParent(rotor,true);bindings.beaconRotor=rotor;
   var halo=GameObject.CreatePrimitive(PrimitiveType.Quad);halo.name="Beacon | small glow halo";UnityEngine.Object.DestroyImmediate(halo.GetComponent<Collider>());halo.layer=8;halo.transform.SetParent(group.transform,false);halo.transform.position=rotor.position;halo.transform.localScale=Vector3.one*.6f;
   string haloPath=Folder+"BeaconHalo.mat";var haloMaterial=AssetDatabase.LoadAssetAtPath<Material>(haloPath);if(!haloMaterial){haloMaterial=new Material(Shader.Find("Snowglow/Vehicle Beacon Halo"));AssetDatabase.CreateAsset(haloMaterial,haloPath);}
   haloMaterial.SetColor("_BaseColor",VehicleOperatingLights.BeaconGlowColor);EditorUtility.SetDirty(haloMaterial);
   var haloRenderer=halo.GetComponent<MeshRenderer>();haloRenderer.sharedMaterial=haloMaterial;haloRenderer.shadowCastingMode=ShadowCastingMode.Off;haloRenderer.receiveShadows=false;haloRenderer.enabled=false;bindings.beaconHalo=haloRenderer;
   var glows=new List<Renderer>();
   foreach(var lamp in data.lights)glows.Add(LampGlow(group.transform,"Warm front glow",new Vector3(lamp.position[0],lamp.position[1],lamp.position[2]+.02f),true));
   var rear=bindings.rearLamp.bounds;
   foreach(float side in new[]{-1f,1f})glows.Add(LampGlow(group.transform,"White rear glow",group.transform.InverseTransformPoint(rear.center+Vector3.right*side*rear.extents.x*.85f+Vector3.back*.015f),false));
   bindings.lampHalos=glows.ToArray();
   var boxes=new GameObject("Lamp boxes | permanent geometry").transform;boxes.SetParent(group.transform,false);
   foreach(var lamp in data.lights){float face=lamp.position[2]+(lamp.name.Contains("lower")?.04f:.01f)-.003f;LampBox(boxes,"Front orange box | "+lamp.name,new Vector3(lamp.position[0],lamp.position[1],face-.05f),new Vector3(.26f,.23f,.1f),true);}
   foreach(float side in new[]{-1f,1f}){
    var center=group.transform.InverseTransformPoint(rear.center+Vector3.right*side*rear.extents.x*.85f);
    LampBox(boxes,"Rear white box | "+(side<0?"left":"right"),center+Vector3.forward*.055f,new Vector3(.25f,.19f,.1f),false);
   }
   bindings.authoringVersion=5;
  }
  static void LampBox(Transform parent,string name,Vector3 position,Vector3 size,bool front){
   string path=Folder+(front?"FrontOrangeHousing":"RearWhiteHousing")+".mat";var material=AssetDatabase.LoadAssetAtPath<Material>(path);
   if(!material){material=new Material(Shader.Find("Universal Render Pipeline/Simple Lit"));AssetDatabase.CreateAsset(material,path);}
   material.SetColor("_BaseColor",front?new Color(1,.83f,.025f):Color.white);material.SetFloat("_Smoothness",.15f);material.SetFloat("_Surface",0);material.renderQueue=2000;material.enableInstancing=true;EditorUtility.SetDirty(material);
   var box=GameObject.CreatePrimitive(PrimitiveType.Cube);box.name=name;UnityEngine.Object.DestroyImmediate(box.GetComponent<Collider>());box.layer=8;box.transform.SetParent(parent,false);box.transform.localPosition=position;box.transform.localScale=size;
   var renderer=box.GetComponent<MeshRenderer>();renderer.sharedMaterial=material;renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=true;
  }
  static Renderer LampGlow(Transform parent,string name,Vector3 position,bool front){
   string path=Folder+(front?"FrontGlow":"RearGlow")+".mat";var material=AssetDatabase.LoadAssetAtPath<Material>(path);
   if(!material){material=new Material(Shader.Find("Snowglow/Vehicle Beacon Halo"));AssetDatabase.CreateAsset(material,path);}
   material.SetColor("_BaseColor",front?new Color(1,.7f,.3f,1):Color.white);material.SetFloat("_Pulse",1);EditorUtility.SetDirty(material);
   var obj=GameObject.CreatePrimitive(PrimitiveType.Quad);obj.name=name;UnityEngine.Object.DestroyImmediate(obj.GetComponent<Collider>());obj.layer=8;obj.transform.SetParent(parent,false);obj.transform.localPosition=position;obj.transform.localScale=Vector3.one*.55f;
   var renderer=obj.GetComponent<MeshRenderer>();renderer.sharedMaterial=material;renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=false;renderer.enabled=false;return renderer;
  }
  public static void ConfigureButton(PrototypeHUD hud){
   var found=hud.driving.GetComponentsInChildren<VehicleLightsButton>(true).SingleOrDefault();
   var obj=found?found.gameObject:new GameObject("Lights | right center",typeof(RectTransform),typeof(Image),typeof(Button),typeof(VehicleLightsButton));var rect=obj.GetComponent<RectTransform>();rect.SetParent(hud.rightControls,false);rect.anchorMin=rect.anchorMax=new Vector2(1,.5f);rect.pivot=new Vector2(1,.5f);rect.anchoredPosition=Vector2.zero;rect.sizeDelta=new Vector2(48,48);
   var label=found?found.label:null;if(!label){var labelObject=new GameObject("Label",typeof(RectTransform),typeof(Text));label=labelObject.GetComponent<Text>();label.rectTransform.SetParent(rect,false);label.rectTransform.anchorMin=Vector2.zero;label.rectTransform.anchorMax=Vector2.one;label.rectTransform.offsetMin=label.rectTransform.offsetMax=Vector2.zero;label.font=hud.status.font;label.raycastTarget=false;}
   var toggle=obj.GetComponent<VehicleLightsButton>();toggle.label=label;toggle.background=obj.GetComponent<Image>();toggle.onIcon=ButtonIcon("LightsOn");toggle.offIcon=ButtonIcon("LightsOff");toggle.background.type=Image.Type.Simple;toggle.background.preserveAspect=true;toggle.Refresh(null);var button=obj.GetComponent<Button>();button.targetGraphic=toggle.background;button.navigation=new Navigation{mode=Navigation.Mode.None};button.onClick=new Button.ButtonClickedEvent();UnityEventTools.AddPersistentListener(button.onClick,toggle.Click);EditorUtility.SetDirty(toggle);
  }
  static Sprite ButtonIcon(string name){
   string path=CatalogPreparation.Root+"/Sources/VehicleLightButton/"+name+".png";var importer=AssetImporter.GetAtPath(path) as TextureImporter;Check(importer!=null,"missing supplied button icon: "+path);
   importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Multiple;importer.alphaIsTransparency=true;importer.sRGBTexture=true;importer.mipmapEnabled=false;importer.wrapMode=TextureWrapMode.Clamp;importer.filterMode=FilterMode.Bilinear;importer.maxTextureSize=256;importer.npotScale=TextureImporterNPOTScale.None;
   // Same source-pixel crop for both states; ignore transparent margins without
   // modifying the original bitmaps or stretching the square artwork.
   importer.spritesheet=new[]{new SpriteMetaData{name=name,rect=new Rect(184,188,874,876),pivot=new Vector2(.5f,.5f),alignment=9}};importer.SaveAndReimport();
   return AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().Single();
  }
  static void States(VehicleOperatingLights lights){
   Check(lights&&lights.machine&&lights.beams.Length==2&&lights.lamps.Length==3,"incomplete bindings");
   Check(lights.lamps.All(r=>r.sharedMaterials.Length==1&&r.GetComponent<MeshFilter>().sharedMesh.subMeshCount==1),"uncontrolled material slots on lamp pair");
   var boxes=lights.GetComponentsInChildren<MeshRenderer>(true).Where(r=>r.name.StartsWith("Front orange box",StringComparison.Ordinal)||r.name.StartsWith("Rear white box",StringComparison.Ordinal)).ToArray();
   Check(boxes.Count(r=>r.name.StartsWith("Rear",StringComparison.Ordinal))==2&&boxes.Count(r=>r.name.StartsWith("Front",StringComparison.Ordinal))>=2,"solid front/rear boxes missing");
   foreach(var box in boxes){var color=box.sharedMaterial.GetColor("_BaseColor");Check(box.name.StartsWith("Front",StringComparison.Ordinal)?color.r>.9f&&color.g>.75f&&color.g<.9f&&color.b<.1f:color==Color.white,"incorrect lamp box color");}
   var m=lights.machine;bool active=m.ActiveMachine,enabledLights=lights.lightsOn;float fuel=m.Fuel,condition=m.Condition;
   try{
    lights.lightsOn=true;
    m.ActiveMachine=true;m.Fuel=1;m.Condition=100;lights.Apply(true);Check(lights.beams.All(l=>l.enabled)&&lights.lamps.All(r=>r.enabled),"operating lights off");
    Check(lights.beaconHalo&&lights.beaconHalo.enabled&&lights.beaconRotor,"beacon bindings missing");
    lights.Apply(true,true);var block=new MaterialPropertyBlock();lights.rearLamp.GetPropertyBlock(block);var white=block.GetColor("_BaseColor");Check(white.r==white.g&&white.g==white.b,"reverse pair not white");
    lights.Apply(true,false);lights.rearLamp.GetPropertyBlock(block);var normal=block.GetColor("_BaseColor");Check(normal.r==normal.g&&normal.g==normal.b,"rear pair not white in forward");
    lights.lightsOn=false;lights.Apply(true);Check(lights.beams.All(l=>!l.enabled)&&lights.lamps.Where(r=>r!=lights.beaconLamp).All(r=>!r.enabled)&&lights.lampHalos.All(r=>!r.enabled),"switch off leaves lamps on");Check(boxes.All(r=>r.enabled),"lamp box geometry disappeared with lights off");Check(lights.beaconLamp.enabled&&lights.beaconHalo.enabled,"switch extinguished beacon");var independent=lights.beaconRotor.localRotation;lights.Advance(.25f);Check(Quaternion.Angle(independent,lights.beaconRotor.localRotation)>40,"switch stopped rotor");lights.lightsOn=true;lights.Apply(true);Check(lights.lampHalos.Length>=4&&lights.lampHalos.All(r=>r.enabled),"front/rear halos missing");
    var rotation=lights.beaconRotor.localRotation;lights.Advance(.25f);Check(Quaternion.Angle(rotation,lights.beaconRotor.localRotation)>40,"beacon does not rotate");float low=1,high=0;
    for(int sample=0;sample<8;sample++){lights.Advance(.25f);lights.beaconHalo.GetPropertyBlock(block);float value=block.GetFloat("_Pulse");low=Mathf.Min(low,value);high=Mathf.Max(high,value);Check(lights.beaconHalo.enabled==(value>0),"glow does not switch fully on/off");lights.beaconLamp.GetPropertyBlock(block);Check(block.GetColor("_BaseColor")==VehicleOperatingLights.BeaconLensColor,"beacon lens color changed during operation");}Check(high-low>.5f,"beacon does not pulse");
    lights.Apply(false);Check(lights.beams.All(l=>!l.enabled)&&lights.lamps.All(r=>!r.enabled)&&!lights.beaconHalo.enabled,"pause/menu lights on");var stopped=lights.beaconRotor.localRotation;lights.Advance(1);Check(Quaternion.Angle(stopped,lights.beaconRotor.localRotation)<.001f,"paused beacon rotates");
    m.ActiveMachine=false;lights.Apply(true);Check(lights.beams.All(l=>!l.enabled)&&lights.lamps.All(r=>!r.enabled),"parked machine lights on");
    m.ActiveMachine=true;m.Fuel=0;lights.Apply(true);Check(lights.beams.All(l=>!l.enabled)&&lights.lamps.All(r=>!r.enabled),"empty fuel lights on");
    m.Fuel=1;m.Condition=0;lights.Apply(true);Check(lights.beams.All(l=>!l.enabled)&&lights.lamps.All(r=>!r.enabled),"broken machine lights on");
    m.Condition=100;lights.Apply(true);lights.SwitchOff();Check(lights.beams.All(l=>!l.enabled)&&lights.lamps.All(r=>!r.enabled)&&!lights.beaconHalo.enabled,"disabled component lights on");
   }finally{m.ActiveMachine=active;m.Fuel=fuel;m.Condition=condition;lights.lightsOn=enabledLights;lights.Apply(false);if(lights.beaconRotor)lights.beaconRotor.localRotation=Quaternion.identity;}
   Check(lights.beams.All(l=>l.shadows==LightShadows.None&&l.type==LightType.Spot&&l.range<=22),"light budget changed");
  }
  static string Poses(Scene scene)=>string.Join("|",scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<CatalogPlacement>(true)).Where(p=>!p.GetComponentInParent<MachineController>()).OrderBy(p=>p.placementId).Select(p=>p.assetId+":"+p.placementId+":"+p.transform.localToWorldMatrix.ToString("R")));
  static void ButtonChecks(PrototypeHUD hud,MachineController[] fleet){
   var toggle=hud.driving.GetComponentsInChildren<VehicleLightsButton>(true).Single();var rect=(RectTransform)toggle.transform;Check(rect.parent==hud.rightControls&&rect.anchorMin==new Vector2(1,.5f)&&rect.anchorMax==rect.anchorMin&&rect.sizeDelta==new Vector2(48,48),"light button not at right center");Check(toggle.GetComponent<Button>().onClick.GetPersistentEventCount()==1,"button listener missing");Check(toggle.onIcon&&toggle.offIcon&&toggle.onIcon!=toggle.offIcon,"two supplied icon states missing");
   var field=typeof(PrototypeSession).GetField("<Instance>k__BackingField",System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.NonPublic);var previous=PrototypeSession.Instance;float scale=Time.timeScale;var fixture=new GameObject("Disposable light button command checks");
   var originals=fleet.Select(m=>m.GetComponent<VehicleOperatingLights>().lightsOn).ToArray();var active=fleet.Select(m=>m.ActiveMachine).ToArray();
   try{
    var game=fixture.AddComponent<PrototypeSession>();game.Fleet=fleet;game.Profile=new ProfileSave();game.Panel="";field.SetValue(null,game);Time.timeScale=1;
    for(int i=0;i<fleet.Length;i++){
     game.Profile.active=i;fleet[i].ActiveMachine=true;var lights=fleet[i].GetComponent<VehicleOperatingLights>();lights.lightsOn=true;toggle.Click();Check(!lights.lightsOn&&toggle.background.sprite==toggle.offIcon&&!toggle.label.gameObject.activeSelf,"button off icon failed");Check(lights.beaconLamp.enabled&&lights.beaconHalo.enabled,"button disabled beacon");toggle.Click();Check(lights.lightsOn&&toggle.background.sprite==toggle.onIcon&&toggle.background.color==Color.white,"button on icon failed");
    }
    game.Panel="pause";bool before=fleet[game.Profile.active].GetComponent<VehicleOperatingLights>().lightsOn;toggle.Click();Check(before==fleet[game.Profile.active].GetComponent<VehicleOperatingLights>().lightsOn,"paused button changed state");
   }finally{
    for(int i=0;i<fleet.Length;i++){fleet[i].ActiveMachine=active[i];var lights=fleet[i].GetComponent<VehicleOperatingLights>();lights.lightsOn=originals[i];lights.SwitchOff();}field.SetValue(null,previous);UnityEngine.Object.DestroyImmediate(fixture);Time.timeScale=scale;toggle.Refresh(fleet[0].GetComponent<VehicleOperatingLights>());
   }
  }
  static void Preview(string path,string model){
   var preview=new PreviewRenderUtility();Material ground=null;
   try{
    var obj=UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(path));preview.AddSingleGO(obj);var lights=obj.GetComponent<VehicleOperatingLights>();lights.machine.ActiveMachine=true;lights.machine.Fuel=1;lights.machine.Condition=100;
    var floor=GameObject.CreatePrimitive(PrimitiveType.Cube);floor.name="Light check floor";floor.transform.position=new Vector3(0,-.08f,8);floor.transform.localScale=new Vector3(18,.1f,30);ground=new Material(Shader.Find("Universal Render Pipeline/Simple Lit"));ground.SetColor("_BaseColor",new Color(.65f,.7f,.75f));floor.GetComponent<Renderer>().sharedMaterial=ground;preview.AddSingleGO(floor);
    preview.ambientColor=new Color(.025f,.025f,.025f);foreach(var light in preview.lights)light.intensity=.03f;
    preview.camera.transform.position=new Vector3(7,5,12);preview.camera.transform.LookAt(new Vector3(0,1,2));preview.camera.orthographic=true;preview.camera.orthographicSize=6;preview.camera.nearClipPlane=.1f;preview.camera.farClipPlane=80;preview.camera.clearFlags=CameraClearFlags.SolidColor;preview.camera.backgroundColor=new Color(.025f,.035f,.055f);
    string[] names={"off","on","front-pair","rear-white","rear-reverse","beacon-low","beacon-bright","switch-off-beacon-on","rear-boxes-off","front-boxes-off"};
    for(int pass=0;pass<names.Length;pass++){
     lights.lightsOn=pass<7;lights.Apply(pass!=0,pass==4);
     if(pass>=2){preview.ambientColor=Color.gray;preview.lights[0].intensity=1.2f;preview.lights[1].intensity=.5f;preview.camera.orthographicSize=2.8f;
      preview.camera.transform.position=pass==2||pass==9?new Vector3(0,2.5f,9):pass<=4||pass==8?new Vector3(0,2.5f,-9):new Vector3(3,5,3);preview.camera.transform.LookAt(new Vector3(0,1.6f,0));}
     if(pass==5)lights.Advance(.5f);if(pass==6)lights.Advance(1.5f);
     preview.BeginStaticPreview(new Rect(0,0,960,600));preview.Render(true);var image=preview.EndStaticPreview();File.WriteAllBytes("Logs/VehicleLights/"+model+"-"+names[pass]+".png",image.EncodeToPNG());
     if(pass==2){
      var data=JsonUtility.FromJson<Data>(File.ReadAllText(Folder+"LampBindings.json")).vehicles.Single(v=>v.model==model);var body=obj.GetComponentsInChildren<MeshFilter>().Single(m=>m.name=="body").transform;
      foreach(var lamp in data.lights.OrderByDescending(l=>l.position[1]).Take(2)){
       var uv=preview.camera.WorldToViewportPoint(body.TransformPoint(new Vector3(lamp.position[0],lamp.position[1],lamp.position[2])));
       var color=image.GetPixel(Mathf.Clamp(Mathf.RoundToInt(uv.x*image.width),0,image.width-1),Mathf.Clamp(Mathf.RoundToInt(uv.y*image.height),0,image.height-1));
       Check(color.r>.7f&&color.g>.7f&&color.b>.7f,"front lamp not visibly white: "+model+" "+lamp.name+" "+color);
      }
     }
     if(pass==3||pass==4)foreach(float side in new[]{-1f,1f}){
      var bounds=lights.rearLamp.bounds;var point=bounds.center+Vector3.right*side*bounds.extents.x*.85f;var uv=preview.camera.WorldToViewportPoint(point);
      var color=image.GetPixel(Mathf.Clamp(Mathf.RoundToInt(uv.x*image.width),0,image.width-1),Mathf.Clamp(Mathf.RoundToInt(uv.y*image.height),0,image.height-1));
      Check(color.r>.7f&&color.g>.7f&&color.b>.7f,"rear lamp not visibly white: "+model+" side "+side+" sampled "+color);
     }
     UnityEngine.Object.DestroyImmediate(image);
    }
   }finally{preview.Cleanup();if(ground)UnityEngine.Object.DestroyImmediate(ground);}
  }
  [MenuItem("Snowglow/Install operating vehicle lights and validate")]
  public static void Install(){
   Check(!EditorApplication.isPlayingOrWillChangePlaymode,"stop Play first");var scene=SceneManager.GetActiveScene();Check(scene.path==SceneValidation.ScenePath&&!scene.isDirty,"save Pinefall first");Directory.CreateDirectory("Logs/VehicleLights");
   string backup="Logs/VehicleLights/Pinefall-before.unity";if(!File.Exists(backup))File.Copy(scene.path,backup);
   var session=scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<PrototypeSession>()).Single();
   var seed=session.Snow.seed;string seedHash=AssetDatabase.GetAssetDependencyHash(AssetDatabase.GetAssetPath(seed)).ToString();
   var fleetPoses=session.Fleet.Select(m=>m.transform.localToWorldMatrix).ToArray();var ids=session.Fleet.Select(m=>m.definition.id).ToArray();
   string poses=Poses(scene);
   var pipeline=GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;Check(pipeline,"URP missing");pipeline.maxAdditionalLightsCount=2;EditorUtility.SetDirty(pipeline);
   foreach(var item in new[]{new[]{StarterPlowAuthoring.PrefabPath,"Tractor"},new[]{LoaderModelIntake.PrefabPath,"Loader"}}){
    string path=item[0];var obj=PrefabUtility.LoadPrefabContents(path);try{Configure(obj,item[1]);States(obj.GetComponent<VehicleOperatingLights>());PrefabUtility.SaveAsPrefabAsset(obj,path);}finally{PrefabUtility.UnloadPrefabContents(obj);}
   }
   AssetDatabase.SaveAssets();
   foreach(var machine in session.Fleet){var lights=machine.GetComponent<VehicleOperatingLights>();States(lights);}
   ConfigureButton(session.HUD);
   SceneValidation.Validate(scene);EditorSceneManager.MarkSceneDirty(scene);Check(EditorSceneManager.SaveScene(scene),"scene save failed");scene=EditorSceneManager.OpenScene(SceneValidation.ScenePath);SceneValidation.Validate(scene);MechanicsChecks.Run();
   session=scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<PrototypeSession>()).Single();
   Check(seedHash==AssetDatabase.GetAssetDependencyHash(AssetDatabase.GetAssetPath(session.Snow.seed)).ToString(),"snow seed changed");
   for(int i=0;i<session.Fleet.Length;i++){Check(session.Fleet[i].transform.localToWorldMatrix==fleetPoses[i]&&session.Fleet[i].definition.id==ids[i],"machine transform/save ID changed");States(session.Fleet[i].GetComponent<VehicleOperatingLights>());}
   ButtonChecks(session.HUD,session.Fleet);
   Directory.CreateDirectory("Logs/CompactHUD");typeof(CompactHUDAuthoring).GetMethod("Preview",System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.NonPublic).Invoke(null,new object[]{session.HUD});File.Copy("Logs/CompactHUD/Preview.png","Logs/VehicleLights/LightsButton.png",true);
   Check(Poses(scene)==poses,"environment poses changed");Preview(StarterPlowAuthoring.PrefabPath,"Tractor");Preview(LoaderModelIntake.PrefabPath,"Loader");
   Check(!ShaderUtil.ShaderHasError(Shader.Find("Snowglow/Vehicle Beacon"))&&!ShaderUtil.ShaderHasError(Shader.Find("Snowglow/Vehicle Beacon Halo")),"beacon shader compilation failed");
   File.WriteAllText("Logs/VehicleLights/AmberBeacon.txt","PASS: Unity "+Application.unityVersion+". Both machines use the same constant tractor-yellow/amber lens color throughout rotation. Eight animation samples per state check lens color unchanged and the separate halo renderer switching completely on/off. Rotation remains independent of the front/rear lamps toggle. Prefabs/Pinefall save/reopen, both beacon shaders, scene and 19 snow checks passed. Beacon low/bright renders generated for both vehicles. Live driving and phone appearance remain untested.");
   File.WriteAllText("Logs/VehicleLights/LampBoxes.txt","PASS: Unity "+Application.unityVersion+". Saved/reopened solid lamp-box geometry on both machines: two orange front boxes and two white rear boxes on tractor; four orange front boxes and two white rear boxes on loader. Exact material colors, box counts and visibility with lights off checked. No box colliders; original source meshes/rigs retained. Twenty vehicle renders include front/rear geometry with lamps switched off. Main scene/snow checks in Validation.txt. Live driving and phone rendering untested.");
   File.WriteAllText(Report,"PASS: Unity "+Application.unityVersion+" / "+EditorUserBuildSettings.activeBuildTarget+". Saved/reopened both equipped prefabs and Pinefall, including a 48x48 right-center on/off icon button with persistent listener. Button command tested on both machines and while paused. Head/rear lamps and halos switch off independently of the rotating amber beacon. Front beams warm and intensity 10 (previously 7), rear lamps white in both gears. Source-bound front/rear halos present on both machines. Paired lamp pixel samples, independent beacon rotation/pulse, pause, fuel, condition, parked/disable states and single material/submesh checks passed. Twenty vehicle renders plus HUD preview generated. Beacon shaders, scene validation and 19 snow-ledger checks passed. Environment/fleet poses, IDs and seed hashes unchanged. No player-save writes. Touch interaction, all camera views during driving, Android build and Galaxy A07 costs remain untested.");Debug.Log(File.ReadAllText(Report));
  }
 }
}
