using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem;
using UnityEngine;

namespace Devdog.InventorySystem
{
    public class BasicCollectionSaverLoader : ICollectionSaver, ICollectionLoader
    {

        public void SaveItems(ItemCollectionBase collection, Uri saveLocation, byte[] serializedData, Action<bool> callback)
        {
            PlayerPrefs.SetString("InventorySystem_" + collection.collectionName.ToLower().Replace(" ", "_"), System.Text.Encoding.UTF8.GetString(serializedData));
            if (callback != null)
                callback(true); // All good
        }

        public void LoadItems(ItemCollectionBase collection, Uri loadLocation, Action<byte[]> callback)
        {
            if (PlayerPrefs.HasKey("InventorySystem_" + collection.collectionName.ToLower().Replace(" ", "_")) == false)
                return; // Don't handle the callback, no data found.

            string data = PlayerPrefs.GetString("InventorySystem_" + collection.collectionName.ToLower().Replace(" ", "_"));
            callback(System.Text.Encoding.UTF8.GetBytes(data));
        }
    }
}
