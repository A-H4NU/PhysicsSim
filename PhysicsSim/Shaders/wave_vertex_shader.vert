#version 460 core

#define E 2.7182818284590452353602874
#define PI 3.1415926535897932384626433832795

layout (location = 0) in vec4 position;
layout (location = 1) in vec4 color;

layout (location = 10) uniform mat4 modelview;
layout (location = 11) uniform mat4 projection;

layout (location = 20) uniform float amp;	// amplitude
layout (location = 21) uniform float len;	// length of the string
layout (location = 22) uniform float freq;	// frequency
layout (location = 23) uniform float speed;
layout (location = 24) uniform float time;

out vec4 vs_color;

float sooth(float x)
{
    x = abs(x);
    float a = 1 / (1 + pow(E, -1/sqrt(x)));
    return (2 - 2 * a) * (2 - 2 * a);
}

void main(void)
{
    float x = position.x + len / 2;
	float res = 0;
    float wavenumber = 2 * PI / speed * freq;
    float angularFrequency = 2 * PI * freq;
    int n = 0;
    while (time * speed > 2 * n * len + x && n <= 10)
    {
        float para = angularFrequency * (time - len / speed * n) - wavenumber * (x + len * n);
        res += pow(0.7, n + 1) * amp * sooth(para) * sin(para);
        n += 1;
    }
    n = 1;
    while (time * speed > len * 2 * n- x && n <= 11)
    {
        float para = angularFrequency * (time - len / speed * n) + wavenumber * (x - len * n);
        res -= pow(0.6, n - 1) * amp
            * sooth(para)
            * sin(para);
        n += 1;
    }
    vec4 pos = vec4(position.x, res, 0, 1);
	gl_Position = projection * modelview * pos;
	vs_color = color;
}