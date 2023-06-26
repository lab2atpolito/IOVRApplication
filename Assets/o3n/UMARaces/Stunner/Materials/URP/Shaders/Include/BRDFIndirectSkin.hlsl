void BRDFIndirectSkin_float(bool isSkin, Texture2D skinBrdfTexture, out half3 skinColorAdd)
{
#if SHADERGRAPH_PREVIEW
   skinColorAdd = 0;
#else
   half3 normalizedLightDir = normalize(light.dir);
   float dotNL = dot(normal, normalizedLightDir);
   float2 brdfUV = float2(dotNL * 0.5 + 0.5, 0.7 * dot(light.color, fixed3(0.2126, 0.7152, 0.0722)));
   half3 brdf = tex2D( _BRDFTex, brdfUV ).rgb;
   skinColorAdd = float3(0,0,0);
#endif
}