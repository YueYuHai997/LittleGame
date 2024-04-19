Shader "Unlit/FoilCard"
{
    Properties
    {
        _Color("Color",Color)=(1,1,1,1)
        [HDR]_FoilColor("FoilColor",Color)=(1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _Noise("Noise",2D) = "white" {}
        _ViewDirectionDisplacement("view Direction Displacement",Float) = 0
        _RandomDirections("Random directions",2D)= "white" {}
        _FoilMask("Foil mask",2D) = "white" {}
        _MaskThreshold("_Mask Threshold",Range(0,1)) = 0
        _GradientMap("Gradient map",2D) ="white" {}
        _ParallaxOffset("Parallx offset",Float) =0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
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
                float2 uv : TEXCOORD0;
                float3 normal:NORMAL;
                float4 tangent:TANGENT;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 viewDir:TEXCOORD1;
                float3 viewDirTangent:TEXCOORD2;
                float3 normal:NORMAL;
            };

            sampler2D _MainTex;
            sampler2D _Noise;
            float _ViewDirectionDisplacement;
            sampler2D _GradientMap;
            sampler2D _FoilMask;
            float4 _MainTex_ST;

            float _ParallaxOffset;
            fixed4 _Color;;
            float _MaskThreshold;
            fixed4 _FoilColor;

            sampler2D _RandomDirections;


            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.viewDir = WorldSpaceViewDir(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);

                float4 objCam = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.0));
                float3 viewDir = v.vertex.xyz - objCam.xyz;
                float tangentSign = v.tangent.w * unity_WorldTransformParams.w ;
                float3 bitangent = cross(v.normal.xyz, v.tangent.xyz) * tangentSign;
                o.viewDirTangent = float3(dot(viewDir, v.tangent.xyz)/2,
                                          dot(viewDir, bitangent.xyz),
                                          dot(viewDir, v.normal.xyz));


                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                float noise = tex2D(_Noise, i.uv).x;
                float2 uv = i.uv + i.viewDirTangent * noise * _ParallaxOffset;
                float displ = (tex2D(_Noise, uv).x * 2.0 - 1.0) * _ViewDirectionDisplacement;
                float3 randomdir = normalize(tex2D(_RandomDirections, uv).xyz);
                float dotProduct = saturate(dot(randomdir + i.normal, normalize(i.viewDir + displ)));
                float fresnel = pow(1.0 - dotProduct, 2.0);
                float samplingVal = (sin((i.viewDir.x + i.viewDir.y) * 1.0 * UNITY_TWO_PI) * 0.5 + 0.5) * fresnel;
                float mask = tex2D(_FoilMask, uv).x;
                col = lerp(col, tex2D(_GradientMap, dotProduct) *_FoilColor, step(_MaskThreshold, mask));
                return col;
            }
            ENDCG
        }
    }
}