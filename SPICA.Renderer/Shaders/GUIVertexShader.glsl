#version 130

precision highp float;

in vec4 Coords;

out vec2 TexCoord;

void main() {
	TexCoord = Coords.zw;

	gl_Position = vec4(Coords.xy * 2 - 1, 0, 1);
}