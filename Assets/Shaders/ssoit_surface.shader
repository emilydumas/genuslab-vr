
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
        Pass{
            // One pass of Selection Sort Order-Independent Transparency drawing
            // Fragment shader will only output if the eye depth matches the value
            // in _dtex at that point.

            CGPROGRAM
            #include "UnityCG.cginc"
            uniform sampler2D _dtex;
            uniform sampler2D _MainTex2;
            uniform float _DepthTolerance;
            #pragma vertex vert
            #pragma fragment frag
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenpos : TEXCOORD1;
                float3 normal : TEXCOORD2;
                float eyedepth : TEXCOORD3;
            };
            
            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.screenpos = ComputeScreenPos(o.pos);
                o.normal = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
                o.uv = v.texcoord;
                COMPUTE_EYEDEPTH(o.eyedepth);
                return o;
            }
            
            fixed4 frag (v2f i) : COLOR {
                float target_depth = tex2Dproj(_dtex, i.screenpos);

                // FIXME: The next test is a hack to prevent an unexplained problem.  We should be testing
                // for (i.eyedepth == target_depth), i.e. exact match of depth of this fragment to a depth
                // recorded by ZSort.  However, doing this results in no pixels being rendered on the edges
                // of triangles.
                // To fix this, we consider depths to match as long as they are close.  Making _DepthTolerance
                // big enough removes the artifacts, but at the cost of potential rendering artifacts when
                // fragment depths are genuinely close but different.

                // A proper fix would require understanding why the exact match fails...

                if (abs(i.eyedepth - target_depth) > _DepthTolerance) {
                    discard;
                }

                // Depths match.  Return the color from MainTex
                fixed4 col = tex2D(_MainTex2,i.uv);
                return col;
            }
            ENDCG
        }
        CGPROGRAM
            #include "UnityCG.cginc"
            #pragma surface surf Standard
            #define _dtex _Z4
            //#include "ssoitPassImpl.cginc"
            sampler2D _MainTex;
            //PROBLEM???? The _MainTex used should be the one from properties
            struct Input {
                float2 uv_MainTex;
            };
            void surf (Input IN, inout SurfaceOutputStandard o) {
                o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
                o.Alpha = tex2D (_MainTex, IN.uv_MainTex).a;
            }
        ENDCG
       
        Pass{
            // One pass of Selection Sort Order-Independent Transparency drawing
            // Fragment shader will only output if the eye depth matches the value
            // in _dtex at that point.

            CGPROGRAM
            #include "UnityCG.cginc"
            uniform sampler2D _dtex;
            uniform sampler2D _MainTex;
            uniform float _DepthTolerance;
            #pragma vertex vert
            #pragma fragment frag
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenpos : TEXCOORD1;
                float3 normal : TEXCOORD2;
                float eyedepth : TEXCOORD3;
            };
            
            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.screenpos = ComputeScreenPos(o.pos);
                o.normal = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
                o.uv = v.texcoord;
                COMPUTE_EYEDEPTH(o.eyedepth);
                return o;
            }
            
            fixed4 frag (v2f i) : COLOR {
                float target_depth = tex2Dproj(_dtex, i.screenpos);

                // FIXME: The next test is a hack to prevent an unexplained problem.  We should be testing
                // for (i.eyedepth == target_depth), i.e. exact match of depth of this fragment to a depth
                // recorded by ZSort.  However, doing this results in no pixels being rendered on the edges
                // of triangles.
                // To fix this, we consider depths to match as long as they are close.  Making _DepthTolerance
                // big enough removes the artifacts, but at the cost of potential rendering artifacts when
                // fragment depths are genuinely close but different.

                // A proper fix would require understanding why the exact match fails...

                if (abs(i.eyedepth - target_depth) > _DepthTolerance) {
                    discard;
                }

                // Depths match.  Return the color from MainTex
                fixed4 col = tex2D(_MainTex,i.uv);
                return col;
            }
            ENDCG
        }
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
                o.Alpha = tex2D (_MainTex, IN.uv_MainTex).a;
            } 
    	ENDCG
        Pass{
            // One pass of Selection Sort Order-Independent Transparency drawing
            // Fragment shader will only output if the eye depth matches the value
            // in _dtex at that point.

            CGPROGRAM
            #include "UnityCG.cginc"
            uniform sampler2D _dtex;
            uniform sampler2D _MainTex;
            uniform float _DepthTolerance;
            #pragma vertex vert
            #pragma fragment frag
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenpos : TEXCOORD1;
                float3 normal : TEXCOORD2;
                float eyedepth : TEXCOORD3;
            };
            
            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.screenpos = ComputeScreenPos(o.pos);
                o.normal = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
                o.uv = v.texcoord;
                COMPUTE_EYEDEPTH(o.eyedepth);
                return o;
            }
            
            fixed4 frag (v2f i) : COLOR {
                float target_depth = tex2Dproj(_dtex, i.screenpos);

                // FIXME: The next test is a hack to prevent an unexplained problem.  We should be testing
                // for (i.eyedepth == target_depth), i.e. exact match of depth of this fragment to a depth
                // recorded by ZSort.  However, doing this results in no pixels being rendered on the edges
                // of triangles.
                // To fix this, we consider depths to match as long as they are close.  Making _DepthTolerance
                // big enough removes the artifacts, but at the cost of potential rendering artifacts when
                // fragment depths are genuinely close but different.

                // A proper fix would require understanding why the exact match fails...

                if (abs(i.eyedepth - target_depth) > _DepthTolerance) {
                    discard;
                }

                // Depths match.  Return the color from MainTex
                fixed4 col = tex2D(_MainTex,i.uv);
                return col;
            }
            ENDCG
        }
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
                o.Alpha = tex2D (_MainTex, IN.uv_MainTex).a;
            }
        ENDCG
        Pass{
            // One pass of Selection Sort Order-Independent Transparency drawing
            // Fragment shader will only output if the eye depth matches the value
            // in _dtex at that point.

            CGPROGRAM
            #include "UnityCG.cginc"
            uniform sampler2D _dtex;
            uniform sampler2D _MainTex;
            uniform float _DepthTolerance;
            #pragma vertex vert
            #pragma fragment frag
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenpos : TEXCOORD1;
                float3 normal : TEXCOORD2;
                float eyedepth : TEXCOORD3;
            };
            
            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.screenpos = ComputeScreenPos(o.pos);
                o.normal = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
                o.uv = v.texcoord;
                COMPUTE_EYEDEPTH(o.eyedepth);
                return o;
            }
            
            fixed4 frag (v2f i) : COLOR {
                float target_depth = tex2Dproj(_dtex, i.screenpos);

                // FIXME: The next test is a hack to prevent an unexplained problem.  We should be testing
                // for (i.eyedepth == target_depth), i.e. exact match of depth of this fragment to a depth
                // recorded by ZSort.  However, doing this results in no pixels being rendered on the edges
                // of triangles.
                // To fix this, we consider depths to match as long as they are close.  Making _DepthTolerance
                // big enough removes the artifacts, but at the cost of potential rendering artifacts when
                // fragment depths are genuinely close but different.

                // A proper fix would require understanding why the exact match fails...

                if (abs(i.eyedepth - target_depth) > _DepthTolerance) {
                    discard;
                }

                // Depths match.  Return the color from MainTex
                fixed4 col = tex2D(_MainTex,i.uv);
                return col;
            }
            ENDCG
        }
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
                 o.Alpha = tex2D (_MainTex, IN.uv_MainTex).a;
            }
        ENDCG
	}
	FallBack "Diffuse"
}
