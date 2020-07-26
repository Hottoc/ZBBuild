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
		_Wavelength("Wavelength", Float) = 10
		_DepthRampTex("Depth Ramp", 2D) = "white" {}
		_NoiseTex("Noise Texture", 2D) = "white" {}
		_MainTex("Main Texture", 2D) = "white" {}
		_DistortStrength("Distort Strength", float) = 1.0
		
		
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"RenderType" = "Opaque"
			}
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
				fixed4 _Color;
				float4 _EdgeColor;
				float _WaveSpeed;
				float _WaveAmp;

				

				struct vertexInput
				{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float4 texCoord : TEXCOORD1;

				};

				struct vertexOutput
				{
					float4 pos : SV_POSITION;
					float4 texCoord : TEXCOORD0;
					float4 screenPos : TEXCOORD1;
				};
				
				vertexOutput vert(vertexInput input)
				{
					vertexOutput output;

					// convert input to world space
					output.pos = UnityObjectToClipPos(input.vertex);

					float4 normal4 = float4(input.normal, 0.0);
					float3 normal = normalize(mul(normal4, unity_WorldToObject).xyz);

					output.texCoord = ComputeGrabScreenPos(output.pos);
					// apply wave animation
					float noiseSample = tex2Dlod(_NoiseTex, float4(input.texCoord.xy, 0, 0));
					output.pos.y += sin(_Time.y * _WaveSpeed * noiseSample) * _WaveAmp;
					output.pos.x += cos(_Time.x * _WaveSpeed * noiseSample) * _WaveAmp;

					// compute depth
					output.screenPos = ComputeScreenPos(output.pos);

					// texture coordinates 
					output.texCoord = input.texCoord;

					return output;
				}
				

				float4 frag(vertexOutput input) : COLOR
				{
					// apply depth texture
					float4 depthSample = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, input.screenPos);
					float depth = LinearEyeDepth(depthSample).r;

					// create foamline
					float foamLine = 1 - saturate(_DepthFactor * (depth - input.screenPos.w));
					float4 foamRamp = float4(tex2D(_DepthRampTex, float2(foamLine, 0.5)).rgb, 1.0);

					// multiply the edge color by the foam factor to get the edge,

					//Sample Main Texture
					float4 albedo = tex2D(_MainTex, input.texCoord.xy);

					// add color of the water
					float4 col = _Color * foamRamp * albedo;

					return col;
				}
				ENDCG
			}
		}
		//FallBack "Diffuse"
}