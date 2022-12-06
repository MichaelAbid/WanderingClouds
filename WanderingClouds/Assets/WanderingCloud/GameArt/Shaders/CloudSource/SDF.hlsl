//Reference : https://iquilezles.org/articles/distfunctions/
void LineSDF_float(float3 Pos,float3 A,float3 B, float radius, out float result)
{
    float3 toEntry = Pos - A, AB = B - A;
    float h = clamp( dot(toEntry,AB)/dot(AB,AB), 0.0, 1.0 );
    result = length( toEntry - AB*h ) - radius;
}