﻿Shader "UI Extensions/SoftMaskShader"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)

		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15

		_Min("Min",Vector) = (0,0,0,0)
		_Max("Max",Vector) = (1,1,0,0)
		_AlphaMask("AlphaMask - Must be Wrapped",2D) = "white"{}
		_AlphaUV("AlphaUV",Vector) = (0,0,0,0)
		_CutOff("CutOff",Float) = 0
		[MaterialToggle]
		_HardBlend("HardBlend",Float) = 0
		_FlipAlphaMask("Flip Alpha Mask",int) = 0
		_NoOuterClip("Outer Clip",int) = 0
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Stencil
		{
			Ref[_Stencil]
			Comp[_StencilComp]
			Pass[_StencilOp]
			ReadMask[_StencilReadMask]
			WriteMask[_StencilWriteMask]
		}

		LOD 0

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest[unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask[_ColorMask]

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
			};

			fixed4 _Color;
			fixed4 _TextureSampleAdd;

			bool _UseAlphaClip;

			float4 _ProgressColor;
			float _Value;
			int _LeftToRight;

			int _FlipAlphaMask = 0;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.worldPosition = IN.vertex;
				OUT.vertex = mul(UNITY_MATRIX_MVP, OUT.worldPosition);

				OUT.texcoord = IN.texcoord;

				#ifdef UNITY_HALF_TEXEL_OFFSET
					OUT.vertex.xy += (_ScreenParams.zw - 1.0)*float2(-1,1);
				#endif

				OUT.color = IN.color * _Color;
				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaMask;

			float2 _AlphaUV;

			float2 _Min;
			float2 _Max;

			float _CutOff;

			bool _HardBlend = false;
			bool _NoOuterClip = false;

			fixed4 frag(v2f IN) : SV_Target
			{
				half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;

				// Do we want to clip the image to the Mask Rectangle?
				if (!_NoOuterClip && (IN.texcoord.x < _Min.x || IN.texcoord.x > _Max.x || IN.texcoord.y < _Min.y || IN.texcoord.y > _Max.y)) // Yes we do
					color.a = 0;
				else // It's in the mask rectangle, so apply the alpha of the mask provided.
				{
					float a = tex2D(_AlphaMask, (IN.texcoord - _Min) / _AlphaUV).a;

					if (a <= _CutOff)
						a = 0;
					else
					{
						if(_HardBlend)
							a = 1;
					}

					if (_FlipAlphaMask == 1)
						a = 1 - a;

					if(!(IN.texcoord.x < _Min.x || IN.texcoord.x > _Max.x || IN.texcoord.y < _Min.y || IN.texcoord.y > _Max.y))
						color.a *= a;
				}

				if (_UseAlphaClip)
					clip(color.a - 0.001);

				return color;
			}
			ENDCG
		}
	}
}