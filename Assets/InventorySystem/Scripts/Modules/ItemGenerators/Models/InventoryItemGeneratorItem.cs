using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.InventorySystem.Models
{
    public partial class InventoryItemGeneratorItem
    {
        /// <summary>
        /// The item we want to submit
        /// </summary>
        public InventoryItemBase item;
    
        /// <summary>
        /// The chance this item will be chosen.
        /// Value from 0.0f to 1.0f.
        /// 0 = no chance
        /// 1.0f = 100% chance (filters still affect the object)
        /// </summary>
        public float chanceFactor = 1.0f;


        public InventoryItemGeneratorItem(InventoryItemBase item, float chanceFactor)
        {
            this.item = item;
            this.chanceFactor = chanceFactor;
        }
    }
}