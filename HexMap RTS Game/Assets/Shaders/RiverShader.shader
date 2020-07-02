Shader "Custom/RiverShader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
		SubShader
		{
			Tags { "RenderType" = "Transparent" "Queue" = "Transparent+1" }
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard alpha vertex:vert

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			#include "Water.cginc"
			#include "HexCellData.cginc"

			sampler2D _MainTex;

			struct Input
	{
		float2 uv_MainTex;
		float visibility;
	};

	half _Glossiness;
	half _Metallic;
	fixed4 _Color;

	// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
	// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
	// #pragma instancing_options assumeuniformscaling
	UNITY_INSTANCING_BUFFER_START(Props)
		// put more per-instance properties here
UNITY_INSTANCING_BUFFER_END(Props)


void vert(inout appdata_full v, out Input data)
	{
		UNITY_INITIALIZE_OUTPUT(Input, data);
		float4 cell0 = GetCellData(v, 0);
		float4 cell1 = GetCellData(v, 1);
		data.visibility = cell0.x * v.color.x + cell1.x * v.color.y;
		data.visibility = lerp(0.25, 1, data.visibility);
	}

	  void surf(Input IN, inout SurfaceOutputStandard o)
{
		float river = River(IN.uv_MainTex, _MainTex);

	fixed4 c = saturate(_Color + river);
	o.Albedo = c.rgb * IN.visibility;
	o.Metallic = _Metallic;
	o.Smoothness = _Glossiness;
	o.Alpha = c.a;
}
ENDCG
		}//a
			FallBack "Diffuse"
}
