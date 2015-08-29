using UnityEngine;
using System.Collections;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem
{
    [AddComponentMenu("InventorySystem/Managers/Item manager")]
    [RequireComponent(typeof(InventorySettingsManager))]
    [RequireComponent(typeof(InventoryManager))]
    [RequireComponent(typeof(InventoryTriggererManager))]
    [RequireComponent(typeof(InventoryPlayerManager))]
    [RequireComponent(typeof(InventoryInputManager))]
    public partial class ItemManager : MonoBehaviour
    {
        [InventoryRequired]
        public InventoryItemDatabase itemDatabase;


        #region Convenience properties

        public InventoryItemBase[] items { get { return itemDatabase.items; } set { itemDatabase.items = value; }}
        public InventoryCurrency[] currencies { get { return itemDatabase.currencies; } set { itemDatabase.currencies = value; }}
        public InventoryItemRarity[] itemRarities { get { return itemDatabase.itemRarities; } set { itemDatabase.itemRarities = value; } }
        public InventoryItemCategory[] itemCategories { get { return itemDatabase.itemCategories; } set { itemDatabase.itemCategories = value; } }
        public InventoryItemProperty[] properties { get { return itemDatabase.properties; } set { itemDatabase.properties = value; } }
        public InventoryEquipStat[] equipStats { get { return itemDatabase.equipStats; } set { itemDatabase.equipStats = value; } }
        public string[] equipStatTypes { get { return itemDatabase.equipStatTypes; } set { itemDatabase.equipStatTypes = value; } }
        public InventoryEquipType[] equipTypes { get { return itemDatabase.equipTypes; } set { itemDatabase.equipTypes = value; } }
        public InventoryCraftingCategory[] craftingCategories { get { return itemDatabase.craftingCategories; } set { itemDatabase.craftingCategories = value; } }

        #endregion


        private static ItemManager _instance;
        public static ItemManager instance {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<ItemManager>();
                }

                return _instance;
            }
        }


        public void Awake()
        {
            _instance = this;


#if UNITY_EDITOR
            if (itemDatabase == null)
                Debug.LogError("Item Database is not assigned!", transform);

#endif
        }

    }
}

// using UnityEditor;