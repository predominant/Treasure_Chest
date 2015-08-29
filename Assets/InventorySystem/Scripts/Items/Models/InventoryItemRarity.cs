using UnityEngine;
using System.Collections;

namespace Devdog.InventorySystem.Models
{
    /// <summary>
    /// Used to define rarity of items.
    /// </summary>
    [System.Serializable]
    public partial class InventoryItemRarity
    {
        public uint ID;
        public string name;
        public Color color = Color.white;

        /// <summary>
        /// The item that is used when dropping something, leave null to use the object model itself.
        /// </summary>
        [Tooltip("The item that is used when dropping something, leave null to use the object model itself.")]
        public GameObject dropObject;


        public InventoryItemRarity()
        {

        }
        public InventoryItemRarity(uint id, string name, Color color, GameObject dropObject)
        {
            this.ID = id;
            this.name = name;
            this.color = color;
            this.dropObject = dropObject;
        }
    }
}