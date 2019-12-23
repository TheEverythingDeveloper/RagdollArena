// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TimeBarShader"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.69
		_Alpha("Alpha", 2D) = "white" {}
		_MaskTexture("MaskTexture", 2D) = "white" {}
		_Speed("Speed", Float) = 0
		_Lines("Lines", 2D) = "white" {}
		_LineTexture("LineTexture", 2D) = "white" {}
		_Horizontal("Horizontal", Float) = 1
		_LinesColor("LinesColor", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Lines;
		uniform float _Speed;
		uniform float _Horizontal;
		uniform sampler2D _LineTexture;
		uniform float4 _LineTexture_ST;
		uniform float4 _LinesColor;
		uniform sampler2D _MaskTexture;
		uniform float4 _MaskTexture_ST;
		uniform sampler2D _Alpha;
		uniform float4 _Alpha_ST;
		uniform float _Cutoff = 0.69;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float mulTime14 = _Time.y * _Speed;
			float2 appendResult20 = (float2(_Horizontal , 0.0));
			float2 panner12 = ( sin( mulTime14 ) * appendResult20 + i.uv_texcoord);
			float2 uv_LineTexture = i.uv_texcoord * _LineTexture_ST.xy + _LineTexture_ST.zw;
			o.Emission = ( tex2D( _Lines, panner12 ) + ( tex2D( _LineTexture, uv_LineTexture ) * _LinesColor ) ).rgb;
			o.Alpha = 1;
			float2 uv_MaskTexture = i.uv_texcoord * _MaskTexture_ST.xy + _MaskTexture_ST.zw;
			float2 uv_Alpha = i.uv_texcoord * _Alpha_ST.xy + _Alpha_ST.zw;
			clip( saturate( ( ( tex2D( _MaskTexture, uv_MaskTexture ).a + ( tex2D( _Alpha, uv_Alpha ).a * 1.0 ) ) - 1.0 ) ) - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16700
1927;194;1426;824;763.5259;229.5084;2.362318;True;True
Node;AmplifyShaderEditor.CommentaryNode;70;-345.4396,-784.3611;Float;False;1573.652;640.4629;Background Color;9;67;15;19;14;20;54;13;12;16;;0.2122642,0.6861529,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-261.3736,-392.3023;Float;False;Property;_Speed;Speed;3;0;Create;True;0;0;False;0;0;-0.17;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;10;-394.2719,376.8338;Float;False;1633.1;696.144;OpacityMask;7;55;49;48;47;36;8;2;;1,0.5333334,0.9741055,1;0;0
Node;AmplifyShaderEditor.SamplerNode;2;-273.0681,596.7725;Float;True;Property;_Alpha;Alpha;1;0;Create;True;0;0;False;0;None;7b6e177da34b211479a676179f6ee59b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;19;-295.4396,-572.3974;Float;False;Property;_Horizontal;Horizontal;6;0;Create;True;0;0;False;0;1;8.7;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;14;-62.13335,-392.0221;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;55;0.9712722,893.3758;Float;False;Constant;_Float0;Float 0;7;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;20;-48.43948,-606.3973;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;13;-168.2017,-734.3611;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SinOpNode;54;123.9488,-396.8983;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;36;84.85732,420.9125;Float;True;Property;_MaskTexture;MaskTexture;2;0;Create;True;0;0;False;0;None;7b6e177da34b211479a676179f6ee59b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;258.6809,736.8389;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;71;239.3438,-115.0408;Float;False;744.7101;463.7821;Lines;3;68;65;69;;0.6138485,0.4719206,0.990566,1;0;0
Node;AmplifyShaderEditor.SamplerNode;65;289.3438,-65.04077;Float;True;Property;_LineTexture;LineTexture;5;0;Create;True;0;0;False;0;None;697e00e90bfbb194d864b5d03137ca4d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;47;551.2598,647.5646;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;69;380.4395,135.7413;Float;False;Property;_LinesColor;LinesColor;7;0;Create;True;0;0;False;0;0,0,0,0;0.1509434,0.1509434,0.1509434,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;12;357.4973,-536.9515;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;16;631.1924,-530.626;Float;True;Property;_Lines;Lines;4;0;Create;True;0;0;False;0;None;006edaacf9cfcba4394f76cd37642ae6;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;48;761.5728,811.5348;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;754.054,79.8613;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;67;993.2122,-490.9215;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;49;1029.891,765.2937;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1351.041,-102.4251;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;TimeBarShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.69;True;True;0;True;TransparentCutout;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;14;0;15;0
WireConnection;20;0;19;0
WireConnection;54;0;14;0
WireConnection;8;0;2;4
WireConnection;8;1;55;0
WireConnection;47;0;36;4
WireConnection;47;1;8;0
WireConnection;12;0;13;0
WireConnection;12;2;20;0
WireConnection;12;1;54;0
WireConnection;16;1;12;0
WireConnection;48;0;47;0
WireConnection;48;1;55;0
WireConnection;68;0;65;0
WireConnection;68;1;69;0
WireConnection;67;0;16;0
WireConnection;67;1;68;0
WireConnection;49;0;48;0
WireConnection;0;2;67;0
WireConnection;0;10;49;0
ASEEND*/
//CHKSM=669E0BF759D398CE9BD9BA0D36D964C031748328