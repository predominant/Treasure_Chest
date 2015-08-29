#if UFPS

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Devdog.InventorySystem.Integration.UFPS;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem.Editors
{
    [CustomEditor(typeof(EquippableUFPSInventoryItem), true)]
    [CanEditMultipleObjects()]
    public class EquippableUFPSInventoryItemEditor : EquippableInventoryItemEditor
    {

        private EquippableUFPSInventoryItem tar;


        public override void OnEnable()
        {
            base.OnEnable();


            tar = (EquippableUFPSInventoryItem) target;

        }

        protected override void OnCustomInspectorGUI(params CustomOverrideProperty[] extraOverride)
        {
            var l = new List<CustomOverrideProperty>(extraOverride);
            if (tar.useUFPSItemData)
            {
                l.Add(new CustomOverrideProperty("_id", null));
                l.Add(new CustomOverrideProperty("_name", null));
                l.Add(new CustomOverrideProperty("_description", null));
            }

            base.OnCustomInspectorGUI(l.ToArray());
        }
    }
}

#endif