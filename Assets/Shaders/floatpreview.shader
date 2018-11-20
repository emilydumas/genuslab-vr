Shader "floatpreview" {
    // Main texture map is interpreted as RFloat
    // But then rendered as grayscale
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _min("Scale minimum (=black)", Float) = 0
        _max("Scale maximum (=white)", Float) = 1
        _invert("-1 to exchange role of black/white", Int) = 1
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull off
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#include "UnityCG.cginc"

            uniform sampler2D _MainTex;
            uniform float _min;
            uniform float _max;
            uniform int _invert;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

			struct v2f
			{
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
			};
 
			v2f vert (appdata v)
			{
				v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				return o;
			}
 
            fixed4 frag (v2f i) : SV_Target {
                float texval = tex2D(_MainTex, i.uv);
                float t = clamp(texval,_min,_max);
                t = (t - _min) / (_max - _min);
                t = _invert*t + 0.5*(1 - _invert);
                return fixed4(t,t,t,0);
           }
           ENDCG
       }
   }
}