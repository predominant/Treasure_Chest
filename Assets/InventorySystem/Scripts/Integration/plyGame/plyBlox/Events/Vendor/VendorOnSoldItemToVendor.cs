#if PLY_GAME

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using plyBloxKit;
using plyGame;

namespace Devdog.InventorySystem.Integration.plyGame.plyBlox
{
    [plyEvent("Inventory Pro/Vendors", "Vendor OnSoldItemToVendor", Description = @"Called when the user sells an object to a vendor." + "\n\n" +
        "<b>Temp variables:</b>\n\n" + 
        "- item (InventoryItemBase)\n" + 
        "- itemID (int)\n" + 
        "- amount (int)\n" +
        "- vendor (Vendor)")]
    public class VendorOnSoldItemToVendor : plyEvent
    {
        public override void Run()
        {
            base.Run();
        }

        public override System.Type HandlerType()
        {
            // here the Event is linked to the correct handler
            return typeof(VendorEventHandler);
        }
    }
}

#endif