
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HachigayoLab.PolychoraViewer
{
    public class SixDegreesController : UdonSharpBehaviour
    {
        Vector3 initPosition, lastPosition;
        Quaternion initRotation, lastRotation;
        IScreen screen;
        bool use;
        [SerializeField] bool zRotation = true;
        void Start()
        {
            initPosition = transform.localPosition;
            if (!zRotation) initPosition.z -= 0.002f;
            initRotation = transform.localRotation;
            screen = transform.parent.parent.GetComponent<IScreen>();
        }
        void Update()
        {
            if (!zRotation)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, initPosition.z);
                transform.localRotation = new Quaternion(initRotation.x, initRotation.y, transform.localRotation.z, transform.localRotation.w);
            }
            if (use)
            {
                Quaternion relativeRotation = transform.parent.localRotation;
                Vector3 deltaPosition = relativeRotation * (lastPosition - transform.localPosition);
                Quaternion deltaRotation = relativeRotation * transform.localRotation * Quaternion.Inverse(relativeRotation * lastRotation);
                screen.CalcInput(deltaPosition, deltaRotation);
                lastPosition = transform.localPosition;
                lastRotation = transform.localRotation;
            }
        }
        public override void OnPickupUseDown()
        {
            use = true;
            lastPosition = transform.localPosition;
            lastRotation = transform.localRotation;
            screen.OnGrab();
        }

        public override void OnPickupUseUp()
        {
            use = false;
            screen.OnRelease();
        }

        public override void OnDrop() 
        {
            transform.localPosition = initPosition;
            transform.localRotation = initRotation;
        }
    }
}