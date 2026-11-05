using UnityEngine;
using System;
using UnityEngine.SceneManagement;
namespace Snowglow {
 public sealed class PrototypeSession : MonoBehaviour {
  public static PrototypeSession Instance {get;private set;}
  public MachineController[] Fleet;public SnowField Snow;public PrototypeInput Input;public CameraRig CameraRig;public PrototypeHUD HUD;public SynthAudio Audio;
  public ProfileSave Profile;public MachineController Active=>Fleet[Mathf.Clamp(Profile.active,0,Fleet.Length-1)];
  public string Panel="dashboard",Notice="Welcome to Pinefall. Start at the depot.";
  public bool IsRunning=>Panel=="";public int Level=>1+Profile.xp/300;
  public int Multiplier=>Mathf.Clamp(1+(int)(Profile.chain/4),1,8);
  public float Remaining=>Profile.remaining;
  float nextSave;bool ready;string pendingDummy="";
  void Awake(){
   Instance=this;Time.timeScale=0;Application.targetFrameRate=30;QualitySettings.vSyncCount=0;
   Profile=SaveStore.Load();Profile.active=Mathf.Clamp(Profile.active,0,Fleet.Length-1);
   for(int i=0;i<Fleet.Length;i++){
    var m=Fleet[i];m.Initialize();m.Owned=i==0;m.Fuel=m.definition.fuelCapacity;
    if(Profile.machines!=null)foreach(var save in Profile.machines)if(save.id==m.definition.id){
     m.Owned=save.owned;m.Fuel=Mathf.Clamp(save.fuel,0,m.definition.fuelCapacity);m.Condition=Mathf.Clamp(save.condition,0,100);m.Tyres=Mathf.Clamp(save.tyres,0,3);m.Engine=Mathf.Clamp(save.engine,0,3);
     m.Tool.Load=Mathf.Clamp(save.load,0,m.equipment.capacity);m.Tool.Lift=save.lift;m.Tool.Tilt=save.tilt;m.Tool.Angle=save.angle;m.Tool.Auto=true;m.Tool.Working=save.working;
     if(Profile.inJob&&save.position.y>0&&Mathf.Abs(save.position.x)<480&&Mathf.Abs(save.position.z)<480)m.transform.SetPositionAndRotation(save.position,save.rotation);
    }
   }
   // The starter is always free, including profiles that stored owned=false.
   Fleet[0].Owned=true;
   if(!Active.Owned)Profile.active=0;
   Profile.snow=SnowVolumeGrid.Upgrade(Profile.snow);
   Snow.Initialize(Profile.job,Profile.inJob?Profile.snow:null);
   foreach(var m in Fleet)m.SetActiveMachine(m==Active);
   Audio.SetMuted(Profile.muted);ready=true;ApplyQuality();HUD.RefreshMenu();
  }
  public void Open(string panel){Panel=panel;if(panel!=""){Audio.Reverse(false);Audio.StopMachinery();}Input.Clear();Time.timeScale=panel==""?1:0;HUD.RefreshMenu();if(panel!="")Save();}
  public void TogglePause(){if(Panel=="")Open("pause");else if(Panel=="pause")Open("");}
  public void StartJob(int kind,bool free=false){
   if(Profile.inJob){Notice="Finish or abandon the active job before starting another.";Open("dashboard");return;}
   Profile.job=kind;Profile.freeWork=free;Profile.inJob=true;Profile.remaining=kind==0?720:900;Profile.earned=Profile.chain=Profile.chainTime=0;
   Profile.transaction=Guid.NewGuid().ToString("N");Snow.ResetToSeed();foreach(var vehicle in Fleet){vehicle.Tool.Load=0;vehicle.RecoverToDepot();}RecreditTarget();Open("");Save();
  }
  void RecreditTarget(){
   // Every accepted contract uses current snow. Abandon/restart reloads a fresh worksite.
   Snow.SetContract(Profile.job);for(int i=0;i<Snow.Grid.credit.Length;i++)Snow.Grid.credit[i]=Snow.IsTarget(Snow.Grid.X(i),Snow.Grid.Z(i))?Snow.Grid.depth[i]:0;
  }
  public void Continue(){if(!Profile.inJob){Notice="Choose a contract or Free Work first.";Open("contracts");return;}Open("");}
  public void RecordWork(float volume){
   if(volume<=0||!Profile.inJob||!IsRunning)return;
   Profile.chain+=volume;Profile.chainTime=4;Profile.earned+=volume*(1+Mathf.Min(.75f,(Multiplier-1)*.1f))*3;
   Audio.Clear(Multiplier);
  }
  void Update(){
   if(!ready)return;
   if(IsRunning&&Profile.inJob){if(!Profile.freeWork)Profile.remaining-=Time.deltaTime;Profile.chainTime-=Time.deltaTime;if(Profile.chainTime<=0)Profile.chain=0;
    if(!Profile.freeWork&&Snow.Progress>=.9f)Finish();}
   if(Time.unscaledTime>nextSave){nextSave=Time.unscaledTime+30;Save();}
   Audio.UpdateMachinery(Active,Input.Throttle,Input.Brake,IsRunning,Time.unscaledDeltaTime);
   float forwardSpeed=Active.Body?Vector3.Dot(Active.Body.linearVelocity,Active.transform.forward):0;
   Audio.Reverse(Audio.MachineryOperating&&SynthAudio.ShouldPlayReverseAlarm(Input.Reverse,forwardSpeed));
  }
  public void Finish(){
   if(!Profile.inJob)return;if(!Profile.freeWork&&Snow.Progress<.9f){Notice="Clear at least 90% of the marked contract area.";return;}
   if(Profile.lastReward==Profile.transaction)return;
   int basePay=Profile.freeWork?0:350;float late=Profile.remaining<0?.7f:1;
   int reward=Mathf.RoundToInt((basePay+Profile.earned)*late);Profile.coins+=reward;Profile.xp+=Profile.freeWork?0:180;
   Profile.lastReward=Profile.transaction;Profile.inJob=false;Notice="Shift complete: +"+reward+" coins. "+(late<1?"Late completion: reduced pay.":"Pinefall thanks you.");
   Audio.Chime();Open("results");Save();
  }
  public void Abandon(){Profile.inJob=false;Profile.snow=null;Save();ReloadDistrict();}
  public void FreshDistrict(){if(Profile.inJob){Notice="Finish or abandon the active job first.";return;}Profile.snow=null;foreach(var m in Fleet){m.Tool.Load=0;m.RecoverToDepot();}Save();ReloadDistrict();}
  void ReloadDistrict(){Time.timeScale=1;SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);}
  public void Switch(int index){
   if(!Active.AtDepot){Notice="Return to the depot to switch machinery.";return;}
   if(!Fleet[index].Owned){Notice="Purchase this machine in the machinery shop.";return;}
   if(Active.Speed>1){Notice="Stop before switching machinery.";return;}
   Active.SetActiveMachine(false);Profile.active=index;Active.SetActiveMachine(true);Active.Tool.ResetSweep();Input.Clear();Notice=Active.definition.displayName+" selected.";Save();HUD.RefreshMenu();
  }
  public void BuyMachine(int i){if(Fleet[i].Owned){Notice="Already owned.";return;}if(!Pay(Fleet[i].definition.price))return;Fleet[i].Owned=true;Notice="Machine purchased and waiting at the depot.";Save();HUD.RefreshMenu();}
  bool Pay(int cost){if(Profile.coins<cost){Notice="Not enough coins. Use the dummy store for prototype testing.";return false;}Profile.coins-=cost;return true;}
  public void Upgrade(bool tyres){
   if(!Active.AtDepot){Notice="Return to the depot to fit parts.";return;}int level=tyres?Active.Tyres:Active.Engine;
   if(level>=3){Notice="Upgrade already at maximum.";return;}if(!Pay(200+level*200))return;
   if(tyres)Active.Tyres++;else Active.Engine++;Notice="Compatible part fitted to "+Active.definition.displayName;Audio.Chime();Save();HUD.RefreshMenu();
  }
  public void Service(bool fuel){
   if(!Active.AtDepot){Notice="Return to the depot for service.";return;}
   int cost=Mathf.CeilToInt(fuel?(Active.definition.fuelCapacity-Active.Fuel)*2:(100-Active.Condition)*3);
   if(!Pay(cost))return;if(fuel)Active.Fuel=Active.definition.fuelCapacity;else Active.Condition=100;Notice="Service complete. Cost: "+cost;Save();HUD.RefreshMenu();
  }
  public void Recover(){Active.RecoverToDepot();Notice="Recovered to the depot. Snow and load are retained.";Input.Clear();Save();}
  public void Dummy(string outcome){
   if(outcome=="new"){pendingDummy=Guid.NewGuid().ToString("N");Notice="SIMULATED purchase ready. No money is charged.";return;}
   if(outcome=="cancel"||outcome=="fail"){pendingDummy="";Notice="SIMULATED purchase "+outcome+". No currency granted.";return;}
   if(pendingDummy==""){Notice="Create a new simulated purchase first.";return;}
   if(Profile.lastDummy==pendingDummy){Notice="Duplicate transaction ignored.";return;}
   Profile.lastDummy=pendingDummy;Profile.coins+=2000;Notice="SIMULATED +2,000 coins. No real payment.";Save();
  }
  public void ApplyQuality(){
   var asset=UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline as UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset;
   if(asset!=null){asset.renderScale=Profile.quality==0?.7f:.9f;asset.shadowDistance=Profile.quality==0?30:65;}
   foreach(var m in Fleet)m.Assistance=Profile.assists;
  }
  public void Save(){
   if(!ready)return;
   Profile.machines=new MachineSave[Fleet.Length];
   for(int i=0;i<Fleet.Length;i++){var m=Fleet[i];Profile.machines[i]=new MachineSave{id=m.definition.id,owned=m.Owned,fuel=m.Fuel,condition=m.Condition,tyres=m.Tyres,engine=m.Engine,position=m.transform.position,rotation=m.transform.rotation,load=m.Tool.Load,lift=m.Tool.Lift,tilt=m.Tool.Tilt,angle=m.Tool.Angle,auto=m.Tool.Auto,working=m.Tool.Working};}
   Profile.snow=Profile.inJob?Snow.Grid:null;SaveStore.Write(Profile);
  }
  void OnApplicationPause(bool pause){if(pause&&ready){Open("pause");Save();}}
  void OnApplicationFocus(bool focus){if(!focus&&ready&&IsRunning)Open("pause");}
  void OnApplicationQuit(){Save();}
  void OnDestroy(){if(Instance==this)Instance=null;Time.timeScale=1;}
 }
}
