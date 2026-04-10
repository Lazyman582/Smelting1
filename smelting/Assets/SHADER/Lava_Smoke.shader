Shader "Custom/HeavyHeatSmoke"
{
    Properties
    {
        _MainTex("烟雾贴图", 2D) = "white" {}
        _Color("烟雾颜色", Color) = (0.95,0.94,0.90,0.5)
        _Density("烟雾浓度", Range(0.1, 1.0)) = 0.6
    }
        SubShader
        {
            Tags
            {
                "Queue" = "Transparent"
                "RenderType" = "Transparent"
                "IgnoreProjector" = "True"
            }

            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            Lighting Off

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0; // 修复：正确语义
                    float4 color : COLOR;
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    float2 uv : TEXCOORD0; // 修复：正确语义
                    float4 color : COLOR;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _Color;
                float _Density;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    o.color = v.color * _Color;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.uv);
                // 强化烟雾浓度，拒绝稀薄透明
                col.a *= _Density;
                col *= i.color;

                // 软边缘处理，保留体积感
                float edge = 1.0 - length(i.uv - 0.5) * 1.2;
                col.a *= saturate(edge);

                return col;
            }
            ENDCG
        }
        }
            FallBack "Transparent/VertexLit"
}