Shader "HachigayoLab/FourMaze/UI" {
  Properties {
    _Color("Color", Color) = (1,1,1,1)
  }
  SubShader {
    Tags {"RenderType"="Opaque"}
    LOD 100

    Pass {
      Cull Off

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"

      struct appdata {
        float4 vertex : POSITION;
        float4 another : TEXCOORD0;
 				UNITY_VERTEX_INPUT_INSTANCE_ID
      };

      struct v2f {
        float4 vertex : SV_POSITION;
        fixed4 color : COLOR;
 				UNITY_VERTEX_OUTPUT_STEREO
      };

      fixed4 _Color;

      v2f vert (appdata v) {
        float4 from = mul(unity_ObjectToWorld, v.vertex);
        float4 to = mul(unity_ObjectToWorld, float4(v.another.xyz, 1));
        float3 dir = to - from;
        float3 viewDir = _WorldSpaceCameraPos - from;
        float3 normal = normalize(cross(dir, viewDir));
        from = from + float4(normal * v.another.w * .001, 0);

        v2f o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_OUTPUT(v2f, o);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
        o.vertex = UnityWorldToClipPos(from);
        o.color = _Color;
        return o;
      }

      fixed4 frag (v2f i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        return i.color;
      }
      ENDCG
    }
  }
}
