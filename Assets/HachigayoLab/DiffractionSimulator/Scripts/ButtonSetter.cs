#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using VRC.Udon;
using TMPro;
using UnityEditor.Events;
using UnityEngine.Events;
using System.Reflection;

namespace HachigayoLab.DiffractionSimulator
{
    public class ButtonSetter : MonoBehaviour
    {
        [SerializeField] UdonBehaviour target;
        void Event()
        {
            string[] events = new string[]
            {
                "M100",
                "M110",
                "M111",
                "M200",
                "M220",
                "M311",
                "M400",
                "M331",
                "M422",
                "M333",
                "M511",
                "M440",
                "M531",
                "M620",
                "M533",
                "M444"
            };
            var method = target.GetType().GetMethod(
                "SendCustomEvent",
                BindingFlags.Instance | BindingFlags.Public,
                null,
                new[] { typeof(string) },
                null
            );
            Button[] buttons = GetComponentsInChildren<Button>();
            int count = Mathf.Min(buttons.Length, events.Length);
            Debug.Log(buttons.Length + ", " + events.Length);
            for (int i = 0; i < count; i++)
            {
                UnityEvent e = buttons[i].onClick;
                int c = e.GetPersistentEventCount();
                Debug.Log(i + ", " + c);
                for (int j = 0; j < c; j++) UnityEventTools.RemovePersistentListener(e, 0);
                buttons[i].onClick.RemoveAllListeners();
                UnityEventTools.AddStringPersistentListener(e,
                    (UnityAction<string>)System.Delegate.CreateDelegate(typeof(UnityAction<string>), target, method),
                    events[i]
                );
            }
        }

        void Label()
        {
            string[] labels = new string[]
            {
                "<voffset=0.05em>(</voffset>1 0 0<voffset=0.05em>)</voffset>",
                "<voffset=0.05em>(</voffset>1 1 0<voffset=0.05em>)</voffset>",
                "<voffset=0.05em>(</voffset>1 1 1<voffset=0.05em>)</voffset>",
                "<voffset=0.05em>(</voffset>2 0 0<voffset=0.05em>)</voffset>",
                "<voffset=0.05em>(</voffset>2 2 0<voffset=0.05em>)</voffset>",
                "<voffset=0.05em>(</voffset>3 1 1<voffset=0.05em>)</voffset>",
                "<voffset=0.05em>(</voffset>4 0 0<voffset=0.05em>)</voffset>",
                "<voffset=0.05em>(</voffset>3 3 1<voffset=0.05em>)</voffset>",
                "<voffset=0.05em>(</voffset>4 2 2<voffset=0.05em>)</voffset>",
                "<voffset=0.05em>(</voffset>3 3 3<voffset=0.05em>)</voffset>",
                "<voffset=0.05em>(</voffset>5 1 1<voffset=0.05em>)</voffset>",
                "<voffset=0.05em>(</voffset>4 4 0<voffset=0.05em>)</voffset>",
                "<voffset=0.05em>(</voffset>5 3 1<voffset=0.05em>)</voffset>",
                "<voffset=0.05em>(</voffset>6 2 0<voffset=0.05em>)</voffset>",
                "<voffset=0.05em>(</voffset>5 3 3<voffset=0.05em>)</voffset>",
                "<voffset=0.05em>(</voffset>4 4 4<voffset=0.05em>)</voffset>",
            };
            TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();
            int count = Mathf.Min(texts.Length, labels.Length);
            for (int i = 0; i < count; i++) texts[i].text = labels[i];
        }

        [CustomEditor(typeof(ButtonSetter))]
        public class ButtonSetterEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();

                if (GUILayout.Button("Event"))
                {
                    ButtonSetter targetScript = (ButtonSetter)target;
                    targetScript.Event();
                }

                if (GUILayout.Button("Label"))
                {
                    ButtonSetter targetScript = (ButtonSetter)target;
                    targetScript.Label();
                }
            }
        }
    }
}
#endif