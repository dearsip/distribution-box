Shader "HachigayoLab/CameraReflector/TextureFirstwriter"
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
      sampler2D _MainTex;
      float4 _MainTex_ST;

      v2f vert (appdata v) {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        if (_UdonMirror > .5) o.pos.x = -o.pos.x;
        o.uv = v.uv;
        return o;
      }

      fixed4 frag (v2f i) : SV_Target {
        return fixed4(tex2D(_MainTex, i.uv).rgb, 1);
      }
      ENDCG
    }
  }
}