#version 130

precision highp float;

uniform sampler2D UITexture;

in vec2 TexCoord;

void main() {
	gl_FragColor = texture(UITexture, TexCoord);
}