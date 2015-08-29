using System.Collections;
using Devdog.InventorySystem.Models;
using UnityEngine;
using Devdog.InventorySystem.UI;

namespace Devdog.InventorySystem
{
    public partial class CraftingWindowBase : ItemCollectionBase
    {
        #region Events

        public delegate void CraftSuccess(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint, InventoryItemBase result);
        public delegate void CraftFailed(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint);
        public delegate void CraftProgress(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint, float progress);
        public delegate void CraftCanceled(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint, float progress);

        public event CraftSuccess OnCraftSuccess;
        public event CraftFailed OnCraftFailed;
        public event CraftProgress OnCraftProgress;
        public event CraftCanceled OnCraftCanceled;

        #endregion


        [Header("Crafting")]
        public int craftingCategoryID = 0;
        public InventoryCraftingCategory craftingCategory
        {
            get
            {
#if UNITY_EDITOR
                if (ItemManager.instance.craftingCategories.Length == 0)
                {
                    //Debug.LogWarning("Crafting window in the scene, but no crafting categories defined.", transform);
                    return null;
                }
#endif

                return ItemManager.instance.craftingCategories[craftingCategoryID];
            }
        }


        public bool cancelCraftingOnWindowClose = true;


        public float currentCraftProgress { get; protected set; }
        protected IEnumerator activeCraft { get; set; }


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

        public InventoryCraftingCategory currentCategory { get; protected set; }
        public InventoryCraftingBlueprint currentBlueprint { get; protected set; }


        [Header("Audio & Visuals")]
        public AudioClip successCraftItem;
        public AudioClip failedCraftItem;
        public AudioClip canceledCraftItem;

        public AnimationClip craftAnimation;
        //public AnimationClip successAnimation;
        //public AnimationClip failedAnimation;
        //public AnimationClip canceledAnimation;


        #region Notifies

        public virtual void NotifyCraftSuccess(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint, InventoryItemBase result)
        {
            InventoryManager.instance.lang.craftedItem.Show(blueprint.itemResult.name, blueprint.itemResult.description);

            if (successCraftItem != null)
                InventoryUtility.AudioPlayOneShot(successCraftItem);

            if (OnCraftSuccess != null)
                OnCraftSuccess(category, blueprint, result);
        }

        public virtual void NotifyCraftFailed(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint)
        {
            InventoryManager.instance.lang.craftingFailed.Show(blueprint.itemResult.name, blueprint.itemResult.description);

            if (failedCraftItem != null)
                InventoryUtility.AudioPlayOneShot(failedCraftItem);

            if (OnCraftFailed != null)
                OnCraftFailed(category, blueprint);
        }

        public virtual void NotifyCraftProgress(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint, float progress)
        {
            currentCraftProgress = progress;

            if (OnCraftProgress != null)
                OnCraftProgress(category, blueprint, progress);
        }

        public virtual void NotifyCraftCanceled(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint, float progress)
        {
            InventoryManager.instance.lang.craftingCanceled.Show(currentBlueprint.itemResult.name, currentBlueprint.itemResult.description);
            activeCraft = null;

            if (canceledCraftItem != null)
                InventoryUtility.AudioPlayOneShot(canceledCraftItem);

            if (OnCraftCanceled != null)
                OnCraftCanceled(currentCategory, currentBlueprint, currentCraftProgress);
        }

        #endregion


        /// <summary>
        /// Does the inventory contain the required items?
        /// </summary>
        /// <param name="blueprint"></param>
        /// <param name="alsoScanBank"></param>
        /// <param name="craftCount"></param>
        /// <returns></returns>
        public virtual bool CanCraftBlueprint(InventoryCraftingBlueprint blueprint, bool alsoScanBank, int craftCount)
        {
            if (InventoryManager.CanRemoveCurrency(blueprint.craftingCost, true, alsoScanBank) == false)
            {
                InventoryManager.instance.lang.userNotEnoughGold.Show(blueprint.itemResult.name, blueprint.itemResult.description, craftCount, blueprint.craftingCost.GetFormattedString(craftCount));
                return false;
            }

            // Can the items be stored in the inventory / designated spot?
            if (currentCategory.forceSaveInCollection != null)
            {
                bool added = currentCategory.forceSaveInCollection.CanAddItem(blueprint.itemResult);
                if (added == false)
                {
                    InventoryManager.instance.lang.collectionFull.Show(blueprint.itemResult.name, blueprint.itemResult.description, currentCategory.forceSaveInCollection.collectionName);
                    return false;
                }
            }
            else
            {
                bool added = InventoryManager.CanAddItem(blueprint.itemResult);
                if (added == false)
                {
                    InventoryManager.instance.lang.collectionFull.Show(blueprint.itemResult.name, blueprint.itemResult.description, "Inventory");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Crafts the item and triggers the coroutine method to handle the crafting itself.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="blueprint"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public virtual bool CraftItem(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint, int amount)
        {
            if (activeCraft != null)
                return false; // Already crafting

            activeCraft = _CraftItem(category, blueprint, amount, blueprint.craftingTimeDuration);
            StartCoroutine(activeCraft);

            return true;
        }


        protected virtual IEnumerator _CraftItem(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint, int amount, float currentCraftTime)
        {
            // Override me!
            return null;
        }

        /// <summary>
        /// Cancels any crafting action that is active. For example when you're crafting an item with a timer, cancel it when you walk away.
        /// </summary>
        public virtual void CancelActiveCraft()
        {
            if (activeCraft != null && cancelCraftingOnWindowClose)
            {
                StopCoroutine(activeCraft);
                NotifyCraftCanceled(currentCategory, currentBlueprint, currentCraftProgress);
            }
        }


        protected virtual void RemoveRequiredCraftItems(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint)
        {
            // Remove items from inventory
            foreach (var item in blueprint.requiredItems)
                InventoryManager.RemoveItem(item.item.ID, (uint)item.amount, category.alsoScanBankForRequiredItems); //  * GetCraftInputFieldAmount()

            // Remove gold
            InventoryManager.RemoveCurrency(blueprint.craftingCost);
        }


        protected virtual bool GiveCraftReward(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint)
        {
            if (blueprint.successChanceFactor >= UnityEngine.Random.value)
            {
                // Store crafted item
                var c = GameObject.Instantiate<InventoryItemBase>(blueprint.itemResult);
                c.currentStackSize = (uint)(blueprint.itemResultCount); //  * GetCraftInputFieldAmount()
                if (category.forceSaveInCollection != null)
                {
                    bool added = category.forceSaveInCollection.AddItem(c);
                    if (added == false)
                        return false;
                }
                else
                {
                    bool added = InventoryManager.AddItem(c);
                    if(added == false)
                        return false;
                }

                NotifyCraftSuccess(category, blueprint, c);
                return true;
            }

            NotifyCraftFailed(category, blueprint);
            return false;
        }

    }
}
