using UnityEngine;
namespace Snowglow {
 // All lights and lamp surfaces are persistent Editor-authored bindings.
 public sealed class VehicleOperatingLights:MonoBehaviour {
  public MachineController machine;
  public int authoringVersion;
  public Light[] beams=new Light[0];
  public Renderer[] lamps=new Renderer[0];
  public Renderer rearLamp,beaconLamp,beaconHalo;
  public Renderer[] lampHalos=new Renderer[0];
  public bool lightsOn=true;
  public Transform beaconRotor;
  public float beaconDegreesPerSecond=180;
  float phase;
  MaterialPropertyBlock properties;
  public static readonly Color BeaconLensColor=new Color(1,.78f,.08f,1);
  public static readonly Color BeaconGlowColor=new Color(1,.75f,.12f,1);
  static readonly int BaseColor=Shader.PropertyToID("_BaseColor"),Phase=Shader.PropertyToID("_Phase"),Pulse=Shader.PropertyToID("_Pulse");
  void LateUpdate(){var session=PrototypeSession.Instance;bool operating=session&&session.IsRunning&&Time.timeScale>0;Apply(operating,session&&session.Input&&session.Input.Reverse);if(operating)Advance(Time.deltaTime);}
  public void Apply(bool operating){
   Apply(operating,false);
  }
  public void Apply(bool operating,bool reverse){
   bool on=operating&&machine&&machine.ActiveMachine&&machine.Fuel>0&&machine.Condition>0;
   foreach(var beam in beams)if(beam)beam.enabled=on&&lightsOn;
   foreach(var lamp in lamps)if(lamp)lamp.enabled=on&&(lamp==beaconLamp||lightsOn);
   foreach(var halo in lampHalos)if(halo)halo.enabled=on&&lightsOn;
   if(beaconHalo)beaconHalo.enabled=on;
   if(properties==null)properties=new MaterialPropertyBlock();
   if(beaconLamp){beaconLamp.GetPropertyBlock(properties);properties.SetColor(BaseColor,BeaconLensColor);beaconLamp.SetPropertyBlock(properties);}
   if(beaconHalo){beaconHalo.GetPropertyBlock(properties);properties.SetColor(BaseColor,BeaconGlowColor);beaconHalo.SetPropertyBlock(properties);}
   if(rearLamp){rearLamp.GetPropertyBlock(properties);properties.SetColor(BaseColor,new Color(2,2,2));rearLamp.SetPropertyBlock(properties);}
  }
  public void Advance(float seconds){
   if(!beaconLamp||!beaconLamp.enabled)return;
   phase=Mathf.Repeat(phase+Mathf.Max(0,seconds)*beaconDegreesPerSecond,360);
   if(beaconRotor)beaconRotor.localRotation=Quaternion.Euler(0,phase,0);
   if(properties==null)properties=new MaterialPropertyBlock();
   beaconLamp.GetPropertyBlock(properties);properties.SetFloat(Phase,phase*Mathf.Deg2Rad);beaconLamp.SetPropertyBlock(properties);
   if(beaconHalo){float flash=Mathf.Cos(phase*Mathf.Deg2Rad)>0?1:0;beaconHalo.enabled=flash>0;beaconHalo.GetPropertyBlock(properties);properties.SetFloat(Pulse,flash);beaconHalo.SetPropertyBlock(properties);}
  }
  public void Toggle(){lightsOn=!lightsOn;var session=PrototypeSession.Instance;Apply(session&&session.IsRunning&&Time.timeScale>0);}
  public void SwitchOff(){foreach(var beam in beams)if(beam)beam.enabled=false;foreach(var lamp in lamps)if(lamp)lamp.enabled=false;foreach(var halo in lampHalos)if(halo)halo.enabled=false;if(beaconHalo)beaconHalo.enabled=false;}
  void OnDisable(){SwitchOff();}
 }
}
