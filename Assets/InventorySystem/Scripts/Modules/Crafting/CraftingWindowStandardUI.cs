using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.Models;
using Devdog.InventorySystem.UI;

using Random = UnityEngine.Random;

namespace Devdog.InventorySystem
{
    [AddComponentMenu("InventorySystem/Windows/Crafting standard")]
    [RequireComponent(typeof(UIWindow))]
    public partial class CraftingWindowStandardUI : CraftingWindowBase
    {
        //public InventoryCraftingCategory defaultCategory
        //{
        //    get { return ItemManager.instance.craftingCategories[defaultCategoryID]; }
        //}


        //[Header("Crafting")]
        //public int defaultCategoryID;

        /// <summary>
        /// Crafting category title
        /// </summary>
        [Header("General UI references")]
        public UnityEngine.UI.Text currentCategoryTitle;

        /// <summary>
        /// Crafting category description
        /// </summary>
        public UnityEngine.UI.Text currentCategoryDescription;

        [InventoryRequired]
        public RectTransform blueprintsContainer;


        [Header("Blueprint prefabs")]
        public InventoryCraftingCategoryUI blueprintCategoryPrefab;
        
        /// <summary>
        /// The button used to select the prefab the user wishes to craft.
        /// </summary>
        [InventoryRequired] public InventoryCraftingBlueprintUI blueprintButtonPrefab;

        /// <summary>
        /// A single required item to be shown in the UI.
        /// </summary>
        [InventoryRequired] public InventoryUIItemWrapper blueprintRequiredItemPrefab;

        #region Crafting item page

        [Header("Craft blueprint UI References")]
        public InventoryUIItemWrapper blueprintIcon;
        public UnityEngine.UI.Text blueprintTitle;
        public UnityEngine.UI.Text blueprintDescription;

        [InventoryRequired]
        public RectTransform blueprintRequiredItemsContainer;
        public UnityEngine.UI.Slider blueprintCraftProgressSlider;


        public InventoryCurrencyUIGroup blueprintCurrencyUI;
        //public UnityEngine.UI.Text blueprintCraftCostText;

        /// <summary>
        /// Craft the selected item button
        /// </summary>
        [InventoryRequired]
        public UnityEngine.UI.Button blueprintCraftButton;
        public UnityEngine.UI.Button blueprintMinCraftButton;
        public UnityEngine.UI.InputField blueprintCraftAmountInput;
        public UnityEngine.UI.Button blueprintPlusCraftButton;

        #endregion

        [Header("UI window pages")]
        public UIWindowPage noBlueprintSelectedPage;
        public UIWindowPage blueprintCraftPage;


        [Header("Audio & Visuals")]
        public Color itemsAvailableColor = Color.white;
        public Color itemsNotAvailableColor = Color.red;



        #region Pools

        [NonSerialized]
        protected InventoryPool<InventoryCraftingCategoryUI> categoryPool;
        
        [NonSerialized]
        protected InventoryPool<InventoryCraftingBlueprintUI> blueprintPool;

        [NonSerialized]
        protected InventoryPool<InventoryUIItemWrapper> blueprintRequiredItemsPool;

        #endregion
        

        public override void Awake()
        {
            //base.Awake();

            if (blueprintCategoryPrefab != null)
                categoryPool = new InventoryPool<InventoryCraftingCategoryUI>(blueprintCategoryPrefab, 16);
            
#if UNITY_EDITOR
            if (blueprintButtonPrefab == null)
                Debug.LogWarning("Blueprint button prefab is empty in CraftingWindowStandardUI", gameObject);

            if (blueprintRequiredItemPrefab == null)
                Debug.LogWarning("Blueprint required item prefab is empty in CraftingWindowStandardUI", gameObject);

            if(blueprintCraftButton == null)
                Debug.LogWarning("Blueprint craft button is requred", gameObject);
#endif

            blueprintPool = new InventoryPool<InventoryCraftingBlueprintUI>(blueprintButtonPrefab, 128);
            blueprintRequiredItemsPool = new InventoryPool<InventoryUIItemWrapper>(blueprintRequiredItemPrefab, 8);

            if (craftingCategoryID >= 0 && craftingCategoryID <= ItemManager.instance.craftingCategories.Length - 1)
                currentCategory = craftingCategory;

            if(blueprintMinCraftButton != null)
            {
                blueprintMinCraftButton.onClick.AddListener(() =>
                {
                    if(Input.GetKey(KeyCode.LeftShift))
                        blueprintCraftAmountInput.text = (GetCraftInputFieldAmount() - 10).ToString();
                    else
                        blueprintCraftAmountInput.text = (GetCraftInputFieldAmount() - 1).ToString();

                    ValidateCraftInputFieldAmount();
                });
            }
            if(blueprintPlusCraftButton != null)
            {
                blueprintPlusCraftButton.onClick.AddListener(() =>
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                        blueprintCraftAmountInput.text = (GetCraftInputFieldAmount() + 10).ToString();
                    else
                        blueprintCraftAmountInput.text = (GetCraftInputFieldAmount() + 1).ToString();

                    ValidateCraftInputFieldAmount();
                });
            }

            blueprintCraftButton.onClick.AddListener(() => CraftItem(currentCategory, currentBlueprint, GetCraftInputFieldAmount()));
        }

        public override void Start()
        {
            base.Start();


            window.OnShow += () =>
            {
                if (currentCategory != null)
                    SetCraftingCategory(currentCategory);

                if (currentBlueprint != null)
                    SetBlueprint(currentBlueprint);
            };

            window.OnHide += CancelActiveCraft;


            foreach (var col in InventoryManager.GetLootToCollections())
            {
                col.OnAddedItem += (items, amount, cameFromCollection) =>
                {
                    if (currentBlueprint != null)
                        SetBlueprint(currentBlueprint);
                };
                col.OnRemovedItem += (InventoryItemBase item, uint itemID, uint slot, uint amount) =>
                {
                    if (currentBlueprint != null)
                        SetBlueprint(currentBlueprint);
                };
                col.OnDroppedItem += (InventoryItemBase item, uint slot, GameObject droppedObj) =>
                {
                    CancelActiveCraft(); // If the user drops something.

                    if (currentBlueprint != null)
                        SetBlueprint(currentBlueprint);
                };
            }

            InventoryManager.instance.inventory.OnCurrencyChanged += (float before, InventoryCurrencyLookup lookup) =>
            {
                if (currentBlueprint != null)
                    SetBlueprint(currentBlueprint);
            };
        }

        protected virtual int GetCraftInputFieldAmount()
        {
            if(blueprintCraftAmountInput != null)
                return int.Parse(blueprintCraftAmountInput.text);

            return 1;
        }

        protected virtual void ValidateCraftInputFieldAmount()
        {
            int amount = GetCraftInputFieldAmount();
            if (amount < 1)
                amount = 1;
            else if (amount > 999)
                amount = 999;

            blueprintCraftAmountInput.text = amount.ToString();
        }


        public virtual void SetCraftingCategory(InventoryCraftingCategory category)
        {
            categoryPool.DestroyAll();
            blueprintPool.DestroyAll();
            currentCategory = category;
            if (blueprintCraftAmountInput != null)
                blueprintCraftAmountInput.text = "1"; // Reset
            
            CancelActiveCraft(); // Just in case

            if(currentCategoryTitle != null)
                currentCategoryTitle.text = currentCategory.name;
        
            if (currentCategoryDescription != null)
                currentCategoryDescription.text = currentCategory.description;

            if (noBlueprintSelectedPage != null)
                noBlueprintSelectedPage.Show();

            if (blueprintCraftPage == null && currentCategory.blueprints.Length > 0)
                SetBlueprint(currentCategory.blueprints[0]); // Select first blueprint

            int lastItemCategory = -1;
            foreach (var b in currentCategory.blueprints)
            {
                if (b.playerLearnedBlueprint == false)
                    continue;

                var blueprintObj = blueprintPool.Get();
                blueprintObj.transform.SetParent(blueprintsContainer);
                InventoryUtility.ResetTransform(blueprintObj.transform);
                blueprintObj.Set(b);

                if (blueprintCategoryPrefab != null)
                {
                    if (lastItemCategory != b.itemResult.category.ID)
                    {
                        lastItemCategory = (int)b.itemResult.category.ID;

                        var uiCategory = categoryPool.Get();
                        uiCategory.Set(b.itemResult.category.name);

                        uiCategory.transform.SetParent(blueprintsContainer);
                        blueprintObj.transform.SetParent(uiCategory.container);

                        InventoryUtility.ResetTransform(uiCategory.transform);
                        InventoryUtility.ResetTransform(blueprintObj.transform);
                    }
                }

                var bTemp = b; // Store capture list, etc.
                blueprintObj.button.onClick.AddListener(() =>
                {
                    currentBlueprint = bTemp;
                    SetBlueprint(currentBlueprint);
                    CancelActiveCraft(); // Just in case         

                    if (blueprintCraftPage != null && blueprintCraftPage.isVisible == false)
                        blueprintCraftPage.Show();
                });
            }
        }

        protected virtual void SetBlueprint(InventoryCraftingBlueprint blueprint)
        {
            if (window.isVisible == false)
                return;

            // Set all the details for the blueprint.
            if (blueprintTitle != null)
                blueprintTitle.text = blueprint.name;

            if (blueprintDescription != null)
                blueprintDescription.text = blueprint.description;

            if (blueprintIcon != null)
            {
                blueprintIcon.item = blueprint.itemResult;
                blueprintIcon.item.currentStackSize = (uint)blueprint.itemResultCount;
                blueprintIcon.Repaint();
                blueprintIcon.item.currentStackSize = 1; // Reset
            }

            if (blueprintCraftProgressSlider)
                blueprintCraftProgressSlider.value = 0.0f; // Reset

            if (blueprintCurrencyUI != null)
            {
                //if (InventoryManager.CanRemoveCurrency(blueprint.craftingCost, true, craftingCategory.alsoScanBankForRequiredItems))
                //    blueprintCraftCostText.color = itemsAvailableColor;
                //else
                //    blueprintCraftCostText.color = itemsNotAvailableColor;

                blueprintCurrencyUI.Repaint(blueprint.craftingCost);
            }


            blueprintRequiredItemsPool.DestroyAll();
            foreach (var item in blueprint.requiredItems)
            {
                var ui = blueprintRequiredItemsPool.Get();
                item.item.currentStackSize = (uint)item.amount;
                ui.transform.SetParent(blueprintRequiredItemsContainer);
                InventoryUtility.ResetTransform(ui.transform);

                ui.item = item.item;
                if (InventoryManager.GetItemCount(item.item.ID, currentCategory.alsoScanBankForRequiredItems) >= item.amount)
                    ui.icon.color = itemsAvailableColor;
                else
                    ui.icon.color = itemsNotAvailableColor;

                ui.Repaint();
                item.item.currentStackSize = 1; // Reset
            }
        }

        /// <summary>
        /// Called when an item is being crafted.
        /// </summary>
        /// <param name="progress"></param>
        public override void NotifyCraftProgress(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint, float progress)
        {
            base.NotifyCraftProgress(category, blueprint, progress);

            if (blueprintCraftProgressSlider != null)
                blueprintCraftProgressSlider.value = progress;
        }


        protected override IEnumerator _CraftItem(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint, int amount, float currentCraftTime)
        {
            bool canCraft = CanCraftBlueprint(blueprint, category.alsoScanBankForRequiredItems, amount);
            if (canCraft)
            {
                float counter = currentCraftTime;
                while (true)
                {
                    yield return new WaitForSeconds(Time.deltaTime); // Update loop
                    counter -= Time.deltaTime;
                    NotifyCraftProgress(category, blueprint, 1.0f - Mathf.Clamp01(counter / currentCraftTime));

                    if (counter <= 0.0f)
                        break;
                }


                RemoveRequiredCraftItems(category, blueprint);
                GiveCraftReward(category, blueprint);

                amount--;
                if (amount > 0)
                {
                    if (blueprintCraftAmountInput != null)
                        blueprintCraftAmountInput.text = amount.ToString();

                    activeCraft = _CraftItem(category, blueprint, amount, Mathf.Clamp(currentCraftTime / blueprint.craftingTimeSpeedupFactor, 0.0f, blueprint.craftingTimeDuration));
                    StartCoroutine(activeCraft);
                }
                else
                {
                    activeCraft = null; // All done
                }
            }
            else
            {
                //StopCoroutine(activeCraft);
                activeCraft = null;
            }
        }


        public override bool CanCraftBlueprint(InventoryCraftingBlueprint blueprint, bool alsoScanBank, int craftCount)
        {
            bool can = base.CanCraftBlueprint(blueprint, alsoScanBank, craftCount);
            if (can == false)
                return false;

            foreach (var item in blueprint.requiredItems)
            {
                uint count = InventoryManager.GetItemCount(item.item.ID, alsoScanBank);
                if (count < item.amount * craftCount)
                {
                    InventoryManager.instance.lang.craftingDontHaveRequiredItems.Show(item.item.name, item.item.description, blueprint.name);
                    return false;
                }
            }

            return true;
        }
    }
}