using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem.Editors
{

    [CustomPropertyDrawer(typeof(InventoryCurrencyLookup), true)]
    public class InventoryCurrencyLookupEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var db = InventoryEditorUtility.GetItemDatabase(true, false);
            
            var amount = property.FindPropertyRelative("_amount");
            var currencyID = property.FindPropertyRelative("_currencyID");


            position.width /= 2;
            position.width -= 5;

            var currency = InventoryEditorUtility.PopupField(position, "", db.pluralCurrenciesStrings, db.currencies, o => o.ID == currencyID.intValue);
            if (currency != null)
                currencyID.intValue = (int)currency.ID;

            position.x += position.width + 10;

            var prevLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 60;
            EditorGUI.PropertyField(position, amount);
            EditorGUIUtility.labelWidth = prevLabelWidth;


            EditorGUI.EndProperty();
        }
    }
}