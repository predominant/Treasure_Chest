using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.Dialogs;
using Devdog.InventorySystem.Models;
using Devdog.InventorySystem.UI;

namespace Devdog.InventorySystem
{
    [RequireComponent(typeof(ItemManager))]
    [RequireComponent(typeof(InventorySettingsManager))]
    [RequireComponent(typeof(InventoryTriggererManager))]
    [RequireComponent(typeof(InventoryPlayerManager))]
    [RequireComponent(typeof(InventoryInputManager))]
    [AddComponentMenu("InventorySystem/Managers/InventoryManager")]
    public partial class InventoryManager : MonoBehaviour
    {
        #region Variables

        [Header("Windows")]
        public InventoryUI inventory;
        public SkillbarUI skillbar;
        public BankUI bank;
        public CharacterUI character;
        public LootUI loot;
        public VendorUI vendor;
        public NoticeUI notice;
        public CraftingWindowStandardUI craftingStandard;
        public CraftingWindowLayoutUI craftingLayout;
        public SelectableObjectInfoUI selectableObjectInfo;
        public InventoryContextMenu contextMenu;

        [Header("Triggerers")]
        public ObjectTriggererFPSUI objectTriggererFPSUI;
        public ObjectTriggererRangeUI objectTriggererRangeUI;

        [Header("Dialogs")]
        public ConfirmationDialog confirmationDialog;
        public ItemBuySellDialog buySellDialog;
        public IntValDialog intValDialog;
        public IntValDialog unstackDialog;


        [Header("Databases")]
        [SerializeField]
        [InventoryRequired]
        private InventoryLangDatabase _lang;
        /// All languages, notifications, stuff like that.
        public InventoryLangDatabase lang
        {
            get
            {
                return _lang;
            }
            set { _lang = value; }
        }


        /// <summary>
        /// The parent holds all collection's objects to keep the scene clean.
        /// </summary>
        public Transform collectionObjectsParent { get; private set; }

        /// <summary>
        /// Collections such as the Inventory are used to loot items.
        /// When an item is picked up the item will be moved to the inventory. You can create multiple Inventories and limit types per inventory.
        /// </summary>
        private static List<InventoryCollectionLookup<ItemCollectionBase>> lootToCollections;
        private static List<InventoryCollectionLookup<CharacterUI>> equipToCollections;
        private static List<ItemCollectionBase> bankCollections;

        #endregion

        private static InventoryManager _instance;
        public static InventoryManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<InventoryManager>();
                }

                return _instance;
            }
        }

        public void Awake()
        {
            _instance = this;

#if UNITY_EDITOR
            if (lang == null)
                Debug.LogError("Language Database is not assigned!", transform);
            
#endif

            lootToCollections = new List<InventoryCollectionLookup<ItemCollectionBase>>(4);
            equipToCollections = new List<InventoryCollectionLookup<CharacterUI>>(4);
            bankCollections = new List<ItemCollectionBase>(4);


            collectionObjectsParent = new GameObject("__COLLECTION_OBJECTS").transform;
            collectionObjectsParent.transform.SetParent(transform);
        }

        public void Start()
        {
            // TODO: Remove this once addopted
            if(InventoryTriggererManager.instance == null)
                Debug.LogWarning("No InventoryTriggererManager found in scene, starting from version V2.1.6 a pickup manager is required in your scene.", transform);

            if(InventoryPlayerManager.instance == null)
                Debug.LogWarning("No InventoryPlayerManager found in scene, starting from version V2.1.6 a player manager is required in your scene.", transform);
        }

        public void OnLevelWasLoaded(int level)
        {
            // Level loaded, reset the cooldowns
            foreach (var category in ItemManager.instance.itemCategories)
            {
                category.lastUsageTime = 0.0f;
            }
        }


        #region Collection stuff

        protected virtual InventoryCollectionLookup<ItemCollectionBase> GetBestLootCollectionForItem(InventoryItemBase item)
        {
            InventoryCollectionLookup<ItemCollectionBase> best = null;

            foreach (var lookup in lootToCollections)
            {
                if (lookup.collection.CanAddItem(item))
                {
                    if (best == null)
                        best = lookup;
                    else if (lookup.priority > best.priority)
                        best = lookup;
                }
            }

            return best;
        }
        protected virtual InventoryCollectionLookup<ItemCollectionBase> GetBestLootCollectionForItem(InventoryItemBase item, bool hasToFitAll)
        {
            if (hasToFitAll)
                return GetBestLootCollectionForItem(item);


            InventoryCollectionLookup<ItemCollectionBase> best = null;

            foreach (var lookup in lootToCollections)
            {
                if (lookup.collection.CanAddItemCount(item) > 0)
                {
                    if (best == null)
                        best = lookup;
                    else if (lookup.priority > best.priority)
                        best = lookup;
                }
            }

            return best;
        }


        /// <summary>
        /// Get the item count of all items in the lootable collections.
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns>Item count in all lootable collections.</returns>
        public static uint GetItemCount(uint itemID, bool checkBank)
        {
            uint count = 0;
            foreach (var collection in lootToCollections)
                count += collection.collection.GetItemCount(itemID);

            if(checkBank)
            {
                foreach (var collection in bankCollections)
                    count += collection.GetItemCount(itemID);
            }

            return count;
        }

        /// <summary>
        /// Get the first item from all lootable collections.
        /// </summary>
        /// <param name="itemID">ID of the object your searching for</param>
        /// <returns></returns>
        public static InventoryItemBase Find(uint itemID, bool checkBank)
        {
            foreach (var col in lootToCollections)
            {
                var item = col.collection.Find(itemID);
                if(item != null)
                    return item;   
            }

            if(checkBank)
            {
                foreach (var col in bankCollections)
                {
                    var item = col.Find(itemID);
                    if (item != null)
                        return item;
                }
            }

            return null;
        }

        /// <summary>
        /// Get all items with a given ID
        /// </summary>
        /// <param name="itemID">ID of the object your searching for</param>
        /// <returns></returns>
        public static List<InventoryItemBase> FindAll(uint itemID, bool checkBank)
        {
            var list = new List<InventoryItemBase>(8);
            foreach (var col in lootToCollections)
            {
                // Linq.Concat doesn't seem to work.. :/
                foreach (var item in col.collection.FindAll(itemID))
                {
                    list.Add(item);
                }
            }
        
            if(checkBank)
            {
                foreach (var col in bankCollections)
                {
                    // Linq.Concat doesn't seem to work.. :/
                    foreach (var item in col.FindAll(itemID))
                    {
                        list.Add(item);
                    }
                }
            }

            return list;
        }


        /// <summary>
        /// Add an item to an inventory.
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <param name="storedItems">The items that were stored, item might be broken up into stacks</param>
        /// <returns></returns>
        public static bool AddItem(InventoryItemBase item, ICollection<InventoryItemBase> storedItems = null, bool repaint = true)
        {
            if (CanAddItem(item) == false)
            {
                instance.lang.collectionFull.Show(item.name, item.description, instance.inventory.collectionName);
                return false;
            }
            
            //// All items fit in 1 collection
    		var bestCollection1 = instance.GetBestLootCollectionForItem(item, false).collection;
			if (item.currentStackSize <= item.maxStackSize && bestCollection1.CanAddItemCount(item) >= item.currentStackSize)
			{
				bestCollection1.AddItem(item);
			    if(storedItems != null)
                    storedItems.Add(item);
				
                return true; // CanAddItemCount makes sure it can be stored, so AddItem can not have failed.
			}

            // Not all items fit in 1 collection, divide them, grab best collection after each iteration
            // Keep going until stack is divided over collections.
            while (item.currentStackSize > 0)
            {
                var bestCollection = instance.GetBestLootCollectionForItem(item, false).collection;
                uint canStoreInCollection = bestCollection.CanAddItemCount(item);

                var copy = GameObject.Instantiate<InventoryItemBase>(item);
                copy.currentStackSize = (uint)Mathf.Min(Mathf.Min(item.currentStackSize, item.maxStackSize), canStoreInCollection);
                bestCollection.AddItem(copy);

                item.currentStackSize -= copy.currentStackSize;
			    if(storedItems != null)
                    storedItems.Add(copy);

                //item.currentStackSize = (uint)Mathf.Max(item.currentStackSize, 0); // Make sure it's positive
            }

            Destroy(item.gameObject); // Item is divided over collections, no longer need it.

            return true;
        }

        ///// <summary>
        ///// Add items to an inventory.
        ///// </summary>
        ///// <param name="items">The items to add</param>
        ///// <param name="storeAsMuchAsPossible">Store as much as possible, will store as many as possible and discard the rest.</param>
        ///// <param name="storedItems">The items that were stored, item might be broken up into stacks</param>
        ///// <param name="repaint">Should items be repainted? True will be fine in most cases</param>
        ///// <returns></returns>
        //public static bool AddItems(IEnumerable<InventoryItemBase> items, bool storeAsMuchAsPossible, ICollection<InventoryItemBase> storedItems = null, bool repaint = true)
        //{
        //    var toDict = new Dictionary<ItemCollectionBase, List<InventoryItemBase>>();

        //    foreach (var item in items)
        //    {
        //        var best = instance.GetBestLootCollectionForItem(item);
        //        if (best != null)
        //        {
        //            if (toDict.ContainsKey(best.collection) == false)
        //                toDict.Add(best.collection, new List<InventoryItemBase>());

        //            toDict[best.collection].Add(item);
        //        }
        //        else if (storeAsMuchAsPossible == false)
        //        {
        //            instance.lang.collectionFull.Show(item.name, item.description, instance.inventory.collectionName);
        //            return false; // Not all items can be stored.
        //        }
        //    }

        //    // Collection is filled
        //    foreach (var item in toDict)
        //    {
        //        item.Key.AddItems(item.Value, storedItems, repaint);
        //    }
        
        //    return true;
        //}

        /// <summary>
        /// Add an item to an inventory and remove it from the collection it was previously in.
        /// </summary>
        /// <param name="item">The item to add</param>
        /// <param name="storedItems">The items that were stored, item might be broken up into stacks</param>
        /// <returns></returns>
        public static bool AddItemAndRemove(InventoryItemBase item, ICollection<InventoryItemBase> storedItems = null, bool repaint = true)
        {
            var best = instance.GetBestLootCollectionForItem(item);

            if (best != null)
            {
                return best.collection.AddItemAndRemove(item, storedItems, repaint);
            }

            instance.lang.collectionFull.Show(item.name, item.description, instance.inventory.collectionName);
            return false;
        }

        public static bool CanAddItem(InventoryItemBase item)
        {
            return CanAddItemCount(item) >= item.currentStackSize;
        }

        public static uint CanAddItemCount(InventoryItemBase item)
        {
            uint count = 0;
            foreach (var lookup in lootToCollections)
                count += lookup.collection.CanAddItemCount(item);

            return count;
        }


        /// <summary>
        /// Remove an item from the inventories / bank when checkBank = true.
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="amount"></param>
        /// <param name="checkBank">Also search the bankf or items, bank items take priority over items in the inventories</param>
        public static uint RemoveItem(uint itemID, uint amount, bool checkBank)
        {
            var allItems = GetItemCount(itemID, checkBank); // All the items in all looting collections
            if (allItems < amount)
            {
                Debug.LogWarningFormat("Tried to remove {0} items, only {1} items available, check with FindAll().Count first.", amount, allItems);
                return 0;
            }

            uint amountToRemove = amount;
            if (checkBank)
            {
                foreach (var bank in bankCollections)
                {
                    if (amountToRemove > 0)
                    {
                        amountToRemove -= bank.RemoveItem(itemID, amountToRemove);
                    }
                    else
                        break;
                }
            }

            foreach (var inventory in lootToCollections)
            {
                //var items = bank.FindAll(itemID);
                if (amountToRemove > 0)
                {
                    amountToRemove -= inventory.collection.RemoveItem(itemID, amountToRemove);
                }
                else
                    break;
            }

            return amount - amountToRemove;
        }


        /// <summary>
        /// Add a collection that functions as an Inventory. Items will be looted to this collection.
        /// </summary>
        /// <param name="collection">The collection to add.</param>
        /// <param name="priority">
        /// How important is the collection, if you 2 collections can hold the item, which one should be chosen?
        /// Range of 0 to 100
        /// </param>
        public static void AddInventoryCollection(ItemCollectionBase collection, int priority)
        {
#if UNITY_EDITOR
            if (collection == null)
            {
                Debug.LogError("Added inventory collection to manager that was NULL!");
                return;
            }
            if (priority < 0)
            {
                Debug.LogError("Priority has to be higher than 0");
                return;
            }
#endif

            lootToCollections.Add(new InventoryCollectionLookup<ItemCollectionBase>(collection, priority));
        }


        public static void RemoveInventoryCollection(ItemCollectionBase collection)
        {
            lootToCollections.RemoveAll(o => o.collection = collection);
            //var found = lootToCollections.FirstOrDefault(o => o.collection == collection);
            //if (found != null)
                //lootToCollections.Remove(found);

            //lootToCollections.Remove(new InventoryCollectionLookup(collection, priority));
        }


        /// <summary>
        /// Check if a given collection is a loot to collection.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static bool IsInventoryCollection(ItemCollectionBase collection)
        {
            return lootToCollections.Any(col => col.collection == collection);
        }

        /// <summary>
        /// Check if a given collection is a equip to collection.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static bool IsEquipToCollection(ItemCollectionBase collection)
        {
            return equipToCollections.Any(col => col.collection == collection);
        }

        /// <summary>
        /// Add a collection that functions as an Equippable collection. Items can be equipped to this collection.
        /// </summary>
        /// <param name="collection">The collection to add.</param>
        /// <param name="priority">
        /// How important is the collection, if you 2 collections can hold the item, which one should be chosen?
        /// Range of 0 to 100
        /// 
        /// Note: This method is not used yet, it only registers the Equippable collection, that's it.
        /// </param>
        public static void AddEquipCollection(CharacterUI collection, int priority)
        {
            equipToCollections.Add(new InventoryCollectionLookup<CharacterUI>(collection, priority));
        }

        public static void RemoveEquipCollection(CharacterUI collection)
        {
            equipToCollections.RemoveAll(o => o.collection == collection);
            //var found = equipToCollections.FirstOrDefault(o => o.collection == collection);
            //if (found != null)
                //equipToCollections.Remove(found);
        }


        public static void AddBankCollection(ItemCollectionBase collection)
        {
            bankCollections.Add(collection);
        }

        public static void RemoveBankCollection(ItemCollectionBase collection)
        {
            bankCollections.Remove(collection);
        }

        /// <summary>
        /// Get all bank collections
        /// I casted it to an array (instead of list) to avoid messing with the internal list.
        /// </summary>
        /// <returns></returns>
        public static ItemCollectionBase[] GetBankCollections()
        {
            return bankCollections.ToArray();
        }

        public static ItemCollectionBase[] GetLootToCollections()
        {
            var l = new List<ItemCollectionBase>(lootToCollections.Count);
            foreach (var item in lootToCollections)
                l.Add(item.collection);

            return l.ToArray();
        }

        public static CharacterUI[] GetEquipToCollections()
        {
            var l = new List<CharacterUI>(equipToCollections.Count);
            foreach (var item in equipToCollections)
                l.Add(item.collection);

            return l.ToArray();
        }

        #endregion

        #region Currencies 

        protected static float CanRemoveCurrencyCountInventories(uint currencyID, bool allowConversions)
        {
            float totalAmount = 0.0f;
            foreach (var col in lootToCollections)
            {
                if (col.collection.canContainCurrencies == false)
                    continue;

                totalAmount += col.collection.CanRemoveCurrencyCount(currencyID, allowConversions);
            }

            return totalAmount;
        }
        protected static float CanRemoveCurrencyCountBanks(uint currencyID, bool allowConversions)
        {
            float totalAmount = 0.0f;
            foreach (var bankCollection in bankCollections)
            {
                if (bankCollection.canContainCurrencies == false)
                    continue;

                totalAmount += bankCollection.CanRemoveCurrencyCount(currencyID, allowConversions);
            }

            return totalAmount;
        }

        /// <summary>
        /// Can we remove the amount of given currency?
        /// </summary>
        /// <param name="currencyLookup"></param>
        /// <param name="allowCurrencyConversion">Allow converting a higher currency down to this currency? For example convert gold to silver.</param>
        /// <param name="checkBank"></param>
        /// <returns></returns>
        public static bool CanRemoveCurrency(InventoryCurrencyLookup currencyLookup, bool allowCurrencyConversion, bool checkBank)
        {
            return CanRemoveCurrency(currencyLookup.amount, currencyLookup.currency.ID, allowCurrencyConversion, checkBank);
        }

        /// <summary>
        /// Can we remove the amount of given currency?
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="currencyID"></param>
        /// <param name="allowCurrencyConversion">Allow converting a higher currency down to this currency? For example convert gold to silver.</param>
        /// <returns></returns>
        public static bool CanRemoveCurrency(float amount, uint currencyID, bool allowCurrencyConversion)
        {
            return CanRemoveCurrency(amount, currencyID, allowCurrencyConversion, false);
        }

        /// <summary>
        /// Can we remove the amount of given currency?
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="currencyID"></param>
        /// <param name="allowCurrencyConversion">Allow converting a higher currency down to this currency? For example convert gold to silver.</param>
        /// <param name="checkBank"></param>
        /// <returns></returns>
        public static bool CanRemoveCurrency(float amount, uint currencyID, bool allowCurrencyConversion, bool checkBank)
        {
            float totalAmount = CanRemoveCurrencyCountInventories(currencyID, allowCurrencyConversion);
            if (checkBank)
            {
                totalAmount += CanRemoveCurrencyCountBanks(currencyID, allowCurrencyConversion);
            }

            return totalAmount >= amount;
        }

        public static bool CanAddCurrency(InventoryCurrencyLookup currencyLookup)
        {
            return CanAddCurrency(currencyLookup.amount, currencyLookup.currency.ID);
        }

        public static bool CanAddCurrency(float amount, uint currencyID)
        {
            float totalAmount = amount;
            foreach (var col in lootToCollections)
            {
                if (col.collection.canContainCurrencies == false)
                    continue;

                totalAmount += col.collection.CanAddCurrencyCount(currencyID);
            }

            return totalAmount >= amount;
        }

        /// <summary>
        /// Add currency to the loot to collections.
        /// Note: Currency is auto. converted if it's exceeding the conversion restrictions.
        /// </summary>
        /// <param name="lookup"></param>
        /// <param name="amountMultipier"></param>
        /// <returns></returns>
        public static bool AddCurrency(InventoryCurrencyLookup lookup, float amountMultipier = 1.0f)
        {
            return AddCurrency(lookup.amount * amountMultipier, lookup.currency.ID);
        }

        /// <summary>
        /// Add currency to the loot to collections.
        /// Note: Currency is auto. converted if it's exceeding the conversion restrictions.
        /// </summary>
        /// <param name="amount">The amount to add</param>
        /// <param name="currencyID">The currencyID (type) of currency to add.</param>
        /// <returns></returns>
        public static bool AddCurrency(float amount, uint currencyID)
        {
            if (CanAddCurrency(amount, currencyID) == false)
                return false;

            float toAdd = amount;
            foreach (var col in lootToCollections)
            {
                if (col.collection.canContainCurrencies == false)
                    continue;
                
                float canAdd = col.collection.CanAddCurrencyCount(currencyID);
                if (canAdd >= toAdd)
                {
                    // All currency can be stored in a single collection.
                    col.collection.AddCurrency(toAdd, currencyID);
                    return true;
                }

                // We've got to spit it, and share the currency over multiple collections.
                toAdd -= canAdd;

                // Will eventually reach the canAdd >= toAdd and add the remainer to a collection.
                col.collection.AddCurrency(canAdd, currencyID);
            }

            Debug.Log("Couldn't add currency even though check passed! Please report this error + stack trace");
            return false;
        }

        public static bool RemoveCurrency(InventoryCurrencyLookup lookup, float amountMultipier = 1.0f)
        {
            return RemoveCurrency(lookup.amount * amountMultipier, lookup.currency.ID);
        }

        public static bool RemoveCurrency(float amount, uint currencyID)
        {
            if (CanRemoveCurrency(amount, currencyID, true) == false)
                return false;

            float toRemove = amount;
            foreach (var col in lootToCollections)
            {
                float canRemove = col.collection.CanRemoveCurrencyCount(currencyID);
                if (canRemove >= toRemove)
                {
                    // All currency can be stored in a single collection.
                    col.collection.RemoveCurrency(toRemove, currencyID, true);
                    return true;
                }

                // We've got to spit it, and share the currency over multiple collections.
                toRemove -= canRemove;

                // Will eventually reach the canAdd >= toAdd and add the remainer to a collection.
                col.collection.RemoveCurrency(canRemove, currencyID, true);
            }

            Debug.Log("Couldn't remove currency even though check passed! Please report this error + stack trace");
            return false;
        }


        #endregion

    }
}