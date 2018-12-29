// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "RPG/ExpShader"
{
	Properties
	{
		_xgmTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (0,0,0,1)
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			struct vertexInput
			{
				float4 vertex:POSITION;
				float2 uv:TEXCOORD0;
				float3 norm:NORMAl;
			};
			struct vertexOutput
			{
				float4 position:SV_POSITION;
				float2 uv:TEXCOORD0;
				float3 worldNormal:TEXCOORD1;
			};

			vertexOutput vert (vertexInput v)
			{
				vertexOutput o;
				o.position = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.worldNormal = mul(unity_WorldToObject, v.norm);
				return o;
			}

			uniform sampler2D _xgmTex;
			uniform fixed4 _Color;

			fixed4 frag (vertexOutput v) : SV_Target
			{
				fixed4 col = tex2D(_xgmTex, v.uv);
				float3 l = normalize(_WorldSpaceLightPos0);
				float3 n = normalize(v.worldNormal);
				float dif = dot(n, l);
				return col * _Color * dif;
			}

			ENDCG
		}
	}
	Fallback "Diffuse"
}