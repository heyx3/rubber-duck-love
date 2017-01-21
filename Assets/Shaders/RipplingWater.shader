Shader "Water/Water"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0

        _Color("Color", Color) = (1.0,1.0,1.0,0.0)
    }

        SubShader
        {
            Tags
            {
                "IgnoreProjector" = "True"
                "RenderType" = "Opaque"
                "PreviewType" = "Plane"
                "CanUseSpriteAtlas" = "False"
            }

            Cull Off
            Lighting Off
            ZWrite Off

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 2.0
                #pragma multi_compile _ PIXELSNAP_ON
                #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
                #include "UnityCG.cginc"

                struct appdata_t
                {
                    float4 vertex   : POSITION;
                    float2 texcoord : TEXCOORD0;
                };

                struct v2f
                {
                    float4 vertex   : SV_POSITION;
                    float uv        : TEXCOORD0;
                    float2 worldPos : TEXCOORD1;
                };

                v2f vert(appdata_t IN)
                {
                    v2f OUT;
                    OUT.vertex = UnityObjectToClipPos(IN.vertex);
                    OUT.uv = IN.texcoord;

                    #ifdef PIXELSNAP_ON
                    OUT.vertex = UnityPixelSnap(OUT.vertex);
                    #endif

                    OUT.worldPos = mul(unity_ObjectToWorld, IN.vertex).xy;

                    return OUT;
                }

                sampler2D _MainTex;
                sampler2D _AlphaTex;

                float4 _MainTex_TexelSize;
                float4 _Color;
                
                #define MAX_WAVES_CIRCULAR 5
                uniform float circular_amplitude[MAX_WAVES_CIRCULAR];
                uniform float circular_period[MAX_WAVES_CIRCULAR];
                uniform float circular_speed[MAX_WAVES_CIRCULAR];
                uniform float circular_startTime[MAX_WAVES_CIRCULAR];
                uniform float circular_dropoff[MAX_WAVES_CIRCULAR];
                uniform float2 circular_startWorldPos[MAX_WAVES_CIRCULAR];
                uniform int circular_number = 0;
                #define MAX_WAVES_DIRECTIONAL 5
                uniform float directional_amplitude[MAX_WAVES_DIRECTIONAL];
                uniform float directional_period[MAX_WAVES_DIRECTIONAL];
                uniform float directional_startTime[MAX_WAVES_DIRECTIONAL];
                uniform float2 directional_velocity[MAX_WAVES_DIRECTIONAL];
                uniform int directional_number = 0;

                uniform float waveDropoffRate = 3.0;
                uniform float waveSharpness = 2.0;

                float getHeight(float2 worldPos)
                {
                    float waveHeight = 0.0;
                    for (int i = 0; i < circular_number; ++i)
                    {
                        float timeSinceCreated = _Time.y - circular_startTime[i];
                        float dist = distance(circular_startWorldPos[i], worldPos);
                        float heightScale = max(0.0, lerp(0.0, 1.0, 1.0 - (dist / circular_dropoff[i])));
                        heightScale = pow(heightScale, waveDropoffRate);

                        float cutoff = circular_period[i] * circular_speed[i] * timeSinceCreated;
                        cutoff = max(0.0, (cutoff - dist) / cutoff);

                        float innerVal = (dist / circular_period[i]) + (-timeSinceCreated * circular_speed[i]);
                        float waveScale = circular_amplitude[i] * heightScale * cutoff;

                        float heightOffset = sin(innerVal);
                        heightOffset = -1.0 + (2.0 * pow(0.5 + (0.5 * heightOffset),
                                                         waveSharpness));

                        waveHeight += waveScale * heightOffset;
                    }
                    for (int j = 0; j < directional_number; ++j)
                    {
                        float2 flowDir = directional_velocity[j];
                        float speed = length(flowDir);
                        flowDir /= speed;

                        const float amplitude = directional_amplitude[j];
                        const float period = directional_period[j];
                        float timeSinceCreated = directional_startTime[j] - _Time.y;

                        float dist = dot(flowDir, worldPos);

                        float innerVal = (dist / period) + (-timeSinceCreated * speed);
                        float waveScale = amplitude;

                        float heightOffset = sin(innerVal);
                        heightOffset = -1.0 + (2.0 * pow(0.5 + (0.5 * heightOffset),
                                                         waveSharpness));

                        waveHeight += waveScale * heightOffset;
                    }
                    return waveHeight;
                }
                float3 getNormal(float2 worldPos)
                {
                    //TODO: Implement. Also implement in Water::Sample()!
                    return float3(0.0, 0.0, 1.0);
                }

                fixed4 frag(v2f IN) : SV_Target
                {
                    fixed4 texColor = tex2D(_MainTex, IN.uv);
                #if ETC1_EXTERNAL_ALPHA
                    // get the color from an external texture (usecase: Alpha support for ETC1 on android)
                    texColor.a = tex2D(_AlphaTex, uv).r;
                #endif //ETC1_EXTERNAL_ALPHA
                
                    float height = getHeight(IN.worldPos);

                    float f = 0.5 + (0.5 * height);
                    return fixed4(f, f, f, 1.0);//DEBUG

                    return texColor * _Color;
                }
            ENDCG
        }
    }
}
