Shader "HachigayoLab/PolychoraViewer/Edge" {
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
        float3 uvs : TEXCOORD0;
        float4 normal : TEXCOORD1;
        float4 center : TEXCOORD2;
      };

      struct v2f {
        float4 vertex : SV_POSITION;
        fixed4 color : COLOR;
      };

      float _Ortho;
      float _Distance;
      float _Radius;
      float _Tanfov;
      float _Inversed;
      float _Glass;
      float4x4 _Axis;

      v2f vert (appdata v) {
        float4 pos4d = mul(_Axis, float4(v.vertex.xyz, v.uvs.x));
        float4 pos = float4((_Ortho ? pos4d.xyz/_Radius : pos4d.xyz/((_Distance - pos4d.w)*_Tanfov)) * 2, 1);
        if ((_Glass < 0.5) && ((mul(_Axis, v.normal).w * _Distance - v.uvs.y -  1e-9) * (-_Inversed) < 0)) {
          pos = float4(0, 0, 0, 0);
        }
        v2f o;
        o.vertex = UnityObjectToClipPos(pos);
        o.color = v.color;
        o.vertex.xy *= _ScreenParams.xy * 0.5;
        return o;
      }

      float _Width;

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
          
          float scale = _Width / sqrt(sqrLength);
          normal *= scale;
          tangent *= scale;

          v2f o;
          o.color = input[0].color;
          float2 s = (_ScreenParams.zw - 1.0) * 2.0;
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
