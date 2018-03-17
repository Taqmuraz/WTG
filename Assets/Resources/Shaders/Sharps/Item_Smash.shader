Shader "D&T/Item_Smash" {
	Properties {
		_MainTex ("Texture (RGB)", 2D) = "white" {}
		_AmbientColor("Ambient color", Color) = (1,1,1,1)
		_EmissivColor("Emissiv color", Color) = (1,1,1,1)
		_Power ("Power", Range(0, 1)) = 0.5
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};
		fixed4 _AmbientColor;
		fixed4 _EmissivColor;
		float _Power;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = pow((_AmbientColor + _EmissivColor), _Power);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
}
