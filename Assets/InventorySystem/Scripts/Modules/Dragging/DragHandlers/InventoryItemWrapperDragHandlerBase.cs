using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.UI;
using UnityEngine.EventSystems;

namespace Devdog.InventorySystem.UI
{
    public abstract class InventoryItemWrapperDragHandlerBase
    {
        public int priority { get; protected set; }
        public InventoryUIItemWrapperBase currentlyDragging { get; protected set; }
        public InventoryUIDragLookup dragLookup { get; protected set; }


        protected InventoryItemWrapperDragHandlerBase(int priority)
        {
            this.priority = priority;
            this.dragLookup = new InventoryUIDragLookup();
        }
        
        /// <summary>
        /// Can this handler be used to drag an item? Return true if allowed to use, false if not.
        /// </summary>
        public abstract bool CanUse(InventoryUIItemWrapperBase wrapper, PointerEventData eventData);

        public abstract InventoryUIDragLookup OnBeginDrag(InventoryUIItemWrapperBase wrapperToDrag, PointerEventData eventData);
        public abstract InventoryUIDragLookup OnDrag(PointerEventData eventData);
        public abstract InventoryUIDragLookup OnEndDrag(InventoryUIItemWrapperBase currentlyHoveringWrapper, PointerEventData eventData);
    }
}
