#version 330 core
#extension GL_ARB_explicit_uniform_location: enable

precision highp float;

#define BOOL_SMOOTH_SK   (1 <<  1)
#define BOOL_RIGID_SK    (1 <<  2)
#define BOOL_HEMI_ENB    (1 <<  5) //Phong Enable on Pokémon
#define BOOL_HEMIAO_ENB  (1 <<  6) //Rim Enable on Pokémon
#define BOOL_COLOR_A     (1 <<  7)
#define BOOL_BONE_W      (1 <<  8)
#define BOOL_UV0_ENB     (1 <<  9)
#define BOOL_UV1_ENB     (1 << 10)
#define BOOL_UV2_ENB     (1 << 11)
#define BOOL_VTXLGT_ENB  (1 << 12)
#define BOOL_TEX1_ENB    (1 << 13)
#define BOOL_TEX2_ENB    (1 << 14)

#define FA_NORM    (1 << 1)
#define FA_TAN     (1 << 2)
#define FA_COL     (1 << 3)
#define FA_TEX0    (1 << 4)
#define FA_TEX1    (1 << 5)
#define FA_TEX2    (1 << 6)
#define FA_BONE    (1 << 7)
#define FA_WEIGHT  (1 << 8)

#define S0_POS	   0
#define S0_NORM    1
#define S0_TAN	   2
#define S0_COL	   3
#define S1_TEX0    0
#define S1_TEX1    1
#define S1_TEX2    2
#define S1_WEIGHT  3

uniform mat4 ProjMatrix;
uniform mat4 ViewMatrix;
uniform mat4 ModelMatrix;

uniform vec4 PosOffset;
uniform vec4 Scales0;
uniform vec4 Scales1;

uniform mat4 Transforms[32];

uniform float ColorScale;

uniform int BoolUniforms;
uniform int FixedAttr;

uniform vec4 FixedNorm;
uniform vec4 FixedTan;
uniform vec4 FixedCol;
uniform vec4 FixedTex0;
uniform vec4 FixedTex1;
uniform vec4 FixedTex2;
uniform vec4 FixedBone;
uniform vec4 FixedWeight;

uniform vec4 PowerScale;

uniform vec4 MDiffuse;

uniform int ObjNormalMap;
uniform int BumpMode;

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

out vec3 View;
out vec3 World;
out vec4 Position;
out vec3 ONormal;
out vec3 Normal;
out vec3 Tangent;
out vec4 Color;
out vec2 TexCoord0;
out vec2 TexCoord1;
out vec2 TexCoord2;

void main() {
	/*
	 * Note: The order in which variables in accessed in important on (some) GPUs, so don't change it!
	 * In particular, Intel drivers seems to order attributes in the order they're accessed on the code.
	 * This is hacky, but GPUs that supports the explicit location doesn't have this problem (yay!).
	 */
	Position = PosOffset + vec4(a0_pos * Scales0[S0_POS], 1);

	Normal    = (FixedAttr & FA_NORM) != 0 ? FixedNorm.xyz : a1_norm * Scales0[S0_NORM];
	Tangent   = (FixedAttr & FA_TAN)  != 0 ? FixedTan.xyz  : a2_tan  * Scales0[S0_TAN];
	Color     = (FixedAttr & FA_COL)  != 0 ? FixedCol      : a3_col  * Scales0[S0_COL];
	TexCoord0 = (FixedAttr & FA_TEX0) != 0 ? FixedTex0.xy  : a4_tex0 * Scales1[S1_TEX0];
	TexCoord1 = (FixedAttr & FA_TEX1) != 0 ? FixedTex1.xy  : a5_tex1 * Scales1[S1_TEX1];
	TexCoord2 = (FixedAttr & FA_TEX2) != 0 ? FixedTex2.xy  : a6_tex2 * Scales1[S1_TEX2];

	//Apply bone transform
	int b0, b1, b2, b3;

	if ((FixedAttr & FA_BONE) != 0) {
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

	if ((BoolUniforms & BOOL_SMOOTH_SK) != 0) {
		if ((FixedAttr & FA_WEIGHT) != 0)
			w = FixedWeight;
		else
			w = a8_weight * Scales1[S1_WEIGHT];
	}

	if ((BoolUniforms & BOOL_BONE_W) == 0) w[3] = 0;

	vec4 Pos;

	Pos  = (Transforms[b0] * Position) * w[0];
	Pos += (Transforms[b1] * Position) * w[1];
	Pos += (Transforms[b2] * Position) * w[2];
	Pos += (Transforms[b3] * Position) * w[3];

	ONormal = Normal;

	if (ObjNormalMap != 0 && BumpMode == 1) {
		Normal  = vec3(0, 0, 1);
		Tangent = vec3(1, 0, 0);
	}

	vec3 ONrm, Nrm, Tan;

	ONrm  = (mat3(Transforms[b0]) * ONormal) * w[0];
	ONrm += (mat3(Transforms[b1]) * ONormal) * w[1];
	ONrm += (mat3(Transforms[b2]) * ONormal) * w[2];
	ONrm += (mat3(Transforms[b3]) * ONormal) * w[3];

	Nrm   = (mat3(Transforms[b0]) * Normal)  * w[0];
	Nrm  += (mat3(Transforms[b1]) * Normal)  * w[1];
	Nrm  += (mat3(Transforms[b2]) * Normal)  * w[2];
	Nrm  += (mat3(Transforms[b3]) * Normal)  * w[3];

	Tan   = (mat3(Transforms[b0]) * Tangent) * w[0];
	Tan  += (mat3(Transforms[b1]) * Tangent) * w[1];
	Tan  += (mat3(Transforms[b2]) * Tangent) * w[2];
	Tan  += (mat3(Transforms[b3]) * Tangent) * w[3];

	float Sum = w[0] + w[1] + w[2] + w[3];

	Position = (Position * (1 - Sum)) + Pos;
	ONormal  = (ONormal  * (1 - Sum)) + ONrm;
	Normal   = (Normal   * (1 - Sum)) + Nrm;
	Tangent  = (Tangent  * (1 - Sum)) + Tan;

	Position.w = 1;

	ONormal = normalize(mat3(ModelMatrix) * ONormal);
	Normal  = normalize(mat3(ModelMatrix) * Normal);
	Tangent = normalize(mat3(ModelMatrix) * Tangent);

	Color.rgb *= ColorScale;

	View = -normalize(vec3(ViewMatrix * ModelMatrix * Position));

	if (Scales0[S0_COL] == 0 && (FixedAttr & FA_COL)  == 0) {
		//Color doesn't exist, use Material Diffuse instead
		Color = MDiffuse;
	}

	if ((BoolUniforms & (BOOL_HEMI_ENB | BOOL_HEMIAO_ENB)) != 0) {
		/*
		 * On Pokémon models, when this bit is set output Color seems to contain Rim Color.
		 * Alpha value from color is multiplied by a material Constant color and then added to shaded texture color.
		 * Other games that uses the H3D shaders will output Hemisphere lighting color on the Vertex Color.
		 * To make both games happy, we output Hemisphere color on RGB and Rim intensity on Alpha.
		 * TODO: Hemisphere lighting values are hardcoded, make then configurable on Light instead.
		 */
		float Sky = ONormal.y * 0.8 + 0.2;
		float Rim = 1 - dot(ONormal, View);

		vec3 HemiCol = mix(vec3(0), vec3(1), Sky);

		if ((BoolUniforms & BOOL_HEMIAO_ENB) != 0) {
			//Ambient occlusion is stored on Alpha of Vertex Color
			HemiCol *= Color.aaa;

			if (PowerScale.y != 0) {
				//This is needed by Pokémon only (their custom shader calculates Rim light)
				Color.a = clamp(pow(Rim, PowerScale.x) * PowerScale.y, 0, 1);
			}
		}

		Color.rgb = clamp(Color.rgb + HemiCol * MDiffuse.rgb, 0, 1);
	}

	World = vec3(ModelMatrix * Position);

	Position = ProjMatrix * ViewMatrix * ModelMatrix * Position;

	gl_Position = Position;
}