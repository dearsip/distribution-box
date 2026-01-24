Shader "HachigayoLab/CameraReflector/Integrate"
{
  SubShader {
    Tags { "RenderType"="Opaque" }
    ZWrite Off
    ZTest Always
    Cull Off

    Pass {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"

      struct appdata {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
      };

      struct v2f {
        float4 pos : SV_POSITION;
        float2 uv : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
      };

      sampler2D _UdonTextureL;
      sampler2D _UdonTextureR;

      v2f vert(appdata v) {
        v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
        o.uv = v.uv;
        o.pos = float4(v.uv.x * 2 - 1, 1 - v.uv.y * 2, 0, 1);
        return o;
      }

      fixed4 frag (v2f i) : SV_Target {
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        if (unity_StereoEyeIndex == 0) return tex2D(_UdonTextureL, i.uv);
        else return tex2D(_UdonTextureR, i.uv);
      }
      ENDCG
    }
  }
}