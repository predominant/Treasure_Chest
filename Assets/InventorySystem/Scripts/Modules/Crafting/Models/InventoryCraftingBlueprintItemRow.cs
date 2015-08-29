using UnityEngine;
using System;
using System.Collections.Generic;

namespace Devdog.InventorySystem.Models
{
    [System.Serializable]
    public partial class InventoryCraftingBlueprintItemRow
    {

        /// <summary>
        /// The item in this row
        /// </summary>
        public InventoryItemBase item;

        /// <summary>
        /// The amount of items required.
        /// </summary>
        public int amount = 1;

    }
}