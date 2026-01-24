Shader "HachigayoLab/PolychoraViewer/Line" {
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
      #pragma geometry geom
      #pragma fragment frag
      #include "UnityCG.cginc"

      struct appdata {
        float4 vertex : POSITION;
        fixed4 color : COLOR;
      };

      struct v2f {
        float4 vertex : SV_POSITION;
        fixed4 color : COLOR;
      };

      v2f vert (appdata v) {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.vertex.xy *= _ScreenParams.xy * 0.5;
        o.color = v.color;
        return o;
      }

      [maxvertexcount(4)]
      void geom(line v2f input[2], inout TriangleStream<v2f> outStream)
      {
        float2 p0 = input[0].vertex.xy / input[0].vertex.w;
        float2 p1 = input[1].vertex.xy / input[1].vertex.w;

        float2 tangent = p1 - p0;
        float sqrLength = dot(tangent, tangent);
        if (sqrLength > 0.0)
        {
          float2 normal = tangent.yx * float2(-1.0, 1.0);
          
          float scale = 2 / sqrt(sqrLength);
          normal *= scale;
          tangent *= scale;

          v2f o;
          float2 s = (_ScreenParams.zw - 1.0) * 2.0;
          o.color = input[0].color;
          o.vertex = input[0].vertex;
          o.vertex.xy = (p0 + normal / o.vertex.w - tangent) * o.vertex.w * s;
          outStream.Append(o);
          o.vertex.xy = (p0 - normal / o.vertex.w - tangent) * o.vertex.w * s;
          outStream.Append(o);
          o.vertex = input[1].vertex;
          o.vertex.xy = (p1 + normal / o.vertex.w + tangent) * o.vertex.w * s;
          outStream.Append(o);
          o.vertex.xy = (p1 - normal / o.vertex.w + tangent) * o.vertex.w * s;
          outStream.Append(o);
        }
      }

      fixed4 frag (v2f i) : SV_Target {
        return i.color;
      }
      ENDCG
    }
  }
}
