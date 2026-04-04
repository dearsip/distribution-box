
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HachigayoLab.DiffractionSimulator
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SyncRot : UdonSharpBehaviour
    {
        [UdonSynced, FieldChangeCallback(nameof(Value))] Quaternion qValue;
        public Quaternion Value { get => qValue; set { transform.localRotation = qValue = value; } }
        VRCPlayerApi localPlayer;
        float count, requestInterval = .2f;
        bool request, grabbed;
        void Start()
        {
            localPlayer = Networking.LocalPlayer;
        }

        void Update()
        {
            if (grabbed) SetValue(transform.localRotation);
            if (count > 0) count -= Time.deltaTime;
            if (request && count <= 0)
            {
                RequestSerialization();
                count = requestInterval;
                request = false;
            }
        }

        public void SetValue(Quaternion value)
        {
            if (Value == value) return;
            if (!localPlayer.IsOwner(gameObject)) Networking.SetOwner(localPlayer, gameObject);
            Value = value;
            request = true;
        }

        public override void OnPickup()
        {
            Networking.SetOwner(localPlayer, gameObject);
            grabbed = true;
        }

        public override void OnDrop()
        {
            grabbed = false;
        }

    }
}
