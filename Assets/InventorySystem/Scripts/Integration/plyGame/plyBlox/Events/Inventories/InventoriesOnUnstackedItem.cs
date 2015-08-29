#if PLY_GAME

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using plyBloxKit;
using plyGame;

namespace Devdog.InventorySystem.Integration.plyGame.plyBlox
{
    [plyEvent("Inventory Pro/Inventories", "Inventories OnUnstackedItem", Description = "Called when an item was unstacked in this collection. <b>Note that it can only be used on the player.</b>" + "\n\n" +
        "<b>Temp variables:</b>\n\n" +
        "- itemID (int)\n" +
        "- fromSlot (int)\n" +
        "- amount (ItemCollectionBase)")]
    public class InventoriesOnUnstackedItem : plyEvent
    {
        public override void Run()
        {
            base.Run();
        }

        public override System.Type HandlerType()
        {
            // here the Event is linked to the correct handler
            return typeof(InventoriesEventHandler);
        }
    }
}

#endif