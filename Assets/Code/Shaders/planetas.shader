Shader "Custom/PlanetLit2D"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "bump" {}
        _LightPos("Light Position", Vector) = (0,0,0,0)
        _LightIntensity("Light Intensity", Range(0,5)) = 1
        _LightRange("Light Range", Float) = 5
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _NormalMap;
            float4 _MainTex_ST;

            float3 _LightPos;
            float _LightIntensity;
            float _LightRange;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                return o;
            }

                fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // 1️⃣ Normal map
                float3 nSample = UnpackNormal(tex2D(_NormalMap, i.uv)); 
                float2 nXY = nSample.xy;
                float nZ = sqrt(saturate(1 - dot(nXY,nXY)));
                float3 normal = normalize(float3(nXY, nZ));

                // 2️⃣ Vector luz en 2D
                float2 L2 = _LightPos.xy - i.worldPos.xy;
                float3 toLight = normalize(float3(L2, 0.0));

                // 3️⃣ Dot product Lambert
                float NdotL = saturate(dot(normal, toLight));

                // 4️⃣ Atenuación por distancia
                float distance = length(L2);
                float attenuation = saturate(1.0 - pow(distance / _LightRange, 2));

                // 5️⃣ Factor luz final
                float lightFactor = NdotL * _LightIntensity * attenuation + 0.1;
                col.rgb *= lightFactor;

                return col;
            }
            ENDCG
        }
    }
}
