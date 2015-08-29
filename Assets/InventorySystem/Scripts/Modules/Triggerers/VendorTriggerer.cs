using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Devdog.InventorySystem.Dialogs;

namespace Devdog.InventorySystem
{
    using System.Linq;

    using UnityEngine.Serialization;

    /// <summary>
    /// Represents a vendor that sells / buys something
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(ObjectTriggerer))]
    [RequireComponent(typeof(SelectableObjectInfo))]
    [AddComponentMenu("InventorySystem/Triggers/Vendor triggererHandler")]
    public partial class VendorTriggerer : MonoBehaviour, IObjectTriggerUser, IInventoryItemContainer
    {
        [Header("Vendor")]
        //public string vendorName;
        public bool canSellToVendor;

        [Header("Items")]
        [FormerlySerializedAs("forSale")]
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



        /// <summary>
        /// All prices are multiplied with this value. If you want to make items 10% more expensive in a certain are, or on a certain vendor, use this.
        /// </summary>
        [Range(0.0f, 10.0f)]
        [Header("Buying / Selling")]
        public float buyPriceFactor = 1.0f;

        /// <summary>
        /// All sell prices are multiplied with this value. If you want to make items 10% more expensive in a certain are, or on a certain vendor, use this.
        /// </summary>
        [Range(0.0f, 10.0f)]
        public float sellPriceFactor = 1.0f;

        [Range(1, 999)]
        public uint maxBuyItemCount = 999;
        public bool removeItemAfterPurchase = false;

        /// <summary>
        /// Can items be bought back after they're sold?
        /// </summary>
        [Header("Buy back")]
        public bool enableBuyBack = true;

        /// <summary>
        /// How expensive is the item to buy back. item.sellPrice * buyBackCostFactor = the final price to buy back an item.
        /// </summary>
        [Range(0.0f, 10.0f)]
        public float buyBackPriceFactor = 1.0f;

        /// <summary>
        /// Max number of items in buy back window.
        /// </summary>
        public uint maxBuyBackItemSlotsCount = 10;

        /// <summary>
        /// Is buyback shared between all vendors with the same category?
        /// </summary>
        public bool buyBackIsShared = false;

        /// <summary>
        /// The category this vendor belongs to, used for sharing the buyback.
        /// Shared buyback is shared based on the vendor categeory, all vendors with the same category will have the same buy back items.
        /// </summary>
        [Tooltip("Shared buyback is shared based on the vendor categeory, all vendors with the same category will have the same buy back items.")]
        public string vendorCategory = "Default";

        /// <summary>
        /// Generator used to generate a random set of items for this vendor
        /// </summary>
        public IItemGenerator itemGenerator { get; set; }

        protected VendorUI vendorUI;
        protected Animator animator;


        /// <summary>
        /// List of items that can be bought back, not static and is vendor specific.
        /// Only used if buyBackIsShared = false
        /// if buyBackIsShared = true the static buyBackDist will be used.
        /// </summary>
        [HideInInspector]
        [NonSerialized]
        public List<InventoryItemBase> buyBackList = new List<InventoryItemBase>();

        /// <summary>
        /// A dictionary of all items that have been sold to vendors. It's static so items are shared by catergory.
        /// Key: name / category of the buyback
        /// Value: list of all items that can be bought back.
        /// </summary>
        public static Dictionary<string, List<InventoryItemBase>> buyBackDict;

        [NonSerialized]
        protected Transform buyBackParent;
        
        [NonSerialized]
        protected ObjectTriggerer triggerer;


        public void Awake()
        {
            vendorUI = InventoryManager.instance.vendor;
            if (vendorUI == null)
            {
                Debug.LogWarning("No vendor UI found, yet there's a vendor in the scene.", transform);
                return;
            }

            animator = GetComponent<Animator>();
            triggerer = GetComponent<ObjectTriggerer>();
            triggerer.window = vendorUI.window;
            triggerer.handleWindowDirectly = false; // We're in charge now :)

            buyBackDict = new Dictionary<string, List<InventoryItemBase>>();

            buyBackParent = new GameObject("Vendor_BuyBackContainer").transform;
            buyBackParent.SetParent(InventoryManager.instance.collectionObjectsParent);


            triggerer.OnTriggerUse += Use;
            triggerer.OnTriggerUnUse += UnUse;
        }


        protected virtual void Use(InventoryPlayer player)
        {
            // Set items
            vendorUI.SetItems(items, false, false);
            vendorUI.currentVendor = this;
            vendorUI.OnRemovedItem += vendorUI_OnRemovedItem;

            vendorUI.window.Show();
        }

        protected virtual void UnUse(InventoryPlayer player)
        {
            vendorUI.currentVendor = null;
            vendorUI.OnRemovedItem -= vendorUI_OnRemovedItem;

            vendorUI.window.Hide();
        }

        protected void vendorUI_OnRemovedItem(InventoryItemBase item, uint itemID, uint slot, uint amount)
        {
            //forSale[slot] = null;
        }


        /// <summary>
        /// Sell an item to this vendor.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        public virtual void SellItemToVendor(InventoryItemBase item)
        {
            uint max = InventoryManager.GetItemCount(item.ID, false);

            if (CanSellItemToVendor(item, max) == false)
            {
                InventoryManager.instance.lang.vendorCannotSellItem.Show(item.name, item.description, max);
                return;
            }

            InventoryManager.instance.buySellDialog.ShowDialog(InventoryManager.instance.vendor.window.transform, "Sell " + name, "Are you sure you want to sell " + name, 1, (int)max, item, ItemBuySellDialogAction.Selling, this, (amount) =>
            {
                // Sell items
                SellItemToVendorNow(item, (uint)amount);

            }, (amount) =>
            {
                // Canceled

            });
        }

        /// <summary>
        /// Sell item now to this vendor. The vendor doesn't sell the object, the user sells to this vendor.
        /// Note that this does not show any UI or warnings and immediately handles the action.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public virtual bool SellItemToVendorNow(InventoryItemBase item, uint amount)
        {
            if (CanSellItemToVendor(item, amount) == false)
                return false;

            if (enableBuyBack)
            {
                var copy = GameObject.Instantiate<InventoryItemBase>(item);
                copy.currentStackSize = (uint)amount;
                copy.transform.SetParent(buyBackParent.transform);

                if (buyBackIsShared)
                {
                    if (buyBackDict.ContainsKey(vendorCategory) == false)
                        buyBackDict.Add(vendorCategory, new List<InventoryItemBase>());

                    buyBackDict[vendorCategory].Add(copy);
                    if (buyBackDict[vendorCategory].Count > maxBuyBackItemSlotsCount)
                    {
                        Destroy(buyBackDict[vendorCategory][0].gameObject);
                        buyBackDict[vendorCategory].RemoveAt(0);
                    }
                }
                else
                {
                    // Not shared drop in object array
                    buyBackList.Add(copy);
                    if (buyBackList.Count > maxBuyBackItemSlotsCount)
                    {
                        Destroy(buyBackList[0].gameObject);
                        buyBackList.RemoveAt(0);
                    }
                }
            }

            InventoryManager.AddCurrency(GetSellPrice(item, amount), item.sellPrice.currency.ID);
            InventoryManager.RemoveItem(item.ID, amount, false);

            vendorUI.NotifyItemSoldToVendor(item, amount);
            return true;
        }

        public virtual bool CanSellItemToVendor(InventoryItemBase item, uint amount)
        {
            if (canSellToVendor == false)
                return false;

            if (item.isSellable == false)
                return false;

            return true;
        }

        public virtual bool CanBuyItemBackFromVendor(InventoryItemBase item, uint amount)
        {
            float totalCost = GetBuyBackPrice(item, amount);

            if (GetBuyBackItemCount(item.ID) < amount)
                return false; // Something wen't wrong, we don't have that many items for buy-back.

            if (InventoryManager.CanRemoveCurrency(totalCost, item.sellPrice.currency.ID, true) == false)
            {
                string totalCostString = item.sellPrice.GetFormattedString(amount * buyBackPriceFactor);
                var c = InventoryManager.instance.inventory.GetCurrencyByID(item.sellPrice.currency.ID);

                InventoryManager.instance.lang.userNotEnoughGold.Show(item.name, item.description, amount, totalCostString, c != null ? c.GetFormattedString() : "");
                return false; // Not enough gold for this many
            }
            
            if (CanAddItemsToInventory(item, amount) == false)
            {
                InventoryManager.instance.lang.collectionFull.Show(item.name, item.description, "Inventory");
                return false;
            }

            return true;
        }

        public virtual bool CanBuyItemFromVendor(InventoryItemBase item, uint amount)
        {
            float totalCost = GetBuyPrice(item, amount);

            if (InventoryManager.CanRemoveCurrency(totalCost, item.buyPrice.currency.ID, true) == false)
            {
                string totalCostString = item.buyPrice.GetFormattedString(amount * buyPriceFactor);
                var c = InventoryManager.instance.inventory.GetCurrencyByID(item.buyPrice.currency.ID);

                InventoryManager.instance.lang.userNotEnoughGold.Show(item.name, item.description, amount, totalCostString, c != null ? c.GetFormattedString() : "");
                return false; // Not enough gold for this many
            }

            if (CanAddItemsToInventory(item, amount) == false)
            {
                InventoryManager.instance.lang.collectionFull.Show(item.name, item.description, "Inventory");
                return false;
            }

            return true;
        }

        public virtual void BuyItemFromVendor(InventoryItemBase item, bool isBuyBack)
        {
            ItemBuySellDialogAction action = ItemBuySellDialogAction.Buying;
            uint maxAmount = maxBuyItemCount;
            if (isBuyBack)
            {
                action = ItemBuySellDialogAction.BuyingBack;
                maxAmount = item.currentStackSize;
            }

            InventoryManager.instance.buySellDialog.ShowDialog(InventoryManager.instance.vendor.window.transform, "Buy item " + item.name, "How many items do you want to buy?", 1, (int)maxAmount, item, action, this, (amount) =>
            {
                // Clicked yes!
                if(isBuyBack)
                    BuyItemBackFromVendorNow(item, (uint)amount);
                else
                    BuyItemFromVendorNow(item, (uint)amount);

            }, (amount) =>
            {
                // Clicked cancel...

            });
        }


        public virtual bool BuyItemBackFromVendorNow(InventoryItemBase item, uint amount)
        {
            if (CanBuyItemBackFromVendor(item, amount) == false)
                return false;

            if (buyBackIsShared)
            {
                buyBackDict[vendorCategory][(int)item.index].currentStackSize -= amount;
                if (buyBackDict[vendorCategory][(int)item.index].currentStackSize < 1)
                {
                    Destroy(buyBackDict[vendorCategory][(int)item.index].gameObject);
                    buyBackDict[vendorCategory].RemoveAt((int)item.index);
                }
            }
            else
            {
                buyBackList[(int)item.index].currentStackSize -= amount;
                if (buyBackList[(int)item.index].currentStackSize < 1)
                {
                    Destroy(buyBackList[(int)item.index].gameObject);
                    buyBackList.RemoveAt((int)item.index);
                }
            }

            var c1 = GameObject.Instantiate<InventoryItemBase>(item);
            c1.currentStackSize = amount;

            InventoryManager.RemoveCurrency(GetBuyBackPrice(item, amount), item.sellPrice.currency.ID);
            InventoryManager.AddItem(c1); // Will handle unstacking if the stack size goes out of bounds.

            vendorUI.NotifyItemBoughtBackFromVendor(item, amount);
            return true;
        }

        public int GetBuyBackItemCount(uint itemID)
        {
            if (buyBackIsShared)
            {
                return buyBackDict[vendorCategory].Sum(o => o.ID == itemID ? (int)o.currentStackSize : 0);
            }

            return buyBackList.Sum(o => o.ID == itemID ? (int)o.currentStackSize : 0);
        }


        /// <summary>
        /// Buy an item from this vendor, this does not display a dialog, but moves the item directly to the inventory.
        /// Note that this does not show any UI or warnings and immediately handles the action.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        public virtual bool BuyItemFromVendorNow(InventoryItemBase item, uint amount)
        {
            if (CanBuyItemFromVendor(item, amount) == false)
                return false;


            var c1 = GameObject.Instantiate<InventoryItemBase>(item);
            c1.currentStackSize = amount;

            InventoryManager.RemoveCurrency(GetBuyPrice(item, amount), item.buyPrice.currency.ID);
            InventoryManager.AddItem(c1); // Will handle unstacking if the stack size goes out of bounds.
            
            if (removeItemAfterPurchase)
            {
                item.itemCollection.SetItem(item.index, null);
                item.itemCollection.NotifyItemRemoved(item, item.ID, item.index, item.currentStackSize);
                item.itemCollection[item.index].Repaint();
                //Destroy(item.gameObject);
            }


            vendorUI.NotifyItemBoughtFromVendor(item, amount);

            return true;
        }


        /// <summary>
        /// Can this item * amount be added to the inventory, is there room?
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <returns>True if items can be placed, false is not.</returns>
        protected virtual bool CanAddItemsToInventory(InventoryItemBase item, uint amount)
        {
            uint originalStackSize = item.currentStackSize;

            item.currentStackSize = amount;
            bool can = InventoryManager.CanAddItem(item);
            item.currentStackSize = originalStackSize; // Reset

            return can;
        }

        public virtual float GetBuyBackPrice(InventoryItemBase item, uint amount)
        {
            return item.sellPrice.amount * amount * buyBackPriceFactor;
        }
        public virtual float GetBuyPrice(InventoryItemBase item, uint amount)
        {
            return item.buyPrice.amount * amount * buyPriceFactor;
        }
        public virtual float GetSellPrice(InventoryItemBase item, uint amount)
        {
            return item.sellPrice.amount * amount * sellPriceFactor;
        }
    }
}