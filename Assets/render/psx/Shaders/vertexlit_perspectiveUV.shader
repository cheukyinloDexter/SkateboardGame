Shader "psx/vertexlit_perspective" {
    Properties{
        _MainTex("Base (RGB)", 2D) = "white" {}
    }
        SubShader{
            Tags { "RenderType" = "Opaque" }
            LOD 200

            Pass {
                Name "0"
                Lighting On

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct v2f {
                    float4 pos : SV_POSITION;
                    half4 color : COLOR0;
                    half4 colorFog : COLOR1;
                    float2 uv_MainTex : TEXCOORD0;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                uniform half4 unity_FogStart;
                uniform half4 unity_FogEnd;

                v2f vert(appdata_full v) {
                    v2f o;

                    // Snap vertex to pixel grid (optional PS1-style)
                    float4 clipPos = UnityObjectToClipPos(v.vertex);
                    float4 snapped = clipPos;
                    snapped.xyz = clipPos.xyz / clipPos.w;
                    snapped.x = floor(160 * snapped.x) / 160;
                    snapped.y = floor(120 * snapped.y) / 120;
                    snapped.xyz *= clipPos.w;
                    o.pos = snapped;

                    // Vertex lighting
                    o.color = float4(ShadeVertexLightsFull(v.vertex, v.normal, 4, true), 1.0);
                    o.color *= v.color;

                    // Perspective-correct texture coordinates (NO distance scaling)
                    o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);

                    // Fog calculation
                    float distance = length(UnityObjectToViewPos(v.vertex));
                    float fogFactor = saturate((unity_FogEnd - distance) / (unity_FogEnd - unity_FogStart));
                    o.colorFog = unity_FogColor;
                    o.colorFog.a = fogFactor;

                    // Optional polygon cutoff for far distance
                    if (distance > unity_FogStart.z + unity_FogColor.a * 255) {
                        o.pos.w = 0;
                    }

                    return o;
                }

                float4 frag(v2f IN) : COLOR {
                    half4 tex = tex2D(_MainTex, IN.uv_MainTex);
                    half4 lit = tex * IN.color;
                    return lit * IN.colorFog.a + IN.colorFog * (1 - IN.colorFog.a);
                }

                ENDCG
            }
    }
}
