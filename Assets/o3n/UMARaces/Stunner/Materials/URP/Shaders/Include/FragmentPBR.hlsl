void FragmentPBR_float(bool isStandard, float3 positionWS, half3 normalWS, half3 viewDirectionWS, float4 shadowCoord,
   half3 bakedGI, half3 albedo, half metallic, half3 specular, half smoothness, half occlusion, half alpha,
   half3 vertexLighting, out half4 RGBA)
{
   if (isStandard)
   {
   #if SHADERGRAPH_PREVIEW
      half3 color = half3(1,1,1);
   #else
      BRDFData brdfData;
      InitializeBRDFData(albedo, metallic, specular, smoothness, alpha, brdfData);
       
      Light mainLight = GetMainLight(shadowCoord);
      MixRealtimeAndBakedGI(mainLight, normalWS, bakedGI, half4(0, 0, 0, 0));

      half3 color = GlobalIllumination(brdfData, bakedGI, occlusion, normalWS, viewDirectionWS);
      color += LightingPhysicallyBased(brdfData, mainLight, normalWS, viewDirectionWS);

   #ifdef _ADDITIONAL_LIGHTS
      uint pixelLightCount = GetAdditionalLightsCount();
      for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
      {
         Light light = GetAdditionalLight(lightIndex, positionWS);
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
