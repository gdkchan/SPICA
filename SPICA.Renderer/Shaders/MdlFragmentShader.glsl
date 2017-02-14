#version 150

precision highp float;

#define LUT_STEP  0.00392156862 //1 / 255

#define FLAG_BUMP_RENORM   (1 << 0)
#define FLAG_CLAMP_HLIGHT  (1 << 1)
#define FLAG_D0_ENB        (1 << 2)
#define FLAG_D1_ENB        (1 << 3)
#define FLAG_R_ENB         (1 << 4)
#define FLAG_G0_ENB        (1 << 5)
#define FLAG_G1_ENB        (1 << 6)

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

struct UVTransform_t {
	vec2 Scale;
	mat2 Transform;
	vec2 Translation;
};

uniform UVTransform_t UVTransforms[3];

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

	int UpColorBuff;
	int UpAlphaBuff;

	CombArg_t Args[3];

	vec4 Color;
};

uniform Combiner_t Combiners[6];

uniform int CombinersCount;

uniform vec4 BuffColor;
uniform vec4 SAmbient;
uniform vec4 MEmission;
uniform vec4 MDiffuse;
uniform vec4 MAmbient;
uniform vec4 MSpecular;

uniform int FragFlags;
uniform int FresnelSel;
uniform int BumpIndex;
uniform int BumpMode;

struct Light_t {
	vec3 Position;
	vec4 Ambient;
	vec4 Diffuse;
	vec4 Specular;
};

uniform Light_t Lights[8];

uniform int LightsCount;
uniform int TexUnitSource[3];

struct LUT_t {
	int IsAbs;
	int Input;
	float Scale;
};

uniform LUT_t LUTs[6];

uniform sampler2D Texture[3];
uniform samplerCube TextureCube;

uniform UBDist0 { vec4 Dist0[64]; };
uniform UBDist1 { vec4 Dist1[64]; };
uniform UBFresnel { vec4 Fresnel[64]; };
uniform UBReflecR { vec4 ReflecR[64]; };
uniform UBReflecG { vec4 ReflecG[64]; };
uniform UBReflecB { vec4 ReflecB[64]; };

float CosNormalHalf;
float CosViewHalf;
float CosNormalView;
float CosLightNormal;
float CosLightSpot;
float CosPhi;

in vec3 EyeDir;
in vec3 WorldPos;
in vec3 Reflec;
in vec3 Normal;
in vec3 Tangent;
in vec4 Color;
in vec2 TexCoord0;
in vec2 TexCoord1;
in vec2 TexCoord2;

uniform mat4 ModelMatrix;

out vec4 Output;

vec4 GetTexColor(int Unit);

float GetLUTVal(int SrcLUT);

void main() {
	vec3 Nrm = Normal;

	//When bump == 2, Tangent is modified instead (but for what Tangent is used?)
	if (BumpMode == 1) {
		Nrm = GetTexColor(BumpIndex).xyz * 2 - 1;
		Nrm = mat3(Tangent, cross(Normal, Tangent), Normal) * Nrm;

		//Renormalize
		if ((FragFlags & FLAG_BUMP_RENORM) != 0) Nrm.z = sqrt(max(1 - dot(Nrm.xy, Nrm.xy), 0));
	}

	vec4 Color0 = GetTexColor(0);
	vec4 Color1 = GetTexColor(1);
	vec4 Color2 = GetTexColor(2);

	vec4 FragPrimaryColor = MEmission + MAmbient * SAmbient;
	vec4 FragSecondaryColor = vec4(0, 0, 0, 1);

	for (int i = 0; i < LightsCount; i++) {
		vec3 LightDir = normalize(Lights[i].Position - WorldPos);
		vec3 HalfVec = normalize(LightDir - EyeDir);

		CosNormalHalf  = dot(Nrm, HalfVec);
		CosViewHalf    = dot(-EyeDir, HalfVec);
		CosNormalView  = dot(Nrm, -EyeDir);
		CosLightNormal = dot(LightDir, Nrm);
		//TODO: CosLightSpot
		//TODO: CosPhi

		float fi = ((FragFlags & FLAG_CLAMP_HLIGHT) != 0 && CosLightNormal < 0) ? 0 : 1;

		float d0 = (FragFlags & FLAG_D0_ENB) != 0 ? GetLUTVal(LUT_DIST0) : 1;
		float d1 = (FragFlags & FLAG_D1_ENB) != 0 ? GetLUTVal(LUT_DIST1) : 1;

		vec4 r = vec4(0, 0, 0, 1);

		if ((FragFlags & FLAG_R_ENB) != 0) {
			r.r = GetLUTVal(LUT_REFLECR);
			r.g = GetLUTVal(LUT_REFLECG);
			r.b = GetLUTVal(LUT_REFLECB);
		}

		float g0 = 1, g1 = 1;

		if ((FragFlags & FLAG_G0_ENB) != 0) g0 = (2 * CosNormalHalf * CosNormalView)  / CosViewHalf;
		if ((FragFlags & FLAG_G1_ENB) != 0) g1 = (2 * CosNormalHalf * CosLightNormal) / CosViewHalf;

		vec4 Ambient = MAmbient * Lights[i].Ambient;
		vec4 Diffuse = MDiffuse * Lights[i].Diffuse;
		vec4 Specular = (MSpecular * d0 * g0 + r * d1 * g1) * Lights[i].Specular;

		Diffuse  = fi * (Ambient + Diffuse * CosLightNormal);
		Specular = fi * Specular;

		if ((FresnelSel & FLAG_PRI) != 0) Diffuse.a  *= GetLUTVal(LUT_FRESNEL);
		if ((FresnelSel & FLAG_SEC) != 0) Specular.a *= GetLUTVal(LUT_FRESNEL);

		FragPrimaryColor   += Diffuse;
		FragSecondaryColor += Specular;
	}

	vec4 CombBuffer = BuffColor;

	for (int Stage = 0; Stage < CombinersCount; Stage++) {
		vec4 ColorArgs[3];
		vec4 AlphaArgs[3];

		if (Combiners[Stage].UpColorBuff != 0) CombBuffer.rgb = Output.rgb;
		if (Combiners[Stage].UpAlphaBuff != 0) CombBuffer.a   = Output.a;

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
				case 13: ColorArg = CombBuffer; break;
				case 14: ColorArg = Combiners[Stage].Color; break;
				case 15: ColorArg = Output; break;
			}

			switch (Combiners[Stage].Args[Param].AlphaSrc) {
				case 0: AlphaArg = Color; break;
				case 1: AlphaArg = FragPrimaryColor; break;
				case 2: AlphaArg = FragSecondaryColor; break;
				case 3: AlphaArg = Color0; break;
				case 4: AlphaArg = Color1; break;
				case 5: AlphaArg = Color2; break;
				case 13: AlphaArg = CombBuffer; break;
				case 14: AlphaArg = Combiners[Stage].Color; break;
				case 15: AlphaArg = Output; break;
			}

			switch (Combiners[Stage].Args[Param].ColorOp) {
				case 0:  ColorArgs[Param] =     ColorArg.rgba; break;
				case 1:  ColorArgs[Param] = 1 - ColorArg.rgba; break;
				case 2:  ColorArgs[Param] =     ColorArg.aaaa; break;
				case 3:  ColorArgs[Param] = 1 - ColorArg.aaaa; break;
				case 4:  ColorArgs[Param] =     ColorArg.rrrr; break;
				case 5:  ColorArgs[Param] = 1 - ColorArg.rrrr; break;
				case 8:  ColorArgs[Param] =     ColorArg.gggg; break;
				case 9:  ColorArgs[Param] = 1 - ColorArg.gggg; break;
				case 12: ColorArgs[Param] =     ColorArg.bbbb; break;
				case 13: ColorArgs[Param] = 1 - ColorArg.bbbb; break;
			}

			switch (Combiners[Stage].Args[Param].AlphaOp) {
				//    RGBA (not used because Alpha only uses 1 channel)
				//1 - RGBA (not used because Alpha only uses 1 channel)
				case 0: AlphaArgs[Param] =     AlphaArg.aaaa; break;
				case 1: AlphaArgs[Param] = 1 - AlphaArg.aaaa; break;
				case 2: AlphaArgs[Param] =     AlphaArg.rrrr; break;
				case 3: AlphaArgs[Param] = 1 - AlphaArg.rrrr; break;
				case 4: AlphaArgs[Param] =     AlphaArg.gggg; break;
				case 5: AlphaArgs[Param] = 1 - AlphaArg.gggg; break;
				case 6: AlphaArgs[Param] =     AlphaArg.bbbb; break;
				case 7: AlphaArgs[Param] = 1 - AlphaArg.bbbb; break;
			}
		}

		switch (Combiners[Stage].ColorCombine) {
			case 0: Output.rgb = ColorArgs[0].rgb; break;
			case 1: Output.rgb = ColorArgs[0].rgb * ColorArgs[1].rgb; break;
			case 2: Output.rgb = min(ColorArgs[0].rgb + ColorArgs[1].rgb, 1); break;
			case 3: Output.rgb = clamp(ColorArgs[0].rgb + ColorArgs[1].rgb - 0.5, 0, 1); break;
			case 4: Output.rgb = mix(ColorArgs[1].rgb, ColorArgs[0].rgb, ColorArgs[2].rgb); break;
			case 5: Output.rgb = max(ColorArgs[0].rgb - ColorArgs[1].rgb, 0); break;
			case 6: Output.rgb = vec3(clamp(dot(ColorArgs[0].rgb, ColorArgs[1].rgb), 0, 1)); break;
			case 7: Output.rgb = vec3(clamp(dot(ColorArgs[0], ColorArgs[1]), 0, 1)); break;
			case 8: Output.rgb = clamp(ColorArgs[0].rgb * ColorArgs[1].rgb + ColorArgs[2].rgb, 0, 1); break;
			case 9: Output.rgb = clamp(min(ColorArgs[0].rgb + ColorArgs[1].rgb, 1) * ColorArgs[2].rgb, 0, 1); break;
		}

		switch (Combiners[Stage].AlphaCombine) {
			case 0: Output.a = AlphaArgs[0].a; break;
			case 1: Output.a = AlphaArgs[0].a * AlphaArgs[1].a; break;
			case 2: Output.a = min(AlphaArgs[0].a + AlphaArgs[1].a, 1); break;
			case 3: Output.a = clamp(AlphaArgs[0].a + AlphaArgs[1].a - 0.5, 0, 1); break;
			case 4: Output.a = mix(AlphaArgs[1].a, AlphaArgs[0].a, AlphaArgs[2].a); break;
			case 5: Output.a = max(AlphaArgs[0].a - AlphaArgs[1].a, 0); break;
			case 6: Output.a = clamp(dot(AlphaArgs[0].rgb, AlphaArgs[1].rgb), 0, 1); break;
			case 7: Output.a = clamp(dot(AlphaArgs[0], AlphaArgs[1]), 0, 1); break;
			case 8: Output.a = clamp(AlphaArgs[0].a * AlphaArgs[1].a + AlphaArgs[2].a, 0, 1); break;
			case 9: Output.a = clamp(min(AlphaArgs[0].a + AlphaArgs[1].a, 1) * AlphaArgs[2].a, 0, 1); break;
		}

		Output.rgb *= Combiners[Stage].ColorScale;
		Output.a *= Combiners[Stage].AlphaScale;
	}

	if (AlphaTestEnb != 0) {
		bool Pass = true;
		float Ref = float(AlphaTestRef) / 255;

		switch (AlphaTestFunc) {
			case 0: Pass = false; break;
			case 1: Pass = true; break;
			case 2: Pass = Output.a == Ref; break;
			case 3: Pass = Output.a != Ref; break;
			case 4: Pass = Output.a <  Ref; break;
			case 5: Pass = Output.a <= Ref; break;
			case 6: Pass = Output.a >  Ref; break;
			case 7: Pass = Output.a >= Ref; break;
		}

		if (!Pass) discard;
	}
}

vec2 TransformUV(vec2 UV, int Index) {
	//Note: The 0.5 offset is to rotate around the center
	UV = (UV - UVTransforms[Index].Translation) * UVTransforms[Index].Scale;
	UV = UVTransforms[Index].Transform * (UV - 0.5f) + 0.5f;

	return UV;
}

vec4 GetTexColor(int Unit) {
	vec4 Color;

	switch (TexUnitSource[Unit]) {
		case 0: Color = texture(Texture[Unit], TransformUV(TexCoord0, Unit)); break;
		case 1: Color = texture(Texture[Unit], TransformUV(TexCoord1, Unit)); break;
		case 2: Color = texture(Texture[Unit], TransformUV(TexCoord2, Unit)); break;
		case 3: Color = texture(TextureCube, Reflec); break;
		case 4:
			vec3 R = vec3(Reflec.x, Reflec.y, Reflec.z + 1);
			float m = 2 * sqrt(dot(R, R));
			Color = texture(Texture[Unit], R.xy / m + 0.5);
			break;
	}

	return Color;
}

float GetLUTVal(int SrcLUT, int Index) {
	int i = Index >> 2;
	int j = Index &  3;

	float Value;

	switch (SrcLUT) {
		case LUT_DIST0:   Value = Dist0[i][j];   break;
		case LUT_DIST1:   Value = Dist1[i][j];   break;
		case LUT_FRESNEL: Value = Fresnel[i][j]; break;
		case LUT_REFLECR: Value = ReflecR[i][j]; break;
		case LUT_REFLECG: Value = ReflecG[i][j]; break;
		case LUT_REFLECB: Value = ReflecB[i][j]; break;
	}

	return Value;
}

float GetLUTVal(int SrcLUT) {
	float IndexVal;

	switch (LUTs[SrcLUT].Input) {
		case LUT_IN_NH: IndexVal = CosNormalHalf; break;
		case LUT_IN_VH: IndexVal = CosViewHalf; break;
		case LUT_IN_NV: IndexVal = CosNormalView; break;
		case LUT_IN_LN: IndexVal = CosLightNormal; break;
		case LUT_IN_LP: IndexVal = CosLightSpot; break;
		case LUT_IN_CP: IndexVal = CosPhi; break;
	}

	if (LUTs[SrcLUT].IsAbs == 0) {
		IndexVal = clamp(IndexVal, -1, 1);
		IndexVal = (IndexVal < 0 ? IndexVal + 2 : IndexVal) * 0.5;
	} else {
		IndexVal = clamp(IndexVal, 0, 1);
	}

	int Left = int(IndexVal * 0xff);
	int Right = min(Left + 1, 0xff);

	if ((LUTs[SrcLUT].IsAbs == 0 && Left == 0x7f) || Left == 0xff) {
		//Table end, prevent wrapping
		Right = Left;
	}

	float LVal = GetLUTVal(SrcLUT, Left);
	float RVal = GetLUTVal(SrcLUT, Right);

	float Value = mix(LVal, RVal, (IndexVal - float(Left) * LUT_STEP) / LUT_STEP);

	return clamp(Value * LUTs[SrcLUT].Scale, 0, 1);
}