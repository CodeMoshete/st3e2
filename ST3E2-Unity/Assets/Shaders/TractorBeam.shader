Shader "Custom/TractorBeamUnlit"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_AddTex("Texture2", 2D) = "white" {}
		_Reveal("Reveal Amount", Range(0,1)) = 1
		_GradSize("Reveal Gradient Size", Range(0, 1)) = 0.2
	}
	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 100
		ZWrite Off
		Lighting Off
		Tags {Queue = Transparent}
		Blend One One
		Cull Off

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
				float2 auv : TEXCOORD1;
				float2 buv : TEXCOORD2;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 auv : TEXCOORD1;
				float2 buv : TEXCOORD2;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _AddTex;
			float4 _AddTex_ST;
			float _Reveal;
			float _GradSize;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.auv = TRANSFORM_TEX(v.auv, _AddTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 add = tex2D(_AddTex, i.auv);
				col.rgb = min(col.rgb, add.rgb);

				float gradValue = (1 - _Reveal) * _GradSize;
				float lowerBound = (1 - _Reveal) - (_GradSize - gradValue);
				float alphaRampVal = saturate((1 - i.uv.y - lowerBound) / _GradSize);
				col.rgb *= alphaRampVal;

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
