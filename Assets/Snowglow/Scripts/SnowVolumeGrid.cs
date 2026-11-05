using System;
using System.Collections.Generic;
namespace Snowglow {
 // Depth/credit are metres; transfers and totals are cubic metres.
 [Serializable] public sealed class SnowVolumeGrid {
  public const int Size=512;public const float Origin=-128;public const float Cell=.5f,Area=Cell*Cell;
  public float[] depth=new float[Size*Size],credit=new float[Size*Size];
  public bool[] blocked=new bool[Size*Size];
  public float initialVolume,disposed;public int revision;
  public int authoredLayoutVersion;
  // Cohesive, mound-like snow rather than liquid: roughly 32 degree resting sides.
  public const float ReposeSlope=.625f,Cohesion=.03f,SlumpSpeed=1.2f;
  const int SpreadRadius=6;
  static readonly int[] NeighborX={-1,0,1,-1,1,-1,0,1},NeighborZ={-1,-1,-1,0,0,1,1,1};
  static readonly float[] SpreadWeights=MakeSpreadWeights();
  readonly Queue<int> settling=new Queue<int>();readonly HashSet<int> queued=new HashSet<int>();
  int settleSequence;
  static float[] MakeSpreadWeights(){int side=SpreadRadius*2+1;var result=new float[side*side];for(int z=-SpreadRadius;z<=SpreadRadius;z++)for(int x=-SpreadRadius;x<=SpreadRadius;x++)result[(z+SpreadRadius)*side+x+SpreadRadius]=(float)Math.Sqrt(x*x+z*z)*Cell*ReposeSlope;return result;}
  void Enqueue(int i){if(i>=0&&i<depth.Length&&queued.Add(i))settling.Enqueue(i);}
  void Wake(int i){Enqueue(i);int x=i%Size,z=i/Size;for(int n=0;n<8;n++){int nx=x+NeighborX[n],nz=z+NeighborZ[n];if(nx>=0&&nz>=0&&nx<Size&&nz<Size)Enqueue(nz*Size+nx);}}
  public void WakePiles(){settling.Clear();queued.Clear();for(int i=0;i<depth.Length;i++)if(depth[i]>Cell*ReposeSlope+Cohesion)Enqueue(i);}
  public int Index(float x,float z){int ix=(int)Math.Floor((x-Origin)/Cell),iz=(int)Math.Floor((z-Origin)/Cell);return ix<0||iz<0||ix>=Size||iz>=Size?-1:iz*Size+ix;}
  public float X(int i){return Origin+(i%Size+.5f)*Cell;}public float Z(int i){return Origin+(i/Size+.5f)*Cell;}
  public static SnowVolumeGrid Upgrade(SnowVolumeGrid source){
   if(source==null||source.depth==null||source.credit==null||source.depth.Length!=source.credit.Length)return null;
   if(source.depth.Length==Size*Size)return source;
   if(source.depth.Length!=256*256)return null;
   var result=new SnowVolumeGrid{initialVolume=source.initialVolume,disposed=source.disposed,authoredLayoutVersion=source.authoredLayoutVersion};
   for(int z=0;z<Size;z++)for(int x=0;x<Size;x++){
    int old=(z/2)*256+x/2,i=z*Size+x;result.depth[i]=source.depth[old];result.credit[i]=source.credit[old];
    result.blocked[i]=source.blocked!=null&&source.blocked.Length==256*256&&source.blocked[old];
   }return result;
  }
  public float Take(int i,float amount,out float newlyCredited) {
   newlyCredited=0;if(i<0||i>=depth.Length||amount<=0)return 0;
   float taken=Math.Min(amount,depth[i]*Area);depth[i]=Math.Max(0,depth[i]-taken/Area);newlyCredited=Math.Min(credit[i]*Area,taken);credit[i]=Math.Max(0,credit[i]-newlyCredited/Area);if(taken>0)Wake(i);revision++;return taken;
  }
  public bool CanDeposit(float x,float z){return x>Origin+4&&x<Origin+Size*Cell-4&&z>Origin+4&&z<Origin+Size*Cell-4;}
  public float Deposit(float x,float z,float volume,Action<int> changed) {
   if(volume<=0||!CanDeposit(x,z))return 0;float left=volume;
   // Prefer lower supported locations near the discharge. The distance bias makes
   // a mound that grows wider with its height instead of repeatedly stacking rods.
   int center=Index(x,z),cx=center%Size,cz=center/Size;
   while(left>.000001f){
    int best=-1;float score=float.MaxValue;
    for(int dz=-SpreadRadius;dz<=SpreadRadius;dz++)for(int dx=-SpreadRadius;dx<=SpreadRadius;dx++){
     int ix=cx+dx,iz=cz+dz;if(ix<0||iz<0||ix>=Size||iz>=Size)continue;
     int i=iz*Size+ix;if((blocked!=null&&blocked[i])||depth[i]>=2.5f-.000001f)continue;
     float candidate=depth[i]+SpreadWeights[(dz+SpreadRadius)*(SpreadRadius*2+1)+dx+SpreadRadius];
     if(candidate<score){score=candidate;best=i;}
    }
    if(best<0)break;
    float add=Math.Min(left,Math.Min((2.5f-depth[best])*Area,.08f*Area));
    if(add<=0)break;depth[best]+=add/Area;left-=add;Wake(best);changed?.Invoke(best);
   }revision++;return volume-left;
  }
  // Process a bounded queue of unstable cells. Volume moves downhill gradually;
  // passive movement creates no work credit and cannot cross static obstacles.
  public int Settle(float dt,int cellBudget,Action<int> changed){
   if(dt<=0||cellBudget<=0)return 0;
   int work=Math.Min(cellBudget,settling.Count),moved=0;
   for(int step=0;step<work;step++){
    int i=settling.Dequeue();queued.Remove(i);if(depth[i]<=0||(blocked!=null&&blocked[i]))continue;
    int x=i%Size,z=i/Size,best=-1;float excess=0;int first=settleSequence++%8;
    for(int k=0;k<8;k++){
     int n=(first+k)%8,nx=x+NeighborX[n],nz=z+NeighborZ[n];if(nx<0||nz<0||nx>=Size||nz>=Size)continue;
     int j=nz*Size+nx;if(blocked!=null&&blocked[j])continue;
     float distance=Cell*(NeighborX[n]!=0&&NeighborZ[n]!=0?1.41421356f:1);
     float unstable=depth[i]-depth[j]-distance*ReposeSlope-Cohesion;
     if(unstable>excess){excess=unstable;best=j;}
    }
    if(best<0||excess<.001f)continue;
    float volume=Math.Min(excess*.5f,SlumpSpeed*Math.Min(dt,.05f))*Area;
    float taken=Take(i,volume,out float unused);depth[best]+=taken/Area;Wake(best);
    changed?.Invoke(i);changed?.Invoke(best);moved++;
   }return moved;
  }
  public float Total(){double sum=0;foreach(float d in depth)sum+=d;return (float)(sum*Area);}
 }
}
