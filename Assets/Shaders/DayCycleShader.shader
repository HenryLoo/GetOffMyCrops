Shader "GOMC/DayCycleShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_DayColour("Day Colour", Color) = (1, 0.5, 0, 1)
		_NightColour("Night Colour", Color) = (0.05, 0.1, 0.2, 1)
		_LevelProgress("Level Progress", Range( 0, 1 )) = 0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			fixed4 _DayColour;
			fixed4 _NightColour;
			float _LevelProgress;

			fixed4 frag (v2f i) : SV_Target
			{
				float multiplier = 0.5;
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 lightCol = lerp(_DayColour, _NightColour, _LevelProgress);
				col = lerp(col, lightCol, _LevelProgress * multiplier);

				return col;
			}
			ENDCG
		}
	}
}
