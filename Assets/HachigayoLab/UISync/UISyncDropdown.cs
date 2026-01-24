
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using TMPro;

namespace HachigayoLab.UISync
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class UISyncDropdown : UdonSharpBehaviour
    {
        [SerializeField] float requestInterval = .5f;
        float count;
        bool request = true;
        VRCPlayerApi localPlayer;
        TMP_Dropdown dropdown;

        [UdonSynced, FieldChangeCallback(nameof(Value))] int iValue;
        public int Value { get => iValue; set { dropdown.value = iValue = value; } }
        public void SetValue(int value)
        {
            if (Value == value) return;
            if (!localPlayer.IsOwner(gameObject)) { Networking.SetOwner(localPlayer, gameObject); }
            Value = value;
            request = true;
        }

        public void OnValueChanged() { SetValue(dropdown.value); }

        void Start()
        {
            dropdown = GetComponent<TMP_Dropdown>();
            localPlayer = Networking.LocalPlayer;
            if (localPlayer.IsOwner(gameObject)) { Value = dropdown.value; }
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
