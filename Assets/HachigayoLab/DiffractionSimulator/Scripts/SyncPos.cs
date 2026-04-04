
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HachigayoLab.DiffractionSimulator
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SyncPos : UdonSharpBehaviour
    {
        [UdonSynced, FieldChangeCallback(nameof(Value))] Vector3 vValue;
        public Vector3 Value { get => vValue; set { transform.localPosition = vValue = value.normalized * distance; } }
        VRCPlayerApi localPlayer;
        float count, requestInterval = .2f, distance;
        bool request, grabbed;
        void Start()
        {
            localPlayer = Networking.LocalPlayer;
            distance = transform.localPosition.magnitude;
        }

        void Update()
        {
            if (grabbed) SetValue(transform.localPosition);
            if (count > 0) count -= Time.deltaTime;
            if (request && count <= 0)
            {
                RequestSerialization();
                count = requestInterval;
                request = false;
            }
        }

        public void SetValue(Vector3 value)
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
