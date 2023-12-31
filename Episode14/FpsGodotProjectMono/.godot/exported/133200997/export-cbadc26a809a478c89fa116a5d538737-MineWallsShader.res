RSRC                    VisualShader            ��������                                            i      resource_local_to_scene    resource_name    output_port_for_preview    default_input_values    expanded_output_ports    source    texture    texture_type    script    parameter_name 
   qualifier    color_default    texture_filter    texture_repeat    texture_source    input_name 	   operator 	   function    op_type    hint    min    max    step    default_value_enabled    default_value    code    graph_offset    mode    modes/blend    modes/depth_draw    modes/cull    modes/diffuse    modes/specular    flags/depth_prepass_alpha    flags/depth_test_disabled    flags/sss_mode_skin    flags/unshaded    flags/wireframe    flags/skip_vertex_transform    flags/world_vertex_coords    flags/ensure_correct_normals    flags/shadows_disabled    flags/ambient_light_disabled    flags/shadow_to_opacity    flags/vertex_lighting    flags/particle_trails    flags/alpha_to_coverage     flags/alpha_to_coverage_and_one    flags/debug_shadow_splits    nodes/vertex/0/position    nodes/vertex/connections    nodes/fragment/0/position    nodes/fragment/2/node    nodes/fragment/2/position    nodes/fragment/3/node    nodes/fragment/3/position    nodes/fragment/7/node    nodes/fragment/7/position    nodes/fragment/9/node    nodes/fragment/9/position    nodes/fragment/11/node    nodes/fragment/11/position    nodes/fragment/23/node    nodes/fragment/23/position    nodes/fragment/24/node    nodes/fragment/24/position    nodes/fragment/25/node    nodes/fragment/25/position    nodes/fragment/26/node    nodes/fragment/26/position    nodes/fragment/27/node    nodes/fragment/27/position    nodes/fragment/29/node    nodes/fragment/29/position    nodes/fragment/30/node    nodes/fragment/30/position    nodes/fragment/31/node    nodes/fragment/31/position    nodes/fragment/32/node    nodes/fragment/32/position    nodes/fragment/33/node    nodes/fragment/33/position    nodes/fragment/34/node    nodes/fragment/34/position    nodes/fragment/35/node    nodes/fragment/35/position    nodes/fragment/36/node    nodes/fragment/36/position    nodes/fragment/connections    nodes/light/0/position    nodes/light/connections    nodes/start/0/position    nodes/start/connections    nodes/process/0/position    nodes/process/connections    nodes/collide/0/position    nodes/collide/connections    nodes/start_custom/0/position    nodes/start_custom/connections     nodes/process_custom/0/position !   nodes/process_custom/connections    nodes/sky/0/position    nodes/sky/connections    nodes/fog/0/position    nodes/fog/connections        &   local://VisualShaderNodeTexture_u04x6 �      1   local://VisualShaderNodeTexture2DParameter_xvl8e �      8   local://VisualShaderNodeTextureParameterTriplanar_ucbqe O      8   local://VisualShaderNodeTextureParameterTriplanar_5vqka �      8   local://VisualShaderNodeTextureParameterTriplanar_0m383       $   local://VisualShaderNodeInput_5nih3 x      $   local://VisualShaderNodeInput_fgckl �      /   local://VisualShaderNodeTransformVecMult_f7nny �      ,   local://VisualShaderNodeTransformFunc_xtxdu        '   local://VisualShaderNodeVectorOp_1j107 N      "   local://VisualShaderNodeMix_jwm3v �      -   local://VisualShaderNodeFloatParameter_u76om       -   local://VisualShaderNodeFloatParameter_676kk x      -   local://VisualShaderNodeFloatParameter_pfl1f �      &   local://VisualShaderNodeFloatOp_7hiv3 _      -   local://VisualShaderNodeColorParameter_hg3xi �      '   local://VisualShaderNodeVectorOp_mpn0p �      -   local://VisualShaderNodeFloatParameter_u281l          local://VisualShader_qbhkb ~         VisualShaderNodeTexture                   #   VisualShaderNodeTexture2DParameter    	      
   NormalMap                *   VisualShaderNodeTextureParameterTriplanar    	         RockAlbedoTriplanar                *   VisualShaderNodeTextureParameterTriplanar    	         RockNormalTriplanar                *   VisualShaderNodeTextureParameterTriplanar    	         RockRoughnessTriplanar          VisualShaderNodeInput             view_matrix          VisualShaderNodeInput             vertex       !   VisualShaderNodeTransformVecMult             VisualShaderNodeTransformFunc             VisualShaderNodeVectorOp                      VisualShaderNodeMix                          ?   ?   ?           �?  �?  �?            ?   ?   ?                  VisualShaderNodeFloatParameter    	         DetailNormalPower                           �?         VisualShaderNodeFloatParameter    	         NormalMapPower                  �@                 �?         VisualShaderNodeFloatParameter    	         RoughnessMult                           �?         VisualShaderNodeFloatOp                      VisualShaderNodeColorParameter    	      	   RockTint                   VisualShaderNodeVectorOp                      VisualShaderNodeFloatParameter    	         TriPlanerScaleHard                  �?         VisualShader (         �	  shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_back, diffuse_lambert, specular_schlick_ggx;

uniform sampler2D RockAlbedoTriplanar : source_color;
uniform vec4 RockTint : source_color = vec4(1.000000, 1.000000, 1.000000, 1.000000);
uniform sampler2D RockRoughnessTriplanar;
uniform float RoughnessMult : hint_range(0, 1) = 1;
uniform sampler2D NormalMap : hint_normal;
uniform sampler2D RockNormalTriplanar : hint_normal;
uniform float DetailNormalPower : hint_range(0, 1) = 1;
uniform float NormalMapPower : hint_range(0, 5) = 1;


// TextureParameterTriplanar
	vec4 triplanar_texture(sampler2D p_sampler, vec3 p_weights, vec3 p_triplanar_pos) {
		vec4 samp = vec4(0.0);
		samp += texture(p_sampler, p_triplanar_pos.xy) * p_weights.z;
		samp += texture(p_sampler, p_triplanar_pos.xz) * p_weights.y;
		samp += texture(p_sampler, p_triplanar_pos.zy * vec2(-1.0, 1.0)) * p_weights.x;
		return samp;
	}

	uniform vec3 triplanar_scale = vec3(1.0, 1.0, 1.0);
	uniform vec3 triplanar_offset;
	uniform float triplanar_sharpness = 0.5;

	varying vec3 triplanar_power_normal;
	varying vec3 triplanar_pos;

void vertex() {
// TextureParameterTriplanar
	{
		triplanar_power_normal = pow(abs(NORMAL), vec3(triplanar_sharpness));
		triplanar_power_normal /= dot(triplanar_power_normal, vec3(1.0));
		triplanar_pos = VERTEX * triplanar_scale + triplanar_offset;
		triplanar_pos *= vec3(1.0, -1.0, 1.0);
	}
}

void fragment() {
// TextureParameterTriplanar:7
	vec4 n_out7p0 = triplanar_texture(RockAlbedoTriplanar, triplanar_power_normal, triplanar_pos);


// ColorParameter:34
	vec4 n_out34p0 = RockTint;


// VectorOp:35
	vec3 n_out35p0 = vec3(n_out7p0.xyz) * vec3(n_out34p0.xyz);


// TextureParameterTriplanar:11
	vec4 n_out11p0 = triplanar_texture(RockRoughnessTriplanar, triplanar_power_normal, triplanar_pos);


// FloatParameter:32
	float n_out32p0 = RoughnessMult;


// FloatOp:33
	float n_out33p0 = n_out11p0.x * n_out32p0;


	vec4 n_out2p0;
// Texture2D:2
	n_out2p0 = texture(NormalMap, UV);


// TextureParameterTriplanar:9
	vec4 n_out9p0 = triplanar_texture(RockNormalTriplanar, triplanar_power_normal, triplanar_pos);


// FloatParameter:30
	float n_out30p0 = DetailNormalPower;


// Mix:29
	vec3 n_out29p0 = mix(vec3(n_out2p0.xyz), vec3(n_out9p0.xyz), vec3(n_out30p0));


// FloatParameter:31
	float n_out31p0 = NormalMapPower;


// Output:0
	ALBEDO = n_out35p0;
	ROUGHNESS = n_out33p0;
	NORMAL_MAP = n_out29p0;
	NORMAL_MAP_DEPTH = n_out31p0;


}
 3   
    ��D   B4             5   
     ��  D6            7   
     C�  D8            9   
     R�  ��:            ;   
     R�  �<            =   
     R�  4�>            ?   
    �'� ���@            A   
    ��  k�B            C   
     � ���D            E   
    @�  ��F         	   G   
   {��ą{��H         
   I   
     D  �CJ            K   
     p�  �CL            M   
     D  �CN            O   
     *�  4�P            Q   
     4�  ��R            S   
     ��  M�T            U   
     �B  %�V            W   
   ����^�^�X       D                                                                                                             	              
           !      "       #             #              !       #               	             !              $                   RSRC