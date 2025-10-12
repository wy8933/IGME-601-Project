Shader "URP/CleanSurface"
{
    Properties
    {
        _CleanTex ("Clean Texture", 2D) = "white" {}
        _DirtyTex ("Dirty Texture", 2D) = "black" {}
        _MaskTex  ("Clean Mask (Runtime)", 2D) = "black" {}
        _DirtyStrength ("Dirty Strength", Range(0, 2)) = 1
        _Contrast ("Mask Contrast", Range(0.1, 4)) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "ForwardUnlit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_CleanTex); SAMPLER(sampler_CleanTex);
            TEXTURE2D(_DirtyTex); SAMPLER(sampler_DirtyTex);
            TEXTURE2D(_MaskTex);  SAMPLER(sampler_MaskTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _CleanTex_ST;
                float4 _DirtyTex_ST;
                float _DirtyStrength;
                float _Contrast;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
            };

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            float4 frag (Varyings i) : SV_Target
            {
                float2 uv = i.uv;

                // sampling the texture
                float4 clean = SAMPLE_TEXTURE2D(_CleanTex, sampler_CleanTex, uv);
                float4 dirty = SAMPLE_TEXTURE2D(_DirtyTex, sampler_DirtyTex, uv) * _DirtyStrength;
                float mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, uv).r;

                // soft or sharp edge
                mask = saturate(pow(mask, 1.0 / _Contrast));

                // mask=0 ¡ú dirty, mask=1 ¡ú clean
                float4 finalColor = lerp(dirty, clean, mask);
                return finalColor;
            }
            ENDHLSL
        }
    }
    FallBack Off
}
