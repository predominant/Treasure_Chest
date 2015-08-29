using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Devdog.InventorySystem.Models;
using Devdog.InventorySystem.UI;

namespace Devdog.InventorySystem
{
    /// <summary>
    /// A physical representation of a crafting station.
    /// </summary>
    [AddComponentMenu("InventorySystem/Triggers/Crafting triggererHandler")]
    [RequireComponent(typeof(ObjectTriggerer))]
    public class CraftingTriggerer : MonoBehaviour, IObjectTriggerUser
    {
        public int craftingCategoryID = 0; // What category can we craft from?
        protected InventoryCraftingCategory category
        {
            get
            {
                return ItemManager.instance.craftingCategories[craftingCategoryID];
            }
        }

        [NonSerialized]
        protected UIWindow window;
        protected static CraftingTriggerer currentCraftingStation;

        [NonSerialized]
        protected ObjectTriggerer triggerer;

        public void Awake()
        {
            if (InventoryManager.instance.craftingStandard == null)
            {
                Debug.LogWarning("Crafting triggerer in scene, but no crafting window found", transform);
                return;
            }

            window = InventoryManager.instance.craftingStandard.window;

            triggerer = GetComponent<ObjectTriggerer>();
            triggerer.window = window;
            triggerer.handleWindowDirectly = false; // We're in charge now :)

            if (triggerer.window == null)
            {
                Debug.LogWarning("Crafting triggerer created but no CraftingStandardUI found in scene, or not set in managers.", transform);
                return;
            }

            window.OnHide += () =>
            {
                currentCraftingStation = null;
            };

            triggerer.OnTriggerUse += (player) =>
            {
                window.Toggle();

                if (window.isVisible)
                {
                    currentCraftingStation = this;
                    InventoryManager.instance.craftingStandard.SetCraftingCategory(category);
                }
            };
            triggerer.OnTriggerUnUse += (player) =>
            {
                if (currentCraftingStation == this)
                    window.Hide();
            };
        }
    }
}