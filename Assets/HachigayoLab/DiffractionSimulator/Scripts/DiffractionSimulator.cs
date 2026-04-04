
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace HachigayoLab.DiffractionSimulator
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DiffractionSimulator : UdonSharpBehaviour
    {
        [SerializeField] GameObject crystalPrefab, rayInPrefab, rayOutPrefab, planePrefab;
        [SerializeField] float lConst = 5.43f, lambda = 1.54f;
        [SerializeField] Slider thickness, region, scale, plane;
        [SerializeField] Toggle diamond;
        float fPlane;
        bool bDiamond;
        [SerializeField]
        Vector3[] coords = new Vector3[]
        {
            Vector3.zero,
            new Vector3(0, .5f, .5f),
            new Vector3(.5f, 0, .5f),
            new Vector3(.5f, .5f, 0),
            Vector3.one * .25f,
            new Vector3(.25f, .75f, .75f),
            new Vector3(.75f, .25f, .75f),
            new Vector3(.75f, .75f, .25f),
        };
        Material[] crystalMat, rayInMat, rayOutMat;
        Transform rayInT, rayOutT, dirInT, dirOutT, crystalT, planeT;
        Transform[] planeTs;
        SyncPos dirIn, dirOut;
        SyncRot crystal;
        Matrix4x4 axis;
        GameObject[] lattice, planes;
        void Start()
        {
            axis = new Matrix4x4(new Vector4(lConst, 0, 0, 0), new Vector4(0, lConst, 0, 0), new Vector4(0, 0, lConst, 0), new Vector4(0, 0, 0, 1));
            crystalT = transform.Find("Crystal");
            rayInT = transform.Find("RayIn");
            rayOutT = transform.Find("RayOut");
            dirInT = transform.Find("DirIn");
            dirOutT = transform.Find("DirOut");
            crystal = crystalT.GetComponent<SyncRot>();
            dirIn = dirInT.GetComponent<SyncPos>();
            dirOut = dirOutT.GetComponent<SyncPos>();
            crystalMat = new Material[coords.Length];
            rayInMat = new Material[coords.Length];
            rayOutMat = new Material[coords.Length];
            lattice = new GameObject[coords.Length * 3];
            int c = 0;
            for (int i = 0; i < coords.Length; i++)
            {
                lattice[c] = Instantiate(crystalPrefab, crystalT);
                crystalMat[i] = lattice[c].GetComponent<MeshRenderer>().material;
                crystalMat[i].SetMatrix("_Axis", axis);
                crystalMat[i].SetVector("_Coord", coords[i]);
                c++;
                lattice[c] = Instantiate(rayInPrefab, rayInT);
                rayInMat[i] = lattice[c].GetComponent<MeshRenderer>().material;
                rayInMat[i].SetVector("_Coord", coords[i]);
                c++;
                lattice[c] = Instantiate(rayOutPrefab, rayOutT);
                rayOutMat[i] = lattice[c].GetComponent<MeshRenderer>().material;
                rayOutMat[i].SetVector("_Coord", coords[i]);
                c++;
            }
            for (int i = 3; i < lattice.Length; i++) lattice[i].SetActive(false);
            planeT = transform.Find("Plane");
            planes = new GameObject[4];
            planeTs = new Transform[planes.Length];
            for (int i = 0; i < planes.Length; i++)
            {
                planes[i] = Instantiate(planePrefab, planeT);
                planeTs[i] = planes[i].transform;
                planes[i].SetActive(false);
            }
        }

        void Update()
        {
            rayInT.rotation = dirInT.rotation = Quaternion.LookRotation(dirInT.position - rayInT.position);
            rayOutT.rotation = dirOutT.rotation = Quaternion.LookRotation(dirOutT.position - rayOutT.position);
            Matrix4x4 mIn = Matrix4x4.TRS(Vector3.zero, Quaternion.Inverse(rayInT.rotation) * crystalT.rotation, Vector3.one) * axis;
            Matrix4x4 mOut = Matrix4x4.TRS(Vector3.zero, Quaternion.Inverse(rayOutT.rotation) * crystalT.rotation, Vector3.one) * axis;
            for (int i = 0; i < coords.Length; i++)
            {
                rayInMat[i].SetMatrix("_Axis", mIn);
                rayOutMat[i].SetMatrix("_InAxis", mIn);
                rayOutMat[i].SetMatrix("_Axis", mOut);
            }
            planeT.rotation = Quaternion.LookRotation(dirInT.position - rayInT.position + dirOutT.position - rayOutT.position);
            float d = lambda * .5f / Mathf.Sin(Mathf.Deg2Rad * Vector3.Angle(-rayInT.forward, rayOutT.forward) * .5f);
            for (int i = 1; i < planes.Length; i++) planeTs[i].localPosition = Vector3.forward * i * d;

            if (Utilities.IsValid(thickness)) for (int i = 0; i < coords.Length; i++)
            {
                rayInMat[i].SetFloat("_Scale", thickness.value * .3f);
                rayOutMat[i].SetFloat("_Scale", thickness.value * .3f);
            }
            if (Utilities.IsValid(region)) for (int i = 0; i < coords.Length; i++)
            {
                crystalMat[i].SetFloat("_Region", region.value);
                rayInMat[i].SetFloat("_Region", region.value);
                rayOutMat[i].SetFloat("_Region", region.value);
            }
            if (Utilities.IsValid(scale)) transform.localScale = Vector3.one * scale.value * .02f;
            if (Utilities.IsValid(plane) && (plane.value != fPlane))
            {
                fPlane = plane.value;
                int c = (int)fPlane;
                for (int i = 0; i < c; i++) planes[i].SetActive(true);
                for (int i = c; i < planes.Length; i++) planes[i].SetActive(false);
            }
            if (Utilities.IsValid(diamond) && (diamond.isOn != bDiamond))
            {
                bDiamond = diamond.isOn;
                for (int i = 3; i < lattice.Length; i++) lattice[i].SetActive(bDiamond);
            }
        }

        void SetMiller(Vector3 miller)
        {
            crystal.SetValue(Quaternion.FromToRotation(miller, Vector3.up));
            float theta = Mathf.Asin(lambda * miller.magnitude / (2 * lConst));
            Vector3 v = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0);
            dirOut.SetValue(v);
            v.x = -v.x;
            dirIn.SetValue(v);
        }

        public void M100() { SetMiller(new Vector3(1, 0, 0)); }
        public void M110() { SetMiller(new Vector3(1, 1, 0)); }
        public void M111() { SetMiller(new Vector3(1, 1, 1)); }
        public void M200() { SetMiller(new Vector3(2, 0, 0)); }
        public void M220() { SetMiller(new Vector3(2, 2, 0)); }
        public void M311() { SetMiller(new Vector3(3, 1, 1)); }
        public void M400() { SetMiller(new Vector3(4, 0, 0)); }
        public void M331() { SetMiller(new Vector3(3, 3, 1)); }
        public void M422() { SetMiller(new Vector3(4, 2, 2)); }
        public void M333() { SetMiller(new Vector3(3, 3, 3)); }
        public void M511() { SetMiller(new Vector3(5, 1, 1)); }
        public void M440() { SetMiller(new Vector3(4, 4, 0)); }
        public void M531() { SetMiller(new Vector3(5, 3, 1)); }
        public void M620() { SetMiller(new Vector3(6, 2, 0)); }
        public void M533() { SetMiller(new Vector3(5, 3, 3)); }
        public void M444() { SetMiller(new Vector3(4, 4, 4)); }
    }
}
