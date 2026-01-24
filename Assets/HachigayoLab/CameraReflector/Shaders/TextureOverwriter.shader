Shader "HachigayoLab/CameraReflector/TextureOverwriter"
{
  Properties {
    _Right ("Right", Float) = 0
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
      float _UdonNumber;
      float _UdonTarget;
      sampler2D _MainTex;
      sampler2D _UdonTextureB;
      sampler2D _UdonTextureL;
      sampler2D _UdonTextureR;
      sampler2D _UdonTextureP;

      v2f vert (appdata v) {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        if (_UdonMirror > .5) o.pos.x = -o.pos.x;
        o.uv = v.uv;
        return o;
      }

      fixed4 frag (v2f i) : SV_Target {
        fixed4 c = tex2D(_MainTex, i.uv);
        if (_UdonNumber > .5) return fixed4((tex2D(_UdonTextureB, i.uv) * (1 - c.a) + c * c.a).rgb, 1);
        else if (_UdonTarget < .5) return fixed4((tex2D(_UdonTextureL, i.uv) * (1 - c.a) + c * c.a).rgb, 1);
        else if (_UdonTarget < 1.5) return fixed4((tex2D(_UdonTextureR, i.uv) * (1 - c.a) + c * c.a).rgb, 1);
        else return fixed4((tex2D(_UdonTextureP, i.uv) * (1 - c.a) + c * c.a).rgb, 1);
      }
      ENDCG
    }
  }
}