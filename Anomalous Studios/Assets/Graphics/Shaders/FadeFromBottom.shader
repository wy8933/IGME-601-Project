Shader "Custom/FadeFromBottom_URP"
{
    Properties
    {
        _BaseMap("Base Map", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _FadeStart("Fade Start Height", Float) = 0.0
        _FadeRange("Fade Range", Float) = 1.0
        [Toggle] _DoubleSided("Render Both Sides", Float) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _BaseMap;
            float4 _BaseMap_ST;
            float4 _BaseColor;
            float _FadeStart;
            float _FadeRange;

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.worldPos = TransformObjectToWorld(v.positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _BaseMap);
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                half4 col = tex2D(_BaseMap, i.uv) * _BaseColor;

                
                float fade = saturate((i.worldPos.y - _FadeStart) / _FadeRange);
                col.a *= fade;

                return col;
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Lit"
}
