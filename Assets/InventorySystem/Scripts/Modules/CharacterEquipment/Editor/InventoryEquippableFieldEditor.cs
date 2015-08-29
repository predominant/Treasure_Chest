using UnityEngine;
using UnityEditor;
using System.Collections;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem.Editors
{
    [CustomEditor(typeof(InventoryEquippableField))]
    public class InventoryEquippableFieldEditor : Editor
    {
        private SerializedProperty equipTypes;

        private UnityEditorInternal.ReorderableList list;

        public void OnEnable()
        {
            equipTypes = serializedObject.FindProperty("_equipTypes");

            list = new UnityEditorInternal.ReorderableList(serializedObject, equipTypes, true, true, true, true);
            list.drawHeaderCallback += rect => EditorGUI.LabelField(rect, "Which types can be placed in this field?");
            list.drawElementCallback += (rect, index, active, focused) =>
            {
                rect.height = 16;
                rect.y += 3;

                var i = equipTypes.GetArrayElementAtIndex(index);
                i.intValue = EditorGUI.Popup(rect, i.intValue, InventoryEditorUtility.equipTypesStrings);
                //EditorGUI.PropertyField(rect, equipTypes.GetArrayElementAtIndex(index));
                //EditorGUILayout.Popup(e.intValue, InventoryEditorUtility.equipTypesStrings);

            };
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical();

            DrawPropertiesExcluding(serializedObject, new string[]
            {
                "ID",
                "_equipTypes"
            });
            EditorGUILayout.EndVertical();


            EditorGUILayout.LabelField("Edit types at Tools/InventorySystem/Equip manager", InventoryEditorStyles.titleStyle);
            //EditorGUILayout.BeginVertical(InventoryEditorStyles.reorderableListStyle);
            list.DoLayoutList();
            //EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

    }
}