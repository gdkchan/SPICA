namespace SPICA.Renderer.Shaders
{
    static class VertexShader
    {
        public const string Code = @"
#version 330 core
#extension GL_ARB_explicit_uniform_location: enable

precision highp float;

#define POS     0
#define NORM    1
#define TAN     2
#define COL     3
#define TEX0    0
#define TEX1    1
#define TEX2    2
#define WEIGHT  3

uniform mat4 ProjMatrix;
uniform mat4 ViewMatrix;
uniform mat4 ModelMatrix;

uniform vec4 PosOffset;
uniform vec4 Scales0;
uniform vec4 Scales1;

uniform mat4 Transforms[32];

uniform int SmoothSkin;

uniform vec4 FixedColor;
uniform vec4 FixedBone;
uniform vec4 FixedWeight;

#ifdef GL_ARB_explicit_uniform_location
    layout(location = 0) in vec3 a0_pos;
    layout(location = 1) in vec3 a1_norm;
    layout(location = 2) in vec3 a2_tan;
    layout(location = 3) in vec4 a3_col;
    layout(location = 4) in vec2 a4_tex0;
    layout(location = 5) in vec2 a5_tex1;
    layout(location = 6) in vec2 a6_tex2;
    layout(location = 7) in vec4 a7_bone;
    layout(location = 8) in vec4 a8_weight;
#else
    in vec3 a0_pos;
    in vec3 a1_norm;
    in vec3 a2_tan;
    in vec4 a3_col;
    in vec2 a4_tex0;
    in vec2 a5_tex1;
    in vec2 a6_tex2;
    in vec4 a7_bone;
    in vec4 a8_weight;
#endif

out mat3 ModelMtx;
out vec3 EyeDir;
out vec3 WorldPos;

out vec3 Normal;
out vec3 Tangent;
out vec4 Color;
out vec2 TexCoord0;
out vec2 TexCoord1;
out vec2 TexCoord2;

void main() {
    /*
     * Note: The order in which variables in accessed in important on (some) GPUs, so don't change it!
     * In particular, Intel drivers seems to order attributes in the order they're accessed on the code
     * This is hacky, but GPUs that supports the explicit location doesn't have this problem (yay!)
     */
    vec4 Position = PosOffset + vec4(a0_pos * Scales0[POS], 1);
    
    Normal = normalize(mat3(ModelMatrix) * (a1_norm * Scales0[NORM]));
    Tangent = normalize(mat3(ModelMatrix) * (a2_tan * Scales0[TAN]));
    Color = FixedColor.w != -1 ? FixedColor : a3_col * Scales0[COL];
    TexCoord0 = vec2(a4_tex0.x, -a4_tex0.y) * Scales1[TEX0];
    TexCoord1 = vec2(a5_tex1.x, -a5_tex1.y) * Scales1[TEX1];
    TexCoord2 = vec2(a6_tex2.x, -a6_tex2.y) * Scales1[TEX2];
    
    //Apply bone transform
    int b0, b1, b2, b3;
    
    if (FixedBone.w != 0) {
        b0 = int(FixedBone[0]);
        b1 = int(FixedBone[1]);
        b2 = int(FixedBone[2]);
        b3 = 0;
    } else {
        b0 = int(a7_bone[0]);
        b1 = int(a7_bone[1]);
        b2 = int(a7_bone[2]);
        b3 = int(a7_bone[3]);
    }
    
    if (SmoothSkin != 0) {
        float w0, w1, w2, w3;
        
        if (FixedWeight.w != 0) {
            w0 = FixedWeight[0];
            w1 = FixedWeight[1];
            w2 = FixedWeight[2];
            w3 = 0;
        } else {
            w0 = a8_weight[0] * Scales1[WEIGHT];
            w1 = a8_weight[1] * Scales1[WEIGHT];
            w2 = a8_weight[2] * Scales1[WEIGHT];
            w3 = a8_weight[3] * Scales1[WEIGHT];
        }
        
        vec4 p;
        
        p  = (Transforms[b0] * Position) * w0;
        p += (Transforms[b1] * Position) * w1;
        p += (Transforms[b2] * Position) * w2;
        p += (Transforms[b3] * Position) * w3;
        
        float Sum = w0 + w1 + w2 + w3;
        Position = (Position * (1 - Sum)) + p;
    } else {
        Position *= Transforms[b0];
    }
    
    Position.w = 1;
    
    ModelMtx = mat3(ModelMatrix);
    EyeDir = normalize(vec3(ViewMatrix * ModelMatrix * Position));
    WorldPos = vec3(ModelMatrix * Position);
    
    gl_Position = ProjMatrix * ViewMatrix * ModelMatrix * Position;
}
";
    }
}
