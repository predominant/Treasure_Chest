using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem.Editors
{
    [CustomEditor(typeof(CurrencyInventoryItem), true)]
    public class CurrencyInventoryItemEditor : InventoryItemBaseEditor
    {
        protected SerializedProperty currencyID;
        
        public override void OnEnable()
        {
            base.OnEnable();
            currencyID = serializedObject.FindProperty("_currencyID");
        }

        protected override void PropertiesDrawElement(Rect rect, int index, bool isactive, bool isfocused, bool drawRestore, bool drawPercentage)
        {
            base.PropertiesDrawElement(rect, index, isactive, isfocused, false, drawPercentage);
        }

        protected override void OnCustomInspectorGUI(params CustomOverrideProperty[] extraOverride)
        {
            var l = new List<CustomOverrideProperty>(extraOverride);
            l.Add(new CustomOverrideProperty("_currencyID", () =>
                {
                    var db = InventoryEditorUtility.GetItemDatabase(true, false);
                    if (db != null)
                    {
                        var currency = InventoryEditorUtility.PopupField("Currency type", db.pluralCurrenciesStrings, db.currencies, o => o.ID == (uint)currencyID.intValue);
                        if (currency != null)
                            currencyID.intValue = (int)currency.ID;
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("No database found in current scene, can't select currency type", MessageType.Warning);
                    }
                }));
          
            base.OnCustomInspectorGUI(l.ToArray());
        }
    }
}