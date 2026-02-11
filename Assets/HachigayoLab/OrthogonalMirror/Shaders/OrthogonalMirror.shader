Shader "HachigayoLab/OrthogonalMirror/OrthogonalMirror"
{
    Properties
    {
        _MainTex("Base (RGB)", 2D) = "white" {}
        [HideInInspector] _LeftTex ("", 2D) = "white" {}
        [HideInInspector] _RightTex ("", 2D) = "white" {}
        [HideInInspector] _PhotoTex ("", 2D) = "white" {}
    }
    SubShader
    {
        Tags{ "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityInstancing.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _VRChatCameraMode;
            float _Mirror;

            sampler2D _LeftTex;
            sampler2D _RightTex;
            sampler2D _PhotoTex;

            struct appdata 
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screen : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.screen = ComputeNonStereoScreenPos(o.pos);
                if (_Mirror > 0.5) o.screen.x = o.screen.w - o.screen.x;

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                half4 tex = tex2D(_MainTex, i.uv);
                half4 refl = _VRChatCameraMode > 0 ? tex2Dproj(_PhotoTex, UNITY_PROJ_COORD(i.screen)) : unity_StereoEyeIndex == 1 ? tex2Dproj(_RightTex, UNITY_PROJ_COORD(i.screen)) : tex2Dproj(_LeftTex, UNITY_PROJ_COORD(i.screen));
                return tex * refl;
            }
            ENDCG
        }
    }
}
