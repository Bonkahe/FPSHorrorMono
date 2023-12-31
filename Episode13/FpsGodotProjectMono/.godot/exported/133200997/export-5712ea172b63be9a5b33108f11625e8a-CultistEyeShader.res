RSRC                    VisualShader            ��������                                            R      resource_local_to_scene    resource_name    output_port_for_preview    default_input_values    expanded_output_ports    parameter_name 
   qualifier    default_value_enabled    default_value    script    hint    min    max    step    op_type 	   operator    code    graph_offset    mode    modes/blend    modes/depth_draw    modes/cull    modes/diffuse    modes/specular    flags/depth_prepass_alpha    flags/depth_test_disabled    flags/sss_mode_skin    flags/unshaded    flags/wireframe    flags/skip_vertex_transform    flags/world_vertex_coords    flags/ensure_correct_normals    flags/shadows_disabled    flags/ambient_light_disabled    flags/shadow_to_opacity    flags/vertex_lighting    flags/particle_trails    flags/alpha_to_coverage     flags/alpha_to_coverage_and_one    flags/debug_shadow_splits    nodes/vertex/0/position    nodes/vertex/connections    nodes/fragment/0/position    nodes/fragment/2/node    nodes/fragment/2/position    nodes/fragment/3/node    nodes/fragment/3/position    nodes/fragment/4/node    nodes/fragment/4/position    nodes/fragment/5/node    nodes/fragment/5/position    nodes/fragment/7/node    nodes/fragment/7/position    nodes/fragment/8/node    nodes/fragment/8/position    nodes/fragment/9/node    nodes/fragment/9/position    nodes/fragment/10/node    nodes/fragment/10/position    nodes/fragment/11/node    nodes/fragment/11/position    nodes/fragment/12/node    nodes/fragment/12/position    nodes/fragment/13/node    nodes/fragment/13/position    nodes/fragment/connections    nodes/light/0/position    nodes/light/connections    nodes/start/0/position    nodes/start/connections    nodes/process/0/position    nodes/process/connections    nodes/collide/0/position    nodes/collide/connections    nodes/start_custom/0/position    nodes/start_custom/connections     nodes/process_custom/0/position !   nodes/process_custom/connections    nodes/sky/0/position    nodes/sky/connections    nodes/fog/0/position    nodes/fog/connections        -   local://VisualShaderNodeColorParameter_kmwmf �
      -   local://VisualShaderNodeColorParameter_eianw J      &   local://VisualShaderNodeFresnel_0coay �      -   local://VisualShaderNodeFloatParameter_e17cu �      "   local://VisualShaderNodeMix_avvvr #      &   local://VisualShaderNodeFloatOp_fxxfk �      -   local://VisualShaderNodeFloatParameter_im5eq �      )   local://VisualShaderNodeSmoothStep_n6pfc ?      -   local://VisualShaderNodeColorParameter_r612u j      '   local://VisualShaderNodeVectorOp_iaa13 �      -   local://VisualShaderNodeFloatParameter_jcgbl �         local://VisualShader_765k8 T         VisualShaderNodeColorParameter          	   Emission          	         VisualShaderNodeColorParameter          
   BaseColor          	         VisualShaderNodeFresnel    	         VisualShaderNodeFloatParameter             FresnelPower                  �?	         VisualShaderNodeMix                                                                    ?   ?   ?         	         VisualShaderNodeFloatOp             	         VisualShaderNodeFloatParameter             FresnelSharpness                  �?	         VisualShaderNodeSmoothStep    	         VisualShaderNodeColorParameter             EdgeEmission          	         VisualShaderNodeVectorOp             	         VisualShaderNodeFloatParameter             EmissionPower                  �?	         VisualShader          )  shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_back, diffuse_lambert, specular_schlick_ggx;

uniform vec4 BaseColor : source_color = vec4(1.000000, 1.000000, 1.000000, 1.000000);
uniform vec4 Emission : source_color = vec4(1.000000, 1.000000, 1.000000, 1.000000);
uniform vec4 EdgeEmission : source_color = vec4(1.000000, 1.000000, 1.000000, 1.000000);
uniform float FresnelPower = 1;
uniform float FresnelSharpness = 1;
uniform float EmissionPower = 1;



void fragment() {
// ColorParameter:3
	vec4 n_out3p0 = BaseColor;


// ColorParameter:2
	vec4 n_out2p0 = Emission;


// ColorParameter:11
	vec4 n_out11p0 = EdgeEmission;


// FloatParameter:5
	float n_out5p0 = FresnelPower;


// Fresnel:4
	float n_out4p0 = pow(1.0 - clamp(dot(NORMAL, VIEW), 0.0, 1.0), n_out5p0);


// FloatParameter:9
	float n_out9p0 = FresnelSharpness;


// FloatOp:8
	float n_out8p0 = n_out4p0 * n_out9p0;


// SmoothStep:10
	float n_in10p0 = 0.00000;
	float n_in10p1 = 1.00000;
	float n_out10p0 = smoothstep(n_in10p0, n_in10p1, n_out8p0);


// Mix:7
	vec3 n_out7p0 = mix(vec3(n_out2p0.xyz), vec3(n_out11p0.xyz), vec3(n_out10p0));


// FloatParameter:13
	float n_out13p0 = EmissionPower;


// VectorOp:12
	vec3 n_out12p0 = n_out7p0 * vec3(n_out13p0);


// Output:0
	ALBEDO = vec3(n_out3p0.xyz);
	EMISSION = n_out12p0;


}
 +             ,   
    ���    -            .   
     ��  ��/            0   
     ��  �C1            2   
    ���  �C3            4   
     �  HC5            6   
     9�  �C7            8   
    ���  D9            :   
     �  �C;            <   
    ���  \C=         	   >   
   ,��`nC?         
   @   
     �  DA       ,                                                           	                    
      
                                                                   	      RSRC