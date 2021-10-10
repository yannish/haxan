Shader "UI/colorOutline"
{
	Properties
	{
		_InColor ("Highlight Color", Color) = (1,1,1,1)
		_OutColor ("Outline Color", Color) = (1,1,1,1)
		_OutlineSize ("Outline Size", Float) = 1
		_StencilRef ("Stencil Reference", Int) = 0
	}

	SubShader
	{
		Tags 
		{ 
			"Queue"="Transparent" 
		}
		
		Stencil
		{
			Ref [_StencilRef]
			ReadMask [_StencilRef]
			WriteMask [_StencilRef]

			Comp NotEqual
			Pass Replace
			Fail Keep
			ZFail Keep
		}

		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off

		CGINCLUDE
		#pragma vertex vert
		#pragma fragment frag
			
		float _OutlineSize;
		float4 _InColor;
		float4 _OutColor;
		ENDCG
		
		Pass
		{
			CGPROGRAM
			float4 vert (float4 vertex : POSITION) : SV_POSITION
			{
				return UnityObjectToClipPos(vertex);
			}
			
			fixed4 frag () : SV_Target
			{
				return _InColor;
			}
			ENDCG
		}
		Pass
		{
			CGPROGRAM
			float4 vert (float4 vertex : POSITION) : SV_POSITION
			{
				float4 pos = UnityObjectToClipPos(vertex);
				pos.xy += pos.w / _ScreenParams.xy * float2(-2,0) * _OutlineSize;
				return pos;
			}
			
			fixed4 frag () : SV_Target
			{
				return _OutColor;
			}
			ENDCG
		}
		Pass
		{
			CGPROGRAM
			float4 vert (float4 vertex : POSITION) : SV_POSITION
			{
				float4 pos = UnityObjectToClipPos(vertex);
				pos.xy += pos.w / _ScreenParams.xy * float2(2,0) * _OutlineSize;
				return pos;
			}
			
			fixed4 frag () : SV_Target
			{
				return _OutColor;
			}
			ENDCG
		}
		Pass
		{
			CGPROGRAM
			float4 vert (float4 vertex : POSITION) : SV_POSITION
			{
				float4 pos = UnityObjectToClipPos(vertex);
				pos.xy += pos.w / _ScreenParams.xy * float2(0,-2) * _OutlineSize;
				return pos;
			}
			
			fixed4 frag () : SV_Target
			{
				return _OutColor;
			}
			ENDCG
		}
		Pass
		{
			CGPROGRAM
			float4 vert (float4 vertex : POSITION) : SV_POSITION
			{
				float4 pos = UnityObjectToClipPos(vertex);
				pos.xy += pos.w / _ScreenParams.xy * float2(0,2) * _OutlineSize;
				return pos;
			}
			
			fixed4 frag () : SV_Target
			{
				return _OutColor;
			}
			ENDCG
		}

		/*Pass
		{
			CGPROGRAM
			float4 vert(float4 vertex : POSITION) : SV_POSITION
			{
				float4 pos = UnityObjectToClipPos(vertex);
				pos.xy += pos.w / _ScreenParams.xy * float2(1,1) * _OutlineSize;
				return pos;
			}

			fixed4 frag() : SV_Target
			{
				return _OutColor;
			}
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			float4 vert(float4 vertex : POSITION) : SV_POSITION
			{
				float4 pos = UnityObjectToClipPos(vertex);
				pos.xy += pos.w / _ScreenParams.xy * float2(-1,1) * _OutlineSize;
				return pos;
			}

			fixed4 frag() : SV_Target
			{
				return _OutColor;
			}
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			float4 vert(float4 vertex : POSITION) : SV_POSITION
			{
				float4 pos = UnityObjectToClipPos(vertex);
				pos.xy += pos.w / _ScreenParams.xy * float2(-1,-1) * _OutlineSize;
				return pos;
			}

			fixed4 frag() : SV_Target
			{
				return _OutColor;
			}
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			float4 vert(float4 vertex : POSITION) : SV_POSITION
			{
				float4 pos = UnityObjectToClipPos(vertex);
				pos.xy += pos.w / _ScreenParams.xy * float2(1,-1) * _OutlineSize;
				return pos;
			}

			fixed4 frag() : SV_Target
			{
				return _OutColor;
			}
			ENDCG
		}*/
	}
}
