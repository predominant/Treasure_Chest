using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Devdog.InventorySystem.Models;
using UnityEditor;
using UnityEngine;

namespace Devdog.InventorySystem.Editors
{
    public class SettingsEditor : InventorySystemEditorCrudBase<SettingsEditor.CategoryLookup>
    {
        public class CategoryLookup
        {
            public string name { get; set; }

            public List<SerializedProperty> serializedProperties = new List<SerializedProperty>(8);

            public CategoryLookup(string name)
            {
                this.name = name;
            }

            public override bool Equals(object obj)
            {
                var o = obj as CategoryLookup;
                return o != null && o.name == name;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        private SerializedObject _serializedObject;
        public SerializedObject serializedObject
        {
            get
            {
                if (_serializedObject == null)
                    _serializedObject = new SerializedObject(settings);

                return _serializedObject;
            }
        }


        protected InventorySettingsManager _settings;
        protected InventorySettingsManager settings
        {
            get
            {
                if(_settings == null)
                    _settings = InventoryEditorUtility.GetSettingsManager();

                return _settings;
            }
        }

        protected override List<CategoryLookup> crudList
        {
            get
            {
                var list = new List<CategoryLookup>(8);
                if (settings != null)
                {
                    var fields = settings.GetType().GetFields();

                    CategoryLookup currentCategory = null;
                    foreach (var field in fields)
                    {
                        var customAttributes = field.GetCustomAttributes(typeof (InventoryEditorCategoryAttribute), true);
                        if (customAttributes.Length == 1)
                        {
                            // Got a category marker
                            currentCategory = new CategoryLookup(customAttributes[0].ToString());
                            list.Add(currentCategory);
                        }

                        if (currentCategory != null)
                        {
                            var prop = serializedObject.FindProperty(field.Name);
                            if (prop != null)
                                currentCategory.serializedProperties.Add(prop);
                        }
                    }
                }

                return list;
            }
            set
            {
                // Doesn't do anything...
            }
        }

        public SettingsEditor(string singleName, string pluralName, EditorWindow window)
            : base(singleName, pluralName, window)
        {
            this.canCreateItems = false;
            this.canDeleteItems = false;
            this.hideCreateItem = true;
        }

        protected override void CreateNewItem()
        {
            
        }

        protected override bool MatchesSearch(CategoryLookup category, string searchQuery)
        {
            string search = searchQuery.ToLower();
            return category.name.ToLower().Contains(search) || category.serializedProperties.Any(o => o.displayName.ToLower().Contains(search));
        }

        public override void Draw()
        {
            #region Path selector

            //InventoryEditorUtility.ErrorIfEmpty(EditorPrefs.GetString("InventorySystem_ItemPrefabPath") == string.Empty, "Inventory item prefab folder is not set, items cannot be saved! Click Set path to a set a save folder.");
            if (EditorPrefs.GetString("InventorySystem_ItemPrefabPath") == string.Empty)
                GUI.color = Color.red;

            EditorGUILayout.BeginHorizontal(InventoryEditorStyles.boxStyle);

            EditorGUILayout.LabelField("Inventory Pro prefab save folder: " + EditorPrefs.GetString("InventorySystem_ItemPrefabPath"));

            GUI.color = Color.white;
            if (GUILayout.Button("Set path", GUILayout.Width(100)))
            {
                string path = EditorUtility.SaveFolderPanel("Choose a folder to save your item prefabs", "", "");
                EditorPrefs.SetString("InventorySystem_ItemPrefabPath", "Assets" + path.Replace(Application.dataPath, ""));
            }

            EditorGUILayout.EndHorizontal();

            GUI.color = Color.white;

            #endregion

            base.Draw();
        }

        protected override void DrawSidebarRow(CategoryLookup category, int i)
        {
            BeginSidebarRow(category, i);

            DrawSidebarRowElement(category.name, 400);

            EndSidebarRow(category, i);
        }

        protected override void DrawDetail(CategoryLookup category, int index)
        {
            EditorGUILayout.BeginVertical(InventoryEditorStyles.boxStyle);
            EditorGUIUtility.labelWidth = InventoryEditorStyles.labelWidth;


            serializedObject.Update();
            foreach (var setting in category.serializedProperties)
            {
                EditorGUILayout.PropertyField(setting, true);
            }
            serializedObject.ApplyModifiedProperties();


            EditorGUIUtility.labelWidth = 0; // Resets it to the default
            EditorGUILayout.EndVertical();
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
