// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ConstructionPlanShader"
{
	Properties
	{
		_MainColor("MainColor", Color) = (0.9339623,0.3304112,0.3304112,0)
		_FresnelColor("FresnelColor", Color) = (0.4528302,0.4528302,0.4528302,0)
		_Bias("Bias", Float) = -1
		_Scale("Scale", Float) = 1.5
		_Power("Power", Float) = 3
		_Timescale("Timescale", Float) = 1
		_AmountOfLines("AmountOfLines", Float) = 1
		_LineWidth("LineWidth", Range( 0 , 2)) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
		};

		uniform float4 _MainColor;
		uniform float4 _FresnelColor;
		uniform float _Bias;
		uniform float _Scale;
		uniform float _Power;
		uniform float _LineWidth;
		uniform float _AmountOfLines;
		uniform float _Timescale;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV6 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode6 = ( _Bias + _Scale * pow( 1.0 - fresnelNdotV6, _Power ) );
			float temp_output_7_0 = saturate( fresnelNode6 );
			float4 lerpResult3 = lerp( _MainColor , _FresnelColor , temp_output_7_0);
			o.Albedo = saturate( lerpResult3 ).rgb;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float mulTime11 = _Time.y * _Timescale;
			o.Alpha = ( _LineWidth + ( temp_output_7_0 * saturate( sin( ( ( ase_vertex3Pos.y * _AmountOfLines ) + mulTime11 ) ) ) ) );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float3 worldPos : TEXCOORD1;
				float3 worldNormal : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16700
1955;13;1350;802;1507.737;825.9363;2.10643;True;True
Node;AmplifyShaderEditor.PosVertexDataNode;2;-707.6531,50.07791;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;12;-645.0328,373.4038;Float;False;Property;_Timescale;Timescale;5;0;Create;True;0;0;False;0;1;3.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-780.2332,210.9039;Float;False;Property;_AmountOfLines;AmountOfLines;6;0;Create;True;0;0;False;0;1;191.21;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;11;-479.9334,379.903;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-522.8339,195.3036;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;13;-297.9339,240.8034;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-887.406,-405.0876;Float;False;Property;_Bias;Bias;2;0;Create;True;0;0;False;0;-1;0.68;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-888.7224,-302.4065;Float;False;Property;_Scale;Scale;3;0;Create;True;0;0;False;0;1.5;3.97;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-881.8372,-213.0805;Float;False;Property;_Power;Power;4;0;Create;True;0;0;False;0;3;3.06;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;14;-144.5332,234.3036;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;6;-705.5414,-302.2655;Float;False;Standard;WorldNormal;ViewDir;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;-1.18;False;2;FLOAT;1.65;False;3;FLOAT;2.83;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;4;-410.4928,-556.34;Float;False;Property;_MainColor;MainColor;0;0;Create;True;0;0;False;0;0.9339623,0.3304112,0.3304112,0;0.108933,0,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;22;36.25731,223.9242;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;5;-406.9597,-383.7762;Float;False;Property;_FresnelColor;FresnelColor;1;0;Create;True;0;0;False;0;0.4528302,0.4528302,0.4528302,0;0.1792453,0.1792453,0.1792453,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;7;-324.7135,-182.5164;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;3;-103.8955,-365.4506;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;199.7497,129.9948;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;28.00906,2.405359;Float;False;Property;_LineWidth;LineWidth;7;0;Create;True;0;0;False;0;0;0.741;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;21;234.2202,-233.0764;Float;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;19;341.7968,63.99213;Float;False;2;2;0;FLOAT;0.4;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;511.1641,-137.5778;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;ConstructionPlanShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;11;0;12;0
WireConnection;15;0;2;2
WireConnection;15;1;16;0
WireConnection;13;0;15;0
WireConnection;13;1;11;0
WireConnection;14;0;13;0
WireConnection;6;1;8;0
WireConnection;6;2;9;0
WireConnection;6;3;10;0
WireConnection;22;0;14;0
WireConnection;7;0;6;0
WireConnection;3;0;4;0
WireConnection;3;1;5;0
WireConnection;3;2;7;0
WireConnection;17;0;7;0
WireConnection;17;1;22;0
WireConnection;21;0;3;0
WireConnection;19;0;20;0
WireConnection;19;1;17;0
WireConnection;0;0;21;0
WireConnection;0;9;19;0
ASEEND*/
//CHKSM=6B08F1625839A0FA36EC465A0319630194CA0951