using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Devdog.InventorySystem.Models;
using Devdog.InventorySystem.UI;

namespace Devdog.InventorySystem
{
    [AddComponentMenu("InventorySystem/Windows/Bank")]
    [RequireComponent(typeof(UIWindow))]
    public partial class BankUI : ItemCollectionBase
    {
        [Header("Behavior")]
        public UnityEngine.UI.Button sortButton;

        [SerializeField]
        private uint _initialCollectionSize = 80;
        /// <inheritdoc />
        public override uint initialCollectionSize { get { return _initialCollectionSize; } }

        /// <summary>
        /// When the item is used from this collection, should the item be moved to the inventory?
        /// </summary>
        [Header("Item usage")]
        public bool useMoveToInventory = true;

        private UIWindow _window;
        public UIWindow window
        {
            get
            {
                if (_window == null)
                    _window = GetComponent<UIWindow>();

                return _window;
            }
            protected set { _window = value; }
        }


        [Header("Audio & Visuals")]
        public AudioClip swapItemAudioClip;
        public AudioClip sortAudioClip;
        public AudioClip onAddItemAudioClip; // When an item is added to the inventory


        public override void Awake()
        {
            base.Awake();

            InventoryManager.AddBankCollection(this);

            if(sortButton != null)
            {
                sortButton.onClick.AddListener(() =>
                {
                    SortCollection();

                    if (sortAudioClip)
                        InventoryUtility.AudioPlayOneShot(sortAudioClip);
                });
            }
        }

        public override void Start()
        {
            base.Start();


            // Listen for events
            OnAddedItem += (item, amount, cameFromCollection) =>
            {
                if (onAddItemAudioClip != null)
                    InventoryUtility.AudioPlayOneShot(onAddItemAudioClip);
            };
            OnSwappedItems += (ItemCollectionBase fromCollection, uint fromSlot, ItemCollectionBase toCollection, uint toSlot) =>
            {
                if (swapItemAudioClip != null)
                    InventoryUtility.AudioPlayOneShot(swapItemAudioClip);
            };
        }

        // Items from the bank go straight to the inventory
        public override bool OverrideUseMethod(InventoryItemBase item)
        {
            if (InventorySettingsManager.instance.useContextMenu)
                return false;

            if(useMoveToInventory)
                InventoryManager.AddItemAndRemove(item);

            return useMoveToInventory;
        }

        public override IList<InventoryItemUsability> GetExtraItemUsabilities(IList<InventoryItemUsability> basic)
        {
            var l = base.GetExtraItemUsabilities(basic);

            l.Add(new InventoryItemUsability("To inventory", (item) =>
            {
                var oldCollection = item.itemCollection;
                uint oldIndex = item.index;

                bool added = InventoryManager.AddItem(item);
                if(added)
                {
                    oldCollection.SetItem(oldIndex, null);
                    oldCollection[oldIndex].Repaint();

                    oldCollection.NotifyItemRemoved(item, item.ID, oldIndex, item.currentStackSize);
                }
            }));

            return l;
        }
    
        public override bool CanSetItem(uint slot, InventoryItemBase item)
        {
            bool set = base.CanSetItem(slot, item);
            if (set == false)
                return false;

            if (item == null)
                return true;

            return item.isStorable;
        }
    }
}