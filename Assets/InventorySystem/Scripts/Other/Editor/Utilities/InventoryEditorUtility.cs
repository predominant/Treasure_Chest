using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.CodeDom.Compiler;

namespace Devdog.InventorySystem.Editors
{
    using Devdog.InventorySystem.Models;

    using Object = UnityEngine.Object;

    public static class InventoryEditorUtility
    {
        public static InventoryItemDatabase selectedDatabase { get; set; }
        public static InventoryLangDatabase selectedLangDatabase { get; set; }




        #region Convenience stuff for UI elements

        public static string[] pluralCurrenciesStrings
        {
            get
            {
                var db = GetItemDatabase(true, false);
                if (db == null)
                    return new string[0];

                return db.pluralCurrenciesStrings;
            }
        }
        public static string[] singleCurrenciesStrings
        {
            get
            {
                var db = GetItemDatabase(true, false);
                if (db == null)
                    return new string[0];

                return db.singleCurrenciesStrings;
            }
        }


        public static string[] craftingCategoriesStrings
        {
            get
            {
                var db = GetItemDatabase(true, false);
                if (db == null)
                    return new string[0];

                return db.craftingCategoriesStrings;
            }
        }

        public static string[] propertiesStrings
        {
            get
            {
                var db = GetItemDatabase(true, false);
                if (db == null)
                    return new string[0];

                return db.propertiesStrings;
            }
        }
        public static string[] itemRaritiesStrings
        {
            get
            {
                var db = GetItemDatabase(true, false);
                if (db == null)
                    return new string[0];

                return db.itemRarityStrings;
            }
        }

        public static Color[] itemRaritiesColors
        {
            get
            {
                var db = GetItemDatabase(true, false);
                if (db == null)
                    return new Color[0];

                return db.itemRaritiesColors;
            }
        }
        public static string[] itemCategoriesStrings
        {
            get
            {
                var db = GetItemDatabase(true, false);
                if (db == null)
                    return new string[0];

                return db.itemCategoriesStrings;
            }
        }

        public static string[] equipTypesStrings
        {
            get
            {
                var db = GetItemDatabase(true, false);
                if (db == null)
                    return new string[0];

                return db.equipTypesStrings;
            }
        }

        #endregion

        public static int FindIndex<T>(IEnumerable<T> items, Func<T, bool> predicate)
        {
            int retVal = 0;
            foreach (var item in items)
            {
                if (predicate(item)) return retVal;
                retVal++;
            }
            return -1;
        }

        public static InventoryItemDatabase GetItemDatabase(bool forceFromCurrentScene, bool andSetAsDefault)
        {
            if (selectedDatabase != null && forceFromCurrentScene == false)
                return selectedDatabase;

            var manager = GameObject.FindObjectOfType<ItemManager>();
            if (manager != null)
            {
                if (andSetAsDefault)
                    selectedDatabase = manager.itemDatabase;

                return manager.itemDatabase;
            }

            return null;
        }
        public static InventoryLangDatabase GetLangDatabase()
        {
            if (selectedLangDatabase != null)
                return selectedLangDatabase;

            var manager = GameObject.FindObjectOfType<InventoryManager>();
            if (manager != null)
                return manager.lang;

            return null;
        }


        public static ItemManager GetItemManager()
        {
            return GameObject.FindObjectOfType<ItemManager>();
        }

        public static InventoryManager GetInventoryManager()
        {
            return GameObject.FindObjectOfType<InventoryManager>();
        }


        public static InventorySettingsManager GetSettingsManager()
        {
            // Already dropped error on GetItemManager()
            return GameObject.FindObjectOfType<InventorySettingsManager>();
        }


        public static void GetAllFieldsInherited(Type startType, List<FieldInfo> appendList)
        {
            if (startType == typeof(MonoBehaviour) || startType == null)
                return;

            // Copied fields can be restricted with BindingFlags
            FieldInfo[] fields = startType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                appendList.Add(field);
            }

            // Keep going untill we hit UnityEngine.MonoBehaviour type.
            GetAllFieldsInherited(startType.BaseType, appendList);
        }


        #region UI helpers

        public static T SimpleObjectPicker<T>(string name, Object o, bool sceneObjects, bool required) where T : Object
        {
            if (o == null && required == true && GUI.enabled)
                GUI.color = Color.red;

            var item = (T)EditorGUILayout.ObjectField(name, o, typeof(T), sceneObjects);
            GUI.color = Color.white;

            return item;
        }
        public static T SimpleObjectPicker<T>(Rect rect, string name, Object o, bool sceneObjects, bool required) where T : Object
        {
            if (o == null && required == true && GUI.enabled)
                GUI.color = Color.red;

            var item = (T)EditorGUI.ObjectField(rect, name, o, typeof(T), sceneObjects);
            GUI.color = Color.white;

            return item;
        }

        public static void ErrorIfEmpty(System.Object o, string msg)
        {
            if (o == null)
            {
                EditorGUILayout.HelpBox(msg, MessageType.Error);
            }
        }

        public static void ErrorIfEmpty(Object o, string msg)
        {
            if (o == null)
            {
                EditorGUILayout.HelpBox(msg, MessageType.Error);
            }
        }

        public static void ErrorIfEmpty(bool o, string msg)
        {
            if (o)
            {
                EditorGUILayout.HelpBox(msg, MessageType.Error);
            }
        }

        public static LayerMask LayerMaskField(string label, LayerMask selected, bool showSpecial)
        {

            List<string> layers = new List<string>();
            List<int> layerNumbers = new List<int>();

            string selectedLayers = "";

            for (int i = 0; i < 32; i++)
            {

                string layerName = LayerMask.LayerToName(i);

                if (layerName != "")
                {
                    if (selected == (selected | (1 << i)))
                    {

                        if (selectedLayers == "")
                        {
                            selectedLayers = layerName;
                        }
                        else
                        {
                            selectedLayers = "Mixed";
                        }
                    }
                }
            }

            //EventType lastEvent = Event.current.type;

            if (Event.current.type != EventType.MouseDown && Event.current.type != EventType.ExecuteCommand)
            {
                if (selected.value == 0)
                {
                    layers.Add("Nothing");
                }
                else if (selected.value == -1)
                {
                    layers.Add("Everything");
                }
                else
                {
                    layers.Add(selectedLayers);
                }
                layerNumbers.Add(-1);
            }

            if (showSpecial)
            {
                layers.Add((selected.value == 0 ? "[X] " : "     ") + "Nothing");
                layerNumbers.Add(-2);

                layers.Add((selected.value == -1 ? "[X] " : "     ") + "Everything");
                layerNumbers.Add(-3);
            }

            for (int i = 0; i < 32; i++)
            {

                string layerName = LayerMask.LayerToName(i);

                if (layerName != "")
                {
                    if (selected == (selected | (1 << i)))
                    {
                        layers.Add("[X] " + layerName);
                    }
                    else
                    {
                        layers.Add("     " + layerName);
                    }
                    layerNumbers.Add(i);
                }
            }

            bool preChange = GUI.changed;

            GUI.changed = false;

            int newSelected = 0;

            if (Event.current.type == EventType.MouseDown)
            {
                newSelected = -1;
            }

            newSelected = EditorGUILayout.Popup(label, newSelected, layers.ToArray(), EditorStyles.layerMaskField);

            if (GUI.changed && newSelected >= 0)
            {
                //newSelected -= 1;
                if (showSpecial && newSelected == 0)
                    selected = 0;
                else if (showSpecial && newSelected == 1)
                    selected = -1;
                else
                {

                    if (selected == (selected | (1 << layerNumbers[newSelected])))
                        selected &= ~(1 << layerNumbers[newSelected]);
                    else
                        selected = selected | (1 << layerNumbers[newSelected]);
                }
            }
            else
                GUI.changed = preChange;

            return selected;
        }


        #endregion


        /// <summary>
        /// Uses conversion between index and ID (if the order of items and ID's don't match)
        /// </summary>
        /// <returns>The object</returns>
        public static T PopupField<T>(string name, string[] stringNames, IEnumerable<T> items, Func<T, bool> predicate) where T : class
        {
            int index = FindIndex(items, predicate);
            index = EditorGUILayout.Popup(name, index, stringNames);
            if (items.Count() - 1 >= index && index >= 0)
                return items.ElementAt(index);

            return null;
        }

        public static T PopupField<T>(Rect rect, string name, string[] stringNames, IEnumerable<T> items, Func<T, bool> predicate) where T : class
        {
            int index = FindIndex(items, predicate);
            index = EditorGUI.Popup(rect, name, index, stringNames);
            if (items.Count() - 1 >= index && index >= 0)
                return items.ElementAt(index);

            return null;
        }

        public static void CurrencyLookup(string name, InventoryCurrencyLookup currencyLookup)
        {
            var db = GetItemDatabase(true, false);

            EditorGUILayout.BeginHorizontal();

            var currency = PopupField("", db.pluralCurrenciesStrings, db.currencies, o => o.ID == currencyLookup.currency.ID);
            if (currency != null)
                currencyLookup._currencyID = currency.ID;
            
            var prevLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 60;
            currencyLookup.amount = EditorGUILayout.FloatField("Amount", currencyLookup.amount);
            EditorGUIUtility.labelWidth = prevLabelWidth;

            EditorGUILayout.EndHorizontal();
        }
    }
}