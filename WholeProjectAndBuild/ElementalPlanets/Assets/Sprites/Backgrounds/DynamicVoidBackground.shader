Shader "Unlit/DynamicVoidBackground"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	
	_EtherealLock("Ethereal Lock",Int) = 0
		_EtherealColor1("Ethereal Color 1",Color) = (.5,.5,1,1)
		_EtherealColor2("Ethereal Color 2",Color) = (1,1,1,1)
		_EtherealFade("Ethereal Fade",Range(0.0,1.0)) = 0.0
		_SpinFactor("Spin Factor",Range(0.0,1.0)) = 1.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;

	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		UNITY_FOG_COORDS(1)
			float4 vertex : SV_POSITION;
		float3 worldpos : TEXCOORD1;
		float3 screenPos: TEXCOORD2;

	};

	sampler2D _MainTex;
	int _EtherealLock;
	float4 _EtherealColor1;
	float4 _EtherealColor2;
	float _EtherealFade; //This is the transition to Fade
	float _SpinFactor;
	float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldpos = mul(UNITY_MATRIX_T_MV, v.vertex).xyz;
				o.screenPos = ComputeScreenPos(o.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o, o.vertex);
				return o;
			}
			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
			float rti = ((frac(_Time.y / 10.0) % 1.0)*3.14);
			fixed4 colrot = tex2D(_MainTex, float2((i.uv.x*cos(rti))+ (i.uv.y*sin(rti)), (i.uv.x*sin(rti)) + (i.uv.y*cos(rti))));
			col = (col*lerp(float4(1,1,1,1),colrot,_SpinFactor));
			//fixed4 icecol = tex2D(_FrozenTex, float2((i.worldpos.x % 1), (i.worldpos.y % 1)));
			clip(col.a - .0001);
			// apply fog
			if (_EtherealLock > 0) {
				//float a = (abs(i.screenPos.y - .5) / .5);
				float X = ((i.screenPos.x - .5)/.5);
				float Y = ((i.screenPos.y - .5) / .5)*2;
				float mag = pow(X*X + Y*Y, .5);
				fixed4 bl = lerp(_EtherealColor1, _EtherealColor2, .5+(.25*(sin((mag+(sin(_Time.y*.2)*4*-1))*4))));

				bl = lerp(bl,fixed4(.075,.075,.075,1),_EtherealFade); //Add a little bit of screen space gradient lighting for a sort of spotlight effect

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
