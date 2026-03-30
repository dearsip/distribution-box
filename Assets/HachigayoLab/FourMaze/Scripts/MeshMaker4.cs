#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.ParticleSystemJobs;

public class MeshMaker4 : MonoBehaviour
{
    [SerializeField] string shapeName;
    int dim = 4;
    float faceSize = .49f;
    float[][] regF;
    void Zero(float[] dest) { for (int i = 0; i < dest.Length; i++) dest[i] = 0; }
    void Apply(int dir, float[] p, float distance)
    {
        if ((dir & 1) == 1) distance = -distance;
        p[dir >> 1] += distance;
    }
    void Sub(float[] dest, float[] src1, float[] src2) { for (int i = 0; i < dest.Length; i++) dest[i] = src1[i] - src2[i]; }
    void Copy(float[] dest, float[] src) { for (int i = 0; i < dest.Length; i++) dest[i] = src[i]; }
    int vertexCount;
    Vector3[] vertices;
    Vector4[] another, position;
    int[] drawer_a = new int[] { 0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 4 };
    bool[] drawer_o = new bool[] { false, false, true, false, false, true, true, false, false, false, true, true, false, true, true, false };
    int ForAxis(int a, bool opposite) { return (a << 1) + (opposite ? 1 : 0); }
    void AddLine(float[] p1, float[] p2)
    {
        vertices[vertexCount] = new Vector3(vertexCount / 48, 1 - vertexCount % 2 * 2, 0);
        another[vertexCount] = new Vector4(p2[0], p2[1], p2[2], p2[3]);
        position[vertexCount] = new Vector4(p1[0], p1[1], p1[2], p1[3]);
        vertexCount++;
        vertices[vertexCount] = new Vector3(vertexCount / 48, 1 - vertexCount % 2 * 2, 0);
        another[vertexCount] = new Vector4(p2[0], p2[1], p2[2], p2[3]);
        position[vertexCount] = new Vector4(p1[0], p1[1], p1[2], p1[3]);
        vertexCount++;
        vertices[vertexCount] = new Vector3(vertexCount / 48, 1 - vertexCount % 2 * 2, 0);
        another[vertexCount] = new Vector4(p1[0], p1[1], p1[2], p1[3]);
        position[vertexCount] = new Vector4(p2[0], p2[1], p2[2], p2[3]);
        vertexCount++;
        vertices[vertexCount] = new Vector3(vertexCount / 48, 1 - vertexCount % 2 * 2, 0);
        another[vertexCount] = new Vector4(p1[0], p1[1], p1[2], p1[3]);
        position[vertexCount] = new Vector4(p2[0], p2[1], p2[2], p2[3]);
        vertexCount++;
    }
    void Execute()
    {
        vertexCount = 0;
        vertices = new Vector3[384];
        another = new Vector4[384];
        position = new Vector4[384];
        regF = new float[4][];
        for (int i = 0; i < 3; i++) regF[i] = new float[dim];
        for (int dir = 0; dir < 2 * dim; dir++)
        {
            int a = dir >> 1;
            float f2 = faceSize * 2;

            Zero(regF[0]);
            Apply(dir, regF[0], .5f);

            int[] d1 = new int[dim - 1];
            for (int i = 0; i < dim - 1; i++)
            {
                d1[i] = (a + i + 1) % dim;
                regF[0][d1[i]] = regF[0][d1[i]] - faceSize;
            }
            for (int i = 0; i < dim; i++)
            {
                if (i == a) continue;
                Copy(regF[1], regF[0]);
                Copy(regF[2], regF[1]);
                regF[2][i] = regF[2][i] + f2;
                AddLine(regF[1], regF[2]);
                int[] d2 = new int[dim - 2];
                bool h = false;
                for (int j = 0; j < dim - 2; j++)
                {
                    if (d1[j] == i) h = true;
                    d2[j] = h ? d1[j + 1] : d1[j];
                }
                for (int j = 0; j < (1 << (dim - 2)) - 1; j++)
                {
                    Apply(ForAxis(d2[drawer_a[j]], drawer_o[j]), regF[1], f2);
                    Apply(ForAxis(d2[drawer_a[j]], drawer_o[j]), regF[2], f2);
                    AddLine(regF[1], regF[2]);
                }
            }
        }
        //int[] triangles = new int[vertices.Length * 3 / 2];
        //for (int i = 0; i < triangles.Length; i += 6)
        //{
        //triangles[i] = i;
        //triangles[i + 1] = i+1;
        //triangles[i+2] = i+2;
        //triangles[i+3] = i+2;
        //triangles[i+4] = i+3;
        //triangles[i+5] = i+1;
        //}
        int[] triangles = new int[vertices.Length];
        for (int i = 0; i < triangles.Length; i++) triangles[i] = i;
        Debug.Log(vertices.Length);
        Mesh mesh = new Mesh();
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.SetIndices(triangles, MeshTopology.Quads, 0);
        mesh.SetUVs(0, position);
        mesh.SetUVs(1, another);
        mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 5);
        AssetDatabase.CreateAsset(mesh, "Assets/" + shapeName + ".asset");
    }

    void Reticle()
    {
        Mesh mesh = new Mesh();
        mesh.Clear();
        vertices = new Vector3[] { Vector3.right, Vector3.right, Vector3.left, Vector3.left, Vector3.up, Vector3.up, Vector3.down, Vector3.down, Vector3.forward, Vector3.forward, Vector3.back, Vector3.back };
        another = new Vector4[] { Vector3.left, Vector3.left, Vector3.right, Vector3.right, Vector3.down, Vector3.down, Vector3.up, Vector3.up, Vector3.back, Vector3.back, Vector3.forward, Vector3.forward };
        for (int i = 0; i < another.Length; i++) another[i].w = 1 - i % 2 * 2;
        mesh.vertices = vertices;
        int[] triangles = new int[vertices.Length];
        for (int i = 0; i < triangles.Length; i++) triangles[i] = i;
        mesh.SetIndices(triangles, MeshTopology.Quads, 0);
        mesh.SetUVs(0, another);
        mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 5);
        AssetDatabase.CreateAsset(mesh, "Assets/" + shapeName + ".asset");
    }

    void Reticle2D()
    {
        Mesh mesh = new Mesh();
        mesh.Clear();
        vertices = new Vector3[] { Vector3.right, Vector3.right, Vector3.left, Vector3.left, Vector3.up, Vector3.up, Vector3.down, Vector3.down };
        another = new Vector4[] { Vector3.left, Vector3.left, Vector3.right, Vector3.right, Vector3.down, Vector3.down, Vector3.up, Vector3.up };
        for (int i = 0; i < another.Length; i++) another[i].w = 1 - i % 2 * 2;
        mesh.vertices = vertices;
        int[] triangles = new int[vertices.Length];
        for (int i = 0; i < triangles.Length; i++) triangles[i] = i;
        mesh.SetIndices(triangles, MeshTopology.Quads, 0);
        mesh.SetUVs(0, another);
        mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 5);
        AssetDatabase.CreateAsset(mesh, "Assets/" + shapeName + ".asset");
    }

    void Reticle3D()
    {
        Mesh mesh = new Mesh();
        mesh.Clear();
        vertices = new Vector3[] { Vector3.forward, Vector3.forward, Vector3.back, Vector3.back };
        another = new Vector4[] { Vector3.back, Vector3.back, Vector3.forward, Vector3.forward };
        for (int i = 0; i < another.Length; i++) another[i].w = 1 - i % 2 * 2;
        mesh.vertices = vertices;
        int[] triangles = new int[vertices.Length];
        for (int i = 0; i < triangles.Length; i++) triangles[i] = i;
        mesh.SetIndices(triangles, MeshTopology.Quads, 0);
        mesh.SetUVs(0, another);
        mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 5);
        AssetDatabase.CreateAsset(mesh, "Assets/" + shapeName + ".asset");
    }

    [CustomEditor(typeof(MeshMaker4))]
    public class MeshMaker4Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Execute"))
            {
                MeshMaker4 targetScript = (MeshMaker4)target;
                targetScript.Execute();
            }

            if (GUILayout.Button("Reticle"))
            {
                MeshMaker4 targetScript = (MeshMaker4)target;
                targetScript.Reticle();
            }

            if (GUILayout.Button("Reticle2D"))
            {
                MeshMaker4 targetScript = (MeshMaker4)target;
                targetScript.Reticle2D();
            }

            if (GUILayout.Button("Reticle3D"))
            {
                MeshMaker4 targetScript = (MeshMaker4)target;
                targetScript.Reticle3D();
            }
        }
    }
}
#endif