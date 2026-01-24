#if UNITY_EDITOR
using UnityEngine;
using System.IO;
using UnityEditor;

namespace HachigayoLab.PolychoralAccessory
{
    public class TexturedMeshCreator : MonoBehaviour
    {
        [SerializeField] Mesh[] meshes;
        [SerializeField] float edgeSize = 0.1f;
        [SerializeField] bool symmetryRotation = false;
        [SerializeField] Texture2D[] textures;
        [SerializeField] int eachTextureSize = 256;
        [SerializeField] Color[] edgeColors;

        void CreateMesh()
        {
            Matrix4x4[] cellRotations = symmetryRotation ? cellRotationsSymmetry : cellRotationsForword;

            Vector4[] vertex=new Vector4[]{new Vector4(1f,1f,1f,1f),new Vector4(1f,1f,-1f,1f),new Vector4(1f,-1f,-1f,1f),new Vector4(1f,-1f,1f,1f),new Vector4(-1f,-1f,1f,1f),new Vector4(-1f,-1f,-1f,1f),new Vector4(-1f,1f,-1f,1f),new Vector4(-1f,1f,1f,1f),new Vector4(1f,1f,1f,-1f),new Vector4(1f,1f,-1f,-1f),new Vector4(1f,-1f,-1f,-1f),new Vector4(1f,-1f,1f,-1f),new Vector4(-1f,-1f,1f,-1f),new Vector4(-1f,-1f,-1f,-1f),new Vector4(-1f,1f,-1f,-1f),new Vector4(-1f,1f,1f,-1f),};
            int[][] cellVertex=new int[][]{new int[]{0,1,2,3,6,7,5,4,},new int[]{8,9,10,11,14,15,13,12,},new int[]{0,1,2,3,9,8,10,11,},new int[]{4,5,6,7,13,12,14,15,},new int[]{0,1,6,7,9,8,15,14,},new int[]{2,3,4,5,11,10,13,12,},new int[]{3,0,7,4,8,11,15,12,},new int[]{1,2,5,6,10,9,14,13,},};
            int[][][] cellEdgeIndex=new int[][][]{new int[][]{new int[]{0,1},new int[]{1,2},new int[]{2,3},new int[]{3,0},new int[]{0,5},new int[]{1,4},new int[]{2,6},new int[]{3,7},new int[]{7,6},new int[]{6,4},new int[]{4,5},new int[]{5,7},},new int[][]{new int[]{0,1},new int[]{1,2},new int[]{2,3},new int[]{3,0},new int[]{0,5},new int[]{1,4},new int[]{2,6},new int[]{3,7},new int[]{7,6},new int[]{6,4},new int[]{4,5},new int[]{5,7},},new int[][]{new int[]{0,1},new int[]{1,2},new int[]{2,3},new int[]{3,0},new int[]{0,5},new int[]{1,4},new int[]{2,6},new int[]{3,7},new int[]{5,4},new int[]{4,6},new int[]{6,7},new int[]{7,5},},new int[][]{new int[]{0,1},new int[]{1,2},new int[]{2,3},new int[]{3,0},new int[]{0,5},new int[]{1,4},new int[]{2,6},new int[]{3,7},new int[]{5,4},new int[]{4,6},new int[]{6,7},new int[]{7,5},},new int[][]{new int[]{0,1},new int[]{0,3},new int[]{1,2},new int[]{2,3},new int[]{0,5},new int[]{1,4},new int[]{2,7},new int[]{3,6},new int[]{5,4},new int[]{5,6},new int[]{4,7},new int[]{7,6},},new int[][]{new int[]{0,1},new int[]{0,3},new int[]{1,2},new int[]{2,3},new int[]{0,5},new int[]{1,4},new int[]{2,7},new int[]{3,6},new int[]{5,4},new int[]{5,6},new int[]{4,7},new int[]{7,6},},new int[][]{new int[]{0,1},new int[]{1,2},new int[]{0,3},new int[]{2,3},new int[]{1,4},new int[]{0,5},new int[]{3,7},new int[]{2,6},new int[]{5,4},new int[]{4,6},new int[]{5,7},new int[]{6,7},},new int[][]{new int[]{0,1},new int[]{0,3},new int[]{1,2},new int[]{2,3},new int[]{0,5},new int[]{1,4},new int[]{2,7},new int[]{3,6},new int[]{5,4},new int[]{5,6},new int[]{4,7},new int[]{7,6},},};
            int[][][] cellFaceIndex=new int[][][]{new int[][]{new int[]{0,1,2,3,},new int[]{0,1,4,5,},new int[]{1,2,6,4,},new int[]{2,3,7,6,},new int[]{3,0,5,7,},new int[]{7,6,4,5,},},new int[][]{new int[]{0,1,2,3,},new int[]{0,1,4,5,},new int[]{1,2,6,4,},new int[]{2,3,7,6,},new int[]{3,0,5,7,},new int[]{7,6,4,5,},},new int[][]{new int[]{0,1,2,3,},new int[]{0,1,4,5,},new int[]{1,2,6,4,},new int[]{2,3,7,6,},new int[]{3,0,5,7,},new int[]{5,4,6,7,},},new int[][]{new int[]{0,1,2,3,},new int[]{0,1,4,5,},new int[]{1,2,6,4,},new int[]{2,3,7,6,},new int[]{3,0,5,7,},new int[]{5,4,6,7,},},new int[][]{new int[]{0,1,2,3,},new int[]{0,1,4,5,},new int[]{0,3,6,5,},new int[]{1,2,7,4,},new int[]{2,3,6,7,},new int[]{5,4,7,6,},},new int[][]{new int[]{0,1,2,3,},new int[]{0,1,4,5,},new int[]{0,3,6,5,},new int[]{1,2,7,4,},new int[]{2,3,6,7,},new int[]{5,4,7,6,},},new int[][]{new int[]{0,1,2,3,},new int[]{0,1,4,5,},new int[]{1,2,6,4,},new int[]{0,3,7,5,},new int[]{2,3,7,6,},new int[]{5,4,6,7,},},new int[][]{new int[]{0,1,2,3,},new int[]{0,1,4,5,},new int[]{0,3,6,5,},new int[]{1,2,7,4,},new int[]{2,3,6,7,},new int[]{5,4,7,6,},},};
            Vector4[] cellNormal=new Vector4[]{new Vector4(0f,0f,0f,1f),new Vector4(0f,0f,0f,-1f),new Vector4(1f,0f,0f,0f),new Vector4(-1f,0f,0f,0f),new Vector4(0f,1f,0f,0f),new Vector4(0f,-1f,0f,0f),new Vector4(0f,0f,1f,0f),new Vector4(0f,0f,-1f,0f),};
            Vector4[] cellCenter=new Vector4[]{new Vector4(0f,0f,0f,1f),new Vector4(0f,0f,0f,-1f),new Vector4(1f,0f,0f,0f),new Vector4(-1f,0f,0f,0f),new Vector4(0f,1f,0f,0f),new Vector4(0f,-1f,0f,0f),new Vector4(0f,0f,1f,0f),new Vector4(0f,0f,-1f,0f),};
            float[] cellThreshold=new float[]{1f,1f,1f,1f,1f,1f,1f,1f,};
            int cellCount = cellVertex.Length;

            Vector4[] vertices;
            int vertexCount;
            int[] polygonTriangles;
            int polygonTriangleCount;
            Vector2[] thresholds, uvs;
            Vector4[] normals;
            int indexCount;

            Mesh mesh = new Mesh();
            mesh.Clear();

            int n = 0;
            for (int i = 0; i < cellCount; i++) n += cellVertex[i].Length;
            n *= 2;
            for (int i = 0; i < cellCount; i++) n += meshes[i].vertexCount;
            n += 8;
            vertices = new Vector4[n];
            thresholds = new Vector2[n];
            uvs = new Vector2[n];
            normals = new Vector4[n];
            n = 0;
            for (int i = 0; i < cellCount; i++)
            {
                n += cellEdgeIndex[i].Length * 6;
                n += meshes[i].triangles.Length;
            }
            polygonTriangles = new int[n];

            vertexCount = 0;
            Vector3 t;
            Vector4 c;
            for (int i = 0; i < cellCount; i++)
            {
                Vector2 uvOrigin = new Vector2(i % 4 * .25f, i / 4 * .25f);

                for (int j = 0; j < cellVertex[i].Length; j++)
                {
                    c = cellCenter[i] * edgeSize + vertex[cellVertex[i][j]] * (1 - edgeSize);
                    vertices[vertexCount] = c;
                    thresholds[vertexCount] = new Vector2(cellThreshold[i], 0);
                    uvs[vertexCount] = uvOrigin + new Vector2(.125f, .625f);
                    normals[vertexCount++] = cellNormal[i];
                }
                for (int j = 0; j < cellVertex[i].Length; j++)
                {
                    c = vertex[cellVertex[i][j]];
                    vertices[vertexCount] = c;
                    thresholds[vertexCount] = new Vector2(cellThreshold[i], 0);
                    uvs[vertexCount] = uvOrigin + new Vector2(.125f, .625f);
                    normals[vertexCount++] = cellNormal[i];
                }

                Vector3 min = Vector3.one * 100;
                Vector3 max = Vector3.one * -100;
                for (int j = 0; j < meshes[i].vertexCount; j++)
                {
                    t = meshes[i].vertices[j];
                    min = Vector3.Min(min, t);
                    max = Vector3.Max(max, t);
                }
                float s = 1.6f / Mathf.Max(max.x - min.x, max.y - min.y, max.z - min.z);
                Vector3 center = (min + max) / 2;
                for (int j = 0; j < meshes[i].vertexCount; j++)
                {
                    t = meshes[i].vertices[j];
                    c = cellRotations[i] * new Vector4((t.x - center.x) * s, (t.y - center.y) * s, (t.z - center.z) * s, 1);
                    vertices[vertexCount] = c;
                    thresholds[vertexCount] = new Vector2(cellThreshold[i], 0);
                    uvs[vertexCount] = uvOrigin + meshes[i].uv[j] * .25f;
                    normals[vertexCount++] = cellNormal[i];
                }
            }

            polygonTriangleCount = 0;
            indexCount = 0;
            for (int i = 0; i < cellCount; i++)
            {
                for (int j = 0; j < cellEdgeIndex[i].Length; j++)
                {
                    polygonTriangles[polygonTriangleCount++] = indexCount + cellEdgeIndex[i][j][0];
                    polygonTriangles[polygonTriangleCount++] = indexCount + cellEdgeIndex[i][j][1];
                    polygonTriangles[polygonTriangleCount++] = indexCount + cellVertex[i].Length + cellEdgeIndex[i][j][1];
                    polygonTriangles[polygonTriangleCount++] = indexCount + cellEdgeIndex[i][j][0];
                    polygonTriangles[polygonTriangleCount++] = indexCount + cellVertex[i].Length + cellEdgeIndex[i][j][1];
                    polygonTriangles[polygonTriangleCount++] = indexCount + cellVertex[i].Length + cellEdgeIndex[i][j][0];
                }
                indexCount += cellVertex[i].Length * 2;
                for (int j = 0; j < meshes[i].triangles.Length; j++)
                {
                    polygonTriangles[polygonTriangleCount++] = indexCount + meshes[i].triangles[j];
                }
                indexCount += meshes[i].vertexCount;
            }

            Vector3[] v = new Vector3[vertexCount+8];
            for (int i = 0; i < 8; i++) v[vertexCount+i] = new Vector3(i/4, i%4/2, i%2) * 2 - Vector3.one;
            mesh.SetVertices(v);
            mesh.SetUVs(0, vertices);
            mesh.SetUVs(1, thresholds);
            mesh.SetUVs(2, normals);
            mesh.SetUVs(3, uvs);
            mesh.SetTriangles(polygonTriangles, 0);

            BoneWeight[] boneWeights = new BoneWeight[vertexCount+8];
            for (int i = 0; i < vertexCount; i++)
            {
                boneWeights[i].boneIndex0 = 1;
                boneWeights[i].weight0 = 1;
            }
            for (int i = 0; i < 8; i++)
            {
                boneWeights[vertexCount+i].boneIndex0 = 0;
                boneWeights[vertexCount+i].weight0 = 1;
            }
            mesh.boneWeights = boneWeights;
            Matrix4x4[] bindposes = new Matrix4x4[2];
            bindposes[0] = Matrix4x4.identity;
            bindposes[1] = Matrix4x4.identity;
            mesh.bindposes = bindposes;
            mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 5);

            AssetDatabase.CreateAsset(mesh, "Assets/TexturedHypercube.asset");
        }

        void CreateTexture()
        {
            int col = 4;
            int row = 2;
            Texture2D tex = new Texture2D(eachTextureSize * col, eachTextureSize * row * 2);
            for (int i = 0; i < row; i++)
                for (int j = 0; j < col; j++)
                {
                    Texture2D cellTex = textures[i * col + j];
                    for (int k = 0; k < eachTextureSize; k++)
                        for (int l = 0; l < eachTextureSize; l++)
                            tex.SetPixel(eachTextureSize * j + l, eachTextureSize * i + k, cellTex.GetPixel(l, k));
                }

            for (int i = 0; i < row; i++)
                for (int j = 0; j < col; j++)
                {
                    int colorindex = i * col + j;
                    for (int k = 0; k < eachTextureSize; k++)
                        for (int l = 0; l < eachTextureSize; l++)
                            tex.SetPixel(eachTextureSize * j + l, eachTextureSize * (row + i) + k, edgeColors[colorindex]);
                }

            File.WriteAllBytes("Assets/TexturedHypercube.png", tex.EncodeToPNG());
            AssetDatabase.Refresh();
        }

        Matrix4x4[] cellRotationsForword = new Matrix4x4[] {
            new Matrix4x4(
                new Vector4( 1, 0, 0, 0),
                new Vector4( 0, 1, 0, 0),
                new Vector4( 0, 0, 1, 0),
                new Vector4( 0, 0, 0, 1)),
            new Matrix4x4(
                new Vector4(-1, 0, 0, 0),
                new Vector4( 0, 1, 0, 0),
                new Vector4( 0, 0, 1, 0),
                new Vector4( 0, 0, 0,-1)),
            new Matrix4x4(
                new Vector4( 0, 0, 0,-1),
                new Vector4( 0, 1, 0, 0),
                new Vector4( 0, 0, 1, 0),
                new Vector4( 1, 0, 0, 0)),
            new Matrix4x4(
                new Vector4( 0, 0, 0, 1),
                new Vector4( 0, 1, 0, 0),
                new Vector4( 0, 0, 1, 0),
                new Vector4(-1, 0, 0, 0)),
            new Matrix4x4(
                new Vector4( 1, 0, 0, 0),
                new Vector4( 0, 0, 0,-1),
                new Vector4( 0, 0, 1, 0),
                new Vector4( 0, 1, 0, 0)),
            new Matrix4x4(
                new Vector4( 1, 0, 0, 0),
                new Vector4( 0, 0, 0, 1),
                new Vector4( 0, 0, 1, 0),
                new Vector4( 0,-1, 0, 0)),
            new Matrix4x4(
                new Vector4( 1, 0, 0, 0),
                new Vector4( 0, 1, 0, 0),
                new Vector4( 0, 0, 0,-1),
                new Vector4( 0, 0, 1, 0)),
            new Matrix4x4(
                new Vector4( 1, 0, 0, 0),
                new Vector4( 0, 1, 0, 0),
                new Vector4( 0, 0, 0, 1),
                new Vector4( 0, 0,-1, 0)),
        };
        Matrix4x4[] cellRotationsSymmetry = new Matrix4x4[] {
            new Matrix4x4(
                new Vector4( 1, 0, 0, 0),
                new Vector4( 0, 1, 0, 0),
                new Vector4( 0, 0, 1, 0),
                new Vector4( 0, 0, 0, 1)),
            new Matrix4x4(
                new Vector4(-1, 0, 0, 0),
                new Vector4( 0,-1, 0, 0),
                new Vector4( 0, 0,-1, 0),
                new Vector4( 0, 0, 0,-1)),
            new Matrix4x4(
                new Vector4( 0, 0, 0,-1),
                new Vector4( 0, 0,-1, 0),
                new Vector4( 0, 1, 0, 0),
                new Vector4( 1, 0, 0, 0)),
            new Matrix4x4(
                new Vector4( 0, 0, 0, 1),
                new Vector4( 0, 0, 1, 0),
                new Vector4( 0,-1, 0, 0),
                new Vector4(-1, 0, 0, 0)),
            new Matrix4x4(
                new Vector4( 0, 0, 1, 0),
                new Vector4( 0, 0, 0,-1),
                new Vector4(-1, 0, 0, 0),
                new Vector4( 0, 1, 0, 0)),
            new Matrix4x4(
                new Vector4( 0, 0,-1, 0),
                new Vector4( 0, 0, 0, 1),
                new Vector4( 1, 0, 0, 0),
                new Vector4( 0,-1, 0, 0)),
            new Matrix4x4(
                new Vector4( 0,-1, 0, 0),
                new Vector4( 1, 0, 0, 0),
                new Vector4( 0, 0, 0,-1),
                new Vector4( 0, 0, 1, 0)),
            new Matrix4x4(
                new Vector4( 0, 1, 0, 0),
                new Vector4(-1, 0, 0, 0),
                new Vector4( 0, 0, 0, 1),
                new Vector4( 0, 0,-1, 0)),
        };

        [CustomEditor(typeof(TexturedMeshCreator))]
        public class TexturedMeshCreatorEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();
                TexturedMeshCreator script = target as TexturedMeshCreator;
                if (GUILayout.Button("Create Mesh"))
                {
                    script.CreateMesh();
                }
                if (GUILayout.Button("Create Texture"))
                {
                    script.CreateTexture();
                }
            }
        }
    }
}
#endif