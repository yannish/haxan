Shader "Unlit/colorDither"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _DitherColor("DitherColor", Color) = (1,1,1,1)
        _Dither("Dither", Range(-1,1)) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            ColorMask 0

            Stencil
            {
                Ref 1
                Comp Always
                ZFail Replace
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            //float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                return (1,1,1,1);
            }
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            float4 _Color;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = _Color;
                return col;
            }
            ENDCG
        }

        Pass
        {
            ZTest Always

            Stencil
            {
                Ref 1
                Comp Equal
                ZFail Replace
                ///Pass Keep
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 screenPos : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };


            float _Dither;
            float4 _DitherColor;

            static const float4x4 bigDitherTable = float4x4
            (
                -4.0, 0.0, -3.0, 1.0,
                2.0, -2.0, 3.0, -1.0,
                -3.0, 1.0, -4.0, 0.0,
                3.0, -1.0, 2.0, -2.0
            );


            v2f vert(appdata v)
            {
                v2f o;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);

                return o;
            }


            fixed4 frag(v2f i) : SV_Target
            {
                float aspect = _ScreenParams.x / _ScreenParams.y;
                float2 texCoord = i.screenPos.xy / i.screenPos.w;

                uint2 screenPixel = uint2(texCoord.x * _ScreenParams.x, texCoord.y * _ScreenParams.y);

                float xMask = screenPixel.x % 2;
                float yMask = (screenPixel.y) % 2;

                float lookup = bigDitherTable[screenPixel.y % 4][screenPixel.x % 4] * 0.25f;
                clip(lookup - _Dither);

                return _DitherColor;
            }

            ENDCG
        }
    }
}
