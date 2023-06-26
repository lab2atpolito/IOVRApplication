void FragmentSkinPBR_float(bool isSkin, float3 positionWS, half3 normalWS, half3 viewDirectionWS, float4 shadowCoord,
   half3 bakedGI, half3 albedo, half metallic, half3 specular, half smoothness, half occlusion, half alpha,
   Texture2D brdfTexture, SamplerState samplerState, half3 vertexLighting,
   out half4 RGBA)
{
   if (isSkin)
   {
   #if SHADERGRAPH_PREVIEW
      half3 color = half3(1,1,1);
   #else
      BRDFData brdfData;
      InitializeBRDFData(albedo, metallic, specular, smoothness, alpha, brdfData);
       
      Light mainLight = GetMainLight(shadowCoord);
      MixRealtimeAndBakedGI(mainLight, normalWS, bakedGI, half4(0, 0, 0, 0));

      half3 normalizedLightDir = normalize(mainLight.direction);
      float dotNL = dot(normalWS, normalizedLightDir);
      float2 brdfUV = float2(min(max(dotNL * 0.5 + 0.5, .1), .9), 0);
      half3 brdf = SAMPLE_TEXTURE2D( brdfTexture, samplerState, brdfUV ).rgb;
      mainLight.color = mainLight.color * brdf * occlusion * 8;

      half3 color = GlobalIllumination(brdfData, bakedGI, occlusion, normalWS, viewDirectionWS);
      color += LightingPhysicallyBased(brdfData, mainLight, normalWS, viewDirectionWS);

   #ifdef _ADDITIONAL_LIGHTS
      uint pixelLightCount = GetAdditionalLightsCount();
      for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
      {
         Light light = GetAdditionalLight(lightIndex, positionWS);
         normalizedLightDir = normalize(light.direction);
         dotNL = dot(normalWS, normalizedLightDir);
         brdfUV = float2(min(max(dotNL * 0.5 + 0.5, .1), .9), 0);
         brdf = SAMPLE_TEXTURE2D( brdfTexture, samplerState, brdfUV ).rgb;
         light.color = light.color * brdf * occlusion * 8;
         color += LightingPhysicallyBased(brdfData, light, normalWS, viewDirectionWS);
      }
   #endif

   #ifdef _ADDITIONAL_LIGHTS_VERTEX
      color += vertexLighting * brdfData.diffuse;
   #endif
   #endif
      RGBA = half4(color, alpha);
   }
   else
   {
      RGBA =half4(0,0,0,0);
   }
}
