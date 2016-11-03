namespace SPICA.Renderer.Shaders
{
    class FragmentShader
    {
        public const string Code = @"
#version 150

precision highp float;

struct CombArg_t {
    int ColorArg;
    int ColorOp;
    int AlphaArg;
    int AlphaOp;
};

struct Combiner_t {
	int ColorCombine;
	int AlphaCombine;
	float ColorScale;
	float AlphaScale;
    
    CombArg_t Args[3];
    
	vec4 Color;
};

uniform Combiner_t Combiners[6];

uniform int TexUnit2Source;
uniform int TexUnit3Source;

struct Light_t {
	vec4 Position;
	vec4 Ambient;
	vec4 Diffuse;
	vec4 Specular;
};

uniform Light_t Lights[8];

uniform sampler2D Texture0;
uniform sampler2D Texture1;
uniform sampler2D Texture2;

const vec3 ambient = vec3(0.1, 0.1, 0.1);
const vec3 lightVecNormalized = normalize(vec3(0, 100, 50));
const vec3 lightColor = vec3(1, 1, 1);

in vec3 Normal;
in vec3 Tangent;
in vec4 Color;
in vec2 TexCoord0;
in vec2 TexCoord1;
in vec2 TexCoord2;

out vec4 Output;

void main(void) {
  //float diffuse = clamp(dot(lightVecNormalized, normalize(Normal)), 0.0, 1.0);
  //out_frag_color = vec4(ambient + diffuse * lightColor, 1.0) * texture(Texture0, TexCoord0);
    
    vec4 Color0 = texture(Texture0, TexCoord0);
    vec4 Color1 = texture(Texture1, TexCoord0);
    vec4 Color2, Color3;
    
    switch (TexUnit2Source) {
        case 0: Color2 = texture(Texture2, TexCoord1);
        case 1: Color2 = texture(Texture2, TexCoord2);
    }
    
    //FIXME: Form where texture 3 comes?
    switch (TexUnit3Source) {
        case 0: Color3 = texture(Texture2, TexCoord0);
        case 1: Color3 = texture(Texture2, TexCoord1);
        case 2: Color3 = texture(Texture2, TexCoord2);
    }
    
    vec4 FragPrimaryColor = vec4(0, 0, 0, 0);
    vec4 FragSecondaryColor = vec4(0, 0, 0, 0);
    
    vec4 PrevBuffer;
    vec4 Previous;
    
    for (int Stage = 0; Stage < 6; Stage++) {
        vec3 ColorArgs[3];
        float AlphaArgs[3];
        
        for (int Param = 0; Param < 3; Param++) {
            vec4 ColorArg;
            vec4 AlphaArg;
            
            switch (Combiners[Stage].Args[Param].ColorArg) {
                case 0: ColorArg = Color; break;
                case 1: ColorArg = FragPrimaryColor; break;
                case 2: ColorArg = FragSecondaryColor; break;
                case 3: ColorArg = Color0; break;
                case 4: ColorArg = Color1; break;
                case 5: ColorArg = Color2; break;
                case 6: ColorArg = Color3; break;
                case 13: ColorArg = PrevBuffer; break;
                case 14: ColorArg = Combiners[Stage].Color; break;
                case 15: ColorArg = Previous; break;
            }
            
            switch (Combiners[Stage].Args[Param].AlphaArg) {
                case 0: AlphaArg = Color; break;
                case 1: AlphaArg = FragPrimaryColor; break;
                case 2: AlphaArg = FragSecondaryColor; break;
                case 3: AlphaArg = Color0; break;
                case 4: AlphaArg = Color1; break;
                case 5: AlphaArg = Color2; break;
                case 6: AlphaArg = Color3; break;
                case 13: AlphaArg = PrevBuffer; break;
                case 14: AlphaArg = Combiners[Stage].Color; break;
                case 15: AlphaArg = Previous; break;
            }
            
            switch (Combiners[Stage].Args[Param].ColorOp) {
                case 0: ColorArgs[Param] = ColorArg.rgb; break;
                case 2: ColorArgs[Param] = ColorArg.aaa; break;
                case 4: ColorArgs[Param] = ColorArg.rrr; break;
                case 8: ColorArgs[Param] = ColorArg.ggg; break;
                case 12: ColorArgs[Param] = ColorArg.bbb; break;
                
                case 1: ColorArgs[Param] = 1 - ColorArg.rgb; break;
                case 3: ColorArgs[Param] = 1 - ColorArg.aaa; break;
                case 5: ColorArgs[Param] = 1 - ColorArg.rrr; break;
                case 9: ColorArgs[Param] = 1 - ColorArg.ggg; break;
                case 13: ColorArgs[Param] = 1 - ColorArg.bbb; break;
            }
            
            switch (Combiners[Stage].Args[Param].AlphaOp) {
                case 0: AlphaArgs[Param] = AlphaArg.a; break;
                case 2: AlphaArgs[Param] = AlphaArg.r; break;
                case 4: AlphaArgs[Param] = AlphaArg.g; break;
                case 6: AlphaArgs[Param] = AlphaArg.b; break;
                
                case 1: AlphaArgs[Param] = 1 - AlphaArg.a; break;
                case 3: AlphaArgs[Param] = 1 - AlphaArg.r; break;
                case 5: AlphaArgs[Param] = 1 - AlphaArg.g; break;
                case 7: AlphaArgs[Param] = 1 - AlphaArg.b; break;
            }
        }

        switch (Combiners[Stage].ColorCombine) {
            case 0: Output.rgb = ColorArgs[0]; break;
            case 1: Output.rgb = ColorArgs[0] * ColorArgs[1]; break;
            case 2: Output.rgb = clamp(ColorArgs[0] + ColorArgs[1], 0, 1); break;
            case 3: Output.rgb = clamp(ColorArgs[0] + ColorArgs[1] - 0.5, 0, 1); break;
            case 4: Output.rgb = mix(ColorArgs[0], ColorArgs[1], ColorArgs[2]); break;
            case 5: Output.rgb = clamp(ColorArgs[0] - ColorArgs[1], 0, 1); break;
            case 6: Output.rgb = vec3(0, 0, 0) + clamp(dot(ColorArgs[0], ColorArgs[1]), 0, 1); break;
            case 7: Output.rgb = vec3(0, 0, 0) + clamp(dot(ColorArgs[0], ColorArgs[1]), 0, 1); break;
            case 8: Output.rgb = clamp(ColorArgs[0] * ColorArgs[1] + ColorArgs[2], 0, 1); break;
            case 9: Output.rgb = clamp((ColorArgs[0] + ColorArgs[1]) * ColorArgs[2], 0, 1); break;
        }
        
        switch (Combiners[Stage].AlphaCombine) {
            case 0: Output.a = AlphaArgs[0]; break;
            case 1: Output.a = AlphaArgs[0] * AlphaArgs[1]; break;
            case 2: Output.a = clamp(AlphaArgs[0] + AlphaArgs[1], 0, 1); break;
            case 3: Output.a = clamp(AlphaArgs[0] + AlphaArgs[1] - 0.5, 0, 1); break;
            case 4: Output.a = mix(AlphaArgs[0], AlphaArgs[1], AlphaArgs[2]); break;
            case 5: Output.a = clamp(AlphaArgs[0] - AlphaArgs[1], 0, 1); break;
            case 6: Output.a = clamp(dot(AlphaArgs[0], AlphaArgs[1]), 0, 1); break;
            case 7: Output.a = clamp(dot(AlphaArgs[0], AlphaArgs[1]), 0, 1); break;
            case 8: Output.a = clamp(AlphaArgs[0] * AlphaArgs[1] + AlphaArgs[2], 0, 1); break;
            case 9: Output.a = clamp((AlphaArgs[0] + AlphaArgs[1]) * AlphaArgs[2], 0, 1); break;
        }
        
        Output.rgb *= Combiners[Stage].ColorScale;
        Output.a *= Combiners[Stage].AlphaScale;
        
        PrevBuffer = Previous;
        Previous = Output;
    }

    if (Combiners[0].Args[0].ColorArg == 0) Output = vec4(1, 0, 0, 1);

    //Output = Color0;
}
";
    }
}
