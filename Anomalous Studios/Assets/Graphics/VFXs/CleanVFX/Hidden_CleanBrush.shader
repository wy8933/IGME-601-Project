Shader "Hidden/CleanBrush"
{
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            ZWrite Off
            ZTest Always
            Cull Off
            Blend One Zero

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // input previous mask
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            // parameters: brushUV¡¢radius¡¢softedge¡¢strength
            float2 _BrushUV;
            float  _BrushRadius;
            float  _BrushSoft;
            float  _BrushStrength;

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
                // old mask
                float oldMask = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv).r;

                // calculate distance
                float dist = distance(i.uv, _BrushUV);

                // calculate soft edge
                float inner = _BrushRadius * (1.0 - _BrushSoft);
                float t = saturate(1.0 - smoothstep(inner, _BrushRadius, dist));

                float newMask = saturate(oldMask + t * _BrushStrength);

                // output to R channel
                return float4(newMask, newMask, newMask, 1);
            }
            ENDHLSL
        }
    }
}
