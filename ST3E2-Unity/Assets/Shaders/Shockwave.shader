Shader "Custom/Shockwave"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Alpha("Alpha", Range(0,1)) = 1
		_Color("Color", Color) = (1,1,1,1)

	}
		SubShader
		{
			Tags { "Queue" = "Transparent+2" "RenderType" = "Transparent" }
			LOD 100

			Blend One One
			Lighting Off
			//Cull Off
			//ZTest Always
			//ZWrite Off
			Fog { Mode Off }

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
					float4 position : POSITION;
					float2 uv : TEXCOORD0;
					float3 normal: NORMAL;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					float4 worldSpacePos : TEXCOORD1;
					float4 vertex : POSITION;
					float3 normal : NORMAL;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float4 _Color;
				float _Alpha;

				v2f vert(appdata v)
				{
					v2f o;
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					o.normal = v.normal;
					o.worldSpacePos = mul(unity_ObjectToWorld, v.position);
					o.vertex = UnityObjectToClipPos(v.position);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					// sample the texture
					float3 camPos = _WorldSpaceCameraPos;
					float3 camVec = normalize(camPos - i.worldSpacePos);
					float rampCoords = (1 - dot(camVec, i.normal));
					fixed4 col = tex2D(_MainTex, (0, rampCoords));
					col.rgb *= _Alpha;//_Color.a;
					// apply fog
					return col;
				}
				ENDCG
			}
		}
}
