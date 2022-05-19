Shader "Unlit/WorldSpaceColor"
{
    Properties
    {
        _Scale("Scale", Float) = 1
        _ChunkSize("Chunk Size", Float) = 16
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
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
                float3 worldPos : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Scale;
            float _ChunkSize;

            // https://gist.github.com/h3r/3a92295517b2bee8a82c1de1456431dc
            float hash(float n) { return frac(sin(n) * 1e4); }
            float hash(float2 p) { return frac(1e4 * sin(17.0 * p.x + p.y * 0.1) * (0.1 + abs(sin(p.y * 13.0 + p.x)))); }

            float noise(float3 x) {
                const float3 step = float3(110, 241, 171);

                float3 i = floor(x);
                float3 f = frac(x);
            
                // For performance, compute the base input to a 1D hash from the integer part of the argument and the 
                // incremental change to the 1D based on the 3D -> 1D wrapping
                float n = dot(i, step);

                float3 u = f * f * (3.0 - 2.0 * f);
                return lerp(lerp(lerp( hash(n + dot(step, float3(0, 0, 0))), hash(n + dot(step, float3(1, 0, 0))), u.x),
                        lerp( hash(n + dot(step, float3(0, 1, 0))), hash(n + dot(step, float3(1, 1, 0))), u.x), u.y),
                        lerp(lerp( hash(n + dot(step, float3(0, 0, 1))), hash(n + dot(step, float3(1, 0, 1))), u.x),
                        lerp( hash(n + dot(step, float3(0, 1, 1))), hash(n + dot(step, float3(1, 1, 1))), u.x), u.y), u.z);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float n = (int)floor(noise(i.worldPos * _Scale) / _ChunkSize) * _ChunkSize;
                return fixed4(n, n, n, 1);
            }
            ENDCG
        }
    }
}
