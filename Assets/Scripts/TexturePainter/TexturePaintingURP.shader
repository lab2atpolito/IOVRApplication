Shader "Unlit/TexturePaintingURP"
{
    Properties
    {
        _BaseMap("Base Map", 2D) = "white"
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalRenderPipeline" }
        LOD 100
        ZTest  Off
		ZWrite Off
		Cull   Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct VertexInput
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct FragmentInput
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float2 uv : TEXCOORD1;
                
            };

            float4    _BrushPosition;
			float4x4  _Object2World;
			float4	  _BrushColor;
			float	  _BrushOpacity;
			float	  _BrushHardness;
			float	  _BrushSize;

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
            CBUFFER_END

            FragmentInput vert (VertexInput v)
            {
                FragmentInput o;
                float2 uv = TRANSFORM_TEX(v.uv, _BaseMap);
				float2 uvRemapped   = uv;
				       uvRemapped.y = 1. - uvRemapped.y;
					   uvRemapped   = uvRemapped * 2. - 1.;

					   o.vertex     = float4(uvRemapped.xy, 0., 1.);
				       o.worldPos   = mul(_Object2World, v.vertex);
				       o.uv         = TRANSFORM_TEX(v.uv, _BaseMap);
				return o;
            }

            half4 frag (FragmentInput i) : SV_Target
            {
                half4 col  = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);
				float  size = _BrushSize;
				float  soft = _BrushHardness;
				float  f	= distance(_BrushPosition.xyz, i.worldPos);
					   f    = 1.-smoothstep(size*soft, size, f);
				
					   col  = lerp(col, _BrushColor, f * _BrushPosition.w * _BrushOpacity);
					   col  = saturate(col);

				return col;
            }
            ENDHLSL
        }
    }
}
