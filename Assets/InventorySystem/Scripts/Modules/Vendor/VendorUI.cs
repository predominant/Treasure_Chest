using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.UI;

namespace Devdog.InventorySystem
{
    using Devdog.InventorySystem.Models;

    [AddComponentMenu("InventorySystem/Windows/Vendor UI")]
    [RequireComponent(typeof(UIWindow))]
    public partial class VendorUI : ItemCollectionBase, IInventoryDragAccepter
    {

        #region Events

        public delegate void SoldItemToVendor(InventoryItemBase item, uint amount, VendorTriggerer vendor);
        public delegate void BoughtItemFromVendor(InventoryItemBase item, uint amount, VendorTriggerer vendor);
        public delegate void BoughtItemBackFromVendor(InventoryItemBase item, uint amount, VendorTriggerer vendor);
    

        /// <summary>
        /// Fired when an item is sold.
        /// </summary>
        public event SoldItemToVendor OnSoldItemToVendor;

        /// <summary>
        /// Fired when an item is bought, also fired when an item is bought back.
        /// </summary>
        public event BoughtItemFromVendor OnBoughtItemFromVendor;

        /// <summary>
        /// Fired when an item is bought back from a vendor.
        /// </summary>
        public event BoughtItemBackFromVendor OnBoughtItemBackFromVendor;

        #endregion


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


        protected VendorTriggerer _currentVendor;
        public VendorTriggerer currentVendor
        {
            get
            {
                return _currentVendor;
            }
            set
            {
                _currentVendor = value;
                if (_currentVendor != null)
                {
                    if (window.isVisible == false)
                        return;

                    RepaintWindow();
                }
            }
        }


        public VendorUIBuyBack buyBackCollection;
        public UnityEngine.UI.Text vendorNameText;


        /// <summary>
        /// Prices can be modified per vendor, 0 will generate an int 1 will generate a value of 1.1 and so forth.
        /// </summary>
        //public int roundPriceToDecimals = 0;

        public AudioClip audioWhenSoldItemToVendor;
        public AudioClip audioWhenBoughtItemFromVendor;


        [SerializeField]
        protected uint _initialCollectionSize = 20;
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

            InventoryManager.instance.inventory.OnCurrencyChanged += (float before, InventoryCurrencyLookup lookup) =>
            {
                if (window.isVisible == false)
                    return;

                if (window.isVisible)
                    RepaintWindow();
            };

            window.OnShow += RepaintWindow;
        }


        protected virtual void RepaintWindow()
        {
            foreach (var item in items)
                item.Repaint();

            if (vendorNameText != null)
                vendorNameText.text = _currentVendor.name;
        }

        // <inheritdoc />
        public bool AcceptsDragItem(InventoryItemBase item)
        {
            if (currentVendor == null)
                return false;

            return item.isSellable && currentVendor.canSellToVendor;
        }

        /// <summary>
        /// Called by the InventoryDragAccepter, when an item is dropped on the window / a specific location, this method is called to add a custom behavior.
        /// </summary>
        /// <param name="item"></param>
        public bool AcceptDragItem(InventoryItemBase item)
        {
            if (currentVendor == null || AcceptsDragItem(item) == false)
                return false;

            currentVendor.SellItemToVendor(item);
            return true;
        }

        public override bool OverrideUseMethod(InventoryItemBase item)
        {
            currentVendor.BuyItemFromVendor(item, false);
            return true;
        }

        #region Notifies 

        public virtual void NotifyItemSoldToVendor(InventoryItemBase item, uint amount)
        {
            InventoryManager.instance.lang.vendorSoldItemToVendor.Show(item.name, item.description, amount, currentVendor.name, item.sellPrice.GetFormattedString(amount));

            if (audioWhenSoldItemToVendor != null)
                InventoryUtility.AudioPlayOneShot(audioWhenSoldItemToVendor);

            if (OnSoldItemToVendor != null)
                OnSoldItemToVendor(item, amount, currentVendor);
        }

        public virtual void NotifyItemBoughtFromVendor(InventoryItemBase item, uint amount)
        {
            InventoryManager.instance.lang.vendorBoughtItemFromVendor.Show(item.name, item.description, amount, currentVendor.name, item.buyPrice.GetFormattedString(amount));

            if (audioWhenBoughtItemFromVendor != null)
                InventoryUtility.AudioPlayOneShot(audioWhenBoughtItemFromVendor);

            if (OnBoughtItemFromVendor != null)
                OnBoughtItemFromVendor(item, amount, currentVendor);
        }

        public virtual void NotifyItemBoughtBackFromVendor(InventoryItemBase item, uint amount)
        {
            if (OnBoughtItemBackFromVendor != null)
                OnBoughtItemBackFromVendor(item, amount, currentVendor);
        }

        #endregion

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