
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HachigayoLab.DiffractionSimulator
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Director : UdonSharpBehaviour
    {
        public float distance;
        public Vector3 lastPosition;
        bool grabbed;
        void Start()
        {
            lastPosition = transform.localPosition;
            distance = lastPosition.magnitude;
        }
        void Update()
        {
            if (grabbed) lastPosition = transform.localPosition *= distance / transform.localPosition.magnitude;
            else transform.localPosition = lastPosition;
        }

        public override void OnPickup()
        {
            grabbed = true;
        }

        public override void OnDrop()
        {
            grabbed = false;
        }

    }
}
