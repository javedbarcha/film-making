using UnityEngine;
namespace Snowglow { public static class DistrictFactory {
 public const float BaseHeight=5;
 public static TerrainData CreateTerrainData() {
  var d=new TerrainData{heightmapResolution=257,size=new Vector3(1000,140,1000),baseMapResolution=256};var h=new float[257,257];
  for(int z=0;z<257;z++)for(int x=0;x<257;x++) {
   float wx=x/256f*1000-500,wz=z/256f*1000-500;
   float e=Mathf.SmoothStep(0,1,Mathf.InverseLerp(150,370,Mathf.Max(Mathf.Abs(wx),Mathf.Abs(wz))));
   h[z,x]=(BaseHeight+e*(18+Mathf.PerlinNoise(wx*.007f+8,wz*.007f+8)*60)+e*32*Mathf.PerlinNoise(wx*.016f+19,wz*.016f+19))/140;
  }d.SetHeights(0,0,h);return d;
 }
 public static Transform Build(TerrainData data,PrototypePalette p) {
  var root=PrimitiveKit.Group("PINEFALL | ONE KILOMETRE TEST DISTRICT",null,Vector3.zero);
  var terrain=Terrain.CreateTerrainGameObject(data);terrain.name="Terrain | 1000 x 1000 metres";terrain.transform.SetParent(root);terrain.transform.position=new Vector3(-500,0,-500);
  var t=terrain.GetComponent<Terrain>();t.materialTemplate=p.terrain;t.heightmapPixelError=12;t.basemapDistance=180;t.drawInstanced=true;
  var roads=PrimitiveKit.Group("01 Roads and working surfaces",root,Vector3.zero);
  foreach(float lane in new[]{-65f,0,65f}) { Road(roads,new Vector3(0,0,lane),new Vector3(226,.18f,10),p);Road(roads,new Vector3(lane,0,0),new Vector3(10,.18f,226),p); }
  Road(roads,new Vector3(-95,0,-90),new Vector3(40,.18f,40),p);Road(roads,new Vector3(-76,0,-90),new Vector3(28,.18f,10),p);
  Road(roads,new Vector3(92,0,27),new Vector3(36,.18f,40),p);Road(roads,new Vector3(78,0,27),new Vector3(30,.18f,10),p);
  Road(roads,new Vector3(94,0,-85),new Vector3(34,.18f,32),p);
  var ice=PrimitiveKit.Cube("ICE | braking test",roads,new Vector3(30,BaseHeight+.16f,0),new Vector3(22,.025f,9.7f),p.ice);ice.AddComponent<SurfaceZone>().grip=.28f;
  var ramp=PrimitiveKit.Cube("SLOPE | traction test",roads,new Vector3(94,BaseHeight+1.9f,-34),new Vector3(12,.8f,28),p.asphalt,true);ramp.transform.localRotation=Quaternion.Euler(-8,0,0);
  var props=PrimitiveKit.Group("02 Primitive neighbourhood",root,Vector3.zero);int k=0;
  foreach(float x in new[]{-94f,-34,-18,20,39,94})foreach(float z in new[]{-32f,31,94}) {
   if(x>80&&z<60)continue;House(props,new Vector3(x,BaseHeight,z),k++%2==0?p.wood:p.teal,p,k%3);
  }
  for(int i=0;i<12;i++)PrimitiveKit.Cube("Curb test segment",props,new Vector3(81+i*2,BaseHeight+.18f,-53),new Vector3(1.9f,.35f,.45f),p.stone,true);
  PrimitiveKit.Part("Roadside boulder",PrimitiveType.Sphere,props,new Vector3(116,BaseHeight+1.2f,75),new Vector3(3,2.4f,3.4f),p.stone,true);
  Depot(props,p);Sign(props,new Vector3(-90,BaseHeight,-63),"PINEFALL\nFLEET DEPOT",p);Sign(props,new Vector3(78,BaseHeight,-68),"SNOW STORAGE\nDRIVE IN + DUMP",p);Sign(props,new Vector3(88,BaseHeight,48),"PARKING\nLOADER TEST",p);
  foreach(float x in new[]{-57f,8,57})foreach(float z in new[]{-57f,8,57})Lamp(props,new Vector3(x,BaseHeight,z),p);
  for(int i=0;i<5;i++)ParkedCar(props,new Vector3(83+i*5.5f,BaseHeight,40),p,i%2==0);
  var forest=PrimitiveKit.Group("03 Primitive forest and mountains",root,Vector3.zero);var rng=new System.Random(4207);
  for(int i=0;i<110;i++) {
   float x=(float)rng.NextDouble()*800-400,z=(float)rng.NextDouble()*800-400;if(Mathf.Abs(x)<125&&Mathf.Abs(z)<125)continue;
   var tree=Tree(forest,new Vector3(x,data.GetInterpolatedHeight((x+500)/1000,(z+500)/1000),z),.8f+(float)rng.NextDouble()*.7f,p);tree.gameObject.AddComponent<DistanceVisibility>().distance=230;
  }
  for(int i=0;i<12;i++) {
   float a=i*Mathf.PI*2/12;var m=PrimitiveKit.Part("Rounded mountain | sphere",PrimitiveType.Sphere,forest,new Vector3(Mathf.Sin(a)*405,32,Mathf.Cos(a)*405),new Vector3(150,115+i%3*25,145),p.stone);
   PrimitiveKit.Part("Mountain snow cap",PrimitiveType.Sphere,m.transform,new Vector3(0,.19f,0),new Vector3(.82f,.72f,.82f),p.snow);
  }
  var dump=PrimitiveKit.Group("04 Disposal yard",root,new Vector3(94,BaseHeight,-85));
  PrimitiveKit.Cube("Back retaining wall",dump,new Vector3(0,1,-14),new Vector3(34,2,1),p.stone,true);
  foreach(float x in new[]{-16f,16})PrimitiveKit.Cube("Side retaining wall",dump,new Vector3(x,.8f,0),new Vector3(1,1.6f,28),p.stone,true);
  for(int i=0;i<8;i++)PrimitiveKit.Cube("Disposal marker",dump,new Vector3(-14+i*4,.13f,12),new Vector3(2,.07f,.4f),p.amber);
  var sun=new GameObject("Sun | winter afternoon").AddComponent<Light>();sun.transform.SetParent(root);sun.type=LightType.Directional;sun.intensity=1.25f;sun.color=new Color(1,.89f,.76f);sun.shadows=LightShadows.Soft;sun.transform.rotation=Quaternion.Euler(38,-32,0);
  RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Flat;RenderSettings.ambientLight=new Color(.65f,.73f,.83f);RenderSettings.fog=true;RenderSettings.fogMode=FogMode.Linear;RenderSettings.fogColor=new Color(.66f,.76f,.83f);RenderSettings.fogStartDistance=170;RenderSettings.fogEndDistance=650;return root;
 }
 static void Road(Transform parent,Vector3 at,Vector3 size,PrototypePalette p){PrimitiveKit.Cube("Road | asphalt",parent,new Vector3(at.x,BaseHeight+.03f,at.z),size,p.asphalt,true);}
 static void House(Transform parent,Vector3 at,Material wall,PrototypePalette p,int variation) {
  var h=PrimitiveKit.Group("Cottage | modular primitives",parent,at);float width=7+variation;
  PrimitiveKit.Cube("Foundation",h,new Vector3(0,.25f,0),new Vector3(width+.5f,.5f,7.5f),p.stone,true);PrimitiveKit.Cube("Timber walls",h,new Vector3(0,2.3f,0),new Vector3(width,4,7),wall,true);
  foreach(float side in new[]{-1f,1}) {
   var r=PrimitiveKit.Cube("Sloping roof slab",h,new Vector3(side*width*.25f,5,0),new Vector3(width*.62f,.35f,8),p.dark);r.transform.localRotation=Quaternion.Euler(0,0,-side*30);
   var s=PrimitiveKit.Cube("Roof snow",h,new Vector3(side*width*.25f,5.23f,0),new Vector3(width*.63f,.25f,8.1f),p.snow);s.transform.localRotation=r.transform.localRotation;
  }
  PrimitiveKit.Cube("Chimney",h,new Vector3(2,5.7f,1.6f),new Vector3(.8f,2,.8f),p.stone);PrimitiveKit.Cube("Door",h,new Vector3(0,1.25f,-3.56f),new Vector3(1.2f,2.4f,.12f),p.dark);
  foreach(float x in new[]{-2.2f,2.2f}) {PrimitiveKit.Cube("Window",h,new Vector3(x,2.5f,-3.56f),new Vector3(1.2f,1.4f,.12f),p.amber);PrimitiveKit.Cube("Window snow sill",h,new Vector3(x,1.75f,-3.7f),new Vector3(1.5f,.15f,.45f),p.snow);}
  PrimitiveKit.Cube("Front step",h,new Vector3(0,.2f,-4),new Vector3(2,.4f,1),p.stone,true);
  for(int i=0;i<6;i++)PrimitiveKit.Cube("Fence post",h,new Vector3(-5+i*2,.9f,6),new Vector3(.18f,1.8f,.18f),p.wood,true);PrimitiveKit.Cube("Fence rail",h,new Vector3(0,1.2f,6),new Vector3(10,.16f,.15f),p.wood);
 }
 static Transform Tree(Transform parent,Vector3 at,float scale,PrototypePalette p) {
  var t=PrimitiveKit.Group("Snow tree | cylinder + spheres",parent,at);t.localScale=Vector3.one*scale;PrimitiveKit.Part("Trunk",PrimitiveType.Cylinder,t,new Vector3(0,2.4f,0),new Vector3(.45f,2.4f,.45f),p.wood);
  for(int i=0;i<3;i++){float r=3-i*.65f;PrimitiveKit.Part("Foliage",PrimitiveType.Sphere,t,new Vector3(0,3+i*1.6f,0),new Vector3(r*1.6f,2.3f,r*1.6f),p.green);PrimitiveKit.Part("Snow crown",PrimitiveType.Sphere,t,new Vector3(0,3.6f+i*1.6f,0),new Vector3(r*1.5f,1.3f,r*1.5f),p.snow);}return t;
 }
 static void Lamp(Transform parent,Vector3 at,PrototypePalette p) {
  var g=PrimitiveKit.Group("Street light | cylinders and cubes",parent,at);PrimitiveKit.Part("Pole",PrimitiveType.Cylinder,g,new Vector3(0,3.7f,0),new Vector3(.18f,3.7f,.18f),p.dark,true);PrimitiveKit.Beam("Arm",g,new Vector3(0,7.3f,0),new Vector3(1.8f,7.3f,0),.12f,p.dark);PrimitiveKit.Cube("Lantern",g,new Vector3(1.8f,7.1f,0),new Vector3(1,.3f,.5f),p.amber);
 }
 static void Depot(Transform parent,PrototypePalette p) {
  var g=PrimitiveKit.Group("Fleet workshop",parent,new Vector3(-94,BaseHeight,-107));PrimitiveKit.Cube("Workshop block",g,new Vector3(0,3.5f,0),new Vector3(24,7,8),p.teal,true);PrimitiveKit.Cube("Roof",g,new Vector3(0,7.2f,0),new Vector3(25,.4f,9),p.snow);
  foreach(float x in new[]{-6f,6})PrimitiveKit.Cube("Roller door",g,new Vector3(x,2.6f,4.05f),new Vector3(7,5.2f,.15f),p.dark);
  PrimitiveKit.Part("Fuel tank",PrimitiveType.Cylinder,parent,new Vector3(-112,BaseHeight+1,-85),new Vector3(2,1,2),p.red,true);Sign(parent,new Vector3(-108,BaseHeight,-79),"FUEL + REPAIRS",p);
 }
 static void Sign(Transform parent,Vector3 at,string label,PrototypePalette p) {
  var g=PrimitiveKit.Group(label.Replace("\n"," "),parent,at);PrimitiveKit.Part("Sign post",PrimitiveType.Cylinder,g,new Vector3(0,1.6f,0),new Vector3(.12f,1.6f,.12f),p.dark);PrimitiveKit.Cube("Sign board",g,new Vector3(0,3,0),new Vector3(4,1.5f,.18f),p.teal);
  var text=new GameObject("Label").AddComponent<TextMesh>();text.transform.SetParent(g,false);text.transform.localPosition=new Vector3(0,3,-.12f);text.transform.localRotation=Quaternion.Euler(0,180,0);text.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");text.text=label;text.anchor=TextAnchor.MiddleCenter;text.alignment=TextAlignment.Center;text.characterSize=.19f;text.fontSize=32;text.color=Color.white;
 }
 static void ParkedCar(Transform parent,Vector3 at,PrototypePalette p,bool red) {
  var g=PrimitiveKit.Group("Parked primitive car",parent,at);PrimitiveKit.Cube("Body",g,new Vector3(0,.7f,0),new Vector3(1.9f,.75f,4),red?p.red:p.teal,true);PrimitiveKit.Cube("Cabin",g,new Vector3(0,1.3f,-.2f),new Vector3(1.65f,.8f,2),p.glass);PrimitiveKit.Cube("Roof snow",g,new Vector3(0,1.75f,-.2f),new Vector3(1.7f,.15f,2.1f),p.snow);
  foreach(float x in new[]{-.95f,.95f})foreach(float z in new[]{-1.2f,1.2f}){var wheel=PrimitiveKit.Part("Tyre cylinder",PrimitiveType.Cylinder,g,new Vector3(x,.4f,z),new Vector3(.65f,.12f,.65f),p.dark);wheel.transform.localRotation=Quaternion.Euler(0,0,90);}
 }
}}
