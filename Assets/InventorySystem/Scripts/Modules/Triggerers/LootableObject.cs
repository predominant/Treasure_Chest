using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Devdog.InventorySystem.UI;
using UnityEngine.Serialization;

namespace Devdog.InventorySystem
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(ObjectTriggerer))]
    [AddComponentMenu("InventorySystem/Triggers/Lootable objet")]
    public partial class LootableObject : MonoBehaviour, IObjectTriggerUser, IInventoryItemContainer
    {
        public delegate void LootedItem(InventoryItemBase item, uint itemID, uint slot, uint amount);

        /// <summary>
        /// Called when an item was looted by a player from this lootable object.
        /// </summary>
        public event LootedItem OnLootedItem;



        [FormerlySerializedAs("items")]
        [SerializeField]
        private InventoryItemBase[] _items;
        public InventoryItemBase[] items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
            }
        }


        public LootUI lootWindow { get; protected set; }
        public UIWindow window { get; protected set; }

        protected Animator animator;
        public ObjectTriggerer triggerer { get; protected set; }


        public virtual void Awake()
        {
            //base.Awake();
            lootWindow = InventoryManager.instance.loot;
            if (lootWindow == null)
            {
                Debug.LogWarning("No loot window set, yet there's a lootable object in the scene", transform);
                return;
            }

            window = lootWindow.window;
            triggerer = GetComponent<ObjectTriggerer>();
            triggerer.window = window;
            triggerer.handleWindowDirectly = false; // We're in charge now :)

            animator = GetComponent<Animator>();

            triggerer.OnTriggerUse += Use;
            triggerer.OnTriggerUnUse += UnUse;
        }

        protected void LootWindowOnRemovedItem(InventoryItemBase item, uint itemID, uint slot, uint amount)
        {
            items[slot] = null;

            if (OnLootedItem != null)
                OnLootedItem(item, itemID, slot, amount);
        }

        protected virtual void Use(InventoryPlayer player)
        {
            // Set items
            lootWindow.SetItems(items, true);
            
            lootWindow.OnRemovedItem += LootWindowOnRemovedItem;

            window.Show();
        }

        protected virtual void UnUse(InventoryPlayer player)
        {

            lootWindow.OnRemovedItem -= LootWindowOnRemovedItem;

            window.Hide();
        }
    }
}