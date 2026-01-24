
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

namespace HachigayoLab.PolychoraViewer
{
    public class DomainManager : UdonSharpBehaviour
    {
        [UdonSynced(UdonSyncMode.Smooth)] Vector3 position;
        [SerializeField] Vector4[] _mirrors;
        [SerializeField] Matrix4x4 rotation = Matrix4x4.identity;
        [SerializeField] Vector4 initialPosition = new Vector4(1, 0, 0, 0); 
        [SerializeField] Toggle[] uniformerizers;
        int dim;
        Vector4[] rotMirrors;
        Vector3[] mirrors;
        float[] thresholds;
        Vector4[] _roots, rotRoots;
        Vector3[] roots;
        Transform controller;
        UniformScreen screen;
        bool grabbed;

        void GramSchmidt(Vector4[] vectors)
        {
            for (int i = 0; i < vectors.Length; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    vectors[i] -= Vector4.Project(vectors[i], vectors[j]);
                }
                vectors[i] = vectors[i].normalized;
            }
        }

        void Start()
        {
            controller = transform.Find("Controller");
            screen = transform.parent.parent.GetComponent<UniformScreen>();
            dim = _mirrors.Length;
            _roots = new Vector4[dim];

            Vector4[] orthos = new Vector4[dim];
            for (int i = 0; i < dim; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    orthos[j] = _mirrors[(1+i+j) % dim];
                }
                GramSchmidt(orthos);
                _roots[i] = orthos[dim-1];
            }
            mirrors = new Vector3[dim];
            thresholds = new float[dim];
            roots = new Vector3[dim];
            rotRoots = new Vector4[dim];
            rotMirrors = new Vector4[dim];
            for (int i = 0; i < dim; i++)
            {
                rotRoots[i] = rotation * _roots[i];
                rotMirrors[i] = rotation * _mirrors[i];
            }
            for (int i = 0; i < dim; i++)
            {
                mirrors[i] = new Vector3(rotMirrors[i].x, rotMirrors[i].y, rotMirrors[i].z).normalized;
                thresholds[i] = rotMirrors[i].w / Mathf.Sqrt(1 - rotMirrors[i].w * rotMirrors[i].w);
                roots[i] = new Vector3(-rotRoots[i].x / rotRoots[i].w, -rotRoots[i].y / rotRoots[i].w, -rotRoots[i].z / rotRoots[i].w);
            }

            Transform target;
            target = transform.Find("XY");
            target.localPosition = (roots[0] + roots[1]) * 0.5f;
            target.localRotation = Quaternion.LookRotation(roots[1] - roots[0]) * Quaternion.LookRotation(Vector3.up);
            target.localScale = new Vector3(0.02f, Vector3.Distance(roots[0], roots[1]) * 0.5f, 0.02f);
            target = transform.Find("XZ");
            target.localPosition = (roots[0] + roots[2]) * 0.5f;
            target.localRotation = Quaternion.LookRotation(roots[2] - roots[0]) * Quaternion.LookRotation(Vector3.up);
            target.localScale = new Vector3(0.02f, Vector3.Distance(roots[0], roots[2]) * 0.5f, 0.02f);
            target = transform.Find("YZ");
            target.localPosition = (roots[1] + roots[2]) * 0.5f;
            target.localRotation = Quaternion.LookRotation(roots[2] - roots[1]) * Quaternion.LookRotation(Vector3.up);
            target.localScale = new Vector3(0.02f, Vector3.Distance(roots[1], roots[2]) * 0.5f, 0.02f);
            if (dim > 3)
            {
                transform.Find("W").localPosition = roots[0];
                transform.Find("Z").localPosition = roots[1];
                transform.Find("Y").localPosition = roots[2];
                transform.Find("X").localPosition = roots[3];
                target = transform.Find("XW");
                target.localPosition = (roots[0] + roots[3]) * 0.5f;
                target.localRotation = Quaternion.LookRotation(roots[3] - roots[0]) * Quaternion.LookRotation(Vector3.up);
                target.localScale = new Vector3(0.02f, Vector3.Distance(roots[0], roots[3]) * 0.5f, 0.02f);
                target = transform.Find("YW");
                target.localPosition = (roots[1] + roots[3]) * 0.5f;
                target.localRotation = Quaternion.LookRotation(roots[3] - roots[1]) * Quaternion.LookRotation(Vector3.up);
                target.localScale = new Vector3(0.02f, Vector3.Distance(roots[1], roots[3]) * 0.5f, 0.02f);
                target = transform.Find("ZW");
                target.localPosition = (roots[2] + roots[3]) * 0.5f;
                target.localRotation = Quaternion.LookRotation(roots[3] - roots[2]) * Quaternion.LookRotation(Vector3.up);
                target.localScale = new Vector3(0.02f, Vector3.Distance(roots[2], roots[3]) * 0.5f, 0.02f);
            }
            else
            {
                transform.Find("Z").localPosition = roots[0];
                transform.Find("Y").localPosition = roots[1];
                transform.Find("X").localPosition = roots[2];
                transform.Find("W").gameObject.SetActive(false);
                transform.Find("XW").gameObject.SetActive(false);
                transform.Find("YW").gameObject.SetActive(false);
                transform.Find("ZW").gameObject.SetActive(false);
            }

            position = Vector3.zero;
            for (int i = 0; i < dim; i++)
            {
                position += roots[i] * initialPosition[i];
            }
            controller.localPosition = position;
        }

        void Update()
        {
            if (controller != null)
            {
                if (grabbed) position = controller.localPosition;
                else controller.localPosition = position;
                int[] indices = new int[dim-1];
                Vector3 pos = controller.localPosition;
                Vector3[] mirrorsLog = new Vector3[dim-1];
                for (int i = 0; i < dim-1; i++)
                {
                    float min = float.MaxValue;
                    int minIndex = -1;
                    for (int j = 0; j < mirrors.Length; j++)
                    {
                        bool skip = false;
                        for (int k = 0; k < i; k++) if (j == indices[k]) { skip = true; break; }
                        if (skip) continue;
                        float dist = Vector3.Dot(pos, mirrors[j]) - thresholds[j];
                        if (dist < min)
                        {
                            min = dist;
                            minIndex = j;
                        }
                    }
                    if (min < -0.0001f)
                    {
                        Vector3 mirror = mirrors[minIndex];
                        for (int k = 0; k < i; k++)
                        {
                            mirror -= Vector3.Project(mirror, mirrorsLog[k]);
                        }
                        pos -= min * mirror / mirror.sqrMagnitude;
                        mirrorsLog[i] = mirror.normalized;
                        indices[i] = minIndex;
                    }
                    else break;
                }
                if (dim == 3) pos.z = 0;
                controller.localPosition = pos;

                screen.SetVertex(rotation.transpose * new Vector4(pos.x, pos.y, pos.z, -1).normalized);
            }
        }

        public void Uniformalize()
        {
            Vector4 output = Vector4.zero;
            for (int i = 0; i < uniformerizers.Length; i++)
                if (uniformerizers[i].isOn)
                {
                    output += rotRoots[i] / Vector4.Dot(_roots[i], _mirrors[i]);
                }
            if (output.sqrMagnitude > 0.0001f)
            {
                if (!Networking.LocalPlayer.IsOwner(gameObject)) { Networking.SetOwner(Networking.LocalPlayer, gameObject); }
                position = controller.localPosition = new Vector3(-output.x / output.w, -output.y / output.w, -output.z / output.w);
            }
        }

        public void SetPos(Vector3 pos)
        {
            controller.localPosition = pos;
        }

        public void Grab()
        {
            if (!Networking.LocalPlayer.IsOwner(gameObject)) { Networking.SetOwner(Networking.LocalPlayer, gameObject); }
            grabbed = true;
        }

        public void Drop()
        {
            grabbed = false;
        }
    }
}