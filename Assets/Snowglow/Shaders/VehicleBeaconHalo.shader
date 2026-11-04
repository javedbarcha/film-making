Shader "Snowglow/Vehicle Beacon Halo" {
 Properties {_BaseColor("Glow",Color)=(1,0.015,0.005,1) _Pulse("Pulse",Range(0,1))=1}
 SubShader {
  Tags {"RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent"}
  Pass {
   Tags {"LightMode"="SRPDefaultUnlit"}
   Blend SrcAlpha One
   ZWrite Off
   Cull Off
   HLSLPROGRAM
   #pragma vertex Vert
   #pragma fragment Frag
   #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
   CBUFFER_START(UnityPerMaterial)
   float4 _BaseColor;float _Pulse;
   CBUFFER_END
   struct A {float4 positionOS:POSITION;float2 uv:TEXCOORD0;};
   struct V {float4 positionCS:SV_POSITION;float2 uv:TEXCOORD0;};
   V Vert(A i){V o;float3 center=TransformObjectToWorld(float3(0,0,0));float scale=length(GetObjectToWorldMatrix()._m00_m10_m20);float3 offset=mul((float3x3)UNITY_MATRIX_I_V,float3(i.positionOS.xy*scale,0));o.positionCS=TransformWorldToHClip(center+offset);o.uv=i.uv;return o;}
   half4 Frag(V i):SV_Target {float radius=length(i.uv*2-1);float alpha=pow(saturate(1-radius),2)*(.18+.62*_Pulse);return half4(_BaseColor.rgb,alpha);}
   ENDHLSL
  }
 }
}
