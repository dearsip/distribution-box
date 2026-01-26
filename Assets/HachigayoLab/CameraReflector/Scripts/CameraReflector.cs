
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Rendering;
using VRC.SDKBase;
using VRC.Udon;

namespace HachigayoLab.CameraReflector
{
    public enum PhotoResolutionMode
    {
        _Stream, _HD, _FHD, _2K, _4K, _8K
    }

    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class CameraReflector : UdonSharpBehaviour
    {
        [SerializeField] int cameraCount = 1, cameraDepthOffset = -5, integrateLayer = 22, depthBaseLayer = 18;
        public PhotoResolutionMode PhotoResolution { get => currentPhotoResolution; set { photoChanged = true; currentPhotoResolution = value; } }
        PhotoResolutionMode currentPhotoResolution;
        [SerializeField] GameObject ReflectorCamera;
        [SerializeField] Material textureFirstwriter, textureOverwriter, depthSampler, depthOverwriter;
        GameObject integratePhoto;
        RenderTexture textureB, textureL, textureR, depth0, depth1, depth2, texturePB, textureP, depthP0, depthP1, depthP2;
        Camera refCamera, baseCameraL, baseCameraR, baseCameraP;
        Camera[] cameras;
        Transform baseCameraLTransform, baseCameraRTransform, baseCameraPTransform;
        bool isUserInVR, screenChanged = true, photoChanged = true;
        bool[] flip;
        int idMirror, idNumber, idTarget, idTextureB, idDepth0, idDepth1, idDepth2, streamWidth = 1280, streamHeight = 720, currentCamera = -2, enabledCameraCount, number, target;
        int[] enabledCameraIndices;
        VRCCameraSettings screenCamera, photoCamera;
        Matrix4x4 projectionL, projectionR, projectionP;

        void Start()
        {
            screenCamera = VRCCameraSettings.ScreenCamera;
            photoCamera = VRCCameraSettings.PhotoCamera;
            screenCamera.DepthTextureMode = DepthTextureMode.Depth;
            photoCamera.DepthTextureMode = DepthTextureMode.Depth;
            isUserInVR = Networking.LocalPlayer.IsUserInVR();
            textureB = new RenderTexture(1, 1, 32);
            VRCShader.SetGlobalTexture(idTextureB = VRCShader.PropertyToID("_UdonTextureB"), textureB);
            textureL = new RenderTexture(textureB.descriptor);
            VRCShader.SetGlobalTexture(VRCShader.PropertyToID("_UdonTextureL"), textureL);
            textureR = new RenderTexture(textureL.descriptor);
            VRCShader.SetGlobalTexture(VRCShader.PropertyToID("_UdonTextureR"), textureR);
            depth0 = new RenderTexture(textureB.descriptor);
            depth0.format = RenderTextureFormat.RFloat;
            VRCShader.SetGlobalTexture(idDepth0 = VRCShader.PropertyToID("_UdonDepth0"), depth0);
            depth1 = new RenderTexture(depth0.descriptor);
            VRCShader.SetGlobalTexture(idDepth1 = VRCShader.PropertyToID("_UdonDepth1"), depth1);
            depth2 = new RenderTexture(depth0.descriptor);
            VRCShader.SetGlobalTexture(idDepth2 = VRCShader.PropertyToID("_UdonDepth2"), depth2);
            texturePB = new RenderTexture(1, 1, 32);
            textureP = new RenderTexture(texturePB.descriptor);
            VRCShader.SetGlobalTexture(VRCShader.PropertyToID("_UdonTextureP"), textureP);
            depthP0 = new RenderTexture(texturePB.descriptor);
            depthP0.format = RenderTextureFormat.RFloat;
            depthP1 = new RenderTexture(depthP0.descriptor);
            depthP2 = new RenderTexture(depthP0.descriptor);
            idMirror = VRCShader.PropertyToID("_UdonMirror");
            idNumber = VRCShader.PropertyToID("_UdonNumber");
            idTarget = VRCShader.PropertyToID("_UdonTarget");

            GameObject gameObject = transform.Find("Integrate").gameObject;
            gameObject.SetActive(true);
            gameObject.layer = 1 << integrateLayer;

            integratePhoto = transform.Find("IntegratePhoto").gameObject;
            integratePhoto.SetActive(true);

            gameObject = transform.Find("TextureBase").gameObject;
            gameObject.SetActive(true);
            gameObject.layer = depthBaseLayer;

            gameObject = Instantiate(ReflectorCamera, transform);
            refCamera = gameObject.GetComponent<Camera>();
            Destroy(gameObject.GetComponent<CameraEventManager>());
            refCamera.enabled = true;
            refCamera.depth = -cameraCount * 100 + cameraDepthOffset;
            refCamera.cullingMask = 0;
            refCamera.clearFlags = CameraClearFlags.Nothing;

            // camera settings
            baseCameraLTransform = Instantiate(ReflectorCamera, transform).transform;
            baseCameraL = baseCameraLTransform.GetComponent<Camera>();
            baseCameraL.enabled = true;
            baseCameraL.depth = -(cameraCount + 1) * 3 + cameraDepthOffset;
            baseCameraL.clearFlags = CameraClearFlags.Skybox;
            baseCameraL.targetTexture = textureL;
            baseCameraL.cullingMask = screenCamera.CullingMask & ~(1 << integrateLayer);
            baseCameraL.depthTextureMode = DepthTextureMode.Depth;
            baseCameraL.GetComponent<CameraEventManager>().Initialize(-1, 0);

            baseCameraRTransform = Instantiate(ReflectorCamera, transform).transform;
            baseCameraR = baseCameraRTransform.GetComponent<Camera>();
            baseCameraR.enabled = isUserInVR;
            baseCameraR.depth = -(cameraCount + 1) * 2 + cameraDepthOffset;
            baseCameraR.clearFlags = CameraClearFlags.Skybox;
            baseCameraR.targetTexture = textureR;
            baseCameraR.cullingMask = screenCamera.CullingMask & ~(1 << integrateLayer);
            baseCameraR.depthTextureMode = DepthTextureMode.Depth;
            baseCameraR.GetComponent<CameraEventManager>().Initialize(-1, 1);

            baseCameraPTransform = Instantiate(ReflectorCamera, transform).transform;
            baseCameraP = baseCameraPTransform.GetComponent<Camera>();
            baseCameraP.enabled = false;
            baseCameraP.depth = -(cameraCount + 1) + cameraDepthOffset;
            baseCameraP.clearFlags = CameraClearFlags.Skybox;
            baseCameraP.targetTexture = textureP;
            baseCameraP.depthTextureMode = DepthTextureMode.Depth;
            baseCameraP.GetComponent<CameraEventManager>().Initialize(-1, 2);

            screenCamera.CullingMask = 1 << integrateLayer;
            screenCamera.ClearFlags = CameraClearFlags.Nothing;

            cameras = new Camera[cameraCount * 3];
            for (int i = 0; i < cameraCount * 3; i++)
            {
                Camera cam = cameras[i] = Instantiate(ReflectorCamera, transform).GetComponent<Camera>();
                if (i < cameraCount) cam.enabled = true;
                else if (i < cameraCount * 2) cam.enabled = isUserInVR;
                cam.depth = -(cameraCount + 1) * 3 + 1 + i % cameraCount + i / cameraCount * (cameraCount + 1) + cameraDepthOffset;
                cam.cullingMask = /*baseCameraL.cullingMask ^ ((1 << 10) | (1 << 18))*/ (1 << 9) | (1 << 18) | (1 << depthBaseLayer);
                cam.clearFlags = CameraClearFlags.Nothing;
                cam.depthTextureMode = DepthTextureMode.Depth;
                if (i % cameraCount % 2 == 0) cam.targetTexture = textureB;
                else if (i < cameraCount) cam.targetTexture = textureL;
                else if (i < cameraCount * 2) cam.targetTexture = textureR;
                else cam.targetTexture = textureP;
                cam.GetComponent<CameraEventManager>().Initialize(i % cameraCount, i / cameraCount);
            }

            flip = new bool[cameraCount + 2];
            enabledCameraIndices = new int[cameraCount + 2];
            enabledCameraIndices[0] = 0;
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
                if (w != textureB.width || h != textureB.height)
                {
                    textureB.Release(); textureB.width = w; textureB.height = h; textureB.Create();
                    textureL.Release(); textureL.width = w; textureL.height = h; textureL.Create();
                    textureR.Release(); textureR.width = w; textureR.height = h; textureR.Create();
                    depth0.Release(); depth0.width = w; depth0.height = h; depth0.Create();
                    depth1.Release(); depth1.width = w; depth1.height = h; depth1.Create();
                    depth2.Release(); depth2.width = w; depth2.height = h; depth2.Create();
                }

                refCamera.nearClipPlane = screenCamera.NearClipPlane;
                refCamera.farClipPlane = screenCamera.FarClipPlane;
                refCamera.fieldOfView = screenCamera.FieldOfView;
                baseCameraL.projectionMatrix = projectionL = isUserInVR ? refCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left) : refCamera.projectionMatrix;
                baseCameraR.projectionMatrix = projectionR = refCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
            }

            baseCameraP.enabled = photoCamera.Active;
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
                if (w != texturePB.width || h != texturePB.height)
                {
                    texturePB.Release(); texturePB.width = w; texturePB.height = h; texturePB.Create();
                    textureP.Release(); textureP.width = w; textureP.height = h; textureP.Create();
                    depthP0.Release(); depthP0.width = w; depthP0.height = h; depthP0.Create();
                    depthP1.Release(); depthP1.width = w; depthP1.height = h; depthP1.Create();
                    depthP2.Release(); depthP2.width = w; depthP2.height = h; depthP2.Create();
                }

                baseCameraP.nearClipPlane = photoCamera.NearClipPlane;
                baseCameraP.farClipPlane = photoCamera.FarClipPlane;
                baseCameraP.fieldOfView = photoCamera.FieldOfView;
                baseCameraP.targetTexture = null;
                baseCameraP.targetTexture = textureP;
                projectionP = baseCameraP.projectionMatrix;
                baseCameraP.cullingMask = photoCamera.CullingMask & ~(1 << integrateLayer);
                baseCameraP.clearFlags = photoCamera.ClearFlags;
                baseCameraP.backgroundColor = photoCamera.BackgroundColor;
            }

            enabledCameraCount = 0;
            for (int i = 0; i < cameraCount; i++)
            {
                flip[i + 1] = true; // calculate whether the cameras are flipped

                bool enabled = true; // calculate whether the cameras are enabled
                cameras[i].enabled = enabled;
                cameras[i + cameraCount].enabled = isUserInVR && enabled;
                cameras[i + cameraCount * 2].enabled = photoCamera.Active && enabled;
                cameras[i + cameraCount * 2].cullingMask = baseCameraP.cullingMask & ((1 << 9) | (1 << 18) | (1 << depthBaseLayer));
                if (enabled) enabledCameraIndices[++enabledCameraCount] = i + 1;
            }
            enabledCameraIndices[enabledCameraCount + 1] = cameraCount + 1;

            if (enabledCameraCount % 2 == 0)
            {
                baseCameraL.targetTexture = textureL;
                baseCameraR.targetTexture = textureR;
                baseCameraP.targetTexture = textureP;
            }
            else
            {
                baseCameraL.targetTexture = baseCameraR.targetTexture = textureB;
                baseCameraP.targetTexture = texturePB;
            }

            for (int i = 0; i < enabledCameraCount; i++)
            {
                cameras[enabledCameraIndices[i + 1] - 1].targetTexture = (enabledCameraCount - i) % 2 == 0 ? textureB : textureL;
                if (isUserInVR) cameras[enabledCameraIndices[i + 1] - 1 + cameraCount].targetTexture = (enabledCameraCount - i) % 2 == 0 ? textureB : textureR;
                if (photoCamera.Active) cameras[enabledCameraIndices[i + 1] - 1 + cameraCount * 2].targetTexture = (enabledCameraCount - i) % 2 == 0 ? texturePB : textureP;
            }

            LayerMask layerMask = photoCamera.CullingMask;
            integratePhoto.layer = (layerMask & 1) > 0 ? 0 : (layerMask & (1 << 3)) > 0 ? 3 : (layerMask & (1 << 5)) > 0 ? 5 : (layerMask & (1 << 9)) > 0 ? 9 : 18;
        }

        public void CustomPreCull(int n, int t)
        {
            if (n == -1)
            {
                if (t == 0)
                {
                    baseCameraLTransform.SetPositionAndRotation(VRCCameraSettings.GetEyePosition(Camera.StereoscopicEye.Left), VRCCameraSettings.GetEyeRotation(Camera.StereoscopicEye.Left));
                    baseCameraL.cullingMatrix = baseCameraL.projectionMatrix * baseCameraL.worldToCameraMatrix;
                    baseCameraRTransform.SetPositionAndRotation(VRCCameraSettings.GetEyePosition(Camera.StereoscopicEye.Right), VRCCameraSettings.GetEyeRotation(Camera.StereoscopicEye.Right));
                    baseCameraR.cullingMatrix = baseCameraR.projectionMatrix * baseCameraR.worldToCameraMatrix;

                    VRCShader.SetGlobalTexture(idTextureB, textureB);
                    VRCShader.SetGlobalTexture(idDepth0, depth0);
                    VRCShader.SetGlobalTexture(idDepth1, depth1);
                    VRCShader.SetGlobalTexture(idDepth2, depth2);
                }

                if (t == 2)
                {
                    baseCameraPTransform.SetPositionAndRotation(photoCamera.Position, photoCamera.Rotation);
                    baseCameraP.cullingMatrix = baseCameraP.projectionMatrix * baseCameraP.worldToCameraMatrix;

                    VRCShader.SetGlobalTexture(idTextureB, texturePB);
                    VRCShader.SetGlobalTexture(idDepth0, depthP0);
                    VRCShader.SetGlobalTexture(idDepth1, depthP1);
                    VRCShader.SetGlobalTexture(idDepth2, depthP2);
                }
                currentCamera = -1;
                VRCShader.SetGlobalFloat(idTarget, target = t);
            }
            else
            {
                currentCamera++;
                // calculate projection matrix
                Matrix4x4 pj =
                    (t == 0 ? projectionL : t == 1 ? projectionR : projectionP)
                    * Matrix4x4.Scale(new Vector3(1, 1, -1)) // projection space -> view space
                    * Matrix4x4.Rotate(Quaternion.Inverse(t == 0 ? VRCCameraSettings.GetEyeRotation(Camera.StereoscopicEye.Left) : t == 1 ? VRCCameraSettings.GetEyeRotation(Camera.StereoscopicEye.Right) : photoCamera.Rotation))
                    * Matrix4x4.Translate(-(t == 0 ? VRCCameraSettings.GetEyePosition(Camera.StereoscopicEye.Left) : t == 1 ? VRCCameraSettings.GetEyePosition(Camera.StereoscopicEye.Right) : photoCamera.Position))
                    ; // mirror reflection * (projection space -> view space)
                if (flip[n + 1]) { pj[0, 0] = -pj[0, 0]; pj[0, 1] = -pj[0, 1]; pj[0, 2] = -pj[0, 2]; pj[0, 3] = -pj[0, 3]; } // flip row 0
                cameras[n + t * cameraCount].projectionMatrix = pj;
                cameras[n + t * cameraCount].cullingMatrix = pj * cameras[n + t * cameraCount].worldToCameraMatrix;
            }

            VRCShader.SetGlobalFloat(idMirror, flip[enabledCameraIndices[currentCamera + 1]] ^ flip[enabledCameraIndices[currentCamera + 2]] ? 1 : 0);
            VRCShader.SetGlobalFloat(idNumber, number = n == -1 ? -1 : (enabledCameraCount - currentCamera) % 2);
        }

        void OnRenderObject()
        {
            if (currentCamera == -2) return;
            if (target < 2)
            {
                if (currentCamera == -1) VRCGraphics.Blit(null, enabledCameraCount % 2 == 0 ? depth0 : depth1, depthSampler);
                else VRCGraphics.Blit(null, depth2, depthSampler);
            }
            else
            {
                if (currentCamera == -1) VRCGraphics.Blit(null, enabledCameraCount % 2 == 0 ? depthP0 : depthP1, depthSampler);
                else VRCGraphics.Blit(null, depthP2, depthSampler);
            }
        }

        public void CustomRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (currentCamera == -1) VRCGraphics.Blit(source, destination, textureFirstwriter);
            else
            {
                VRCGraphics.Blit(source, destination, textureOverwriter);
                if (target < 2) VRCGraphics.Blit(source, number == 0 ? depth1 : depth0, depthOverwriter);
                else VRCGraphics.Blit(source, number == 0 ? depthP1 : depthP0, depthOverwriter);
            }
            if (currentCamera + 1 == enabledCameraCount) VRCShader.SetGlobalFloat(idNumber, currentCamera = -2);
        }

        public override void OnVRCCameraSettingsChanged(VRCCameraSettings cameraSettings)
        {
            if (cameraSettings == screenCamera) screenChanged = true;
            else photoChanged = true;
        }

    }
}
