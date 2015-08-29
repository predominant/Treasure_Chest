using UnityEngine;
using System.Collections;
using Devdog.InventorySystem.Models;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Devdog.InventorySystem
{
    [AddComponentMenu("InventorySystem/UI Wrappers/UI Wrapper static")]
    public partial class InventoryUIItemWrapperStatic : InventoryUIItemWrapper
    {
        public override void Update()
        {
            //base.Update();
        }

        public override void OnDisable()
        {

        }

        #region Button handler UI events

        public override void OnPointerUp(PointerEventData eventData)
        {

        }

        public override void OnPointerDown(PointerEventData eventData)
        {

        }

        public override bool OnTap(PointerEventData eventData, InventoryActionInput.EventType eventType)
        {
            return false;
        }

        public override bool OnDoubleTap(PointerEventData eventData, InventoryActionInput.EventType eventType)
        {
            return false;
        }

        public override bool OnLongTap(PointerEventData eventData, InventoryActionInput.EventType eventType)
        {
            return false;
        }

        public virtual void PickupItem()
        {
        
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {

        }

        public override void OnDrag(PointerEventData eventData)
        {

        }

        public override void OnEndDrag(PointerEventData eventData)
        {

        }

        #endregion


        public override void Repaint()
        {
            base.Repaint();

            if (item != null)
            {
                //itemName.text = item.name;
                if (itemName != null && item.rarity != null)
                    itemName.color = item.rarity.color;
            }
            else
            {
                //itemName.text = string.Empty;
            }
        }

        public override void RepaintCooldown()
        {
            //base.RepaintCooldown();
        }
    }
}