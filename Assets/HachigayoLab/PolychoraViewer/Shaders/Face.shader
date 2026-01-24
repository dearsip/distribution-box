Shader "HachigayoLab/PolychoraViewer/Face" {
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
        float3 normal : TEXCOORD3;
        float3 worldPos : TEXCOORD4;
      };

      float _Ortho;
      float _Distance;
      float _Radius;
      float _Tanfov;
      float _Inversed;
      float _Glass;
      float _Border;
      float4x4 _Axis;

      v2f vert (appdata v) {
        float4 pos4d = mul(_Axis, float4(v.vertex.xyz, v.uvs.x));
        float4 pos = float4((_Ortho ? pos4d.xyz/_Radius : pos4d.xyz/((_Distance - pos4d.w)*_Tanfov)) * 2, 1);
        if ((_Glass < 0.5) && ((mul(_Axis, v.normal).w * _Distance - v.uvs.y -  1e-9) * (-_Inversed) < 0)) {
          pos = float4(0, 0, 0, 0);
        }
        v2f o;
        o.vertex = UnityObjectToClipPos(pos);
        o.worldPos = mul(unity_ObjectToWorld, pos);
        pos4d = mul(_Axis, float4(v.center));
        pos = float4((_Ortho ? pos4d.xyz/_Radius : pos4d.xyz/((_Distance - pos4d.w)*_Tanfov)) * 2, 1);
        if (UnityObjectToClipPos(pos).w - UnityObjectToClipPos(float4(0, 0, 0, 1)).w < _Border && v.uvs.z > 0.5) {
          o.vertex = float4(0, 0, 0, 0);
        }
        o.color = v.color;
        o.normal = o.vertex.xyz;
        return o;
      }

      [maxvertexcount(3)]
      void geom (triangle v2f input[3], inout TriangleStream<v2f> outStream)
      {
        float3 normal = normalize(cross(input[1].worldPos - input[0].worldPos, input[2].worldPos - input[0].worldPos));
        normal = UnityObjectToWorldNormal(normal);
        input[0].normal = normal;
        input[1].normal = normal;
        input[2].normal = normal;
        outStream.Append(input[0]);
        outStream.Append(input[1]);
        outStream.Append(input[2]);
        outStream.RestartStrip();
      }

      fixed4 frag (v2f i) : SV_Target {
        float diff = abs(dot(i.normal, _WorldSpaceLightPos0.xyz));
        fixed4 col = i.color * (diff + 0.3) + fixed4(1, 1, 1, 1) * max(diff - 0.7, 0);
        return col;
      }
      ENDCG
    }
  }
}
