using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.Models;
using UnityEditor;
using UnityEngine;

namespace Devdog.InventorySystem.Editors
{
    public class CraftingCategoryEditor : InventorySystemEditorCrudBase<InventoryCraftingCategory>
    {
        protected override List<InventoryCraftingCategory> crudList
        {
            get { return new List<InventoryCraftingCategory>(InventoryEditorUtility.selectedDatabase.craftingCategories); }
            set { InventoryEditorUtility.selectedDatabase.craftingCategories = value.ToArray(); }
        }

        protected CraftingEmptyEditor parentEditor { get; set; }

        public CraftingCategoryEditor(string singleName, string pluralName, EditorWindow window, CraftingEmptyEditor editor)
            : base(singleName, pluralName, window)
        {
            parentEditor = editor;
        }

        protected override bool MatchesSearch(InventoryCraftingCategory item, string searchQuery)
        {
            string search = searchQuery.ToLower();
            return (item.ID.ToString().Contains(search) || item.name.ToLower().Contains(search) || item.description.ToLower().Contains(search));
        }

        protected override void CreateNewItem()
        {
            var item = new InventoryCraftingCategory();
            item.ID = (crudList.Count > 0) ? crudList.Max(o => o.ID) + 1 : 0;
            AddItem(item, true);
        }

        public override void AddItem(InventoryCraftingCategory item, bool editOnceAdded = true)
        {
            base.AddItem(item, editOnceAdded);

            parentEditor.CreateEditors();
            parentEditor.toolbarIndex = parentEditor.childEditors.Count - 1;
        }

        public override void RemoveItem(int index)
        {
            base.RemoveItem(index);

            parentEditor.CreateEditors();
            parentEditor.toolbarIndex = parentEditor.childEditors.Count - 1;
        }


        protected override void DrawSidebarRow(InventoryCraftingCategory item, int i)
        {
            //GUI.color = new Color(1.0f,1.0f,1.0f);
            BeginSidebarRow(item, i);

            DrawSidebarRowElement("#" + item.ID.ToString(), 40);
            DrawSidebarRowElement(item.name, 260);

            EndSidebarRow(item, i);
        }

        protected override void DrawDetail(InventoryCraftingCategory prop, int index)
        {
            EditorGUIUtility.labelWidth = InventoryEditorStyles.labelWidth;


            EditorGUILayout.BeginVertical(InventoryEditorStyles.boxStyle);

            EditorGUILayout.LabelField("Note that this is not used for item categories but rather category types such as Smithing, Tailoring, etc.", InventoryEditorStyles.titleStyle);
            EditorGUILayout.Space();

            prop.name = EditorGUILayout.TextField("Category name", prop.name);
            prop.description = EditorGUILayout.TextField("Category description", prop.description);

            EditorGUILayout.Space();
            prop.alsoScanBankForRequiredItems = EditorGUILayout.Toggle("Scan bank for craft items", prop.alsoScanBankForRequiredItems);
            EditorGUILayout.Space();


            prop.rows = (uint)EditorGUILayout.IntField("Layout rows", (int)prop.rows);
            prop.cols = (uint)EditorGUILayout.IntField("Layout cols", (int)prop.cols);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Forces the result item to be saved in a collection, leave empty to auto. detect the best collection.", InventoryEditorStyles.labelStyle);
            prop.forceSaveInCollection = (ItemCollectionBase)EditorGUILayout.ObjectField("Force save in collection", prop.forceSaveInCollection, typeof(ItemCollectionBase), true);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Category contains " + prop.blueprints.Length + " blueprints.", InventoryEditorStyles.titleStyle);
            EditorGUILayout.EndVertical();


            EditorGUIUtility.labelWidth = 0;
        }

        public override string ToString()
        {
            return singleName + " editor";
        }


        protected override bool IDsOutOfSync()
        {
            return false;
        }

        protected override void SyncIDs()
        {

        }
    }
}
