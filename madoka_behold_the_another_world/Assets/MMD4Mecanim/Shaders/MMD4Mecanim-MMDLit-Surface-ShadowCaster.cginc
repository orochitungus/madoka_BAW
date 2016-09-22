// - Don't upload model data, motion data, this code in github or public space without permission.
// - Don't modify this code without permission.
#include "HLSLSupport.cginc"
#include "UnityShaderVariables.cginc"
#define UNITY_PASS_SHADOWCASTER
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "MMD4Mecanim-MMDLit-Lighting.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

half4 _Color;
sampler2D _MainTex;

struct v2f_surf {
  V2F_SHADOW_CASTER;
  float2 pack0 : TEXCOORD1;
};
v2f_surf vert_surf (appdata_full v) {
  v2f_surf o;
  o.pack0.xy = v.texcoord;
  TRANSFER_SHADOW_CASTER(o)
  return o;
}
fixed4 frag_surf (v2f_surf IN) : MMDLIT_SV_TARGET{
  half4 albedo = (half4)tex2D(_MainTex, IN.pack0.xy);
  albedo.a *= _Color.a; // for Transparency
  MMDLIT_CLIP(albedo.a)

  SHADOW_CASTER_FRAGMENT(IN)
}

fixed4 frag_fast (v2f_surf IN) : MMDLIT_SV_TARGET{
  MMDLIT_CLIP_FAST(1.0)
  SHADOW_CASTER_FRAGMENT(IN)
}
