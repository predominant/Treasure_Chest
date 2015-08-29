#if PLY_GAME

using System;
using System.Collections.Generic;
using System.Linq;
using plyBloxKit;
using UnityEngine;

namespace Devdog.InventorySystem.Integration.plyGame.plyBlox
{
    [plyBlock("Inventory Pro", "Collections", "Move item to inventory", BlockType.Action)]
    public class MoveItemToInventory : plyBlock
    {
        [plyBlockField("Item", ShowName = true, ShowValue = true, DefaultObject = typeof(InventoryUIItemWrapper), EmptyValueName = "-error-", SubName = "Wrapper (slot)", Description = "The wrapper that contains the item that you want to move to the inventory.")]
        public InventoryUIItemWrapper wrapper;
        

        public override void Created()
        {
            blockIsValid = (wrapper != null);
            
            if(blockIsValid == false)
                Log(LogType.Error, "Wrapper has to be set.");
        }

        public override BlockReturn Run(BlockReturn param)
        {
            if (wrapper.item != null)
            {
                InventoryManager.AddItemAndRemove(wrapper.item);
            }

            return BlockReturn.OK;
        }
    }
}

#endif