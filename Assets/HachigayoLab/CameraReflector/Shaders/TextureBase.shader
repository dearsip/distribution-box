Shader "HachigayoLab/CameraReflector/TextureBase"
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
      float _UdonTarget;
      sampler2D _UdonTextureB;
      sampler2D _UdonTextureL;
      sampler2D _UdonTextureR;
      sampler2D _UdonTextureP;
      sampler2D _UdonDepth0;
      sampler2D _UdonDepth1;

      v2f vert(appdata v) {
        v2f o;
        o.uv = v.uv;
        if (_UdonNumber > -.5) o.pos = float4(v.uv.x * 2 - 1, 1 - v.uv.y * 2, 0, 1);
        else o.pos = float4(2, 2, 0, 1);
        return o;
      }

      float frag(v2f i, out fixed4 color : SV_Target) : SV_Depth {
        if (_UdonNumber < .5)
        {
          if (_UdonTarget < .5) color = fixed4(tex2D(_UdonTextureL, i.uv).rgb, 0);
          else if (_UdonTarget < 1.5) color = fixed4(tex2D(_UdonTextureR, i.uv).rgb, 0);
          else color = fixed4(tex2D(_UdonTextureP, i.uv).rgb, 0);
          return SAMPLE_DEPTH_TEXTURE(_UdonDepth0, i.uv);
        }
        else
        {
          color = fixed4(tex2D(_UdonTextureB, i.uv).rgb, 0);
          return SAMPLE_DEPTH_TEXTURE(_UdonDepth1, i.uv);
        }
      }
      ENDCG
    }
  }
}