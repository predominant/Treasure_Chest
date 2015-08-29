#if PLY_GAME

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using plyBloxKit;
using plyGame;

namespace Devdog.InventorySystem.Integration.plyGame.plyBlox
{
    [plyEvent("Inventory Pro/Inventories", "Inventories OnUnstackedItemCollectionFull", Description = "Called when an item is unstacked in this collection, but it was full and therefore the action was aborted. <b>Note that it can only be used on the player.</b>" + "\n\n" +
        "<b>Temp variables:</b>\n\n" +
        "- fromSlot (int)\n" +
        "- toSlot (int)\n" +
        "- amount (int)")]
    public class InventoriesOnUnstackedItemCollectionFull : plyEvent
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