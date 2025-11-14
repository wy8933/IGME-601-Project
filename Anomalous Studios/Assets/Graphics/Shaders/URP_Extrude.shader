Shader "Hidden/Outline_Extrude"
{
    Properties
    {
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _Thickness("Thickness", Float) = 0.02
    }

    SubShader
    {
        Tags{ "RenderType"="Opaque" }

        Cull Front
        ZWrite Off
        ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _OutlineColor;
            float _Thickness;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;

                float3 norm   = normalize(IN.normalOS);
                float3 offset = IN.positionOS.xyz + norm * _Thickness;

                OUT.positionHCS = TransformObjectToHClip(offset);
                return OUT;
            }

            half4 frag (Varyings i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }
    }
}
