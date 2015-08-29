#if PLY_GAME

using System;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.Models;
using plyBloxKit;

namespace Devdog.InventorySystem.Integration.plyGame.plyBlox
{
    public class CraftingEventHandler : plyEventHandler
    {
        public List<plyEvent> onCraftingSuccess = new List<plyEvent>();
        public List<plyEvent> onCraftingFailed = new List<plyEvent>();
        public List<plyEvent> onCraftingProgress = new List<plyEvent>();
        public List<plyEvent> onCraftingCanceled = new List<plyEvent>();


        public override void StateChanged()
        {
            onCraftingSuccess.Clear();
            onCraftingFailed.Clear();
            onCraftingProgress.Clear();
            onCraftingCanceled.Clear();
        }

        public override void AddEvent(plyEvent e)
        {
            if (e.uniqueIdent.Equals("Crafting OnCraftingSuccess"))
                onCraftingSuccess.Add(e);

            if (e.uniqueIdent.Equals("Crafting OnCraftingFailed"))
                onCraftingFailed.Add(e);

            if (e.uniqueIdent.Equals("Crafting OnCraftingProgress"))
                onCraftingProgress.Add(e);

            if (e.uniqueIdent.Equals("Crafting OnCraftingCanceled"))
                onCraftingCanceled.Add(e);
        }

        public override void CheckEvents()
        {
            enabled = onCraftingSuccess.Count > 0
                      || onCraftingFailed.Count > 0
                      || onCraftingProgress.Count > 0
                      || onCraftingCanceled.Count > 0;
        }



        public void OnCraftSuccess(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint, InventoryItemBase result)
        {
            if (onCraftingSuccess.Count > 0)
            {
                RunEvents(onCraftingSuccess,
                    new plyEventArg("item", result),
                    new plyEventArg("itemID", (int)result.ID),
                    new plyEventArg("category", category),
                    new plyEventArg("categoryID", (int)category.ID),
                    new plyEventArg("blueprint", blueprint),
                    new plyEventArg("blueprintID", (int)blueprint.ID));
            }
        }

        public void OnCraftFailed(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint)
        {
            if (onCraftingFailed.Count > 0)
            {
                RunEvents(onCraftingFailed,
                    new plyEventArg("itemID", (int)blueprint.itemResult.ID),
                    new plyEventArg("category", category),
                    new plyEventArg("categoryID", (int)category.ID),
                    new plyEventArg("blueprint", blueprint),
                    new plyEventArg("blueprintID", (int)blueprint.ID));
            }
        }

        public void OnCraftProgress(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint, float progress)
        {
            if (onCraftingProgress.Count > 0)
            {
                RunEvents(onCraftingProgress,
                    new plyEventArg("itemID", (int)blueprint.itemResult.ID),
                    new plyEventArg("category", category),
                    new plyEventArg("categoryID", (int)category.ID),
                    new plyEventArg("blueprint", blueprint),
                    new plyEventArg("blueprintID", (int)blueprint.ID),
                    new plyEventArg("progress", progress));
            }
        }

        public void OnCraftCanceled(InventoryCraftingCategory category, InventoryCraftingBlueprint blueprint, float progress)
        {
            if (onCraftingCanceled.Count > 0)
            {
                RunEvents(onCraftingCanceled,
                    new plyEventArg("itemID", (int)blueprint.itemResult.ID),
                    new plyEventArg("category", category),
                    new plyEventArg("categoryID", (int)category.ID),
                    new plyEventArg("blueprint", blueprint),
                    new plyEventArg("blueprintID", (int)blueprint.ID),
                    new plyEventArg("progress", progress));
            }
        }
    }
}

#endif