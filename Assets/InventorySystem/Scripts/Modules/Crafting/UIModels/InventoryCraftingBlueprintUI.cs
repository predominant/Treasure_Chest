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
    public partial class InventoryCraftingBlueprintUI : MonoBehaviour, IPoolableObject
    {
        [SerializeField]
        protected UnityEngine.UI.Text blueprintName;

        [SerializeField]
        protected UnityEngine.UI.Text blueprintDescription;

        [InventoryRequired]
        public UnityEngine.UI.Button button;
    
        public void Set(InventoryCraftingBlueprint blueprint)
        {
            if(blueprintName != null)
                blueprintName.text = blueprint.name;

            if (blueprintDescription != null)
                blueprintDescription.text = blueprint.description;
        }

        public void Reset()
        {
            button.onClick.RemoveAllListeners();
            // Item has no specific states, no need to reset
        }
    }
}