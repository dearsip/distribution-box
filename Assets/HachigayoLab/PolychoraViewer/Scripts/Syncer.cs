
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HachigayoLab.PolychoraViewer
{
    public class Syncer : UdonSharpBehaviour
    {
        [SerializeField] IScreen screen;
        float count;
        public void Initialize()
        {
            //if (Networking.LocalPlayer.IsOwner(gameObject)) { screen.UpdateAll(); }
            screen.UpdateAll();
        }
        public override void OnPlayerJoined(VRCPlayerApi player) { RequestSerialization(); }
        void Update()
        {
            if (count > 0) { count -= Time.deltaTime; if (count <= 0) { RequestSerialization(); } }
        }

        [UdonSynced, FieldChangeCallback(nameof(Scale))] float fScale;
        public float Scale { get => fScale; set { fScale = value; screen.ScaleChanged(); } }
        public void SetScale(float scale)
        {
            if (!Networking.LocalPlayer.IsOwner(gameObject)) { Networking.SetOwner(Networking.LocalPlayer, gameObject); }
            Scale = scale;
            count = 0.1f;
        }

        [UdonSynced, FieldChangeCallback(nameof(Cell))] float fCell;
        public float Cell { get => fCell; set { fCell = value; screen.CellChanged(); } }
        public void SetCell(float cell)
        {
            if (!Networking.LocalPlayer.IsOwner(gameObject)) { Networking.SetOwner(Networking.LocalPlayer, gameObject); }
            Cell = cell;
            count = 0.1f;
        }

        [UdonSynced, FieldChangeCallback(nameof(Edge))] float fEdge;
        public float Edge { get => fEdge; set { fEdge = value; screen.EdgeChanged(); } }
        public void SetEdge(float edge)
        {
            if (!Networking.LocalPlayer.IsOwner(gameObject)) { Networking.SetOwner(Networking.LocalPlayer, gameObject); }
            Edge = edge;
            count = 0.1f;
        }

        [UdonSynced, FieldChangeCallback(nameof(Xray))] float fXray;
        public float Xray { get => fXray; set { fXray = value; screen.XrayChanged(); } }
        public void SetXray(float xray)
        {
            if (!Networking.LocalPlayer.IsOwner(gameObject)) { Networking.SetOwner(Networking.LocalPlayer, gameObject); }
            Xray = xray;
            count = 0.1f;
        }

        [UdonSynced, FieldChangeCallback(nameof(Fov))] float fFov;
        public float Fov { get => fFov; set { fFov = value; screen.FovChanged(); } }
        public void SetFov(float fov)
        {
            if (!Networking.LocalPlayer.IsOwner(gameObject)) { Networking.SetOwner(Networking.LocalPlayer, gameObject); }
            Fov = fov;
            count = 0.1f;
        }

        [UdonSynced, FieldChangeCallback(nameof(Flip))] bool bFlip;
        public bool Flip { get => bFlip; set { bFlip = value; screen.FlipChanged(); } }
        public void SetFlip(bool flip)
        {
            if (!Networking.LocalPlayer.IsOwner(gameObject)) { Networking.SetOwner(Networking.LocalPlayer, gameObject); }
            Flip = flip;
            count = 0.1f;
        }

        [UdonSynced, FieldChangeCallback(nameof(Glass))] bool bGlass;
        public bool Glass { get => bGlass; set { bGlass = value; screen.GlassChanged(); } }
        public void SetGlass(bool glass)
        {
            if (!Networking.LocalPlayer.IsOwner(gameObject)) { Networking.SetOwner(Networking.LocalPlayer, gameObject);
            }
            Glass = glass;
            count = 0.1f;
        }

        [UdonSynced, FieldChangeCallback(nameof(Vis))] int iVis;
        public int Vis { get => iVis; set { iVis = value; screen.VisChanged(); } }
        public void SetVis(int vis)
        {
            if (!Networking.LocalPlayer.IsOwner(gameObject)) { Networking.SetOwner(Networking.LocalPlayer, gameObject);
            }
            Vis = vis;
            count = 0.1f;
        }

        [UdonSynced] Color[] cColors;
        public Color[] Colors { get => cColors; set { cColors = value; } }
        [UdonSynced, FieldChangeCallback(nameof(ColorIndex))] int iColorIndex;
        public int ColorIndex { get => iColorIndex; set { iColorIndex = value; screen.ColorsChanged(); } }
        public void SetColors(Color[] colors)
        {
            if (!Networking.LocalPlayer.IsOwner(gameObject)) { Networking.SetOwner(Networking.LocalPlayer, gameObject); }
            Colors = colors;
            ColorIndex = (ColorIndex + 1) % 8192;
            count = 0.1f;
        }

        [UdonSynced, FieldChangeCallback(nameof(WRotation))] bool bWRotation;
        public bool WRotation { get => bWRotation; set { bWRotation = value; screen.WRotationChanged(); } }
        public void SetWRotation(bool wRotation)
        {
            if (!Networking.LocalPlayer.IsOwner(gameObject)) { Networking.SetOwner(Networking.LocalPlayer, gameObject); }
            WRotation = wRotation;
            count = 0.1f;
        }

        [UdonSynced, FieldChangeCallback(nameof(NonWRotation))] bool bNonWRotation;
        public bool NonWRotation { get => bNonWRotation; set { bNonWRotation = value; screen.NonWRotationChanged(); } }
        public void SetNonWRotation(bool nonWRotation)
        {
            if (!Networking.LocalPlayer.IsOwner(gameObject)) { Networking.SetOwner(Networking.LocalPlayer, gameObject); }
            NonWRotation = nonWRotation;
            count = 0.1f;
        }

        [UdonSynced, FieldChangeCallback(nameof(Inertial))] bool bInertial;
        public bool Inertial { get => bInertial; set { bInertial = value; screen.InertialChanged(); } }
        public void SetInertial(bool inertial)
        {
            if (!Networking.LocalPlayer.IsOwner(gameObject)) { Networking.SetOwner(Networking.LocalPlayer, gameObject); }
            Inertial = inertial;
            count = 0.1f;
        }

        [UdonSynced, FieldChangeCallback(nameof(SpeedW))] float fSpeedW;
        public float SpeedW { get => fSpeedW; set { fSpeedW = value; screen.SpeedWChanged(); } }
        public void SetSpeedW(float speedW)
        {
            if (!Networking.LocalPlayer.IsOwner(gameObject)) { Networking.SetOwner(Networking.LocalPlayer, gameObject); }
            SpeedW = speedW;
            count = 0.1f;
        }

        [UdonSynced, FieldChangeCallback(nameof(SpeedNonW))] float fSpeedNonW;
        public float SpeedNonW { get => fSpeedNonW; set { fSpeedNonW = value; screen.SpeedNonWChanged(); } }
        public void SetSpeedNonW(float speedNonW) {
            if (!Networking.LocalPlayer.IsOwner(gameObject)) { Networking.SetOwner(Networking.LocalPlayer, gameObject); }
            SpeedNonW = speedNonW;
            count = 0.1f;
        }

        [UdonSynced, FieldChangeCallback(nameof(Axis0))] Vector4 v4Axis0;
        [UdonSynced, FieldChangeCallback(nameof(Axis1))] Vector4 v4Axis1;
        [UdonSynced, FieldChangeCallback(nameof(Axis2))] Vector4 v4Axis2;
        [UdonSynced, FieldChangeCallback(nameof(Axis3))] Vector4 v4Axis3;
        public Vector4 Axis0 { get => v4Axis0; set { v4Axis0 = value; screen.AxisChanged(); } }
        public Vector4 Axis1 { get => v4Axis1; set { v4Axis1 = value; screen.AxisChanged(); } }
        public Vector4 Axis2 { get => v4Axis2; set { v4Axis2 = value; screen.AxisChanged(); } }
        public Vector4 Axis3 { get => v4Axis3; set { v4Axis3 = value; screen.AxisChanged(); } }
        public void SetAxis(Vector4 v0, Vector4 v1, Vector4 v2, Vector4 v3)
        {
            if (!Networking.LocalPlayer.IsOwner(gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
            }
            Axis0 = v0; Axis1 = v1; Axis2 = v2; Axis3 = v3;
            count = 0.1f;
        }

        [UdonSynced, FieldChangeCallback(nameof(Rotate0))] Vector4 v4Rotate0;
        [UdonSynced, FieldChangeCallback(nameof(Rotate1))] Vector4 v4Rotate1;
        [UdonSynced, FieldChangeCallback(nameof(Rotate2))] Vector4 v4Rotate2;
        [UdonSynced, FieldChangeCallback(nameof(Rotate3))] Vector4 v4Rotate3;
        public Vector4 Rotate0 { get => v4Rotate0; set { v4Rotate0 = value; screen.RotateChanged(); } }
        public Vector4 Rotate1 { get => v4Rotate1; set { v4Rotate1 = value; screen.RotateChanged(); } }
        public Vector4 Rotate2 { get => v4Rotate2; set { v4Rotate2 = value; screen.RotateChanged(); } }
        public Vector4 Rotate3 { get => v4Rotate3; set { v4Rotate3 = value; screen.RotateChanged(); } }
        public void SetRotate(Vector4 v0, Vector4 v1, Vector4 v2, Vector4 v3)
        {
            if (!Networking.LocalPlayer.IsOwner(gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
            }
            Rotate0 = v0; Rotate1 = v1; Rotate2 = v2; Rotate3 = v3;
            count = 0.1f;
        }
    }
}