Shader "HachigayoLab/CameraReflector/DepthSampler"
{
  SubShader {
    Cull Off
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
        float4 pos : SV_POSITION;
        float2 uv : TEXCOORD0;
      };

      float _UdonMirror;
      float _UdonNumber;
      v2f vert(appdata v) {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        if (_UdonNumber < -.5 && _UdonMirror > .5) o.pos.x = -o.pos.x;
        o.uv = v.uv;
        return o;
      }

      sampler2D _CameraDepthTexture;
      float frag(v2f i) : SV_Target {
        return SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
      }
      ENDCG
    }
  }
}