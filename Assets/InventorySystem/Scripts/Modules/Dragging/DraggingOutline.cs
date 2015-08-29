using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Devdog.InventorySystem.UI
{
    [RequireComponent(typeof(Outline))]
    [RequireComponent(typeof(InventoryUIItemWrapperBase))]
    public partial class DraggingOutline : MonoBehaviour
    {
        protected Outline outline { get; set; }

        protected InventoryUIItemWrapperBase wrapper { get; private set; }
        protected InventoryEquippableField equippableField { get; private set; }

        public virtual void Start()
        {
            outline = GetComponent<Outline>();
            outline.enabled = false;

            wrapper = GetComponent<InventoryUIItemWrapperBase>();
            equippableField = GetComponent<InventoryEquippableField>();

            InventoryUIDragUtility.OnStartDragging += InventoryUiDragUtilityOnOnStartDragging;
            InventoryUIDragUtility.OnEndDragging += InventoryUiDragUtilityOnOnEndDragging;
        }

        protected virtual void SetOutlineValues()
        {
            if(outline != null)
                outline.enabled = true;
        }

        protected virtual void RemoveOutlineValues()
        {
            if(outline != null)
                outline.enabled = false;
        }

        protected virtual void InventoryUiDragUtilityOnOnStartDragging(InventoryUIDragLookup dragLookup, InventoryUIItemWrapperBase dragging, PointerEventData eventData)
        {
            if (dragging.item != null)
            {
                if (wrapper != null)
                {
                    // Equippable character field
                    if (equippableField != null)
                    {
                        var equippable = dragging.item as EquippableInventoryItem;
                        if (equippable != null)
                        {
                            if (equippableField.equipTypes.Any(o => o == equippable.equipType))
                            {
                                SetOutlineValues();
                            }
                        }
                    }
                    else
                    {
                        if (wrapper.itemCollection != null)
                        {
                            var canSet = wrapper.itemCollection.filters.IsItemAbidingFilters(dragging.item);
                            if (canSet)
                            {
                                SetOutlineValues();
                            }
                        }
                    }
                }
            }
        }


        protected virtual void InventoryUiDragUtilityOnOnEndDragging(InventoryUIDragLookup dragLookup, InventoryUIItemWrapperBase dragging, PointerEventData eventData)
        {
            RemoveOutlineValues();
        }

    }
}
