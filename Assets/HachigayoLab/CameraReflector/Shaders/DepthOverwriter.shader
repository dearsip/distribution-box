Shader "HachigayoLab/CameraReflector/DepthOverwriter"
{
  Properties {
    _MainTex ("Texture", 2D) = "white" {}
  }
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
      v2f vert(appdata v) {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        if (_UdonMirror > .5) o.pos.x = -o.pos.x;
        o.uv = v.uv;
        return o;
      }

      float _UdonNumber;
      sampler2D _UdonDepth0;
      sampler2D _UdonDepth1;
      sampler2D _UdonDepth2;
      sampler2D _MainTex;
      float frag(v2f i) : SV_Target {
        if (_UdonNumber < .5) return max(SAMPLE_DEPTH_TEXTURE(_UdonDepth2, i.uv) * tex2D(_MainTex, i.uv).a, SAMPLE_DEPTH_TEXTURE(_UdonDepth0, i.uv));
        else return max(SAMPLE_DEPTH_TEXTURE(_UdonDepth2, i.uv) * tex2D(_MainTex, i.uv).a, SAMPLE_DEPTH_TEXTURE(_UdonDepth1, i.uv));
      }
      ENDCG
    }
  }
}