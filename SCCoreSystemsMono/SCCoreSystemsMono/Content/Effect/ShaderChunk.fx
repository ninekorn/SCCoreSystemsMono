//float4x4 World;
//float4x4 View;
//float4x4 Projection;

float4x4 WVP;

texture cubeTexture;

sampler TextureSampler = sampler_state
{
	texture = <cubeTexture>;
	mipfilter = LINEAR;
	minfilter = LINEAR;
	magfilter = LINEAR;
};


struct VertexShaderColorPosition
{
	//float4 position :POSITION;
	//float4 color	:COLOR;
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float3 Normal : NORMAL0;

};

struct PixelShaderColorPosition
{
	//float4 position :SV_POSITION;
	//float4 color	:COLOR;
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float3 Normal : NORMAL0;

};

PixelShaderColorPosition shaderVS(VertexShaderColorPosition input)//VertexShaderColorPosition input) //, float4 ipos : POSITION0,float4 Normal : NORMAL0, float2 atlasCoord : TEXCOORD0
{
	PixelShaderColorPosition output = (PixelShaderColorPosition)0;

	output.Position = mul(input.Position, WVP);
	//output.Position = mul(input.Position, World);
	//output.Position = mul(output.Position, View);
	//output.Position = mul(output.Position, Projection);

	output.TexCoord = input.TexCoord;
	output.Normal = input.Normal;


	return output;
}

float4 shaderPS(PixelShaderColorPosition input) :SV_Target
{
	float4 color = tex2D(TextureSampler, input.TexCoord);
	return color;
}

technique NormalTech
{
	pass Pass0
	{
		VertexShader = compile vs_4_0_level_9_1 shaderVS();
		PixelShader = compile ps_4_0_level_9_1 shaderPS();
	}
}