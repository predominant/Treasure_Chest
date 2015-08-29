using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using Devdog.InventorySystem.Dialogs;
using Devdog.InventorySystem.Models;
#if PLY_GAME
using Devdog.InventorySystem.Integration.plyGame.Editors;
#endif


namespace Devdog.InventorySystem.Editors
{
    using System.Linq;

    public class InventoryMainEditor : EditorWindow
    {
        private int toolbarIndex { get; set; }

        public static EmptyEditor itemEditor { get; set; }
        public static EmptyEditor equipEditor { get; set; }
        public static CraftingEmptyEditor craftingEditor { get; set; }
        public static LanguageEditor languageEditor { get; set; }
        public static SettingsEditor settingsEditor { get; set; }

        public static List<IInventorySystemEditorCrud> editors = new List<IInventorySystemEditorCrud>(8);


        private static InventoryMainEditor _window;
        public static InventoryMainEditor window
        {
            get
            {
                if(_window == null)
                    _window = GetWindow<InventoryMainEditor>(false, "Main manager", false);

                return _window;
            }
        }

        protected string[] editorNames
        {
            get
            {
                string[] items = new string[editors.Count];
                for (int i = 0; i < editors.Count; i++)
                {
                    items[i] = editors[i].ToString();
                }

                return items;
            }
        }

        [MenuItem("Tools/Inventory Pro/Main editor", false, -99)] // Always at the top
        public static void ShowWindow()
        {
            _window = GetWindow<InventoryMainEditor>(false, "Main manager", true);
        }

        private void OnEnable()
        {
            minSize = new Vector2(600.0f, 400.0f);
            toolbarIndex = 0;

            //if (InventoryEditorUtility.selectedDatabase == null)
            //    return;

            InventoryProSetupWizard.CheckScene();
            InventoryProSetupWizard.OnIssuesUpdated += UpdateMiniToolbar;

            CreateEditors();
        }

        private void OnDisable()
        {
            InventoryProSetupWizard.OnIssuesUpdated -= UpdateMiniToolbar;
        }

        static internal void UpdateMiniToolbar(List<InventoryProSetupWizard.SetupIssue> issues)
        {
            window.Repaint();
        }

        public virtual void CreateEditors()
        {
            editors.Clear();
            itemEditor = new EmptyEditor("Items editor", this);
            itemEditor.requiresDatabase = true;
            itemEditor.childEditors.Add(new ItemEditor("Item", "Items", this));
            itemEditor.childEditors.Add(new ItemCategoryEditor("Item category", "Item categories", this));
            itemEditor.childEditors.Add(new ItemPropertyEditor("Item property", "Item properties", this) { canReOrderItems = true });
            itemEditor.childEditors.Add(new ItemRarityEditor("Item Rarity", "Item rarities", this) { canReOrderItems = true });
            editors.Add(itemEditor);

            var currencyEditor = new CurrencyEditor("Curreny", "Currencies", this);
            currencyEditor.requiresDatabase = true;
            currencyEditor.canReOrderItems = true;
            editors.Add(currencyEditor);

            equipEditor = new EmptyEditor("Equip editor", this);
            equipEditor.requiresDatabase = true;
            equipEditor.childEditors.Add(new EquipEditor("Stats", this));
#if PLY_GAME
            equipEditor.childEditors.Add(new plyStatsEditor("Ply stats", this));
#endif
            equipEditor.childEditors.Add(new EquipTypeEditor("Equip type", "Equip types", this));
            editors.Add(equipEditor);

            craftingEditor = new CraftingEmptyEditor("Crafting editor", this);
            craftingEditor.requiresDatabase = true;
            editors.Add(craftingEditor);

            languageEditor = new LanguageEditor("Language editor", "Language categories", this);
            editors.Add(languageEditor);

            settingsEditor = new SettingsEditor("Settings editor", "Settings categories", this);
            editors.Add(settingsEditor);
        }

        protected virtual void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal();
            GUI.color = Color.grey;
            if (GUILayout.Button("< DB", InventoryEditorStyles.toolbarStyle, GUILayout.Width(60)))
            {
                InventoryEditorUtility.selectedDatabase = null;
                toolbarIndex = 0;
            }
            GUI.color = Color.white;

            int before = toolbarIndex;
            toolbarIndex = GUILayout.Toolbar(toolbarIndex, editorNames, InventoryEditorStyles.toolbarStyle);
            if (before != toolbarIndex)
                editors[toolbarIndex].Focus();
            
            EditorGUILayout.EndHorizontal();
        }

        internal static void DrawMiniToolbar(List<InventoryProSetupWizard.SetupIssue> issues)
        {
            GUILayout.BeginVertical("Toolbar", GUILayout.ExpandWidth(true));
            
            var issueCount = issues.Sum(o => o.ignore == false ? 1 : 0);
            if (issueCount > 0)
                GUI.color = Color.red;
            else
                GUI.color = Color.green;
            
            if (GUILayout.Button(issueCount + " issues found in scene.", "toolbarbutton", GUILayout.Width(300)))
            {
                InventoryProSetupWizard.ShowWindow();
            }

            GUI.color = Color.white;

            GUILayout.EndVertical();
        }


        protected virtual bool CheckDatabase()
        {
            if (InventoryEditorUtility.selectedDatabase == null)
            {
                ShowItemDatabasePicker();
                return false;
            }

            return true;
        }

        protected virtual void ShowItemDatabasePicker()
        {
            EditorGUILayout.LabelField("Found the following databases in your project folder:", EditorStyles.largeLabel);

            var dbs = AssetDatabase.FindAssets("t:" + typeof(InventoryItemDatabase).Name);
            foreach (var db in dbs)
            {
                EditorGUILayout.BeginHorizontal();

                if (InventoryEditorUtility.GetItemDatabase(true, false) != null && AssetDatabase.GUIDToAssetPath(db) == AssetDatabase.GetAssetPath(InventoryEditorUtility.GetItemDatabase(true, false)))
                    GUI.color = Color.green;

                EditorGUILayout.LabelField(AssetDatabase.GUIDToAssetPath(db), InventoryEditorStyles.labelStyle);
                if (GUILayout.Button("Select", GUILayout.Width(100)))
                {
                    InventoryEditorUtility.selectedDatabase = (InventoryItemDatabase)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(db), typeof(InventoryItemDatabase));
                    OnEnable(); // Re-do editors
                }

                GUI.color = Color.white;
                EditorGUILayout.EndHorizontal();
            }

            if (dbs.Length == 0)
            {
                EditorGUILayout.LabelField("No Item databases found, first create one in your assets folder.");
            }
        }


        public void OnGUI()
        {
            DrawToolbar();

            InventoryEditorUtility.ErrorIfEmpty(EditorPrefs.GetString("InventorySystem_ItemPrefabPath") == string.Empty, "Inventory item prefab folder is not set, items cannot be saved! Please go to settings and define the Inventory item prefab folder.");
            if (EditorPrefs.GetString("InventorySystem_ItemPrefabPath") == string.Empty)
            {
                GUI.enabled = true;
                toolbarIndex = editors.Count - 1;
                // Draw the editor
                editors[toolbarIndex].Draw();

                if (GUI.changed && InventoryEditorUtility.selectedDatabase != null)
                    EditorUtility.SetDirty(InventoryEditorUtility.selectedDatabase); // To make sure it gets saved.

                GUI.enabled = false;
                return;
            }

            if (CheckDatabase() == false && editors[toolbarIndex].requiresDatabase)
                return;

            // Draw the editor
            editors[toolbarIndex].Draw();

            DrawMiniToolbar(InventoryProSetupWizard.setupIssues);

            if (GUI.changed && InventoryEditorUtility.selectedDatabase != null)
                EditorUtility.SetDirty(InventoryEditorUtility.selectedDatabase); // To make sure it gets saved.
        }
    }
}