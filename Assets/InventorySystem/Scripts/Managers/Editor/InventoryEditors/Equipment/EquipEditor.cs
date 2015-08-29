using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Devdog.InventorySystem.Models;
using UnityEditor;
using UnityEngine;

namespace Devdog.InventorySystem.Editors
{
    public class EquipEditor : IInventorySystemEditorCrud
    {

        public virtual InventoryItemDatabase currentItemDatabase
        {
            get { return InventoryEditorUtility.selectedDatabase; }
        }
        public virtual InventoryLangDatabase currentLangDatabase
        {
            get { return InventoryEditorUtility.GetLangDatabase(); }
        }

        public string name { get; set; }
        public EditorWindow window { get; protected set; }
        private UnityEditorInternal.ReorderableList typeList { get; set; }
        private UnityEditorInternal.ReorderableList resultList { get; set; }
        private Vector2 statsScrollPos;
        public bool requiresDatabase { get; set; }

        public EquipEditor(string name, EditorWindow window)
        {
            this.name = name;
            this.window = window;
            this.requiresDatabase = true;

            typeList = new UnityEditorInternal.ReorderableList(InventoryEditorUtility.selectedDatabase != null ? InventoryEditorUtility.selectedDatabase.equipStatTypes : new string[] { }, typeof(System.Type), false, true, true, true);
            typeList.drawHeaderCallback += rect => EditorGUI.LabelField(rect, "Types to scan");
            typeList.drawElementCallback += (rect, index, active, focused) =>
            {
                rect.height = 16;
                rect.y += 2;

                var r = rect;
                r.width -= 60;

                var statTypes = InventoryEditorUtility.selectedDatabase.equipStatTypes;

                EditorGUI.LabelField(r, (string.IsNullOrEmpty(statTypes[index]) == false && System.Type.GetType(statTypes[index]) != null) ? System.Type.GetType(statTypes[index]).FullName : "(NOT SET)");

                var r2 = rect;
                r2.width = 60;
                r2.height = 14;
                r2.x += r.width;
                if (GUI.Button(r2, "Set"))
                {
                    var typePicker = InventoryItemTypePicker.Get();
                    typePicker.Show(InventoryEditorUtility.selectedDatabase);
                    typePicker.OnPickObject += type =>
                    {
                        statTypes[index] = type.AssemblyQualifiedName;
                        window.Repaint();
                        GUI.changed = true; // To save..
                    };
                }
            };
            typeList.onAddCallback += list =>
            {
                var l = new List<string>(InventoryEditorUtility.selectedDatabase.equipStatTypes);
                l.Add(null);
                InventoryEditorUtility.selectedDatabase.equipStatTypes = l.ToArray();
                list.list = InventoryEditorUtility.selectedDatabase.equipStatTypes;

                window.Repaint();
            };
            typeList.onRemoveCallback += list =>
            {
                var l = new List<string>(InventoryEditorUtility.selectedDatabase.equipStatTypes);
                l.RemoveAt(list.index);
                InventoryEditorUtility.selectedDatabase.equipStatTypes = l.ToArray();
                list.list = InventoryEditorUtility.selectedDatabase.equipStatTypes;

                window.Repaint();
            };


            resultList = new UnityEditorInternal.ReorderableList(InventoryEditorUtility.selectedDatabase != null ? InventoryEditorUtility.selectedDatabase.equipStats : new InventoryEquipStat[] { }, typeof(InventoryEquipStat), true, true, false, false);
            resultList.drawHeaderCallback += rect =>
            {
                var r = rect;
                r.width = 40;
                r.x += 15; // Little offset on the start

                EditorGUI.LabelField(r, "Show");


                var r2 = rect;
                r2.width -= r.width;
                r2.x += r.width + 20;
                r2.width /= 3.2f;
                EditorGUI.LabelField(r2, "From type");

                r2.x += r2.width;
                EditorGUI.LabelField(r2, "Display name");

                r2.x += r2.width;
                EditorGUI.LabelField(r2, "Category");

            };
            //resultList.elementHeight = 30;
            resultList.drawElementCallback += (rect, index, active, focused) =>
            {
                rect.height = 16;
                rect.y += 2;

                var stat = InventoryEditorUtility.selectedDatabase.equipStats[index];

                var r = rect;
                r.width = 40;
                stat.show = EditorGUI.Toggle(r, stat.show);
                
                var r2 = rect;
                r2.width -= r.width;
                r2.x += r.width + 5;
                r2.width /= 3.2f;
                EditorGUI.LabelField(r2, stat.fieldInfoNameVisual);

                r2.x += r2.width + 5;
                stat.name = EditorGUI.TextField(r2, stat.name);

                r2.x += r2.width + 5;
                stat.category = EditorGUI.TextField(r2, stat.category);
            };
        }

        public void OnEnable()
        {

        }

        public virtual void Focus()
        {
            if (InventoryEditorUtility.selectedDatabase == null)
                return;

            typeList.list = InventoryEditorUtility.selectedDatabase.equipStatTypes;
            resultList.list = InventoryEditorUtility.selectedDatabase.equipStats;
        }


        public virtual void Draw()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace(); // Center horizontally
            EditorGUILayout.BeginVertical(InventoryEditorStyles.boxStyle, GUILayout.MaxWidth(1000));
            statsScrollPos = EditorGUILayout.BeginScrollView(statsScrollPos);
            

            #region Types picker

            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("Step 1: Pick the item types that you want to scan for character stats.", InventoryEditorStyles.titleStyle);
            EditorGUILayout.LabelField("Note: You only have to pick the top level classes.", InventoryEditorStyles.labelStyle);
            EditorGUILayout.LabelField("If EquippableInventoryItem extends from InventoryItemBase, you don't need to pick base. The system handles inheritance.", InventoryEditorStyles.labelStyle);


            EditorGUILayout.BeginVertical(InventoryEditorStyles.reorderableListStyle);
            typeList.DoLayoutList();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();

            #endregion

            EditorGUILayout.LabelField("Step 2: Scan the types for stats.", InventoryEditorStyles.titleStyle);
            if (GUILayout.Button("Scan types"))
            {
                var oldList = new List<InventoryEquipStat>(InventoryEditorUtility.selectedDatabase.equipStats);
                var displayList = new List<InventoryEquipStat>(64);
                foreach (var type in InventoryEditorUtility.selectedDatabase.equipStatTypes)
                {
                    var fields = new List<FieldInfo>();
                    InventoryEditorUtility.GetAllFieldsInherited(System.Type.GetType(type, true), fields);
                    foreach (var field in fields)
                    {
                        var attr = field.GetCustomAttributes(typeof(InventoryStatAttribute), true);
                        if (attr.Length > 0)
                        {
                            var m = (InventoryStatAttribute)attr[0];

                            var old = oldList.FindAll(o => o.fieldInfoNameVisual == field.ReflectedType.Name + "." + field.Name);
                            if (old.Count == 0)
                            {
                                displayList.Add(new InventoryEquipStat() { name = m.name, typeName = type, fieldInfoName = field.Name, fieldInfoNameVisual = field.ReflectedType.Name + "." + field.Name, show = false, category = "Default" });
                            }
                            else
                            {
                                // Item exists more than once.
                                var already = displayList.Find(o => o.fieldInfoNameVisual == field.ReflectedType.Name + "." + field.Name);
                                if (already == null)
                                {
                                    displayList.Add(old[0]);
                                }
                            }
                        }
                    }
                }

                InventoryEditorUtility.selectedDatabase.equipStats = displayList.ToArray();
                resultList.list = InventoryEditorUtility.selectedDatabase.equipStats; // Update list view
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Step 3: Choose what you want to display.", InventoryEditorStyles.titleStyle);

            EditorGUILayout.BeginVertical(InventoryEditorStyles.reorderableListStyle);
            resultList.DoLayoutList();
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        public override string ToString()
        {
            return name;
        }
    }
}
