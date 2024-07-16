shader_type spatial;
render_mode unshaded;

uniform vec3 color = vec3(1, 1, 1);

void fragment() {
	ALBEDO = color;
}