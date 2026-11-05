using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.IO;
using System;
namespace Snowglow.Editor {
 // This is the ONLY scene factory entry point. It executes in Edit mode and saves concrete assets.
 public static class PrototypeAuthoring {
  const string Marker="Assets/Snowglow/Generated/Ready.txt";static string folder;
  [InitializeOnLoadMethod]static void FirstImport(){EditorApplication.delayCall+=()=>{
   if(Application.isBatchMode||EditorApplication.isPlayingOrWillChangePlaymode||EditorApplication.isCompiling||File.Exists(Marker))return;
   if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty){Debug.Log("Snowglow: choose Snowglow > Bake editable test district when ready.");return;}
   Bake();
  };}
  static void Asset(UnityEngine.Object o,string name){AssetDatabase.CreateAsset(o,folder+"/"+name+".asset");}
  static Material Material(string name,string hex,string shader="Universal Render Pipeline/Simple Lit"){
   var found=Shader.Find(shader);if(found==null)throw new InvalidOperationException("Missing shader: "+shader+". Wait for URP packages to import.");
   ColorUtility.TryParseHtmlString(hex,out Color color);var m=new Material(found){name=name,enableInstancing=true};m.SetColor("_BaseColor",color);if(m.HasProperty("_Smoothness"))m.SetFloat("_Smoothness",.08f);Asset(m,name);return m;
  }
  [MenuItem("Snowglow/Bake editable test district")]
  public static void Bake(){
   if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Stop Play mode before baking assets.");
   if(!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())return;
   Directory.CreateDirectory("Assets/Snowglow/Generated");folder="Assets/Snowglow/Generated/Bake_"+DateTime.UtcNow.ToString("yyyyMMdd_HHmmss_fff");Directory.CreateDirectory(folder);AssetDatabase.Refresh();
   try{
    EditorUtility.DisplayProgressBar("Snowglow","Saving URP, materials and definitions",.05f);
    var renderer=ScriptableObject.CreateInstance<UniversalRendererData>();Asset(renderer,"MobileRenderer");
    var urp=UniversalRenderPipelineAsset.Create(renderer);urp.supportsHDR=false;urp.msaaSampleCount=2;urp.renderScale=.9f;urp.shadowDistance=65;urp.mainLightShadowmapResolution=1024;urp.shadowCascadeCount=1;urp.useSRPBatcher=true;urp.maxAdditionalLightsCount=2;Asset(urp,"MobileURP");
    GraphicsSettings.defaultRenderPipeline=urp;QualitySettings.renderPipeline=urp;
    var p=ScriptableObject.CreateInstance<PrototypePalette>();
    p.snow=Material("Snow white","#EDF4F8");p.asphalt=Material("Road grey","#52616B");p.orange=Material("Plow orange","#D8873F");p.teal=Material("Municipal teal","#3F777D");p.dark=Material("Tyres and steel","#28333B");
    p.glass=Material("Blue glazing","#78BCCB");p.amber=Material("Loader yellow and lamps","#E9BC56");p.wood=Material("Timber brown","#815C46");p.green=Material("Foliage green","#3C6952");p.stone=Material("Mountain grey","#829098");p.ice=Material("Ice blue","#9ECEDB");p.red=Material("Service red","#B85E52");p.terrain=Material("Terrain snow","#FFFFFF","Universal Render Pipeline/Terrain/Lit");Asset(p,"Palette");
    var data=DistrictFactory.CreateTerrainData();
    SnowMaterialAuthoring.Configure(p.snow,false);SnowMaterialAuthoring.Configure(p.terrain,true);
    var texture=new Texture2D(2,2);texture.SetPixels(new[]{Color.white,Color.white,Color.white,Color.white});texture.Apply();Asset(texture,"SnowTerrainTexture");
    var layer=new TerrainLayer{diffuseTexture=texture,tileSize=new Vector2(20,20)};SnowMaterialAuthoring.Configure(layer);Asset(layer,"SnowTerrainLayer");data.terrainLayers=new[]{layer};Asset(data,"Terrain_1000m");
    EditorUtility.DisplayProgressBar("Snowglow","Placing coloured primitives on the terrain",.25f);
    var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);DistrictFactory.Build(data,p);SnowDisposalYardIntake.PlacePrepared(scene);FuelStationIntake.PlacePrepared(scene);ParkingVehiclesIntake.PlacePrepared(scene);WinterHomesIntake.PlacePreparedHomes(scene);TownBlockAuthoring.PlacePreparedBlocks(scene);AlpineMountainsIntake.PlacePreparedMountains(scene);StreetLightsIntake.PlacePreparedLights(scene);AlpineTreesIntake.PlacePreparedTrees(scene);
    RoadPresentationAuthoring.Author(scene);
    var first=StarterPlowAuthoring.Create(new Vector3(-99,SnowField.Ground+.25f,-87),Quaternion.identity,p.snow);var second=LoaderModelIntake.Create(new Vector3(-90,SnowField.Ground+.25f,-87),Quaternion.identity,p.snow);
    var snow=new GameObject("05 Snow | baked primitive columns and editable seed").AddComponent<SnowField>();var seed=ScriptableObject.CreateInstance<SnowSeed>();Asset(seed,"SnowSeed");snow.Bake(p,seed);
    EditorUtility.DisplayProgressBar("Snowglow","Saving snow geometry and collision assets",.55f);
    SnowMeshAssets.SaveChunks(snow,seed);
    EditorUtility.SetDirty(seed);
    var session=new GameObject("06 Game systems | no runtime scene generation").AddComponent<PrototypeSession>();
    session.Fleet=new[]{first,second};session.Snow=snow;session.Input=session.gameObject.AddComponent<PrototypeInput>();
    var cam=new GameObject("Camera | isometric follow orbit and scenic").AddComponent<Camera>();cam.gameObject.AddComponent<AudioListener>();cam.gameObject.AddComponent<UniversalAdditionalCameraData>();
    cam.backgroundColor=new Color(.66f,.76f,.83f);cam.clearFlags=CameraClearFlags.SolidColor;cam.orthographic=true;cam.orthographicSize=22;cam.nearClipPlane=.2f;cam.farClipPlane=1000;cam.transform.position=new Vector3(-123,35,-111);cam.transform.rotation=Quaternion.Euler(41,45,0);
    var cameraRig=cam.gameObject.AddComponent<CameraRig>();cameraRig.view=cam;cameraRig.terrain=UnityEngine.Object.FindFirstObjectByType<Terrain>();session.CameraRig=cameraRig;
    session.Audio=session.gameObject.AddComponent<SynthAudio>();session.Audio.effects=session.gameObject.AddComponent<AudioSource>();session.Audio.effects.playOnAwake=false;
    session.Audio.motor=session.gameObject.AddComponent<AudioSource>();session.Audio.motor.clip=AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Snowglow/Audio/motor.wav");session.Audio.motor.loop=true;session.Audio.motor.volume=0;session.Audio.motor.playOnAwake=true;
    session.Audio.ambience=session.gameObject.AddComponent<AudioSource>();session.Audio.ambience.playOnAwake=false;
    ReverseAlarmAuthoring.Bind(session.Audio);MachineryAudioAuthoring.Bind(session.Audio);
    session.Audio.notes=new AudioClip[8];for(int i=0;i<8;i++)session.Audio.notes[i]=AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Snowglow/Audio/note"+i+".wav");session.Audio.chime=AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Snowglow/Audio/chime.wav");
    session.HUD=HUDFactory.Build();
    TyreTracksAuthoring.Place(session);
    // Transparent primitive objective marker: editable, non-colliding, and separate from snow.
    var marker=PrimitiveKit.Group("07 Contract reference markers",null,Vector3.zero);
    var targets=new[]{new Vector3(-65,5.2f,-88.5f),new Vector3(92.5f,5.2f,21),new Vector3(0,5.2f,0)};
    var sizes=new[]{new Vector2(9,31),new Vector2(31,24),new Vector2(70,9)};
    for(int j=1;j<3;j++){
     var group=PrimitiveKit.Group("Contract "+j+" boundary",marker,targets[j]);
     foreach(float side in new[]{-1f,1}){
      PrimitiveKit.Cube("Long boundary",group,new Vector3(side*sizes[j].x*.5f,0,0),new Vector3(.15f,.03f,sizes[j].y),p.amber);
      PrimitiveKit.Cube("Short boundary",group,new Vector3(0,0,side*sizes[j].y*.5f),new Vector3(sizes[j].x,.03f,.15f),p.amber);
     }
    }
    PlayerSettings.defaultInterfaceOrientation=UIOrientation.LandscapeLeft;PlayerSettings.allowedAutorotateToPortrait=false;PlayerSettings.allowedAutorotateToPortraitUpsideDown=false;
    PlayerSettings.companyName="SnowglowStudio";PlayerSettings.productName="Snowglow";PlayerSettings.SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.Android,"com.snowglow.prototype");
    Time.fixedDeltaTime=.02f;
    EditorUtility.DisplayProgressBar("Snowglow","Saving concrete scene and prefabs",.9f);
    Directory.CreateDirectory("Assets/Scenes");if(!EditorSceneManager.SaveScene(scene,"Assets/Scenes/Pinefall.unity"))throw new IOException("Could not save the baked district scene.");EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Scenes/Pinefall.unity",true)};
    AssetDatabase.SaveAssets();SceneValidation.Validate(scene);File.WriteAllText(Marker,"Concrete edit-mode scene baked at "+folder);AssetDatabase.Refresh();
    Selection.activeGameObject=first.gameObject;if(SceneView.lastActiveSceneView!=null)SceneView.lastActiveSceneView.FrameSelected();
    Debug.Log("Snowglow authoring complete. All terrain, machinery, snow and scenery exist before Play. Scene saved: Assets/Scenes/Pinefall.unity");
   }finally{EditorUtility.ClearProgressBar();}
  }
  [MenuItem("Snowglow/Open baked district")]
  public static void Open(){if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())EditorSceneManager.OpenScene("Assets/Scenes/Pinefall.unity");}
 }
}
