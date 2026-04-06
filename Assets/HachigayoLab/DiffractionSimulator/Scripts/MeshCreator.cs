#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HachigayoLab.DiffractionSimulator
{
    public class MeshCreator : MonoBehaviour
    {
        [SerializeField] string shapeName;
        [SerializeField] int resolution = 2, scale = 7;
        [SerializeField] float radius = .5f, radius2 = 10;

        void Sphere()
        {
            Mesh mesh = new Mesh();
            mesh.name = shapeName;

            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<int> triangles = new List<int>();

            Vector3[] directions = {
                Vector3.up, Vector3.down,
                Vector3.left, Vector3.right,
                Vector3.forward, Vector3.back
            };

            foreach (Vector3 localUp in directions)
            {
                Vector3 axisA = new Vector3(localUp.y, localUp.z, localUp.x);
                Vector3 axisB = Vector3.Cross(localUp, axisA);

                int vertStartIndex = vertices.Count;

                for (int y = 0; y <= resolution; y++)
                {
                    for (int x = 0; x <= resolution; x++)
                    {
                        Vector2 percent = new Vector2(x, y) / resolution;
                        Vector3 pointOnCube =
                            localUp +
                            (percent.x - 0.5f) * 2f * axisA +
                            (percent.y - 0.5f) * 2f * axisB;

                        Vector3 pointOnSphere = pointOnCube.normalized;
                        normals.Add(pointOnSphere);
                        vertices.Add(pointOnSphere * radius);
                    }
                }

                for (int y = 0; y < resolution; y++)
                {
                    for (int x = 0; x < resolution; x++)
                    {
                        int i = vertStartIndex + x + y * (resolution + 1);

                        triangles.Add(i);
                        triangles.Add(i + resolution + 2);
                        triangles.Add(i + resolution + 1);

                        triangles.Add(i);
                        triangles.Add(i + 1);
                        triangles.Add(i + resolution + 2);
                    }
                }
            }

            List<Vector3> allVertices = new List<Vector3>();
            List<Vector3> allNormals = new List<Vector3>();
            List<int> allTriangles = new List<int>();
            List<Vector3> shifts = new List<Vector3>();
            int count = 0;
            for (int i = 0; i < scale * scale * scale; i++)
            {
                for (int j = 0; j < vertices.Count; j++)
                {
                    allVertices.Add(vertices[j]);
                    allNormals.Add(normals[j]);
                    shifts.Add(new Vector4(
                        i % scale - (scale - 1) / 2,
                        i / scale % scale - (scale - 1) / 2,
                        i / scale / scale - (scale - 1) / 2,
                        0));
                }
                for (int j = 0; j < triangles.Count; j++) allTriangles.Add(triangles[j] + count);
                count += vertices.Count;
            }
            mesh.SetVertices(allVertices);
            mesh.SetTriangles(allTriangles, 0);
            mesh.SetNormals(allNormals);
            mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);
            mesh.SetUVs(0, shifts);

            AssetDatabase.CreateAsset(mesh, "Assets/" + shapeName + ".asset");
        }

        void Tube()
        {
            Mesh mesh = new Mesh();
            mesh.name = shapeName;

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            float t = 2 * Mathf.PI / resolution;
            for (int i = 0; i < resolution; i++)
            {
                Vector3 v = new Vector3(Mathf.Cos(t * i), Mathf.Sin(t * i), 0) * radius;
                vertices.Add(v);
                v.z = radius;
                vertices.Add(v);
            }

            for (int i = 0; i < resolution; i++)
            {
                Vector3 v = new Vector3(Mathf.Cos(t * i * .5f) * radius2, Mathf.Sin(t * i * .5f) * radius2, 1) * radius;
                vertices.Add(v);
                v = new Vector3(Mathf.Cos(t * (2 * resolution - 1 - i) * .5f) * radius2, Mathf.Sin(t * (2 * resolution - 1 - i) * .5f) * radius2, 1) * radius;
                vertices.Add(v);
            }

            for (int i = 0; i < resolution - 1; i++)
            {
                triangles.Add(i * 2);
                triangles.Add(i * 2 + 3);
                triangles.Add(i * 2 + 1);

                triangles.Add(i * 2);
                triangles.Add(i * 2 + 2);
                triangles.Add(i * 2 + 3);
            }
            triangles.Add(resolution * 2 - 2);
            triangles.Add(1);
            triangles.Add(resolution * 2 - 1);

            triangles.Add(resolution * 2 - 2);
            triangles.Add(0);
            triangles.Add(1);

            for (int i = 0; i < resolution - 1; i++)
            {
                triangles.Add(resolution * 2 + i * 2);
                triangles.Add(resolution * 2 + i * 2 + 1);
                triangles.Add(resolution * 2 + i * 2 + 3);

                triangles.Add(resolution * 2 + i * 2);
                triangles.Add(resolution * 2 + i * 2 + 3);
                triangles.Add(resolution * 2 + i * 2 + 2);
            }

            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            List<Vector3> allVertices = new List<Vector3>();
            List<int> allTriangles = new List<int>();
            List<Vector3> shifts = new List<Vector3>();
            int count = 0;
            for (int i = 0; i < scale * scale * scale; i++)
            {
                for (int j = 0; j < vertices.Count; j++)
                {
                    allVertices.Add(vertices[j]);
                    shifts.Add(new Vector4(
                        i % scale - (scale - 1) / 2,
                        i / scale % scale - (scale - 1) / 2,
                        i / scale / scale - (scale - 1) / 2,
                        0));
                }
                for (int j = 0; j < triangles.Count; j++) allTriangles.Add(triangles[j] + count);
                count += vertices.Count;
            }
            mesh.SetVertices(allVertices);
            mesh.SetTriangles(allTriangles, 0);
            mesh.RecalculateNormals();
            mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);
            mesh.SetUVs(0, shifts);

            AssetDatabase.CreateAsset(mesh, "Assets/" + shapeName + ".asset");
        }

        void OneTube()
        {
            Mesh mesh = new Mesh();
            mesh.name = shapeName;

            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> triangles = new List<int>();

            float t = 2 * Mathf.PI / resolution;
            for (int i = 0; i < resolution; i++)
            {
                Vector3 v = new Vector3(Mathf.Cos(t * i), Mathf.Sin(t * i), 0) * radius;
                vertices.Add(v);
                uvs.Add(new Vector2(i % 2, 0));
                v.z = radius;
                vertices.Add(v);
                uvs.Add(new Vector2(i % 2, 1));
            }

            for (int i = 0; i < resolution - 1; i++)
            {
                triangles.Add(i * 2);
                triangles.Add(i * 2 + 3);
                triangles.Add(i * 2 + 1);

                triangles.Add(i * 2);
                triangles.Add(i * 2 + 2);
                triangles.Add(i * 2 + 3);
            }
            triangles.Add(resolution * 2 - 2);
            triangles.Add(1);
            triangles.Add(resolution * 2 - 1);

            triangles.Add(resolution * 2 - 2);
            triangles.Add(0);
            triangles.Add(1);

            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.SetUVs(0, uvs);
            mesh.RecalculateNormals();
            mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 1000);

            AssetDatabase.CreateAsset(mesh, "Assets/" + shapeName + ".asset");
        }

        [CustomEditor(typeof(MeshCreator))]
        public class MeshCreatorEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();

                // shapeName = "Crystal", resolution = 3, scale = 7, radius = 1
                if (GUILayout.Button("Sphere"))
                {
                    MeshCreator targetScript = (MeshCreator)target;
                    targetScript.Sphere();
                }

                // shapeName = "Ray", resolution = 8, scale = 7, radius = 0.5f, radius2 = 10
                if (GUILayout.Button("Tube"))
                {
                    MeshCreator targetScript = (MeshCreator)target;
                    targetScript.Tube();
                }

                // shapeName = "Tube", resolution = 16, radius = 0.5f
                if (GUILayout.Button("OneTube"))
                {
                    MeshCreator targetScript = (MeshCreator)target;
                    targetScript.OneTube();
                }
            }
        }
    }
}
#endif