Shader "Unlit/TexturePainting"
{
	/*Properties
	{
		_MainTex("Main Texture", 2D) = "white"
    }*/
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		//LOD	   100
		ZTest  Off
		ZWrite Off
		Cull   Off

		Pass
		{
			CGPROGRAM
			#pragma vertex   vertexFunction
			#pragma fragment fragmentFunction
			
			#include "UnityCG.cginc"

			struct vertexInput
			{
				float4 vertex   : POSITION;
				float2 uv	    : TEXCOORD0;
			};

			struct fragmentInput
			{
				float4 vertex   : SV_POSITION;
				float2 uv       : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
			};

			float4    _BrushPosition;
			float4x4  mesh_Object2World;
			float4	  _BrushColor;
			float	  _BrushOpacity;
			float	  _BrushHardness;
			float	  _BrushSize;

			sampler2D _BaseMap;

			fragmentInput vertexFunction (vertexInput v)
			{
				fragmentInput o;
				float2 uvRemapped   = v.uv;
				       uvRemapped.y = 1. - uvRemapped.y;
					   uvRemapped   = uvRemapped *2. - 1.;
					   o.vertex     = float4(uvRemapped.xy, 0., 1.);
				       o.worldPos   = mul(mesh_Object2World, v.vertex);
				       o.uv         = v.uv;
				return o;
			}
			
			fixed4 fragmentFunction (fragmentInput i) : SV_Target
			{
				float4 col  = tex2D(_BaseMap, i.uv);
				float  size = _BrushSize;
				float  soft = _BrushHardness;
				float  f	= distance(_BrushPosition.xyz, i.worldPos);
					   f    = 1.-smoothstep(size*soft, size, f);
				
					   col  = lerp(col, _BrushColor, f * _BrushPosition.w * _BrushOpacity);
					   col  = saturate(col);

				return col;
			}
			ENDCG
		}

	}
}
