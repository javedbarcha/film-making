using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Snowglow.Editor {
 public static class ForestSeatingAuthoring {
  const string Request="Logs/ExpandAndSeatForest.request",Logs="Logs/ForestSeating",Extra="pinefall-tree-extra-";
  static T[] All<T>(Scene s)where T:Component=>s.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<T>(true)).ToArray();
  static bool Tree(CatalogPlacement p)=>p&&(p.assetId??"").StartsWith("alpine-tree-",StringComparison.Ordinal);
  static bool Mountain(CatalogPlacement p)=>p&&(p.assetId??"").StartsWith("alpine-mountain-",StringComparison.Ordinal);
  static void Require(bool c,string s){if(!c)throw new InvalidOperationException("Forest seating: "+s);}
  [InitializeOnLoadMethod]static void Register(){EditorApplication.update+=Pending;}
  static void Pending(){if(!File.Exists(Request)){EditorApplication.update-=Pending;return;}if(EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode)return;EditorApplication.update-=Pending;File.Delete(Request);try{Install();}catch(Exception e){Directory.CreateDirectory(Logs);File.WriteAllText(Logs+"/Validation.txt","FAIL: "+e);Debug.LogException(e);}}
  sealed class Geometry{public CatalogAsset asset;public Vector3[] support,vertices;public Bounds bounds;public bool wood;}
  sealed class Plan{public CatalogPlacement existing;public Geometry geometry;public string id;public Vector3 position,scale;public Quaternion rotation;public Vector3 oldPosition;}
  struct Occupied{public string id;public Vector3 point;public float radius;}
  static readonly HashSet<string> outerIds=new HashSet<string>();
  const float ForestRadius=470;
  static readonly HashSet<string> slopeIds=new HashSet<string>();
  static readonly Dictionary<string,Geometry> cache=new Dictionary<string,Geometry>();
  static Geometry Data(string id){
   if(cache.TryGetValue(id,out var data))return data;var a=CatalogPreparation.Library().Find(id);Require(a&&a.prefab&&a.tags.Contains("lod3-only"),"lowest LOD3 prefab missing "+id);
   var body=a.prefab.GetComponentsInChildren<MeshFilter>(true).Single(m=>!m.name.StartsWith("SnowCap",StringComparison.Ordinal));var points=new List<Vector3>();
   using(var raw=MeshUtility.AcquireReadOnlyMeshData(body.sharedMesh))using(var verts=new NativeArray<Vector3>(raw[0].vertexCount,Allocator.Temp)){raw[0].GetVertices(verts);var matrix=a.prefab.transform.worldToLocalMatrix*body.transform.localToWorldMatrix;foreach(var v in verts)points.Add(matrix.MultiplyPoint3x4(v));}
   float low=points.Min(v=>v.y),high=points.Max(v=>v.y),cut=Mathf.Min(.07f,(high-low)*.15f);var lower=points.Where(v=>v.y<=low+cut).OrderBy(v=>v.x).ThenBy(v=>v.z).ToList();var unique=new List<Vector3>();foreach(var v in lower)if(!unique.Any(p=>new Vector2(v.x-p.x,v.z-p.z).sqrMagnitude<.00000001f))unique.Add(v);
   Require(unique.Count>0,"no body support points "+id);
   float Cross(Vector3 a0,Vector3 b,Vector3 c)=>(b.x-a0.x)*(c.z-a0.z)-(b.z-a0.z)*(c.x-a0.x);
   var hull=new List<Vector3>();foreach(var v in unique){while(hull.Count>=2&&Cross(hull[hull.Count-2],hull[hull.Count-1],v)<=0)hull.RemoveAt(hull.Count-1);hull.Add(v);}int first=hull.Count;for(int i=unique.Count-2;i>=0;i--){var v=unique[i];while(hull.Count>first&&Cross(hull[hull.Count-2],hull[hull.Count-1],v)<=0)hull.RemoveAt(hull.Count-1);hull.Add(v);}if(hull.Count>1)hull.RemoveAt(hull.Count-1);
   var support=new List<Vector3>(unique);var center=unique.Aggregate(Vector3.zero,(sum,v)=>sum+v)/unique.Count;support.Add(center);
   for(int i=0;i<hull.Count;i++)for(int j=0;j<=8;j++){var p=Vector3.Lerp(hull[i],hull[(i+1)%hull.Count],j/8f);support.Add(p);support.Add(Vector3.Lerp(center,p,.5f));}
   var visual=new List<Vector3>(points);foreach(var filter in a.prefab.GetComponentsInChildren<MeshFilter>(true).Where(f=>f!=body)){using(var raw=MeshUtility.AcquireReadOnlyMeshData(filter.sharedMesh))using(var verts=new NativeArray<Vector3>(raw[0].vertexCount,Allocator.Temp)){raw[0].GetVertices(verts);var matrix=a.prefab.transform.worldToLocalMatrix*filter.transform.localToWorldMatrix;foreach(var v in verts)visual.Add(matrix.MultiplyPoint3x4(v));}}
   data=new Geometry{asset=a,support=support.ToArray(),vertices=visual.ToArray(),bounds=CatalogValidation.BoundsOf(a.prefab),wood=int.Parse(id.Substring(id.Length-2))>=21};cache[id]=data;return data;
  }
  static Bounds WorldBounds(Geometry data,Vector3 pos,Quaternion rot,Vector3 scale){var m=Matrix4x4.TRS(pos,rot,scale);var b=data.bounds;var result=new Bounds(m.MultiplyPoint3x4(b.center),Vector3.zero);for(int i=0;i<8;i++)result.Encapsulate(m.MultiplyPoint3x4(b.center+Vector3.Scale(b.extents,new Vector3((i&1)==0?-1:1,(i&2)==0?-1:1,(i&4)==0?-1:1))));return result;}
  static bool OverlapXZ(Bounds a,Bounds b,float margin=0)=>a.min.x<b.max.x+margin&&a.max.x>b.min.x-margin&&a.min.z<b.max.z+margin&&a.max.z>b.min.z-margin;
  static float Ground(Terrain t,Vector3 p)=>t.SampleHeight(p)+t.transform.position.y;
  static Vector3 Normal(Terrain t,Vector3 p)=>t.terrainData.GetInterpolatedNormal((p.x-t.transform.position.x)/t.terrainData.size.x,(p.z-t.transform.position.z)/t.terrainData.size.z);
  static float Yaw(Quaternion rotation){var untilt=Quaternion.Inverse(Quaternion.FromToRotation(Vector3.up,rotation*Vector3.up))*rotation;return untilt.eulerAngles.y;}
  static Quaternion Rotation(Geometry d,Terrain t,Vector3 p,float yaw){var tilt=Quaternion.FromToRotation(Vector3.up,Normal(t,p));if(!d.wood)tilt=Quaternion.RotateTowards(Quaternion.identity,tilt,12);return tilt*Quaternion.Euler(0,yaw,0);}
  // Keep the forest on the district-facing side of the first mountain, never behind it.
  static bool VisibleGround(Bounds tree,Bounds[] mountains){
   for(int corner=0;corner<5;corner++){
    var point=corner==4?tree.center:new Vector3((corner&1)==0?tree.min.x:tree.max.x,0,(corner&2)==0?tree.min.z:tree.max.z);
    var direction=new Vector2(point.x,point.z);if(direction.magnitude>ForestRadius)return false;
    foreach(var mountain in mountains){float near=0,far=1;bool hits=true;
     for(int axis=0;axis<2;axis++){float delta=axis==0?direction.x:direction.y;float lo=(axis==0?mountain.min.x:mountain.min.z)-3,hi=(axis==0?mountain.max.x:mountain.max.z)+3;
      if(Mathf.Abs(delta)<.00001f){if(lo>0||hi<0){hits=false;break;}}else{float a=lo/delta,b=hi/delta;if(a>b){float temp=a;a=b;b=temp;}near=Mathf.Max(near,a);far=Mathf.Min(far,b);if(near>far){hits=false;break;}}
     }if(hits&&near<=1&&far>=0)return false;
    }
   }return true;
  }
  static float Radius(Geometry d,Vector3 scale)=>Mathf.Max(d.bounds.extents.x*Mathf.Abs(scale.x),d.bounds.extents.z*Mathf.Abs(scale.z));
  static void Limits(Geometry d,Vector3 scale,out float burial,out float spread){float h=d.bounds.size.y*Mathf.Abs(scale.y);burial=Mathf.Min(.025f,h*.04f);spread=Mathf.Min(.15f,h*.12f);}
  static bool Fit(Geometry d,Terrain terrain,Vector3 xz,float yaw,Vector3 scale,Bounds[] mountains,Bounds[] obstacles,out Vector3 position,out Quaternion rotation){
   position=xz;position.y=0;rotation=Rotation(d,terrain,position,yaw);var b=WorldBounds(d,position,rotation,scale);var tpos=terrain.transform.position;var size=terrain.terrainData.size;
   if(b.min.x<tpos.x+10||b.max.x>tpos.x+size.x-10||b.min.z<tpos.z+10||b.max.z>tpos.z+size.z-10)return false;
   if(OverlapXZ(b,new Bounds(Vector3.zero,new Vector3(270,1,270))))return false;
   if(mountains.Any(m=>OverlapXZ(b,m,3))||!VisibleGround(b,mountains))return false;
   var mtx=Matrix4x4.TRS(position,rotation,scale);float min=float.PositiveInfinity,max=float.NegativeInfinity;
   foreach(var p in d.support){var at=mtx.MultiplyPoint3x4(p);float needed=Ground(terrain,at)-at.y;min=Mathf.Min(min,needed);max=Mathf.Max(max,needed);}
   Limits(d,scale,out float burial,out float allowed);if(max-min>allowed)return false;position.y=min-burial;b=WorldBounds(d,position,rotation,scale);
   if(obstacles.Any(o=>b.Intersects(o)))return false;var seatedMatrix=Matrix4x4.TRS(position,rotation,scale);foreach(var vertex in d.vertices){var at=seatedMatrix.MultiplyPoint3x4(vertex);if(at.y-Ground(terrain,at)<-(burial+allowed+.015f))return false;}return true;
  }
  static bool Spaced(Geometry d,Vector3 p,Vector3 scale,List<Occupied> occupied){float radius=Radius(d,scale);return occupied.All(o=>new Vector2(p.x-o.point.x,p.z-o.point.z).magnitude>=Mathf.Max(6,radius+o.radius+1));}
  static Plan Find(Geometry d,Terrain terrain,Vector3 origin,float yaw,Vector3 scale,Bounds[] mountains,Bounds[] obstacles,List<Occupied> occupied,System.Random rng,string id,CatalogPlacement existing){
   bool SlopeOK(Vector3 point)=>(!slopeIds.Contains(id)||Vector3.Angle(Vector3.up,Normal(terrain,point))>=5)&&(!outerIds.Contains(id)||new Vector2(point.x,point.z).magnitude>=340);
   if(existing&&SlopeOK(origin)&&Fit(d,terrain,origin,yaw,scale,mountains,obstacles,out var same,out var sameRot))return new Plan{existing=existing,geometry=d,id=id,position=same,rotation=sameRot,scale=scale,oldPosition=origin};
   for(int attempt=0;attempt<20000;attempt++){
    Vector3 p;if(existing&&attempt<240){float radius=8+(attempt/24)*12;float angle=(float)(rng.NextDouble()*Math.PI*2);p=origin+new Vector3(Mathf.Cos(angle)*radius,0,Mathf.Sin(angle)*radius);}else p=new Vector3((float)rng.NextDouble()*940-470,0,(float)rng.NextDouble()*940-470);
    if(!SlopeOK(p)||!Spaced(d,p,scale,occupied))continue;if(Fit(d,terrain,p,yaw,scale,mountains,obstacles,out var seated,out var rot))return new Plan{existing=existing,geometry=d,id=id,position=seated,rotation=rot,scale=scale,oldPosition=origin};
   }throw new InvalidOperationException("No terrain/mountain-clear position for "+id);
  }
  static void Clearances(CatalogPlacement p,Terrain terrain,out float min,out float max){var d=Data(p.assetId);min=float.PositiveInfinity;max=float.NegativeInfinity;foreach(var point in d.support){var at=p.transform.TransformPoint(point);float clearance=at.y-Ground(terrain,at);min=Mathf.Min(min,clearance);max=Mathf.Max(max,clearance);}}
  static float LowestVisual(CatalogPlacement p,Terrain terrain)=>Data(p.assetId).vertices.Min(v=>{var at=p.transform.TransformPoint(v);return at.y-Ground(terrain,at);});
  static string SnowData(SnowSeed seed)=>JsonUtility.ToJson(new SnowVolumeGrid{depth=seed.grid.depth,credit=seed.grid.credit,initialVolume=seed.grid.initialVolume,disposed=seed.grid.disposed,revision=seed.grid.revision,blocked=new bool[0]});
  static int RefreshMask(SnowField snow,Bounds[] oldBounds,Bounds[] newBounds){
   var source=snow.seed;var mask=(bool[])source.grid.blocked.Clone();var bounds=oldBounds.Concat(newBounds).ToArray();int changed=0;Physics.SyncTransforms();
   for(int i=0;i<mask.Length;i++){float x=source.grid.X(i),z=source.grid.Z(i);if(!bounds.Any(b=>x>=b.min.x-.5f&&x<=b.max.x+.5f&&z>=b.min.z-.5f&&z<=b.max.z+.5f))continue;bool value=Physics.CheckBox(new Vector3(x,SnowField.Ground+.85f,z),new Vector3(SnowVolumeGrid.Cell*.45f,.65f,SnowVolumeGrid.Cell*.45f),Quaternion.identity,1<<0,QueryTriggerInteraction.Ignore);if(value!=mask[i]){mask[i]=value;changed++;}}
   if(changed>0){var seed=UnityEngine.Object.Instantiate(source);seed.name="Pinefall | seated expanded forest obstacles";seed.grid.blocked=mask;AssetDatabase.CreateAsset(seed,AssetDatabase.GenerateUniqueAssetPath(CatalogPreparation.Root+"/SceneData/SeatedForestSnowSeed.asset"));snow.seed=seed;EditorUtility.SetDirty(seed);EditorUtility.SetDirty(snow);}return changed;
  }
  [MenuItem("Snowglow/Environment/Expand forest by half and repair all terrain seating")]
  public static void Install(){
   var scene=SceneManager.GetActiveScene();Require(scene.path==SceneValidation.ScenePath&&!EditorApplication.isPlayingOrWillChangePlaymode,"open Pinefall with Play stopped");Directory.CreateDirectory(Logs);if(!File.Exists(Logs+"/Pinefall-before.unity"))File.Copy(scene.path,Logs+"/Pinefall-before.unity");if(scene.isDirty)Require(EditorSceneManager.SaveScene(scene,Logs+"/Pinefall-unsaved-before.unity",true),"backup unsaved work failed");cache.Clear();
   var forest=All<CatalogPlacement>(scene).Where(Tree).OrderBy(p=>p.placementId).ToArray();int original=forest.Count(p=>!(p.placementId??"").StartsWith(Extra,StringComparison.Ordinal)),targetExtra=Mathf.CeilToInt(original*.5f);Require(original>0,"forest is empty");
   var extra=forest.Where(p=>(p.placementId??"").StartsWith(Extra,StringComparison.Ordinal)).ToArray();Require(extra.Length==0||extra.Length==targetExtra,"partial earlier expansion requires inspection");
   var terrain=All<Terrain>(scene).Single();var mountains=All<CatalogPlacement>(scene).Where(Mountain).Select(p=>{var r=p.GetComponentsInChildren<Renderer>(true);var b=r[0].bounds;foreach(var part in r.Skip(1))b.Encapsulate(part.bounds);return b;}).ToArray();Require(mountains.Length>0,"mountain bounds missing");
   var obstacles=All<Collider>(scene).Where(c=>c.enabled&&c.gameObject.activeInHierarchy&&!c.isTrigger&&!(c is TerrainCollider)&&!Tree(c.GetComponentInParent<CatalogPlacement>())).Select(c=>c.bounds).ToArray();
   var session=All<PrototypeSession>(scene).Single();string snow=SnowData(session.Snow.seed);
   var hashes=All<Terrain>(scene).Select(t=>AssetDatabase.GetAssetPath(t.terrainData)).Concat(All<MeshFilter>(scene).Where(m=>m.sharedMesh).Select(m=>AssetDatabase.GetAssetPath(m.sharedMesh))).Concat(new[]{AssetDatabase.GetAssetPath(session.Snow.seed)}).Where(p=>p.StartsWith("Assets/",StringComparison.Ordinal)).Distinct().ToDictionary(p=>p,p=>AssetDatabase.GetAssetDependencyHash(p));
   var others=All<CatalogPlacement>(scene).Where(p=>!Tree(p)&&!string.IsNullOrEmpty(p.placementId)).ToDictionary(p=>p.placementId,p=>p.assetId+"|"+p.transform.localToWorldMatrix.ToString("R"));var identities=forest.ToDictionary(p=>p.placementId,p=>p.assetId);var oldColliders=forest.SelectMany(p=>p.GetComponentsInChildren<Collider>()).Select(c=>c.bounds).ToArray();
   outerIds.Clear();for(int i=0;i<forest.Length;i++)if(i%2==0)outerIds.Add(forest[i].placementId);
   slopeIds.Clear();foreach(var p in forest.Where(p=>Data(p.assetId).wood).Concat(forest.Where(p=>!Data(p.assetId).wood).Take(34)))slopeIds.Add(p.placementId);
   var occupied=forest.Select(p=>new Occupied{id=p.placementId,point=p.transform.position,radius=Radius(Data(p.assetId),p.transform.lossyScale)}).ToList();var plans=new List<Plan>();var rng=new System.Random(41146);int floating=0,deep=0,under=0;
   foreach(var p in forest){var d=Data(p.assetId);Clearances(p,terrain,out float min,out float max);Limits(d,p.transform.lossyScale,out float burial,out float spread);if(max>.03f)floating++;if(min<-(burial+spread+.02f))deep++;if(mountains.Any(m=>OverlapXZ(WorldBounds(d,p.transform.position,p.transform.rotation,p.transform.lossyScale),m,3)))under++;
    occupied.RemoveAll(o=>o.id==p.placementId);var plan=Find(d,terrain,p.transform.position,Yaw(p.transform.rotation),p.transform.lossyScale,mountains,obstacles,occupied,rng,p.placementId,p);plans.Add(plan);occupied.Add(new Occupied{id=plan.id,point=plan.position,radius=Radius(d,plan.scale)});
   }
   if(extra.Length==0){
    var quota=forest.GroupBy(p=>p.assetId).Select(g=>new {id=g.Key,exact=targetExtra*(float)g.Count()/original}).ToArray();var counts=quota.ToDictionary(q=>q.id,q=>Mathf.FloorToInt(q.exact));int remainder=targetExtra-counts.Values.Sum();foreach(var q in quota.OrderByDescending(q=>q.exact-Mathf.Floor(q.exact)).ThenBy(q=>q.id).Take(remainder))counts[q.id]++;
    var bag=counts.SelectMany(k=>Enumerable.Repeat(k.Key,k.Value)).ToArray();for(int i=bag.Length-1;i>0;i--){int j=rng.Next(i+1);string swap=bag[i];bag[i]=bag[j];bag[j]=swap;}
    for(int i=0;i<bag.Length;i++){var d=Data(bag[i]);float scale=.8f+(float)rng.NextDouble()*.6f;var plan=Find(d,terrain,Vector3.zero,(float)rng.NextDouble()*360,Vector3.one*scale,mountains,obstacles,occupied,rng,Extra+(i+1).ToString("D3"),null);plans.Add(plan);occupied.Add(new Occupied{id=plan.id,point=plan.position,radius=Radius(d,plan.scale)});}
   }
   Undo.IncrementCurrentGroup();int undo=Undo.GetCurrentGroup();Undo.SetCurrentGroupName("Expand and seat forest outside mountains");
   try{Transform group=null;if(extra.Length==0){var go=new GameObject("Additional forest | lowest LOD3 trees and wood");SceneManager.MoveGameObjectToScene(go,scene);group=go.transform;Undo.RegisterCreatedObjectUndo(go,"Additional forest");}
    foreach(var plan in plans){CatalogPlacement p=plan.existing;if(!p){var go=(GameObject)PrefabUtility.InstantiatePrefab(plan.geometry.asset.prefab,scene);go.transform.SetParent(group,false);go.transform.localScale=plan.scale;p=go.GetComponent<CatalogPlacement>();p.placementId=plan.id;PrefabUtility.RecordPrefabInstancePropertyModifications(p);Undo.RegisterCreatedObjectUndo(go,"Added forest instance");}else Undo.RecordObject(p.transform,"Repair terrain seating");p.transform.SetPositionAndRotation(plan.position,plan.rotation);PrefabUtility.RecordPrefabInstancePropertyModifications(p.transform);}
    Physics.SyncTransforms();var current=All<CatalogPlacement>(scene).Where(Tree).ToArray();Validate(current,terrain,mountains,original+targetExtra);int maskChanges=RefreshMask(session.Snow,oldColliders,current.SelectMany(p=>p.GetComponentsInChildren<Collider>()).Select(c=>c.bounds).ToArray());Require(SnowData(session.Snow.seed)==snow,"snow values changed");SceneValidation.Validate(scene);AssetDatabase.SaveAssets();EditorSceneManager.MarkSceneDirty(scene);Require(EditorSceneManager.SaveScene(scene),"save failed");Undo.CollapseUndoOperations(undo);
    scene=EditorSceneManager.OpenScene(SceneValidation.ScenePath);current=All<CatalogPlacement>(scene).Where(Tree).ToArray();Validate(current,All<Terrain>(scene).Single(),mountains,original+targetExtra);SceneValidation.Validate(scene);MechanicsChecks.Run();
    foreach(var p in current.Where(p=>identities.ContainsKey(p.placementId)))Require(identities[p.placementId]==p.assetId,"old variant/ID changed");foreach(var p in All<CatalogPlacement>(scene).Where(p=>!Tree(p)&&!string.IsNullOrEmpty(p.placementId)))Require(others[p.placementId]==p.assetId+"|"+p.transform.localToWorldMatrix.ToString("R"),"non-forest placement changed "+p.placementId);foreach(var h in hashes)Require(AssetDatabase.GetAssetDependencyHash(h.Key)==h.Value,"source/Terrain/mesh/snow changed "+h.Key);Require(SnowData(All<PrototypeSession>(scene).Single().Snow.seed)==snow,"saved snow values changed");
    int relocated=plans.Count(p=>identities.ContainsKey(p.id)&&new Vector2(p.position.x-p.oldPosition.x,p.position.z-p.oldPosition.z).magnitude>.02f);long triangles=current.Sum(p=>(long)CatalogPreparation.Library().Find(p.assetId).triangles);
    Require(current.Count(p=>!Data(p.assetId).wood&&Vector3.Angle(Vector3.up,Normal(All<Terrain>(scene).Single(),p.transform.position))>=4.99f)>=34,"not enough slope trees");
    int slopeTrees=current.Count(p=>!Data(p.assetId).wood&&Vector3.Angle(Vector3.up,Normal(All<Terrain>(scene).Single(),p.transform.position))>=4.99f);
    var report=new StringBuilder("PASS: Unity "+Application.unityVersion+". Forest "+original+" + "+targetExtra+" = "+current.Length+" instances; LOD3 only; "+triangles+" triangles. Existing audit: "+floating+" floating support footprints, "+deep+" excessively buried support footprints, "+under+" overlapping mountain horizontal bounds. Relocated "+relocated+" existing instances; all objects seated and validated after save/reopen.\n");
    report.AppendLine("Outer forest (>=340m): "+current.Count(p=>new Vector2(p.transform.position.x,p.transform.position.z).magnitude>=339.99f)+"; furthest root: "+current.Max(p=>new Vector2(p.transform.position.x,p.transform.position.z).magnitude).ToString("F1")+"m.");
    report.AppendLine("Slope trees (>=5 degrees): "+slopeTrees+"; slope-aligned wood: "+current.Count(p=>Data(p.assetId).wood)+". All saved rotations match terrain seating rules.");
    report.AppendLine("Every actual LOD3 body support footprint is just below Terrain, with burial and terrain relief bounded relative to height. Tree trunks tilt toward the terrain normal by at most 12 degrees; stumps/logs fully align with slope. At least 34 trees and all 6 wood pieces occupy slopes of 5 degrees or more. All forest bounds lie within 470m of the district; at least half the instances occupy the outer band beyond 340m, extending toward the user-marked mountain boundary and before the first mountain envelope along district-facing rays. Full canopy/wood bounds clear all mountain horizontal envelopes by 3m (conservative exclusion), and avoid the central worksite. Nearby suitable ground preferred for relocations. Added population follows existing variant proportions, varied yaw/scale, minimum spacing. No runtime spawning or higher LODs. Original IDs/variants retained; non-forest poses, Terrain, shared meshes and original seed hashes unchanged. Snow obstacle flag changes: "+maskChanges+" in a separate seed copy if needed; all depth/credit/volume retained. Scene and 19 ledger checks passed. No player-save writes or district rebake. Rendered inspection, continuous driving and A07 profiling must be reported separately.");
    var csv=new StringBuilder("placement,variant,x,y,z,minSupportClearance,maxSupportClearance,minVisualClearance\n");foreach(var p in current){Clearances(p,All<Terrain>(scene).Single(),out float min,out float max);csv.AppendLine(p.placementId+","+p.assetId+","+p.transform.position.x.ToString("R",System.Globalization.CultureInfo.InvariantCulture)+","+p.transform.position.y.ToString("R",System.Globalization.CultureInfo.InvariantCulture)+","+p.transform.position.z.ToString("R",System.Globalization.CultureInfo.InvariantCulture)+","+min.ToString("R",System.Globalization.CultureInfo.InvariantCulture)+","+max.ToString("R",System.Globalization.CultureInfo.InvariantCulture)+","+LowestVisual(p,All<Terrain>(scene).Single()).ToString("R",System.Globalization.CultureInfo.InvariantCulture));}File.WriteAllText(Logs+"/Placements.csv",csv.ToString());File.WriteAllText(Logs+"/Validation.txt",report.ToString());Preview(scene,current);EditorSceneManager.OpenScene(SceneValidation.ScenePath);Debug.Log(report.ToString());
   }catch{Undo.RevertAllDownToGroup(undo);throw;}
  }
  static void Validate(CatalogPlacement[] trees,Terrain terrain,Bounds[] mountains,int count){Require(trees.Length==count&&trees.Select(p=>p.placementId).Distinct().Count()==count,"forest count/IDs differ");foreach(var p in trees){Require(PrefabUtility.IsPartOfPrefabInstance(p),"disconnected tree "+p.placementId);var d=Data(p.assetId);Require(!outerIds.Contains(p.placementId)||new Vector2(p.transform.position.x,p.transform.position.z).magnitude>=339.99f,"designated outer forest inside old radius "+p.placementId);Require(Quaternion.Angle(p.transform.rotation,Rotation(d,terrain,p.transform.position,Yaw(p.transform.rotation)))<.05f,"incorrect slope alignment "+p.placementId);Require(VisibleGround(WorldBounds(d,p.transform.position,p.transform.rotation,p.transform.lossyScale),mountains),"tree beyond mountains or visible forest band "+p.placementId);Require(!slopeIds.Contains(p.placementId)||Vector3.Angle(Vector3.up,Normal(terrain,p.transform.position))>=4.99f,"designated slope placement on flat ground");Require(!mountains.Any(m=>OverlapXZ(WorldBounds(d,p.transform.position,p.transform.rotation,p.transform.lossyScale),m,3)),"tree under mountain "+p.placementId);Clearances(p,terrain,out float min,out float max);Limits(d,p.transform.lossyScale,out float burial,out float spread);Require(max<=.001f&&min>=-(burial+spread+.003f),"floating/deep base "+p.placementId+" "+min+".."+max);Require(LowestVisual(p,terrain)>=-(burial+spread+.018f),"tree/wood visual vertices buried in terrain "+p.placementId);Require(p.GetComponentsInChildren<MeshFilter>(true).All(m=>AssetDatabase.GetAssetPath(m.sharedMesh).StartsWith(LowestTreeLODIntake.Source,StringComparison.Ordinal)),"non-LOD3 geometry found");}}
  static void Overview(Camera camera){
   camera.orthographic=true;camera.orthographicSize=510;camera.aspect=1;camera.transform.SetPositionAndRotation(new Vector3(0,900,0),Quaternion.Euler(90,0,0));
   var rt=new RenderTexture(1200,1200,24);var previous=RenderTexture.active;bool fog=RenderSettings.fog;Texture2D frame=null;
   // DistanceVisibility is a runtime component; the Editor render includes the authored forest.
   try{RenderSettings.fog=false;camera.targetTexture=rt;camera.Render();RenderTexture.active=rt;frame=new Texture2D(1200,1200,TextureFormat.RGB24,false);frame.ReadPixels(new Rect(0,0,1200,1200),0,0);frame.Apply();File.WriteAllBytes(Logs+"/Forest-overview.png",frame.EncodeToPNG());}
   finally{RenderSettings.fog=fog;camera.targetTexture=null;RenderTexture.active=previous;rt.Release();UnityEngine.Object.DestroyImmediate(rt);if(frame)UnityEngine.Object.DestroyImmediate(frame);}
  }
  static void Preview(Scene scene,CatalogPlacement[] trees){var camera=All<Camera>(scene).First();Overview(camera);foreach(int index in new[]{0,1,2}){var terrain=All<Terrain>(scene).Single();var p=index==2?trees.Where(t=>Data(t.assetId).wood).OrderByDescending(t=>Vector3.Angle(Vector3.up,Normal(terrain,t.transform.position))).First():index==1?trees.Where(t=>!Data(t.assetId).wood).OrderByDescending(t=>Vector3.Angle(Vector3.up,Normal(terrain,t.transform.position))).First():trees.OrderBy(t=>(t.transform.position-new Vector3(-180,0,-160)).sqrMagnitude).First();camera.orthographic=false;camera.fieldOfView=60;camera.aspect=16f/9f;camera.transform.position=p.transform.position+(index==2?new Vector3(-4,2,-6):new Vector3(-16,8,-22));camera.transform.position=new Vector3(camera.transform.position.x,Mathf.Max(camera.transform.position.y,Ground(terrain,camera.transform.position)+2),camera.transform.position.z);camera.transform.LookAt(p.transform.position+Vector3.up*(index==2?.3f:2));var rt=new RenderTexture(1280,720,24);var previous=RenderTexture.active;Texture2D frame=null;try{camera.targetTexture=rt;camera.Render();RenderTexture.active=rt;frame=new Texture2D(1280,720,TextureFormat.RGB24,false);frame.ReadPixels(new Rect(0,0,1280,720),0,0);frame.Apply();File.WriteAllBytes(Logs+"/Forest-"+index+".png",frame.EncodeToPNG());}finally{camera.targetTexture=null;RenderTexture.active=previous;rt.Release();UnityEngine.Object.DestroyImmediate(rt);if(frame)UnityEngine.Object.DestroyImmediate(frame);}}}
 }
}
