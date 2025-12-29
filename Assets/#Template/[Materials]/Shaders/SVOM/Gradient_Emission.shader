Shader "SVOM/Gradient_Emission"
{
	Properties
	{
		[HDR]_TopColor("TopColor", Color) = (1,0.9940535,0)
		[HDR]_BottomColor("BottomColor", Color) = (1,0.5127614,0.2396226)
		_Power_X("Power_X", Float) = 0
		_Power_Y("Power_Y", Float) = 1
		_Power_Z("Power_Z", Float) = 0
		_Power_Sum("Power_Sum", Range( -200 , 200)) = 0
		_Gradient_Strength("Gradient_Strength", Range( 0 , 200)) = 0.5
		[HDR]_ReflectColor("ReflectColor", Color) = (1,1,1)
		_ReflectPower("ReflectPower", Float) = 5
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#define ASE_VERSION 19800
		struct Input
		{
			float4 ase_positionOS4f;
			float3 worldPos;
			float3 worldNormal;
		};

		uniform float3 _BottomColor;
		uniform float3 _TopColor;
		uniform float _Gradient_Strength;
		uniform float _Power_X;
		uniform float _Power_Y;
		uniform float _Power_Z;
		uniform float _Power_Sum;
		uniform float3 _ReflectColor;
		uniform float _ReflectPower;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float4 ase_positionOS4f = v.vertex;
			o.ase_positionOS4f = ase_positionOS4f;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 temp_cast_0 = (_Gradient_Strength).xxxx;
			float3 ase_positionOS = i.ase_positionOS4f.xyz;
			float4 appendResult30 = (float4(( ase_positionOS.x * _Power_X ) , ( ase_positionOS.y * _Power_Y ) , ( ase_positionOS.z * _Power_Z ) , 0.0));
			float4 smoothstepResult14 = smoothstep( float4( 0,0,0,0 ) , temp_cast_0 , ( appendResult30 + _Power_Sum ));
			float3 lerpResult5 = lerp( _BottomColor , _TopColor , smoothstepResult14.xyz);
			float3 ase_positionWS = i.worldPos;
			float3 ase_viewVectorWS = ( _WorldSpaceCameraPos.xyz - ase_positionWS );
			float3 ase_viewDirWS = normalize( ase_viewVectorWS );
			float3 ase_normalWS = i.worldNormal;
			float fresnelNdotV31 = dot( ase_normalWS, ase_viewDirWS );
			float fresnelNode31 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV31, _ReflectPower ) );
			float3 lerpResult33 = lerp( lerpResult5 , _ReflectColor , fresnelNode31);
			o.Albedo = lerpResult33;
			o.Emission = lerpResult33;
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
