//Shader that colors the surface of a quad using water ripples
//    based on the world-space position of the fragments.
//The ripples are controlled by the Water script.
//There are two types of waves:
//    * "Circular": rippling outwards from a point
//    * "Directional": rippling along a direction
//Note that Directional waves are messed up right now; don't use them yet.

Shader "Water/Water"
{
    Properties
    {
        _MainTex("Diffuse Tex", 2D) = "white" {}
        _Color("Tint", Color) = (1.0,1.0,1.0,1.0)

        _Ambient("Ambient light", Float) = 0.3
        _Diffuse("Diffuse light", Float) = 0.7
        _Specular("Specular light", Vector) = (10,10,10,10)
        _SpecIntensity("Specular intensity", Float) = 64.0

        _RippleCenterSize("Ripple center size", Float) = 0.2
    }

        SubShader
        {
            Tags
            {
                "RenderType" = "Opaque"
                "PreviewType" = "Plane"
            }

            Cull Off
            Lighting Off
            ZWrite Off
            Blend One Zero

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 2.0
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

                    OUT.worldPos = mul(unity_ObjectToWorld, IN.vertex).xy;

                    return OUT;
                }

                sampler2D _MainTex;

                float4 _Color;
                
                //Pack the wave data into vectors to reduce the number of uniforms.
                #define MAX_WAVES_CIRCULAR 5
                uniform float4 circular_AmpPerSpdStt[MAX_WAVES_CIRCULAR];
                uniform float4 circular_PosDropTsc[MAX_WAVES_CIRCULAR];
                uniform int circular_number = 0;
                #define MAX_WAVES_DIRECTIONAL 5
                uniform float3 directional_AmpPerStt[MAX_WAVES_DIRECTIONAL];
                uniform float2 directional_Velocity[MAX_WAVES_DIRECTIONAL];
                uniform int directional_number = 0;

                float _RippleCenterSize;

                uniform float waveDropoffRate = 3.0;
                uniform float waveSharpness = 2.0;

                uniform float3 lightDir = float3(0.57735, 0.57735, -0.57735);
                float _Ambient, _Diffuse, _SpecIntensity;
                float3 _Specular;


                float getHeight(float2 worldPos)
                {
                    float waveHeight = 0.0;

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

                        float2 toCenter = (startPos - worldPos);
                        float dist = length(toCenter);
                        toCenter /= dist;

                        float constrainedDist = max(_RippleCenterSize, dist);

                        float heightScale = max(0.0, lerp(0.0, 1.0, 1.0 - (constrainedDist / dropoff)));
                        heightScale = pow(heightScale, waveDropoffRate);

                        float outerCutoff = period * speed * timeSinceCreated;
                        outerCutoff = max(0.0, (outerCutoff - constrainedDist) / outerCutoff);
                        float innerCutoff = period * speed * timeSinceCutoff;
                        innerCutoff = 1.0f - saturate((innerCutoff - constrainedDist) / innerCutoff);
                        innerCutoff = pow(innerCutoff, 8.0);

                        float innerVal = (constrainedDist / period) + (-timeSinceCreated * speed);
                        float waveScale = amplitude * heightScale * outerCutoff * innerCutoff;

                        float sinVal = sin(innerVal);
                        float mappedSinVal = 0.5 + (0.5 * sinVal);
                        float heightOffset = -1.0 + (2.0 * pow(mappedSinVal, waveSharpness));

                        waveHeight += waveScale * heightOffset;
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

                        float sinVal = sin(innerVal);
                        float mappedSinVal = 0.5 + (0.5 * sinVal);
                        float heightOffset = -1.0 + (2.0 * pow(mappedSinVal, waveSharpness));

                        waveHeight += amplitude * heightOffset;
                    }

                    return waveHeight;
                }
                float3 getNormal(float2 worldPos)
                {
                    const float3 epsilon = float3(0.0001, -0.0001, 0.0);

                    //Get the height at nearby fragments and compute the normal via cross-product.
                    
                    float2 one_zero = worldPos + epsilon.xz,
                           nOne_zero = worldPos + epsilon.yz,
                           zero_one = worldPos + epsilon.zx,
                           zero_nOne = worldPos + epsilon.zy;

                    float3 p_zero_zero = float3(worldPos, getHeight(worldPos)),
                           p_one_zero = float3(one_zero, getHeight(one_zero)),
                           p_nOne_zero = float3(nOne_zero, getHeight(nOne_zero)),
                           p_zero_one = float3(zero_one, getHeight(zero_one)),
                           p_zero_nOne = float3(zero_nOne, getHeight(zero_nOne));

                    float3 norm1 = cross(normalize(p_one_zero - p_zero_zero),
                                         normalize(p_zero_one - p_zero_zero)),
                           norm2 = cross(normalize(p_nOne_zero - p_zero_zero),
                                         normalize(p_zero_nOne - p_zero_zero)),
                           normFinal = normalize((norm1 * sign(norm1.z)) +
                                                 (norm2 * sign(norm2.z)));

                    return normFinal;
                }

                float3 getBrightness(float3 normal)
                {
                    float brightness = _Ambient;
                    
                    //Diffuse.
                    float dotNorm = max(0.0, dot(normal, -lightDir));
                    brightness += _Diffuse * dotNorm;

                    //Specular.
                    //Use Blinn half angle modification for performance at the cost of accuracy.
                    //Keep in mind we're using an ortho projection.
                    float3 h = normalize(float3(0.0, 0.0, -1.0) - lightDir);
                    float3 spec = _Specular * pow(saturate(dot(h, normal)), _SpecIntensity);

                    return float3(brightness, brightness, brightness) + spec;
                }


                fixed4 frag(v2f IN) : SV_Target
                {
                    float2 screenUV = IN.vertex.xy / _ScreenParams.xy;
                    fixed4 texColor = tex2D(_MainTex, screenUV);
                
                    float height = getHeight(IN.worldPos);
                    float3 normal = getNormal(IN.worldPos);
                    float3 brightness = getBrightness(normal);

                    return texColor * _Color * float4(brightness, 1.0);
                }
            ENDCG
        }
    }
}
