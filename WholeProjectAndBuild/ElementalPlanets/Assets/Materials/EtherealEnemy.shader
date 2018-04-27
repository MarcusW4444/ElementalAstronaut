Shader "Unlit/EtherealEnemy"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_EtherealFactor("Ethereal Factor",Range(0,1)) = 0.0
	}
		SubShader
	{
		Tags{ "RenderType" = "Transparent" }
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
		float2 uv : TEXCOORD0;

	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		UNITY_FOG_COORDS(1)
			float4 vertex : SV_POSITION;
		float3 worldpos : TEXCOORD1;
	};

	sampler2D _MainTex;
	
	float _EtherealFactor;
	float4 _MainTex_ST;

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.worldpos = v.vertex.xyz;

		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		UNITY_TRANSFER_FOG(o,o.vertex);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		// sample the texture
		fixed4 col = tex2D(_MainTex, i.uv);
		
		float mx = max(max(col.r, col.g), col.b);
		float mn = min(min(col.r, col.g), col.b);
		
		float iscol = ((mx-mn)/1.0);
		fixed4 ethcol = fixed4(mx, mx, mx, col.a);//lerp()
		clip(col.a - .0001)
		// apply fog
		UNITY_APPLY_FOG(i.fogCoord, col);
	return lerp(col,ethcol, _EtherealFactor);
	}
		ENDCG
	}
	}
}
