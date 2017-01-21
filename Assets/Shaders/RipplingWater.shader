Shader "Water/Water"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0

        _Color("Color", Color) = (1.0,1.0,1.0,1.0)
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
                uniform float4 circular_AmpPerSpdStt[MAX_WAVES_CIRCULAR];
                uniform float4 circular_PosDropTsc[MAX_WAVES_CIRCULAR];
                uniform int circular_number = 0;
                #define MAX_WAVES_DIRECTIONAL 5
                uniform float3 directional_AmpPerStt[MAX_WAVES_DIRECTIONAL];
                uniform float2 directional_Velocity[MAX_WAVES_DIRECTIONAL];
                uniform int directional_number = 0;

                uniform float waveDropoffRate = 3.0;
                uniform float waveSharpness = 2.0;

                float4 getHeightAndNormal(float2 worldPos)
                {
                    float waveHeight = 0.0;
                    float3 waveNormal = float3(0.0, 0.0, 0.00001);

                    for (int i = 0; i < circular_number; ++i)
                    {
                        const float amplitude = circular_AmpPerSpdStt[i].x,
                                    period = circular_AmpPerSpdStt[i].y,
                                    speed = circular_AmpPerSpdStt[i].z,
                                    startTime = circular_AmpPerSpdStt[i].w,
                                    dropoff = circular_PosDropTsc[i].z,
                                    timeSinceCutoff = circular_PosDropTsc[i].w;
                        const float2 startPos = circular_PosDropTsc[i].xy;

                        float timeSinceCreated = _Time.y - startTime;
                        float dist = distance(startPos, worldPos);
                        float heightScale = max(0.0, lerp(0.0, 1.0, 1.0 - (dist / dropoff)));
                        heightScale = pow(heightScale, waveDropoffRate);

                        float outerCutoff = period * speed * timeSinceCreated;
                        outerCutoff = max(0.0, (outerCutoff - dist) / outerCutoff);
                        float innerCutoff = period * speed * timeSinceCutoff;
                        innerCutoff = 1.0f - saturate((innerCutoff - dist) / innerCutoff);
                        innerCutoff = pow(innerCutoff, 8.0);

                        float innerVal = (dist / period) + (-timeSinceCreated * speed);
                        float waveScale = amplitude * heightScale * outerCutoff * innerCutoff;

                        float sinVal = sin(innerVal);
                        float mappedSinVal = 0.5 + (0.5 * sinVal);
                        float heightOffset = -1.0 + (2.0 * pow(mappedSinVal, waveSharpness));

                        waveHeight += waveScale * heightOffset;

                        //Did some calc to figure out how to get the slope.
                        float derivative = waveScale * cos(innerVal);


                    }
                    for (int j = 0; j < directional_number; ++j)
                    {
                        const float amplitude = directional_AmpPerStt[i].x,
                                    period = directional_AmpPerStt[i].y,
                                    startTime = directional_AmpPerStt[i].z;

                        float2 flowDir = directional_Velocity[j];
                        float speed = length(flowDir);
                        flowDir /= speed;

                        float timeSinceCreated = startTime - _Time.y;

                        float dist = dot(flowDir, worldPos);
                        float innerVal = (dist / period) + (-timeSinceCreated * speed);
                        float waveScale = amplitude;

                        float heightOffset = sin(innerVal);
                        heightOffset = -1.0 + (2.0 * pow(0.5 + (0.5 * heightOffset),
                                                         waveSharpness));

                        waveHeight += waveScale * heightOffset;
                    }

                    waveNormal = normalize(waveNormal);
                    return float4(waveHeight, waveNormal);
                }

                fixed4 frag(v2f IN) : SV_Target
                {
                    fixed4 texColor = tex2D(_MainTex, IN.uv);
                #if ETC1_EXTERNAL_ALPHA
                    // get the color from an external texture (usecase: Alpha support for ETC1 on android)
                    texColor.a = tex2D(_AlphaTex, uv).r;
                #endif //ETC1_EXTERNAL_ALPHA
                
                    float4 heightAndNormal = getHeightAndNormal(IN.worldPos);

                    float f = 0.5 + (0.5 * heightAndNormal.x);
                    return fixed4(f, f, f, 1.0);//DEBUG

                    return texColor * _Color;
                }
            ENDCG
        }
    }
}
