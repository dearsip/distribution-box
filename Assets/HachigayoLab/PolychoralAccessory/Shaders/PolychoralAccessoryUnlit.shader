Shader "HachigayoLab/PolychoralAccessoryUnlit" {
  Properties {
    _MainTex ("Texture", 2D) = "white" {}
    _Ortho ("Ortho", Float) = 0
    _Distance ("Distance", Float) = 5.4641016151378
    _Radius ("Radius", Float) = 1.4142135623731
    _Tanfov ("Tanfov", Float) = 0.2679491924311
    // _Inversed ("Inversed", Float) = 0
    // _Glass ("Glass", Float) = 0
    _Scale ("Scale", Float) = 0.1
    _RotationSpeed ("RotationSpeed", Float) = 4
    _InitialRotation1 ("InitialRotation1", Vector) = (1, 0, 0, 0)
    _InitialRotation2 ("InitialRotation2", Vector) = (0, 1, 0, 0)
    _InitialRotation3 ("InitialRotation3", Vector) = (0, 0, 1, 0)
    _InitialRotation4 ("InitialRotation4", Vector) = (0, 0, 0, 1)
    _AutoRot1From ("AutoRot1From", Vector) = (1, 0, 0, 0)
    _AutoRot1To ("AutoRot1To", Vector) = (0, 1, 0, 0) // Perpendicular to From vector
    _AutoRot1Speed ("AutoRot1Speed", Float) = 0
    _AutoRot2From ("AutoRot2From", Vector) = (0, 0, 1, 0)
    _AutoRot2To ("AutoRot2To", Vector) = (0, 0, 0, 1)
    _AutoRot2Speed ("AutoRot2Speed", Float) = 0
  }
  SubShader {
    Tags {"RenderType"="Opaque"}
    LOD 100

    Pass
    {
      Tags { "LightMode"="ShadowCaster" }
      Cull Off
      ZWrite On
      ColorMask 0

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #pragma multi_compile_fog
      #include "UnityCG.cginc"

      struct appdata {
        float4 vertex : POSITION; // (0,0,0,1) in mesh, reflecting the position of the bone
        float4 position : TEXCOORD0; // four-dimensional coordinates
        float2 uvs : TEXCOORD1; // = (threshold, 0)
        float4 normal : TEXCOORD2;
      };

      struct v2f {
        float4 vertex : SV_POSITION;
        UNITY_FOG_COORDS(4)
      };

      float _Ortho;
      float _Distance;
      float _Radius;
      float _Tanfov;
      float _Scale;
      float _RotationSpeed;
      float4 _InitialRotation1;
      float4 _InitialRotation2;
      float4 _InitialRotation3;
      float4 _InitialRotation4;
      // float _Inversed;
      // float _Glass;
      float4 _AutoRot1From;
      float4 _AutoRot1To;
      float _AutoRot1Speed;
      float4 _AutoRot2From;
      float4 _AutoRot2To;
      float _AutoRot2Speed;

      float4 Reflect(float4 src, float4 normal) {
        return src - 2 * dot(src, normal) * normal;
      }

      float4 Rotate(float4 src, float4 from, float4 tohalf) { // double angle rotation from from to tohalf
        return Reflect(Reflect(src, from), tohalf);
      }

      float4 RotV(float4 src, float4 rot) {
        return Rotate(src, float4(0, 0, 0, 1), rot);
      }

      v2f vert (appdata v) {
        float4x4 rot = float4x4(_InitialRotation1, _InitialRotation2, _InitialRotation3, _InitialRotation4);
        float4 to1 = cos(_AutoRot1Speed * _Time.y) * _AutoRot1From + sin(_AutoRot1Speed * _Time.y) * _AutoRot1To;
        float4 to2 = cos(_AutoRot2Speed * _Time.y) * _AutoRot2From + sin(_AutoRot2Speed * _Time.y) * _AutoRot2To;
        float t = length(v.vertex.xyz) * _RotationSpeed;
        float4 r = float4(0, 0, 0, 1);
        if (t > 1e-9) r = float4(sin(t) * normalize(v.vertex.xyz), cos(t));
        float4 pos4d = RotV(Rotate(Rotate(mul(v.position, rot), _AutoRot1From, to1), _AutoRot2From, to2), r);
        // --- projection ---
        float4 pos = float4((_Ortho ? pos4d.xyz/_Radius : pos4d.xyz/((_Distance - pos4d.w)*_Tanfov)) * 2 * _Scale, 1);
        // --- backcell culling ---
        pos4d = RotV(Rotate(Rotate(mul(v.normal, rot), _AutoRot1From, to1), _AutoRot2From, to2), r);
        if (/*(_Glass < 0.5) &&*/ ((pos4d.w * _Distance - v.uvs.x - 1e-9) /** (-_Inversed)*/ < 0)) {
          pos = float4(0, 0, 0, 0);
        }
        v2f o;
        o.vertex = UnityObjectToClipPos(pos);
        UNITY_TRANSFER_FOG(o, o.vertex);
        return o;
      }

      fixed4 frag (v2f i) : SV_Target {
        return 0;
      }
      ENDCG
    }

    Pass {
      Cull Off

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #pragma multi_compile_fog
      #include "UnityCG.cginc"

      struct appdata {
        float4 vertex : POSITION; // (0,0,0,1) in mesh, reflecting the position of the bone
        float4 position : TEXCOORD0; // four-dimensional coordinates
        float2 uvs : TEXCOORD1; // = (threshold, 0)
        float4 normal : TEXCOORD2;
        float2 uv : TEXCOORD3;
      };

      struct v2f {
        float4 vertex : SV_POSITION;
        float2 uv : TEXCOORD3;
        UNITY_FOG_COORDS(4)
      };

      sampler2D _MainTex;
      float4 _MainTex_ST;
      float _Ortho;
      float _Distance;
      float _Radius;
      float _Tanfov;
      float _Scale;
      float _RotationSpeed;
      float4 _InitialRotation1;
      float4 _InitialRotation2;
      float4 _InitialRotation3;
      float4 _InitialRotation4;
      // float _Inversed;
      // float _Glass;
      float4 _AutoRot1From;
      float4 _AutoRot1To;
      float _AutoRot1Speed;
      float4 _AutoRot2From;
      float4 _AutoRot2To;
      float _AutoRot2Speed;

      float4 Reflect(float4 src, float4 normal) {
        return src - 2 * dot(src, normal) * normal;
      }

      float4 Rotate(float4 src, float4 from, float4 tohalf) { // double angle rotation from from to tohalf
        return Reflect(Reflect(src, from), tohalf);
      }

      float4 RotV(float4 src, float4 rot) {
        return Rotate(src, float4(0, 0, 0, 1), rot);
      }

      v2f vert (appdata v) {
        float4x4 rot = float4x4(_InitialRotation1, _InitialRotation2, _InitialRotation3, _InitialRotation4);
        float4 to1 = cos(_AutoRot1Speed * _Time.y) * _AutoRot1From + sin(_AutoRot1Speed * _Time.y) * _AutoRot1To;
        float4 to2 = cos(_AutoRot2Speed * _Time.y) * _AutoRot2From + sin(_AutoRot2Speed * _Time.y) * _AutoRot2To;
        float t = length(v.vertex.xyz) * _RotationSpeed;
        float4 r = float4(0, 0, 0, 1);
        if (t > 1e-9) r = float4(sin(t) * normalize(v.vertex.xyz), cos(t));
        float4 pos4d = RotV(Rotate(Rotate(mul(v.position, rot), _AutoRot1From, to1), _AutoRot2From, to2), r);
        // --- projection ---
        float4 pos = float4((_Ortho ? pos4d.xyz/_Radius : pos4d.xyz/((_Distance - pos4d.w)*_Tanfov)) * 2 * _Scale, 1);
        // --- backcell culling ---
        pos4d = RotV(Rotate(Rotate(mul(v.normal, rot), _AutoRot1From, to1), _AutoRot2From, to2), r);
        if (/*(_Glass < 0.5) &&*/ ((pos4d.w * _Distance - v.uvs.x - 1e-9) /** (-_Inversed)*/ < 0)) {
          pos = float4(0, 0, 0, 0);
        }
        v2f o;
        o.vertex = UnityObjectToClipPos(pos);
        UNITY_TRANSFER_FOG(o, o.vertex);
        o.uv = TRANSFORM_TEX(v.uv, _MainTex);
        return o;
      }

      fixed4 frag (v2f i) : SV_Target {
        fixed4 col = tex2D(_MainTex, i.uv);
        UNITY_APPLY_FOG(i.fogCoord, col);
        return col;
      }
      ENDCG
    }
  }
}
