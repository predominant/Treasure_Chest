using UnityEngine;
using UnityEditor;
using System.Collections;


namespace uGUIPanelManager
{
    [CustomPropertyDrawer (typeof(PanelStateSettings))]
    public class uGUIStateTransformDrawer : PropertyDrawer
    {
        private int fieldHeight = 80;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property.FindPropertyRelative("callBeforeEnterState"), true);
            position.y += fieldHeight;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("callAfterEnterState"), true);
            position.y += fieldHeight;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("callBeforeLeaveState"), true);
            position.y += fieldHeight;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("callAfterLeaveState"), true);
            position.y += fieldHeight;

        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return fieldHeight * 4;

        }


    }
}