#if EASY_SAVE_2

using UnityEngine;
using System.Collections;
using Devdog.InventorySystem;
using System.Collections.Generic;
using System.Reflection;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem
{

    public partial class ItemCollectionBase
    {
        private string easySaveCollectionName
        {
            get { return collectionName.ToLower().Replace(" ", "_"); }
        }


        /// <summary>
        /// Save the collection using EasySave2
        /// This method will use the collections name for the file name.
        /// </summary>
        public void SaveEasySave2(params string[] additionalFields)
        {
            SaveEasySave2(easySaveCollectionName + ".txt");
        }

        /// <summary>
        /// Save the collection using EasySave2
        /// This method will write to the given file name.
        /// </summary>
        /// <param name="fileName">The name of the file you want to save to</param>
        public void SaveEasySave2(string fileName, params string[] additionalFields)
        {
            using (ES2Writer writer = ES2Writer.Create(fileName, new ES2Settings() { fileMode = ES2Settings.ES2FileMode.Create }))
            {
                _SaveEasySave2(writer, "", additionalFields);
                writer.Save();
            }
        }

        /// <summary>
        /// Save the collection using EasySave2
        /// This method will write to the given ES2Writer with the prefix before all entries
        /// </summary>
        /// <param name="writer">The opened ES2 writer to write to</param>
        /// <param name="prefix">A string to prepend to all entries</param>
        public void SaveEasySave2(ES2Writer writer, string prefix, params string[] additionalFields)
        {
            _SaveEasySave2(writer, prefix, additionalFields);
        }

        private void _SaveEasySave2(ES2Writer writer, string prefix, string[] additionalFields)
        {
            if (useReferences)
            {
                var l = InventorySerializationUtility.GetCollectionReferenceLookups(this);

                writer.Write(l, prefix + "itemReferenceLookups_" + easySaveCollectionName);
            }
            else
            {
                var l = InventorySerializationUtility.GetCollectionLookups(this);

                writer.Write(l, prefix + "itemLookups_" + easySaveCollectionName);
            }

            var list = new List<float>(additionalFields.Length);
            for (int i = 0; i < additionalFields.Length; i++)
            {
                var f = GetType().GetField(additionalFields[i], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (f != null)
                {
                    //float val = (float);
                    list.Add(float.Parse(f.GetValue(this).ToString()));
                }
            }

            if (list.Count > 0)
                writer.Write(list.ToArray(), prefix + "additinalFields_" + easySaveCollectionName);

        }


        /// <summary>
        /// Load the collection using EasySave2
        /// This method uses the collections name to load the data.
        /// </summary>
        public void LoadEasySave2(params string[] additionalFields)
        {
            LoadEasySave2(easySaveCollectionName + ".txt");
        }

        /// <summary>
        /// Load the collection using EasySave2
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadEasySave2(string fileName, params string[] additionalFields)
        {
            if (ES2.Exists(fileName) == false)
            {
                // No data to load yet
                Debug.Log("Can't load from file " + fileName + " file does not exist.", gameObject);
                return;
            }

            // Load all the items

            using (ES2Reader reader = ES2Reader.Create(fileName))
            {
                if (useReferences)
                    _LoadEasySave2References(reader, "", additionalFields);
                else
                    _LoadEasySave2(reader, "", additionalFields);

                _LoadEasySave2Additional(reader, "", additionalFields);
            }
        }

        /// <summary>
        /// Load the collection using EasySave2
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadEasySave2(ES2Reader reader, string prefix, params string[] additionalFields)
        {
            if (useReferences)
            {
                _LoadEasySave2References(reader, prefix, additionalFields);
            }
            else
            {
                _LoadEasySave2(reader, prefix, additionalFields);
            }

            _LoadEasySave2Additional(reader, prefix, additionalFields);
        }


        private void _LoadEasySave2Additional(ES2Reader reader, string prefix, string[] additionalFields)
        {
            if (reader.TagExists(prefix + "additinalFields_" + easySaveCollectionName) == false)
                return;

            float[] additional = reader.ReadArray<float>(prefix + "additinalFields_" + easySaveCollectionName);

            for (int i = 0; i < additional.Length; i++)
            {
                var f = GetType().GetField(additionalFields[i], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (f != null)
                {
                    var t = f.GetValue(this).GetType();
                    if (t == typeof(int))
                        f.SetValue(this, (int)additional[i]);
                    else if (t == typeof(float))
                        f.SetValue(this, (float)additional[i]);
                    else if (t == typeof(uint))
                        f.SetValue(this, (uint)additional[i]);
                    else
                        Debug.LogWarning("Type not found for " + t.ToString());
                }
            }
        }

        private void _LoadEasySave2References(ES2Reader reader, string prefix, string[] additionalFields)
        {
            // Read data from the file in any order.
            InventoryItemReferenceSaveLookup[] data = reader.ReadArray<InventoryItemReferenceSaveLookup>(prefix + "itemReferenceLookups_" + easySaveCollectionName);

            var l = new List<InventoryItemBase>(data.Length);
            var cols = Object.FindObjectsOfType<ItemCollectionBase>();

            foreach (var item in data)
            {
                if (item.itemID == -1)
                    l.Add(null);
                else
                {
                    foreach (var col in cols)
                    {
                        if (col.collectionName == item.referenceOfCollection)
                        {
                            // Found it
                            l.Add(col.Find((uint)item.itemID));
                        }
                    }

                }
            }

            SetItems(l.ToArray(), false);
        }

        private void _LoadEasySave2(ES2Reader reader, string prefix, string[] additionalFields)
        {

            // Read data from the file in any order.
            InventoryItemSaveLookup[] data = reader.ReadArray<InventoryItemSaveLookup>(prefix + "itemLookups_" + easySaveCollectionName);

            var l = new List<InventoryItemBase>(data.Length);
            var i = ItemManager.instance;

            foreach (var item in data)
            {
                if (item.itemID == -1)
                    l.Add(null);
                else
                {
                    var copy = GameObject.Instantiate<InventoryItemBase>(i.items[item.itemID]);
                    copy.currentStackSize = item.amount;
                    l.Add(copy);
                }
            }

            SetItems(l.ToArray(), true);
        }

    }
}

#endif