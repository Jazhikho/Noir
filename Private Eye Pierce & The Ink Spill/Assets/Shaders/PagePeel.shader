Shader "UI/PagePeel"
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
        _CurlAmount ("Curl Amount", Range(0, 1)) = 0
        _CurlFromRight ("Curl From Right (1) or Left (0)", Float) = 1
        _PeelSoftness ("Peel Edge Softness", Range(0.01, 0.3)) = 0.05
    }
    SubShader
    {
        Tags { "Queue" = "Overlay" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _CurlAmount;
            float _CurlFromRight;
            float _PeelSoftness;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float alpha;
                if (_CurlFromRight > 0.5)
                {
                    float curlLine = 1.0 - _CurlAmount;
                    float edge = uv.x;
                    alpha = 1.0 - smoothstep(curlLine, curlLine + _PeelSoftness, edge);
                }
                else
                {
                    float curlLine = _CurlAmount;
                    float edge = uv.x;
                    alpha = smoothstep(curlLine - _PeelSoftness, curlLine, edge);
                }
                fixed4 col = tex2D(_MainTex, uv);
                col.a *= alpha;
                return col;
            }
            ENDCG
        }
    }
    Fallback "UI/Default"
}
