// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UMOM/AcrylicMosaic"
{
    Properties {
        _BurnColor("BurnColor", Color) = (1, 1, 1, 0.1)
        _TintTex("TintColor", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _FresnelBias("Freshel Bias", float) = 1
        _FresnelScale("Freshel Scale", float) = 1
        _FresnelPower("Freshel Power", float) = 1
        _FresnelColor("Freshnel Color", Color) = (1, 1, 1, 0.8)
        [HideInInspector]
        _MainTex ("Tint Color (RGB)", 2D) = "white" {}
    }
    Category {
    
        Tags { "RenderType" = "Opaque" }

		CGINCLUDE
		
        #include "UnityCG.cginc"
        #include "../ThirdParty/PhotoshopBlendModes.cginc"
        
        struct appdata_t {
            float4 vertex : POSITION;
            float2 texcoord: TEXCOORD0;
            float3 normal : NORMAL;
            float4 color    : COLOR; 
        };
   
        struct v2f {
            float4 vertex : POSITION;  
            float4 uv: TEXCOORD0;
            float2 uv2: TEXCOORD1;
            float4 fresnel : NORMAL;  
            float4 color : COLOR;
        };

        sampler2D _MainTex;
        sampler2D _NoiseTex;
        sampler2D _TintTex;
        float2 _MainTex_ST;
        float _FresnelBias;
        float _FresnelScale;
        float _FresnelPower;
        float4 _FresnelColor;
        float _Scale;
        float4 _BurnColor;
 
        
        v2f vert (appdata_t v) 
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = ComputeScreenPos(o.vertex);
            o.uv2 = v.texcoord;
            o.color = v.color;
            float3 i = normalize(ObjSpaceViewDir(v.vertex));
            
            //Freshel
            o.fresnel = _FresnelBias + _FresnelScale * pow(1 + dot(i, v.normal), -_FresnelPower);
            
            #if UNITY_UV_STARTS_AT_TOP
            float scale = -1.0;
            #else
            float scale = 1.0;
            #endif
            return o;
        }
        

        half4 frag(v2f i) : COLOR
        {
            float4 col = tex2D(_MainTex, i.uv.xy / i.uv.w);
           
            //Color Burn
            col.xyz = LinearBurn(col.xyz, _BurnColor.xyz);
            
            //Tint
            float4 tint = tex2D(_TintTex, i.uv2);
            col.xyz = normalize(col.xyz) * tint.xyz;
            
            //Noise
            float4 noise = tex2D(_NoiseTex, i.uv2);
            col = lerp(col, noise, noise.r);

            //Freshnel
            col = lerp(col, 1 - _FresnelColor, 1 - i.fresnel);
            return col;
        }
        
        ENDCG
        SubShader
        {
 
            Pass
            {            
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                ENDCG
            }
    
        }
    }
}
