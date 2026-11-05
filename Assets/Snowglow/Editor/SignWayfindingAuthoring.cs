using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
namespace Snowglow.Editor {
 public static class SignWayfindingAuthoring {
  const string Marker="Wayfinding | curated town signs v6";
  sealed class Plan {public string id,purpose;public int variant,arrow;public CatalogPlacement sign;public Vector3 position,read,target;public Quaternion rotation;public bool wall,boundary;public List<Row> rows=new List<Row>();}
  sealed class Row {public float height;public string text;public Row(float h,string t){height=h;text=t;}}
  static T[] All<T>(Scene s)where T:Component=>s.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<T>(true)).ToArray();
  static bool Sign(CatalogPlacement p)=>p&&(p.assetId??"").StartsWith("alpine-sign-",StringComparison.Ordinal);
  static void Require(bool ok,string message){if(!ok)throw new InvalidOperationException("Sign wayfinding: "+message);}
  static Vector3 Flat(Vector3 v)=>new Vector3(v.x,0,v.z);
  static Bounds Bounds(CatalogPlacement p,Vector3 at,Quaternion rotation){var b=CatalogPreparation.Library().Find(p.assetId).localBounds;var m=Matrix4x4.TRS(at,rotation,p.transform.lossyScale);var result=new Bounds(m.MultiplyPoint3x4(b.center),Vector3.zero);for(int i=0;i<8;i++)result.Encapsulate(m.MultiplyPoint3x4(b.center+Vector3.Scale(b.extents,new Vector3((i&1)==0?-1:1,(i&2)==0?-1:1,(i&4)==0?-1:1))));return result;}
  static bool XZ(Bounds a,Bounds b,float gap)=>a.min.x<b.max.x+gap&&a.max.x>b.min.x-gap&&a.min.z<b.max.z+gap&&a.max.z>b.min.z-gap;
  static Plan[] Layout(Scene scene){
   var signs=All<CatalogPlacement>(scene).Where(Sign).ToArray();var plans=new List<Plan>();
   Plan Add(string id,int variant,Vector3 at,Vector3 read,string purpose,Vector3 target,int arrow=0){var p=new Plan{id=id,variant=variant,sign=signs.SingleOrDefault(s=>s.placementId==id),position=at,read=read,purpose=purpose,target=target,arrow=arrow};plans.Add(p);return p;}
   var home=All<CatalogPlacement>(scene).Where(p=>(p.placementId??"").StartsWith("pinefall-home-",StringComparison.Ordinal)).OrderBy(p=>(Flat(p.transform.position)-new Vector3(-82,0,44)).sqrMagnitude).First();var entrance=home.GetComponentsInChildren<AssetAnchor>().Single(a=>a.role==AnchorRole.Entrance).transform.position;
   var depot=new Vector3(-94,0,-90);var town=Vector3.zero;
   Add("pinefall-sign-07",3,new Vector3(-72,0,-98),new Vector3(-72,0,-90),"One snow depot sign at depot driveway",depot,1);
   var fuel=Add("pinefall-sign-10",10,new Vector3(-113,0,-78),new Vector3(-104,0,-78),"One standing Fuel + Repair board beside red fuel tank",new Vector3(-112,0,-85));fuel.rows.Add(new Row(2.15f,"FUEL + REPAIR"));
   Add("pinefall-sign-14",10,new Vector3(-119,0,-94),new Vector3(-111,0,-94),"One Workshop Parts + Service board beside workshop",depot).rows.Add(new Row(2.15f,"WORKSHOP\nPARTS + SERVICE"));
   var lodge=Add("pinefall-sign-06",2,new Vector3(-73,0,8),new Vector3(-65,0,8),"One Alpine Lodge sign beside existing "+home.placementId,entrance,1);lodge.rows.Add(new Row(2.2f,"ALPINE LODGE"));lodge.rows.Add(new Row(1.96f,"> "+(Mathf.RoundToInt(Flat(entrance-lodge.read).magnitude/5)*5)+" m"));
   Add("pinefall-sign-12",8,new Vector3(12,0,-7.5f),new Vector3(12,0,0),"One Town Center arrow west along central road",town,1);
   Add("pinefall-sign-15",11,new Vector3(13,0,73),new Vector3(13,0,65),"One Alpine Market beside northern house block",new Vector3(22,0,82));
   var directory=Add("pinefall-sign-05",1,new Vector3(-7.5f,0,7.5f),town,"One stacked directory at town-center junction",town);directory.rows.AddRange(new[]{new Row(2.96f,"PINEFALL"),new Row(2.43f,"SNOW DEPOT"),new Row(1.90f,"WORKSHOP"),new Row(1.37f,"TOWN CENTER")});
   var roads=All<Renderer>(scene).Where(r=>r.name=="Road | asphalt"&&(r.bounds.size.x>150||r.bounds.size.z>150)).Select(r=>r.bounds).OrderBy(b=>b.center.z).ThenBy(b=>b.center.x).ToArray();Require(roads.Length==6,"expected six through-roads (twelve outer ends)");int number=0;
   foreach(var road in roads)foreach(int side in new[]{-1,1}){bool x=road.size.x>road.size.z;var at=road.center;at.y=0;var read=at;if(x){at.x=side<0?road.min.x-6:road.max.x+6;read.x=side<0?road.min.x+5:road.max.x-5;}else{at.z=side<0?road.min.z-6:road.max.z+6;read.z=side<0?road.min.z+5:road.max.z-5;}number++;Add(number==1?"pinefall-sign-09":"pinefall-sign-road-end-"+number.ToString("D2"),5,at,read,"Road Closed at outer road end "+number,at);}
   for(int i=0;i<12;i++){float angle=i*Mathf.PI*2/12;var at=new Vector3(Mathf.Cos(angle)*165,0,Mathf.Sin(angle)*165);var p=Add(i==0?"pinefall-sign-08":"pinefall-sign-boundary-"+(i+1).ToString("D2"),4,at,at.normalized*155,"Alpine Danger boundary board "+(i+1),town);p.boundary=true;if(p.sign)p.read=Flat(p.sign.transform.position)-Flat(p.sign.transform.position).normalized*10;}
   return plans.ToArray();
  }
  static Quaternion Facing(CatalogPlacement p,Vector3 position,Vector3 read){var face=p.GetComponentsInChildren<AssetAnchor>().Single(a=>a.id=="label-face").transform;var localFront=p.transform.InverseTransformDirection(-face.forward);return Quaternion.LookRotation(Flat(read-position).normalized,Vector3.up)*Quaternion.Inverse(Quaternion.LookRotation(localFront,Vector3.up));}
  static void Caption(Plan plan){
   foreach(var t in plan.sign.GetComponentsInChildren<TextMesh>(true).ToArray())UnityEngine.Object.DestroyImmediate(t.gameObject);
   foreach(var r in plan.sign.GetComponentsInChildren<MeshRenderer>(true).Where(r=>r.name=="Lettering")){r.gameObject.SetActive(plan.rows.Count==0);PrefabUtility.RecordPrefabInstancePropertyModifications(r.gameObject);}
   if(plan.rows.Count==0)return;var anchor=plan.sign.GetComponentsInChildren<AssetAnchor>().Single(a=>a.id=="label-face").transform;
   foreach(var row in plan.rows){var text=new GameObject("Label | destination-aware scene caption").AddComponent<TextMesh>();text.transform.SetParent(anchor,false);text.transform.position=anchor.position+Vector3.up*(row.height-anchor.localPosition.y);text.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");text.text=row.text;text.anchor=TextAnchor.MiddleCenter;text.alignment=TextAlignment.Center;text.fontSize=48;text.characterSize=.13f;text.color=new Color(.96f,.96f,.87f);text.GetComponent<MeshRenderer>().sharedMaterial=text.font.material;}
  }
  static Bounds[] canopies=new Bounds[0];
  static void CacheCanopies(Scene scene){canopies=All<CatalogPlacement>(scene).Where(p=>(p.assetId??"").StartsWith("alpine-tree-",StringComparison.Ordinal)).Select(p=>{var renderers=p.GetComponentsInChildren<Renderer>();var b=renderers[0].bounds;foreach(var r in renderers.Skip(1))b.Encapsulate(r.bounds);return b;}).ToArray();}
  static bool ClearView(Scene scene,Plan plan,Vector3 position,Quaternion rotation,bool ignoreOtherSigns=true){
   var terrain=All<Terrain>(scene).Single();var eye=plan.read;eye.y=terrain.SampleHeight(eye)+terrain.transform.position.y+1.8f;var face=plan.sign.GetComponentsInChildren<AssetAnchor>().Single(a=>a.id=="label-face").transform;var end=position+rotation*plan.sign.transform.InverseTransformPoint(face.position);var ray=end-eye;
   if(canopies.Any(b=>b.IntersectRay(new Ray(eye,ray.normalized),out float hit)&&hit<ray.magnitude-.03f))return false;
   return !Physics.RaycastAll(eye,ray.normalized,ray.magnitude-.03f,~0,QueryTriggerInteraction.Ignore).Any(h=>!h.collider.GetComponentInParent<SnowField>()&&!h.collider.GetComponentInParent<MachineController>()&&(!Sign(h.collider.GetComponentInParent<CatalogPlacement>())||(!ignoreOtherSigns&&h.collider.GetComponentInParent<CatalogPlacement>()!=plan.sign)));
  }
  public static void Apply(Scene scene){
   if(IsometricLayoutAuthoring.Applied(scene)){IsometricLayoutAuthoring.ValidateSigns(scene);return;}
   if(All<Transform>(scene).Any(t=>t.name==Marker)){Validate(scene);return;}
   Directory.CreateDirectory("Logs/AlpineSignBoards");if(!File.Exists("Logs/AlpineSignBoards/Pinefall-before-curated-signs.unity"))File.Copy(scene.path,"Logs/AlpineSignBoards/Pinefall-before-curated-signs.unity");
   CacheCanopies(scene);var plans=Layout(scene);var allowed=new HashSet<string>(plans.Select(p=>p.id));var existing=All<CatalogPlacement>(scene).Where(Sign).ToArray();var parent=existing.First().transform.parent;
   foreach(var p in plans){if(!p.sign){var a=CatalogPreparation.Library().Find("alpine-sign-"+p.variant.ToString("D2"));var go=(GameObject)PrefabUtility.InstantiatePrefab(a.prefab,scene);go.transform.SetParent(parent,false);p.sign=go.GetComponent<CatalogPlacement>();p.sign.placementId=p.id;PrefabUtility.RecordPrefabInstancePropertyModifications(p.sign);}Require(p.sign.assetId=="alpine-sign-"+p.variant.ToString("D2"),"wrong retained variant "+p.id);}
   var terrain=All<Terrain>(scene).Single();var roads=All<Renderer>(scene).Where(r=>r.name=="Road | asphalt").Select(r=>r.bounds).ToArray();var blocked=All<Collider>(scene).Where(c=>c.enabled&&c.gameObject.activeInHierarchy&&!c.isTrigger&&!(c is TerrainCollider)&&!c.GetComponentInParent<SnowField>()&&!c.GetComponentInParent<MachineController>()&&!Sign(c.GetComponentInParent<CatalogPlacement>())).Select(c=>c.bounds).ToList();blocked.AddRange(canopies);
   foreach(var plan in plans){if(plan.wall){plan.rotation=Facing(plan.sign,plan.position,plan.read);var anchor=plan.sign.GetComponentsInChildren<AssetAnchor>().Single(a=>a.id=="mount").transform;var offset=plan.sign.transform.InverseTransformPoint(anchor.position);plan.position-=plan.rotation*offset;continue;}
    bool found=false;var wanted=plan.position;
    for(int i=0;i<(plan.boundary?257:129);i++){float angle=i*Mathf.PI*2/16,radius=i==0?0:.5f+(i-1)/16*.5f;var position=wanted+new Vector3(Mathf.Cos(angle)*radius,0,Mathf.Sin(angle)*radius);position.y=terrain.SampleHeight(position)+terrain.transform.position.y-.015f;if(plan.boundary&&Mathf.Abs(Flat(position).magnitude-165)>4.5f)continue;if(plan.boundary)plan.read=Flat(position)-Flat(position).normalized*10;var rotation=Facing(plan.sign,position,plan.read);var b=Bounds(plan.sign,position,rotation);if(roads.Any(r=>XZ(b,r,.75f))||blocked.Any(o=>b.Intersects(o))||!ClearView(scene,plan,position,rotation))continue;
     if(plan.arrow!=0){var face=plan.sign.GetComponentsInChildren<AssetAnchor>().Single(a=>a.id=="label-face").transform;var direction=rotation*plan.sign.transform.InverseTransformDirection(face.right)*plan.arrow;if(Vector3.Dot(Flat(plan.target-position).normalized,direction.normalized)<.75f)continue;}
     plan.position=position;plan.rotation=rotation;blocked.Add(b);found=true;break;
    }Require(found,"no clear readable verge for "+plan.id+" ("+plan.purpose+")");
   }
   foreach(var old in existing.Where(p=>!allowed.Contains(p.placementId)))UnityEngine.Object.DestroyImmediate(old.gameObject);
   foreach(var plan in plans){Undo.RecordObject(plan.sign.transform,"Curate road-facing town signs");plan.sign.transform.SetPositionAndRotation(plan.position,plan.rotation);PrefabUtility.RecordPrefabInstancePropertyModifications(plan.sign.transform);Caption(plan);}
   foreach(var old in All<Transform>(scene).Where(t=>t.name.StartsWith("Wayfinding |",StringComparison.Ordinal)).ToArray())UnityEngine.Object.DestroyImmediate(old.gameObject);var marker=new GameObject(Marker);SceneManager.MoveGameObjectToScene(marker,scene);Physics.SyncTransforms();Validate(scene);
  }
  public static Vector3 Reader(Scene scene,string id)=>IsometricLayoutAuthoring.TryReader(scene,id,out var reader)?reader:Layout(scene).Single(p=>p.id==id).read;
  public static void Validate(Scene scene){
   if(IsometricLayoutAuthoring.Applied(scene)){IsometricLayoutAuthoring.ValidateSigns(scene);return;}
   CacheCanopies(scene);var plans=Layout(scene);var signs=All<CatalogPlacement>(scene).Where(Sign).ToArray();Require(signs.Length==31&&plans.All(p=>p.sign)&&signs.All(p=>plans.Any(x=>x.id==p.placementId)),"expected only 31 curated signs");var lines=new List<string>();
   foreach(var p in plans){var face=p.sign.GetComponentsInChildren<AssetAnchor>().Single(a=>a.id=="label-face").transform;var outward=Flat(-face.forward).normalized;float facing=Vector3.Dot(outward,Flat(p.read-p.sign.transform.position).normalized);Require(facing>.96f,"not facing road/junction/town: "+p.id);Require(p.rows.All(row=>p.sign.GetComponentsInChildren<TextMesh>().Any(t=>t.text==row.text)),"missing caption "+p.id);
    if(!p.boundary)Require(All<Renderer>(scene).Where(r=>r.name=="Road | asphalt").Any(r=>p.read.x>=r.bounds.min.x&&p.read.x<=r.bounds.max.x&&p.read.z>=r.bounds.min.z&&p.read.z<=r.bounds.max.z),"reading point not on road "+p.id);
    float arrow=0;if(p.arrow!=0){arrow=Vector3.Dot(Flat(face.right*p.arrow).normalized,Flat(p.target-p.sign.transform.position).normalized);Require(arrow>=.75f,"arrow points away from destination "+p.id);}Require(ClearView(scene,p,p.sign.transform.position,p.sign.transform.rotation,false),"blocked sign sightline "+p.id);
    lines.Add(p.id+" | "+p.purpose+" | at "+p.sign.transform.position+" | reader "+p.read+" | target "+p.target+" | facing dot "+facing.ToString("F3")+" | arrow dot "+arrow.ToString("F3"));
   }
   var danger=plans.Where(p=>p.boundary).Select(p=>Flat(p.sign.transform.position)).ToArray();Require(danger.Length==12&&danger.All(p=>Mathf.Abs(p.magnitude-165)<4.6f),"danger ring count/radius");var angles=danger.Select(p=>Mathf.Repeat(Mathf.Atan2(p.z,p.x)*Mathf.Rad2Deg,360)).OrderBy(a=>a).ToArray();for(int i=0;i<12;i++)Require(Mathf.Abs(Mathf.Repeat(angles[(i+1)%12]-angles[i],360)-30)<3.3f,"uneven danger spacing");
   File.WriteAllLines("Logs/AlpineSignBoards/Wayfinding.txt",lines);
  }
 }
}

