Shader "SVOM/Gradient/Emission/Smooth"
{
	Properties
	{
		[HDR]_TopColor("TopColor", Color) = (0,0.9555726,1)
		[HDR]_BottomColor("BottomColor", Color) = (0.1803922,0.3176471,0.7490196)
		_Split_Pos("Split_Pos", Vector) = (1,1,1,0)
		_Split_Normal_Vector("Split_Normal_Vector", Vector) = (1,1,1,0)
		_Gradient_Power("Gradient_Power", Float) = 2
		[HDR]_Reflect_Color("Reflect_Color", Color) = (4,4,4)
		_Reflect_Power("Reflect_Power", Float) = 5
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.5
		#define ASE_VERSION 19800
		struct Input
		{
			float4 ase_positionOS4f;
			float3 worldPos;
			float3 worldNormal;
		};

		uniform float3 _TopColor;
		uniform float3 _BottomColor;
		uniform float _Gradient_Power;
		uniform float3 _Split_Pos;
		uniform float3 _Split_Normal_Vector;
		uniform float3 _Reflect_Color;
		uniform float _Reflect_Power;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float4 ase_positionOS4f = v.vertex;
			o.ase_positionOS4f = ase_positionOS4f;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_positionOS = i.ase_positionOS4f.xyz;
			float dotResult78 = dot( ( _Split_Pos - ase_positionOS ) , _Split_Normal_Vector );
			float ifLocalVar76 = 0;
			if( dotResult78 <= 0.0 )
				ifLocalVar76 = 0.0;
			else
				ifLocalVar76 = ( dotResult78 / length( _Split_Normal_Vector ) );
			float smoothstepResult14 = smoothstep( 0.0 , _Gradient_Power , ifLocalVar76);
			float3 lerpResult5 = lerp( _TopColor , _BottomColor , smoothstepResult14);
			float3 ase_positionWS = i.worldPos;
			float3 ase_viewVectorWS = ( _WorldSpaceCameraPos.xyz - ase_positionWS );
			float3 ase_viewDirWS = normalize( ase_viewVectorWS );
			float3 ase_normalWS = i.worldNormal;
			float fresnelNdotV86 = dot( ase_normalWS, ase_viewDirWS );
			float fresnelNode86 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV86, _Reflect_Power ) );
			float3 temp_output_89_0 = ( lerpResult5 + ( _Reflect_Color * fresnelNode86 ) );
			o.Albedo = temp_output_89_0;
			o.Emission = temp_output_89_0;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.5
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
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float4 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.customPack1.xyzw = customInputData.ase_positionOS4f;
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
				surfIN.ase_positionOS4f = IN.customPack1.xyzw;
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
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
