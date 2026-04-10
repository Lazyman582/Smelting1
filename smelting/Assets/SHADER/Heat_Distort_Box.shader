Shader "Custom/RealHeatDistort"
{
    Properties
    {
        _NoiseTex("噪声图(黑白噪波)", 2D) = "white" {}
        _Distort("热浪强度", Range(0.001, 0.02)) = 0.006
        _Speed("扰动速度", Range(0.5, 8)) = 4.0
        _Tiling("细节密度", Float) = 12.0

        _HeatTint("高温淡色", Color) = (1.02, 1.01, 0.98, 0.1)
    }

        SubShader
        {
            Tags
            {
                "Queue" = "Transparent+10"
                "RenderType" = "Transparent"
                "IgnoreProjector" = "True"
            }

            GrabPass { "_GrabTexture" }

            Pass
            {
                ZWrite Off
                Blend SrcAlpha OneMinusSrcAlpha
                Cull Off

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
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
                    float4 grabPos : TEXCOORD1;
                };

                sampler2D _GrabTexture;
                float4 _GrabTexture_TexelSize;
                sampler2D _NoiseTex;
                float _Distort;
                float _Speed;
                float _Tiling;
                float4 _HeatTint;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    o.grabPos = ComputeGrabScreenPos(o.pos);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // 双层高频噪声 模拟真实热空气湍流
                    float2 uv1 = i.uv * _Tiling + float2(_Time.y * _Speed * 0.7, _Time.y * _Speed);
                    float2 uv2 = i.uv * (_Tiling * 0.6) + float2(_Time.y * _Speed * 0.4, _Time.y * _Speed * 0.8);

                    float n1 = tex2D(_NoiseTex, uv1).r - 0.5;
                    float n2 = tex2D(_NoiseTex, uv2).g - 0.5;
                    float2 offset = float2(n1, n2) * _Distort;

                    // 屏幕UV采样
                    float2 screenUV = i.grabPos.xy / i.grabPos.w;
                    screenUV += offset;

                    fixed4 col = tex2D(_GrabTexture, screenUV);

                    // 极淡高温暖色 不影响画面 只模拟空气温度
                    col.rgb = lerp(col.rgb, col.rgb * _HeatTint.rgb, _HeatTint.a);
                    return col;
                }
                ENDCG
            }
        }
            FallBack "Hidden/InternalErrorShader"
}