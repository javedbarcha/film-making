using UnityEngine;
namespace Snowglow {
 [System.Serializable] public sealed class MachineryAudioProfile {public AudioClip idle,work,drive;}
 public struct MachineryMix {
  public float enginePitch,engineVolume,loadVolume,transmissionPitch,transmissionVolume;
 }
 public sealed class SynthAudio : MonoBehaviour {
  public AudioSource effects,motor,ambience,reverseAlarm,motorLoad,transmission,brakeAir;
  public const float ReverseAlarmSpeedThreshold=.3f;
  public MachineryAudioProfile tractorAudio=new MachineryAudioProfile(),loaderAudio=new MachineryAudioProfile();
  public AudioClip brakingClip;public bool BrakeCueTriggered {get;private set;}
  float brakeCooldown,previousBrake;
  public AudioClip[] notes;public AudioClip chime;
  public bool MachineryOperating {get;private set;}
  public float EngineRev {get;private set;}public float Workload {get;private set;}
  float next,roadSpeed,driveDemand,wheelSpeed;MachineController lastMachine;
  public void SetMuted(bool muted){if(effects)effects.mute=muted;if(motor)motor.mute=muted;if(ambience)ambience.mute=muted;if(reverseAlarm)reverseAlarm.mute=muted;if(motorLoad)motorLoad.mute=muted;if(transmission)transmission.mute=muted;if(brakeAir)brakeAir.mute=muted;}
  public void Reverse(bool active){if(!reverseAlarm)return;if(active){if(!reverseAlarm.isPlaying)reverseAlarm.Play();}else if(reverseAlarm.isPlaying)reverseAlarm.Stop();}
  public static bool ShouldPlayReverseAlarm(bool reverseEngaged,float forwardSpeed)=>reverseEngaged&&forwardSpeed< -ReverseAlarmSpeedThreshold;
  static void Stop(AudioSource source){if(!source)return;source.volume=0;if(source.isPlaying)source.Stop();}
  public void StopMachinery(){Stop(motor);Stop(motorLoad);Stop(transmission);Stop(brakeAir);BrakeCueTriggered=false;brakeCooldown=previousBrake=0;MachineryOperating=false;EngineRev=Workload=roadSpeed=driveDemand=wheelSpeed=0;lastMachine=null;}
  void OnDisable(){StopMachinery();Reverse(false);}
  // Presentation curves only: never feed acoustic load back into driving physics.
  public static MachineryMix CalculateMix(float speed,float throttle,float load,float wheels,bool loader){
   speed=Mathf.Clamp01(speed);throttle=Mathf.Clamp01(throttle);load=Mathf.Clamp01(load);wheels=Mathf.Clamp01(wheels);
   float rev=Mathf.Clamp01(speed*.52f+throttle*.34f+wheels*.16f-load*.10f);
   float tone=loader?.96f:1;
   return new MachineryMix{enginePitch=(.9f+rev*.4f)*tone,engineVolume=(.115f+throttle*.055f+speed*.035f+load*.055f)*(loader?1.08f:1),loadVolume=load*(.07f+throttle*.10f),transmissionPitch=(.85f+wheels*.45f+load*.02f)*(loader?.98f:1),transmissionVolume=Mathf.Clamp01(wheels*3)*(.012f+wheels*.038f+load*.025f)};
  }
  static float Smooth(float current,float target,float dt)=>Mathf.Lerp(current,target,1-Mathf.Exp(-dt/(target>current?.18f:.5f)));
  static void Apply(AudioSource source,float volume,float pitch){if(!source)return;source.volume=Mathf.Clamp01(volume);source.pitch=Mathf.Clamp(pitch,.5f,2);if(volume>.0005f){if(!source.isPlaying&&source.clip)source.Play();}else if(source.isPlaying)source.Stop();}
  void SelectProfile(bool loader){var profile=loader?loaderAudio:tractorAudio;if(profile==null)return;if(motor&&profile.idle)motor.clip=profile.idle;if(motorLoad&&profile.work)motorLoad.clip=profile.work;if(transmission&&profile.drive)transmission.clip=profile.drive;}
  public static bool ShouldBrakeCue(float speed,float brake,float previous,float cooldown)=>speed>.7f&&brake>.25f&&previous<=.25f&&cooldown<=0;
  public void UpdateMachinery(MachineController machine,float throttle,float brake,bool running,float dt){
   if(!running||!machine||!machine.ActiveMachine||!machine.definition||machine.Fuel<=0||machine.Condition<=0){StopMachinery();return;}
   if(lastMachine!=machine){StopMachinery();lastMachine=machine;SelectProfile(machine.definition.loader);}
   BrakeCueTriggered=false;
   MachineryOperating=true;dt=Mathf.Clamp(dt,0,.1f);float maxSpeed=Mathf.Max(1,machine.definition.maxSpeed);
   float speed=machine.Body?machine.Body.linearVelocity.magnitude:machine.Speed;
   brakeCooldown=Mathf.Max(0,brakeCooldown-dt);
   if(brakeAir&&brakingClip&&ShouldBrakeCue(speed,brake,previousBrake,brakeCooldown)){brakeAir.volume=Mathf.Lerp(.12f,.22f,Mathf.Clamp01(speed/maxSpeed));brakeAir.pitch=machine.definition.loader?.95f:1;brakeAir.Play();brakeCooldown=1;BrakeCueTriggered=true;}
   previousBrake=brake;float demand=Mathf.Clamp01(throttle)*(1-Mathf.Clamp01(brake));
   float wheelMotion=0;int count=0;if(machine.wheels!=null)foreach(var wheel in machine.wheels)if(wheel){wheelMotion+=Mathf.Abs(wheel.rpm)*wheel.radius*(2*Mathf.PI/60);count++;}if(count>0)wheelMotion/=count;
   float snow=Mathf.Clamp01(machine.LastSnowForce/Mathf.Max(1,machine.definition.mass*2));float slip=Mathf.Clamp01(machine.Slip/Mathf.Max(1,count));float stalled=demand*(1-Mathf.Clamp01(speed/(maxSpeed*.2f)))*.75f;
   float carried=machine.Tool&&machine.equipment?Mathf.Clamp01(machine.Tool.Load/Mathf.Max(.01f,machine.equipment.capacity)):0;
   float load=Mathf.Clamp01(Mathf.Max(snow,Mathf.Max(slip*.65f*demand,stalled))+carried*demand*.2f);
   // Old work resistance/slip must not keep a coasting or stopped machine growling.
   load*=Mathf.Clamp01(demand*3+speed/maxSpeed);
   roadSpeed=Smooth(roadSpeed,Mathf.Clamp01(speed/maxSpeed),dt);driveDemand=Smooth(driveDemand,demand,dt);wheelSpeed=Smooth(wheelSpeed,Mathf.Clamp01(Mathf.Max(wheelMotion,speed)/maxSpeed),dt);Workload=Smooth(Workload,load,dt);
   var mix=CalculateMix(roadSpeed,driveDemand,Workload,wheelSpeed,machine.definition.loader);EngineRev=Mathf.Clamp01(roadSpeed*.52f+driveDemand*.34f+wheelSpeed*.16f-Workload*.10f);
   Apply(motor,mix.engineVolume,mix.enginePitch);Apply(motorLoad,mix.loadVolume,mix.enginePitch*.98f);Apply(transmission,mix.transmissionVolume,mix.transmissionPitch);
  }
  public void Clear(int multiplier){if(Time.time<next)return;next=Time.time+.12f;effects.PlayOneShot(notes[Mathf.Clamp(multiplier-1,0,notes.Length-1)],.3f);}
  public void Chime(){effects.PlayOneShot(chime,.5f);}
 }
}
