
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HachigayoLab.PolychoraViewer
{
    public class SizeManager : UdonSharpBehaviour
    {
        Vector3 initPosition, lastPosition;
        Quaternion initRotation;
        [SerializeField] GameObject target;
        bool use;
        void Start()
        {
            initPosition = transform.localPosition;
            initRotation = transform.localRotation;
        }
        void Update()
        {
            if (use)
            {
                Vector3 deltaPosition = transform.position - lastPosition;
                float scale = Mathf.Clamp(target.transform.localScale.x + deltaPosition.y, 0.01f, 100f);
                target.transform.localScale = new Vector3(scale, scale, scale);
            }
            transform.localPosition = initPosition;
            transform.localRotation = initRotation;
            lastPosition = transform.position;
        }
        public override void OnPickupUseDown()
        {
            use = true;
        }

        public override void OnPickupUseUp()
        {
            use = false;
        }
    }
}