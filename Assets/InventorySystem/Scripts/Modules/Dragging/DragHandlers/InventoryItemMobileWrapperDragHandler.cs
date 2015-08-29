using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Devdog.InventorySystem.UI
{
    public class InventoryItemMobileWrapperDragHandler : InventoryItemStandardWrapperDragHandler
    {
        public InventoryItemMobileWrapperDragHandler(int priority)
            : base(priority)
        {
            
        }

        public override bool CanUse(InventoryUIItemWrapperBase wrapper, PointerEventData eventData)
        {
            if (Application.isMobilePlatform)
            {
                if (eventData.pointerId != -1 && Input.touchCount > 0)
                {
                    return true; // Mobile drag
                }
            }

            return false;
        }
    }
}