// - Don't upload model data, motion data, this code in github or public space without permission.
// - Don't modify this code without permission.
#include "HLSLSupport.cginc"
#include "UnityShaderVariables.cginc"
#define UNITY_PASS_FORWARDADD
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "MMD4Mecanim-MMDLit-AutoLight.cginc"

#define INTERNAL_DATA
#define WorldReflectionVector(data,normal) data.worldRefl
#define WorldNormalVector(data,normal) normal

#include "MMD4Mecanim-MMDLit-Lighting.cginc"
#include "MMD4Mecanim-MMDLit-SurfaceEdge-Lighting.cginc"
struct v2f_surf {
	float4 pos : SV_POSITION;
	LIGHTING_COORDS(0,1)
};

v2f_surf vert_surf (appdata_full v)
{
	v2f_surf o;
	float3 worldN = mul((float3x3)_UNITY_OBJECT_TO_WORLD, SCALED_NORMAL);
	v.vertex = MMDLit_GetEdgeVertex(v.vertex, v.normal);
	o.pos = MMDLit_TransformEdgeVertex(v.vertex);
	TRANSFER_VERTEX_TO_FRAGMENT(o);
	return o;
}

fixed4 frag_surf (v2f_surf IN) : MMDLIT_SV_TARGET
{
	half alpha;
	half3 albedo = MMDLit_GetAlbedo(alpha);

	MMDLIT_CLIP(alpha)
	
	half3 c = MMDLit_Lighting(albedo, LIGHT_ATTENUATION(IN), half3(0.0, 0.0, 0.0));
	c = min(c, 1.0);
	c *= alpha;
	return fixed4(c, 0.0);
}
