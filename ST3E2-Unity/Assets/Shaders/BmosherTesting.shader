﻿Shader "Toon/Bmosher" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_RimColor ("Rim Color", Color) = (0.97,0.88,1,0.75)
		_RimPower ("Rim Power", Float) = 2.5
		_MainTex ("Diffuse (RGB) Alpha (A)", 2D) = "white" {}
		_BumpMap ("Normal (Normal)", 2D) = "bump" {}
		_SpecularTex ("Specular Level (R) Gloss (G) Rim Mask (B)", 2D) = "gray" {}
		_RampTex ("Toon Ramp (RGB)", 2D) = "white" {}
		_Cutoff ("Alphatest Cutoff", Range(0, 1)) = 0.5
	}

	SubShader{
		Tags { "RenderType" = "Opaque" }
		
		CGPROGRAM
			#pragma surface surf TF3 vertex:vert alphatest:_Cutoff
			#pragma target 3.0

			struct Input
			{
				float2 uv_MainTex;
				float3 worldNormal;
				INTERNAL_DATA
			};
			
			sampler2D _MainTex, _SpecularTex, _BumpMap, _RampTex;
			float4 _RimColor;
			float _RimPower;

			inline fixed4 LightingTF2 (SurfaceOutput s, fixed3 lightDir, fixed3 viewDir, fixed atten)
			{
				fixed3 h = normalize (lightDir + viewDir);
				
				fixed NdotL = dot(s.Normal, lightDir) * 0.5 + 0.5;
				fixed3 ramp = tex2D(_RampTex, float2(NdotL * atten)).rgb;
				
				float nh = max (0, dot (s.Normal, h));
				float spec = pow (nh, s.Gloss * 128) * s.Specular;
				
				fixed4 c;
				c.rgb = ((s.Albedo * ramp * _LightColor0.rgb + _LightColor0.rgb * spec) * (atten * 2));
				c.a = s.Alpha;
				return c;
			}
			
			inline fixed4 LightingTF3(SurfaceOutput s, fixed3 lightDir, fixed3 viewDir, fixed atten)
			{
				fixed4 c;
				c.rgb = -0.35 + (pow(dot(s.Normal, viewDir), 5));
//				c.rgb = (-0.35 + pow(dot(s.Normal, lightDir),20));
				return c;
			}

			void vert (inout appdata_full v) 
			{
//				float4 sn = mul(UNITY_MATRIX_IT_MV, float3(0,1,0));
//				float3 viewDir = normalize(WorldSpaceViewDir(v.vertex));
//				v.vertex.xyz += -dot(v.normal, viewDir);
				float3 test = float3(0,1,0);
				if(dot(v.normal.xyz, test) > 0.5)
				{
//					v.vertex.y += 0.25;
//					v.color.rgba = float4(1,1,1,1.0);
				}
			}
			
			void surf (Input IN, inout SurfaceOutput o)
			{
				o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
				o.Alpha = tex2D(_MainTex, IN.uv_MainTex).a;
				o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
//				float3 specGloss = tex2D(_SpecularTex, IN.uv_MainTex).rgb;
//				o.Specular = specGloss.r;
//				o.Gloss = specGloss.g;
				
//				half3 rim = pow(max(0, dot(float3(0, 1, 0), WorldNormalVector (IN, o.Normal))), _RimPower) * _RimColor.rgb * _RimColor.a * specGloss.b;
//				o.Emission = rim;
			}
		ENDCG
	}
	Fallback "Transparent/Cutout/Bumped Specular"
}
