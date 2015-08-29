using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.Models;
using UnityEngine;

namespace Devdog.InventorySystem
{
    //[RequireComponent(typeof(LootableObject))]
    [AddComponentMenu("InventorySystem/Other/Inventory item container generator")]
    public partial class InventoryItemContainerGenerator : MonoBehaviour
    {
        public InventoryItemGeneratorFilterGroup[] filterGroups = new InventoryItemGeneratorFilterGroup[0];

        public IInventoryItemContainer container { get; protected set; }

        public bool generateAtGameStart = true;

        public int minAmountTotal = 2;
        public int maxAmountTotal = 5;

        public FilterGroupsItemGenerator generator { get; protected set; }

        public void Awake()
        {
            container = (IInventoryItemContainer)GetComponent(typeof(IInventoryItemContainer));
            
            generator = new FilterGroupsItemGenerator(filterGroups);
            generator.SetItems(ItemManager.instance.items);

            if (generateAtGameStart)
            {
                container.items = generator.Generate(minAmountTotal, maxAmountTotal, true); // Create instances is required to get stack size to work (Can't change stacksize on prefab)
            }
        }
    }
}
