using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.Models;
using Devdog.InventorySystem.UI;

namespace Devdog.InventorySystem
{
    [AddComponentMenu("InventorySystem/Windows/Crafting layout")]
    [RequireComponent(typeof(UIWindow))]
    public partial class CraftingWindowLayoutUI : CraftingWindowBase
    {


        //[Header("Behavior")] // Moved to custom editor
        public Dictionary<uint, uint> currentBlueprintItemsDict { get; protected set; }


        [SerializeField]
        protected uint _initialCollectionSize = 9;
        public override uint initialCollectionSize
        {
            get
            {
                return _initialCollectionSize;
            }
        }

        #region UI Elements

        [Header("UI elements")]
        public UnityEngine.UI.Text blueprintTitle;
        public UnityEngine.UI.Text blueprintDescription;

        public InventoryCurrencyUIGroup blueprintCraftCost;

        public UnityEngine.UI.Slider blueprintCraftProgressSlider;

        public InventoryUIItemWrapperBase blueprintItemResult;
        public UnityEngine.UI.Button blueprintCraftButton;
    
        #endregion

        [Header("Audio & Visuals")]
        public Color enoughGoldColor;
        public Color notEnoughGoldColor;

        
        public override void Awake()
        {
            base.Awake();
            currentBlueprintItemsDict = new Dictionary<uint, uint>(9);

            if(craftingCategory != null)
                SetCraftingCategory(craftingCategory);
        
            blueprintCraftButton.onClick.AddListener(() =>
            {
                if(currentBlueprint != null)
                    CraftItem(currentCategory, currentBlueprint, 1);
            });
        }

        public override void Start()
        {
            base.Start();


            window.OnHide += CancelActiveCraft;
            window.OnShow += () =>
            {
                ValidateReferences();
                ValidateBlueprint();
            };

            foreach (var col in InventoryManager.GetLootToCollections())
            {
                col.OnRemovedItem += (InventoryItemBase item, uint itemID, uint slot, uint amount) =>
                {
                    if (window.isVisible == false)
                        return;

                    ValidateReferences();
                    ValidateBlueprint();
                };
                col.OnAddedItem += (itemArr, amount, cameFromCollection) =>
                {
                    if (window.isVisible == false)
                        return;

                    foreach (var i in items)
                    {
                        i.Repaint();
                    }

                    ValidateBlueprint();
                };
                col.OnUsedItem += (InventoryItemBase item, uint itemID, uint slot, uint amount) =>
                {
                    if (window.isVisible == false)
                        return;

                    foreach (var i in items)
                    {
                        if (i.item != null && i.item.ID == itemID)
                            i.Repaint();
                    }

                    if (currentBlueprintItemsDict.ContainsKey(itemID))
                    {
                        CancelActiveCraft(); // Used an item that we're using in crafting.
                        ValidateBlueprint();
                    }
                };
            }
        }


        protected void ValidateReferences()
        {
            foreach (var item in items)
            {
                if (item.item != null)
                {
                    var i = item.item;
                    // If the item was dropped remove it from the references window
                    if (i.itemCollection == null)
                    {
                        item.item = null;
                        continue;
                    }

                    // If the original item no longer exists, scan the inventories for another item, can be null
                    if (i.itemCollection[i.index].item == null)
                        item.item = InventoryManager.Find(i.ID, currentCategory.alsoScanBankForRequiredItems);

                    uint count = InventoryManager.GetItemCount(i.ID, currentCategory.alsoScanBankForRequiredItems);
                    if (count == 0)
                        item.item = null;
                }

                item.Repaint();
            }
        }

        public virtual void SetCraftingCategory(InventoryCraftingCategory category)
        {
            currentCategory = category;
            CancelActiveCraft(); // Just in case

#if UNITY_EDITOR
            if (currentCategory == null)
            {
                Debug.LogWarning("Received a null object when trying to set the crafting category.", transform);
                return;
            }
#endif

            if(category.cols * category.rows > items.Length)
            {
                AddSlots((uint)(category.cols * category.rows - items.Length)); // Increase
            }
            else if (category.cols * category.rows < items.Length)
            {
                RemoveSlots((uint)(items.Length - category.cols * category.rows)); // Decrease
            }
        }

        protected virtual void SetBlueprint(InventoryCraftingBlueprint blueprint)
        {
            currentBlueprint = blueprint;

            // Set all the details for the blueprint.
            if (blueprintTitle != null)
                blueprintTitle.text = blueprint.name;

            if (blueprintDescription != null)
                blueprintDescription.text = blueprint.description;

            SetBlueprintResult(blueprint);

            if (blueprintCraftCost != null)
            {
                blueprintCraftCost.Repaint(blueprint.craftingCost);
            }
        }


        protected virtual void SetBlueprintResult(InventoryCraftingBlueprint blueprint)
        {
            if (blueprintItemResult != null)
            {
                if (blueprint != null)
                {
                    blueprintItemResult.item = blueprint.itemResult;
                    blueprintItemResult.item.currentStackSize = (uint)blueprint.itemResultCount;
                    blueprintItemResult.Repaint();
                    blueprintItemResult.item.currentStackSize = 1; // Reset
                }
                else
                {
                    bool nullBefore = blueprintItemResult.item == null;
                    blueprintItemResult.item = null;
                
                    if(nullBefore == false)
                        blueprintItemResult.Repaint();
                }
            }
        }


        /// <summary>
        /// Tries to find a blueprint based on the current layout / items inside the UI item wrappers (items).
        /// </summary>
        /// <param name="cat"></param>
        /// <returns>Returns blueprint if found one, null if not.</returns>
        public virtual InventoryCraftingBlueprint GetBlueprintFromCurrentLayout(InventoryCraftingCategory cat)
        {
            if(items.Length != cat.cols * cat.rows)
            {
                Debug.LogWarning("Updating blueprint but blueprint layout cols/rows don't match the collection");
            }

            int totalItemCountInLayout = 0; // Nr of items inside the UI wrappers.
            foreach (var item in items)
            {
                if (item.item != null)
                    totalItemCountInLayout++;
            }

            foreach (var b in cat.blueprints)
            {
                foreach (var a in b.blueprintLayouts)
                {
                    if (a.enabled)
                    {
                        var hasItems = new Dictionary<uint, uint>(); // ItemID, amount
                        //var requiredItems = new Dictionary<uint, uint>(); // ItemID, amount
                        currentBlueprintItemsDict.Clear();

                        int counter = 0; // Item index counter
                        int shouldHaveCount = 0; // Amount we should have..
                        int hasCount = 0; // How many slots in our layout
                        bool shouldBreak = false;
                        foreach (var r in a.rows)
                        {
                            if (shouldBreak)
                                break;

                            foreach (var c in r.columns)
                            {
                                if (shouldBreak)
                                    break;

                                if (c.item != null && c.amount > 0)
                                {
                                    if (currentBlueprintItemsDict.ContainsKey(c.item.ID) == false)
                                        currentBlueprintItemsDict.Add(c.item.ID, 0);

                                    currentBlueprintItemsDict[c.item.ID] += (uint)c.amount;
                                    shouldHaveCount++;

                                    if (items[counter].item != null)
                                    {
                                        if (items[counter].item.ID != c.item.ID)
                                        {
                                            shouldBreak = true;
                                            break; // Item in the wrong place...
                                        }

                                        if(hasItems.ContainsKey(c.item.ID) == false)
                                        {
                                            hasItems.Add(c.item.ID, InventoryManager.GetItemCount(c.item.ID, cat.alsoScanBankForRequiredItems));
                                        }

                                        hasCount++;
                                    }
                                    else if(items[counter].item == null && c != null)
                                    {
                                        shouldBreak = true;
                                        break;
                                    }
                                }

                                counter++;
                            }
                        }

                        if (shouldBreak)
                            break; // Onto the next one

                        // Filled slots test
                        if (totalItemCountInLayout != hasCount || shouldHaveCount != hasCount)
                            break;

                        // Check count
                        foreach (var item in currentBlueprintItemsDict)
                        {
                            if (hasItems.ContainsKey(item.Key) == false || hasItems[item.Key] < item.Value)
                                shouldBreak = true;
                        }

                        if (shouldBreak)
                            break;

                        return b;
                    }
                }
            }

            return null; // Nothing found
        }

        /// <summary>
        /// Check if the bluerint is still valid and craftable.
        /// </summary>
        protected virtual void ValidateBlueprint()
        {
            SetBlueprintResult(null); // Clear the old, check again
            var blueprint = GetBlueprintFromCurrentLayout(currentCategory);
            if (blueprint != null)
            {
                // Found something to craft!
                SetBlueprint(blueprint);
            }
            else
            {
                currentBlueprint = null;
                currentBlueprintItemsDict.Clear();
            }
        }

        public override bool SetItem(uint slot, InventoryItemBase item)
        {
            bool set = base.SetItem(slot, item);
            if(set)
            {
                ValidateBlueprint();
            }

            return set;
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

        protected override void RemoveRequiredCraftItems(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint)
        {
            // Remove items from inventory
            uint[] keys = currentBlueprintItemsDict.Keys.ToArray();
            uint[] vals = currentBlueprintItemsDict.Values.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                InventoryManager.RemoveItem(keys[i], vals[i], category.alsoScanBankForRequiredItems); //  * GetCraftInputFieldAmount()
            }

            // Remove gold
            InventoryManager.RemoveCurrency(blueprint.craftingCost);
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
                currentBlueprintItemsDict.Clear();
                ValidateReferences();

                if (amount > 0)
                {
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


        /// <summary>
        /// Does the inventory contain the required items?
        /// </summary>
        /// <param name="blueprint"></param>
        /// <param name="alsoScanBank"></param>
        /// <param name="craftCount"></param>
        /// <returns></returns>
        public override bool CanCraftBlueprint(InventoryCraftingBlueprint blueprint, bool alsoScanBank, int craftCount)
        {
            bool can = base.CanCraftBlueprint(blueprint, alsoScanBank, craftCount);
            if (can == false)
                return false;

            // No blueprint found
            if (GetBlueprintFromCurrentLayout(currentCategory) == null)
                return false;

            return true;
        }
    }
}