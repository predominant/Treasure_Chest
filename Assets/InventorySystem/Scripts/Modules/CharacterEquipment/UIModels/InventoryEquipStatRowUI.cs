using UnityEngine;
using System.Collections;
using Devdog.InventorySystem.Models;
using UnityEngine.UI;

namespace Devdog.InventorySystem.UI
{
    /// <summary>
    /// Used to define a row of stats.
    /// </summary>
    public partial class InventoryEquipStatRowUI : MonoBehaviour, IPoolableObject
    {
        /// <summary>
        /// Name of the stat
        /// </summary>
        public UnityEngine.UI.Text statName;

        /// <summary>
        /// Status result
        /// </summary>
        public UnityEngine.UI.Text stat;


        public virtual void Repaint(string name, string stat)
        {
            this.statName.text = name;
            this.stat.text = stat;
        }

        public void Reset()
        {
            // Item has no specific states, no need to reset
        }
    }
}