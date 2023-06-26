float getDistanceSqrToUvCenter(float2 uv)
{
   return pow(0.5 - uv.x, 2) + pow(0.5 - uv.y, 2);
}

float getDistanceToUvCenter(float2 uv)
{
   return sqrt(getDistanceSqrToUvCenter(uv));
}

float getIrisRatio(float distanceSqr, float radiusVisible, float fadeRatio)
{
   float fadeStart = pow(radiusVisible * fadeRatio, 2);
   float fadeRange = pow(radiusVisible, 2) - fadeStart;
   if (distanceSqr < fadeStart) {
      return 1;
   } else {
      float inRange = distanceSqr - fadeStart;
      return saturate(1 - inRange / fadeRange);
   }
}
        
float getMask(float distanceSqr)
{
   return 1 - distanceSqr;
}
        
float getHeight(float distanceSqr, float radiusVisible, float eyeSizeFactor, float anteriorChamberDepth) {
   float r = distanceSqr / radiusVisible * eyeSizeFactor;
   return anteriorChamberDepth * saturate(1.0 - 18.4 * r);
}

float2 physicallyBased(float3 viewDirOS, float3 normalW, float mask, float heightW) 
{
   float4 frontNormal = float4(0,0,1,0);
   float3 refractedW = viewDirOS;
   float cosAlpha = dot(frontNormal, -refractedW);
   float dist = heightW / cosAlpha;
   float3 offsetW = dist * refractedW;
   return float2(mask, mask) * offsetW;
}
        
float2 correctOffset(float2 offset, float2 uv, float radiusVisible)
{
   float offsetLength = getDistanceToUvCenter(uv + offset);
   if (offsetLength < radiusVisible) {
      return offset;
   } else {
      float ratio = radiusVisible / offsetLength; 
      return offset * ratio;
   }
}
        
float2 getRefractionOffset(half3 viewDirectionOS, half3 normalWS, float distanceSqr,
   float radiusVisible, float eyeSizeFactor, float anteriorChamberDepth, float2 uv)
{
   if (distanceSqr > radiusVisible)
   {
      return float2(0,0);
   }
   float mask = getMask(distanceSqr);
   float height = getHeight(distanceSqr, radiusVisible, eyeSizeFactor, anteriorChamberDepth);
   float2 offset = physicallyBased(viewDirectionOS, normalWS, mask, height);
   return correctOffset(offset, uv, radiusVisible);
}

void RefractionEyes_float(Texture2D mainTexture2D, Texture2D irisTexture2D, float3 irisEmissionColor, float3 scaleraEmissionColor,
   half3 normalTS, float3 viewDirectionTS, SamplerState samplerState, float2 uv, float radiusVisible, float fadeRatio,
   float anteriorChamberDepth, float eyeSizeFactor,
   out half4 RGBA, out half3 emmission)
{
   float distanceSqrToCenter = getDistanceSqrToUvCenter(uv);
   float irisRatio = getIrisRatio(distanceSqrToCenter, radiusVisible, fadeRatio);

   float2 offsetUv = getRefractionOffset(viewDirectionTS, normalTS, distanceSqrToCenter, radiusVisible,
      eyeSizeFactor, anteriorChamberDepth, uv) + uv;
   float4 blackColor = float4(0,0,0,1);
   float4 irisColor = lerp(SAMPLE_TEXTURE2D( irisTexture2D, samplerState, offsetUv ), blackColor, 1 - irisRatio);
   float4 eyeColor = SAMPLE_TEXTURE2D( mainTexture2D, samplerState, offsetUv );
   float3 albedo = lerp(eyeColor.rgb, irisColor.rgb, irisRatio);
   float alpha = min(eyeColor.a, irisColor.a);
   RGBA = half4(albedo.rgb, alpha);
   emmission = lerp(eyeColor.rgb * scaleraEmissionColor, irisColor.rgb * irisEmissionColor, irisRatio);
}
