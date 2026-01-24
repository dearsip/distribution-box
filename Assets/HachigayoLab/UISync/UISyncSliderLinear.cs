
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

namespace HachigayoLab.UISync
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
    public class UISyncSliderLinear : UdonSharpBehaviour
    {
        [SerializeField] float requestInterval = .5f;
        VRCPlayerApi localPlayer;
        Slider slider;

        [UdonSynced(UdonSyncMode.Linear), FieldChangeCallback(nameof(Value))] float fValue;
        public float Value { get => fValue; set { slider.value = fValue = value; } }
        public void SetValue(float value)
        {
            if (Value == value) return;
            if (!localPlayer.IsOwner(gameObject)) { Networking.SetOwner(localPlayer, gameObject); }
            Value = value;
        }

        public void OnValueChanged() { SetValue(slider.value); }

        void Start()
        {
            slider = GetComponent<Slider>();
            localPlayer = Networking.LocalPlayer;
            if (localPlayer.IsOwner(gameObject)) { Value = slider.value; }
        }
    }
}
