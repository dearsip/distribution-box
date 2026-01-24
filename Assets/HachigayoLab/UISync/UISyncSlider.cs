
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

namespace HachigayoLab.UISync
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class UISyncSlider : UdonSharpBehaviour
    {
        [SerializeField] float requestInterval = .5f;
        float count;
        bool request = true;
        VRCPlayerApi localPlayer;
        Slider slider;

        [UdonSynced, FieldChangeCallback(nameof(Value))] float fValue;
        public float Value { get => fValue; set { slider.value = fValue = value; } }
        public void SetValue(float value)
        {
            if (Value == value) return;
            if (!localPlayer.IsOwner(gameObject)) { Networking.SetOwner(localPlayer, gameObject); }
            Value = value;
            request = true;
        }

        public void OnValueChanged() { SetValue(slider.value); }

        void Start()
        {
            slider = GetComponent<Slider>();
            localPlayer = Networking.LocalPlayer;
            if (localPlayer.IsOwner(gameObject)) { Value = slider.value; }
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
