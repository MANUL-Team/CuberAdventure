Shader "CuberAdventure/PixelWaterLit"
{
    Properties
    {
        [PerRendererData] _MainTex ("Mask / Sprite", 2D) = "white" {}
        [PerRendererData] _MaskTex ("Light Mask", 2D) = "white" {}
        [PerRendererData] _NormalMap ("Normal Map", 2D) = "bump" {}
        _Color ("Shallow", Color) = (0.25, 0.65, 0.95, 0.55)
        _DeepColor ("Deep", Color) = (0.05, 0.25, 0.5, 0.75)
        _FoamColor ("Foam", Color) = (0.85, 0.95, 1.0, 0.9)
        _PixelSize ("Pixels", Float) = 48
        _NoiseScale ("Noise Scale", Float) = 3.5
        _WaveSpeed ("Wave Speed", Float) = 0.7
        _WaveAmp ("Wave Amp", Range(0, 0.08)) = 0.022
        _WaveFreq ("Wave Freq", Float) = 0.55
        _SurfaceUV ("Surface UV", Range(0, 1)) = 0.75
        _SurfaceSoft ("Surface Soft", Range(0.005, 0.08)) = 0.018
        _FoamAmount ("Foam Amount", Range(0, 1)) = 0.85
        _FoamSoft ("Foam Softness", Range(0.5, 8)) = 2.4
        _FoamGurgle ("Foam Gurgle", Range(0, 1)) = 0.75
        _DepthContrast ("Depth Contrast", Range(0, 1)) = 0.65
        _NormalStrength ("Normal Strength", Range(0, 2)) = 0.9
        _Ripple1 ("Ripple1", Vector) = (0, 0, -999, 0)
        _Ripple2 ("Ripple2", Vector) = (0, 0, -999, 0)
        _Ripple3 ("Ripple3", Vector) = (0, 0, -999, 0)
        [MaterialToggle] _ZWrite ("ZWrite", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        Cull Off
        ZWrite [_ZWrite]

        Pass
        {
            Tags { "LightMode"="Universal2D" }
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"
            #pragma vertex LitVertex
            #pragma fragment LitFragment
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/ShapeLightShared.hlsl"
            #pragma multi_compile_instancing
            #pragma multi_compile _ DEBUG_DISPLAY
            #pragma multi_compile _ SKINNED_SPRITE

            struct Attributes
            {
                COMMON_2D_INPUTS
                half4 color : COLOR;
                UNITY_SKINNED_VERTEX_INPUTS
            };

            struct Varyings
            {
                COMMON_2D_LIT_OUTPUTS
                half4 color : COLOR;
                float3 worldPos : TEXCOORD4;
            };

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_MaskTex); SAMPLER(sampler_MaskTex);

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                half4 _DeepColor;
                half4 _FoamColor;
                float _PixelSize;
                float _NoiseScale;
                float _WaveSpeed;
                float _WaveAmp;
                float _WaveFreq;
                float _SurfaceUV;
                float _SurfaceSoft;
                float _FoamAmount;
                float _FoamSoft;
                float _FoamGurgle;
                float _DepthContrast;
                float _NormalStrength;
                float4 _Ripple1;
                float4 _Ripple2;
                float4 _Ripple3;
            CBUFFER_END

            float Hash21(float2 p)
            {
                p = frac(p * float2(127.1, 311.7));
                p += dot(p, p + 19.19);
                return frac(p.x * p.y);
            }

            float ValueNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float a = Hash21(i);
                float b = Hash21(i + float2(1, 0));
                float c = Hash21(i + float2(0, 1));
                float d = Hash21(i + float2(1, 1));
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            float FBM(float2 p)
            {
                float v = 0.0;
                float a = 0.5;
                [unroll] for (int i = 0; i < 4; i++)
                {
                    v += ValueNoise(p) * a;
                    p = p * 2.05 + float2(17.1, 9.3);
                    a *= 0.5;
                }
                return v;
            }

            float2 Pix(float2 uv, float pixels)
            {
                return floor(uv * pixels) / pixels;
            }

            // World-space multi-frequency wave (amplitude ~1)
            float SurfaceWave(float worldX, float t)
            {
                float w =
                    sin(worldX * _WaveFreq + t * 1.15) * 0.55 +
                    sin(worldX * (_WaveFreq * 1.85) - t * 1.4) * 0.28 +
                    sin(worldX * (_WaveFreq * 0.42) + t * 0.6) * 0.17;
                w += (FBM(float2(worldX * 0.12, t * 0.22)) - 0.5) * 0.3;
                return w;
            }

            float RippleFoam(float2 worldXY, float4 rip)
            {
                // rip.xy = origin, rip.z = birth time, rip.w = strength
                float age = _Time.y - rip.z;
                if (age < 0.0 || age > 2.4 || rip.w < 0.01)
                    return 0.0;
                float radius = age * 3.2;
                float d = abs(length(worldXY - rip.xy) - radius);
                float ring = exp(-d * d * 9.0);
                float fade = 1.0 - saturate(age / 2.4);
                return ring * fade * rip.w;
            }

            // Body look preserved; free surface is a wavy cut + foam at buoyancy line
            void SampleWater(float2 uv, float3 worldPos, out half3 albedo, out half alpha, out half3 normalTS)
            {
                float t = _Time.y * _WaveSpeed;
                float2 wuv = worldPos.xy * (_NoiseScale * 0.15);
                float2 puv = Pix(wuv + float2(t * 0.12, t * 0.03), _PixelSize * 0.35);
                float2 puv2 = Pix(wuv * 1.7 + float2(-t * 0.1, t * 0.08), _PixelSize * 0.5);

                float n = FBM(puv);
                float n2 = FBM(puv2 + 3.7);
                float cells = ValueNoise(puv * 2.2);

                // Depth relative to free surface (not quad top)
                float wave = SurfaceWave(worldPos.x, t);
                float surface = _SurfaceUV + wave * _WaveAmp;
                float below = surface - uv.y; // >0 under water

                float depth = saturate((surface - uv.y) / max(surface, 0.05) * _DepthContrast + (n - 0.5) * 0.12);
                half3 body = lerp(_Color.rgb, _DeepColor.rgb, depth);
                body = lerp(body, body * 1.06, cells * 0.2);
                body = lerp(body, body * 0.94, n2 * 0.18);

                // Soft free-surface silhouette (no hard line)
                float soft = max(_SurfaceSoft, 0.004);
                float edgeKeep = smoothstep(-soft, soft * 0.35, below);

                // Crest band around the wavy top boundary
                float crest = exp(-abs(below) * (_FoamSoft + 1.2) * 18.0);
                crest *= smoothstep(-soft * 2.0, soft * 0.5, below); // mostly under / on surface

                // Gurgling foam clumps ON the crest only
                float gurgle = FBM(float2(worldPos.x * 0.45 + t * 1.7, t * 2.4));
                float gurgle2 = FBM(float2(worldPos.x * 0.95 - t * 2.2, t * 3.0));
                float clump = smoothstep(0.38, 0.68, gurgle) * smoothstep(0.32, 0.65, gurgle2 + crest * 0.15);
                float foam = crest * clump * _FoamAmount * (0.5 + _FoamGurgle * gurgle);
                foam += crest * smoothstep(0.7, 0.9, gurgle2) * _FoamGurgle * 0.35;

                // Interaction ripples
                float rip =
                    RippleFoam(worldPos.xy, _Ripple1) +
                    RippleFoam(worldPos.xy, _Ripple2) +
                    RippleFoam(worldPos.xy, _Ripple3);
                // Ripples only near free surface
                float surfBand = saturate(1.0 - abs(below) / max(soft * 8.0, 0.04));
                foam = saturate(foam + rip * surfBand * 0.85);

                body = lerp(body, _FoamColor.rgb, foam);
                body.rgb = lerp(body.rgb, lerp(body.rgb, _FoamColor.rgb, 0.22), crest * 0.3 * (1.0 - foam));

                float hL = SurfaceWave(worldPos.x - 0.35, t);
                float hR = SurfaceWave(worldPos.x + 0.35, t);
                float hD = FBM(puv + float2(0, -0.03));
                float hU = FBM(puv + float2(0, 0.03));
                normalTS = normalize(half3((hL - hR) * _NormalStrength * 2.0, (hD - hU) * _NormalStrength, 1.0));

                half maskA = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).a;
                albedo = body;
                alpha = saturate(lerp(_Color.a, _DeepColor.a, depth) + foam * 0.2) * maskA * edgeKeep;
                alpha = min(alpha, 0.85);
            }

            Varyings LitVertex(Attributes input)
            {
                UNITY_SKINNED_VERTEX_COMPUTE(input);
                SetUpSpriteInstanceProperties();
                input.positionOS = UnityFlipSprite(input.positionOS, unity_SpriteProps.xy);

                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.positionCS = TransformObjectToHClip(input.positionOS);
                o.uv = input.uv;
                o.lightingUV = half2(ComputeScreenPos(o.positionCS / o.positionCS.w).xy);
                o.worldPos = TransformObjectToWorld(input.positionOS);
                o.color = input.color * unity_SpriteColor;
                return o;
            }

            half4 LitFragment(Varyings input) : SV_Target
            {
                half3 albedo; half alpha; half3 normalTS;
                SampleWater(input.uv, input.worldPos, albedo, alpha, normalTS);
                albedo *= input.color.rgb;
                alpha *= input.color.a;

                half4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, input.uv);
                SurfaceData2D surfaceData;
                InputData2D inputData;
                InitializeSurfaceData(albedo, alpha, mask, normalTS, surfaceData);
                InitializeInputData(input.uv, input.lightingUV, inputData);
                return CombinedShapeLightShared(surfaceData, inputData);
            }
            ENDHLSL
        }

        Pass
        {
            Tags { "LightMode"="NormalsRendering" }
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"
            #pragma vertex NormalsRenderingVertex
            #pragma fragment NormalsRenderingFragment
            #pragma multi_compile_instancing
            #pragma multi_compile _ SKINNED_SPRITE

            struct Attributes
            {
                COMMON_2D_NORMALS_INPUTS
                float4 color : COLOR;
                UNITY_SKINNED_VERTEX_INPUTS
            };
            struct Varyings
            {
                COMMON_2D_NORMALS_OUTPUTS
                half4 color : COLOR;
            };
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Normals2DCommon.hlsl"
            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                half4 _DeepColor;
                half4 _FoamColor;
                float _PixelSize;
                float _NoiseScale;
                float _WaveSpeed;
                float _WaveAmp;
                float _WaveFreq;
                float _SurfaceUV;
                float _SurfaceSoft;
                float _FoamAmount;
                float _FoamSoft;
                float _FoamGurgle;
                float _DepthContrast;
                float _NormalStrength;
                float4 _Ripple1;
                float4 _Ripple2;
                float4 _Ripple3;
            CBUFFER_END

            Varyings NormalsRenderingVertex(Attributes input)
            {
                UNITY_SKINNED_VERTEX_COMPUTE(input);
                SetUpSpriteInstanceProperties();
                input.positionOS = UnityFlipSprite(input.positionOS, unity_SpriteProps.xy);
                Varyings o = CommonNormalsVertex(input);
                o.color = input.color * _Color * unity_SpriteColor;
                return o;
            }
            half4 NormalsRenderingFragment(Varyings input) : SV_Target
            {
                SetUpSpriteInstanceProperties();
                return CommonNormalsFragment(input, input.color);
            }
            ENDHLSL
        }

        Pass
        {
            Tags { "LightMode"="UniversalForward" "Queue"="Transparent" "RenderType"="Transparent" }
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Core2D.hlsl"
            #pragma vertex UnlitVertex
            #pragma fragment UnlitFragment
            #pragma multi_compile_instancing
            #pragma multi_compile _ DEBUG_DISPLAY SKINNED_SPRITE

            struct Attributes { COMMON_2D_INPUTS half4 color:COLOR; UNITY_SKINNED_VERTEX_INPUTS };
            struct Varyings { COMMON_2D_OUTPUTS half4 color:COLOR; float3 worldPos:TEXCOORD4; };
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/2DCommon.hlsl"
            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                half4 _DeepColor;
                half4 _FoamColor;
                float _PixelSize;
                float _NoiseScale;
                float _WaveSpeed;
                float _WaveAmp;
                float _WaveFreq;
                float _SurfaceUV;
                float _SurfaceSoft;
                float _FoamAmount;
                float _FoamSoft;
                float _FoamGurgle;
                float _DepthContrast;
                float _NormalStrength;
                float4 _Ripple1;
                float4 _Ripple2;
                float4 _Ripple3;
            CBUFFER_END

            float Hash21(float2 p){ p=frac(p*float2(127.1,311.7)); p+=dot(p,p+19.19); return frac(p.x*p.y); }
            float ValueNoise(float2 p){ float2 i=floor(p),f=frac(p); float a=Hash21(i),b=Hash21(i+float2(1,0)),c=Hash21(i+float2(0,1)),d=Hash21(i+float2(1,1)); float2 u=f*f*(3-2*f); return lerp(lerp(a,b,u.x),lerp(c,d,u.x),u.y); }
            float FBM(float2 p){ float v=0,a=0.5; [unroll] for(int i=0;i<3;i++){ v+=ValueNoise(p)*a; p=p*2.05+17.1; a*=0.5;} return v; }
            float SurfaceWave(float worldX, float t)
            {
                return sin(worldX*_WaveFreq+t*1.15)*0.55
                     + sin(worldX*(_WaveFreq*1.85)-t*1.4)*0.28
                     + sin(worldX*(_WaveFreq*0.42)+t*0.6)*0.17;
            }

            Varyings UnlitVertex(Attributes input)
            {
                UNITY_SKINNED_VERTEX_COMPUTE(input);
                SetUpSpriteInstanceProperties();
                input.positionOS = UnityFlipSprite(input.positionOS, unity_SpriteProps.xy);
                Varyings o = CommonUnlitVertex(input);
                o.color = input.color * unity_SpriteColor;
                o.worldPos = TransformObjectToWorld(input.positionOS);
                return o;
            }
            half4 UnlitFragment(Varyings input) : SV_Target
            {
                float t = _Time.y * _WaveSpeed;
                float2 wuv = input.worldPos.xy * (_NoiseScale * 0.15);
                float2 puv = floor((wuv + float2(t*0.12,t*0.03)) * _PixelSize * 0.35) / (_PixelSize * 0.35);
                float n = FBM(puv);
                float wave = SurfaceWave(input.worldPos.x, t);
                float surface = _SurfaceUV + wave * _WaveAmp;
                float below = surface - input.uv.y;
                float soft = max(_SurfaceSoft, 0.004);
                float edgeKeep = smoothstep(-soft, soft*0.35, below);
                float depth = saturate((surface - input.uv.y) / max(surface,0.05) * _DepthContrast + (n-0.5)*0.12);
                half3 rgb = lerp(_Color.rgb, _DeepColor.rgb, depth);
                float crest = exp(-abs(below)*(_FoamSoft+1.2)*18.0) * smoothstep(-soft*2.0, soft*0.5, below);
                float gurgle = FBM(float2(input.worldPos.x*0.45+t*1.7, t*2.4));
                float foam = crest * smoothstep(0.38,0.68,gurgle) * _FoamAmount;
                rgb = lerp(rgb, _FoamColor.rgb, saturate(foam));
                half a = saturate(lerp(_Color.a,_DeepColor.a,depth)+foam*0.2) * SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,input.uv).a * input.color.a * edgeKeep;
                return half4(rgb * input.color.rgb, min(a, 0.85));
            }
            ENDHLSL
        }
    }
}
