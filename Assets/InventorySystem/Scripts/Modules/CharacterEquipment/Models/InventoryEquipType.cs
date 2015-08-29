using UnityEngine;
using System.Collections;

namespace Devdog.InventorySystem.Models
{
    [System.Serializable]
    public partial class InventoryEquipType
    {
        /// <summary>
        /// Unique ID
        /// </summary>
        [HideInInspector]
        public int ID;

        /// <summary>
        /// Name of this equip type (example: 2 handed sword, bow, right wheel, saddle)
        /// </summary>
        public string name;

        /// <summary>
        /// Disallow other types to be set while this one is active.
        /// For example when equipping a greatsword, you might want to un-equip the shield.
        /// Indices to avoid serialization loop
        /// ItemManager.instance.equipTypes[blockTypes[i]]
        /// </summary>
        public int[] blockTypes = new int[0];
    }
}