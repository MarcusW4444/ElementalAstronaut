// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/CamShader" {

	Properties{
		_Desaturation("Desaturation",Range(0,1)) = 0
		_Death("Death",Range(0,1)) = 0
		_PainValue("PainValue",Range(0,1)) = 0
		_DeathTime("Death Time",Float) = 0
		_LifeValue("Life Value",Range(0,1)) = 1
		_StimValue("Stim Value",Range(0,1)) = 0
		_PostValue("Post Value",Int) = 4
		_MainTex("Pain Texture", 2D) = "white" {}

		_BorderCurve("Border Curve",Range(.1,10)) = 1
	}
		SubShader
	{
		// Draw ourselves after all opaque geometry
		Tags{ "Queue" = "Transparent" }

		// Grab the screen behind the object into _BackgroundTexture
		GrabPass
	{
		"_BackgroundTexture"
	}

		// Render the object with the texture generated above, and invert the colors
		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

		struct v2f
	{
		float4 grabPos : TEXCOORD0;
		float4 pos : SV_POSITION;
		
		float3 wpos : TEXCOORD1;
		//Screen pos please
	};
	float _Desaturation;
	float _Death;

	v2f vert(appdata_base v) {
		v2f o;
		// use UnityObjectToClipPos from UnityCG.cginc to calculate 
		// the clip-space of the vertex
		//o.wpos = v.vertex;
		o.wpos = mul(unity_ObjectToWorld, v.vertex).xyz;
		o.pos = UnityObjectToClipPos(v.vertex);
		
		// use ComputeGrabScreenPos function from UnityCG.cginc
		// to get the correct texture coordinate
		o.grabPos = ComputeGrabScreenPos(o.pos);
		return o;
	}

	sampler2D _BackgroundTexture;

	float _DeathTime;
	float _LifeValue;
	
	int _PostValue;
	half4 frag(v2f i) : SV_Target
	{
		half4 bgcolor = tex2Dproj(_BackgroundTexture, i.grabPos);
		half m = max(max(bgcolor.r, bgcolor.g), bgcolor.b);
		half av = ((bgcolor.r + bgcolor.g + bgcolor.b) / 3);
		half post = max(1,_PostValue);

		float rs = sqrt(pow((i.grabPos.x * 2) - 1, 2) + pow((i.grabPos.y*2) - 1,2))/(sqrt(2));

		if ((_Death > 0)&& (rs > _LifeValue) && (_LifeValue < 1)) {
			//half4 anti = half4(bgcolor.r*(1 - m), bgcolor.g*(1 - m), bgcolor.b*(1 - m), 1);
			int vu = int(av/(1.0 / post));
			half4 co = lerp(bgcolor, half4(0, 0, 0, 1), _Death*.25);
			co = half4(((half)vu)*(1/post), ((half)vu) * (1 / post), ((half)vu) * (1 / post),1); //Posterize
			//if (av >= .5)
				//co = lerp(bgcolor,half4(.5, .5, .5, 1), _Death);

			//co = lerp(co, half4(.2, .2, .2, 1), 1.0 - min(1.0,max(0.0,(1.0 / (1.0 + max((((_Time.y - ((float)_DeathTime)) - .8) / 2.0),0.0))))));
			return co;
		}
		else {
			//Process for Saturation and redness
		return lerp(bgcolor,half4(m,m,m,1), _Desaturation);
			//return bgcolor;
		}
		
	}
		ENDCG
	}

		Blend SrcAlpha OneMinusSrcAlpha
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

	};

	sampler2D _MainTex;
	sampler2D _PainTex;
	float4 _MainTex_ST;
	float _PainValue;
	float _BorderCurve;
	float _StimValue;
	float4 _Center;

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);

		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		UNITY_TRANSFER_FOG(o,o.vertex);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		// sample the texture
		fixed4 col = tex2D(_MainTex, i.uv);
	fixed4 paincol = tex2D(_MainTex, i.uv);
	// apply fog

	//float dist = sqrt((i.vertex.x - _Center.x)+ (i.vertex.y - _Center.y)+ (i.vertex.z - _Center.z)); //length(i.vertex.xyz - _Center);

	//col = lerp(col,fixed4(1,0,0,col.a),min(.99,dist/50));
	//clip(col-0.0001);
	//col = fixed4(0, 0, 0, 1);
	if (_PainValue < .5f) {
		//col = fixed4(col.r, col.g, col.b, col.a*(_PainValue/.5));
		col = fixed4(1-_StimValue, .8*_StimValue, 0.3*_StimValue,lerp(col.r*(_PainValue / .5), pow(col.r, 1.0 / _BorderCurve),(_PainValue / .5)));
	}
	else {
		float v = (1 - ((_PainValue - .5) / .5));

		
		col = fixed4((1* (1 - _StimValue) * v), .8*_StimValue, 0.3*_StimValue, pow(col.r, 1.0 / _BorderCurve));
	}

	//fixed4 stimcol = fixed4(0,.8,.3, pow(col.r, 1.0 / _BorderCurve));
	//col = lerp(col,stimcol, _StimValue);
	UNITY_APPLY_FOG(i.fogCoord, col);
	return col;
	}
		ENDCG
	}

	}
}