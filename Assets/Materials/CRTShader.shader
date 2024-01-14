Shader "Unlit/CRTShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScanlineSpeed ("Scanline Speed", Float) = 1.0
        _ScanlineFrequency ("Scanline Frequency", Float) = 50.0
        _ScanlineIntensity ("Scanline Intensity", Float) = 0.05
        _NoiseIntensity ("Noise Intensity", Float) = 0.25
        _DistortionAmount ("Distortion Amount", Float) = 0.0005
        _ColorBleedAmount ("Color Bleed Amount", Float) = 0.005
        _BarrelDistortionAmount ("Barrel Distortion Amount", Float) = 0.05
        _InterferenceAmount ("Interference Amount", Float) = 0.001
        _StaticNoiseIntensity ("Static Noise Intensity", Float) = 0.01
        _WarpIntensity ("Warp Intensity", Float) = 0.01
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
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
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            // Declare properties
            float _ScanlineSpeed;
            float _ScanlineFrequency;
            float _ScanlineIntensity;
            float _NoiseIntensity;
            float _DistortionAmount;
            float _ColorBleedAmount;
            float _BarrelDistortionAmount;
            float _InterferenceAmount;
            float _StaticNoiseIntensity;
            float _WarpIntensity;


            //Noise
            float noise(float2 coord)
            {
                return frac(sin(dot(coord, float2(12.9898, 78.233))) * 43758.5453);
            }

            v2f vert(appdata v)
            {
                // Apply barrel distortion to UV coordinates
                float2 uv = v.uv;
                uv = uv * 2.0 - 1.0; // Convert from 0-1 range to -1 to 1
                float dist = dot(uv, uv);

                uv *= 1.0 + dist * _BarrelDistortionAmount; // Barrel distortion
                uv = (uv + 1.0) / 2.0; // Convert back to 0-1 range
                
                v2f o;
                o.uv = uv;

                o.vertex = UnityObjectToClipPos(v.vertex);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Add image warping
                float warpIntensity = _WarpIntensity;
                float2 warp = noise(i.uv * _SinTime.y) * warpIntensity;
                i.uv += warp;

                // Sample the texture with the warped coordinates
                fixed4 col = tex2D(_MainTex, i.uv);

                // CRT Distortion
                float distortion = sin(i.uv.y * _ScanlineFrequency) * _DistortionAmount;
                col.r += tex2D(_MainTex, float2(i.uv.x + distortion, i.uv.y)).r;
                col.g += tex2D(_MainTex, float2(i.uv.x, i.uv.y + distortion)).g;
                col.b += tex2D(_MainTex, float2(i.uv.x - distortion, i.uv.y)).b;

                // Color Bleeding
                col.r = lerp(col.r, tex2D(_MainTex, float2(i.uv.x + distortion, i.uv.y)).r, _ColorBleedAmount);
                col.g = lerp(col.g, tex2D(_MainTex, float2(i.uv.x, i.uv.y + distortion)).g, _ColorBleedAmount);
                col.b = lerp(col.b, tex2D(_MainTex, float2(i.uv.x - distortion, i.uv.y)).b, _ColorBleedAmount);

                // CRT Scanlines
                float timeOffset = _Time.y * _ScanlineSpeed;
                float scanlineValue = sin((i.uv.y + timeOffset) * _ScanlineFrequency);

                float noiseValue = noise(i.uv * _Time.y) * _NoiseIntensity;
                float combinedEffect = abs(scanlineValue) * _ScanlineIntensity + noiseValue;
                combinedEffect = clamp(combinedEffect, 0.0, 1.0);

                float scanlineEffect = 1.0 - combinedEffect;
                col *= scanlineEffect;

                float interference = noise(i.uv * _Time.y) * _InterferenceAmount;
                col *= 1.0 - interference;

                // Add static noise
                float staticNoiseIntensity = _StaticNoiseIntensity;
                float staticNoise = noise(i.uv * _Time.y) * staticNoiseIntensity;
                col.rgb += staticNoise;


                return col;
            }
            ENDCG
        }
    }
}