Shader "HachigayoLab/FourMaze/FourMaze" {
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
        float4 position : TEXCOORD0;
        float4 another : TEXCOORD1;
 				UNITY_VERTEX_INPUT_INSTANCE_ID
      };

      struct v2f {
        float4 vertex : SV_POSITION;
        fixed4 color : COLOR;
 				UNITY_VERTEX_OUTPUT_STEREO
      };

      float4 _Center;
      float4x4 _Axis;
      float4 _ScaRet;
      fixed4 _Color;
      int _Wall;
      float4 _Clips[5];
      int _UseClip;

      v2f vert (appdata v) {
        int n = (int)(v.vertex.x);
        if ((_Wall & (1 << n)) != 0)
        {
          v2f o;
          UNITY_SETUP_INSTANCE_ID(v);
          UNITY_INITIALIZE_OUTPUT(v2f, o);
          UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
          o.vertex = float4(-2,-2,0,1);
          o.color = _Color;
          return o;
        }

        float4 from = v.position + _Center;
        float4 to = v.another + _Center;

        float f, t, fmax = 0, tmin = 1;

        [unroll]
        for (int i = 0; i < 5; i++)
        {
          if ((_UseClip & (1 << i)) != 0)
          {
            f = dot(_Clips[i], from);
            t = dot(_Clips[i], to);
            if (f < 0) { if (t >= 0) fmax = max(fmax, f / (f - t)); else fmax = 1; }
            else if (t < 0) tmin = min(tmin, f / (f - t));
          }
        }

        float4 fromr = mul(_Axis, from);
        float4 tor = mul(_Axis, to);

        [unroll]
        for (int i = 0; i < 3; i++)
        {
          f = _ScaRet.w * fromr.w + fromr[i]; t = _ScaRet.w * tor.w + tor[i];
          if (f < 0) { if (t >= 0) fmax = max(fmax, f / (f - t)); else fmax = 1; }
          else if (t < 0) tmin = min(tmin, f / (f - t));
          f = _ScaRet.w * fromr.w - fromr[i]; t = _ScaRet.w * tor.w - tor[i];
          if (f < 0) { if (t >= 0) fmax = max(fmax, f / (f - t)); else fmax = 1; }
          else if (t < 0) tmin = min(tmin, f / (f - t));
        }

        if (tmin - fmax < 0.000001)
        {
          v2f o;
          UNITY_SETUP_INSTANCE_ID(v);
          UNITY_INITIALIZE_OUTPUT(v2f, o);
          UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
          o.vertex = float4(-2,-2,0,1);
          o.color = _Color;
          return o;
        }

        fromr = fromr * (1 - fmax) + tor * fmax;
        from = fromr * _ScaRet / (_ScaRet.w * fromr.w);
        from.w = 1;
        tor = fromr * (1 - tmin) + tor * tmin;
        to = tor * _ScaRet / (_ScaRet.w * tor.w);
        to.w = 1;

        from = mul(unity_ObjectToWorld, from);
        to = mul(unity_ObjectToWorld, to);
        float3 dir = to - from;
        float3 viewDir = _WorldSpaceCameraPos - from;
        float3 normal = normalize(cross(dir, viewDir));
        from = from + float4(normal * v.vertex.y * .001, 0);

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
