#version 460 core

in vec2 vs_textureCoord;
uniform sampler2D textureObject;

out vec4 color;

void main(void)
{
	color = texture(textureObject, vs_textureCoord);
}