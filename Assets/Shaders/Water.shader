Shader "Custom Shader/Water"
{
	Properties
	{
		// color of the water
		_Color("Color", Color) = (1, 1, 1, 1)
		// color of the edge effect
		_EdgeColor("Edge Color", Color) = (1, 1, 1, 1)
		// width of the edge effect
		_DepthFactor("Depth Factor", float) = 1.0
		_WaveSpeed("Wave Speed", float) = 1.0
		_WaveAmp("Wave Amp", float) = 0.2
		_DepthRampTex("Depth Ramp", 2D) = "white" {}
		_NoiseTex("Noise Texture", 2D) = "white" {}
		_ExtraHeight("Extra Height", float) = 0.0
		_MainTex("Main Texture", 2D) = "white" {}
		
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
			}
			// Background distortion
			Pass
			{

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "AutoLight.cginc"

				// Properties
				sampler2D _CameraDepthTexture;
				sampler2D _DepthRampTex;
				sampler2D _NoiseTex;
				sampler2D _MainTex;
				float _DepthFactor;
				float4 _Color;
				float4 _EdgeColor;
				float _WaveSpeed;
				float _WaveAmp;
				float _ExtraHeight;
				

				struct vertexInput
				{
					float4 vertex : POSITION;
					float4 texCoord : TEXCOORD1;

				};

				struct vertexOutput
				{
					float4 pos : SV_POSITION;
					float4 texCoord : TEXCOORD0;
					float4 screenPos : TEXCOORD1;
				};

				struct v2f
				{
					V2F_SHADOW_CASTER;
				};
				
				vertexOutput vert(vertexInput input)
				{
					vertexOutput output;

					// convert input to world space
					output.pos = UnityObjectToClipPos(input.vertex);

					// apply wave animation
					float noiseSample = tex2Dlod(_NoiseTex, float4(input.texCoord.xy, 0, 0));
					output.pos.y += sin(_Time * _WaveSpeed * noiseSample) * _WaveAmp;
					output.pos.x += cos(_Time * _WaveSpeed * noiseSample) * _WaveAmp;

					// compute depth (screenPos is a float4)
					output.screenPos = ComputeScreenPos(output.pos);

					output.texCoord = input.texCoord;

					return output;
				}
				

				float4 frag(vertexOutput input) : COLOR
				{
				  float4 depthSample = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, input.screenPos);
				  float depth = LinearEyeDepth(depthSample).r;

				  // apply the DepthFactor to be able to tune at what depth values
				  // the foam line actually starts
				  float foamLine = 1 - saturate(_DepthFactor * (depth - input.screenPos.w));
				  float4 foamRamp = float4(tex2D(_DepthRampTex, float2(foamLine, 0.5)).rgb, 1.0);

				  // multiply the edge color by the foam factor to get the edge,

				  //Sample Main Texture
				  float4 albedo = tex2D(_MainTex, input.texCoord.xy);

				  // then add that to the color of the water
				  //float4 col = _Color + foamLine * _EdgeColor;
				  float4 col = _Color * foamRamp * albedo;

				  return col;
				}
				ENDCG
			}
		}
}