#version 330 core
in vec2 fragPos;

out vec4 FragColor;

uniform vec2 uPointA;
uniform vec2 uPointB;
uniform float uRadius;
uniform vec3 uColor;

float sdfSegment(vec2 p, vec2 a, vec2 b) {
    vec2 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h);
}

void main() {
    float d = sdfSegment(fragPos, uPointA, uPointB);

    float distToEdge = d - uRadius;
    float upp = fwidth(distToEdge);
    float pixd = distToEdge / upp + 1;
    float line = smoothstep(1.0, 0.0, abs(pixd));

    if (line < 0.01) discard;

    FragColor = vec4(uColor, line);
}