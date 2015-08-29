#if PLY_GAME

using System;
using System.Collections.Generic;
using Devdog.InventorySystem.Editors;
using UnityEditor;
using plyBloxKit;
using plyBloxKitEditor;
using plyCommonEditor;
using plyGame;
using plyCommon;
using plyGameEditor;
using UnityEngine;

namespace Devdog.InventorySystem.Integration.plyGame.plyBlox
{
    [plyPropertyHandler(typeof(InventoryItemBaseFieldData))]
    public class InventoryItemBaseFieldData_Handler : plyBlockFieldHandler
    {
        //private ItemsAsset itemsAsset;

        public override object GetCopy(object obj)
        {
            InventoryItemBaseFieldData target = obj as InventoryItemBaseFieldData;
            if (target != null) return target.Copy();
            return new InventoryItemBaseFieldData();
        }

        public override void OnFocus(object obj, plyBlock fieldOfBlock)
        {
            //InventoryItemBaseFieldData target = obj == null ? new InventoryItemBaseFieldData() : obj as InventoryItemBaseFieldData;
            //if (itemsAsset == null)
            //{
            //    itemsAsset = (ItemsAsset)EdGlobal.LoadOrCreateAsset<ItemsAsset>(plyEdUtil.DATA_PATH_SYSTEM + "items.asset", "Item Definitions");
            //}

            //itemsAsset.UpdateItemCache();

            //// check if saved still valid
            //if (!string.IsNullOrEmpty(target.id))
            //{
            //    bool found = false;
            //    UniqueID id = new UniqueID(target.id);
            //    for (int i = 0; i < itemsAsset.items.Count; i++)
            //    {
            //        if (id == itemsAsset.items[i].prefabId) { found = true; break; }
            //    }
            //    if (!found)
            //    {
            //        target.id = "";
            //        target.cachedName = "";
            //        ed.ForceSerialise();
            //    }
            //}
        }

        public override bool DrawField(ref object obj, plyBlock fieldOfBlock)
        {
            bool ret = (obj == null);
            InventoryItemBaseFieldData target = obj == null ? new InventoryItemBaseFieldData() : obj as InventoryItemBaseFieldData;


            if (GUILayout.Button((target.item == null) ? "(No item selected)" : target.item.name, EditorStyles.objectField))
            {
                var picker = InventoryItemPicker.Get();
                picker.Show(InventoryEditorUtility.GetItemDatabase(true, false));
                picker.OnPickObject += (item) =>
                {
                    target.item = item;

                    GUI.changed = true;
                    ed.ForceSerialise();
                    ed.Repaint();
                };
            }

            obj = target;
            return ret;
        }
    }
}

#endif