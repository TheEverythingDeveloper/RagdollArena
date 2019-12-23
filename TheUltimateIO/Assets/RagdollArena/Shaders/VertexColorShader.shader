// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "VertexColorShader"
{
	Properties
	{
		_RedColor("RedColor", Color) = (0,0,0,0)
		_GreenColor("GreenColor", Color) = (0,0,0,0)
		_BlueColor("BlueColor", Color) = (0,0,0,0)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float4 vertexColor : COLOR;
		};

		uniform float4 _RedColor;
		uniform float4 _GreenColor;
		uniform float4 _BlueColor;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Albedo = ( ( _RedColor * i.vertexColor.r ) + ( _GreenColor * i.vertexColor.g ) + ( i.vertexColor.b * _BlueColor ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16700
1927;188;1426;830;678.8698;186.0486;1;True;True
Node;AmplifyShaderEditor.ColorNode;5;-602.0999,527.2;Float;False;Property;_BlueColor;BlueColor;2;0;Create;True;0;0;False;0;0,0,0,0;0.1084906,1,0.3937735,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;4;-602.6005,320.9998;Float;False;Property;_GreenColor;GreenColor;1;0;Create;True;0;0;False;0;0,0,0,0;0.3160377,0.9315008,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;1;-594.0001,-111.4;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;3;-605.9002,108.4;Float;False;Property;_RedColor;RedColor;0;0;Create;True;0;0;False;0;0,0,0,0;0.3207547,0.2072802,0.2791589,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2;-351.5997,113.1;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-348.7998,323.1002;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-350.7,517.8;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;9;-104.8413,114.3678;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;257.2,115.2;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;VertexColorShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;2;0;3;0
WireConnection;2;1;1;1
WireConnection;6;0;4;0
WireConnection;6;1;1;2
WireConnection;8;0;1;3
WireConnection;8;1;5;0
WireConnection;9;0;2;0
WireConnection;9;1;6;0
WireConnection;9;2;8;0
WireConnection;0;0;9;0
ASEEND*/
//CHKSM=2D2B1CE929B3CE4D4F0FA7D393B878D8FD1EA990