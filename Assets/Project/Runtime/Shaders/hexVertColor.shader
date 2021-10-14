Shader "UI/hexVertColor"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Alpha("Alpha", Range(0.0, 1.0)) = 1
        _Inset("Inset",  Range(0.0, 1.0)) = 1
        _Outset("Outset",  Range(0.0, 1.0)) = 0

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
            float _Outset;
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
                clip(_Inset - i.color.x);
                clip(i.color.x - _Outset);
                fixed4 col = fixed4(_Color.xyz, _Alpha);
                //col.w = _Alpha;
                return col;
            }

            ENDCG
        }
    }
}
