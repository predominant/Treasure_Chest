using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem.Editors
{
    [CustomEditor(typeof(InventoryItemBase), true)]
    public class InventoryItemBaseEditor : InventoryEditorBase
    {
        //private InventoryItemBase item;

        protected SerializedProperty id;
        protected SerializedProperty itemName; // Name is used by Editor.name...
        protected SerializedProperty description;
        protected SerializedProperty properties;
        protected SerializedProperty useCategoryCooldown;
        protected SerializedProperty category;
        protected SerializedProperty icon;
        protected SerializedProperty weight;
        protected SerializedProperty requiredLevel;
        protected SerializedProperty rarity;
        protected SerializedProperty buyPrice;
        protected SerializedProperty sellPrice;
        protected SerializedProperty isDroppable;
        protected SerializedProperty isSellable;
        protected SerializedProperty isStorable;
        protected SerializedProperty maxStackSize;
        protected SerializedProperty cooldownTime;


        private UnityEditorInternal.ReorderableList propertiesList { get; set; }

        public override void OnEnable()
        {
            base.OnEnable();

            id = serializedObject.FindProperty("_id");
            itemName = serializedObject.FindProperty("_name");
            description = serializedObject.FindProperty("_description");
            properties = serializedObject.FindProperty("_properties");
            useCategoryCooldown = serializedObject.FindProperty("_useCategoryCooldown");
            category = serializedObject.FindProperty("_category");
            icon = serializedObject.FindProperty("_icon");
            weight = serializedObject.FindProperty("_weight");
            requiredLevel = serializedObject.FindProperty("_requiredLevel");
            rarity = serializedObject.FindProperty("_rarity");
            buyPrice = serializedObject.FindProperty("_buyPrice");
            sellPrice = serializedObject.FindProperty("_sellPrice");
            isDroppable = serializedObject.FindProperty("_isDroppable");
            isSellable = serializedObject.FindProperty("_isSellable");
            isStorable = serializedObject.FindProperty("_isStorable");
            maxStackSize = serializedObject.FindProperty("_maxStackSize");
            cooldownTime = serializedObject.FindProperty("_cooldownTime");


            var t = (InventoryItemBase)target;

            propertiesList = new UnityEditorInternal.ReorderableList(serializedObject, properties, true, true, true, true);
            propertiesList.drawHeaderCallback += rect => GUI.Label(rect, "Item properties");
            propertiesList.elementHeight = 40;
            propertiesList.drawElementCallback += (rect, index, active, focused) =>
            {
                PropertiesDrawElement(rect, index, active, focused, true, true);
            };
            propertiesList.onAddCallback += (list) =>
            {
                var l = new List<InventoryItemProperty>(t.properties);
                l.Add(new InventoryItemProperty());
                t.properties = l.ToArray();

                GUI.changed = true; // To save..
                EditorUtility.SetDirty(target);
                serializedObject.ApplyModifiedProperties();
                Repaint();
            };
        }

        protected virtual void PropertiesDrawElement(Rect rect, int index, bool isactive, bool isfocused, bool drawRestore, bool drawPercentage)
        {
            var t = (InventoryItemBase)target;

            rect.height = 16;
            rect.y += 2;

            var r2 = rect;
            r2.y += 20;
            r2.width /= 2;
            r2.width -= 5;

            var popupRect = rect;
            popupRect.width /= 2;
            popupRect.width -= 5;

            var i = t.properties[index];
            var db = InventoryEditorUtility.selectedDatabase;

            var prop = InventoryEditorUtility.PopupField(popupRect, "", db.propertiesStrings, db.properties, o => o.ID == i.ID);
            if (prop != null)
                i.ID = prop.ID;

            //// Variables
            //int propertyIndex = InventoryEditorUtility.FindIndex(db.properties, o => o.ID == i.ID);
            //propertyIndex = EditorGUI.Popup(popupRect, propertyIndex, db.propertiesStrings);
            //if(db.properties.Length >= propertyIndex && propertyIndex > 0)
            //    i.ID = db.properties[propertyIndex].ID;

            popupRect.x += popupRect.width;
            popupRect.x += 5;

            if (i.isFactor)
            {
                popupRect.width -= 65;

                i.ID = Mathf.Max(i.ID, 0);
                i.value = EditorGUI.TextField(popupRect, i.value);

                var p = popupRect;
                p.x += popupRect.width + 5;
                p.width = 60;
                EditorGUI.LabelField(p, "(" + (i.floatValue - 1.0f) * 100.0f + "%)");
            }
            else
            {
                i.ID = Mathf.Max(i.ID, 0);
                i.value = EditorGUI.TextField(popupRect, i.value);
            }

            if (drawRestore)
            {
                var r3 = r2;
                r3.width /= 2;
                r3.width -= 5;
                EditorGUI.LabelField(r3, "Action effect");

                r3.x += r3.width + 5;
                i.actionEffect = (InventoryItemProperty.ActionEffect)EditorGUI.EnumPopup(r3, i.actionEffect);

                r2.x += r2.width + 5;
            }

            if (drawPercentage)
            {
                i.isFactor = EditorGUI.Toggle(r2, "Is factor (multiplier 0...1)", i.isFactor);
                r2.x += r2.width + 5;
            }


            // Changed something, copy property data
            if (GUI.changed)
            {
                // We're actually copying the values, can't edit source, because of value
                if (db.properties.Length > 0)
                {
                    var p = db.properties.FirstOrDefault(o => o.ID == i.ID);
                    if (p != null)
                    {
                        i.name = p.name;
                        i.showInUI = p.showInUI;
                        i.color = p.color;
                    }
                }

                EditorUtility.SetDirty(target);
                serializedObject.ApplyModifiedProperties();
                Repaint();
            }
        }


        private IEnumerator DestroyImmediateThis(InventoryItemBase obj)
        {
            yield return null;
            DestroyImmediate(obj.gameObject, false); // Destroy this object
        }


        protected override void OnCustomInspectorGUI(params CustomOverrideProperty[] extraOverride)
        {
            base.OnCustomInspectorGUI(extraOverride);

            serializedObject.Update();
            overrides = extraOverride;
            
            if (InventoryEditorUtility.selectedDatabase == null)
                InventoryEditorUtility.GetItemDatabase(true, true);

            if (InventoryEditorUtility.selectedDatabase == null)
            {
                EditorGUILayout.HelpBox("No item database set, can't modify item!", MessageType.Error);
                return;
            }

            var db = InventoryEditorUtility.selectedDatabase;

            //if (buyPrice.intValue < sellPrice.intValue)
            //    EditorGUILayout.HelpBox("Buy price is lower than the sell price, are you sure?", MessageType.Warning);
                
            // Can't go below 0
            if (cooldownTime.floatValue < 0.0f)
                cooldownTime.floatValue = 0.0f;

            GUI.color = Color.yellow;
            var obj = (InventoryItemBase)target;
            if (obj.gameObject.activeInHierarchy && db.itemRarities.Length > obj._rarity &&
                db.itemRarities.First(o => o.ID == obj._rarity).dropObject != null)
            {
                if (GUILayout.Button("Convert to drop object"))
                {
                    var dropObj = db.itemRarities.First(o => o.ID == obj._rarity).dropObject;
                    var dropInstance = (GameObject)PrefabUtility.InstantiatePrefab(dropObj.gameObject);
                    var holder = dropInstance.AddComponent<ObjectTriggererItemHolder>();
                    dropInstance.AddComponent<ObjectTriggererItem>(); // For item pickup

                    string path = AssetDatabase.GetAssetPath(db.items[obj.ID]);
                    var asset = (GameObject) AssetDatabase.LoadAssetAtPath(path, typeof (GameObject));
                    holder.item = asset.GetComponent<InventoryItemBase>();

                    dropInstance.transform.SetParent(obj.transform.parent);
                    dropInstance.transform.SetSiblingIndex(obj.transform.GetSiblingIndex());
                    dropInstance.transform.position = obj.transform.position;
                    dropInstance.transform.rotation = obj.transform.rotation;
                    
                    Selection.activeGameObject = holder.gameObject;

                    obj.StartCoroutine(DestroyImmediateThis(obj));
                    return;
                }
            }
            GUI.color = Color.white;



            //if (InventoryEditorUtility.GetItemDatabase(true, false).items.FirstOrDefault(o => AssetDatabase.GetAssetPath(o) == AssetDatabase.GetAssetPath(target)) == null)
            //{
            //    EditorGUILayout.HelpBox("Note that the item you're editing is not in this scene's database " + InventoryEditorUtility.selectedDatabase.name, MessageType.Warning);


            //    //GUI.color = Color.yellow;
            //    //if (GUILayout.Button("Add to database"))
            //    //{
            //    //    var l = new List<InventoryItemBase>(InventoryEditorUtility.GetItemDatabase().items);
            //    //    l.Add((InventoryItemBase)target);
            //    //    InventoryEditorUtility.GetItemDatabase().items = l.ToArray();
            //    //}
            //    //GUI.color = Color.white;
            //}

            var excludeList = new List<string>()
            {
                "m_Script",
                "_id",
                "_name",
                "_description",
                "_properties",
                "_category",
                "_useCategoryCooldown",
                "_icon",
                "_weight",
                "_requiredLevel",
                "_rarity",
                "_buyPrice",
                "_sellPrice",
                "_isDroppable",
                "_isSellable",
                "_isStorable",
                "_maxStackSize",
                "_cooldownTime"
            };

            GUILayout.Label("Default", InventoryEditorStyles.titleStyle);
            EditorGUILayout.BeginVertical(InventoryEditorStyles.boxStyle);
            if (FindOverride("_id") != null)
                GUI.enabled = false;

            EditorGUILayout.LabelField("ID: ", id.intValue.ToString());
            GUI.enabled = true;

            if (FindOverride("_name") != null)
                GUI.enabled = false;

            GUI.SetNextControlName("ItemEditor_itemName");
            EditorGUILayout.PropertyField(itemName);

            GUI.enabled = true;

            if (FindOverride("_description") != null)
                GUI.enabled = false;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Description", GUILayout.Width(EditorGUIUtility.labelWidth - 5));
            EditorGUILayout.BeginVertical();
            EditorGUILayout.HelpBox("Note, that you can use rich text like <b>asd</b> to write bold text and <i>Potato</i> to write italic text.", MessageType.Info);
            description.stringValue = EditorGUILayout.TextArea(description.stringValue, InventoryEditorStyles.richTextArea);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();


            GUI.enabled = true;

            EditorGUILayout.PropertyField(icon);

            EditorGUILayout.EndVertical();


            // Draws remaining items
            GUILayout.Label("Item specific", InventoryEditorStyles.titleStyle);
            EditorGUILayout.BeginVertical(InventoryEditorStyles.boxStyle);

            foreach (var item in extraOverride)
            {
                if (item.action != null)
                    item.action();

                excludeList.Add(item.serializedName);
            }

            DrawPropertiesExcluding(serializedObject, excludeList.ToArray());
            EditorGUILayout.EndVertical();

            #region Properties

            GUILayout.Label("Item properties", InventoryEditorStyles.titleStyle);
            GUILayout.Label("You can create properties in the Item editor / Item property editor");
            
            EditorGUILayout.BeginVertical(InventoryEditorStyles.reorderableListStyle);
            propertiesList.DoLayoutList();
            EditorGUILayout.EndVertical();

            #endregion

            int rarityIndex = InventoryEditorUtility.FindIndex(db.itemRarities, o => o.ID == rarity.intValue);

            
            GUILayout.Label("Behavior", InventoryEditorStyles.titleStyle);
            EditorGUILayout.BeginVertical(InventoryEditorStyles.boxStyle);

            GUILayout.Label("Details", InventoryEditorStyles.titleStyle);
            if (db.itemRarities.Length >= rarityIndex && rarityIndex > 0)
            {
                var color = db.itemRaritiesColors[rarityIndex];
                color.a = 1.0f; // Ignore alpha in the editor.
                GUI.color = color;
            }

            var rar = InventoryEditorUtility.PopupField("Rarity", db.itemRarityStrings, db.itemRarities, o => o.ID == rarity.intValue);
            if (rar != null)
                rarity.intValue = (int)rar.ID;
            
            GUI.color = Color.white;

            var cat = InventoryEditorUtility.PopupField("Category", db.itemCategoriesStrings, db.itemCategories, o => o.ID == category.intValue);
            if (cat != null)
                category.intValue = (int)cat.ID;
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(useCategoryCooldown);
            if (useCategoryCooldown.boolValue)
                EditorGUILayout.LabelField(string.Format("({0} seconds)", db.itemCategories[category == null ? 0 : category.intValue].cooldownTime));

            EditorGUILayout.EndHorizontal();
            if (useCategoryCooldown.boolValue == false)
                EditorGUILayout.PropertyField(cooldownTime);


            GUILayout.Label("Buying & Selling", InventoryEditorStyles.titleStyle);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Buy price", GUILayout.Width(InventoryEditorStyles.labelWidth));
            EditorGUILayout.PropertyField(buyPrice);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Sell price", GUILayout.Width(InventoryEditorStyles.labelWidth));
            EditorGUILayout.PropertyField(sellPrice);
            EditorGUILayout.EndHorizontal();


            GUILayout.Label("Restrictions", InventoryEditorStyles.titleStyle);
            EditorGUILayout.PropertyField(isDroppable);
            EditorGUILayout.PropertyField(isSellable);
            EditorGUILayout.PropertyField(isStorable);
            EditorGUILayout.PropertyField(maxStackSize);
            EditorGUILayout.PropertyField(weight);
            EditorGUILayout.PropertyField(requiredLevel);


            //GUILayout.Label("Audio & Visuals", InventoryEditorStyles.titleStyle);
            //EditorGUILayout.PropertyField(icon);
            

            //EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();


            serializedObject.ApplyModifiedProperties();
        }
    }
}