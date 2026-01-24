
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using TMPro;

namespace HachigayoLab.UISync
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class UISyncInputField : UdonSharpBehaviour
    {
        [SerializeField] float requestInterval = .5f;
        float count;
        bool request = true;
        VRCPlayerApi localPlayer;
        TMP_InputField inputField;

        [UdonSynced, FieldChangeCallback(nameof(Value))] string sValue;
        public string Value { get => sValue; set { inputField.text = sValue = value; } }
        public void SetValue(string value)
        {
            if (Value == value) return;
            if (!localPlayer.IsOwner(gameObject)) { Networking.SetOwner(localPlayer, gameObject); }
            Value = value;
            request = true;
        }

        public void OnValueChanged() { SetValue(inputField.text); }

        void Start()
        {
            inputField = GetComponent<TMP_InputField>();
            localPlayer = Networking.LocalPlayer;
            if (localPlayer.IsOwner(gameObject)) { Value = inputField.text; }
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
