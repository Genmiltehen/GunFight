#version 330 core
in vec2 fragPos;
out vec4 FragColor;

uniform vec2 uCenter;
uniform vec2 uSize;  // Half-extents (width/2, height/2)
uniform float uAngle; // Rotation in radians
uniform vec3 uColor;

mat2 rotate2d(float _angle){
    return mat2(cos(_angle),-sin(_angle),
                sin(_angle),cos(_angle));
}

float sdfBox(vec2 p, vec2 size) {
    vec2 d = abs(p) - size;
    return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0);
}

void main() {
    float d = sdfBox(rotate2d(uAngle) * (fragPos - uCenter), uSize);

    float upp = fwidth(d);
    float pixd = d / upp + 1.0;
    float line = smoothstep(1.0, 0.0, abs(pixd));

    if (line < 0.01) discard;

    FragColor = vec4(uColor, line);
}