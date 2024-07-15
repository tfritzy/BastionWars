shader_type spatial;

uniform float range_size = .1;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	float range_start = 1.0 - range_size;
	float x = abs(UV.r-.5) * 2.0;
	float y = abs(UV.g-.5) * 2.0;
	float distIntoX = max(x - range_start, 0);
	float percentIntoX = distIntoX / range_size;
	float distIntoY = max(y - range_start, 0);
	float percentIntoY = distIntoY / range_size;
	
	ALPHA = max(percentIntoX, percentIntoY);
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
