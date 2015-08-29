using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Devdog.InventorySystem.Demo
{
    public partial class EquippableEventTester : MonoBehaviour
    {
        public void Awake()
        {
            var item = GetComponent<EquippableInventoryItem>();

            item.OnUsedItem += amount =>
            {
                Debug.Log("Used item " + item.name);
            };
            item.OnEquipped += to =>
            {
                Debug.Log("Equipped item " + item.name);
            };
            item.OnDroppedItem += position =>
            {
                Debug.Log("Dropped item " + item.name);
            };
            item.OnUnEquipped += () =>
            {
                Debug.Log("On UnEquipped item " + item.name);
            };

        }
    }
}
