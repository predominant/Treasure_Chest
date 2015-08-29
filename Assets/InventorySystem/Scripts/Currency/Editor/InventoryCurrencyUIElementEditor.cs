using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Devdog.InventorySystem.UI;
using System.Linq;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem.Editors
{
    [CustomPropertyDrawer(typeof(InventoryCurrencyUIElement), true)]
    [CanEditMultipleObjects()]
    public class InventoryCurrencyUIElementEditor : PropertyDrawer
    {      
        private SerializedProperty overrideStringFormat;
        private SerializedProperty overrideStringFormatString;

        private SerializedProperty amount;
        private SerializedProperty icon;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) - 20.0f; // -20 for collapsable row (foldout)
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            

            overrideStringFormat = property.FindPropertyRelative("overrideStringFormat");
            overrideStringFormatString = property.FindPropertyRelative("overrideStringFormatString");

            amount = property.FindPropertyRelative("amount");
            icon = property.FindPropertyRelative("icon");

            EditorGUILayout.PropertyField(amount);
            EditorGUILayout.PropertyField(icon);
            

            EditorGUILayout.PropertyField(overrideStringFormat);
            if (overrideStringFormat.boolValue)
            {
                EditorGUILayout.PropertyField(overrideStringFormatString);
            }

            EditorGUI.EndProperty();
        }
    }
}