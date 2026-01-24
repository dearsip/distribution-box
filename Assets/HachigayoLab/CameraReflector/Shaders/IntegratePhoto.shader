Shader "HachigayoLab/CameraReflector/IntegratePhoto"
{
  SubShader {
    Tags { "RenderType"="Transparent" "Queue"="Transparent+3000" }
    ZWrite Off
    ZTest Always
    Cull Off
    Blend SrcAlpha OneMinusSrcAlpha

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

      float _VRChatCameraMode;
      sampler2D _UdonTextureP;

      v2f vert(appdata v) {
        v2f o;
        o.uv = v.uv;
        if (_VRChatCameraMode > .5 && _VRChatCameraMode < 2.5) o.pos = float4(v.uv.x * 2 - 1, 1 - v.uv.y * 2, 0, 1);
        else o.pos = float4(2, 2, 0, 1);
        return o;
      }

      fixed4 frag (v2f i) : SV_Target {
        return tex2D(_UdonTextureP, i.uv);
      }
      ENDCG
    }
  }
}