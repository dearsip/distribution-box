
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HachigayoLab.PolychoraViewer
{
    public class CursorRenderer : UdonSharpBehaviour
    {
        Mesh mesh;
        void Start()
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().sharedMesh = mesh;
            mesh.SetVertices( new Vector3[]{new Vector3(.5f,.5f,.5f),new Vector3(-.5f,-.5f,-.5f),new Vector3(.5f,-.5f,.5f),new Vector3(-.5f,.5f,-.5f),new Vector3(-.5f,-.5f,.5f),new Vector3(.5f,.5f,-.5f),new Vector3(-.5f,.5f,.5f),new Vector3(.5f,-.5f,-.5f)});
            mesh.SetIndices(new int[]{0,1,2,3,4,5,6,7}, MeshTopology.Lines, 0);
            mesh.SetColors(new Color[]{Color.white,Color.white,Color.white,Color.white,Color.white,Color.white,Color.white,Color.white});
        }
    }
}