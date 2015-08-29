using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Devdog.InventorySystem.UI;

namespace Devdog.InventorySystem.Editors
{
    [CustomEditor(typeof(UIWindowInteractive), true)]
    public class UIWindowInteractiveEditor : UIWindowEditor
    {
        private SerializedProperty keyCombination;
        private UnityEditorInternal.ReorderableList keyCombinationList;
        private static UIWindowInteractive _tar;

        private static string keyCombinationNames
        {
            get
            {
                if (_tar == null)
                    return string.Empty;

                return JoinToString(_tar.keyCombination, ",");
            }
        }

        public static string JoinToString(KeyCode[] theList, string separator)
        {
            if (theList == null)
                return "";
            List<string> aListStrings = new List<string>();
            foreach (KeyCode keyCode in theList)
            {
                aListStrings.Add(keyCode.ToString());
            }
            return string.Join(separator, aListStrings.ToArray());
        }

        public override void OnEnable()
        {
            base.OnEnable();
            _tar = (UIWindowInteractive) target;

            keyCombination = serializedObject.FindProperty("keyCombination");

            keyCombinationList = new UnityEditorInternal.ReorderableList(serializedObject, keyCombination, false, true, true, true);
            keyCombinationList.drawHeaderCallback += rect => GUI.Label(rect, "Key combination(s)");
            keyCombinationList.drawElementCallback += (rect, index, active, focused) =>
            {
                rect.height = 16;
                rect.y += 2;

                EditorGUI.PropertyField(rect, keyCombination.GetArrayElementAtIndex(index));
            };
        }

        protected static new void DrawWindowName(Vector3 position, string name)
        {
            Handles.Label(position, name + "(" + keyCombinationNames + ")", "GUIEditor.BreadcrumbLeft");
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            
            EditorGUILayout.BeginVertical();
            keyCombinationList.DoLayoutList();
            EditorGUILayout.EndVertical();

            
            serializedObject.ApplyModifiedProperties();
        }
    }
}