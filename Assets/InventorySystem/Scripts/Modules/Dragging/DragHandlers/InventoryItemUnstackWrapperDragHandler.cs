using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.Models;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Devdog.InventorySystem.UI
{
    public class InventoryItemUnstackWrapperDragHandler : InventoryItemStandardWrapperDragHandler
    {
        //protected InventoryUIDragLookup dragLookup { get; set; }


        public InventoryItemUnstackWrapperDragHandler(int priority)
            : base(priority)
        {
            dragLookup = new InventoryUIDragLookup();
        }


        public override bool CanUse(InventoryUIItemWrapperBase wrapper, PointerEventData eventData)
        {
            return InventorySettingsManager.instance.unstackKeys.AllPressed(eventData, InventoryActionInput.EventType.All);
        }

        public override InventoryUIDragLookup OnBeginDrag(InventoryUIItemWrapperBase wrapper, PointerEventData eventData)
        {
            return base.OnBeginDrag(wrapper, eventData);
        }

        public override InventoryUIDragLookup OnDrag(PointerEventData eventData)
        {
            return base.OnDrag(eventData);
        }

        public override InventoryUIDragLookup OnEndDrag(InventoryUIItemWrapperBase wrapper, PointerEventData eventData)
        {
            return base.OnEndDrag(wrapper, eventData);
        }
    }
}