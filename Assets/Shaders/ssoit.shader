Shader "ssoit" {
    Properties
    {
       _Z1("Rank 1 Depth Texture", 2D) = "black" {} 
       _Z2("Rank 2 Depth Texture", 2D) = "black" {} 
       _Z3("Rank 3 Depth Texture", 2D) = "black" {} 
       _Z4("Rank 4 Depth Texture", 2D) = "black" {} 
       _MainTex("Main Texture", 2D) = "white" {}
       _DepthTolerance("Depth difference considered to be zero", Float) = 0.006
    }
    SubShader {
        Tags {"Queue" = "Transparent" "RenderType"="Transparent" }
        Cull off
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite on
        ZTest LEqual
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define _dtex _Z4
            #include "ssoitPassImpl.cginc"
            ENDCG
        }
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define _dtex _Z3
            #include "ssoitPassImpl.cginc"
            ENDCG
        }
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define _dtex _Z2
            #include "ssoitPassImpl.cginc"
            ENDCG
        }
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define _dtex _Z1
            #include "ssoitPassImpl.cginc"
            ENDCG
        }
    }
}