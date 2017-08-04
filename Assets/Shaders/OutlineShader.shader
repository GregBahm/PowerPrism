Shader "Hidden/OutlineShader"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "black" {}
		_OutlineColor("Outline COlor", Color) = (1,1,1,1)
		_OutlineHigh("Outline High", Range(0,1)) = 0.75
		_OutlineLow("Outline Lowt", Range(0,1)) = 0
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;
			sampler2D _FullBlurredTexture;
			float _OutlineHigh;
			float _OutlineLow;
			float4 _OutlineColor;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 base = tex2D(_MainTex, i.uv);
				fixed4 outlineSource = tex2D(_FullBlurredTexture, i.uv);
				bool outline = outlineSource.x < _OutlineHigh && outlineSource.x > _OutlineLow;
				return lerp(base, _OutlineColor, outline * _OutlineColor.a);
			}
			ENDCG
		}
	}
}
