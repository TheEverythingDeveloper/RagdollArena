// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TPShader"
{
	Properties
	{
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_Shorts("_Shorts", 2D) = "white" {}
		_Socks("_Socks", 2D) = "white" {}
		_Shirt("_Shirt", 2D) = "white" {}
		_ColorA("_ColorA", Color) = (0,0,0,0)
		_ColorB("_ColorB", Color) = (0,0,0,0)
		_ColorC("_ColorC", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
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
			float2 uv_texcoord;
		};

		uniform sampler2D _Shirt;
		uniform float4 _Shirt_ST;
		uniform sampler2D _TextureSample0;
		uniform float4 _TextureSample0_ST;
		uniform sampler2D _Shorts;
		uniform float4 _Shorts_ST;
		uniform sampler2D _Socks;
		uniform float4 _Socks_ST;
		uniform float4 _ColorA;
		uniform float4 _ColorB;
		uniform float4 _ColorC;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Shirt = i.uv_texcoord * _Shirt_ST.xy + _Shirt_ST.zw;
			float2 uv_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			float4 tex2DNode1 = tex2D( _TextureSample0, uv_TextureSample0 );
			float2 uv_Shorts = i.uv_texcoord * _Shorts_ST.xy + _Shorts_ST.zw;
			float2 uv_Socks = i.uv_texcoord * _Socks_ST.xy + _Socks_ST.zw;
			float4 break30 = ( ( tex2D( _Shirt, uv_Shirt ) * tex2DNode1.r ) + ( tex2D( _Shorts, uv_Shorts ) * tex2DNode1.g ) + ( tex2D( _Socks, uv_Socks ) * tex2DNode1.b ) );
			o.Albedo = ( ( break30.r * _ColorA ) + ( break30.g * _ColorB ) + ( break30.b * _ColorC ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16700
1927;188;1426;830;1682.391;819.396;2.55988;True;True
Node;AmplifyShaderEditor.SamplerNode;5;-521.442,372.1511;Float;True;Property;_Socks;_Socks;2;0;Create;True;0;0;False;0;None;8bd7d56cf78dd814cb0af546dbe12cd4;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-515.9401,-111.0338;Float;True;Property;_Shirt;_Shirt;3;0;Create;True;0;0;False;0;None;6f67913d30d12ea4cbeadc7b13c3fc36;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-524.8868,125.0708;Float;True;Property;_Shorts;_Shorts;1;0;Create;True;0;0;False;0;None;e25467b0a1a312f46aaa67c6d0006192;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-930.2686,105.7697;Float;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;None;2b6572ca42b86c844bb9e7d81296f71d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-158.7334,361.6669;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-154.1846,114.1468;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-153.0257,-111.2356;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;31;96.27691,134.305;Float;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;15;554.2897,392.3886;Float;False;Property;_ColorC;_ColorC;6;0;Create;True;0;0;False;0;0,0,0,0;1,0.2682647,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;30;312.2907,135.7583;Float;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.ColorNode;13;539.694,-120.2141;Float;False;Property;_ColorA;_ColorA;4;0;Create;True;0;0;False;0;0,0,0,0;1,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;14;536.5636,175.8305;Float;False;Property;_ColorB;_ColorB;5;0;Create;True;0;0;False;0;0,0,0,0;1,0,0.5506706,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;33;800.6304,202.5587;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;802.4374,-26.93207;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;786.1744,408.5583;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;35;1001.209,198.9447;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1157.061,29.17611;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;TPShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;9;0;5;0
WireConnection;9;1;1;3
WireConnection;7;0;2;0
WireConnection;7;1;1;2
WireConnection;8;0;3;0
WireConnection;8;1;1;1
WireConnection;31;0;8;0
WireConnection;31;1;7;0
WireConnection;31;2;9;0
WireConnection;30;0;31;0
WireConnection;33;0;30;1
WireConnection;33;1;14;0
WireConnection;34;0;30;0
WireConnection;34;1;13;0
WireConnection;32;0;30;2
WireConnection;32;1;15;0
WireConnection;35;0;34;0
WireConnection;35;1;33;0
WireConnection;35;2;32;0
WireConnection;0;0;35;0
ASEEND*/
//CHKSM=F680D36795A0D0143160BEF57B38BFB72189B83B