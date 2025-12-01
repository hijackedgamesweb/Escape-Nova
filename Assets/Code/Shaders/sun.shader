Shader "Custom/WhiteKeepColor_Intensity"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _TargetColor ("Target Color", Color) = (1,0,0,1)
        _Intensity ("Intensity", Range(0, 5)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _TargetColor;
            float _Intensity;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv);
                float luminance = texColor.r; // sprite en escala de grises
                fixed4 finalColor = lerp(fixed4(1,1,1,1), _TargetColor, saturate((1 - luminance) * _Intensity));
                finalColor.a = texColor.a;
                return finalColor;
            }
            ENDCG
        }
    }
}
