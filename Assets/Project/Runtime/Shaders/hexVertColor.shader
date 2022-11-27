Shader "UI/hexVertColor"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Alpha("Alpha", Range(0.0, 1.0)) = 1
        //_Inset("Inset",  Range(0.0, 1.0)) = 1
        _Size("Size",  Range(0.0, 1.0)) = 0
        _Thickness("Thickness",  Range(0.0, 1.0)) = 0.5

        _Dither("Dither", Range(-1,1)) = 1

       /* _MyColor("Some Color", Color) = (1,1,1,1)
        _MyVector("Some Vector", Vector) = (0,0,0,0)
        _MyFloat("My float", Range(0,1)) = 0.5
        _MyTexture("Texture", 2D) = "white" {}
        _MyCubemap("Cubemap", CUBE) = "" {}*/
        //_Inset("Inset",  Float) = 1;
        //_MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
		Blend SrcAlpha OneMinusSrcAlpha

        Tags { "Queue" = "Transparent" }
        //Tags { "RenderType"="Opaque" }
        LOD 100


        Pass
        {
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
                //float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            float _Inset;
            float _Size;
            float _Thickness;
            float _Alpha;
            fixed4 _Color;

            struct v2f 
            {
                float4 pos : SV_POSITION;
                fixed4 color : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                //o.color.xyz = v.normal * 0.5 + 0.5;
                //o.color.w = 1.0;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target 
            { 
                //clip(_Inset - i.color.x);

                clip(i.color.x - (1.0 - _Size));
                clip(1.0 - ( _Size - _Thickness) - i.color.x);
                //clip(-(_Size - _Thickness) + i.color.x);
                //clip(-(_Inset - _Thickness) + i.color.x);
                fixed4 col = fixed4(_Color.rgb, _Color.a);
                //fixed4 col = fixed4(_Color.xyz, _Alpha);
                //col.w = _Alpha;
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
                Pass Keep
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
            float4 _Color;

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

                return _Color;
            }

            ENDCG
        }
    }
}
