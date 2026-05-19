Shader "RetroRescue/TVNoise"
{
    Properties
    {
        _MainTex ("Main Tex", 2D) = "white" {}
        _NoiseOpacity ("Noise Opacity", Range(0,1)) = 0.5
        _Clarity ("Clarity", Range(0,1)) = 0.5
        _Speed ("Noise Speed", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float2 uv : TEXCOORD0; float4 vertex : SV_POSITION; };

            sampler2D _MainTex;
            float _NoiseOpacity;
            float _Clarity;
            float _Speed;
            float4 _Time;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // simple pseudo-noise using sin
            float noise(float2 uv)
            {
                float n = sin((uv.x + uv.y) * 50.0 + _Time.y * _Speed);
                return saturate(n * 0.5 + 0.5);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float n = noise(i.uv);
                // blend between noisy static and original using clarity
                fixed4 staticCol = lerp(fixed4(0.5,0.5,0.5,1.0), fixed4(0,0,0,1.0), n);
                float noiseBlend = lerp(_NoiseOpacity, 0.0, _Clarity);
                fixed4 outCol = lerp(staticCol, col, _Clarity);
                outCol = lerp(outCol, staticCol, noiseBlend);
                return outCol;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
