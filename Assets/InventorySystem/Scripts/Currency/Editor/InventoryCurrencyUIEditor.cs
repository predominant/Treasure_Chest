using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Devdog.InventorySystem.UI;

namespace Devdog.InventorySystem.Editors
{
    using System.Linq;

    [CustomEditor(typeof(InventoryCurrencyUI), true)]
    [CanEditMultipleObjects()]
    public class InventoryCurrencyUIEditor : InventoryEditorBase
    {      
        private SerializedProperty currencyID;
        
        public override void OnEnable()
        {
            base.OnEnable();

            currencyID = serializedObject.FindProperty("_currencyID");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            OnCustomInspectorGUI();
            serializedObject.ApplyModifiedProperties();
        }


        protected override void OnCustomInspectorGUI(params CustomOverrideProperty[] extraOverride)
        {
            var db = InventoryEditorUtility.GetItemDatabase(true, false);

            var currency = InventoryEditorUtility.PopupField("Currency", db.pluralCurrenciesStrings, db.currencies, o => o.ID == (uint)currencyID.intValue);
            if (currency != null)
                currencyID.intValue = (int)currency.ID;

            var doNotDrawList = new List<string>()
            {
                "m_Script",
                "_currencyID"
            };

            foreach (var extra in extraOverride)
            {
                extra.action();
                doNotDrawList.Add(extra.serializedName);
            }

            DrawPropertiesExcluding(serializedObject, doNotDrawList.ToArray());
        }
    }
}