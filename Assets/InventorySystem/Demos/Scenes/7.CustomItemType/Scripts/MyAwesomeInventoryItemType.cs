using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Devdog.InventorySystem;

namespace Devdog.InventorySystem.Demo
{
    public class MyAwesomeInventoryItemType : InventoryItemBase
    {
        public float failFactor = 0.5f; // 0.0f is always success, 1.0f is always failure, 0.5f is 50% chance.

        public override int Use()
        {
            int used = base.Use();
            if (used == -1)
                return -1;

            // Do something specific...

            // 50% chance.
            if (Random.value > failFactor)
            {
                currentStackSize--;
                NotifyItemUsed(1, true);
                return 1; // No need to worry about anything else, the stack will be removed if no objects are left.
            }

            NotifyItemUsed(0, true);
            return 0; // 1, the item was used, but no items were used (stack decrease) in the process
        }


        /// <summary>
        /// Override the GetInfo() method. This method returns a list of info groups.
        /// A InfoBoxUI.Row is a group of text
        /// Each InfoBoxUI.Row is seperated using the seperator defined in the InfoBoxUI UI element.
        /// </summary>
        /// <returns></returns>
        public override LinkedList<InventoryItemInfoRow[]> GetInfo()
        {
            var list = base.GetInfo();

            list.AddFirst(new InventoryItemInfoRow[]{
                    new InventoryItemInfoRow("Fail chance", (failFactor * 100).ToString())
                });

            return list;
        }	
    }   
}
