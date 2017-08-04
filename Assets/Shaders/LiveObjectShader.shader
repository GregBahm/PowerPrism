Shader "Unlit/LiveObjectShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma geometry geo
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2g
			{
				float2 uv : TEXCOORD0;
				float4 worldPos : TEXCOORD1;
				float4 miniPos : TEXCOORD2;
			};

			struct g2f
			{
				float2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
			};

			sampler2D _MainTex;

			float4x4 _miniTransform;
			
			v2g vert (appdata v)
			{
				v2g o;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.miniPos = mul(_miniTransform, v.vertex);
				o.uv = v.uv;
				return o;
			}

			[maxvertexcount(6)]
			void geo(triangle v2g p[3], inout TriangleStream<g2f> triStream)
			{
				g2f o;
				o.uv = p[0].uv;
				o.pos = mul(UNITY_MATRIX_VP, p[0].worldPos);
				triStream.Append(o);

				o.uv = p[1].uv;
				o.pos = mul(UNITY_MATRIX_VP, p[1].worldPos);
				triStream.Append(o);

				o.uv = p[2].uv;
				o.pos = mul(UNITY_MATRIX_VP, p[2].worldPos);
				triStream.Append(o);

				triStream.RestartStrip();

				o.uv = p[0].uv;
				o.pos = mul(UNITY_MATRIX_VP, p[0].miniPos);
				triStream.Append(o);

				o.uv = p[1].uv;
				o.pos = mul(UNITY_MATRIX_VP, p[1].miniPos);
				triStream.Append(o);

				o.uv = p[2].uv;
				o.pos = mul(UNITY_MATRIX_VP, p[2].miniPos);
				triStream.Append(o);
			}
			
			fixed4 frag (g2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}
	}
}
