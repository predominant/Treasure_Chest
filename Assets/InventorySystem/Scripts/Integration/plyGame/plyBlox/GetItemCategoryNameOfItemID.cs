#if PLY_GAME

using System;
using System.Collections.Generic;
using System.Linq;
using plyBloxKit;
using UnityEngine;

namespace Devdog.InventorySystem.Integration.plyGame.plyBlox
{
    [plyBlock("Inventory Pro", "Items", "Get category name of itemID", BlockType.Variable, Order = 4, ShowName = "Get category name of item by ID",
        ReturnValueString = "Return - category name (string)", ReturnValueType = typeof(String_Value),
        CustomStyle = "plyBlox_VarYellowDark", Description = "Returns the item's category name.")]
    public class GetItemCategoryNameOfItemID : String_Value
    {
        [plyBlockField("Item ID", ShowName = true, ShowValue = true, DefaultObject = typeof(String_Value), Description = "The item ID")]
        public String_Value itemID;

        public override void Created()
        {
            int result;
            blockIsValid = int.TryParse(itemID.value, out result);
        }

        public override BlockReturn Run(BlockReturn param)
        {
            Debug.Log("Looking for : " + int.Parse(itemID.value));
            value = ItemManager.instance.items[int.Parse(itemID.value)].category.name;
            return BlockReturn.OK;
        }
    }
}

#endif