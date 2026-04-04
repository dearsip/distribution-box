Shader "HachigayoLab/DiffractionSimulator/Crystal" {
  Properties {
    _Color("Color", Color) = (1,1,1,1)
    _Scale("Scale", Float) = .5
    _Region("Region", Float) = 4
    _Coord("Coord", Vector) = (0, 0, 0, 0)
  }
  SubShader {
    Tags {"RenderType"="Opaque"}
    LOD 100

    Pass {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"

      struct appdata {
        float4 vertex : POSITION;
        float3 normal : NORMAL;
        float3 shift : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
      };

      struct v2f {
        float4 vertex : SV_POSITION;
        float3 normal: TEXCOORD0;
        float3 light: TEXCOORD1;
        UNITY_VERTEX_OUTPUT_STEREO
      };

      float _Scale;
      float _Region;
      float4 _Coord;
      float4x4 _Axis;
      fixed4 _Color;

      v2f vert (appdata v) {
        v2f o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_OUTPUT(v2f, o);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
        if (_Region - max(max(abs(v.shift.x), abs(v.shift.y)), abs(v.shift.z)) < .5) o.vertex = float4(-2,-2,0,1);
        else o.vertex = UnityObjectToClipPos(v.vertex * _Scale + mul(_Axis, v.shift + _Coord));
        o.normal = UnityObjectToWorldNormal(v.normal);
        o.light = _WorldSpaceLightPos0.xyz;
        return o;
      }

      fixed4 frag (v2f i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        return _Color * (.75 + .25 * dot(i.normal, i.light));
      }
      ENDCG
    }
  }
}
