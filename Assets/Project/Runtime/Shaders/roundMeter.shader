Shader "Unlit/roundMeter"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)

        _OuterRadius("Radius", Range(0, 1)) = 1
        _Thickness("Thickness", Range(0, 1)) = 1
        _Angle("Angle",  Range(0, 1)) = 0
        _Rotation("Rotation",  Range(0, 1)) = 0
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

            #define PI 3.1415926538

            #include "UnityCG.cginc"
            #include "Rotate.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };


            float4 _Color;
            float _OuterRadius;
            float _Thickness;
            float _Angle;
            float _Rotation;
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
                // sample the texture
                fixed4 col = _Color;

                float2 rotatedUv = rotateUV(i.uv,(_Rotation * _Direction + 0.5) * PI * 2);

                float2 remapped = 2 * rotatedUv - float2(1, 1);
                float distSqr = remapped.x * remapped.x + remapped.y * remapped.y;
                
                clip((1 - distSqr));
                //clip((1 - distSqr) - _OuterRadius);

                clip(distSqr - _Thickness);

                float arctan = degrees(atan2(remapped.x , remapped.y) * _Direction);
                //arctan = (arctan + 90) * 2;

                clip(arctan - ((1 - _Angle) * 360 - 180));

                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);

                return col;
            }
            ENDCG
        }
    }
}
