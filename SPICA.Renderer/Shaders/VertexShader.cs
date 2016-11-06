namespace SPICA.Renderer.Shaders
{
    class VertexShader
    {
        public const string Code = @"
#version 150
#extension GL_ARB_explicit_uniform_location: enable

precision highp float;

uniform mat4 ProjMatrix;
uniform mat4 ViewMatrix;
uniform mat4 ModelMatrix;

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
    gl_Position = ProjMatrix * ViewMatrix * ModelMatrix * vec4(a0_pos, 1);
    
    ModelMtx = mat3(ModelMatrix);
    EyeDir = normalize(vec3(ViewMatrix * ModelMatrix * vec4(a0_pos, 1)));
    WorldPos = vec3(ModelMatrix * vec4(a0_pos, 1));
    
    Normal = normalize(mat3(ModelMatrix) * a1_norm);
    Tangent = normalize(mat3(ModelMatrix) * a2_tan);
    Color = a3_col;
    TexCoord0 = vec2(a4_tex0.x, -a4_tex0.y);
    TexCoord1 = vec2(a5_tex1.x, -a5_tex1.y);
    TexCoord2 = vec2(a6_tex2.x, -a6_tex2.y);
}
";
    }
}
