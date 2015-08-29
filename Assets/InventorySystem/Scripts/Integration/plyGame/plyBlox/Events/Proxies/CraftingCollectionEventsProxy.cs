#if PLY_GAME

using System;
using System.Collections.Generic;
using Devdog.InventorySystem.Models;
using plyBloxKit;
using UnityEngine;
using Devdog.InventorySystem.UI;

namespace Devdog.InventorySystem.Integration.plyGame.plyBlox
{

    /// <summary>
    /// Relays all events to plyGame's plyBox
    /// </summary>
    public partial class CraftingCollectionEventsProxy : MonoBehaviour
    {
        private CraftingEventHandler eventHandler { get; set; }
        private CraftingWindowLayoutUI layout { get; set; }
        private CraftingWindowStandardUI standard { get; set; }


        // <inheritdoc />
        public void Start()
        {
            eventHandler = GetComponent<CraftingEventHandler>();
            layout = InventoryManager.instance.craftingLayout;
            standard = InventoryManager.instance.craftingStandard;

            if (layout != null)
            {
                layout.OnCraftSuccess += OnCraftSuccess;
                layout.OnCraftFailed += OnCraftFailed;
                layout.OnCraftProgress += OnCraftProgress;
                layout.OnCraftCanceled += OnCraftCanceled;
            }

            if (standard != null)
            {
                standard.OnCraftSuccess += OnCraftSuccess;
                standard.OnCraftFailed += OnCraftFailed;
                standard.OnCraftProgress += OnCraftProgress;
                standard.OnCraftCanceled += OnCraftCanceled;
            }
        }

        public void OnDestroy()
        {
            if (layout != null)
            {
                layout.OnCraftSuccess -= OnCraftSuccess;
                layout.OnCraftFailed -= OnCraftFailed;
                layout.OnCraftProgress -= OnCraftProgress;
                layout.OnCraftCanceled -= OnCraftCanceled;
            }

            if (standard != null)
            {
                standard.OnCraftSuccess -= OnCraftSuccess;
                standard.OnCraftFailed -= OnCraftFailed;
                standard.OnCraftProgress -= OnCraftProgress;
                standard.OnCraftCanceled -= OnCraftCanceled;
            }
        }


        public void OnCraftSuccess(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint, InventoryItemBase result)
        {
            if (eventHandler != null)
                eventHandler.OnCraftSuccess(category, blueprint, result);
        }

        public void OnCraftFailed(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint)
        {
            if (eventHandler != null)
                eventHandler.OnCraftFailed(category, blueprint);
        }

        public void OnCraftProgress(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint, float progress)
        {
            if (eventHandler != null)
                eventHandler.OnCraftProgress(category, blueprint, progress);
        }

        public void OnCraftCanceled(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint, float progress)
        {
            if (eventHandler != null)
                eventHandler.OnCraftCanceled(category, blueprint, progress);
        }
    }
}

#endif