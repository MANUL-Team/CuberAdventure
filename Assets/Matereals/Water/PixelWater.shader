Shader "CuberAdventure/PixelWaterLit"
{
    Properties
    {
        [PerRendererData] _MainTex ("Mask / Sprite", 2D) = "white" {}
        [PerRendererData] _MaskTex ("Light Mask", 2D) = "white" {}
        [PerRendererData] _NormalMap ("Normal Map", 2D) = "bump" {}
        _Color ("Shallow", Color) = (0.22, 0.58, 0.92, 0.5)
        _DeepColor ("Deep", Color) = (0.04, 0.2, 0.45, 0.72)
        _FoamColor ("Foam", Color) = (0.92, 0.98, 1.0, 1.0)
        _PixelSize ("Pixels", Float) = 48
        _NoiseScale ("Noise Scale", Float) = 3.5
        _WaveSpeed ("Wave Speed", Float) = 1.85
        _WaveAmp ("Wave Height (world)", Range(0.1, 3)) = 0.45
        _WaveFreq ("Wave Freq (per ~70u)", Float) = 14
        _SurfaceUV ("Surface UV", Range(0, 1)) = 1
        _EdgeSoft ("Edge Soft (world)", Range(0.1, 2)) = 0.35
        _FoamWidth ("Foam Width (world)", Range(0.2, 3)) = 0.7
        _FoamAmount ("Foam Amount", Range(0, 1)) = 1
        _FoamGurgle ("Foam Gurgle", Range(0, 1)) = 0.8
        _DepthContrast ("Depth Contrast", Range(0, 1)) = 0.78
        _NormalStrength ("Normal Strength", Range(0, 2)) = 0.9
        _SplashSpeed ("Splash Wave Speed", Float) = 0.7
        _RectSize ("Rect Size", Vector) = (1, 1, 0, 0)
        _Splash1 ("Splash1", Vector) = (0, 0, -999, 0)
        _Splash2 ("Splash2", Vector) = (0, 0, -999, 0)
        _Splash3 ("Splash3", Vector) = (0, 0, -999, 0)
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
                float _EdgeSoft;
                float _FoamWidth;
                float _FoamAmount;
                float _FoamGurgle;
                float _DepthContrast;
                float _NormalStrength;
                float _SplashSpeed;
                float4 _RectSize;
                float4 _Splash1;
                float4 _Splash2;
                float4 _Splash3;
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

            // World-space waves — crest size stays constant; big pools get more crests
            float WaveLength()
            {
                // WaveFreq ≈ how many crests you'd see on a ~70-unit-wide pool
                return max(70.0 / max(_WaveFreq, 0.5), 2.5);
            }

            float SurfaceWave(float worldX, float t)
            {
                float waveLen = WaveLength();
                float phase = worldX / waveLen;
                float w =
                    sin(phase * 6.183 + t * 2.1 + FBM(float2(phase * 0.35, t * 0.15)) * 2.5) * 0.42 +
                    sin(phase * 11.7 - t * 2.85 + 1.7) * 0.22 +
                    sin(phase * 2.45 + t * 1.15) * 0.18;
                float chop = FBM(float2(phase * 0.85 + t * 0.55, t * 0.9));
                float chop2 = FBM(float2(phase * 1.9 - t * 0.7, phase * 0.4 + t * 0.35));
                w += (chop - 0.5) * 0.55;
                w += (chop2 - 0.5) * 0.35;
                w += pow(saturate(chop * chop2 * 1.4), 2.0) * 0.45;
                return saturate(w * 0.5 + 0.5) * 2.0 - 1.0;
            }

            void SampleWater(float2 uv, float3 worldPos, out half3 albedo, out half alpha, out half3 normalTS)
            {
                float t = _Time.y * _WaveSpeed;

                // Body noise tiles in world space so flooded caves don't stretch one pattern
                float2 wuv = worldPos.xy / max(WaveLength() * 2.5, 1.0) * _NoiseScale;
                float2 puv = Pix(wuv + float2(t * 0.16, t * 0.05), _PixelSize * 0.35);
                float2 puv2 = Pix(wuv * 1.6 + float2(-t * 0.12, t * 0.09), _PixelSize * 0.5);

                float n = FBM(puv);
                float n2 = FBM(puv2 + 3.7);
                float cells = ValueNoise(puv * 2.2);

                float depth = saturate((1.0 - uv.y) * _DepthContrast + (n - 0.5) * 0.12);
                half3 body = lerp(_Color.rgb, _DeepColor.rgb, depth);
                body = lerp(body, body * 1.06, cells * 0.18);
                body = lerp(body, body * 0.94, n2 * 0.15);

                float height = max(_RectSize.y, 0.5);
                // All surface metrics in WORLD units → UV, so tall/short pools look the same
                float amp = clamp(_WaveAmp / height, 0.002, 0.15);
                float widthUV = clamp(_FoamWidth / height, 0.004, 0.2);
                float edgeSoft = clamp(_EdgeSoft / height, 0.0015, 0.08);

                float wave = SurfaceWave(worldPos.x, t);
                float surface = (_SurfaceUV - amp) + wave * amp;
                float below = surface - uv.y;
                float edgeKeep = smoothstep(-edgeSoft, edgeSoft * 0.55, below);

                float sideWorld = min(uv.x, 1.0 - uv.x) * max(_RectSize.x, 0.5);
                float sideWave = 0.5 + 0.5 * SurfaceWave(worldPos.y * 0.35 + t * 0.15, t * 0.7);
                float sideErode = smoothstep(0.0, 0.35 + 0.2 * sideWave, sideWorld);
                float topFactor = smoothstep(0.55, 0.85, uv.y);
                edgeKeep *= lerp(1.0, sideErode, topFactor);

                float dist = abs(below);
                float crest = saturate(1.0 - dist / widthUV);
                crest = crest * crest * (3.0 - 2.0 * crest);
                crest *= edgeKeep;

                float g1 = FBM(float2(worldPos.x / WaveLength() * 0.9 + t * 2.6, t * 3.4));
                float g2 = FBM(float2(worldPos.x / WaveLength() * 1.7 - t * 3.2, t * 4.1));
                float clump = lerp(0.45, 1.0, smoothstep(0.3, 0.7, g1));
                clump *= lerp(0.55, 1.0, smoothstep(0.25, 0.65, g2));
                float foam = crest * _FoamAmount * clump * (0.55 + _FoamGurgle * g1 * 0.45);
                float peak = saturate(wave * 0.5 + 0.5);
                foam += crest * peak * _FoamGurgle * smoothstep(0.55, 0.85, g2) * 0.4;
                foam = saturate(foam);

                body = lerp(body, _FoamColor.rgb, foam);
                body = lerp(body, lerp(body, _FoamColor.rgb, 0.35), crest * (1.0 - foam) * 0.45);

                float hL = SurfaceWave(worldPos.x - 0.35, t);
                float hR = SurfaceWave(worldPos.x + 0.35, t);
                normalTS = normalize(half3((hL - hR) * _NormalStrength * 4.0, (n2 - n) * _NormalStrength, 1.0));

                half maskA = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).a;
                albedo = body;
                alpha = saturate(lerp(_Color.a, _DeepColor.a, depth) + foam * 0.15) * maskA * edgeKeep;
                alpha = min(alpha, 0.82);
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
                float _EdgeSoft;
                float _FoamWidth;
                float _FoamAmount;
                float _FoamGurgle;
                float _DepthContrast;
                float _NormalStrength;
                float _SplashSpeed;
                float4 _RectSize;
                float4 _Splash1;
                float4 _Splash2;
                float4 _Splash3;
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
                float _EdgeSoft;
                float _FoamWidth;
                float _FoamAmount;
                float _FoamGurgle;
                float _DepthContrast;
                float _NormalStrength;
                float _SplashSpeed;
                float4 _RectSize;
                float4 _Splash1;
                float4 _Splash2;
                float4 _Splash3;
            CBUFFER_END

            float Hash21(float2 p){ p=frac(p*float2(127.1,311.7)); p+=dot(p,p+19.19); return frac(p.x*p.y); }
            float ValueNoise(float2 p){ float2 i=floor(p),f=frac(p); float a=Hash21(i),b=Hash21(i+float2(1,0)),c=Hash21(i+float2(0,1)),d=Hash21(i+float2(1,1)); float2 u=f*f*(3-2*f); return lerp(lerp(a,b,u.x),lerp(c,d,u.x),u.y); }
            float FBM(float2 p){ float v=0,a=0.5; [unroll] for(int i=0;i<3;i++){ v+=ValueNoise(p)*a; p=p*2.05+17.1; a*=0.5;} return v; }
            float WaveLength(){ return max(70.0 / max(_WaveFreq, 0.5), 2.5); }
            float SurfaceWave(float worldX, float t)
            {
                float phase = worldX / WaveLength();
                float w =
                    sin(phase * 6.183 + t * 2.1) * 0.42 +
                    sin(phase * 11.7 - t * 2.85 + 1.7) * 0.22 +
                    sin(phase * 2.45 + t * 1.15) * 0.18;
                float chop = FBM(float2(phase * 0.85 + t * 0.55, t * 0.9));
                w += (chop - 0.5) * 0.55;
                w += (FBM(float2(phase * 1.9 - t * 0.7, t * 0.35)) - 0.5) * 0.35;
                return w;
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
                float2 wuv = input.worldPos.xy / max(WaveLength()*2.5,1.0) * _NoiseScale;
                float2 puv = floor((wuv+float2(t*0.16,t*0.05))*_PixelSize*0.35)/(_PixelSize*0.35);
                float n = FBM(puv);
                float depth = saturate((1-input.uv.y)*_DepthContrast+(n-0.5)*0.12);
                half3 rgb = lerp(_Color.rgb, _DeepColor.rgb, depth);
                float height = max(_RectSize.y, 0.5);
                float wave = SurfaceWave(input.worldPos.x, t);
                float amp = clamp(_WaveAmp / height, 0.002, 0.15);
                float surface = (_SurfaceUV - amp) + wave * amp;
                float below = surface - input.uv.y;
                float edgeSoft = clamp(_EdgeSoft / height, 0.0015, 0.08);
                float edgeKeep = smoothstep(-edgeSoft, edgeSoft*0.55, below);
                float sideWorld = min(input.uv.x, 1.0 - input.uv.x) * max(_RectSize.x, 0.5);
                float sideErode = smoothstep(0.0, 0.4, sideWorld);
                float topFactor = smoothstep(0.55, 0.85, input.uv.y);
                edgeKeep *= lerp(1.0, sideErode, topFactor);
                float widthUV = clamp(_FoamWidth / height, 0.004, 0.2);
                float crest = saturate(1.0 - abs(below) / widthUV);
                crest = crest*crest*(3.0-2.0*crest) * edgeKeep;
                float g = FBM(float2(input.worldPos.x/WaveLength()*0.9+t*1.8, t*2.5));
                float foam = crest * _FoamAmount * lerp(0.5,1.0,g);
                rgb = lerp(rgb, _FoamColor.rgb, saturate(foam));
                half a = saturate(lerp(_Color.a,_DeepColor.a,depth)+foam*0.15) * SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,input.uv).a * input.color.a * edgeKeep;
                return half4(rgb*input.color.rgb, min(a,0.82));
            }
            ENDHLSL
        }
    }
}
