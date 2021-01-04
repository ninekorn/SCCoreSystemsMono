//float4x4 WVP;
float4x4 World;
float4x4 View;
float4x4 Projection;

texture cubeTexture;

sampler TextureSampler = sampler_state
{
    texture = <cubeTexture>;
    mipfilter = LINEAR;
    minfilter = LINEAR;
    magfilter = LINEAR;
};

struct InstancingVSinput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 TexCoord : TEXCOORD0;
};

struct InstancingVSoutput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 TexCoord : TEXCOORD0;
};

//POSITION1 //NORMAL //BLENDWEIGHT//float4 instanceDirForward : POSITION2, float4 instanceDirRight : POSITION3, float4 instanceDirUp  : POSITION4 // float4x4 instanceDird : POSITION2,
InstancingVSoutput InstancingVS(InstancingVSinput input, float4x4 instanceTransform : POSITION1, float2 atlasCoord : TEXCOORD1)
{
    InstancingVSoutput output;

    float4 mod_input_vertex_pos = input.Position;

    instanceTransform._41 *= 2;
    instanceTransform._42 *= 2;
    instanceTransform._43 *= 2;

    input.Position.x += instanceTransform._41;
    input.Position.y += instanceTransform._42;
    input.Position.z += instanceTransform._43;

    float4 worldPos = mul(input.Position, transpose(instanceTransform));
    float4 worldPosition = mul(worldPos, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    output.TexCoord = float2((input.TexCoord.x / 2.0f) + (1.0f / 2.0f * atlasCoord.x), (input.TexCoord.y / 2.0f) + (1.0f / 2.0f * atlasCoord.y));
    output.Normal = input.Normal;
    return output;
}

float4 InstancingPS(InstancingVSoutput input) : COLOR0
{
    float4 color = tex2D(TextureSampler, input.TexCoord);
    //float x = dot(input.Normal, float3(1, 0.25, 0.4));
    //x = x * 0.5 - 0.5;
    //float4 colorFinal = float4(lerp(float3(color.x * 0.45, color.y * 0.45, color.z * 0.45), float3(color.x * 0.55, color.y * 0.55, color.z * 0.55), x),1);
    return color;
}

technique Instancing
{
    pass Pass0
    {
        VertexShader = compile vs_4_0_level_9_1 InstancingVS();
        PixelShader = compile ps_4_0_level_9_1 InstancingPS();
    }
}