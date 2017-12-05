Shader "Outline"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_OutLineColor("Outline Color", Color) = (1,1,1,1)
		_ExtrudeAmount("Extrude Amount", Range(0, 0.2)) = 0.2
	}
		SubShader
	{
		Cull Front
		Tags{ "RenderType" = "Transparent" }

		Pass
	{
		Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

		float4 _OutLineColor;
	struct appdata
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
	};
	float _ExtrudeAmount;

	v2f vert(appdata IN)
	{
		v2f o;
		float4 ex = IN.vertex;
		ex.xyz += (IN.normal.xyz * _ExtrudeAmount);
		o.vertex = mul(UNITY_MATRIX_MVP, ex);
		o.uv = IN.uv;
		return o;
	}

	float4 frag(v2f i) : SV_Target
	{
		// sample the texture
		float4 col = _OutLineColor;
		// apply fog
		return col;
	}
		ENDCG
	}


	}
}
