using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Devdog.InventorySystem
{
    using Devdog.InventorySystem.Models;

    [AddComponentMenu("InventorySystem/UI Wrappers/UI Wrapper Vendor")]
    public partial class InventoryUIItemWrapperVendor : InventoryUIItemWrapper
    {
        public UnityEngine.UI.Text buyPrice;

        public Color affordableColor = Color.white;
        public Color notAffordableColor = Color.red;

        private VendorUI _vendor;
        public VendorUI vendor
        {
            get
            {
                // In a property because on Awake() is fired when instantiated (before it's parented)...

                if (_vendor == null)
                {
                    var vendors = GetComponentsInParent<VendorUI>(true); // GetComponentInParent (single), doesn't handle in-active objects.
                    if(vendors.Length > 0)
                        _vendor = vendors[0];
                }

                if (_vendor == null)
                    Debug.LogWarning("No VendorUI found in parent of InventoryUIItemWrapperVendor! You can only use the *WrapperVendor in a vendor collection.", transform);

                return _vendor;
            }
        }


        public bool isInBuyBack
        {
            get
            {
                return gameObject.GetComponentsInParent<VendorUIBuyBack>(true).Length > 0;
            }
        }


        public static bool hideWhenEmpty = true;


        public override void Awake()
        {
            base.Awake();
            //isInBuyBack = gameObject.GetComponentsInParent<VendorUIBuyBack>(true).Length > 0;
        }

        public override void Update()
        {
            //base.Update();
        }


        #region Button handler UI events

   
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (pointerDownOnUIElement == null)
                return;

            var canvas = gameObject.GetComponentInParent<Canvas>();
            if (canvas.renderMode != RenderMode.WorldSpace)
            {
                if (screenSpaceRect.Contains(eventData.position) == false)
                    return;
            }
            
            if (InventorySettingsManager.instance.useContextMenu)
            {
                base.OnPointerDown(eventData);
                return;
            }

            if (item != null)
                vendor.currentVendor.BuyItemFromVendor(item, isInBuyBack);
        }

        public override void TriggerContextMenu()
        {
            //base.TriggerContextMenu();

            var contextMenu = InventoryManager.instance.contextMenu;
            contextMenu.ClearMenuOptions();
            contextMenu.AddMenuOption("Buy", item, (i) =>
            {
                vendor.currentVendor.BuyItemFromVendor(i, isInBuyBack);
            });

            contextMenu.window.Show();
        }



        #endregion


        public override void Repaint()
        {
            base.Repaint();
        
            if (item != null && vendor != null)
            {
                if (hideWhenEmpty)
                    gameObject.SetActive(true);

                //itemName.text = item.name;
                if(item.rarity != null)
                    itemName.color = item.rarity.color;

                if (vendor.currentVendor != null)
                {
                    float finalPrice = 0.0f;
                    InventoryCurrencyLookup currency;

                    if (isInBuyBack)
                    {
                        finalPrice = vendor.currentVendor.GetBuyBackPrice(item, 1);
                        currency = item.sellPrice;
                    }
                    else
                    {
                        finalPrice = vendor.currentVendor.GetBuyPrice(item, 1);
                        currency = item.buyPrice;
                    }

                    buyPrice.text = currency.GetFormattedString();

                    if (InventoryManager.CanRemoveCurrency(finalPrice, currency.currency.ID, true))
                        buyPrice.color = affordableColor;
                    else
                        buyPrice.color = notAffordableColor;
                }
            }
            else
            {
                //itemName.text = string.Empty;
                buyPrice.text = string.Empty;
            
                if (hideWhenEmpty)
                    gameObject.SetActive(false);
            }
        }

        public override void RepaintCooldown()
        {
            //base.RepaintCooldown();
        }
    }
}