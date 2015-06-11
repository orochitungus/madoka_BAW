Shader "Custom/TestShader" {
	Properties {
		_MainTex ("Base", 2D) = "white" {}
	}
	SubShader {
		Tags {
             "Queue" = "Transparent"
         }

		 // First Pass
         Cull Front
		
		CGPROGRAM
		#pragma surface surf Lambert alpha


		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float4 vtxColor : COLOR;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			 half4 color = tex2D( _MainTex, IN.uv_MainTex );
             o.Albedo = color.rgb;
             o.Alpha = color.a;

		}
		ENDCG
		
		// Second Pass
        Cull Back

		CGPROGRAM
		#pragma surface surf Lambert alpha


		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float4 vtxColor : COLOR;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			 half4 color = tex2D( _MainTex, IN.uv_MainTex );
             o.Albedo = color.rgb;
             o.Alpha = color.a;

		}
		ENDCG
	} 
	FallBack "Diffuse"
}
