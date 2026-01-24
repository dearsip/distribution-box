
using UdonSharp;
using UnityEngine;
using UnityEngine.Rendering;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Rendering;

namespace HachigayoLab.CameraReflector
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class CameraEventManager : UdonSharpBehaviour
    {
        CameraReflector cameraReflector;
        int index, target;
        public void Initialize(int index, int target)
        {
            cameraReflector = transform.parent.GetComponent<CameraReflector>();
            this.index = index;
            this.target = target;
        }

        void OnPreCull()
        {
            cameraReflector.CustomPreCull(index, target);
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            cameraReflector.CustomRenderImage(source, destination);
        }
    }
}
