using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Devdog.InventorySystem.Editors
{
    public class CraftingEmptyEditor : EmptyEditor
    {


        public CraftingEmptyEditor(string name, EditorWindow window)
            : base(name, window)
        {
            CreateEditors();
        }


        public void CreateEditors()
        {
            childEditors.Clear();
            if (InventoryEditorUtility.selectedDatabase != null)
            {
                foreach (var cat in InventoryEditorUtility.selectedDatabase.craftingCategories)
                {
                    childEditors.Add(new CraftingBlueprintEditor(cat.name + " blueprint", cat.name + " blueprints", window, this));
                }
            }

            childEditors.Add(new CraftingCategoryEditor("Crafting category", "Crafting categories", window, this));
        }

        
        public override string ToString()
        {
            return name;
        }
    }
}
