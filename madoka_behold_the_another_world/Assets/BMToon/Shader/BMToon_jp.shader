// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "BMToon/BMT_Basic_jp" {
	Properties{
		[Space]
		[Toggle(USE_EDGE)] _UseEdge("エッジ使用", Float) = 1
		[Space]

		_UnityAmbient("Unityの環境色有効化",Range(0,2)) = 0.0
		[Space]

		[Header(MainColorParameter)]
		_MainTex("メインテクスチャ", 2D) = "white" {}
		_MainColor("メインテクスチャ補正色", Color) = (1,1,1,1)
		[Space]

		[Header(ShadowColorParameter)]
		_ShadowTex("影テクスチャ", 2D) = "white" {}
		_ShadowColor("影テクスチャ補正色", Color) = (0.5,0.5,0.5,1)
		_ShadowPower("自己乗算影",float) = 1.0
		[Space]

		[Header(RampParameter)]
		_RampTex("ランプテクスチャ", 2D) = "white" {}
		[Space]

		[Header(ShadowRateParameter)]
		_ShadowRate("影になりやすさ",Range(-1,1)) = 0.0
		_ShadowRateTex("影になりやすさテクスチャ", 2D) = "gray" {}
		[Space]

		[Header(HighlightParameter)]
		_HighlightTex("ハイライト色テクスチャ", 2D) = "white" {}
		_HighlightColor("ハイライト色", Color) = (1,1,1,1)
		_HighlightRate("ハイライト出やすさ(Use_RampAlpha&ShadowRateTexGreen)",Range(-1,1)) = 0.0
		[Space]

		[Header(SpecularParameter)]
		_SpecularTex("スぺキュラ色テクスチャ", 2D) = "white" {}
		_SpecularColor("スぺキュラ補正色", Color) = (0,0,0,1)
		_SpecularPower("スぺキュラ鋭さ",Range(1,64)) = 1
		[Space]

		[Header(SphereMapParameter)]
		_SphereTex("スフィアテクスチャ", 2D) = "black" {}
		_SphereColor("スフィアテクスチャ補正色", Color) = (1,1,1,1)
		_SphereMode("スフィアモード（0:加算 1:乗算）",Range(0,1)) = 0
		[Space]

		[Header(EdgeParameter)]
		_EdgeColTex("エッジ色テクスチャ", 2D) = "white" {}
		_EdgeColor("エッジ補正色",Color) = (0,0,0,1)
		_EdgeSizeTex("エッジ大きさテクスチャ", 2D) = "white" {}
		_EdgeSize("エッジ大きさ",Range(0,10)) = 2.5
		[Space]

		[Header(NormalMapParameter)]
		_BumpMap("ノーマルマップテクスチャ", 2D) = "bump" {}
		_NormalScale("ノーマルマップ強度", float) = 1.0
		[Space]

		[Header(RimLightParameter)]
		_RimLightTex("リムライトテクスチャ", 2D) = "white" {}
		_RimLightColor("リムライト補正色", Color) = (0,0,0,1)
		_RimRampTex("リムライト用ランプテクスチャ", 2D) = "white" {}
		_RimLightPower("リムライト鋭さ", float) = 8.0
		_LightInfluence_Rim("メインライト影響度",Range(0,1)) = 0
		
		/*
		[Space]
		[Toggle(USE_EDGE)] _UseEdge("エッジ有効", Float) = 1
		[Space]

		_UnityAmbient("ユニティのアンビエント色有効化",Range(0,1)) = 0.0
		[Space]

		[Header(MainColorParameter)]
		_MainTex("メインテクスチャ", 2D) = "white" {}
		_MainColor("メイテンクスチャ補正色", Color) = (1,1,1,1)
		[Space]

		[Header(ShadowColorParameter)]
		_ShadowTex("影テクスチャ", 2D) = "white" {}
		_ShadowColor("影テクスチャ補正色", Color) = (0.5,0.5,0.5,1)
		_ShadowPower("自己乗算影",float) = 1.0
		[Space]

		[Header(ShadowRateParameter)]
		_RampTex("ランプテクスチャ", 2D) = "white" {}
		_ShadowRate("影になりやすさ",Range(-1,1)) = 0.0
		_ShadowRateTex("影になりやすさテクスチャ", 2D) = "gray" {}
		_HighlightRate("ハイライトになりやすさ(Use_RampAlpha&RateTexGreen)",Range(-1,1)) = 0.0
		[Space]

		[Header(SpecularParameter)]
		_SpecularTex("スぺキュラ色テクスチャ", 2D) = "white" {}
		_SpecularColor("スぺキュラ補正色", Color) = (0,0,0,1)
		_SpecularPower("スぺキュラ鋭さ",Range(1,64)) = 1
		[Space]

		[Header(SphereMapParameter)]
		_SphereTex("スフィアテクスチャ", 2D) = "black" {}
		_SphereColor("スフィアテクスチャ補正色", Color) = (1,1,1,1)
		_SphereMode("スフィアモード（0:加算 1:乗算）",Range(0,1)) = 0
		[Space]

		[Header(EdgeParameter)]
		_EdgeColTex("エッジ色テクスチャ", 2D) = "white" {}
		_EdgeColor("エッジ補正色",Color) = (0,0,0,1)
		_EdgeSizeTex("エッジ大きさテクスチャ", 2D) = "white" {}
		_EdgeSize("エッジ大きさ",Range(0,10)) = 2.5
		[Space]

		[Header(NormalMapParameter)]
		_BumpMap("ノーマルマップテクスチャ", 2D) = "bump" {}
		_NormalScale("ノーマルマップ強度", float) = 1.0
		[Space]

		[Header(RimLightParameter)]
		_RimLightTex("リムライトテクスチャ", 2D) = "white" {}
		_RimLightColor("リムライト補正色", Color) = (0,0,0,1)
		_RimRampTex("リムライト用ランプテクスチャ", 2D) = "white" {}
		_RimLightPower("リムライト鋭さ", float) = 8.0
		_LightInfluence_Rim("メインライト影響度",Range(0,1)) = 0
		*/

		[HideInInspector] _Mode("__mode", Float) = 0.0
		[HideInInspector] _SrcBlend("__src", Float) = 1.0
		[HideInInspector] _DstBlend("__dst", Float) = 0.0
		[HideInInspector] _ZWrite("__zw", Float) = 1.0
		[HideInInspector] _Cutoff("__cutoff", Float) = 0.5
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200
		Blend [_SrcBlend] [_DstBlend]
		ZWrite [_ZWrite]
		cull off
		//メインライト処理
		Pass{
		Name "BMT_BASIC_MAIN"
		Tags{
		"LightMode" = "ForwardBase"
	}
		CGPROGRAM

#pragma target 3.0
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fwdbase_fullshadows
#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
#include "UnityCG.cginc"
#include "AutoLight.cginc"

	uniform float _UnityAmbient;
	uniform float _ShadowRate;
	uniform sampler2D _ShadowRateTex;
	uniform float4 _ShadowRateTex_ST;
	uniform float4 _MainColor;
	uniform sampler2D _ShadowTex;
	uniform float4 _ShadowTex_ST;
	uniform float4 _ShadowColor;
	uniform sampler2D _MainTex;
	uniform float4 _MainTex_ST;
	uniform sampler2D _RampTex;
	uniform float4 _RampTex_ST;
	uniform float _ShadowPower;
	uniform fixed4 _LightColor0;

	uniform sampler2D _SpecularTex;
	uniform float4 _SpecularTex_ST;
	uniform float4 _SpecularColor;
	uniform float _SpecularPower;

	uniform sampler2D _SphereTex;
	uniform float4 _SphereTex_ST;
	uniform float4 _SphereColor;
	uniform float _SphereMode;

	uniform sampler2D _BumpMap;
	uniform float4 _BumpMap_ST;
	uniform float _NormalScale;

	uniform float4 _RimLightColor;
	uniform float _RimLightPower;
	uniform float _LightInfluence_Rim;
	uniform sampler2D _RimLightTex;
	uniform float4 _RimLightTex_ST;
	uniform sampler2D _RimRampTex;
	uniform float4 _RimRampTex_ST;

	uniform float _Cutoff;

	uniform sampler2D _HighlightTex;
	uniform float4 _HighlightTex_ST;
	uniform float _HighlightRate;
	uniform float4 _HighlightColor;

	//バーテックスシェーダからフラグメントシェーダに渡す構造体
	struct v2f {
		float4 pos : SV_POSITION;
		float2 tex : TEXCOORD0;
		float3 Eye : TEXCOORD1;
		float3 lightDir : TEXCOORD2;
		float2 sptex : TEXCOORD3;
		float3 vlight : TEXCOORD4;
		LIGHTING_COORDS(5, 6)
	};

	//バーテックスシェーダ
	v2f vert(appdata_full v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.tex = v.texcoord;

		TANGENT_SPACE_ROTATION;
		o.Eye = mul(rotation, ObjSpaceViewDir(v.vertex));
		o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex));

		float2 NormalWV = normalize(mul((fixed3x3)UNITY_MATRIX_V, mul(unity_ObjectToWorld, float4(v.normal.xyz, 0.0)).xyz));
		o.sptex.x = NormalWV.x * 0.5f + 0.5f;
		o.sptex.y = NormalWV.y * 0.5f + 0.5f;

		float3 worldN = mul((float3x3)unity_ObjectToWorld, SCALED_NORMAL);
		#ifdef LIGHTMAP_OFF
			float3 shlight = ShadeSH9(float4(worldN, 1.0));
			o.vlight = shlight*2;
		#endif

		TRANSFER_VERTEX_TO_FRAGMENT(o);

		return o;
	}
	//フラグメントシェーダ
	float4 frag(v2f i) : COLOR{
	//ノーマルマップ
	float3 normal = lerp(float3(0,0,1),UnpackNormal(tex2D(_BumpMap,i.tex * _BumpMap_ST.xy + _BumpMap_ST.zw)),_NormalScale);

	//光源方向
	float3 LightDir = i.lightDir;
	_LightColor0.a = 1;
	//スぺキュラ
	float3 HalfVector = normalize(normalize(i.Eye) + normalize(LightDir));
	float4 SpecularCol = tex2D(_SpecularTex, i.tex * _SpecularTex_ST.xy + _SpecularTex_ST.zw);
	float3 Specular = pow(max(0, dot(HalfVector, normalize(normal))), _SpecularPower) * _SpecularColor * _LightColor0 * SpecularCol.rgb * 2.0;

	float shadow = LIGHT_ATTENUATION(i);

	float4 retCol = float4(0,0,0,1);

	float4 MainCol;
	MainCol = tex2D(_MainTex, i.tex * _MainTex_ST.xy + _MainTex_ST.zw) * _MainColor;

	float4 ShadowCol;
	ShadowCol = tex2D(_ShadowTex, i.tex * _ShadowTex_ST.xy + _ShadowTex_ST.zw) * _ShadowColor;
	if (_ShadowPower > 1) ShadowCol *= pow(MainCol, _ShadowPower);

	MainCol.rgb = lerp(MainCol.rgb, MainCol.rgb*i.vlight, _UnityAmbient);
	ShadowCol.rgb = lerp(ShadowCol.rgb, ShadowCol.rgb*i.vlight, _UnityAmbient);

	//スフィア
	float4 SpCol = tex2D(_SphereTex,i.sptex * _SphereTex_ST.xy + _SphereTex_ST.zw);
	SpCol *= _SphereColor;
	MainCol = lerp(MainCol + float4(SpCol.rgb,0), MainCol * SpCol, _SphereMode);
	ShadowCol = lerp(ShadowCol + SpCol, ShadowCol * SpCol, _SphereMode);
	MainCol.rgb += Specular;

	//影なりやすさ
	float4 ShadowAndHighLightTex = tex2D(_ShadowRateTex, i.tex * _ShadowRateTex_ST.xy + _ShadowRateTex_ST.zw);
	float ShadowRate = _ShadowRate - (ShadowAndHighLightTex.r * 2 - 1);
	float HighLightRate = _HighlightRate - (ShadowAndHighLightTex.g * 2 - 1);
	float d = max(0, dot(normalize(LightDir.xyz), normalize(normal))) * shadow;

	float ramp = tex2D(_RampTex, float2(max(0, min(1, d - ShadowRate)), 0.5) * _RampTex_ST.xy + _RampTex_ST.zw).r;
	float rampHL = tex2D(_RampTex, float2(max(0, min(1, d - HighLightRate)), 0.5) * _RampTex_ST.xy + _RampTex_ST.zw).a;

	//ramp *= shadow;
	float4 HL = (1 - rampHL)*_LightColor0 *_HighlightColor*tex2D(_HighlightTex, i.tex * _HighlightTex_ST.xy + _HighlightTex_ST.zw) * 2;
	HL.a = 0;
	retCol = lerp(ShadowCol * _LightColor0, MainCol * _LightColor0 + HL, ramp);

	//リムライト
	float4 Rim = tex2D(_RimLightTex, i.tex * _RimLightTex_ST.xy + _RimLightTex_ST.zw) * _RimLightColor;
	float RimD = dot(normalize(i.Eye),normalize(normal));

	float rim_ramp = 1 - tex2D(_RimRampTex, float2(max(0, min(1, pow(RimD, _RimLightPower))), 0.5) * _RimRampTex_ST.xy + _RimRampTex_ST.zw).r;
	Rim.rgb = rim_ramp * Rim.rgb * lerp(1,_LightColor0, _LightInfluence_Rim);

	float RimLV = max(0,dot(normalize(-i.Eye), normalize(LightDir)));
	Rim *= lerp(1,RimLV, _LightInfluence_Rim);
	retCol.rgb += Rim.rgb * 8;
#if _ALPHATEST_ON
	half alpha = retCol.a;
	clip(alpha - _Cutoff);
#endif
	return retCol;
	}

		ENDCG
	}
		//-----------------------------------------------------------------------------
		//サブライト用パス
		
		Pass{
		Name "BMT_BASIC_SUB"
		Tags{ "LightMode" = "ForwardAdd" }

		Blend One One
		CGPROGRAM
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fwdadd_fullshadows
#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
#include "UnityCG.cginc"
#include "AutoLight.cginc"

		uniform float _UnityAmbient;
		uniform float _ShadowRate;
		uniform sampler2D _ShadowRateTex;
		uniform float4 _ShadowRateTex_ST;
		uniform float4 _MainColor;
		uniform sampler2D _ShadowTex;
		uniform float4 _ShadowTex_ST;
		uniform float4 _ShadowColor;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform sampler2D _RampTex;
		uniform float4 _RampTex_ST;
		uniform float _ShadowPower;
		uniform fixed4 _LightColor0;

		uniform sampler2D _SpecularTex;
		uniform float4 _SpecularTex_ST;
		uniform float4 _SpecularColor;
		uniform float _SpecularPower;

		uniform sampler2D _SphereTex;
		uniform float4 _SphereTex_ST;
		uniform float4 _SphereColor;
		uniform float _SphereMode;

		uniform sampler2D _BumpMap;
		uniform float4 _BumpMap_ST;
		uniform float _NormalScale;

		uniform float4 _RimLightColor;
		uniform float _RimLightPower;
		uniform float _LightInfluence_Rim;
		uniform sampler2D _RimLightTex;
		uniform float4 _RimLightTex_ST;
		uniform sampler2D _RimRampTex;
		uniform float4 _RimRampTex_ST;

		uniform sampler2D _HighlightTex;
		uniform float4 _HighlightTex_ST;
		uniform float _HighlightRate;
		uniform float4 _HighlightColor;

		uniform float _Cutoff;

	//バーテックスシェーダからフラグメントシェーダに渡す構造体
	struct v2f {
		float4 pos : SV_POSITION;
		float2 tex : TEXCOORD0;
		float3 Eye : TEXCOORD1;
		float3 lightDir : TEXCOORD2;
		float2 sptex : TEXCOORD3;
		float3 vlight : TEXCOORD4;
		float3 PoitLight : TEXCOORD5;
		LIGHTING_COORDS(6, 7)
	};
	//バーテックスシェーダ
	v2f vert(appdata_full v) {
		v2f o;
		o = (v2f)0;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.tex = v.texcoord;

		TANGENT_SPACE_ROTATION;
		o.Eye = mul(rotation, ObjSpaceViewDir(v.vertex));
		o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex));


		float2 NormalWV = normalize(mul((fixed3x3)UNITY_MATRIX_V, mul(unity_ObjectToWorld, float4(v.normal.xyz, 0.0)).xyz));
		o.sptex.x = NormalWV.x * 0.5f + 0.5f;
		o.sptex.y = NormalWV.y * 0.5f + 0.5f;

		float3 worldN = mul((float3x3)unity_ObjectToWorld, SCALED_NORMAL);
#ifdef LIGHTMAP_OFF
		float3 shlight = ShadeSH9(float4(worldN, 1.0));
		o.vlight = shlight * 2;
#ifdef VERTEXLIGHT_ON
		float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
		o.PoitLight = Shade4PointLights(
			unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
			unity_LightColor0, unity_LightColor1, unity_LightColor2, unity_LightColor3,
			unity_4LightAtten0, worldPos, worldN);
#endif
#endif

		TRANSFER_VERTEX_TO_FRAGMENT(o);

		return o;
	}
	//フラグメントシェーダ
	float4 frag(v2f i) : COLOR{
		//ノーマルマップ
		float3 normal = lerp(float3(0,0,1),UnpackNormal(tex2D(_BumpMap,i.tex * _BumpMap_ST.xy + _BumpMap_ST.zw)),_NormalScale);
		//光源方向
		float3 LightDir = i.lightDir;
		float len = length(LightDir);

		//スぺキュラ
		float3 HalfVector = normalize(normalize(i.Eye) + normalize(LightDir));
		float4 SpecularCol = tex2D(_SpecularTex, i.tex * _SpecularTex_ST.xy + _SpecularTex_ST.zw);
		float3 Specular = pow(max(0, dot(HalfVector, normalize(normal))), _SpecularPower) * _SpecularColor * _LightColor0 * SpecularCol.rgb * 2.0;

		float shadow = LIGHT_ATTENUATION(i);
		float4 retCol = float4(0,0,0,1);

		float4 MainCol;
		MainCol = tex2D(_MainTex, i.tex * _MainTex_ST.xy + _MainTex_ST.zw) * _MainColor;

		//スフィア
		float4 SpCol = tex2D(_SphereTex,i.sptex * _SphereTex_ST.xy + _SphereTex_ST.zw);
		SpCol *= _SphereColor;
		MainCol = lerp(MainCol + SpCol, MainCol * SpCol, _SphereMode);
		MainCol.rgb += Specular;

		//影なりやすさ
		float4 ShadowAndHighLightTex = tex2D(_ShadowRateTex, i.tex * _ShadowRateTex_ST.xy + _ShadowRateTex_ST.zw);
		float ShadowRate = _ShadowRate - (ShadowAndHighLightTex.r * 2 - 1);
		float HighLightRate = _HighlightRate - (ShadowAndHighLightTex.g * 2 - 1);
		float d = max(0, dot(normalize(LightDir.xyz), normalize(normal))) * shadow;

		float ramp = tex2D(_RampTex, float2(max(0, min(1, d - ShadowRate)), 0.5) * _RampTex_ST.xy + _RampTex_ST.zw).r;
		float rampHL = tex2D(_RampTex, float2(max(0, min(1, d - HighLightRate)), 0.5) * _RampTex_ST.xy + _RampTex_ST.zw).a;

		//ramp *= shadow;
		float4 HL = (1 - rampHL)*_LightColor0 *_HighlightColor*tex2D(_HighlightTex, i.tex * _HighlightTex_ST.xy + _HighlightTex_ST.zw) * 2;
		HL.a = 0;

		//ramp *= shadow;
		retCol = lerp(0, MainCol * _LightColor0 + (1 - rampHL)*_LightColor0 + HL, ramp);

#if _ALPHATEST_ON
		half alpha = retCol.a;
		clip(alpha - _Cutoff);
#endif
		return retCol;
	}

		ENDCG
	}
		//----------------------------------------------------------------------------------------
		//エッジ---
		Pass{

		Cull         Front                            // Front(表) は非表示
		ZTest        Less                             // 深度バッファと比較(近距離)

		CGPROGRAM                                     // Cgコード
#pragma shader_feature USE_EDGE
#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
#include "UnityCG.cginc"                      // 基本セット
#pragma target 3.0                            // Direct3D 9 上の Shader Model 3.0 にコンパイル
#pragma vertex        vertFunc                // バーテックスシェーダーに vertFunc を使用
#pragma fragment    fragFunc                  // フラグメントシェーダーに fragFunc を使用

													  // Cgコード内で、使用する宣言

	uniform	sampler2D _EdgeColTex;
	uniform sampler2D _EdgeSizeTex;
	float _EdgeSize;
	float4 _EdgeColor;
	float _UseEdge;

	float4 _MainTex_ST;                           // uv

	struct v2f {                                  // vertex シェーダーと fragment シェーダーの橋渡し
		float4 pos      : SV_POSITION;
		float2 uv       : TEXCOORD0;
	};

	v2f vertFunc(appdata_tan v) {                 // Vertex Shader
		v2f o = (v2f)0;
		#if !USE_EDGE
			return o;
		#else
			o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
			float4 pos = UnityObjectToClipPos(v.vertex);                               // 頂点
			float4 normal = normalize(UnityObjectToClipPos(float4(v.normal, 0)));    // 法線

			float scale_col = tex2Dlod(_EdgeSizeTex, float4(o.uv,0,0)).r;
			float  edgeScale = _EdgeSize * 0.002 * scale_col * saturate(1 - length(WorldSpaceViewDir(v.vertex))*0.05);                                       // Edge スケール係数
			float4 addpos = normal * pos.w * edgeScale;                                 // 頂点座標拡張方向とスケール
			o.pos = pos + addpos;
			return o;
		#endif
	}

	float4 fragFunc(v2f i) : COLOR{               // Fragment Shader
		#if !USE_EDGE
				return 0;
		#else
			float4 col = tex2D(_EdgeColTex, i.uv.xy);
			col *= _EdgeColor;
			return col;
		#endif
	}
		ENDCG                                         // Cgコード終了


	}
	}
	FallBack "VertexLit"
	CustomEditor "BMToonGUI"
}
