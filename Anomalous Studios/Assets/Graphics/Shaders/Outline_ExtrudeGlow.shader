Shader "Custom/OutlineExtrudeGlow"
{
    Properties
    {
        _OutlineColor("Outline Color", Color) = (1,1,0,1)
        _Thickness("Thickness", Float) = 0.03
        _GlowIntensity("Glow Intensity", Float) = 4.0
    }

    SubShader
    {
        Tags{ "Queue"="Transparent+100" "RenderType"="Transparent" }

        Cull Front
        ZWrite Off
        ZTest Always
        Blend One One

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _OutlineColor;
            float _Thickness;
            float _GlowIntensity;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float3 norm = normalize(IN.normalOS);
                float3 pos = IN.positionOS.xyz + norm *_Thickness;

                OUT.positionHCS = TransformObjectToHClip(pos);
                return OUT;
            }

            half4 frag(Varyings i) : SV_Target
            {
                return _OutlineColor * _GlowIntensity;
            }
            ENDHLSL
        }
    }
}
