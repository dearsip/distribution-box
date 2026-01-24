
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Rendering;
using VRC.SDKBase;
using VRC.Udon.Common;
using VRC.Udon;

namespace HachigayoLab.FloatingTool
{
    public class FloatingTool : UdonSharpBehaviour
    {
        [SerializeField] float maxSpeed = 3.0f;
        [SerializeField] float accel = 2f;
        bool active, hover, vr;
        float speed, gravity;
        Vector3 move;
        Collider floor;
        VRCPlayerApi localPlayer;
        public override void InputMoveHorizontal(float value, UdonInputEventArgs args) { move.x = value; }
        public override void InputMoveVertical(float value, UdonInputEventArgs args) { move.z = value; }
        public override void InputLookVertical(float value, UdonInputEventArgs args) { move.y = value; }

        public override void InputJump(bool value, UdonInputEventArgs args)
        {
            if (args.boolValue)
            {
                if (active)
                {
                    active = hover = floor.enabled = false;
                    localPlayer.SetVelocity(localPlayer.GetVelocity());
                    localPlayer.SetGravityStrength(gravity);
                }
                else if (!localPlayer.IsPlayerGrounded())
                {
                    active = true;
                    gravity = localPlayer.GetGravityStrength();
                    localPlayer.SetGravityStrength(0);
                }
            }
        }

        void Start()
        {
            floor = GetComponent<Collider>();
            floor.enabled = false;
            localPlayer = Networking.LocalPlayer;
            vr = localPlayer.IsUserInVR();
        }

        void Update()
        {
            Vector3 playerPosition = localPlayer.GetPosition();

            Vector3 v = move;
            if (!vr)
            {
                v = Vector3.zero;
                if (Input.GetKey(KeyCode.A)) v += Vector3.left;
                if (Input.GetKey(KeyCode.D)) v += Vector3.right;
                if (Input.GetKey(KeyCode.W)) v += Vector3.forward;
                if (Input.GetKey(KeyCode.S)) v += Vector3.back;
                if (Input.GetKey(KeyCode.Q)) v += Vector3.down;
                if (Input.GetKey(KeyCode.E)) v += Vector3.up;
            }

            bool b = v.sqrMagnitude < .001f;
            if (b) speed -= Time.deltaTime * 2;
            else speed += Time.deltaTime * 2;
            speed = Mathf.Clamp01(speed);
            if (!active) return;

            if (v.sqrMagnitude > 1f) v = v.normalized;
            v = localPlayer.GetRotation() * v * (maxSpeed * speed);
            v = Vector3.Lerp(localPlayer.GetVelocity(), v, .2f);

        #if !UNITY_EDITOR
            if (((!vr && VRCCameraSettings.PhotoCamera.Active) ||
                VRCCameraSettings.PhotoCamera.CameraMode == VRCCameraMode.DroneHandheld ||
                VRCCameraSettings.PhotoCamera.CameraMode == VRCCameraMode.DroneFPV)
                && !Input.GetMouseButton(1))
                v = Vector3.zero;

            b = b && v.sqrMagnitude < .001f;
            if (!hover && b)
            {
                floor.enabled = true;
                transform.position = playerPosition + Vector3.up * .005f;
            }
            else if (hover && !b)
            {
                floor.enabled = false;
            }
            hover = b;
            if (!hover)
        #endif
                localPlayer.SetVelocity(v);
        }
    }
}
