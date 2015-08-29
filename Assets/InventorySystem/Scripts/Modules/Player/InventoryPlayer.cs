using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Devdog.InventorySystem.Models;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

namespace Devdog.InventorySystem
{
    [AddComponentMenu("InventorySystem/Player/Inventory player")]
    public partial class InventoryPlayer : MonoBehaviour
    {
        public delegate void PickedUpItem(uint itemID, uint itemAmount);
        public event PickedUpItem OnPickedUpItem;


        public InventoryPlayerEquipTypeBinder[] equipLocations;


        /// <summary>
        /// The CharacterUI collection that handles this characters equipments.
        /// </summary>
        [FormerlySerializedAs("associatedCharacterUI")]
        public CharacterUI characterCollection;

        
        /// <summary>
        /// This player's inventories
        /// </summary>
        [FormerlySerializedAs("inventories")]
        public ItemCollectionBase[] inventoryCollections = new ItemCollectionBase[0];
        

        /// <summary>
        /// This player's skillbar
        /// </summary>
        [FormerlySerializedAs("skillbar")]
        public SkillbarUI skillbarCollection;

        ///// <summary>
        ///// This player's banks
        ///// </summary>
        //public ItemCollectionBase[] banks;

        //[Header("For instantiating only")]
        public bool dynamicallyFindUIElements = false;

        public string characterUIPath = "Canvas/InventoryAndCharacter/PaddingBox/CharacterWindow";
        public string[] inventoryPaths = new string[0];
        public string skillbarPath = "Canvas/Skillbar";



        private InventoryPlayerRangeHelper _rangeHelper;
        public InventoryPlayerRangeHelper rangeHelper
        {
            get
            {
                if (_rangeHelper == null)
                {
                    var comps = GetComponentsInChildren<InventoryPlayerRangeHelper>(true); // GetComponentInChildren (single) doesn't grab in-active objects.
                    _rangeHelper = comps.Length > 0 ? comps[0] : null;
                }

                return _rangeHelper;
            }
            protected set { _rangeHelper = value; }
        }

        public InventoryPlayerEquipHelper equipHelper { get; set; }


        public virtual void Awake()
        {
            if (dynamicallyFindUIElements)
                FindUIElements();

#if UNITY_EDITOR

            if (characterCollection == null)
                Debug.LogWarning("No character collection set on the player, if you have a global equip collection you can ignore this message.", transform);

            if(inventoryCollections.Length == 0)
                Debug.LogWarning("No inventories found on player, if you have a global inventory collection you can ignore this message", transform);

            if(skillbarCollection == null)
                Debug.LogWarning("No skillbar on character", transform);

            if (GetComponentInChildren<InventoryPlayerRangeHelper>() == null)
                Debug.LogError("No InventoryPlayerRangeHelper found on player", transform);


#endif

            InventoryPlayerManager.AddPlayer(this);
            equipHelper = new InventoryPlayerEquipHelper(this);
        }




        public void NotifyPickedUpItem(uint itemID, uint itemAmount)
        {
            if (OnPickedUpItem != null)
                OnPickedUpItem(itemID, itemAmount);
        }


        protected virtual void FindUIElements()
        {
            characterCollection = FindElement<CharacterUI>(characterUIPath, "CharacterUI");
            
            var l = new List<ItemCollectionBase>(inventoryPaths.Length);
            for (int i = 0; i < inventoryPaths.Length; i++)
            {
                var inventory = FindElement<ItemCollectionBase>(inventoryPaths[i], "ItemCollectionBase");
                if(inventory != null)
                    l.Add(inventory);

            }

            inventoryCollections = l.ToArray();

            skillbarCollection = FindElement<SkillbarUI>(skillbarPath, "SkillbarUI");
        }


        private T FindElement<T>(string path, string logName) where T : class
        {
            var obj = GameObject.Find(path);
            if (obj == null)
                Debug.LogWarning(logName + " path is not valid on player: " + path, transform);
            else
                return obj.GetComponent<T>();

            return null;
        }


        public virtual void SetInputControllersActive(bool active)
        {
            this.enabled = active;
            this.rangeHelper.enabled = active;
            
            var userControl = gameObject.GetComponent<IInventoryPlayerController>();
            if (userControl == null)
            {
                Debug.LogWarning("No component found on player that implements IInventoryPlayerController. If you implement your own controller, be sure to implement it.", transform);
                return;
            }

            userControl.SetActive(active);
        }


        

        /// <summary>
        /// For collider based characters
        /// </summary>
        /// <param name="col"></param>
        public virtual void OnTriggerEnter(Collider col)
        {
            TryPickup(col.gameObject);
        }


        /// <summary>
        /// For 2D collider based characters
        /// </summary>
        /// <param name="col"></param>
        public virtual void OnTriggerEnter2D(Collider2D col)
        {
            TryPickup(col.gameObject);
        }

        /// <summary>
        /// Collision pickup attempts
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void TryPickup(GameObject obj)
        {
            // Just for safety in-case the collision matrix isn't set up correctly..
            if (obj.layer == InventorySettingsManager.instance.equipmentLayer)
                return;

            if (InventorySettingsManager.instance.itemTriggerOnPlayerCollision || CanPickupGold(obj))
            {
                var item = obj.GetComponent<ObjectTriggererItem>();
                if (item != null)
                    item.Use(this);
            }
        }

        protected virtual bool CanPickupGold(GameObject obj)
        {
            return InventorySettingsManager.instance.alwaysTriggerGoldItemPickupOnPlayerCollision && obj.GetComponent<CurrencyInventoryItem>() != null;
        }

        /// <summary>
        /// Add the range helper this object depends on.
        /// </summary>
        public void AddRangeHelper()
        {
            var col = new GameObject("_Col");
            col.transform.SetParent(transform);
            InventoryUtility.ResetTransform(col.transform);

            col.gameObject.AddComponent<InventoryPlayerRangeHelper>();
        }
    }
}
