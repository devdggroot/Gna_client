Shader "AlphaMask/Basic" 
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_AlphaTex("Alpha (A)", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }

		ZWrite Off

		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask RGB

		Pass
		{
			SetTexture[_MainTex]{
				
				ConstantColor[_Color]
				Combine texture * constant
			}
			
			SetTexture[_AlphaTex]{
				
				ConstantColor[_Color]
				Combine previous, texture * previous
			}
		}
	}
}