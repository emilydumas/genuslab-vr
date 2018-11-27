Shader "Custom/ssoit_surface" {
	Properties {
	   _Z1("Rank 1 Depth Texture", 2D) = "black" {} 
       _Z2("Rank 2 Depth Texture", 2D) = "black" {} 
       _Z3("Rank 3 Depth Texture", 2D) = "black" {} 
       _Z4("Rank 4 Depth Texture", 2D) = "black" {} 
       _MainTex("Main Texture", 2D) = "white" {}
       _DepthTolerance("Depth difference considered to be zero", Float) = 0.006
	}
	SubShader {
		
		Tags { "RenderType"="Transparent" "Queue" = "Transparent"}
        Cull off
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite off
        ZTest LEqual
        CGPROGRAM
            #define _dtex _Z4
            #include "ssoitPassImpl.cginc"
            #pragma surface surf Standard
            //#pragma target 3.0
            struct Input {
                float2 uv_MainTex;
            };
            void surf (Input IN, inout SurfaceOutputStandard o) {
                o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
            }
        ENDCG
        CGPROGRAM  
            #define _dtex _Z3
            #include "ssoitPassImpl.cginc"
            #pragma surface surf Standard
           // #pragma target 3.0 
            struct Input {
                float2 uv_MainTex;
            };
            void surf (Input IN, inout SurfaceOutputStandard o) {
                o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
            } 
    	ENDCG
        CGPROGRAM
            #define _dtex _Z2
            #include "ssoitPassImpl.cginc"
            #pragma surface surf Standard  
           // #pragma target 3.0
            struct Input {
                float2 uv_MainTex;
            };
            void surf (Input IN, inout SurfaceOutputStandard o) {
                o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
            }
        ENDCG
        CGPROGRAM
            #define _dtex _Z1
            #include "ssoitPassImpl.cginc"
			#pragma surface surf Standard 
            //#pragma target 3.0
            struct Input {
                float2 uv_MainTex;
            };
            void surf (Input IN, inout SurfaceOutputStandard o) {
                 o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
            }
        ENDCG
        
	}
	FallBack "Diffuse"
}
