Shader "AlphaMask/Outline"
{
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_AlphaTex("Alpha (A)", 2D) = "white" {}

		_Distance("Distance", Float) = 1
		_OutlineColor("Outline", Color) = (0, 0, 0, 1)
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
			};

			sampler2D _MainTex;
			sampler2D _AlphaTex;

			float4 _MainTex_ST;

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			//basic AlphaMask.shader
			/*fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.texcoord);
				fixed4 col2 = tex2D(_AlphaTex, i.texcoord);

				return fixed4(col.r, col.g, col.b, col2.a);
			}*/
		
			float4 _MainTex_TexelSize;
			half4 _Color;
			
			float _Distance;
			half4 _OutlineColor;

			float4 sampleColor(float2 coord)
			{
				float4 temp = tex2D(_MainTex, coord);
				temp.a *= tex2D(_AlphaTex, coord).a;

				return temp * _Color; // Always apply _Color multiplier
			}

			half4 frag(v2f_img i) : SV_Target
			{
				float4 color = sampleColor(i.uv);

				// Simple sobel filter for the alpha channel.
				float d = _MainTex_TexelSize.xy * _Distance;

				/*half a1 = tex2D(_AlphaTex, i.uv + d * float2(-1, -1)).a;
				half a2 = tex2D(_AlphaTex, i.uv + d * float2(0, -1)).a;
				half a3 = tex2D(_AlphaTex, i.uv + d * float2(+1, -1)).a;

				half a4 = tex2D(_AlphaTex, i.uv + d * float2(-1, 0)).a;
				half a6 = tex2D(_AlphaTex, i.uv + d * float2(+1, 0)).a;

				half a7 = tex2D(_AlphaTex, i.uv + d * float2(-1, +1)).a;
				half a8 = tex2D(_AlphaTex, i.uv + d * float2(0, +1)).a;
				half a9 = tex2D(_AlphaTex, i.uv + d * float2(+1, +1)).a;*/
				half a1 = sampleColor(i.uv + d * float2(-1, -1)).a;
				half a2 = sampleColor(i.uv + d * float2(0, -1)).a;
				half a3 = sampleColor(i.uv + d * float2(+1, -1)).a;

				half a4 = sampleColor(i.uv + d * float2(-1, 0)).a;
				half a6 = sampleColor(i.uv + d * float2(+1, 0)).a;

				half a7 = sampleColor(i.uv + d * float2(-1, +1)).a;
				half a8 = sampleColor(i.uv + d * float2(0, +1)).a;
				half a9 = sampleColor(i.uv + d * float2(+1, +1)).a;

				float gx = -a1 - a2 * 2 - a3 + a7 + a8 * 2 + a9;
				float gy = -a1 - a4 * 2 - a7 + a3 + a6 * 2 + a9;

				float w = sqrt(gx * gx + gy * gy) / 4;

				// Mix the contour color.
				return half4(lerp(color.rgb, _OutlineColor.rgb, w), lerp(color.a, _OutlineColor.a, w));
			}
			ENDCG
		}
	}
	Fallback "AlphaMask/Basic"
}