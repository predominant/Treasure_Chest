using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Devdog.InventorySystem.Editors
{
    public class InventoryItemPicker : InventoryObjectPickerBase<InventoryItemBase>
    {

        protected InventoryItemDatabase sceneDatabase;

        public static InventoryItemPicker Get(string title = "Item picker", Vector2 minSize = new Vector2())
        {
            var window = GetWindow<InventoryItemPicker>(true);
            window.windowTitle = title;
            window.minSize = minSize;
            window.isUtility = true;

            return window;
        }


        public virtual void Show(InventoryItemDatabase database)
        {
            if (database != null)
                Show(database.items);
            else
                Debug.LogWarning("Given database is null...");
        }

        public override void Show(IList<InventoryItemBase> objectsToPickFrom)
        {
            base.Show(objectsToPickFrom);

            sceneDatabase = InventoryEditorUtility.GetItemDatabase(true, false);
        }

        protected override IList<InventoryItemBase> FindObjects(bool searchProjectFolder)
        {
            IList<InventoryItemBase> objects = new List<InventoryItemBase>(1024);
            if (InventoryEditorUtility.GetItemDatabase(true, false) != null)
                objects = InventoryEditorUtility.GetItemDatabase(true, false).items;

            return objects;
        }


        protected override bool MatchesSearch(InventoryItemBase obj, string search)
        {
            return obj.name.ToLower().Contains(search) || obj.description.ToLower().Contains(search) || 
                obj.ID.ToString().Contains(search) || obj.GetType().Name.ToLower().Contains(search) ||
            (InventoryEditorUtility.GetItemDatabase(true, false) != null && InventoryEditorUtility.GetItemDatabase(true, false).itemRarities.First(o => o.ID == obj._rarity).name.ToLower().Contains(search));
        }


        protected override void DrawObjectButton(InventoryItemBase item)
        {
            if (sceneDatabase != null)
            {
                if(sceneDatabase.itemRarities.Length > 0)
                {
                    var rarity = sceneDatabase.itemRarities.FirstOrDefault(o => o.ID == item._rarity);

                    if (rarity != null)
                    {
                        var prevColor = GUI.color;

                        GUI.color = rarity.color;
                        if (GUILayout.Button(rarity.name, "ButtonLeft", GUILayout.Width(80)))
                        {
                            searchQuery = rarity.name;
                            Repaint();
                        }

                        GUI.color = prevColor;
                    }
                }
                else
                {
                    if (GUILayout.Button("", "ButtonLeft", GUILayout.Width(80)))
                    {

                    }
                }
            }

            if (GUILayout.Button("#" + item.ID + " " + item.name, "ButtonRight"))
                NotifyPickedObject(item);
        }
    }
}
