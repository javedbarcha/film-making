Shader "Snowglow/Vehicle Beacon" {
 Properties {_BaseColor("Permanent amber lens",Color)=(1,0.78,0.08,1) _Center("Local lens center",Vector)=(0,0,0,0) _Phase("Rotation phase",Float)=0}
 SubShader {
  Tags {"RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry"}
  Pass {
   Tags {"LightMode"="SRPDefaultUnlit"}
   HLSLPROGRAM
   #pragma vertex Vert
   #pragma fragment Frag
   #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
   CBUFFER_START(UnityPerMaterial)
   float4 _BaseColor; float4 _Center; float _Phase;
   CBUFFER_END
   struct A {float4 positionOS:POSITION;float3 normalOS:NORMAL;};
   struct V {float4 positionCS:SV_POSITION;float3 local:TEXCOORD0;};
   V Vert(A i){V o;o.positionCS=TransformObjectToHClip(i.positionOS.xyz);o.local=i.normalOS;return o;}
   half4 Frag(V i):SV_Target {
    // The physical lens keeps its color. Only the separate glow flashes.
    return half4(_BaseColor.rgb*(.85+.15*saturate(i.local.y)),1);
   }
   ENDHLSL
  }
 }
}
