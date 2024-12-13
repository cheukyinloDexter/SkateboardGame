Shader "ScreenPocket/BIRP/Effect/Crying"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Length ("Length", Float) = 8
        _WaveCycle ("WaveCycle", Float) = 8
        _Speed ("Speed", Float) = 1
        _Weight ("Weight", Float) = 1
        [Header(Rim)]
        _RimColor ("RimColor", Color) = (1,1,1,1)
        _RimCenter ( "RimCenter", Range(-1,1) ) = 0
        _RimSoftness ( "RimSoftness", Range(0,1) ) = 0
        [Header(Specular)]
        _SpecularColor ("SpecularColor", Color) = (1,1,1,1)
        _SpecularCenter ( "SpecularCenter", Range(-1,1) ) = 0
        _SpecularSoftness ( "SpecularSoftness", Range(0,1) ) = 0
        [Header(Lighting)]
        _Emissive ("Emissive", Color) = (0,0,0,0)
    }
    SubShader
    {
        Tags {
            "LightMode" = "ForwardBase"
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 100

        GrabPass
        {
            "_GrabPassTexture"
        }
        
        Pass
        {
            Zwrite Off
	        ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #define USING_FOG (defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2))

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 grab : TEXCOORD1;
                float3 normal : TEXCOORD2;
                float3 worldPos : TEXCOORD3;
                UNITY_FOG_COORDS(4)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);

                float4 grabScreenPos = ComputeGrabScreenPos(o.vertex);
                o.grab = grabScreenPos.xy/grabScreenPos.w;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            sampler2D _GrabPassTexture;
            float4 _GrabPassTexture_TexelSize;
            //ズラシに関わる情報
            float _Length;
            float _WaveCycle;
            float _Speed;
            float _Weight;

            //リムライト
            fixed4 _RimColor;
            float _RimCenter;
            float _RimSoftness;

            //スペキュラ
            fixed4 _SpecularColor;
            float _SpecularCenter;
            float _SpecularSoftness;

            //ライティングにかける係数
            fixed4 _Emissive;

            float2 GetUV(float2 baseUV)
            {
                float2 uv;
                float time = _Time.w;
                //float2 length = (_ScreenParams.zw-1) * _Length;
                float2 length = (_GrabPassTexture_TexelSize.xy) * _Length;
                uv.x = baseUV.x + cos(baseUV.y * _WaveCycle * UNITY_TWO_PI + time * _Speed) * length.x * sin(baseUV.x * UNITY_TWO_PI);
                uv.y = baseUV.y + sin(baseUV.x * _WaveCycle * UNITY_TWO_PI + time * _Speed) * length.y * sin(baseUV.y * UNITY_TWO_PI);
                return lerp(baseUV, uv, _Weight);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = GetUV(i.grab);
                fixed4 grab = tex2D(_GrabPassTexture, uv);
                fixed4 col = tex2D(_MainTex, i.uv);
                
                //アルファ値がグレーの部分を白くする
                //float grayToColor = (1-abs(col.a-0.5)*2);
                //白を加算する
                //return grab * col + grayToColor;
                
                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz-i.worldPos.xyz);
                float rim = 1 - saturate(abs(dot(i.normal, viewDir)));
                rim = smoothstep(_RimCenter-_RimSoftness,_RimCenter+_RimSoftness, rim);

                float3 halfVector = normalize(_WorldSpaceLightPos0.xyz + viewDir);
                
                float specular = max(0.0, dot(halfVector, i.normal));
                specular = smoothstep(_SpecularCenter-_SpecularSoftness,_SpecularCenter+_SpecularSoftness, specular);

                //ライトカラーの合成　リムとスペキュラの範囲にライトカラーとエミッシブをかける
                const fixed3 lightColor = (_RimColor * rim + _SpecularColor * specular) * (_LightColor0.rgb + _Emissive);
                fixed3 color = saturate(grab * col + lightColor);

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, color);
                
                return fixed4(color, col.a);
            }
            ENDCG
        }
    }
}
