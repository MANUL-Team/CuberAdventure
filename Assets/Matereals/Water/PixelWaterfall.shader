Shader "CuberAdventure/PixelWaterfallLit"
{
    Properties
    {
        [PerRendererData] _MainTex ("Mask / Sprite", 2D) = "white" {}
        [PerRendererData] _MaskTex ("Light Mask", 2D) = "white" {}
        [PerRendererData] _NormalMap ("Normal Map", 2D) = "bump" {}
        _Color ("Water", Color) = (0.35, 0.75, 1.0, 0.65)
        _FoamColor ("Foam", Color) = (0.9, 0.97, 1.0, 0.95)
        _DeepColor ("Deep", Color) = (0.12, 0.4, 0.75, 0.8)
        _PixelSize ("Pixels", Float) = 56
        _NoiseScale ("Noise Scale", Float) = 4.5
        _FlowSpeed ("Flow Speed", Float) = 1.8
        _EdgeErode ("Edge Erode", Range(0, 0.5)) = 0.18
        _Density ("Density", Range(0.2, 1.5)) = 0.95
        _NormalStrength ("Normal Strength", Range(0, 2)) = 1.1
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

            struct Attributes { COMMON_2D_INPUTS half4 color:COLOR; UNITY_SKINNED_VERTEX_INPUTS };
            struct Varyings { COMMON_2D_LIT_OUTPUTS half4 color:COLOR; float3 worldPos:TEXCOORD4; };
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_MaskTex); SAMPLER(sampler_MaskTex);

            CBUFFER_START(UnityPerMaterial)
                half4 _Color, _FoamColor, _DeepColor;
                float _PixelSize, _NoiseScale, _FlowSpeed, _EdgeErode, _Density, _NormalStrength;
            CBUFFER_END

            float Hash21(float2 p){ p=frac(p*float2(127.1,311.7)); p+=dot(p,p+19.19); return frac(p.x*p.y); }
            float ValueNoise(float2 p){ float2 i=floor(p),f=frac(p); float a=Hash21(i),b=Hash21(i+float2(1,0)),c=Hash21(i+float2(0,1)),d=Hash21(i+float2(1,1)); float2 u=f*f*(3-2*f); return lerp(lerp(a,b,u.x),lerp(c,d,u.x),u.y); }
            float FBM(float2 p){ float v=0,a=0.5; [unroll] for(int i=0;i<4;i++){ v+=ValueNoise(p)*a; p=p*2.03+float2(13.1,7.7); a*=0.5;} return v; }

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
                float t = _Time.y * _FlowSpeed;
                // Local UV flow down the sheet (works when rotated) + world detail
                float2 flowUV = float2(input.uv.x, input.uv.y - t);
                float2 pix = floor(flowUV * _PixelSize) / _PixelSize;
                float2 wpix = floor((input.worldPos.xy * _NoiseScale * 0.2 + float2(0, -t * 1.5)) * _PixelSize * 0.4) / (_PixelSize * 0.4);

                float n = FBM(wpix * 1.2);
                float n2 = FBM(wpix * 2.4 + 5.1);
                float streaks = pow(saturate(1.0 - abs(frac(pix.y * 8.0 + n * 2.0) - 0.5) * 2.0), 2.5);
                streaks *= step(0.25, n2);

                // Fire-like soft silhouette: erode edges with noise (not a hard rectangle look)
                float edge = min(input.uv.x, 1.0 - input.uv.x);
                float edgeMask = smoothstep(0.0, _EdgeErode + n * 0.12, edge);
                float topCap = smoothstep(0.0, 0.08, input.uv.y) * smoothstep(1.0, 0.9, input.uv.y);
                float shape = edgeMask * topCap;
                shape *= lerp(0.55, 1.0, n);
                shape = saturate(shape * _Density);

                half3 rgb = lerp(_DeepColor.rgb, _Color.rgb, saturate(input.uv.y * 0.4 + n * 0.45));
                rgb = lerp(rgb, _FoamColor.rgb, streaks * 0.7 + (1.0 - edgeMask) * 0.35);
                // Foam at source (top) and splash (bottom) — soft
                float topFoam = smoothstep(0.75, 1.0, input.uv.y) * n2;
                float botFoam = (1.0 - smoothstep(0.0, 0.25, input.uv.y)) * n;
                rgb = lerp(rgb, _FoamColor.rgb, saturate(topFoam * 0.55 + botFoam * 0.4));

                float hL = FBM(wpix + float2(-0.04,0));
                float hR = FBM(wpix + float2(0.04,0));
                float hD = FBM(wpix + float2(0,-0.04));
                float hU = FBM(wpix + float2(0,0.04));
                half3 normalTS = normalize(half3((hL-hR)*_NormalStrength, (hD-hU)*_NormalStrength, 1));

                half maskA = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv).a;
                half alpha = saturate(shape * lerp(_Color.a, 1.0, streaks * 0.3)) * maskA * input.color.a;
                alpha = min(alpha, 0.92);

                half4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, input.uv);
                SurfaceData2D surfaceData;
                InputData2D inputData;
                InitializeSurfaceData(rgb * input.color.rgb, alpha, mask, normalTS, surfaceData);
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
            struct Attributes { COMMON_2D_NORMALS_INPUTS float4 color:COLOR; UNITY_SKINNED_VERTEX_INPUTS };
            struct Varyings { COMMON_2D_NORMALS_OUTPUTS half4 color:COLOR; };
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/Normals2DCommon.hlsl"
            CBUFFER_START(UnityPerMaterial)
                half4 _Color,_FoamColor,_DeepColor;
                float _PixelSize,_NoiseScale,_FlowSpeed,_EdgeErode,_Density,_NormalStrength;
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
            Tags { "LightMode"="UniversalForward" }
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
                half4 _Color,_FoamColor,_DeepColor;
                float _PixelSize,_NoiseScale,_FlowSpeed,_EdgeErode,_Density,_NormalStrength;
            CBUFFER_END
            float Hash21(float2 p){ p=frac(p*float2(127.1,311.7)); p+=dot(p,p+19.19); return frac(p.x*p.y); }
            float ValueNoise(float2 p){ float2 i=floor(p),f=frac(p); float a=Hash21(i),b=Hash21(i+float2(1,0)),c=Hash21(i+float2(0,1)),d=Hash21(i+float2(1,1)); float2 u=f*f*(3-2*f); return lerp(lerp(a,b,u.x),lerp(c,d,u.x),u.y); }
            float FBM(float2 p){ float v=0,a=0.5; [unroll] for(int i=0;i<3;i++){v+=ValueNoise(p)*a;p=p*2.03;a*=0.5;} return v; }
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
                float t=_Time.y*_FlowSpeed;
                float2 wpix=floor((input.worldPos.xy*_NoiseScale*0.2+float2(0,-t*1.5))*_PixelSize*0.4)/(_PixelSize*0.4);
                float n=FBM(wpix);
                float edge=min(input.uv.x,1-input.uv.x);
                float shape=smoothstep(0,_EdgeErode+n*0.1,edge)*_Density;
                half3 rgb=lerp(_DeepColor.rgb,_Color.rgb,n);
                half a=shape*SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,input.uv).a*input.color.a*_Color.a;
                return half4(rgb*input.color.rgb,saturate(a));
            }
            ENDHLSL
        }
    }
}
