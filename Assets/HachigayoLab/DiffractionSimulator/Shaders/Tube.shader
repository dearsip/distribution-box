Shader "HachigayoLab/DiffractionSimulator/Tube" {
  Properties {
		_MainTex ("Texture", 2D) = "white" {}
    _Color ("Color", Color) = (1,1,1,1)
  }
  SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 100
    Cull Off

    Pass {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"

      struct appdata {
        float4 vertex : POSITION;
        float3 normal : NORMAL;
        float2 uv : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
      };

      struct v2f {
        float4 vertex : SV_POSITION;
        float3 normal: TEXCOORD0;
        float3 light: TEXCOORD1;
        float2 uv : TEXCOORD2;
        UNITY_VERTEX_OUTPUT_STEREO
      };

      fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;

      v2f vert (appdata v) {
        v2f o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_OUTPUT(v2f, o);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.normal = UnityObjectToWorldNormal(v.normal);
        o.light = _WorldSpaceLightPos0.xyz;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
        return o;
      }

      fixed4 frag (v2f i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				;
        return tex2D(_MainTex, i.uv) * _Color * (.75 + .25 * dot(i.normal, i.light));
      }
      ENDCG
    }
  }
}
