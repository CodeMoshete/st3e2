Shader "Custom/Planets/PlanetSurface" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_LightsColor ("Lights Color", Color) = (1,1,1,1)
		_MainTex ("Diffuse (RGB) Alpha (A)", 2D) = "white" {}
		_IllumTex ("Illumination Map", 2D) = "black" {}
		_IllumMult ("Illumination Multiplier", Range(0, 5)) = 1
		_GlareIntensity ("Glare Intensity", Range(0,2)) = 0.2
		_GlareFocus ("Glare Focus", Range(1,50)) = 2
	}

	SubShader{
		Tags { "RenderType" = "Opaque"}
		
		Blend SrcAlpha OneMinusSrcAlpha 
		
		CGPROGRAM
			#pragma surface surf TF3 alphatest:Off
			#pragma target 3.0

			struct Input
			{
				float2 uv_MainTex;
				float3 worldNormal;
				fixed3 viewDir;
				fixed3 lightDir;
			};
			
			struct EditorSurfaceOutput {
				half3 Albedo;
				half3 Normal;
				half3 Emission;
				half3 Gloss;
				half Specular;
				half Alpha;
				fixed3 CustomIllumination;
				half4 Lighting2;
			};
			
			sampler2D _MainTex;
			sampler2D _IllumTex;
			float _AlphaMult;
			float _AlphaContrast;
			float _GlareIntensity;
			float _GlareFocus;
			fixed4 _Color;
			fixed4 _LightsColor;
			float _IllumMult;
			
			inline fixed4 LightingTF3(EditorSurfaceOutput s, fixed3 lightDir, fixed3 viewDir, fixed atten)
			{
				fixed4 c = fixed4(0,0,0,0);
				float lightAmt = dot(s.Normal, lightDir);
				fixed3 nightIllum = fixed3(0,0,0);
				if(lightAmt > 0)
				{
					lightAmt = min(_GlareIntensity, pow(lightAmt, _GlareFocus));
				}
				else
				{
					nightIllum = (s.CustomIllumination * (-lightAmt));
				}
				
				c.rgb = lightAmt + nightIllum;
				return c;
			}
			
			void surf (Input IN, inout EditorSurfaceOutput o)
			{
				o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb * _Color.rgb;
				o.CustomIllumination = tex2D(_IllumTex, IN.uv_MainTex).rgb * _LightsColor.rgb * _IllumMult;
				o.Alpha = 1;
			}
		ENDCG
	}
	Fallback "Transparent/Cutout/Bumped Specular"
}
