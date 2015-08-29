using UnityEngine;
using System;

namespace Devdog.InventorySystem.Models
{
    /// <summary>
    /// Used to define categories for items, categories can have a global cooldown, this can be usefull to cooldown all potions for example.
    /// </summary>
    [System.Serializable]
    public partial class InventoryItemCategory
    {
        public uint ID;
        public string name;

        /// <summary>
        /// If you don't want a cooldown leave it at 0.0
        /// </summary>
        [Range(0,999)]
        public float cooldownTime;
    
        [HideInInspector]
        [NonSerialized]
        public float lastUsageTime;


        /// <summary>
        /// Limit a category to specific types, for example potions -> consumables.
        /// </summary>
        [HideInInspector]
        public string[] _onlyAllowTypes = new string[0];
    }
}