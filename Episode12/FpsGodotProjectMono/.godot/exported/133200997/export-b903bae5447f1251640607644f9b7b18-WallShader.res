RSRC                    VisualShader            ��������                                            x      resource_local_to_scene    resource_name    output_port_for_preview    default_input_values    expanded_output_ports    parameter_name 
   qualifier    texture_type    color_default    texture_filter    texture_repeat    texture_source    script    input_name    op_type    hint    min    max    step    default_value_enabled    default_value 	   operator    source    texture    code    graph_offset    mode    modes/blend    modes/depth_draw    modes/cull    modes/diffuse    modes/specular    flags/depth_prepass_alpha    flags/depth_test_disabled    flags/sss_mode_skin    flags/unshaded    flags/wireframe    flags/skip_vertex_transform    flags/world_vertex_coords    flags/ensure_correct_normals    flags/shadows_disabled    flags/ambient_light_disabled    flags/shadow_to_opacity    flags/vertex_lighting    flags/particle_trails    flags/alpha_to_coverage     flags/alpha_to_coverage_and_one    flags/debug_shadow_splits    nodes/vertex/0/position    nodes/vertex/connections    nodes/fragment/0/position    nodes/fragment/8/node    nodes/fragment/8/position    nodes/fragment/9/node    nodes/fragment/9/position    nodes/fragment/10/node    nodes/fragment/10/position    nodes/fragment/11/node    nodes/fragment/11/position    nodes/fragment/12/node    nodes/fragment/12/position    nodes/fragment/13/node    nodes/fragment/13/position    nodes/fragment/15/node    nodes/fragment/15/position    nodes/fragment/17/node    nodes/fragment/17/position    nodes/fragment/18/node    nodes/fragment/18/position    nodes/fragment/19/node    nodes/fragment/19/position    nodes/fragment/20/node    nodes/fragment/20/position    nodes/fragment/22/node    nodes/fragment/22/position    nodes/fragment/23/node    nodes/fragment/23/position    nodes/fragment/24/node    nodes/fragment/24/position    nodes/fragment/28/node    nodes/fragment/28/position    nodes/fragment/29/node    nodes/fragment/29/position    nodes/fragment/30/node    nodes/fragment/30/position    nodes/fragment/31/node    nodes/fragment/31/position    nodes/fragment/32/node    nodes/fragment/32/position    nodes/fragment/33/node    nodes/fragment/33/position    nodes/fragment/34/node    nodes/fragment/34/position    nodes/fragment/35/node    nodes/fragment/35/position    nodes/fragment/36/node    nodes/fragment/36/position    nodes/fragment/37/node    nodes/fragment/37/position    nodes/fragment/38/node    nodes/fragment/38/position    nodes/fragment/39/node    nodes/fragment/39/position    nodes/fragment/connections    nodes/light/0/position    nodes/light/connections    nodes/start/0/position    nodes/start/connections    nodes/process/0/position    nodes/process/connections    nodes/collide/0/position    nodes/collide/connections    nodes/start_custom/0/position    nodes/start_custom/connections     nodes/process_custom/0/position !   nodes/process_custom/connections    nodes/sky/0/position    nodes/sky/connections    nodes/fog/0/position    nodes/fog/connections        8   local://VisualShaderNodeTextureParameterTriplanar_2jw43 [      8   local://VisualShaderNodeTextureParameterTriplanar_8cf5g �      $   local://VisualShaderNodeInput_jhwcg       8   local://VisualShaderNodeTextureParameterTriplanar_c4mis X      8   local://VisualShaderNodeTextureParameterTriplanar_fycde �      8   local://VisualShaderNodeTextureParameterTriplanar_7dmth       8   local://VisualShaderNodeTextureParameterTriplanar_n2aon f      8   local://VisualShaderNodeTextureParameterTriplanar_yacrp �      .   local://VisualShaderNodeVectorDecompose_akw53       -   local://VisualShaderNodeFloatParameter_8s3hu O      &   local://VisualShaderNodeFloatOp_jnv7h �      "   local://VisualShaderNodeMix_4yagj �      "   local://VisualShaderNodeMix_846mi P      "   local://VisualShaderNodeMix_wygok �      )   local://VisualShaderNodeSmoothStep_82hu8 X      .   local://VisualShaderNodeVectorDecompose_2tfqu �      '   local://VisualShaderNodeVectorOp_g20g2       &   local://VisualShaderNodeTexture_pswyf H      1   local://VisualShaderNodeTexture2DParameter_ewvy6 |      "   local://VisualShaderNodeMix_e7vkn �      -   local://VisualShaderNodeFloatParameter_m8wek M      &   local://VisualShaderNodeFloatOp_84wrv �      )   local://VisualShaderNodeSmoothStep_uejb4 �      -   local://VisualShaderNodeFloatParameter_cvjfn       -   local://VisualShaderNodeColorParameter_rcib7 �      '   local://VisualShaderNodeVectorOp_237j4 �         local://VisualShader_g3k1m       *   VisualShaderNodeTextureParameterTriplanar             BandNormal                *   VisualShaderNodeTextureParameterTriplanar             BandRoughness          VisualShaderNodeInput                             color       *   VisualShaderNodeTextureParameterTriplanar             BaseAlbedo                *   VisualShaderNodeTextureParameterTriplanar             BaseNormal                *   VisualShaderNodeTextureParameterTriplanar             BaseRoughness       *   VisualShaderNodeTextureParameterTriplanar             BandAlbedo                *   VisualShaderNodeTextureParameterTriplanar          
   HeightMix                    VisualShaderNodeVectorDecompose             VisualShaderNodeFloatParameter             HeightMixMult          VisualShaderNodeFloatOp                      VisualShaderNodeMix                                              �?  �?  �?            ?   ?   ?                  VisualShaderNodeMix                                              �?  �?  �?            ?   ?   ?                  VisualShaderNodeMix                                              �?  �?  �?            ?   ?   ?                  VisualShaderNodeSmoothStep                                              �?  �?  �?            ?   ?   ?                   VisualShaderNodeVectorDecompose             VisualShaderNodeVectorOp                      VisualShaderNodeTexture                   #   VisualShaderNodeTexture2DParameter             WallNormalMap          VisualShaderNodeMix                                              �?  �?  �?            ?   ?   ?                  VisualShaderNodeFloatParameter             NormalMapTypeBlend                            ?         VisualShaderNodeFloatOp             VisualShaderNodeSmoothStep             VisualShaderNodeFloatParameter             NormalMapPower                  �@                 �?         VisualShaderNodeColorParameter             Tint                   VisualShaderNodeVectorOp                      VisualShader 8         �  shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_back, diffuse_lambert, specular_schlick_ggx;

uniform vec4 Tint : source_color = vec4(1.000000, 1.000000, 1.000000, 1.000000);
uniform sampler2D BandAlbedo : source_color;
uniform sampler2D BaseAlbedo : source_color;
uniform sampler2D HeightMix : source_color;
uniform float HeightMixMult;
uniform sampler2D BandRoughness;
uniform sampler2D BaseRoughness;
uniform sampler2D WallNormalMap;
uniform sampler2D BandNormal : hint_normal;
uniform sampler2D BaseNormal : hint_normal;
uniform float NormalMapTypeBlend : hint_range(0, 1) = 0.5;
uniform float NormalMapPower : hint_range(0, 4) = 1;


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
// ColorParameter:38
	vec4 n_out38p0 = Tint;


// TextureParameterTriplanar:15
	vec4 n_out15p0 = triplanar_texture(BandAlbedo, triplanar_power_normal, triplanar_pos);


// TextureParameterTriplanar:11
	vec4 n_out11p0 = triplanar_texture(BaseAlbedo, triplanar_power_normal, triplanar_pos);


// Input:10
	vec4 n_out10p0 = COLOR;


// TextureParameterTriplanar:17
	vec4 n_out17p0 = triplanar_texture(HeightMix, triplanar_power_normal, triplanar_pos);


// VectorDecompose:18
	float n_out18p0 = vec3(n_out17p0.xyz).x;
	float n_out18p1 = vec3(n_out17p0.xyz).y;
	float n_out18p2 = vec3(n_out17p0.xyz).z;


// FloatParameter:19
	float n_out19p0 = HeightMixMult;


// FloatOp:20
	float n_out20p0 = n_out18p0 * n_out19p0;


// VectorOp:30
	vec3 n_out30p0 = pow(vec3(n_out10p0.xyz), vec3(n_out20p0));


// SmoothStep:28
	vec3 n_in28p0 = vec3(0.00000, 0.00000, 0.00000);
	vec3 n_in28p1 = vec3(1.00000, 1.00000, 1.00000);
	vec3 n_out28p0 = smoothstep(n_in28p0, n_in28p1, n_out30p0);


// VectorDecompose:29
	float n_out29p0 = n_out28p0.x;
	float n_out29p1 = n_out28p0.y;
	float n_out29p2 = n_out28p0.z;


// FloatOp:35
	float n_out35p0 = n_out29p0 + n_out29p2;


// SmoothStep:36
	float n_in36p0 = 0.00000;
	float n_in36p1 = 1.00000;
	float n_out36p0 = smoothstep(n_in36p0, n_in36p1, n_out35p0);


// Mix:22
	vec3 n_out22p0 = mix(vec3(n_out15p0.xyz), vec3(n_out11p0.xyz), vec3(n_out36p0));


// VectorOp:39
	vec3 n_out39p0 = vec3(n_out38p0.xyz) * n_out22p0;


// TextureParameterTriplanar:9
	vec4 n_out9p0 = triplanar_texture(BandRoughness, triplanar_power_normal, triplanar_pos);


// TextureParameterTriplanar:13
	vec4 n_out13p0 = triplanar_texture(BaseRoughness, triplanar_power_normal, triplanar_pos);


// Mix:24
	vec3 n_out24p0 = mix(vec3(n_out9p0.xyz), vec3(n_out13p0.xyz), vec3(n_out36p0));


	vec4 n_out31p0;
// Texture2D:31
	n_out31p0 = texture(WallNormalMap, UV);


// TextureParameterTriplanar:8
	vec4 n_out8p0 = triplanar_texture(BandNormal, triplanar_power_normal, triplanar_pos);


// TextureParameterTriplanar:12
	vec4 n_out12p0 = triplanar_texture(BaseNormal, triplanar_power_normal, triplanar_pos);


// Mix:23
	vec3 n_out23p0 = mix(vec3(n_out8p0.xyz), vec3(n_out12p0.xyz), vec3(n_out36p0));


// FloatParameter:34
	float n_out34p0 = NormalMapTypeBlend;


// Mix:33
	vec3 n_out33p0 = mix(vec3(n_out31p0.xyz), n_out23p0, vec3(n_out34p0));


// FloatParameter:37
	float n_out37p0 = NormalMapPower;


// Output:0
	ALBEDO = n_out39p0;
	ROUGHNESS = n_out24p0.x;
	NORMAL_MAP = n_out33p0;
	NORMAL_MAP_DEPTH = n_out37p0;


}
 2   
    ��D   �3             4   
    ���  HC5            6   
    ���  �C7            8   
    ��  �9            :   
     ��  \D;            <   
     �� ��D=            >   
     ��  �D?            @   
    ���  �A            B   
    ���  �C            D   
     ��  9�E         	   F   
     ��  %�G         
   H   
    ���   �I            J   
     ��  ��K            L   
     ��  �CM            N   
     ��  DO            P   
     ��  ��Q            R   
    ���  ��S            T   
    ���  ��U            V   
     �C  ��W            X   
     p�  �Y            Z   
     �C   �[            \   
     C  �]            ^   
     �  ��_            `   
     H�  Ca            b   
     �C  �Cc            d   
   -".D�J��e            f   
   �ZyD%F�g       t                                                         
                                                                                         !              !      !           	   "       !                                               	                                  #             #      #       $      $             $             $             %           
   &       '              '      '                     RSRC