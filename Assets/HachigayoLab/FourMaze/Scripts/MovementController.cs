
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace HachigayoLab.FourMaze
{
    public class MovementController : UdonSharpBehaviour
    {
        Vector3 initPosition, lastPosition, secLastPosition;
        Quaternion initRotation, lastRotation, secLastRotation;
        FourMaze owner;
        GameObject menu;
        bool use;
        [HideInInspector] public bool three;
        float height;
        [SerializeField] Slider forward, side;
        VRCPlayerApi localPlayer;

        void Start()
        {
            Transform t = transform.parent.parent;
            owner = t.GetComponent<FourMaze>();
            menu = t.Find("Menu").gameObject;
            initPosition = transform.position;
            initRotation = transform.localRotation;
            localPlayer = Networking.LocalPlayer;
            height = localPlayer.GetAvatarEyeHeightAsMeters();
            if (!localPlayer.IsUserInVR()) side.value = .1f;
        }
        void Update()
        {
            if (use)
            {
                Vector3 deltaPosition = (secLastPosition - transform.position) * 8 / height * side.value;
                Quaternion deltaRotation = Quaternion.SlerpUnclamped(Quaternion.identity, Quaternion.Inverse(secLastRotation) * transform.rotation, forward.value);
                float w = deltaRotation.x + deltaRotation.z;
                owner.CalcMovement(new Vector4(deltaPosition.x, deltaPosition.y, three ? 0 : deltaPosition.z, w));
                secLastPosition = lastPosition;
                secLastRotation = lastRotation;
                lastPosition = transform.position;
                lastRotation = transform.rotation;
            }
        }
        public override void OnPickupUseDown()
        {
            use = true;
            secLastPosition = lastPosition = transform.position;
            secLastRotation = lastRotation = transform.rotation;
        }

        public override void OnPickupUseUp()
        {
            use = false;
        }

        public override void OnDrop()
        {
            transform.position = initPosition;
            transform.rotation = initRotation;
            menu.SetActive(true);
        }

        public override void OnAvatarEyeHeightChanged(VRCPlayerApi player, float prevEyeHeightAsMeters)
        {
            if (player == localPlayer) height = localPlayer.GetAvatarEyeHeightAsMeters();
        }
    }
}
