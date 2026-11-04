using UnityEngine;
namespace Snowglow {
 // Updates the already-authored material when Unity rebuilds the shared dynamic font atlas.
 [ExecuteAlways,DisallowMultipleComponent,RequireComponent(typeof(TextMesh))]
 public sealed class WorldSignLabel : MonoBehaviour {
  public TextMesh caption;public Material material;
  void OnEnable(){Font.textureRebuilt+=AtlasChanged;Refresh();}
  void OnDisable(){Font.textureRebuilt-=AtlasChanged;}
  void AtlasChanged(Font font){if(caption&&caption.font==font)Refresh();}
  public void Refresh(){if(!caption)caption=GetComponent<TextMesh>();if(!caption||!caption.font||!material)return;var renderer=GetComponent<MeshRenderer>();if(renderer.sharedMaterial!=material)renderer.sharedMaterial=material;var atlas=caption.font.material.mainTexture;if(material.mainTexture!=atlas)material.mainTexture=atlas;}
 }
}
