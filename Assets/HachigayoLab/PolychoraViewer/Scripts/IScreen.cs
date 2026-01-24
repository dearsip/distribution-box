
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HachigayoLab.PolychoraViewer
{
    public class IScreen : UdonSharpBehaviour
    {
        public virtual void CalcInput(Vector3 deltaPosition, Quaternion deltaRotation) {}
        public virtual void OnGrab() {}
        public virtual void OnRelease() {}
        public virtual void UpdateAll() {}
        public virtual void ScaleChanged() {}
        public virtual void CellChanged() {}
        public virtual void EdgeChanged() {}
        public virtual void XrayChanged() {}
        public virtual void FovChanged() {}
        public virtual void FlipChanged() {}
        public virtual void GlassChanged() {}
        public virtual void VisChanged() {}
        public virtual void ColorsChanged() {}
        public virtual void WRotationChanged() {}
        public virtual void NonWRotationChanged() {}
        public virtual void InertialChanged() {}
        public virtual void SpeedWChanged() {}
        public virtual void SpeedNonWChanged() {}
        public virtual void AxisChanged() {}
        public virtual void RotateChanged() {}
    }
}