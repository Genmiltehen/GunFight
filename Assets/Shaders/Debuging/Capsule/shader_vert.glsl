#version 330 core
layout (location = 0) in vec2 aPos;
layout (location = 1) in vec2 aTexCoord;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

out vec2 fragPos; 

void main() {
    vec4 worldPos = uModel * vec4(aPos, 0.0, 1.0);
    fragPos = worldPos.xy;
    
    gl_Position = uProjection * uView * worldPos;
}