Shader "HachigayoLab/PolychoraViewer/FaceQuest" {
  SubShader {
    Tags {"RenderType"="Opaque"}
    LOD 100

    Pass
    {
      Tags { "LightMode"="ShadowCaster" }
      ZWrite On
      ColorMask 0
    }

    Pass {
      Cull Off

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"

      struct appdata {
        float4 vertex : POSITION;
        fixed4 color : COLOR;
      };

      struct v2f {
        float4 vertex : SV_POSITION;
        fixed4 color : COLOR;
        float3 worldPos : TEXCOORD4;
      };

      v2f vert (appdata v) {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.color = v.color;
        o.worldPos = mul(unity_ObjectToWorld, o.vertex).xyz;
        return o;
      }

      fixed4 frag (v2f i) : SV_Target {
        fixed4 col = i.color * 0.8;
        return col;
      }
      ENDCG
    }
  }
}
