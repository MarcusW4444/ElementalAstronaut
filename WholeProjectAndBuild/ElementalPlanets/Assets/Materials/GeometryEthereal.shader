﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/GeometryEthreal"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		
		
		_EtherealLock("Ethereal Lock",Int) = 0
		_EtherealColor1 ("Ethereal Color 1",Color) = (.5,.5,1,1)
		_EtherealColor2("Ethereal Color 2",Color) = (1,1,1,1)
		_EtherealFade("Ethereal Fade",Range(0.0,1.0)) = 0.0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" }
		LOD 100
		Cull Off
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag alpha
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color    : COLOR;
				float2 uv : TEXCOORD0;
				
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
					fixed4 color : COLOR;
				float4 vertex : SV_POSITION;
				float3 worldpos : TEXCOORD1;
				float3 screenPos: TEXCOORD2;
				
			};

			sampler2D _MainTex;
			int _EtherealLock;
			float4 _Color;
			float4 _EtherealColor1;
			float4 _EtherealColor2;
			float _EtherealFade; //This is the transition to Fade
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldpos = mul(UNITY_MATRIX_T_MV, v.vertex).xyz;
				o.screenPos = ComputeScreenPos(o.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = (tex2D(_MainTex, i.uv)*i.color);
				//fixed4 icecol = tex2D(_FrozenTex, float2((i.worldpos.x % 1), (i.worldpos.y % 1)));
			clip(col.a - .0001);
				// apply fog
			if (_EtherealLock > 0) {
				float a = (abs(i.screenPos.y-.5)/.5);
				fixed4 bl = lerp(_EtherealColor1, _EtherealColor2, .5+(.25*(sin((i.screenPos.x+_Time.y)*2)))+(.25*sin(((_Time.y*0*20)+ (a*100))*.1)*(1-_EtherealFade)));
				
				bl = lerp(bl,fixed4(.1,.1,.1,1),_EtherealFade); //Add a little bit of screen space gradient lighting for a sort of spotlight effect

				UNITY_APPLY_FOG(i.fogCoord, col);

				return bl;
			}
			else {
					UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			
			}
			ENDCG
		}
	}
}
