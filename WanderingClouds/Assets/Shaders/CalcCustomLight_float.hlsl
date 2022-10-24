#ifdef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

struct CustomLightData
{
    float3 albedo;
};

float3 CalcCustomLight(CustomLightData d)
{
    
    return d.albedo;
}


float3 CalcCustomLight_float(float3 Albedo, out float3 Color)
{
    CustomLightData d;
    d.albedo = Albedo;

    Color = CalcCustomLight(d);
}

#endif
