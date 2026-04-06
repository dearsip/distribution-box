Shader "HachigayoLab/DiffractionSimulator/RayOut" {
  Properties {
    _Scale("Scale", Float) = .1
    _Distance("Distance", Float) = 50
    _Frequency("Frequency", Float) = 0.64935
    _Region("Region", Float) = 4
    _Coord("Coord", Vector) = (0, 0, 0, 0)
  }
  SubShader {
    Tags {"RenderType"="Opaque"}
    LOD 100
    Cull Off

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
        float4 normal: TEXCOORD0;
        float4 light: TEXCOORD1;
        UNITY_VERTEX_OUTPUT_STEREO
      };

      float _Scale;
      float _Region;
      float _Distance;
      float _Frequency;
      float4 _Coord;
      float4x4 _Axis;
      float4x4 _InAxis;

      v2f vert (appdata v) {
        v2f o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_OUTPUT(v2f, o);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
        float4 pos = v.vertex * _Scale + mul(_Axis, v.shift + _Coord);
        float d = pos.z;
        if (v.vertex.z > 0) pos.z = _Distance;
        if (_Region - max(max(abs(v.shift.x), abs(v.shift.y)), abs(v.shift.z)) < .5) o.vertex = float4(-2,-2,0,1);
        else o.vertex = UnityObjectToClipPos(pos);
        o.normal = float4(UnityObjectToWorldNormal(v.normal), (_Distance - mul(_InAxis, v.shift + _Coord).z + pos.z - d) * _Frequency);
        o.light = float4(_WorldSpaceLightPos0.xyz, 0);
        return o;
      }

      fixed3 hsv2rgb(fixed3 c) {
        fixed4 K = fixed4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
        fixed3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
        return c.z * lerp(fixed3(1.0,1.0,1.0), saturate(p - 1.0), c.y);
      }

      fixed4 frag (v2f i) : SV_Target {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        return fixed4(hsv2rgb(fixed3(i.normal.w, 1, .8 + .5 * dot(i.normal, i.light))), 1);
      }
      ENDCG
    }
  }
}
