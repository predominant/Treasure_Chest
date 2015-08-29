using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using Devdog.InventorySystem.UI;

namespace Devdog.InventorySystem
{
    using Devdog.InventorySystem.Models;

    [AddComponentMenu("InventorySystem/Windows/Vendor buy back")]
    public partial class VendorUIBuyBack : ItemCollectionBase
    {
        private UIWindowPage _window;
        public UIWindowPage window
        {
            get
            {
                if (_window == null)
                    _window = GetComponent<UIWindowPage>();

                return _window;
            }
            protected set { _window = value; }
        }

        [InventoryRequired]
        public VendorUI vendorUI;
    

        [SerializeField]
        protected uint _initialCollectionSize = 10;
        public override uint initialCollectionSize
        {
            get
            {
                return _initialCollectionSize;
            }
        }

    
        public override void Awake()
        {
            base.Awake();
        }

        public override void Start()
        {
            base.Start();


            InventoryManager.instance.inventory.OnCurrencyChanged += (float amountAdded, InventoryCurrencyLookup lookup) =>
            {
                Repaint();
            };

            window.OnShow += UpdateItems;
            vendorUI.OnSoldItemToVendor += (InventoryItemBase item, uint amount, VendorTriggerer vendor) =>
            {
                UpdateItems();
            };

            vendorUI.OnBoughtItemBackFromVendor += (InventoryItemBase item, uint amount, VendorTriggerer vendor) =>
            {
                UpdateItems();
            };
        }


        protected virtual void Repaint()
        {
            foreach (var item in items)
            {
                item.Repaint();
            }
        }

        public override bool OverrideUseMethod(InventoryItemBase item)
        {
            vendorUI.currentVendor.BuyItemFromVendor(item, true);
            return true;
        }

        protected virtual void UpdateItems()
        {
            if (vendorUI.currentVendor == null)
                return;

            if (vendorUI.currentVendor.enableBuyBack)
            {
                if (vendorUI.currentVendor.buyBackIsShared)
                {
                    if (VendorTriggerer.buyBackDict.ContainsKey(vendorUI.currentVendor.vendorCategory))
                        SetItems(VendorTriggerer.buyBackDict[vendorUI.currentVendor.vendorCategory].ToArray(), true);
                }
                else
                    SetItems(vendorUI.currentVendor.buyBackList.ToArray(), true);
            }
        }

        public override void SetItems(InventoryItemBase[] toSet, bool setParent, bool repaint = true)
        {
            if (vendorUI.currentVendor == null || vendorUI.currentVendor.enableBuyBack == false)
                return;

            base.SetItems(toSet, setParent, false);

            Repaint();
        }

        public override bool CanMergeSlots(uint slot1, ItemCollectionBase collection2, uint slot2)
        {
            return false;
        }
        public override bool SwapOrMerge(uint slot1, ItemCollectionBase handler2, uint slot2, bool repaint = true)
        {
            return SwapSlots(slot1, handler2, slot2, repaint);    
        }
    }
}