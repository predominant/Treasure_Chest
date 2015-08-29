using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Devdog.InventorySystem.UI
{
    public class InventoryItemStandardWrapperDragHandler : InventoryItemWrapperDragHandlerBase
    {


        public InventoryItemStandardWrapperDragHandler(int priority)
            : base(priority)
        {
            dragLookup = new InventoryUIDragLookup();
        }


        public override bool CanUse(InventoryUIItemWrapperBase wrapper, PointerEventData eventData)
        {
            return eventData.button == PointerEventData.InputButton.Left;
        }

        public override InventoryUIDragLookup OnBeginDrag(InventoryUIItemWrapperBase wrapper, PointerEventData eventData)
        {
            currentlyDragging = wrapper;
            dragLookup.Reset();

            dragLookup.startIndex = (int) wrapper.index;
            dragLookup.startItemCollection = wrapper.itemCollection;

            return dragLookup;
        }

        public override InventoryUIDragLookup OnDrag(PointerEventData eventData)
        {
            if (currentlyDragging == null)
                return dragLookup;

            currentlyDragging.transform.position = new Vector3(eventData.position.x, eventData.position.y, 0.0f);
            return dragLookup;
        }

        public override InventoryUIDragLookup OnEndDrag(InventoryUIItemWrapperBase wrapper, PointerEventData eventData)
        {
            if (currentlyDragging == null)
                return dragLookup;

            if (wrapper != null)
            {
                dragLookup.endIndex = (int)wrapper.index;
                dragLookup.endItemCollection = wrapper.itemCollection;
            }

            UnityEngine.Object.Destroy(currentlyDragging.gameObject); // No longer need it, destroy 
            currentlyDragging = null;

            return dragLookup;
        }
    }
}