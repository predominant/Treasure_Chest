using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.Models;
using UnityEngine;

namespace Devdog.InventorySystem
{
    [AddComponentMenu("InventorySystem/SaveLoad/SimpleCollectionSaverLoader")]
    public partial class SimpleCollectionSaverLoader : MonoBehaviour
    {

        public JsonCollectionSerializer serializer { get; private set; }
        public ItemCollectionBase collection { get; private set; }
        public BasicCollectionSaverLoader saverLoader { get; private set; }

        public void Awake()
        {
            serializer = new JsonCollectionSerializer();
            collection = GetComponent<ItemCollectionBase>();
            saverLoader = new BasicCollectionSaverLoader();

            Load();
        }

        public void OnApplicationQuit()
        {
            Save();
        }

        public virtual void Save()
        {
            if (collection.useReferences)
            {
                var r = GetReferenceLookups();
                var bytes = serializer.SerializeItemReferences(r);
                saverLoader.SaveItems(collection, null, bytes, null);
                Debug.Log("Saved " + r.Length + " references");
            }
            else
            {
                var r = GetItemLookups();
                var bytes = serializer.SerializeItems(r);
                saverLoader.SaveItems(collection, null, bytes, null);
                Debug.Log("Saved " + r.Length + " items");
            }
        }

        public virtual void Load()
        {
            if (collection.useReferences)
            {
                //var r = saverLoader.LoadItems(collection, null, (bytes) =>
                //{
                //    var b = serializer.DeserializeItemReferences(bytes);
                //    var itemsArray = new InventoryItemBase[b.Count];
                //    for (int i = 0; i < itemsArray.Length; i++)
                //    {
                //        itemsArray[i] = b[i].itemID
                //    }
                //    collection.SetItems(b.ToArray());

                //    Debug.Log("Saved " + r.Count + " references"); 
                //});
            }
            else
            {
                saverLoader.LoadItems(collection, null, (bytes) =>
                {
                    var b = serializer.DeserializeItems(bytes);
                    var itemsArray = new InventoryItemBase[b.Count];
                    for (int i = 0; i < itemsArray.Length; i++)
                    {
                        if (b[i].itemID != -1)
                        {
                            itemsArray[i] = Instantiate<InventoryItemBase>(ItemManager.instance.items[b[i].itemID]);
                            itemsArray[i].currentStackSize = b[i].amount;
                        }
                        else
                            itemsArray[i] = null;
                    }
                    collection.SetItems(itemsArray, true);

                    Debug.Log("Loaded " + itemsArray.Length + " items"); 
                });
            }
        }


        public virtual InventoryItemReferenceSaveLookup[] GetReferenceLookups()
        {
            return InventorySerializationUtility.GetCollectionReferenceLookups(collection);
        }

        public virtual InventoryItemSaveLookup[] GetItemLookups()
        {
            return InventorySerializationUtility.GetCollectionLookups(collection);
        }

    }
}
