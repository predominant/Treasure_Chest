#if PLY_GAME

using System;
using System.Collections.Generic;
using System.Linq;
using plyBloxKit;
using UnityEngine;

namespace Devdog.InventorySystem.Integration.plyGame.plyBlox
{
    [plyBlock("Inventory Pro", "Items", "Drop item now", BlockType.Action, Description = "Drop the item without any UI.")]
    public class DropItemNow : plyBlock
    {
        [plyBlockField("Item to drop now", ShowName = true, ShowValue = true, DefaultObject = typeof(InventoryItemBase), EmptyValueName = "-this-", SubName = "InventorySystem item", Description = "The item you wist to drop")]
        public InventoryItemBase item;

        public override void Created()
        {
            blockIsValid = item != null;
            
        }


        public override BlockReturn Run(BlockReturn param)
        {
            var d = item.Drop(InventoryPlayerManager.instance.currentPlayer.transform.position);
            if (d != null)
            {
                item.itemCollection[item.index].Repaint();
                return BlockReturn.OK;
            }

            return BlockReturn.Error; // Item not dropped??
        }
    }
}

#endif