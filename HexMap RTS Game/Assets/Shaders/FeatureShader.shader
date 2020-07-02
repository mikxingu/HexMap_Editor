Shader "Custom/FeatureShader" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		[NoScaleOffset] _GridCoordinates("Grid Coordinates", 2D) = "white" {}
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			#pragma surface surf Standard fullforwardshadows vertex:vert
			#pragma target 3.0

			#include "/HexCellData.cginc"

			sampler2D _MainTex, _GridCoordinates;

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;

			struct Input {
				float2 uv_MainTex;
				float visibility;
			};

			void vert(inout appdata_full v, out Input data)
	 {
UNITY_INITIALIZE_OUTPUT(Input, data);
float3 pos = mul(unity_ObjectToWorld, v.vertex);

float4 gridUV = float4(pos.xz, 0, 0);
gridUV.x *= 1 / (4 * 8.66025404);
gridUV.y *= 1 / (2 * 15.0);
float2 cellDataCoordinates =
	floor(gridUV.xy) + tex2Dlod(_GridCoordinates, gridUV).rg;
cellDataCoordinates *= 2;

data.visibility = GetCellData(cellDataCoordinates).x;
data.visibility = lerp(0.25, 1, data.visibility);
}

void surf(Input IN, inout SurfaceOutputStandard o)
{
fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
o.Albedo = c.rgb * IN.visibility;
o.Metallic = _Metallic;
o.Smoothness = _Glossiness;
o.Alpha = c.a;
}
ENDCG
	}
		FallBack "Diffuse"
}