using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Reflection;
using TMPro;

namespace HachigayoLab.UISync
{
    [InitializeOnLoad]
    public static class UISyncAutoBinder
    {
        const string MenuPath = "Tools/UI Sync Auto Binder";

        static bool Enabled
        {
            get => EditorPrefs.GetBool(MenuPath, true);
            set => EditorPrefs.SetBool(MenuPath, value);
        }

        static UISyncAutoBinder()
        {
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            EditorApplication.delayCall += UpdateMenuCheck;
        }

        [MenuItem(MenuPath)]
        static void Toggle()
        {
            Enabled = !Enabled;
            UpdateMenuCheck();

            Debug.Log($"[UISyncAutoBinder] Enabled = {Enabled}");
        }

        [MenuItem(MenuPath, true)]
        static bool ToggleValidate()
        {
            Menu.SetChecked(MenuPath, Enabled);
            return true;
        }

        static void UpdateMenuCheck()
        {
            Menu.SetChecked(MenuPath, Enabled);
        }

        static void OnHierarchyChanged()
        {
            if (!Enabled) return;

            var gameObject = Selection.activeGameObject;
            if (gameObject == null) return;

            var slider = gameObject.GetComponent<Slider>();
            if (slider != null) { SliderAutoBinder(gameObject, slider); return; }

            var toggle = gameObject.GetComponent<Toggle>();
            if (toggle != null) { ToggleAutoBinder(gameObject, toggle); return; }

            var dropdown = gameObject.GetComponent<TMP_Dropdown>();
            if (dropdown != null) { DropdownAutoBinder(gameObject, dropdown); return; }

            var inputField = gameObject.GetComponent<TMP_InputField>();
            if (inputField != null) { InputFieldAutoBinder(gameObject, inputField); return; }
        }

        static void SliderAutoBinder(GameObject gameObject, Slider slider)
        {
            foreach (var monoBehaviour in gameObject.GetComponents<MonoBehaviour>())
            {
                if (monoBehaviour == null) continue;
                if (monoBehaviour.GetType().Name != "UdonBehaviour") continue;

                var method = monoBehaviour.GetType().GetMethod(
                    "SendCustomEvent",
                    BindingFlags.Instance | BindingFlags.Public,
                    null,
                    new[] { typeof(string) },
                    null
                );
                if (method == null) continue;

                if (HasPersistentListener(slider, monoBehaviour, method))
                    continue;

                Undo.RecordObject(slider, "Auto Bind Value Changed Event");

                UnityEventTools.AddStringPersistentListener(
                    slider.onValueChanged,
                    (UnityAction<string>)System.Delegate.CreateDelegate(
                        typeof(UnityAction<string>),
                        monoBehaviour,
                        method
                    ),
                    "OnValueChanged"
                );

                EditorUtility.SetDirty(slider);

                break;
            }
        }

        static void ToggleAutoBinder(GameObject gameObject, Toggle toggle)
        {
            foreach (var monoBehaviour in gameObject.GetComponents<MonoBehaviour>())
            {
                if (monoBehaviour == null) continue;
                if (monoBehaviour.GetType().Name != "UdonBehaviour") continue;

                var method = monoBehaviour.GetType().GetMethod(
                    "SendCustomEvent",
                    BindingFlags.Instance | BindingFlags.Public,
                    null,
                    new[] { typeof(string) },
                    null
                );
                if (method == null) continue;

                if (HasPersistentListener(toggle, monoBehaviour, method))
                    continue;

                Undo.RecordObject(toggle, "Auto Bind Value Changed Event");

                UnityEventTools.AddStringPersistentListener(
                    toggle.onValueChanged,
                    (UnityAction<string>)System.Delegate.CreateDelegate(
                        typeof(UnityAction<string>),
                        monoBehaviour,
                        method
                    ),
                    "OnValueChanged"
                );

                EditorUtility.SetDirty(toggle);

                break;
            }
        }

        static void DropdownAutoBinder(GameObject gameObject, TMP_Dropdown dropdown)
        {
            foreach (var monoBehaviour in gameObject.GetComponents<MonoBehaviour>())
            {
                if (monoBehaviour == null) continue;
                if (monoBehaviour.GetType().Name != "UdonBehaviour") continue;

                var method = monoBehaviour.GetType().GetMethod(
                    "SendCustomEvent",
                    BindingFlags.Instance | BindingFlags.Public,
                    null,
                    new[] { typeof(string) },
                    null
                );
                if (method == null) continue;

                if (HasPersistentListener(dropdown, monoBehaviour, method))
                    continue;

                Undo.RecordObject(dropdown, "Auto Bind Value Changed Event");

                UnityEventTools.AddStringPersistentListener(
                    dropdown.onValueChanged,
                    (UnityAction<string>)System.Delegate.CreateDelegate(
                        typeof(UnityAction<string>),
                        monoBehaviour,
                        method
                    ),
                    "OnValueChanged"
                );

                EditorUtility.SetDirty(dropdown);

                break;
            }
        }

        static void InputFieldAutoBinder(GameObject gameObject, TMP_InputField inputField)
        {
            foreach (var monoBehaviour in gameObject.GetComponents<MonoBehaviour>())
            {
                if (monoBehaviour == null) continue;
                if (monoBehaviour.GetType().Name != "UdonBehaviour") continue;

                var method = monoBehaviour.GetType().GetMethod(
                    "SendCustomEvent",
                    BindingFlags.Instance | BindingFlags.Public,
                    null,
                    new[] { typeof(string) },
                    null
                );
                if (method == null) continue;

                if (HasPersistentListener(inputField, monoBehaviour, method))
                    continue;

                Undo.RecordObject(inputField, "Auto Bind Value Changed Event");

                UnityEventTools.AddStringPersistentListener(
                    inputField.onValueChanged,
                    (UnityAction<string>)System.Delegate.CreateDelegate(
                        typeof(UnityAction<string>),
                        monoBehaviour,
                        method
                    ),
                    "OnValueChanged"
                );

                EditorUtility.SetDirty(inputField);

                break;
            }
        }

        static bool HasPersistentListener(Slider slider, Object target, MethodInfo method)
        {
            var evt = slider.onValueChanged;
            int count = evt.GetPersistentEventCount();

            for (int i = 0; i < count; i++)
            {
                if (evt.GetPersistentTarget(i) == target &&
                    evt.GetPersistentMethodName(i) == method.Name)
                {
                    return true;
                }
            }
            return false;
        }

        static bool HasPersistentListener(Toggle toggle, Object target, MethodInfo method)
        {
            var evt = toggle.onValueChanged;
            int count = evt.GetPersistentEventCount();

            for (int i = 0; i < count; i++)
            {
                if (evt.GetPersistentTarget(i) == target &&
                    evt.GetPersistentMethodName(i) == method.Name)
                {
                    return true;
                }
            }
            return false;
        }

        static bool HasPersistentListener(TMP_Dropdown dropdown, Object target, MethodInfo method)
        {
            var evt = dropdown.onValueChanged;
            int count = evt.GetPersistentEventCount();

            for (int i = 0; i < count; i++)
            {
                if (evt.GetPersistentTarget(i) == target &&
                    evt.GetPersistentMethodName(i) == method.Name)
                {
                    return true;
                }
            }
            return false;
        }

        static bool HasPersistentListener(TMP_InputField inputField, Object target, MethodInfo method)
        {
            var evt = inputField.onValueChanged;
            int count = evt.GetPersistentEventCount();

            for (int i = 0; i < count; i++)
            {
                if (evt.GetPersistentTarget(i) == target &&
                    evt.GetPersistentMethodName(i) == method.Name)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
