
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace HachigayoLab.PolychoraViewer
{
    public class Activater : UdonSharpBehaviour
    {
        [UdonSynced, FieldChangeCallback(nameof(Active))] bool bActive;
        public bool Active
        {
            get => bActive;
            set
            {
                bActive = value;
                toggle.isOn = value;
                target.SetActive(value);
            }
        }
        Toggle toggle;
        [SerializeField] GameObject target;
        void Start()
        {
            toggle = GetComponent<Toggle>();
        }
        public void OnToggle()
        {
            Active = toggle.isOn;
            RequestSerialization();
        }
    }
}