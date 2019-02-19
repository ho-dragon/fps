Shader "Unlit/TestDissolve" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_DissolveTex("Dissolve Texture", 2D) = "white" {}
		_DissolveSize("Dissolve Size",  Range(0.0, 1.0)) = 0.5
		_SliceAmount("Slice Amount", Range(0.0, 1.0)) = 0.5
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 worldPos : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _DissolveTex;
			float4 _DissolveSize;
			float _SliceAmount;

			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target {				
				clip((tex2D(_DissolveTex, i.uv)) - _SliceAmount);
				fixed4 col = tex2D(_MainTex, i.uv);
				//fixed4 col = lerp(tex2D(_MainTex, i.uv), tex2D(_DissolveTex, i.uv), _SliceAmount); //blend
				return col;
			}
			ENDCG
		}
	}
}
