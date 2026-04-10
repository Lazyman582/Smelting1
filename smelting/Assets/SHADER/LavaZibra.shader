Shader "Unlit/LavaZibra"
{
    Properties
    {
        _MainTex("岩浆贴图", 2D) = "white" {}
        _Color("基础色调", Color) = (1,0.2,0.05,1)
        _EmissionPower("发光强度", Float) = 2.0
        _ScrollSpeedX("流动速度 X", Float) = 0.1
        _ScrollSpeedY("流动速度 Y", Float) = 0.05
        _Contrast("对比度", Float) = 1.5
        _Brightness("亮度", Float) = 1.0
    }

        SubShader
        {
            Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }
            LOD 100
            ZWrite On
            Cull Off

            Pass
            {
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
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _Color;
                float _EmissionPower;
                float _ScrollSpeedX;
                float _ScrollSpeedY;
                float _Contrast;
                float _Brightness;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    // UV 滚动，形成流动效果
                    float2 scroll = float2(_Time.x * _ScrollSpeedX, _Time.x * _ScrollSpeedY);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex) + scroll;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.uv);

                // 对比度 + 亮度
                col.rgb = (col.rgb - 0.5) * _Contrast + 0.5;
                col.rgb *= _Brightness;

                // 叠加颜色
                col.rgb *= _Color.rgb;

                // 自发光
                col.rgb *= _EmissionPower;

                col.a = 1; // 强制不透明
                return col;
            }
            ENDCG
        }
        }
            FallBack "Unlit/Color"
}