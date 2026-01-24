using UnityEngine;
using UnityEditor;
using UdonSharpEditor;

namespace HachigayoLab.PolychoraViewer
{
    [CustomEditor(typeof(UniformScreen))]
    public class UniformScreenEditor : Editor
    {
        private string axisText, rotateText;

        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;

            UniformScreen myTarget = (UniformScreen)target;
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

            base.OnInspectorGUI();
        }

        private void ApplyTextToMatrix(string text, UniformScreen target, int mode)
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
    }
}