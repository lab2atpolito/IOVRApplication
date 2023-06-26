void ShadowCoord_float(float3 worldPos, out half4 shadowCoord)
{
   #if SHADERGRAPH_PREVIEW
   shadowCoord = half4(0,0,0,0);
   #else
   #if SHADOWS_SCREEN
   half4 clipPos = TransformWorldToHClip(worldPos);
   shadowCoord = ComputeScreenPos(clipPos);
   #else
   shadowCoord = TransformWorldToShadowCoord(worldPos);
   #endif
   #endif
}