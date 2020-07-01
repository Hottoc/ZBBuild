Shader "Custom Shaders/Water"
{
    Properties
    {
        _Color("Color", Color) = (0,0,0,0)
        _Strength("Strength", Range(0,5)) = 1.0
        _Speed("Speed",Range(-200, 200)) = 100
    }
        SubShader
    {
        Tags
        {
        "RenderType" = "transparent"
        }

        Pass
    {
        Cull Off

        CGPROGRAM

        #pragma vertex vertexFunc

        #pragma fragment fragmentFunc

        float4 _Color;
        float _Strength;
        float _Speed;

        struct vertexInput {
            float4 vertex : POSITION;
        };

        struct vertexOutput
        {
            float4 pos : SV_POSITION;
        };

        vertexOutput vertexFunc(vertexInput IN) {
            vertexOutput o;

            float4 worldPos = mul(unity_ObjectToWorld, IN.vertex);

            float displacement = (cos(worldPos.y) + cos((worldPos.x / 5.0)+ (_Speed * _Time)));

            worldPos.y = worldPos.y + (displacement * _Strength);

            o.pos = mul(UNITY_MATRIX_VP, worldPos);

            return o;
        };

        float4 fragmentFunc(vertexOutput IN) : COLOR {
            return _Color;
        }
        ENDCG
            }
    }
}
