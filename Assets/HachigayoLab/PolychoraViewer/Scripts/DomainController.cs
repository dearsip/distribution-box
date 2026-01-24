
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HachigayoLab.PolychoraViewer
{
    public class DomainController : UdonSharpBehaviour
    {
        DomainManager domainManager;
        void Start()
        {
            domainManager = transform.parent.GetComponent<DomainManager>();
        }
        public override void OnPickup()
        {
            domainManager.Grab();
        }
        public override void OnDrop() 
        {
            domainManager.Drop();
        }
    }
}
