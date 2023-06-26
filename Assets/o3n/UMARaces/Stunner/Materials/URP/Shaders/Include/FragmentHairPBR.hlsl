struct KajiyaKayData
{
   float3 normalDir;
   float3 binormalDir;
   float3 primaryHighlightColor;
   float3 secondaryHighlightColor;
};

#if SHADERGRAPH_PREVIEW
#else

float3 permute(float3 x) {
   return fmod(x*x*34.0 + x, 289);
}

// Snoise function source: https://gist.github.com/fadookie/25adf86ae7e2753d717c
float snoise(float2 v)
{
   const float4 C = float4(0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439);
   float2 i = floor( v + dot(v, C.yy) );
   float2 x0 = v - i + dot(i, C.xx);
   int2 i1 = (x0.x > x0.y) ? float2(1.0, 0.0) : float2(0.0, 1.0);
   float4 x12 = x0.xyxy + C.xxzz;
   x12.xy -= i1;
   i = fmod(i, 289);
   float3 p = permute(permute(i.y + float3(0.0, i1.y, 1.0 )) + i.x + float3(0.0, i1.x, 1.0 ));
   float3 m = max(0.5 - float3(dot(x0, x0), dot(x12.xy, x12.xy), dot(x12.zw, x12.zw)), 0.0);
   m = m*m ;
   m = m*m ;
   float3 x = 2.0 * frac(p * C.www) - 1.0;
   float3 h = abs(x) - 0.5;
   float3 ox = floor(x + 0.5);
   float3 a0 = x - ox;
   m *= 1.79284291400159 - 0.85373472095314 * ( a0*a0 + h*h );
   float3 g;
   g.x = a0.x * x0.x + h.x * x0.y;
   g.yz = a0.yz * x12.xz + h.yz * x12.yw;
   return 130.0 * dot(m, g);
}

// square root is not applied for calculating sinTH since we pow() it later with exponent. Same effect can be achived with 2x exponent value.
float kajiyaKay(float3 binormalDir, float3 normalDir, float3 h, float exponent, float shift, float offset)
{
   float3 tangent = normalize(binormalDir +  normalDir * shift);
   tangent = normalize(float3(tangent.x, tangent.y + offset, tangent.z));
   float dotTH = dot(tangent , h);
   float sinTH = 1.0 - (dotTH * dotTH);
   float dirAtten = smoothstep(-1.0, 0.0, dotTH);
   return dirAtten * pow(sinTH , 2 * exponent);
}
            
float3 calculateHighlightComponent(float3 color, float kajiyaKayCompoenent, float lightToWorldNormalFactor, float3 lightColor, float3 lightAlpha) 
{
   return color * kajiyaKayCompoenent * lightToWorldNormalFactor * lightColor.rgb * lightAlpha;
}

float3 getNormalDir(float3 normal, float3 tangent, float3 binormal, float3 worldNormal)
{
   return normalize(normal.x * tangent + normal.y * binormal + normal.z * worldNormal);
}


KajiyaKayData getKajiyaKeyData(float3 baseColor, float highlightWhiteness, half3 normalWS, float3 tangentWS, float3 normal)
{
   KajiyaKayData kajiyaKayData;
   float3 whiteColor = float3(1,1,1);
   kajiyaKayData.primaryHighlightColor = lerp(baseColor, whiteColor, highlightWhiteness);
   kajiyaKayData.secondaryHighlightColor = lerp(baseColor, whiteColor, highlightWhiteness * 0.3);

   float3 binormalWS = normalize (cross(normalWS, tangentWS));
   
   kajiyaKayData.normalDir = getNormalDir(normal, tangentWS, binormalWS, normalWS);
   kajiyaKayData.binormalDir = normalize(cross(tangentWS, kajiyaKayData.normalDir));
   return kajiyaKayData;
}

float3 getHairHighlightComponent(KajiyaKayData kajiyaKeyData, half3 normalWS, half3 viewDirectionWS, half3 lightDirection,
   float primaryHighlightExponent, float secondaryHighlightExponent, float highlightBias,
   float secondaryHighlightOffset, float noise, float hairHighlightStrength, float3 lightColor, float lightAlpha)
{
   float3 h = normalize(viewDirectionWS + lightDirection);
   float primaryKajiyaKayComponent = kajiyaKay(kajiyaKeyData.binormalDir, kajiyaKeyData.normalDir, h,
      primaryHighlightExponent, highlightBias, noise);
   float secondaryKajiyaKayComponent = kajiyaKay(kajiyaKeyData.binormalDir, kajiyaKeyData.normalDir, h,
      secondaryHighlightExponent, highlightBias, secondaryHighlightOffset + noise);
   
   float cosLightToWorldNormal = clamp(dot(lightDirection, normalWS), 0.0 , 1.0 );
                
   float3 primaryHighlightComponent = calculateHighlightComponent(
      kajiyaKeyData.primaryHighlightColor, primaryKajiyaKayComponent, cosLightToWorldNormal, lightColor, lightAlpha);
   float3 secondaryHighlightComponent = calculateHighlightComponent(
      kajiyaKeyData.secondaryHighlightColor, secondaryKajiyaKayComponent, cosLightToWorldNormal, lightColor, lightAlpha);
                
   return hairHighlightStrength * (primaryHighlightComponent + secondaryHighlightComponent);
}

half3 LightingHairPhysicallyBased(BRDFData brdfData, half3 lightColor, half3 lightDirectionWS,
   half lightAttenuation, half3 normalWS, half3 viewDirectionWS, float3 hairHighlightComponent)
{
   half NdotL = saturate(dot(normalWS, lightDirectionWS));
   half3 radiance = lightColor * (lightAttenuation * NdotL);
   brdfData.diffuse += hairHighlightComponent * lightAttenuation;
   return DirectBDRF(brdfData, normalWS, lightDirectionWS, viewDirectionWS) * radiance;
}

half3 LightingHairPhysicallyBased(BRDFData brdfData, Light light, half3 normalWS, half3 viewDirectionWS,
   float3 hairHighlightComponent)
{
   return LightingHairPhysicallyBased(brdfData, light.color, light.direction,
      light.distanceAttenuation * light.shadowAttenuation, normalWS, viewDirectionWS, hairHighlightComponent);
}

#endif

void FragmentHairPBR_float(float3 positionWS, half3 normalWS, half3 viewDirectionWS, float4 shadowCoord,
   half3 bakedGI, half3 albedo, half metallic, half3 specular, half smoothness, half occlusion, half alpha,
   half3 vertexLighting, float highlightWhiteness, float3 tangentWS, float3 normal,
   float primaryHighlightExponent, float secondaryHighlightExponent, float highlightBias, float secondaryHighlightOffset, float2 uv,
   float noiseStrengthU, float noiseStrengthV, float noiseFactor, float hairHighlightStrength,
   out half4 RGBA)
{
   #if SHADERGRAPH_PREVIEW
      half3 color = half3(1,1,1);
   #else
      BRDFData brdfData;
      InitializeBRDFData(albedo, metallic, specular, smoothness, alpha, brdfData);
       
      KajiyaKayData kajiyaKeyData = getKajiyaKeyData(albedo, highlightWhiteness, normalWS, tangentWS, normal);
      float noise = alpha * noiseFactor * snoise(uv * float2(noiseStrengthU , noiseStrengthV));
      
      Light mainLight = GetMainLight(shadowCoord);
      float3 hairHighlightComponent = getHairHighlightComponent(kajiyaKeyData, normalWS, viewDirectionWS,
         mainLight.direction,primaryHighlightExponent, secondaryHighlightExponent, highlightBias,
         secondaryHighlightOffset, noise, hairHighlightStrength, mainLight.color, 1);
      MixRealtimeAndBakedGI(mainLight, normalWS, bakedGI, half4(0, 0, 0, 0));
      
      half3 color = GlobalIllumination(brdfData, bakedGI, occlusion, normalWS, viewDirectionWS);
      color += LightingHairPhysicallyBased(brdfData, mainLight, normalWS, viewDirectionWS, hairHighlightComponent);

   #ifdef _ADDITIONAL_LIGHTS
      uint pixelLightCount = GetAdditionalLightsCount();
      for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
      {
         Light light = GetAdditionalLight(lightIndex, positionWS);
         hairHighlightComponent = getHairHighlightComponent(kajiyaKeyData, normalWS, viewDirectionWS,
            light.direction,primaryHighlightExponent, secondaryHighlightExponent, highlightBias,
            secondaryHighlightOffset, noise, hairHighlightStrength, light.color, 1);
         color += LightingHairPhysicallyBased(brdfData, light, normalWS, viewDirectionWS, hairHighlightComponent);
      }
   #endif

   #ifdef _ADDITIONAL_LIGHTS_VERTEX
      color += vertexLighting * brdfData.diffuse;
   #endif
   #endif
      RGBA = half4(color, alpha);
}
