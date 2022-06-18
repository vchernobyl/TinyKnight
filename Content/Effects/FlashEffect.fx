sampler s0;
float4 flash_color;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(s0, coords);
    
    if (color.a)
        return flash_color;

    return color;
}

technique Technique1
{
    pass P0
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
};