Shader "HachigayoLab/4DSDFSliceStacker/4S3_Static" {
  Properties {
    _Global ("Global", Float) = 1
    _Local ("Local", Float) = 1
    _Stereo ("Stereo", Float) = 0
    _Shape ("Shape", Float) = 0
    _Custom ("Custom", Float) = 0
    _Frame ("Frame", Float) = .1
    _Scale ("Scale", Float) = 1
    _FoV ("FoV", Float) = 0.1666666
    _Thickness ("Thickness", Float) = 1
    _Volumetric ("Volumetric", Float) = 0
    _Back ("Back", Float) = .7
    _Front ("Front", Float) = 0
    _Occlusion ("Occlusion", Float) = 0
    _Steps1 ("Steps1", Float) = 6
    _Steps2 ("Steps2", Float) = 6
    _Stride1 ("Stride1", Float) = 1
    _Stride2 ("Stride2", Float) = 1
    _SlideSpeed1 ("SlideSpeed1", Float) = .1
    _SlideSpeed2 ("SlideSpeed2", Float) = .2
    _Offset1 ("Offset1", Float) = 0
    _Offset2 ("Offset2", Float) = 0
    _Ax ("Ax", Float) = 1
    _Ay ("Ay", Float) = 1
    _Az ("Az", Float) = 1
    _Aw ("Aw", Float) = 1
    _E1 ("E1", Float) = 1
    _E2 ("E2", Float) = 1
    _E3 ("E3", Float) = 1
    _R1 ("R1", Float) = .9
    _R2 ("R2", Float) = .3
    _R3 ("R3", Float) = .1
    _SliceRotXY ("SliceRotXY", Float) = 0
    _SliceRotYZ ("SliceRotYZ", Float) = 0
    _SliceRotZX ("SliceRotZX", Float) = 0
    _SliceRotXW ("SliceRotXW", Float) = 0
    _SliceRotYW ("SliceRotYW", Float) = 0
    _SliceRotZW ("SliceRotZW", Float) = 0
    _ObjectRotXY ("ObjectRotXY", Float) = 0
    _ObjectRotYZ ("ObjectRotYZ", Float) = 0
    _ObjectRotZX ("ObjectRotZX", Float) = 0
    _ObjectRotXW ("ObjectRotXW", Float) = 0
    _ObjectRotYW ("ObjectRotYW", Float) = 0
    _ObjectRotZW ("ObjectRotZW", Float) = 0
  }
  SubShader {
    Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" "DisableBatching" = "True"}
    LOD 100

    Pass {
      Cull Back
      ZWrite Off
      ZTest Off
      Blend SrcAlpha OneMinusSrcAlpha

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"
      #include "Lighting.cginc"
      #define PI 3.14159265

      struct appdata {
        float4 vertex : POSITION;
        UNITY_VERTEX_INPUT_INSTANCE_ID
      };

      struct v2f {
        float4 vertex : SV_POSITION;
        float3 localPos : NORMAL;
        float4 projPos : TEXCOORD0;
        float4 slice0 : TANGENT;
        float4 slice1 : TEXCOORD1;
        float4 slice2 : TEXCOORD2;
        float4 slice3 : TEXCOORD3;
        float4 object0 : TEXCOORD4;
        float4 object1 : TEXCOORD5;
        float4 object2 : TEXCOORD6;
        float4 object3 : TEXCOORD7;
        UNITY_VERTEX_OUTPUT_STEREO
      };

      float _Global;
      float _Local;
      float _Stereo;
      float _Shape;
      float _Custom;
      float _Frame;
      float _Scale;
      float _FoV;
      float _Thickness;
      float _Volumetric;
      float _Back;
      float _Front;
      float _Occlusion;
      float _Steps1;
      float _Steps2;
      float _Stride1;
      float _Stride2;
      float _SlideSpeed1;
      float _SlideSpeed2;
      float _Offset1;
      float _Offset2;
      float _Ax;
      float _Ay;
      float _Az;
      float _Aw;
      float _E1;
      float _E2;
      float _E3;
      float _R1;
      float _R2;
      float _R3;
      float _SliceRotXY;
      float _SliceRotYZ;
      float _SliceRotZX;
      float _SliceRotXW;
      float _SliceRotYW;
      float _SliceRotZW;
      float _ObjectRotXY;
      float _ObjectRotYZ;
      float _ObjectRotZX;
      float _ObjectRotXW;
      float _ObjectRotYW;
      float _ObjectRotZW;
      UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

      float4 Reflect(float4 src, float4 normal) {
        return src - 2 * dot(src, normal) * normal;
      }

      float4 Rotate(float4 src, float4 from, float4 tohalf) {
        return Reflect(Reflect(src, from), tohalf);
      }

      float4 RotV(float4 src, float4 rot) {
        return Rotate(src, float4(0, 0, 0, 1), rot);
      }

      v2f vert (appdata v) {
        v2f o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_OUTPUT(v2f, o);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        float3 dir = float3(_SliceRotXW, _SliceRotYW, _SliceRotZW);
        float t = length(dir) * PI;
        float4 br = float4(0, 0, 0, 1);
        if (t > 1e-9) br = float4(-sin(t) * normalize(dir), cos(t));
        o.slice0 = 
        Rotate(
        Rotate(
            Rotate(
            Rotate(
                float4(1, 0, 0, 0),
                float4(1, 0, 0, 0), float4(cos(_SliceRotXY*PI), sin(_SliceRotXY*PI), 0, 0)
            ),
            float4(0, 1, 0, 0), float4(0, cos(_SliceRotYZ*PI), sin(_SliceRotYZ*PI), 0)
            ),
            float4(0, 0, 1, 0), float4(sin(_SliceRotZX*PI), 0, cos(_SliceRotZX*PI), 0)
        ),
        float4(0, 0, 0, 1), br
        );
        o.slice1 = Rotate( Rotate( Rotate( Rotate( float4(0, 1, 0, 0), float4(1, 0, 0, 0), float4(cos(_SliceRotXY*PI), sin(_SliceRotXY*PI), 0, 0)), float4(0, 1, 0, 0), float4(0, cos(_SliceRotYZ*PI), sin(_SliceRotYZ*PI), 0)), float4(0, 0, 1, 0), float4(sin(_SliceRotZX*PI), 0, cos(_SliceRotZX*PI), 0)), float4(0, 0, 0, 1), br);
        o.slice2 = Rotate( Rotate( Rotate( Rotate( float4(0, 0, 1, 0), float4(1, 0, 0, 0), float4(cos(_SliceRotXY*PI), sin(_SliceRotXY*PI), 0, 0)), float4(0, 1, 0, 0), float4(0, cos(_SliceRotYZ*PI), sin(_SliceRotYZ*PI), 0)), float4(0, 0, 1, 0), float4(sin(_SliceRotZX*PI), 0, cos(_SliceRotZX*PI), 0)), float4(0, 0, 0, 1), br);
        o.slice3 = Rotate( Rotate( Rotate( Rotate( float4(0, 0, 0, 1), float4(1, 0, 0, 0), float4(cos(_SliceRotXY*PI), sin(_SliceRotXY*PI), 0, 0)), float4(0, 1, 0, 0), float4(0, cos(_SliceRotYZ*PI), sin(_SliceRotYZ*PI), 0)), float4(0, 0, 1, 0), float4(sin(_SliceRotZX*PI), 0, cos(_SliceRotZX*PI), 0)), float4(0, 0, 0, 1), br);

        dir = float3(_ObjectRotXW, _ObjectRotYW, _ObjectRotZW);
        t = length(dir) * PI;
        br = float4(0, 0, 0, 1);
        if (t > 1e-9) br = float4(-sin(t) * normalize(dir), cos(t));
        o.object0 = 
          Rotate(
            Rotate(
              Rotate(
                Rotate(
                  float4(1, 0, 0, 0),
                  float4(1, 0, 0, 0), float4(cos(_ObjectRotXY*PI), sin(_ObjectRotXY*PI), 0, 0)
                ),
                float4(0, 1, 0, 0), float4(0, cos(_ObjectRotYZ*PI), sin(_ObjectRotYZ*PI), 0)
              ),
              float4(0, 0, 1, 0), float4(sin(_ObjectRotZX*PI), 0, cos(_ObjectRotZX*PI), 0)
            ),
            float4(0, 0, 0, 1), br
          ) ;
        o.object1 = Rotate( Rotate( Rotate( Rotate( float4(0, 1, 0, 0), float4(1, 0, 0, 0), float4(cos(_ObjectRotXY*PI), sin(_ObjectRotXY*PI), 0, 0)), float4(0, 1, 0, 0), float4(0, cos(_ObjectRotYZ*PI), sin(_ObjectRotYZ*PI), 0)), float4(0, 0, 1, 0), float4(sin(_ObjectRotZX*PI), 0, cos(_ObjectRotZX*PI), 0)), float4(0, 0, 0, 1), br);
        o.object2 = Rotate( Rotate( Rotate( Rotate( float4(0, 0, 1, 0), float4(1, 0, 0, 0), float4(cos(_ObjectRotXY*PI), sin(_ObjectRotXY*PI), 0, 0)), float4(0, 1, 0, 0), float4(0, cos(_ObjectRotYZ*PI), sin(_ObjectRotYZ*PI), 0)), float4(0, 0, 1, 0), float4(sin(_ObjectRotZX*PI), 0, cos(_ObjectRotZX*PI), 0)), float4(0, 0, 0, 1), br);
        o.object3 = Rotate( Rotate( Rotate( Rotate( float4(0, 0, 0, 1), float4(1, 0, 0, 0), float4(cos(_ObjectRotXY*PI), sin(_ObjectRotXY*PI), 0, 0)), float4(0, 1, 0, 0), float4(0, cos(_ObjectRotYZ*PI), sin(_ObjectRotYZ*PI), 0)), float4(0, 0, 1, 0), float4(sin(_ObjectRotZX*PI), 0, cos(_ObjectRotZX*PI), 0)), float4(0, 0, 0, 1), br);

        o.vertex = UnityObjectToClipPos(float4(v.vertex.xyz * _Frame * 10, 1));
        o.projPos = ComputeScreenPos(o.vertex);
        COMPUTE_EYEDEPTH(o.projPos.z);
        o.localPos = v.vertex.xyz;
        return o;
      }

      fixed3 hsv2rgb(fixed3 c) {
        fixed4 K = fixed4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
        fixed3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
        return c.z * lerp(fixed3(1.0,1.0,1.0), saturate(p - 1.0), c.y);
      }

      float dist(float4 pos) {
        float shape = _Shape + _Custom;
        if (shape <  .5) return length(pos) - .98; // hypersphere
        if (shape < 1.5) return sqrt(pow(sqrt(pos.x * pos.x + pos.z * pos.z) - .6, 2) + pos.y * pos.y + pos.w * pos.w) - .3; // spheritorus
        if (shape < 2.5) return sqrt(pow(sqrt(pos.x * pos.x + pos.y * pos.y + pos.z * pos.z) - .6, 2) + pos.w * pos.w) - .3; // torisphere
        if (shape < 3.5) return sqrt(pow(sqrt(pos.x * pos.x + pos.y * pos.y) - .5, 2) + pow(sqrt(pos.z * pos.z + pos.w * pos.w) - .5, 2)) - .2; // tiger
        if (shape < 4.5) return sqrt(pow(sqrt(pow(sqrt(pos.x * pos.x + pos.z * pos.z) - .5, 2) + pos.y * pos.y) - .3, 2) + pos.w * pos.w) - .1; // ditorus
        if (shape < 5.5) return sqrt(pos.x * pos.x + pos.y * pos.y + pos.w * pos.w) - sqrt(pos.z * pos.z + .36); // spherihyperboloid
        if (shape < 6.5) return -sqrt(pos.x * pos.x + pos.y * pos.y) + sqrt(pos.z * pos.z + pos.w * pos.w + .36); // hyperhyperboloid
        if (shape < 7.5) return pow(pow(pow(pow(abs(pos.x) / _Ax, 2 / _E1) + pow(abs(pos.y) / _Ay, 2 / _E1), _E1 / _E2) + pow(abs(pos.z) / _Az, 2 / _E2), _E2 / _E3) + pow(abs(pos.w) / _Aw, 2 / _E3), _E3 / 2) - _R1;
        if (shape < 8.5) return pow(pow(pow(pow(pow(abs(pos.x) / _Ax, 2 / _E1) + pow(abs(pos.y) / _Ay, 2 / _E1), _E1 / _E2) + pow(abs(pos.z) / _Az, 2 / _E2), _E2 / 2) - _R1, 2 / _E3) + pow(abs(pos.w) / _Aw, 2 / _E3), _E3 / 2) - _R2;
        if (shape < 9.5) return pow(pow(pow(pow(pow(abs(pos.x) / _Ax, 2 / _E1) + pow(abs(pos.y) / _Ay, 2 / _E1), _E1 / 2) - _R1, 2 / _E2) + pow(abs(pos.z) / _Az, 2 / _E2), _E2 / _E3) + pow(abs(pos.w) / _Aw, 2 / _E3), _E3 / 2) - _R2;
        if (shape < 10.5) return pow(pow(pow(pow(abs(pos.x) / _Ax, 2 / _E1) + pow(abs(pos.y) / _Ay, 2 / _E1), _E1 / 2) - _R1, 2 / _E3) + pow(pow(pow(abs(pos.z) / _Az, 2 / _E2) + pow(abs(pos.w) / _Aw, 2 / _E2), _E2 / 2) - _R2, 2 / _E3), _E3 / 2) - _R3;
        if (shape < 11.5) return pow(pow(pow(pow(pow(pow(abs(pos.x) / _Ax, 2 / _E1) + pow(abs(pos.y) / _Ay, 2 / _E1), _E1 / 2) - _R1, 2 / _E2) + pow(abs(pos.z) / _Az, 2 / _E2), _E2 / 2) - _R2, 2 / _E3) + pow(abs(pos.w) / _Aw, 2 / _E3), _E3 / 2) - _R3;
        if (shape < 12.5) return pow(pow(pow(abs(pos.x) / _Ax, 2 / _E1) + pow(abs(pos.y) / _Ay, 2 / _E1), _E1 / _E2) + pow(abs(pos.z) / _Az, 2 / _E2) + pow(_R1, 2 / _E2), _E2 / 2) - pow(pow(abs(pos.w) / _Aw, 2 / _E3) + pow(_R2, 2 / _E3), _E3 / 2);
        if (shape < 13.5) return pow(pow(abs(pos.x) / _Ax, 2 / _E1) + pow(abs(pos.y) / _Ay, 2 / _E1) + pow(_R1, 2 / _E1), _E1 / 2) - pow(pow(abs(pos.z) / _Az, 2 / _E2) + pow(abs(pos.w) / _Aw, 2 / _E2) + pow(_R2, 2 / _E2), _E2 / 2);
        return length(pos) - .98;
      }

      fixed4 frag (v2f input) : SV_Target {
        if (_Local < .5 && _Global < .5) return 0;
        float4x4 SliRot = float4x4(input.slice0, input.slice1, input.slice2, input.slice3);
        float4x4 ObjRot = transpose(float4x4(input.object0, input.object1, input.object2, input.object3));
        float FoV = _FoV * PI;
        float Scale = cos(FoV);
        FoV = sin(FoV);

        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        float4 dpos = float4(input.localPos.xyz / Scale * 2, 0);
        Scale = 1 / (Scale * _Frame * 5);
        float4 cpos = float4(mul(unity_WorldToObject, float4(_WorldSpaceCameraPos,1)).xyz * Scale, 0);
        if (_Stereo > .5)
        {
          float length_ = length(dpos-cpos) * 3 * _Frame;
          if (dpos.x > 0)
          {
            dpos.x = dpos.x - length_;
            cpos.x = cpos.x - .3 * length_;
          }
          else
          {
            dpos.x = dpos.x + length_;
            cpos.x = cpos.x + .3 * length_;
          }
        }
        float4 ddir = float4(dpos.xyz * FoV,1);
        float4 cdir = float4(cpos.xyz * FoV,1);
        float4 cam4 = float4(0,0,0,FoV > 0 ? -1/FoV : -1);
        float4 ndir = normalize(dpos - cpos);
        float nthr = dot(ndir, cpos);
        float near = _ProjectionParams.y * Scale;
        float depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(input.projPos))) * Scale / dot(mul(unity_ObjectToWorld, ndir.xyz), -UNITY_MATRIX_V[2].xyz);
        float4x4 invrot = transpose(SliRot);
        dpos = mul(invrot, dpos);
        cpos = mul(invrot, cpos);
        ddir = mul(invrot, ddir);
        cdir = mul(invrot, cdir);
        cam4 = mul(invrot, cam4);
        float4 ddrz = ddir / ddir.z;
        float4 cdrz = cdir / cdir.z;
        ddir = ddir / ddir.w;
        cdir = cdir / cdir.w;
        float step1 = 2 / _Steps1 * _Stride1;
        float step2 = 2 / _Steps2 * _Stride2;
        float4 dirz = (dpos - ddrz * dpos.z) - (cpos - cdrz * cpos.z);
        float4 dirzt = dirz * sign(dirz.w) / max(abs(dirz.w), 0.01);
        dirz = dirz / dirz.w;
        dpos = dpos + ddir * (-dpos.w - _Stride1 + frac(_Time.y * _SlideSpeed1 + .5) * step1 + _Offset1);
        cpos = cpos + cdir * (-cpos.w - _Stride1 + frac(_Time.y * _SlideSpeed1 + .5) * step1 + _Offset1);
        fixed4 output = 0;
        fixed4 color = 0;
        float4 pos2 = 0;
        float4 dir2 = 0;
        float4 dir2t = 0;
        float c = 0;
        float4 reg = 0;
        float d = 0;
        float4 lpos = 0;

        float4 from = 0;
        float4 to = 0;
        float4 vec = 0;
        float4 mid = 0;
        float l = 0;
        float f = 0;
        float m = 0;
        float t = 0;
        float ct = 0;
        float ft = 0;
        float tt = 0;
        float txy = 0;
        float thickness = 1 / _Thickness;
        float fpn = dist(mul(ObjRot, mul(SliRot, cam4)));

        //[unroll]
        for (int i = 0; i < _Steps1; i++)
        {
          dir2 = dpos - cpos;
          dir2t = dir2 * sign(dir2.z) / max(abs(dir2.z), 0.01);
          dir2 = dir2 / dir2.z;
          pos2 = cpos + dir2 * (-cpos.z - _Stride2 + frac(_Time.y * _SlideSpeed2 + .5) * step2 + _Offset2);
          //[unroll]
          for (int j = 0; j < _Steps2; j++)
          {
            lpos = pos2;
            if (all(abs(lpos) < 1))
            {
              d = dist(mul(ObjRot, mul(SliRot, lpos)));
              c = 1 - pow(d * 100 * thickness, 2);
              vec = normalize(FoV > 0 ? lpos-cam4 : -cam4);
              if ((d - dist(mul(ObjRot, mul(SliRot, lpos + vec * 1e-3)))) * fpn > 0) c *= 1 - _Front;
              else c *= 1 - _Back;

              if (_Volumetric > .5)
              {
                from = pos2 - dir2t * .012 * _Thickness;
                to = pos2 + dir2t * .012 * _Thickness;
                vec = to - from;
                if (abs(vec.x) > 0)
                {
                  mid = vec / vec.x;
                  ft = from.x;
                  tt = to.x;
                  txy = 0;
                  from = from + mid * (-from.x + clamp(from.x, -1, 1));
                  to   = to   + mid * (-  to.x + clamp(  to.x, -1, 1));
                }
                if (abs(vec.y) > 0)
                {
                  mid = vec / vec.y;
                  ft = from.y;
                  tt = to.y;
                  txy = 1;
                  from = from + mid * (-from.y + clamp(from.y, -1, 1));
                  to   = to   + mid * (-  to.y + clamp(  to.y, -1, 1));
                }
                vec = to - from;
                l = length(vec);
                if (l > 1e-2)
                {
                  f = dist(mul(ObjRot, mul(SliRot, from)));
                  t = f;
                  vec = vec / l;
                  //[unroll]
                  for (int k = 0; k < 40; k++)
                  {
                    mid = from + vec * t;
                    m = dist(mul(ObjRot, mul(SliRot, mid)));
                    if (f * m < 0)
                    {
                      ct = 1 - pow(2 * ((txy < .5 ? mid.x : mid.y) - ft) / (tt - ft) - 1, 2);
                      vec = normalize(FoV > 0 ? mid-cam4 : -cam4);
                      if ((m - dist(mul(ObjRot, mul(SliRot, mid + vec * 1e-3)))) * fpn > 0) ct *= 1 - _Front;
                      else ct *= 1 - _Back;
                      if (ct > c)
                      {
                        c = ct;
                        d = m;
                        lpos = mid;
                      }
                    }
                    t += max(abs(m), 1e-3);
                    if (t > l) break;
                  }
                }

                from = pos2 - dirzt * .012 * _Thickness;
                to = pos2 + dirzt * .012 * _Thickness;
                vec = to - from;
                if (abs(vec.x) > 0)
                {
                  mid = vec / vec.x;
                  ft = from.x;
                  tt = to.x;
                  txy = 0;
                  from = from + mid * (-from.x + clamp(from.x, -1, 1));
                  to   = to   + mid * (-  to.x + clamp(  to.x, -1, 1));
                }
                if (abs(vec.y) > 0)
                {
                  mid = vec / vec.y;
                  ft = from.y;
                  tt = to.y;
                  txy = 1;
                  from = from + mid * (-from.y + clamp(from.y, -1, 1));
                  to   = to   + mid * (-  to.y + clamp(  to.y, -1, 1));
                }
                vec = to - from;
                l = length(vec);
                if (l > 1e-2)
                {
                  f = dist(mul(ObjRot, mul(SliRot, from)));
                  t = f;
                  vec = vec / l;
                  //[unroll]
                  for (int k = 0; k < 40; k++)
                  {
                    mid = from + vec * t;
                    m = dist(mul(ObjRot, mul(SliRot, mid)));
                    if (f * m < 0)
                    {
                      ct = 1 - pow(2 * ((txy < .5 ? mid.x : mid.y) - ft) / (tt - ft) - 1, 2);
                      vec = normalize(FoV > 0 ? mid-cam4 : -cam4);
                      if ((m - dist(mul(ObjRot, mul(SliRot, mid + vec * 1e-3)))) * fpn > 0) ct *= 1 - _Front;
                      else ct *= 1 - _Back;
                      if (ct > c)
                      {
                        c = ct;
                        d = m;
                        lpos = mid;
                      }
                    }
                    t += max(abs(m), 1e-3);
                    if (t > l) break;
                  }
                }
              }
            }
            else c = 0;

            if (c > 0)
            {
              reg = mul(SliRot, lpos);
              reg.xyz /= 1 + reg.w * FoV;
              t = dot(reg, ndir) - nthr;
              if (t > near && t < depth)
              {
                if (_Occlusion > 0.01)
                {
                  vec = normalize(FoV > 0 ? lpos-cam4 : -cam4);
                  t = .1;
                  //[unroll]
                  for (int k = 0; k < 40; k++)
                  {
                    mid = lpos - vec * t;
                    m = dist(mul(ObjRot, mul(SliRot, mid)));
                    if (m < 1e-3)
                    {
                      c *= 1 - _Occlusion;
                      break;
                    }
                    t += max(abs(m), 1e-3);
                    if (t > 2) break;
                  }
                }

                color = fixed4(hsv2rgb(fixed3((pos2.z / _Stride2 + 1) * .5, (pos2.w / _Stride1 + 3) * .25, c)), c);
                output = max(output, color);
              }
            }
            pos2 += dir2 * step2;
          }
          dpos += ddir * step1;
          cpos += cdir * step1;
        }

        return output;
      }
      ENDCG
    }
  }
}
