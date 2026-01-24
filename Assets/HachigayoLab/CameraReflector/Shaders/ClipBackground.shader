Shader "HachigayoLab/CameraReflector/ClipBackground"
{
  Properties { _Ref("Ref", Int) = 1 }
  SubShader
  {
    Tags { "RenderType"="Transparent" "Queue"="Transparent+1" }
    ZWrite Off
    Blend Zero One, One Zero

    Pass {
      Cull Off
      ZTest Always
      Stencil {
        Ref [_Ref]
        Comp NotEqual
      }

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
      };

      v2f vert (appdata v) {
        v2f o;
        o.pos = float4(v.uv.x * 2 - 1, 1 - v.uv.y * 2, 0, 1);
        return o;
      }

      fixed4 frag (v2f i) : SV_Target {
          return fixed4(0, 0, 0, 0);
      }
      ENDCG
    }
  }
}
