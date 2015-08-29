using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace Devdog.InventorySystem.Editors
{
    [CustomEditor(typeof(CraftingTriggerer), true)]
    public class CraftingTriggererEditor : InventoryEditorBase
    {
        //private CraftingStation item;
        private SerializedObject serializer;

        private SerializedProperty craftingCategoryID;
    

        public override void OnEnable()
        {
            base.OnEnable();

            //item = (CraftingStation)target;
            serializer = serializedObject;

            craftingCategoryID = serializer.FindProperty("craftingCategoryID");
        }


        protected override void OnCustomInspectorGUI(params CustomOverrideProperty[] extraOverride)
        {
            base.OnCustomInspectorGUI(extraOverride);

            serializedObject.Update();

            // Draws remaining items
            EditorGUILayout.BeginVertical();
            DrawPropertiesExcluding(serializer, new string[]
            {
                "m_Script",
                "craftingCategoryID",
            });

            //InventoryEditorUtility.GetItemDatabase(true, false);
            craftingCategoryID.intValue = EditorGUILayout.Popup("Crafting category", craftingCategoryID.intValue, InventoryEditorUtility.craftingCategoriesStrings);

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}