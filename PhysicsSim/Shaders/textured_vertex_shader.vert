#version 460 core

layout (location = 0) in vec4 position;
layout (location = 1) in vec2 textureCoord;

out vec2 vs_textureCoord;

layout (location = 10) uniform mat4 modelView;
layout (location = 11) uniform mat4 projection;

void main(void)
{
	vs_textureCoord = textureCoord;
	gl_Position = projection * modelView * position;
}