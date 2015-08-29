#if PLY_GAME

using System;
using System.Collections.Generic;
using System.Linq;
using plyBloxKit;
using UnityEngine;

namespace Devdog.InventorySystem.Integration.plyGame.plyBlox
{
    [plyBlock("Inventory Pro", "Items", "Add gold to inventory", BlockType.Action, Description = "Adds gold to the inventory.")]
    public class AddGoldToInventory : plyBlock
    {
        [plyBlockField("Amount", ShowName = true, ShowValue = true, DefaultObject = typeof(Int_Value), SubName = "Item amount", Description = "The amount of items you wish to add to the collection")]
        public Int_Value amount;

        [plyBlockField("CurrencyID", ShowName = true, ShowValue = true, DefaultObject = typeof(Int_Value), SubName = "Currency ID", Description = "The ID of the currency you wish to add to this collection (See main editor for ID's).")]
        public Int_Value currencyID;

        public override void Created()
        {
            blockIsValid = true;
        }

        public override BlockReturn Run(BlockReturn param)
        {
            InventoryManager.AddCurrency(amount.value, (uint)currencyID.value);
            return BlockReturn.OK;
        }
    }
}

#endif