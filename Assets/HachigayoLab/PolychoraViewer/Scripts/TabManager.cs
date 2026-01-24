
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HachigayoLab.PolychoraViewer
{
    public class TabManager : UdonSharpBehaviour
    {
        int currentTab = 0;
        [SerializeField] GameObject[] tabs;
        public void OnTabBack() { OnTabChange(-1); }
        public void OnTabForward() { OnTabChange(1); }
        void OnTabChange(int delta)
        {
            tabs[currentTab].SetActive(false);
            currentTab = (currentTab + delta + tabs.Length) % tabs.Length;
            tabs[currentTab].SetActive(true);
        }
    }
}