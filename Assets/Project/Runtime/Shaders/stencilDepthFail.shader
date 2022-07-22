Shader "Unlit/stencilDepthFail"
{
    Properties
    {
        /*_MainTex ("Texture", 2D) = "white" {}*/
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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                return (1,1,1,1);
            }

            ENDCG
        }
    }
}
