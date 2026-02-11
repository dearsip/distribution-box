
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Rendering;
using VRC.Udon;

namespace HachigayoLab.OrthogonalMirror
{
    public enum PhotoResolutionMode
    {
        _Stream, _HD, _FHD, _2K, _4K, _8K
    }

    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class OrthogonalMirror : UdonSharpBehaviour
    {
        public PhotoResolutionMode PhotoResolution { get => currentPhotoResolution; set { photoChanged = true; currentPhotoResolution = value; } }
        PhotoResolutionMode currentPhotoResolution;
        [SerializeField] bool mirror = true;
        [SerializeField] float minDepth = 1;
        Camera refCamera, refCameraP, cameraL, cameraR, cameraP;
        VRCCameraSettings screenCamera, photoCamera;
        RenderTexture textureL, textureR, textureP;
        Matrix4x4 projectionL, projectionR, projectionP;
        Matrix4x4 replace = new Matrix4x4(new Vector4(1, 0, 0, 0), new Vector4(0, 1, 0, 0), new Vector4(0, 0, 0, 1), new Vector4(0, 0, 1, 0));
        Plane[] planes;
        MeshRenderer meshRenderer;
        int streamWidth, streamHeight;
        [SerializeField] GameObject CameraPrefab;
        bool isUserInVR, screenChanged = true, photoChanged = true;
        void Start()
        {
            screenCamera = VRCCameraSettings.ScreenCamera;
        #if !UNITY_EDITOR
            photoCamera = VRCCameraSettings.PhotoCamera;
        #else
            photoCamera = VRCCameraSettings.ScreenCamera;
        #endif
            isUserInVR = Networking.LocalPlayer.IsUserInVR();
            textureL = new RenderTexture(screenCamera.PixelWidth, screenCamera.PixelHeight, 32);
            textureR = new RenderTexture(screenCamera.PixelWidth, screenCamera.PixelHeight, 32);
            textureP = new RenderTexture(photoCamera.PixelWidth, photoCamera.PixelHeight, 32);

            refCamera = Instantiate(CameraPrefab, transform).GetComponent<Camera>();
            refCameraP = Instantiate(CameraPrefab, transform).GetComponent<Camera>();
            refCamera.cullingMask = refCameraP.cullingMask = 0;
            refCamera.clearFlags = refCameraP.clearFlags = CameraClearFlags.Nothing;
            refCamera.enabled = refCameraP.enabled = true;
            refCameraP.targetTexture = textureP;
            cameraL = Instantiate(CameraPrefab, transform).GetComponent<Camera>();
            cameraR = Instantiate(CameraPrefab, transform).GetComponent<Camera>();
            cameraP = Instantiate(CameraPrefab, transform).GetComponent<Camera>();
            cameraL.targetTexture = textureL;
            cameraR.targetTexture = textureR;
            cameraP.targetTexture = textureP;
            cameraL.enabled = true;
            cameraR.enabled = isUserInVR;
            cameraP.enabled = photoCamera.Active;
            refCamera.GetComponent<CameraEventManager>().Initialize(-1);
            refCameraP.GetComponent<CameraEventManager>().Initialize(-2);
            cameraL.GetComponent<CameraEventManager>().Initialize(0);
            cameraR.GetComponent<CameraEventManager>().Initialize(1);
            cameraP.GetComponent<CameraEventManager>().Initialize(2);
            planes = new Plane[6];
            meshRenderer = GetComponent<MeshRenderer>();
            Material material = meshRenderer.material;
            material.SetTexture("_LeftTex", textureL);
            material.SetTexture("_RightTex", textureR);
            material.SetTexture("_PhotoTex", textureP);
            material.SetFloat("_Mirror", mirror ? 1 : 0);
        }

        void Update()
        {
            if (isUserInVR)
            {
                if (!photoCamera.Active)
                {
                    streamWidth = photoCamera.PixelWidth;
                    streamHeight = photoCamera.PixelHeight;
                }
            }
            else
            {
                streamWidth = screenCamera.PixelWidth;
                streamHeight = screenCamera.PixelHeight;
            }

            if (screenChanged)
            {
                screenChanged = false;

                int w = screenCamera.PixelWidth; int h = screenCamera.PixelHeight;
                if (w != textureL.width || h != textureL.height)
                {
                    textureL.Release(); textureL.width = w; textureL.height = h; textureL.Create();
                    textureR.Release(); textureR.width = w; textureR.height = h; textureR.Create();
                }

                refCamera.nearClipPlane = screenCamera.NearClipPlane;
                refCamera.farClipPlane = screenCamera.FarClipPlane;
                refCamera.fieldOfView = screenCamera.FieldOfView;
                projectionL = isUserInVR ? refCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left) : refCamera.projectionMatrix;
                projectionL[2, 0] = 0; projectionL[2, 1] = 0; projectionL[2, 2] =-1; projectionL[2, 3] = 0;
                projectionL[3, 0] = 0; projectionL[3, 1] = 0; projectionL[3, 2] = 0; projectionL[3, 3] = 1;
                projectionR = refCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
                projectionR[2, 0] = 0; projectionR[2, 1] = 0; projectionR[2, 2] =-1; projectionR[2, 3] = 0;
                projectionR[3, 0] = 0; projectionR[3, 1] = 0; projectionR[3, 2] = 0; projectionR[3, 3] = 1;
            }

            if (photoChanged)
            {
                photoChanged = false;

                int w = 1; int h = 1;
                switch (PhotoResolution)
                {
                    case PhotoResolutionMode._Stream: w = streamWidth; h = streamHeight; break;
                    case PhotoResolutionMode._HD: w = 1280; h = 720; break;
                    case PhotoResolutionMode._FHD: w = 1920; h = 1080; break;
                    case PhotoResolutionMode._2K: w = 2560; h = 1440; break;
                    case PhotoResolutionMode._4K: w = 3840; h = 2160; break;
                    case PhotoResolutionMode._8K: w = 7680; h = 4320; break;
                }
                if (w != textureP.width || h != textureP.height)
                {
                    textureP.Release(); textureP.width = w; textureP.height = h; textureP.Create();
                }

                refCameraP.nearClipPlane = photoCamera.NearClipPlane;
                refCameraP.farClipPlane = photoCamera.FarClipPlane;
                refCameraP.fieldOfView = photoCamera.FieldOfView * 0.795f;
                refCameraP.targetTexture = null;
                refCameraP.targetTexture = textureP;
                projectionP = refCameraP.projectionMatrix;
                projectionP[2, 0] = 0; projectionP[2, 1] = 0; projectionP[2, 2] =-1; projectionP[2, 3] = 0;
                projectionP[3, 0] = 0; projectionP[3, 1] = 0; projectionP[3, 2] = 0; projectionP[3, 3] = 1;
            }
        }

        public void CustomPreCull(int t)
        {
            if (t == -1)
            {
                Bounds bounds = meshRenderer.bounds;
                CalculateFrustumPlanes((isUserInVR
                    ? refCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left) : refCamera.projectionMatrix)
                    * Matrix4x4.Scale(new Vector3(1, 1, -1))
                    * Matrix4x4.Rotate(Quaternion.Inverse(VRCCameraSettings.GetEyeRotation(Camera.StereoscopicEye.Left)))
                    * Matrix4x4.Translate(-VRCCameraSettings.GetEyePosition(Camera.StereoscopicEye.Left))
                    , planes);
                cameraL.enabled = GeometryUtility.TestPlanesAABB(planes, bounds);

                if (isUserInVR)
                {
                    CalculateFrustumPlanes(
                        refCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right)
                        * Matrix4x4.Scale(new Vector3(1, 1, -1))
                        * Matrix4x4.Rotate(Quaternion.Inverse(VRCCameraSettings.GetEyeRotation(Camera.StereoscopicEye.Right)))
                        * Matrix4x4.Translate(-VRCCameraSettings.GetEyePosition(Camera.StereoscopicEye.Right))
                        , planes);
                    cameraR.enabled = GeometryUtility.TestPlanesAABB(planes, bounds);
                }
                else cameraR.enabled = false;

                if (photoCamera.Active)
                {
                    CalculateFrustumPlanes(
                        refCameraP.projectionMatrix
                        * Matrix4x4.Scale(new Vector3(1, 1, -1))
                        * Matrix4x4.Rotate(Quaternion.Inverse(photoCamera.Rotation))
                        * Matrix4x4.Translate(-photoCamera.Position)
                        , planes);
                    cameraP.enabled = GeometryUtility.TestPlanesAABB(planes, bounds);
                }
                else cameraP.enabled = false;
                return;
            }

            Vector3 mirPos = transform.position;
            Quaternion mirRot = transform.rotation;

            if (t == 0)
            {
                Vector3 camPos = VRCCameraSettings.GetEyePosition(Camera.StereoscopicEye.Left);
                Quaternion camRot = VRCCameraSettings.GetEyeRotation(Camera.StereoscopicEye.Left);
                Vector3 pos = Quaternion.Inverse(mirRot) * (mirPos - camPos);
                Quaternion rot = Quaternion.Inverse(camRot) * mirRot;
                Matrix4x4 scaler = Matrix4x4.identity;
                scaler[3, 3] = pos.z;
                Matrix4x4 projection = projectionL
                    * Matrix4x4.Translate(new Vector3(0, 0, 1))
                    * Matrix4x4.Scale(mirror ? new Vector3(-1, 1, cameraL.nearClipPlane / minDepth)
                    : new Vector3(1, 1, -cameraL.nearClipPlane / minDepth)) // (view space -> projection space) * adjust depth
                    * replace * Matrix4x4.Rotate(rot) * replace
                    * scaler
                    * Matrix4x4.Translate(new Vector3(pos.x, pos.y, 0))
                    * Matrix4x4.Scale(new Vector3(1, 1, -1)) // projection space -> view space
                    ;
                cameraL.projectionMatrix = projection;
                cameraL.cullingMatrix = projection * cameraL.worldToCameraMatrix;
            }

            else if (t == 1)
            {
                Vector3 camPos = VRCCameraSettings.GetEyePosition(Camera.StereoscopicEye.Right);
                Quaternion camRot = VRCCameraSettings.GetEyeRotation(Camera.StereoscopicEye.Right);
                Vector3 pos = Quaternion.Inverse(mirRot) * (mirPos - camPos);
                Quaternion rot = Quaternion.Inverse(camRot) * mirRot;
                Matrix4x4 scaler = Matrix4x4.identity;
                scaler[3, 3] = pos.z;
                Matrix4x4 projection = projectionR
                    * Matrix4x4.Translate(new Vector3(0, 0, 1))
                    * Matrix4x4.Scale(mirror ? new Vector3(-1, 1, cameraR.nearClipPlane / minDepth)
                    : new Vector3(1, 1, -cameraR.nearClipPlane / minDepth)) // (view space -> projection space) * adjust depth
                    * replace * Matrix4x4.Rotate(rot) * replace
                    * scaler
                    * Matrix4x4.Translate(new Vector3(pos.x, pos.y, 0))
                    * Matrix4x4.Scale(new Vector3(1, 1, -1)) // projection space -> view space
                    ;
                cameraR.projectionMatrix = projection;
                cameraR.cullingMatrix = projection * cameraR.worldToCameraMatrix;
            }

            else
            {
                Vector3 camPos = photoCamera.Position;
                Quaternion camRot = photoCamera.Rotation;
                Vector3 pos = Quaternion.Inverse(mirRot) * (mirPos - camPos);
                Quaternion rot = Quaternion.Inverse(camRot) * mirRot;
                Matrix4x4 scaler = Matrix4x4.identity;
                scaler[3, 3] = pos.z;
                Matrix4x4 projection = projectionP
                    * Matrix4x4.Translate(new Vector3(0, 0, 1))
                    * Matrix4x4.Scale(mirror ? new Vector3(-1, 1, cameraP.nearClipPlane / minDepth)
                    : new Vector3(1, 1, -cameraP.nearClipPlane / minDepth)) // (view space -> projection space) * adjust depth
                    * replace * Matrix4x4.Rotate(rot) * replace
                    * scaler
                    * Matrix4x4.Translate(new Vector3(pos.x, pos.y, 0))
                    * Matrix4x4.Scale(new Vector3(1, 1, -1)) // projection space -> view space
                    ;
                cameraP.projectionMatrix = projection;
                cameraP.cullingMatrix = projection * cameraP.worldToCameraMatrix;
            }
        }

        public override void OnVRCCameraSettingsChanged(VRCCameraSettings cameraSettings)
        {
            if (cameraSettings == screenCamera) screenChanged = true;
            else photoChanged = true;
        }

        void CalculateFrustumPlanes(Matrix4x4 vp, Plane[] planes)
        {
            // Left
            ExtractPlane(ref planes[0],
                vp.m30 + vp.m00,
                vp.m31 + vp.m01,
                vp.m32 + vp.m02,
                vp.m33 + vp.m03);

            // Right
            ExtractPlane(ref planes[1],
                vp.m30 - vp.m00,
                vp.m31 - vp.m01,
                vp.m32 - vp.m02,
                vp.m33 - vp.m03);

            // Bottom
            ExtractPlane(ref planes[2],
                vp.m30 + vp.m10,
                vp.m31 + vp.m11,
                vp.m32 + vp.m12,
                vp.m33 + vp.m13);

            // Top
            ExtractPlane(ref planes[3],
                vp.m30 - vp.m10,
                vp.m31 - vp.m11,
                vp.m32 - vp.m12,
                vp.m33 - vp.m13);

            // Near
            ExtractPlane(ref planes[4],
                vp.m30 + vp.m20,
                vp.m31 + vp.m21,
                vp.m32 + vp.m22,
                vp.m33 + vp.m23);

            // Far
            ExtractPlane(ref planes[5],
                vp.m30 - vp.m20,
                vp.m31 - vp.m21,
                vp.m32 - vp.m22,
                vp.m33 - vp.m23);
        }

        void ExtractPlane(ref Plane plane, float a, float b, float c, float d)
        {
            Vector3 normal = new Vector3(a, b, c);
            float invLen = 1.0f / Mathf.Sqrt(a*a + b*b + c*c);

            plane.normal = normal * invLen;
            plane.distance = d * invLen;
        }
    }
}
