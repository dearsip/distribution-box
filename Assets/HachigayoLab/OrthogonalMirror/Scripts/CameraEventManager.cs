
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HachigayoLab.OrthogonalMirror
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class CameraEventManager : UdonSharpBehaviour
    {
        OrthogonalMirror orthogonalMirror;
        int target;
        public void Initialize(int target)
        {
            orthogonalMirror = transform.parent.GetComponent<OrthogonalMirror>();
            this.target = target;
        }

        void OnPreCull()
        {
            if (target > -2) orthogonalMirror.CustomPreCull(target);
        }
    }
}
