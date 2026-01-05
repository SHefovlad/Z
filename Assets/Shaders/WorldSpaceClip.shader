Shader "Custom/WorldTriplanarLit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Tiling ("Tiling", Float) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        fixed4 _Color;
        float _Tiling;

        struct Input
        {
            float3 worldPos;
            float3 worldNormal;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float3 n = abs(normalize(IN.worldNormal));

            float2 uvX = IN.worldPos.zy * _Tiling;
            float2 uvY = IN.worldPos.xz * _Tiling;
            float2 uvZ = IN.worldPos.xy * _Tiling;

            fixed4 colX = tex2D(_MainTex, uvX);
            fixed4 colY = tex2D(_MainTex, uvY);
            fixed4 colZ = tex2D(_MainTex, uvZ);

            fixed4 col =
                colX * n.x +
                colY * n.y +
                colZ * n.z;

            o.Albedo = col.rgb * _Color.rgb;
            o.Alpha = _Color.a;
        }
        ENDCG
    }

    FallBack "Standard"
}
