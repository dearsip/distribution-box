using UnityEngine;
using UnityEditor;
using UdonSharpEditor;

namespace HachigayoLab.PolychoraViewer
{
    [CustomEditor(typeof(ThreeDimensionalScreen))]
    public class ThreeDimensionalScreenEditor : Editor
    {
        private string axisText, rotateText, colorsText;

        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;

            ThreeDimensionalScreen myTarget = (ThreeDimensionalScreen)target;
            EditorGUILayout.LabelField("Array Importer");

            axisText = EditorGUILayout.TextField("axis", axisText);
            if (GUILayout.Button("Import axis"))
            {
                ApplyTextToMatrix(axisText, myTarget, 0);
            }

            rotateText = EditorGUILayout.TextField("rotation", rotateText);
            if (GUILayout.Button("Import rotation"))
            {
                ApplyTextToMatrix(rotateText, myTarget, 1);
            }

            colorsText = EditorGUILayout.TextField("colors", colorsText);
            if (GUILayout.Button("Import colors"))
            {
                ApplyTextToColors(colorsText, myTarget);
            }

            base.OnInspectorGUI();
        }

        private void ApplyTextToMatrix(string text, ThreeDimensionalScreen target, int mode)
        {
            string[] values = text.Split(',');
            Matrix4x4 matrix = new Matrix4x4();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (float.TryParse(values[i*4+j], out float value))
                    {
                        matrix[i, j] = value;
                    }
                    else
                    {
                        Debug.LogError("Invalid input format for Matrix4x4.");
                        return;
                    }
                }
            }
            if (mode == 0) target.axis = matrix;
            else if (mode == 1) target.rotate = matrix;
            EditorUtility.SetDirty(target);
        }

        private void ApplyTextToColors(string text, ThreeDimensionalScreen target)
        {
            string[] values = text.Split(',');
            if (values.Length % 4 != 0) Debug.LogError("Invalid input format for Matrix4x4.");
            Color[] colors = new Color[values.Length / 4];
            for (int i = 0; i < values.Length / 4; i++)
            {
                if (float.TryParse(values[i*4], out float r) &&
                    float.TryParse(values[i*4+1], out float g) &&
                    float.TryParse(values[i*4+2], out float b) &&
                    float.TryParse(values[i*4+3], out float a))
                {
                    colors[i] = new Color(r, g, b, a);
                }
                else
                {
                    Debug.LogError("Invalid input format for Matrix4x4.");
                    return;
                }
            }
            target.cellColor = colors;
            EditorUtility.SetDirty(target);
        }
    }
}