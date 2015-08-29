using System;
using UnityEngine;

namespace Devdog.InventorySystem.Models
{
    /// <summary>
    /// Used by the reflection approach. All items marked with [InventoryStat] are considered for stat calculations (if enabled in the editor).
    /// For ease of use it's easiest to use the item properties editor.
    /// </summary>
    [System.Serializable]
    public partial class InventoryEquipStat
    {
        /// <summary>
        /// Visual name (Strength, Agility, etc)
        /// </summary>
        public string name;

        /// <summary>
        /// Category of this stat?
        /// </summary>
        public string category;

        /// <summary>
        /// assemblyQualifiedTypeName
        /// </summary>
        public string typeName;

        /// <summary>
        /// Used for reflection to get the value.
        /// </summary>
        public string fieldInfoName;

        /// <summary>
        /// Only use this inside the editor.
        /// </summary>
        public string fieldInfoNameVisual;

        /// <summary>
        /// Show this stat?
        /// </summary>
        public bool show = false;
    }
}