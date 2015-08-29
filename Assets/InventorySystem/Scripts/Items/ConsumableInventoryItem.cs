using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem
{
    /// <summary>
    /// Used to represent items that can be used by the player, and once used reduce 1 in stack size. This includes potions, food, scrolls, etc.
    /// </summary>
    public partial class ConsumableInventoryItem : InventoryItemBase
    {
        /// <summary>
        /// When the item is used, play this sound.
        /// </summary>
        public AudioClip audioClipWhenUsed;


        public override LinkedList<InventoryItemInfoRow[]> GetInfo()
        {
            var basic = base.GetInfo();
            //basic.AddAfter(basic.First, new InfoBoxUI.Row[]{
            //    new InfoBoxUI.Row("Restore health", restoreHealth.ToString(), Color.green, Color.green),
            //    new InfoBoxUI.Row("Restore mana", restoreMana.ToString(), Color.green, Color.green)
            //});


            return basic;
        }

        public override void NotifyItemUsed(uint amount, bool alsoNotifyCollection)
        {
            base.NotifyItemUsed(amount, alsoNotifyCollection);

            SetItemProperties(InventoryPlayerManager.instance.currentPlayer, InventoryItemUtility.SetItemPropertiesAction.Use);
        }


        public override int Use()
        {
            int used = base.Use();
            if(used < 0)
                return used;

            // Do something with item
            currentStackSize--; // Remove 1
            
            NotifyItemUsed(1, true);

            if (audioClipWhenUsed != null)
                InventoryUtility.AudioPlayOneShot(audioClipWhenUsed);
        
            return 1; // 1 item used
        }

        protected virtual void SetItemProperties(InventoryPlayer player, InventoryItemUtility.SetItemPropertiesAction action)
        {
            InventoryItemUtility.SetItemProperties(player, properties, action);
        }
    }
}