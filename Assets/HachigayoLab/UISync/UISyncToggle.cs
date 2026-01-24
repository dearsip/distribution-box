
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

namespace HachigayoLab.UISync
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class UISyncToggle : UdonSharpBehaviour
    {
        [SerializeField] float requestInterval = .5f;
        float count;
        bool request = true;
        VRCPlayerApi localPlayer;
        Toggle toggle;

        [UdonSynced, FieldChangeCallback(nameof(Value))] bool bValue;
        public bool Value { get => bValue; set { toggle.isOn = bValue = value; } }
        public void SetValue(bool value)
        {
            if (Value == value) return;
            if (!localPlayer.IsOwner(gameObject)) { Networking.SetOwner(localPlayer, gameObject); }
            Value = value;
            request = true;
        }

        public void OnValueChanged() { SetValue(toggle.isOn); }

        void Start()
        {
            toggle = GetComponent<Toggle>();
            localPlayer = Networking.LocalPlayer;
            if (localPlayer.IsOwner(gameObject)) { Value = toggle.isOn; }
        }

        void Update()
        {
            if (count > 0) count -= Time.deltaTime;
            if (request && count <= 0)
            {
                RequestSerialization();
                count = requestInterval;
                request = false;
            }
        }
    }
}
