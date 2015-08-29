using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.Models;
using UnityEditor;
using UnityEngine;

namespace Devdog.InventorySystem.Editors
{
    public class EquipTypeEditor : InventorySystemEditorCrudBase<InventoryEquipType>
    {
        protected override List<InventoryEquipType> crudList
        {
            get { return new List<InventoryEquipType>(InventoryEditorUtility.selectedDatabase.equipTypes); }
            set { InventoryEditorUtility.selectedDatabase.equipTypes = value.ToArray(); }
        }

        private UnityEditorInternal.ReorderableList restrictionList;



        public EquipTypeEditor(string singleName, string pluralName, EditorWindow window)
            : base(singleName, pluralName, window)
        {}


        public override void EditItem(InventoryEquipType item)
        {
            base.EditItem(item);

            restrictionList = new UnityEditorInternal.ReorderableList(selectedItem.blockTypes, typeof(int), false, true, true, true);
            restrictionList.drawHeaderCallback += rect => GUI.Label(rect, "Restrictions");
            restrictionList.drawElementCallback += (rect, index, active, focused) =>
            {
                rect.height = 16;
                rect.y += 2;
                rect.width -= 30;
                rect.x += 30; // Some selection room

                if (index < 0 || index >= selectedItem.blockTypes.Length)
                    return;

                if (selectedItem.blockTypes[index] < 0 || selectedItem.blockTypes[index] >= crudList.Count)
                    return;

                if (crudList[selectedItem.blockTypes[index]] == selectedItem)
                {
                    var t = rect;
                    t.width = 200;

                    GUI.backgroundColor = Color.red;
                    EditorGUI.LabelField(t, "Can't block self");

                    rect.x += 205; // +5 for margin
                    rect.width -= 205;
                }

                selectedItem.blockTypes[index] = EditorGUI.Popup(rect, selectedItem.blockTypes[index], InventoryEditorUtility.equipTypesStrings);
                GUI.backgroundColor = Color.white;
            };
            restrictionList.onAddCallback += list =>
            {
                var l = new List<int>(selectedItem.blockTypes);
                l.Add(0);
                selectedItem.blockTypes = l.ToArray();
                list.list = selectedItem.blockTypes;

                window.Repaint();
            };
            restrictionList.onRemoveCallback += list =>
            {
                // Remove the element itself
                var l = new List<int>(selectedItem.blockTypes);
                l.RemoveAt(list.index);
                selectedItem.blockTypes = l.ToArray();
                list.list = selectedItem.blockTypes;

                window.Repaint();
            };
        }


        protected override bool MatchesSearch(InventoryEquipType item, string searchQuery)
        {
            if (item == null || item.name == null)
                return false;

            string search = searchQuery.ToLower();
            return (item.ID.ToString().Contains(search) || item.name.ToLower().Contains(search));
        }

        protected override void CreateNewItem()
        {
            var item = new InventoryEquipType();
            item.ID = (crudList.Count > 0) ? crudList.Max(o => o.ID) + 1 : 0;
            AddItem(item, true);
        }

        public override void RemoveItem(int index)
        {
            // Remove the reference from other equip types' restrictions.
            foreach (var equipType in crudList)
            {
                var r = new List<int>(equipType.blockTypes);
                foreach (int blockType in equipType.blockTypes)
                {
                    if (blockType == index)
                    {
                        r.RemoveAll(o => o == index);
                    }
                }

                // Only update if something changed
                if (r.Count != equipType.blockTypes.Length)
                {
                    equipType.blockTypes = r.ToArray();
                    GUI.changed = true;
                }

            }


            base.RemoveItem(index);
        }

        protected override void DrawSidebarRow(InventoryEquipType item, int i)
        {
            //GUI.color = new Color(1.0f,1.0f,1.0f);
            BeginSidebarRow(item, i);

            DrawSidebarRowElement("#" + item.ID.ToString(), 40);
            DrawSidebarRowElement(item.name, 260);

            EndSidebarRow(item, i);
        }

        protected override void DrawDetail(InventoryEquipType item, int itemIndex)
        {
            EditorGUILayout.BeginVertical(InventoryEditorStyles.boxStyle);
            EditorGUIUtility.labelWidth = InventoryEditorStyles.labelWidth;


            EditorGUILayout.LabelField("#" + item.ID);
            EditorGUILayout.Space();

            item.name = EditorGUILayout.TextField("Name", item.name);
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.EndVertical();


            EditorGUILayout.BeginVertical(InventoryEditorStyles.reorderableListStyle);

            EditorGUILayout.LabelField("You can force other fields to be empty when you set this. For example when equipping a greatsword, you might want to un-equip the shield.", InventoryEditorStyles.labelStyle);
            restrictionList.DoLayoutList();


            EditorGUIUtility.labelWidth = 0;
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
