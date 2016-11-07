namespace SPICA.Renderer.Shaders
{
    static class FragmentShader
    {
        public const string Code = @"
#version 150

precision highp float;

#define FLAG_BUMP_RENORM   1 << 0
#define FLAG_CLAMP_HLIGHT  1 << 1
#define FLAG_D0_ENB        1 << 2
#define FLAG_D1_ENB        1 << 3
#define FLAG_R_ENB         1 << 4
#define FLAG_G0_ENB        1 << 5
#define FLAG_G1_ENB        1 << 6

#define LUT_IN_NH  0
#define LUT_IN_VH  1
#define LUT_IN_NV  2
#define LUT_IN_LN  3
#define LUT_IN_LP  4
#define LUT_IN_CP  5

#define LUT_DIST0    0
#define LUT_DIST1    1
#define LUT_FRESNEL  2
#define LUT_REFLECR  3
#define LUT_REFLECG  4
#define LUT_REFLECB  5

#define FLAG_PRI  1
#define FLAG_SEC  2

uniform int AlphaTestEnb;
uniform int AlphaTestFunc;
uniform int AlphaTestRef;

struct CombArg_t {
    int ColorSrc;
    int ColorOp;
    int AlphaSrc;
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

uniform int TexUnit0Source;
uniform int TexUnit1Source;
uniform int TexUnit2Source;

uniform int FragFlags;
uniform int FresnelSel;
uniform int BumpIndex;
uniform int BumpMode;

uniform vec4 MEmission;
uniform vec4 MDiffuse;
uniform vec4 MAmbient;
uniform vec4 MSpecular;

uniform vec4 SAmbient;

struct Light_t {
	vec3 Position;
	vec4 Ambient;
	vec4 Diffuse;
	vec4 Specular;
};

uniform Light_t Lights[8];

uniform int LightCount;

struct LUT_t {
    int IsAbs;
    int Input;
    float Scale;
};

uniform LUT_t LUTs[6];

uniform sampler2D Texture0;
uniform sampler2D Texture1;
uniform sampler2D Texture2;

uniform UBDist0 { vec4 Dist0[64]; };
uniform UBDist1 { vec4 Dist1[64]; };
uniform UBFresnel { vec4 Fresnel[64]; };
uniform UBReflecR { vec4 ReflecR[64]; };
uniform UBReflecG { vec4 ReflecG[64]; };
uniform UBReflecB { vec4 ReflecB[64]; };

float nh;
float vh;
float nv;
float ln;
float lp;
float cp;

in mat3 ModelMtx;
in vec3 EyeDir;
in vec3 WorldPos;

in vec3 Normal;
in vec3 Tangent;
in vec4 Color;
in vec2 TexCoord0;
in vec2 TexCoord1;
in vec2 TexCoord2;

out vec4 Output;

float GetLUTVal(int SrcLUT);
float Dot3(vec4 l, vec4 r);
float Dot4(vec4 l, vec4 r);
vec3 Dot3v(vec4 l, vec4 r);
vec3 Dot4v(vec4 l, vec4 r);

void main() {
    //TODO: CubeMap
    vec3 r = reflect(EyeDir, Normal);
    float rz = r.z + 1;
    float m = 2 * sqrt(r.x * r.x + r.y * r.y + rz * rz);
    vec2 SphereUV = r.xy / m + 0.5;
    SphereUV.y = -SphereUV.y;
    
    vec4 Color0, Color1, Color2, Color3;
    
    switch (TexUnit0Source) {
        case 0: Color0 = texture(Texture0, TexCoord0); break;
        case 1: Color0 = texture(Texture0, TexCoord1); break;
        case 2: Color0 = texture(Texture0, TexCoord2); break;
        case 4: Color0 = texture(Texture0, SphereUV); break;
    }
    
    switch (TexUnit1Source) {
        case 0: Color1 = texture(Texture1, TexCoord0); break;
        case 1: Color1 = texture(Texture1, TexCoord1); break;
        case 2: Color1 = texture(Texture1, TexCoord2); break;
        case 4: Color1 = texture(Texture1, SphereUV); break;
    }
    
    switch (TexUnit2Source) {
        case 0: Color2 = texture(Texture2, TexCoord0); break;
        case 1: Color2 = texture(Texture2, TexCoord1); break;
        case 2: Color2 = texture(Texture2, TexCoord2); break;
        case 4: Color2 = texture(Texture2, SphereUV); break;
    }
    
    vec4 FragPrimaryColor = MEmission + MAmbient * SAmbient;
    vec4 FragSecondaryColor = vec4(0, 0, 0, 1);
    
    vec3 n = Normal;
    
    if (BumpMode != 0) {
        switch (BumpIndex) {
            case 0: n = Color0.xyz * 2 - 1; break;
            case 1: n = Color1.xyz * 2 - 1; break;
            case 2: n = Color2.xyz * 2 - 1; break;
        }
        
        //Tangent Space
        if (BumpMode == 1) {
            vec3 BiTangent = cross(Normal, Tangent);
            n *= mat3(Tangent, BiTangent, Normal);
        } else {
            n *= ModelMtx;
        }
        
        if ((FragFlags & FLAG_BUMP_RENORM) != 0) n = normalize(n);
    }
    
    for (int i = 0; i < LightCount; i++) {
        vec3 li = normalize(Lights[i].Position - WorldPos);
        vec3 hi = normalize(li - EyeDir);
        
        nh = dot(n, hi);
        vh = dot(EyeDir, hi);
        nv = dot(n, EyeDir);
        ln = dot(li, n);
        //TODO: lp
        //TODO: cp
        
        float fi = ln < 0 ? 0 : 1;
        
        float d0 = (FragFlags & FLAG_D0_ENB) != 0 ? GetLUTVal(LUT_DIST0) : 0;
        float d1 = (FragFlags & FLAG_D1_ENB) != 0 ? GetLUTVal(LUT_DIST1) : 0;
        
        float rr = 0, rg = 0, rb = 0;
        
        if ((FragFlags & FLAG_R_ENB) != 0) {
            rr = GetLUTVal(LUT_REFLECR);
            rg = GetLUTVal(LUT_REFLECG);
            rb = GetLUTVal(LUT_REFLECB);
        }
        
        float g0 = 1, g1 = 1;
        
        if ((FragFlags & FLAG_G0_ENB) != 0) g0 = (2 * nh * nv) / vh;
        if ((FragFlags & FLAG_G1_ENB) != 0) g1 = (2 * nh * ln) / vh;
        
        vec4 Ambient = MAmbient * Lights[i].Ambient;
        vec4 Diffuse = MDiffuse * Lights[i].Diffuse;
        vec4 Specular = (MSpecular * d0 * g0 + vec4(rr, rg, rb, 1) * d1 * g1) * Lights[i].Specular;
        
        Diffuse = fi * (Ambient + Diffuse * ln);
        Specular = fi * Specular;
        
        if ((FresnelSel & FLAG_PRI) != 0) Diffuse.a *= GetLUTVal(LUT_FRESNEL);
        if ((FresnelSel & FLAG_SEC) != 0) Specular.a *= GetLUTVal(LUT_FRESNEL);
        
        FragPrimaryColor += Diffuse;
        FragSecondaryColor += Specular;
    }
    
    vec4 PrevBuffer;
    vec4 Previous;
    
    for (int Stage = 0; Stage < 6; Stage++) {
        vec4 ColorArgs[3];
        vec4 AlphaArgs[3];
        
        for (int Param = 0; Param < 3; Param++) {
            vec4 ColorArg;
            vec4 AlphaArg;
            
            switch (Combiners[Stage].Args[Param].ColorSrc) {
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
            
            switch (Combiners[Stage].Args[Param].AlphaSrc) {
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
            
            switch (Combiners[Stage].Args[Param].ColorOp >> 1) {
                case 0: ColorArgs[Param] = ColorArg.rgba; break;
                case 1: ColorArgs[Param] = ColorArg.aaaa; break;
                case 2: ColorArgs[Param] = ColorArg.rrrr; break;
                case 4: ColorArgs[Param] = ColorArg.gggg; break;
                case 6: ColorArgs[Param] = ColorArg.bbbb; break;
            }
            
            switch (Combiners[Stage].Args[Param].AlphaOp >> 1) {
                //RGBA is not used on the Alpha channel
                case 0: AlphaArgs[Param] = AlphaArg.aaaa; break;
                case 1: AlphaArgs[Param] = AlphaArg.rrrr; break;
                case 2: AlphaArgs[Param] = AlphaArg.gggg; break;
                case 3: AlphaArgs[Param] = AlphaArg.bbbb; break;
            }
            
            if ((Combiners[Stage].Args[Param].ColorOp & 1) != 0) ColorArgs[Param] = 1 - ColorArgs[Param];
            if ((Combiners[Stage].Args[Param].AlphaOp & 1) != 0) AlphaArgs[Param] = 1 - AlphaArgs[Param];
        }
        
        switch (Combiners[Stage].ColorCombine) {
            case 0: Output.rgb = ColorArgs[0].rgb; break;
            case 1: Output.rgb = ColorArgs[0].rgb * ColorArgs[1].rgb; break;
            case 2: Output.rgb = clamp(ColorArgs[0].rgb + ColorArgs[1].rgb, 0, 1); break;
            case 3: Output.rgb = clamp(ColorArgs[0].rgb + ColorArgs[1].rgb - 0.5, 0, 1); break;
            case 4: Output.rgb = mix(ColorArgs[0].rgb, ColorArgs[1].rgb, ColorArgs[2].rgb); break;
            case 5: Output.rgb = clamp(ColorArgs[0].rgb - ColorArgs[1].rgb, 0, 1); break;
            case 6: Output.rgb = clamp(Dot3v(ColorArgs[0], ColorArgs[1]), 0, 1); break;
            case 7: Output.rgb = clamp(Dot4v(ColorArgs[0], ColorArgs[1]), 0, 1); break;
            case 8: Output.rgb = clamp(ColorArgs[0].rgb * ColorArgs[1].rgb + ColorArgs[2].rgb, 0, 1); break;
            case 9: Output.rgb = clamp((ColorArgs[0].rgb + ColorArgs[1].rgb) * ColorArgs[2].rgb, 0, 1); break;
        }
        
        switch (Combiners[Stage].AlphaCombine) {
            case 0: Output.a = AlphaArgs[0].a; break;
            case 1: Output.a = AlphaArgs[0].a * AlphaArgs[1].a; break;
            case 2: Output.a = clamp(AlphaArgs[0].a + AlphaArgs[1].a, 0, 1); break;
            case 3: Output.a = clamp(AlphaArgs[0].a + AlphaArgs[1].a - 0.5, 0, 1); break;
            case 4: Output.a = mix(AlphaArgs[0].a, AlphaArgs[1].a, AlphaArgs[2].a); break;
            case 5: Output.a = clamp(AlphaArgs[0].a - AlphaArgs[1].a, 0, 1); break;
            case 6: Output.a = clamp(Dot3(AlphaArgs[0], AlphaArgs[1]), 0, 1); break;
            case 7: Output.a = clamp(Dot4(AlphaArgs[0], AlphaArgs[1]), 0, 1); break;
            case 8: Output.a = clamp(AlphaArgs[0].a * AlphaArgs[1].a + AlphaArgs[2].a, 0, 1); break;
            case 9: Output.a = clamp((AlphaArgs[0].a + AlphaArgs[1].a) * AlphaArgs[2].a, 0, 1); break;
        }
        
        Output.rgb *= Combiners[Stage].ColorScale;
        Output.a *= Combiners[Stage].AlphaScale;
        
        PrevBuffer = Previous;
        Previous = Output;
    }
    
    if (AlphaTestEnb != 0) {
        bool Pass = true;
        float Ref = float(AlphaTestRef) / 255;
        
        switch (AlphaTestFunc) {
            case 0: Pass = false; break;
            case 1: Pass = true; break;
            case 2: Pass = Output.a == Ref; break;
            case 3: Pass = Output.a != Ref; break;
            case 4: Pass = Output.a < Ref; break;
            case 5: Pass = Output.a <= Ref; break;
            case 6: Pass = Output.a > Ref; break;
            case 7: Pass = Output.a >= Ref; break;
        }
        
        if (!Pass) discard;
    }
}

float GetLUTVal(int SrcLUT) {
    float IndexVal;
    
    switch (LUTs[SrcLUT].Input) {
        case LUT_IN_NH: IndexVal = nh; break;
        case LUT_IN_VH: IndexVal = vh; break;
        case LUT_IN_NV: IndexVal = nv; break;
        case LUT_IN_LN: IndexVal = ln; break;
        case LUT_IN_LP: IndexVal = lp; break;
        case LUT_IN_CP: IndexVal = cp; break;
    }
    
    if (LUTs[SrcLUT].IsAbs == 0) {
        IndexVal = clamp(IndexVal, -1, 1);
        IndexVal = (IndexVal < 0 ? IndexVal + 2 : IndexVal) * 0.5;
    } else {
        IndexVal = clamp(IndexVal, 0, 1);
    }
    
    int Index = int(IndexVal * 0xff);
    int i = Index >> 2;
    int j = Index & 3;
    
    float Value;
    
    switch (SrcLUT) {
        case LUT_DIST0: Value = Dist0[i][j]; break;
        case LUT_DIST1: Value = Dist1[i][j]; break;
        case LUT_FRESNEL: Value = Fresnel[i][j]; break;
        case LUT_REFLECR: Value = ReflecR[i][j]; break;
        case LUT_REFLECG: Value = ReflecG[i][j]; break;
        case LUT_REFLECB: Value = ReflecB[i][j]; break;
    }
    
    return Value * LUTs[SrcLUT].Scale;
}

float Dot3(vec4 l, vec4 r) {
    return l.r * r.r + l.g * r.g + l.b * r.b;
}

float Dot4(vec4 l, vec4 r) {
    return l.r * r.r + l.g * r.g + l.b * r.b + l.a * r.a;
}

vec3 Dot3v(vec4 l, vec4 r) {
    return vec3(1) * Dot3(l, r);
}

vec3 Dot4v(vec4 l, vec4 r) {
    return vec3(1) * Dot4(l, r);
}
";
    }
}
