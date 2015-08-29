using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Devdog.InventorySystem.Dialogs;
using Devdog.InventorySystem.UI;

namespace Devdog.InventorySystem
{
    using Devdog.InventorySystem.Models;

    /// <summary>
    /// Used to trigger item pickup, modify the settings in ObjectTriggererHandler.
    /// </summary>
    [AddComponentMenu("InventorySystem/Triggers/Object triggerer item")]
    [DisallowMultipleComponent]
    //[RequireComponent(typeof(Rigidbody))]
    public partial class ObjectTriggererItem : ObjectTriggererBase
    {

        public delegate void Pickup(InventoryPlayer player);
        /// <summary>
        /// Called when the item is looted / picked up.
        /// </summary>
        public event Pickup OnPickup;


        public override bool triggerMouseClick
        {
            get { return InventorySettingsManager.instance.itemTriggerMouseClick; }
            set { InventorySettingsManager.instance.itemTriggerMouseClick = value; }
        }

        public override KeyCode triggerKeyCode {
            get { return InventorySettingsManager.instance.itemTriggererUseKeyCode; }
            set { InventorySettingsManager.instance.itemTriggererUseKeyCode = value; }
        }

        public override InventoryCursorIcon cursorIcon
        {
            get { return InventorySettingsManager.instance.pickupCursor; }
            set { InventorySettingsManager.instance.pickupCursor = value; }
        }
        public override Sprite uiIcon
        {
            get { return InventorySettingsManager.instance.objectTriggererFPSPickupSprite; }
            set { InventorySettingsManager.instance.objectTriggererFPSPickupSprite = value; }
        }


        public override void NotifyCameInRange(InventoryPlayer player)
        {


        }

        public override void NotifyWentOutOfRange(InventoryPlayer player)
        {


        }

        public override void OnMouseDown()
        {
            if (enabled == false)
                return;

            if (triggerMouseClick && InventoryUIUtility.isHoveringUIElement == false)
            {
                Use();
            }
        }


        public virtual InventoryItemBase GetItem(out bool shouldDestroySource)
        {
            var item = gameObject.GetComponent<InventoryItemBase>();
            if (item != null)
            {
                shouldDestroySource = false;
                return item;
            }

            var holder = gameObject.GetComponent<ObjectTriggererItemHolder>();
            if (holder != null)
            {
                shouldDestroySource = true;
                return holder.item;
            }

            Debug.LogWarning("Trying to pickup item but no suitable handler found!", gameObject);
            shouldDestroySource = false;
            return null;
        }

        public override bool Use(out bool removeSource, bool fireEvents = true)
        {
            return Use(InventoryPlayerManager.instance.currentPlayer, out removeSource, fireEvents);
        }

        public override bool Use(InventoryPlayer player, out bool removeSource, bool fireEvents = true)
        {
            if (inRange)
            {
                removeSource = PickupItem(player, fireEvents);
                if (removeSource)
                {
                    InventoryTriggererManager.instance.currentlyHoveringTriggerer = null;
                }
            }
            else
            {
                bool shouldDestroySource;
                var item = GetItem(out shouldDestroySource);
                if (item != null)
                    InventoryManager.instance.lang.itemCannotBePickedUpToFarAway.Show(item.name, item.description);

                removeSource = false;
            }

            return removeSource;
        }

        public override bool UnUse(bool fireEvents = true)
        {
            return UnUse(InventoryPlayerManager.instance.currentPlayer, fireEvents);
        }

        public override bool UnUse(InventoryPlayer player, bool fireEvents = true)
        {
            // Can't un-use an item.

            return false;
        }

        protected virtual bool PickupItem(InventoryPlayer player, bool fireEvents = true)
        {
            bool shouldDestroySource;
            var item = GetItem(out shouldDestroySource);

            if (item != null)
            {
                uint itemID = item.ID;
                uint itemAmount = item.currentStackSize;

                bool pickedUp = item.PickupItem();
                if (pickedUp)
                {
                    if (fireEvents)
                    {
                        if (OnPickup != null)
                            OnPickup(player);

                        if (player != null)
                            player.NotifyPickedUpItem(itemID, itemAmount);

                        InventoryTriggererManager.instance.NotifyTriggererUsed(this);
                    }

                    if (shouldDestroySource)
                        Destroy(gameObject);
                }

                return pickedUp;
            }

            return false;
        }
    }
}