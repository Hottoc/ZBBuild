Shader "Custom Shaders/Simple Shader"
{
	SubShader{

		Pass {
			CGPROGRAM
			#pragma vertex MyVertexProgram
			#pragma fragment MyFragmentProgram
			#include "UnityCG.cginc"
			float4 MyVertexProgram() : SV_POSITION {
				return 0;
			}

			float4 MyFragmentProgram(float4 position : SV_POSITION) : SV_TARGET {
				return 0;
			}
			ENDCG
		}
	}
}
