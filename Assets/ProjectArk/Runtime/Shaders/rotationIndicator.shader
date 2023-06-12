Shader "Unlit/rotationIndicator"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)

        _Thickness("Thickness", Range(0, 1)) = 1
        _Angle("Angle",  Range(0, 1)) = 0
        _Direction("Direction",  Integer) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_fog

            #include "Rotate.cginc"
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

            float4 _Color;
            float _Thickness;
            float _Angle;
            int _Direction;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = _Color;

                float2 remappedUvs = 2 * i.uv - float2(1, 1);
                float distSqr = remappedUvs.x * remappedUvs.x + remappedUvs.y * remappedUvs.y;

                clip((1 - distSqr));
                clip(distSqr - _Thickness);

                float arctan = degrees(atan2(remappedUvs.x, remappedUvs.y) * _Direction);
                clip(arctan - ((1 - _Angle) * 360 - 180));

                return col;
            }
            ENDCG
        }
    }
}
