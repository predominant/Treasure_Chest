using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem.UI
{
    /// <summary>
    /// A single row in the infobox.
    /// </summary>
    public partial class InfoBoxRowUI : MonoBehaviour, IPoolableObject
    {
        public UnityEngine.UI.Text title;
        public UnityEngine.UI.Text message;


        public void Reset()
        {
            // Item has no specific states, no need to reset
        }
    }
}