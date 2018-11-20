Shader "ZSort"
{
	Properties
	{
		_MinDepthTexture("Min Depth Texture", 2D) = "black" {} 
	}

	SubShader
	{
		ZWrite On
		ZTest LEqual
		Cull Off
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{

			CGPROGRAM
			uniform sampler2D _MinDepthTexture;

			#pragma vertex vert
			#pragma fragment frag			
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 screenpos : TEXCOORD0;
				float eyedepth : TEXCOORD1;
			};
			
			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.screenpos = ComputeScreenPos(o.pos);
				COMPUTE_EYEDEPTH(o.eyedepth);
				return o;
			}
			
			float frag (v2f i) : SV_Target
			{
				float mindepth = tex2Dproj(_MinDepthTexture, i.screenpos);
				if (i.eyedepth <= mindepth) {
					discard;
				}
				return i.eyedepth;
			}
			ENDCG
		}
	}
}
