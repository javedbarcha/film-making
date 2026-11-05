using UnityEngine;

namespace Snowglow {
 public sealed class ExhaustSmokeEffect : MonoBehaviour {
  public MachineController machine;
  public ParticleSystem particles;

  const float IdleRate=1f,ThrottleRate=5.4f,LoadRate=3.2f,MaximumRate=9.6f;

  public static float CalculateEmissionRate(bool active,bool running,bool selected,float fuel,float condition,float throttle,float load){
   if(!active||!running||!selected||fuel<=0||condition<=0)return 0;
   return Mathf.Min(MaximumRate,IdleRate+Mathf.Clamp01(throttle)*ThrottleRate+Mathf.Clamp01(load)*LoadRate);
  }

  void Awake(){
   if(!machine)machine=GetComponentInParent<MachineController>();
   if(particles)particles.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
  }

  void Update(){
   if(!particles)return;
   if(!machine)machine=GetComponentInParent<MachineController>();
   var game=PrototypeSession.Instance;
   bool running=game&&game.IsRunning&&game.Input;
   bool selected=running&&game.Active==machine;
   float throttle=selected?game.Input.Throttle:0;
   float load=machine&&machine.definition?Mathf.Clamp01(machine.LastSnowForce/Mathf.Max(1,machine.definition.mass*2)):0;
   var emission=particles.emission;
   emission.rateOverTime=CalculateEmissionRate(machine&&machine.ActiveMachine,running,selected,machine?machine.Fuel:0,machine?machine.Condition:0,throttle,load);
   if(emission.rateOverTime.constant>0){
    if(!particles.isPlaying)particles.Play();
   }else if(particles.isPlaying||particles.particleCount>0)particles.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
  }

  void OnDisable(){
   if(particles)particles.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
  }
 }
}
