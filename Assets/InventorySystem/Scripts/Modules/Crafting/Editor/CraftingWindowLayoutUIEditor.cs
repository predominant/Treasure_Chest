using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace Devdog.InventorySystem.Editors
{
    [CustomEditor(typeof(CraftingWindowLayoutUI), true)]
    public class CraftingWindowLayoutUIEditor : ItemCollectionBaseEditor
    {
        //private CraftingStation item;
        private SerializedProperty craftingCategoryID;
        private static ItemManager itemManager;

        private string[] allCraftingCategories
        {
            get
            {
                var l = new string[itemManager.craftingCategories.Length];
                for (int i = 0; i < itemManager.craftingCategories.Length; i++)
                    l[i] = itemManager.craftingCategories[i].name;

                return l;
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();

            craftingCategoryID = serializedObject.FindProperty("craftingCategoryID");

            itemManager = Editor.FindObjectOfType<ItemManager>();
            if (itemManager == null)
                Debug.LogError("No item manager found in scene, cannot edit item.");
        }


        protected override void OnCustomInspectorGUI(params CustomOverrideProperty[] extraSpecific)
        {
            base.OnCustomInspectorGUI(new CustomOverrideProperty(craftingCategoryID.name, () =>
            {
                GUILayout.Label("Behavior", InventoryEditorStyles.titleStyle);
                craftingCategoryID.intValue = EditorGUILayout.Popup("Crafting category", craftingCategoryID.intValue, allCraftingCategories);
            }));
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}