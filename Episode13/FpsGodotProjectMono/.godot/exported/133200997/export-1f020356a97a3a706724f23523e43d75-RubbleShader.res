RSRC                    VisualShader            ��������                                            {      resource_local_to_scene    resource_name    output_port_for_preview    default_input_values    expanded_output_ports    source    texture    texture_type    script    parameter_name 
   qualifier    color_default    texture_filter    texture_repeat    texture_source    op_type    input_name 	   operator 	   function    default_value_enabled    default_value    hint    min    max    step    code    graph_offset    mode    modes/blend    modes/depth_draw    modes/cull    modes/diffuse    modes/specular    flags/depth_prepass_alpha    flags/depth_test_disabled    flags/sss_mode_skin    flags/unshaded    flags/wireframe    flags/skip_vertex_transform    flags/world_vertex_coords    flags/ensure_correct_normals    flags/shadows_disabled    flags/ambient_light_disabled    flags/shadow_to_opacity    flags/vertex_lighting    flags/particle_trails    flags/alpha_to_coverage     flags/alpha_to_coverage_and_one    flags/debug_shadow_splits    nodes/vertex/0/position    nodes/vertex/connections    nodes/fragment/0/position    nodes/fragment/2/node    nodes/fragment/2/position    nodes/fragment/3/node    nodes/fragment/3/position    nodes/fragment/4/node    nodes/fragment/4/position    nodes/fragment/5/node    nodes/fragment/5/position    nodes/fragment/7/node    nodes/fragment/7/position    nodes/fragment/9/node    nodes/fragment/9/position    nodes/fragment/11/node    nodes/fragment/11/position    nodes/fragment/13/node    nodes/fragment/13/position    nodes/fragment/17/node    nodes/fragment/17/position    nodes/fragment/18/node    nodes/fragment/18/position    nodes/fragment/19/node    nodes/fragment/19/position    nodes/fragment/20/node    nodes/fragment/20/position    nodes/fragment/21/node    nodes/fragment/21/position    nodes/fragment/23/node    nodes/fragment/23/position    nodes/fragment/24/node    nodes/fragment/24/position    nodes/fragment/25/node    nodes/fragment/25/position    nodes/fragment/26/node    nodes/fragment/26/position    nodes/fragment/27/node    nodes/fragment/27/position    nodes/fragment/28/node    nodes/fragment/28/position    nodes/fragment/29/node    nodes/fragment/29/position    nodes/fragment/30/node    nodes/fragment/30/position    nodes/fragment/31/node    nodes/fragment/31/position    nodes/fragment/32/node    nodes/fragment/32/position    nodes/fragment/33/node    nodes/fragment/33/position    nodes/fragment/34/node    nodes/fragment/34/position    nodes/fragment/35/node    nodes/fragment/35/position    nodes/fragment/36/node    nodes/fragment/36/position    nodes/fragment/connections    nodes/light/0/position    nodes/light/connections    nodes/start/0/position    nodes/start/connections    nodes/process/0/position    nodes/process/connections    nodes/collide/0/position    nodes/collide/connections    nodes/start_custom/0/position    nodes/start_custom/connections     nodes/process_custom/0/position !   nodes/process_custom/connections    nodes/sky/0/position    nodes/sky/connections    nodes/fog/0/position    nodes/fog/connections        &   local://VisualShaderNodeTexture_u04x6 �      1   local://VisualShaderNodeTexture2DParameter_xvl8e �      1   local://VisualShaderNodeTexture2DParameter_cxhxo N      &   local://VisualShaderNodeTexture_a5cim �      8   local://VisualShaderNodeTextureParameterTriplanar_ucbqe �      8   local://VisualShaderNodeTextureParameterTriplanar_5vqka F      8   local://VisualShaderNodeTextureParameterTriplanar_0m383 �      8   local://VisualShaderNodeTextureParameterTriplanar_i73gc 	      8   local://VisualShaderNodeTextureParameterTriplanar_dvaw5 o      8   local://VisualShaderNodeTextureParameterTriplanar_0sy36 �      "   local://VisualShaderNodeMix_0sfr8 2      "   local://VisualShaderNodeMix_6m3xh �      "   local://VisualShaderNodeMix_53kyb :      $   local://VisualShaderNodeInput_5nih3 �      $   local://VisualShaderNodeInput_fgckl �      /   local://VisualShaderNodeTransformVecMult_f7nny 5      ,   local://VisualShaderNodeTransformFunc_xtxdu f      '   local://VisualShaderNodeVectorOp_1j107 �      ,   local://VisualShaderNodeVec3Parameter_q3vkb �      "   local://VisualShaderNodeMix_jwm3v 6      -   local://VisualShaderNodeFloatParameter_u76om �      -   local://VisualShaderNodeFloatParameter_676kk +      -   local://VisualShaderNodeFloatParameter_pfl1f �      &   local://VisualShaderNodeFloatOp_7hiv3       -   local://VisualShaderNodeColorParameter_hg3xi F      '   local://VisualShaderNodeVectorOp_mpn0p �      )   local://VisualShaderNodeSmoothStep_bw7um �         local://VisualShader_ecar6 �         VisualShaderNodeTexture                   #   VisualShaderNodeTexture2DParameter    	      
   NormalMap                #   VisualShaderNodeTexture2DParameter    	         GroundMask          VisualShaderNodeTexture                                   *   VisualShaderNodeTextureParameterTriplanar    	         RockAlbedoTriplanar                *   VisualShaderNodeTextureParameterTriplanar    	         RockNormalTriplanar                *   VisualShaderNodeTextureParameterTriplanar    	         RockRoughnessTriplanar       *   VisualShaderNodeTextureParameterTriplanar    	         BaseAlbedoTriplanar                *   VisualShaderNodeTextureParameterTriplanar    	         BaseNormalTriplanar                *   VisualShaderNodeTextureParameterTriplanar    	         BaseRoughnessTriplanar          VisualShaderNodeMix                                              �?  �?  �?            ?   ?   ?                  VisualShaderNodeMix                                              �?  �?  �?            ?   ?   ?                  VisualShaderNodeMix                                              �?  �?  �?            ?   ?   ?                  VisualShaderNodeInput             view_matrix          VisualShaderNodeInput             vertex       !   VisualShaderNodeTransformVecMult             VisualShaderNodeTransformFunc             VisualShaderNodeVectorOp                      VisualShaderNodeVec3Parameter    	         TriPlanerScaleHard                  �?  �?  �?         VisualShaderNodeMix                          ?   ?   ?           �?  �?  �?            ?   ?   ?                  VisualShaderNodeFloatParameter    	         DetailNormalPower                           �?         VisualShaderNodeFloatParameter    	         NormalMapPower                  �@                 �?         VisualShaderNodeFloatParameter    	         RoughnessMult                           �?         VisualShaderNodeFloatOp                      VisualShaderNodeColorParameter    	      	   RockTint                   VisualShaderNodeVectorOp                      VisualShaderNodeSmoothStep             VisualShader :         ^  shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_back, diffuse_lambert, specular_schlick_ggx;

uniform vec3 TriPlanerScaleHard = vec3(1.000000, 1.000000, 1.000000);
uniform sampler2D RockAlbedoTriplanar : source_color;
uniform vec4 RockTint : source_color = vec4(1.000000, 1.000000, 1.000000, 1.000000);
uniform sampler2D BaseAlbedoTriplanar : source_color;
uniform sampler2D GroundMask;
uniform sampler2D RockRoughnessTriplanar;
uniform float RoughnessMult : hint_range(0, 1) = 1;
uniform sampler2D BaseRoughnessTriplanar;
uniform sampler2D NormalMap : hint_normal;
uniform sampler2D RockNormalTriplanar : hint_normal;
uniform sampler2D BaseNormalTriplanar : hint_normal;
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
// Input:23
	mat4 n_out23p0 = VIEW_MATRIX;


// TransformFunc:26
	mat4 n_out26p0 = inverse(n_out23p0);


// Input:24
	vec3 n_out24p0 = VERTEX;


// TransformVectorMult:25
	vec3 n_out25p0 = (n_out26p0 * vec4(n_out24p0, 1.0)).xyz;


// Vector3Parameter:28
	vec3 n_out28p0 = TriPlanerScaleHard;


// VectorOp:27
	vec3 n_out27p0 = n_out25p0 * n_out28p0;


// TextureParameterTriplanar:7
	vec4 n_out7p0 = triplanar_texture(RockAlbedoTriplanar, triplanar_power_normal, n_out27p0);


// ColorParameter:34
	vec4 n_out34p0 = RockTint;


// VectorOp:35
	vec3 n_out35p0 = vec3(n_out7p0.xyz) * vec3(n_out34p0.xyz);


// TextureParameterTriplanar:13
	vec4 n_out13p0 = triplanar_texture(BaseAlbedoTriplanar, triplanar_power_normal, n_out27p0);


	vec4 n_out5p0;
// Texture2D:5
	n_out5p0 = texture(GroundMask, UV);
	float n_out5p1 = n_out5p0.r;


// Mix:19
	vec3 n_out19p0 = mix(n_out35p0, vec3(n_out13p0.xyz), vec3(n_out5p1));


// TextureParameterTriplanar:11
	vec4 n_out11p0 = triplanar_texture(RockRoughnessTriplanar, triplanar_power_normal, n_out27p0);


// FloatParameter:32
	float n_out32p0 = RoughnessMult;


// FloatOp:33
	float n_out33p0 = n_out11p0.x * n_out32p0;


// TextureParameterTriplanar:18
	vec4 n_out18p0 = triplanar_texture(BaseRoughnessTriplanar, triplanar_power_normal, n_out27p0);


// Mix:20
	vec3 n_out20p0 = mix(vec3(n_out33p0), vec3(n_out18p0.xyz), vec3(n_out5p1));


// SmoothStep:36
	float n_in36p0 = 0.00000;
	float n_in36p1 = 1.00000;
	float n_out36p0 = smoothstep(n_in36p0, n_in36p1, n_out20p0.x);


	vec4 n_out2p0;
// Texture2D:2
	n_out2p0 = texture(NormalMap, UV);


// TextureParameterTriplanar:9
	vec4 n_out9p0 = triplanar_texture(RockNormalTriplanar, triplanar_power_normal, n_out27p0);


// TextureParameterTriplanar:17
	vec4 n_out17p0 = triplanar_texture(BaseNormalTriplanar, triplanar_power_normal, n_out27p0);


// Mix:21
	vec3 n_out21p0 = mix(vec3(n_out9p0.xyz), vec3(n_out17p0.xyz), vec3(n_out5p1));


// FloatParameter:30
	float n_out30p0 = DetailNormalPower;


// Mix:29
	vec3 n_out29p0 = mix(vec3(n_out2p0.xyz), n_out21p0, vec3(n_out30p0));


// FloatParameter:31
	float n_out31p0 = NormalMapPower;


// Output:0
	ALBEDO = n_out19p0;
	ROUGHNESS = n_out36p0;
	NORMAL_MAP = n_out29p0;
	NORMAL_MAP_DEPTH = n_out31p0;


}
 3   
    ��D   B4             5   
     ��  D6            7   
     C�  D8            9   
     \�  pC:            ;   
     �  �C<            =   
     R�  ��>            ?   
     R�  �@            A   
     R�  4�B            C   
     ��  /�D            E   
     ��  ��F         	   G   
     ��  ��H         
   I   
      B  ��J            K   
     �A  �BL            M   
      B  4�N            O   
    �'� ���P            Q   
    ��  k�R            S   
     � ���T            U   
    @�  ��V            W   
   {��ą{��X            Y   
    ��  R�Z            [   
     D  �C\            ]   
     p�  �C^            _   
     D  �C`            a   
     *�  4�b            c   
     4�  ��d            e   
     ��  M�f            g   
     �B  %�h            i   
     �C  pBj       �                                                                                                                                                                                                	                                                                                                             	              
           !      "       #             #       	              #                     !       !                     $      $                                   RSRC