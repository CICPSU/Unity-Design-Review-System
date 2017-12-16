﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "UI Extensions/SoftMaskShaderText"
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
		_CutOff("CutOff",Float) = 0
		[MaterialToggle]
		_HardBlend("HardBlend",Float) = 0
		_FlipAlphaMask("Flip Alpha Mask",int) = 0
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

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest[unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask[_ColorMask]

		Pass
		{
			CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members _ProgressColor,_Value,_Min,_Max,_Mul,_CutOff)
#pragma exclude_renderers d3d11 xbox360
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
				float4 worldPosition2 : COLOR1;

			fixed4 _Color;
			fixed4 _TextureSampleAdd;

			bool _UseAlphaClip;

			float4 _ProgressColor;
			float _Value;
			int _LeftToRight;

			int _HardBlend = false;

			int _FlipAlphaMask = 0;

			sampler2D _MainTex;
			sampler2D _AlphaMask;

			float2 _Min;
			float2 _Max;

			float2 _Mul;

			float _CutOff;
			int _NoOuterClip;
			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.worldPosition = IN.vertex;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
				OUT.worldPosition2 = mul(unity_ObjectToWorld, IN.vertex);

				OUT.texcoord = IN.texcoord;

				#ifdef UNITY_HALF_TEXEL_OFFSET
					OUT.vertex.xy += (_ScreenParams.zw - 1.0)*float2(-1,1);
					OUT.worldPosition2 += (_ScreenParams.zw - 1.0)*float2(-1, 1);
				#endif

				OUT.color = IN.color * _Color;
				return OUT;
			}



			fixed4 frag(v2f IN) : SV_Target
			{
				half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd);

				
				// Do we want to clip the image to the Mask Rectangle?
				if (IN.worldPosition2.x <= _Min.x || IN.worldPosition2.x >= _Max.x || IN.worldPosition2.y <= _Min.y || IN.worldPosition2.y >= _Max.y)
					color.a = 0;
				else // It's in the mask rectangle, so apply the alpha of the mask provided.
				{
					float a = tex2D(_AlphaMask, (IN.worldPosition2.xy - _Max) / (_Max-_Min)).a;

					if (a <= _CutOff)
						a = 0;
					else
					{
						if (_HardBlend)
							a = 1;
					}

					if (_FlipAlphaMask == 1)
						a = 1 - a;

					if(!(IN.worldPosition2.x <= _Min.x || IN.worldPosition2.x >= _Max.x || IN.worldPosition2.y <= _Min.y || IN.worldPosition2.y >= _Max.y))
						color *= a;					
				}

				if (_UseAlphaClip)
					clip(color.a - 0.001);

				return color;
			}
			ENDCG
		}
	}
}