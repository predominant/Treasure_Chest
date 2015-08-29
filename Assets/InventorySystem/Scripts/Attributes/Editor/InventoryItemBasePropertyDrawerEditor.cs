using UnityEngine;
using System.Collections;
using UnityEditor;


namespace Devdog.InventorySystem.Editors
{
    [CustomPropertyDrawer(typeof(InventoryItemBase))]
    public class InventoryItemBasePropertyDrawerEditor : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // TODO, create a custom editor
            //EditorGUI.PropertyField(position, property, label);
            var pos = position;
            pos.width = EditorGUIUtility.labelWidth;
            EditorGUI.PrefixLabel(pos, label);

            pos.x += pos.width;
            pos.width = position.width - EditorGUIUtility.labelWidth;

            if (property.objectReferenceValue == null)
                GUI.color = Color.yellow;

            pos.width -= 30;
            if (GUI.Button(pos, property.objectReferenceValue != null ? property.objectReferenceValue.name : "(No item selected)", EditorStyles.objectField))
            {
                var picker = EditorWindow.GetWindow<InventoryItemPicker>(true);
                picker.Show(InventoryEditorUtility.GetItemDatabase(true, false));

                picker.OnPickObject += (item) =>
                {
                    property.objectReferenceValue = item;
                    property.serializedObject.ApplyModifiedProperties();
                    GUI.changed = true;
                };
            }

            var p = pos;
            p.width = 30;
            p.x += pos.width + 8; // 8 for margin
            if (GUI.Toggle(p, true, "", "VisibilityToggle") == false)
            {
                Selection.activeObject = property.objectReferenceValue;
                //EditorGUIUtility.PingObject(property.objectReferenceValue);
            }


            GUI.color = Color.white;

            EditorGUI.EndProperty();
        }
    }
}