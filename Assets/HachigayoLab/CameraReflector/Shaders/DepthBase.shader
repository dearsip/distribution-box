Shader "HachigayoLab/CameraReflector/DepthBase"
{
  SubShader {
    Tags { "RenderType"="Opaque" "Queue"="Background" }
    ZWrite On
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
      };

      struct v2f {
        float4 pos : SV_POSITION;
        float2 uv : TEXCOORD0;
      };

      float _UdonNumber;
      sampler2D _UdonDepth0;
      sampler2D _UdonDepth1;

      v2f vert(appdata v) {
        v2f o;
        o.uv = v.uv;
        if (_UdonNumber > -.5) o.pos = float4(v.uv.x * 2 - 1, 1 - v.uv.y * 2, 0, 1);
        else o.pos = float4(2, 2, 0, 1);
        return o;
      }

      float frag(v2f i) : SV_Depth {
        if (_UdonNumber < .5) return SAMPLE_DEPTH_TEXTURE(_UdonDepth0, i.uv);
        else return SAMPLE_DEPTH_TEXTURE(_UdonDepth1, i.uv);
      }
      ENDCG
    }
  }
}