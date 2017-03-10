#version 150

precision highp float;

uniform sampler2D LUTs[6];

uniform sampler2D Textures[3];

uniform samplerCube TextureCube;

struct Light_t {
	vec3 Position;
	vec4 Ambient;
	vec4 Diffuse;
	vec4 Specular;
};

uniform int LightsCount;

uniform Light_t Lights[8];

uniform mat3 UVTransforms[3];

uniform vec4 SAmbient;

in vec3 View;
in vec3 World;
in vec4 Position;
in vec3 ONormal;
in vec3 Normal;
in vec3 Tangent;
in vec4 Color;
in vec2 TexCoord0;
in vec2 TexCoord1;
in vec2 TexCoord2;

out vec4 Output;

vec2 CalcSpherical() {
	vec3 r = reflect(-View, ONormal);
	vec3 r1 = vec3(r.x, r.y, r.z + 1);
	float m = 2 * sqrt(dot(r1, r1));
	return r.xy / m + 0.5;
}