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
uniform int TexUnit0Source;
uniform int TexUnit1Source;
uniform int TexUnit2Source;

struct LUT_t {
	int IsAbs;
	int Input;
	float Scale;
};

uniform LUT_t LUTs[6];

uniform sampler2D Texture0;
uniform sampler2D Texture1;
uniform sampler2D Texture2;

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

in vec3 Normal;
in vec3 Tangent;
in vec4 Color;
in vec2 TexCoord0;
in vec2 TexCoord1;
in vec2 TexCoord2;

out vec4 Output;

vec2 TransformUV(vec2 UV, int Index);

float GetLUTVal(int SrcLUT);

void main() {
	vec3 Reflec = reflect(EyeDir, Normal);
    vec3 SReflect = vec3(Reflec.x, Reflec.y, Reflec.z + 1);
	float m = 2 * sqrt(dot(SReflect, SReflect));
	vec2 SphereUV = Reflec.xy / m + 0.5;

	vec4 Color0, Color1, Color2, Color3;

	switch (TexUnit0Source) {
		case 0: Color0 = texture(Texture0, TransformUV(TexCoord0, 0)); break;
		case 1: Color0 = texture(Texture0, TransformUV(TexCoord1, 0)); break;
		case 2: Color0 = texture(Texture0, TransformUV(TexCoord2, 0)); break;
		case 3: Color0 = texture(TextureCube, Reflec); break;
		case 4: Color0 = texture(Texture0, SphereUV); break;
	}

	switch (TexUnit1Source) {
		case 0: Color1 = texture(Texture1, TransformUV(TexCoord0, 1)); break;
		case 1: Color1 = texture(Texture1, TransformUV(TexCoord1, 1)); break;
		case 2: Color1 = texture(Texture1, TransformUV(TexCoord2, 1)); break;
		case 3: Color1 = texture(TextureCube, Reflec); break;
		case 4: Color1 = texture(Texture1, SphereUV); break;
	}

	switch (TexUnit2Source) {
		case 0: Color2 = texture(Texture2, TransformUV(TexCoord0, 2)); break;
		case 1: Color2 = texture(Texture2, TransformUV(TexCoord1, 2)); break;
		case 2: Color2 = texture(Texture2, TransformUV(TexCoord2, 2)); break;
		case 3: Color2 = texture(TextureCube, Reflec); break;
		case 4: Color2 = texture(Texture2, SphereUV); break;
	}

	vec3 Nrm = Normal;

	if (BumpMode != 0) {
		switch (BumpIndex) {
			case 0: Nrm = Color0.xyz * 2 - 1; break;
			case 1: Nrm = Color1.xyz * 2 - 1; break;
			case 2: Nrm = Color2.xyz * 2 - 1; break;
		}

		Nrm = mat3(Tangent, cross(Normal, Tangent), Normal) * Nrm;

		if (BumpMode == 1) Nrm = normalize(Normal + Nrm);

		if ((FragFlags & FLAG_BUMP_RENORM) != 0) Nrm.z = sqrt(max(1 - dot(Nrm.xy, Nrm.xy), 0));
	}

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

		vec4 Reflec = vec4(0, 0, 0, 1);

		if ((FragFlags & FLAG_R_ENB) != 0) {
			Reflec.r = GetLUTVal(LUT_REFLECR);
			Reflec.g = GetLUTVal(LUT_REFLECG);
			Reflec.b = GetLUTVal(LUT_REFLECB);
		}

		float g0 = 1, g1 = 1;

		if ((FragFlags & FLAG_G0_ENB) != 0) g0 = (2 * CosNormalHalf * CosNormalView)  / CosViewHalf;
		if ((FragFlags & FLAG_G1_ENB) != 0) g1 = (2 * CosNormalHalf * CosLightNormal) / CosViewHalf;

		vec4 Ambient = MAmbient * Lights[i].Ambient;
		vec4 Diffuse = MDiffuse * Lights[i].Diffuse;
		vec4 Specular = (MSpecular * d0 * g0 + Reflec * d1 * g1) * Lights[i].Specular;

		Diffuse  = fi * (Ambient + Diffuse * CosLightNormal);
		Specular = fi * Specular;

		if ((FresnelSel & FLAG_PRI) != 0) Diffuse.a  *= GetLUTVal(LUT_FRESNEL);
		if ((FresnelSel & FLAG_SEC) != 0) Specular.a *= GetLUTVal(LUT_FRESNEL);

		FragPrimaryColor   += Diffuse;
		FragSecondaryColor += Specular;
	}

	vec4 CombBuffer = BuffColor;

	for (int Stage = 0; Stage < 6; Stage++) {
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
				case 6: ColorArg = Color3; break;
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
				case 6: AlphaArg = Color3; break;
				case 13: AlphaArg = CombBuffer; break;
				case 14: AlphaArg = Combiners[Stage].Color; break;
				case 15: AlphaArg = Output; break;
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

float GetLUTVal(int SrcLUT, int Index) {
	int i = Index >> 2;
	int j = Index &  3;

	float Value;

	switch (SrcLUT) {
		case LUT_DIST0:   Value = Dist0[i][j]; break;
		case LUT_DIST1:   Value = Dist1[i][j]; break;
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

	//Nearest, smaller (round down) quantized index for this float
	int Left = int(IndexVal * 0xff);

	//End of table, in this case we have nothing to interpolate
	bool End = (LUTs[SrcLUT].IsAbs == 0 && Left == 0x7f) || Left == 0xff;

	//Nearest, bigger (round up) quantized value for this float
	int Right = End ? Left : min(Left + 1, 0xff);

	//Get actual LUT float values for both indices, and interpolates the two
	float LVal = GetLUTVal(SrcLUT, Left);
	float RVal = GetLUTVal(SrcLUT, Right);

	float Value = mix(LVal, RVal, (IndexVal - float(Left) * LUT_STEP) / LUT_STEP);

	return clamp(Value * LUTs[SrcLUT].Scale, 0, 1);
}