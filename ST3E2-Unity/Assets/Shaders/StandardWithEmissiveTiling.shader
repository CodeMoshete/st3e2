Shader "Custom/StandardWithEmissiveTiling"
{
    Properties
    {
        _Color ("Main Color", Color) = (1, 1, 1, 1)
		_EmissionColor("Emission Color", Color) = (1, 1, 1, 1)
        _MainTex ("Diffuse Texture", 2D) = "white" {}
        _EmissionMap ("Emissive Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;
        sampler2D _EmissionMap;
		float4 _Color;
		float4 _EmissionColor;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_EmissionMap;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            // Diffuse texture
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 emissive = tex2D(_EmissionMap, IN.uv_EmissionMap);

            o.Albedo = c.rgb * _Color.rgb;
            o.Emission = emissive.rgb * _EmissionColor.rgb;
            o.Alpha = c.a * _Color.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}