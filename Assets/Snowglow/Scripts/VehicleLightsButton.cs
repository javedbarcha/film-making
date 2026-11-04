using UnityEngine;
using UnityEngine.UI;
namespace Snowglow {
 public sealed class VehicleLightsButton:MonoBehaviour {
  public Text label;
  public Image background;
  public Sprite onIcon,offIcon;
  public void Click(){var game=PrototypeSession.Instance;if(game&&game.IsRunning){var lights=game.Active.GetComponent<VehicleOperatingLights>();if(lights)lights.Toggle();Refresh(lights);}}
  public void Refresh(VehicleOperatingLights lights){bool on=!lights||lights.lightsOn;if(label){label.text=on?"LIGHTS\nON":"LIGHTS\nOFF";label.gameObject.SetActive(!onIcon||!offIcon);}if(background){if(onIcon&&offIcon){background.sprite=on?onIcon:offIcon;background.color=Color.white;}else background.color=on?new Color(.32f,.3f,.13f,.95f):new Color(.12f,.17f,.22f,.95f);}}
  void Update(){var game=PrototypeSession.Instance;if(game&&game.IsRunning)Refresh(game.Active.GetComponent<VehicleOperatingLights>());}
 }
}
