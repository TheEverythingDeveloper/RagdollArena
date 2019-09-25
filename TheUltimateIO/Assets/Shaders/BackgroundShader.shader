// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BackgroundShader"
{
	Properties
	{
		_ColorB("ColorB", Color) = (0,0,0,0)
		_ColorA("ColorA", Color) = (0,0,0,0)
		_NoiseMaskCutout("NoiseMaskCutout", 2D) = "white" {}
		_Intensity("Intensity", Range( 0 , 1)) = 0
		_MovementSpeed("MovementSpeed", Range( 0 , 1)) = 0.22
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _Intensity;
		uniform float4 _ColorA;
		uniform float4 _ColorB;
		uniform sampler2D _NoiseMaskCutout;
		uniform float _MovementSpeed;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 appendResult9 = (float4(0.0 , _MovementSpeed , 0.0 , 0.0));
			float2 panner7 = ( _Time.y * appendResult9.xy + i.uv_texcoord);
			float4 lerpResult4 = lerp( _ColorA , _ColorB , tex2D( _NoiseMaskCutout, panner7 ).r);
			o.Emission = ( _Intensity * lerpResult4 ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16700
1927;194;1426;825;2294.455;1043.627;2.871512;True;True
Node;AmplifyShaderEditor.RangedFloatNode;8;-1384.737,430.9345;Float;False;Property;_MovementSpeed;MovementSpeed;4;0;Create;True;0;0;False;0;0.22;0.032;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;9;-1077.812,410.7175;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleTimeNode;10;-990.8734,550.5964;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;12;-1069.303,246.6002;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;7;-778.9134,323.6072;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;2;-524.4241,16.27675;Float;False;Property;_ColorB;ColorB;0;0;Create;True;0;0;False;0;0,0,0,0;0.6831183,0.3971609,0.7075472,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;1;-517.3301,-187.6728;Float;False;Property;_ColorA;ColorA;1;0;Create;True;0;0;False;0;0,0,0,0;1,0.6745283,0.9457548,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-575.0723,295.6552;Float;True;Property;_NoiseMaskCutout;NoiseMaskCutout;2;0;Create;True;0;0;False;0;cb321ce47bce83045bd2a4fe82bd0900;b45ed5e119f427e40a5507b492810620;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;4;-103.4404,148.6391;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;5;38.12958,-12.11102;Float;False;Property;_Intensity;Intensity;3;0;Create;True;0;0;False;0;0;0.713;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;346.5332,118.4495;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;529.3062,75.23509;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;BackgroundShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;9;1;8;0
WireConnection;7;0;12;0
WireConnection;7;2;9;0
WireConnection;7;1;10;0
WireConnection;3;1;7;0
WireConnection;4;0;1;0
WireConnection;4;1;2;0
WireConnection;4;2;3;1
WireConnection;6;0;5;0
WireConnection;6;1;4;0
WireConnection;0;2;6;0
ASEEND*/
//CHKSM=7602AF92C3EF0F009484318D731BABAE9041B5B0