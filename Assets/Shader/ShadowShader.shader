Shader "TNTC/PlanarShadow"
{
    Properties
    {
        _ShadowColor ("Shadow Color", Color) = (0, 0, 0, 0.6)
        _LightDir ("Fake Light Direction", Vector) = (0, -1, 0, 0)
        _FloorHeight ("Floor Y Position", Float) = 0.0
        _OpacityMultiplier ("Opacity Multiplier", Range(0, 1)) = 1.0 // 1. New Property
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent-1" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Offset -1, -1 

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            float4 _ShadowColor;
            float4 _LightDir;
            float _FloorHeight;
            float _OpacityMultiplier; // 2. Declare the variable

            v2f vert (appdata v)
            {
                v2f o;
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);

                float3 L = normalize(_LightDir.xyz);
                L.y = min(L.y, -0.001); 

                float heightToFloor = worldPos.y - _FloorHeight;
                worldPos.xz = worldPos.xz - L.xz * (heightToFloor / L.y);
                worldPos.y = _FloorHeight + 0.01; 

                o.vertex = mul(UNITY_MATRIX_VP, worldPos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 finalColor = _ShadowColor;
                finalColor.a *= _OpacityMultiplier; // 3. Multiply base alpha by the multiplier
                return finalColor;
            }
            ENDCG
        }
    }
}
