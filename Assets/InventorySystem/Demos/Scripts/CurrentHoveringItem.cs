using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.UI;
using UnityEngine;

namespace Devdog.InventorySystem.Demo
{
    public class CurrentHoveringItem : MonoBehaviour
    {

        public void Update()
        {
            if (InventoryUIUtility.currentlyHoveringWrapper != null && InventoryUIUtility.currentlyHoveringWrapper.item != null)
            {
                Debug.Log("Currently hovering: " + InventoryUIUtility.currentlyHoveringWrapper.item.name);
            }
        }

    }
}
