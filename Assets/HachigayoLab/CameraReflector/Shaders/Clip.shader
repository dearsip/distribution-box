Shader "HachigayoLab/CameraReflector/Clip"
{
  Properties { _Ref("Ref", Int) = 1 }
  SubShader
  {
    Tags { "RenderType"="Transparent" "Queue"="Transparent" }
    ZWrite Off

    CGINCLUDE
    #pragma vertex vert
    #pragma fragment frag
    #include "UnityCG.cginc"

    struct appdata {
      float4 vertex : POSITION;
    };

    struct v2f {
      float4 pos : SV_POSITION;
    };

    v2f vert (appdata v) {
      v2f o;
      o.pos = UnityObjectToClipPos(v.vertex);
      return o;
    }

    fixed4 frag (v2f i) : SV_Target {
        return fixed4(0, 0, 0, 0);
    }
    ENDCG

    Pass {
      Cull Front
      ZTest Less
      Blend Zero One, One Zero

      CGPROGRAM
      ENDCG
    }

    Pass {
      Cull Back
      ZTest GEqual
      Blend Zero One, One Zero

      CGPROGRAM
      ENDCG
    }

    Pass {
      Cull Front
      ZTest Always
      Blend Zero One

      Stencil {
        Ref [_Ref]
        Comp Always
        Pass Replace
      }

      CGPROGRAM
      ENDCG
    }
  }
}
