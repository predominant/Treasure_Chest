using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.Models;
using UnityEditor;
using UnityEngine;

namespace Devdog.InventorySystem.Editors
{
    public class ItemPropertyEditor : InventorySystemEditorCrudBase<InventoryItemProperty>
    {
        protected override List<InventoryItemProperty> crudList
        {
            get { return new List<InventoryItemProperty>(InventoryEditorUtility.selectedDatabase.properties); }
            set { InventoryEditorUtility.selectedDatabase.properties = value.ToArray(); }
        }

        public ItemPropertyEditor(string singleName, string pluralName, EditorWindow window)
            : base(singleName, pluralName, window)
        { }

        protected override bool MatchesSearch(InventoryItemProperty item, string searchQuery)
        {
            string search = searchQuery.ToLower();
            return (item.ID.ToString().Contains(search) || item.name.ToLower().Contains(search));
        }

        protected override void CreateNewItem()
        {
            var item = new InventoryItemProperty();
            item.ID = (crudList.Count > 0) ? crudList.Max(o => o.ID) + 1 : 0;
            AddItem(item, true);
        }

        protected override void DrawSidebarRow(InventoryItemProperty item, int i)
        {
            //GUI.color = new Color(1.0f,1.0f,1.0f);
            BeginSidebarRow(item, i);

            DrawSidebarRowElement("#" + item.ID.ToString(), 40);
            DrawSidebarRowElement(item.name, 260);

            EndSidebarRow(item, i);
        }

        protected override void DrawDetail(InventoryItemProperty prop, int index)
        {
            EditorGUIUtility.labelWidth = InventoryEditorStyles.labelWidth;


            EditorGUILayout.BeginVertical(InventoryEditorStyles.boxStyle);

            EditorGUILayout.LabelField("ID", prop.ID.ToString());
            EditorGUILayout.Space();

            prop.category = EditorGUILayout.TextField("Category", prop.category);
            prop.name = EditorGUILayout.TextField("Name", prop.name);
            EditorGUILayout.Space();
            EditorGUILayout.Space();


            prop.showInUI = EditorGUILayout.Toggle("Show in UI", prop.showInUI);
            if (prop.showInUI)
            {
                prop.color = EditorGUILayout.ColorField("UI Color", prop.color);
                if (prop.color.a == 0.0f)
                    EditorGUILayout.HelpBox("Color alpha is 0, color is transparent.\nThis might not be intended behavior.", MessageType.Warning);


                GUI.color = Color.yellow;
                EditorGUILayout.LabelField("You can use string.Format elements to define the text formatting of the property: ");
                EditorGUILayout.LabelField("{0} = The current amount");
                EditorGUILayout.LabelField("{1} = The max amount");
                EditorGUILayout.LabelField("{2} = The property name");
                GUI.color = Color.white;
                prop.valueStringFormat = EditorGUILayout.TextField("Value string format", prop.valueStringFormat);

                EditorGUILayout.LabelField("Format example: ", prop.GetFormattedString(5.0f));
                EditorGUILayout.LabelField("Format example: ", prop.GetFormattedString(100.0f));
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();


            prop.baseValue = EditorGUILayout.FloatField("Base (start) value", prop.baseValue);
            prop.maxValue = EditorGUILayout.FloatField("Max value", prop.maxValue);
            if (prop.baseValue > prop.maxValue)
                prop.baseValue = prop.maxValue;

            prop.useInStats = EditorGUILayout.Toggle("Use in stats", prop.useInStats);

            

            EditorGUILayout.EndVertical();


            EditorGUIUtility.labelWidth = 0;
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
