
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace HachigayoLab.PolychoraViewer
{
    public class UpdateValue : UdonSharpBehaviour
    {
        Text text;
        Slider slider;
        void Start()
        {
            text = GetComponent<Text>();
            slider = transform.parent.GetComponent<Slider>();
        }
        public void OnUpdateValue()
        {
            text.text = slider.value.ToString("F2");
        }
    }
}