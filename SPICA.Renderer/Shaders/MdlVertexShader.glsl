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

struct UVTransform_t {
    mat2 Transform;
    vec2 Translation;
};

uniform UVTransform_t UVTransforms[3];

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

out vec3 EyeDir;
out vec3 WorldPos;

out vec3 Normal;
out vec3 Tangent;
out vec4 Color;
out vec2 TexCoord0;
out vec2 TexCoord1;
out vec2 TexCoord2;

vec2 TransformUV(vec2 UV, int Index);

void main() {
    /*
     * Note: The order in which variables in accessed in important on (some) GPUs, so don't change it!
     * In particular, Intel drivers seems to order attributes in the order they're accessed on the code
     * This is hacky, but GPUs that supports the explicit location doesn't have this problem (yay!)
     */
    vec4 Position = PosOffset + vec4(a0_pos * Scales0[POS], 1);
    
    Normal = a1_norm * Scales0[NORM];
    Tangent = a2_tan * Scales0[TAN];
    
    Color = FixedColor.w != -1 ? FixedColor : a3_col * Scales0[COL];
    
    TexCoord0 = TransformUV(a4_tex0 * Scales1[TEX0], 0);
    TexCoord1 = TransformUV(a5_tex1 * Scales1[TEX1], 1);
    TexCoord2 = TransformUV(a6_tex2 * Scales1[TEX2], 2);
    
    //Apply bone transform
    int b0, b1, b2, b3;
    
    if (FixedBone.x != -1) {
        b0 = int(FixedBone[0]) & 0x1f;
        b1 = int(FixedBone[1]) & 0x1f;
        b2 = int(FixedBone[2]) & 0x1f;
        b3 = int(FixedBone[3]) & 0x1f;
    } else {
        b0 = int(a7_bone[0]) & 0x1f;
        b1 = int(a7_bone[1]) & 0x1f;
        b2 = int(a7_bone[2]) & 0x1f;
        b3 = int(a7_bone[3]) & 0x1f;
    }
    
    vec4 w = vec4(1, 0, 0, 0);
    
    if (SmoothSkin != 0) {
        if (FixedWeight.x != 0) {
            w[0] = FixedWeight[0];
            w[1] = FixedWeight[1];
            w[2] = FixedWeight[2];
        } else {
            w = a8_weight * Scales1[WEIGHT];
        }
    }
    
    vec4 p;
    
    p  = (Transforms[b0] * Position) * w[0];
    p += (Transforms[b1] * Position) * w[1];
    p += (Transforms[b2] * Position) * w[2];
    p += (Transforms[b3] * Position) * w[3];
    
    vec3 n, t;
    
    n  = (mat3(Transforms[b0]) * Normal) * w[0];
    n += (mat3(Transforms[b1]) * Normal) * w[1];
    n += (mat3(Transforms[b2]) * Normal) * w[2];
    n += (mat3(Transforms[b3]) * Normal) * w[3];
    
    t  = (mat3(Transforms[b0]) * Tangent) * w[0];
    t += (mat3(Transforms[b1]) * Tangent) * w[1];
    t += (mat3(Transforms[b2]) * Tangent) * w[2];
    t += (mat3(Transforms[b3]) * Tangent) * w[3];
    
    float Sum = w[0] + w[1] + w[2] + w[3];
    
    Position = (Position * (1 - Sum)) + p;
    Normal   = (Normal   * (1 - Sum)) + n;
    Tangent  = (Tangent  * (1 - Sum)) + t;
    
    Position.w = 1;
    
    Normal = normalize(mat3(ModelMatrix) * Normal);
    Tangent = normalize(mat3(ModelMatrix) * Tangent);
    
    EyeDir = normalize(vec3(ViewMatrix * ModelMatrix * Position));
    WorldPos = vec3(ModelMatrix * Position);
    
    gl_Position = ProjMatrix * ViewMatrix * ModelMatrix * Position;
}

vec2 TransformUV(vec2 UV, int Index) {
    UV -= 0.5f;
    UV = UVTransforms[Index].Transform * UV;
    UV += UVTransforms[Index].Translation + 0.5f;
    
    return UV;
}