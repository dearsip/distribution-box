
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace HachigayoLab.FourMaze
{
    public class RotationController : UdonSharpBehaviour
    {
        Vector3 initPosition, lastPosition, secLastPosition;
        Quaternion initRotation, lastRotation, secLastRotation;
        FourMaze owner;
        GameObject menu;
        bool use;
        [HideInInspector] public bool three;
        float height;
        [SerializeField] Slider look, roll;
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
            if (!localPlayer.IsUserInVR()) look.value = .1f;
        }
        void Update()
        {
            if (three)
            {
                Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(lastRotation);
                transform.localRotation = new Quaternion(0, 0, deltaRotation.z, deltaRotation.w) * lastRotation;
            }
            if (use)
            {
                Vector3 deltaPosition = (secLastPosition - transform.position) * 5f / height * look.value;
                Quaternion deltaRotation = Quaternion.SlerpUnclamped(Quaternion.identity, transform.rotation * Quaternion.Inverse(secLastRotation), roll.value);
                if (three)
                {
                    deltaPosition.z = 0;
                    deltaRotation[0] = 0;
                    deltaRotation[1] = 0;
                    deltaRotation = deltaRotation.normalized;
                }
                owner.CalcRotation(deltaPosition, deltaRotation);
                secLastPosition = lastPosition;
                secLastRotation = lastRotation;
                lastPosition = transform.position;
                lastRotation = transform.rotation;
            }
            secLastPosition = lastPosition;
            secLastRotation = lastRotation;
            lastPosition = transform.position;
            lastRotation = transform.rotation;
        }
        public override void OnPickupUseDown()
        {
            use = true;
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
