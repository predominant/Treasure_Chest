using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Devdog.InventorySystem
{
    using Devdog.InventorySystem.Models;

    public abstract class ObjectTriggererBase : MonoBehaviour
    {

        /// <summary>
        /// When the item is clicked, should it trigger?
        /// </summary>
        public abstract bool triggerMouseClick { get; set; }

        /// <summary>
        /// When the item is hovered over (center of screen) and a certain key is tapped, should it trigger?
        /// </summary>
        public abstract KeyCode triggerKeyCode { get; set; }

        /// <summary>
        /// The cursor icon used to visualize this triggerer.
        /// </summary>
        public abstract InventoryCursorIcon cursorIcon { get; set; }

        /// <summary>
        /// The icon used inside the UI
        /// </summary>
        public abstract Sprite uiIcon { get; set; }

        /// <summary>
        /// Is this triggerer currently active (used)
        /// </summary>
        public bool isActive { get; protected set; }

        /// <summary>
        /// When true the triggerer will be usable, when false it won't respond to any actions.
        /// </summary>
        public new bool enabled { get; set; }



        public virtual bool inRange
        {
            get
            {
                if (InventoryPlayerManager.instance.currentPlayer == null)
                    return false;

                return InventoryPlayerManager.instance.currentPlayer.rangeHelper.triggerersInRange.Contains(this);

                //return Vector3.SqrMagnitude(InventoryPlayerManager.instance.currentPlayer.transform.position - transform.position) < InventorySettingsManager.instance.useObjectDistance * InventorySettingsManager.instance.useObjectDistance;
            }
        }



        public virtual void Awake()
        {
            enabled = true;
        }



        //public abstract void OnMouseEnter();
        //public abstract void OnMouseExit();
        public abstract void OnMouseDown();


        public virtual bool Toggle(bool fireEvents = true)
        {
            bool removeSource;
            return Toggle(InventoryPlayerManager.instance.currentPlayer, out removeSource, fireEvents);
        }

        public virtual bool Toggle(InventoryPlayer player, out bool removeSource, bool fireEvents = true)
        {
            if (isActive)
            {
                removeSource = false;
                return UnUse(player, fireEvents);
            }
            else
            {
                return Use(player, out removeSource, fireEvents);
            }
        }

        public virtual bool Use(bool fireEvents = true)
        {
            bool removeSource;
            return Use(out removeSource, fireEvents);
        }

        /// <summary>
        /// Use this triggerer
        /// </summary>
        /// <param name="removeSource">Should the triggerer be removed from future checking? For example, an item is picked up and moved.
        /// It can not be picked up again, so should be removed from the potential list of triggerers.</param>
        /// <param name="fireEvents"></param>
        public abstract bool Use(out bool removeSource, bool fireEvents = true);
        public abstract bool Use(InventoryPlayer player, out bool removeSource, bool fireEvents = true);
        public abstract bool UnUse(bool fireEvents = true);
        public abstract bool UnUse(InventoryPlayer player, bool fireEvents = true);


        public abstract void NotifyCameInRange(InventoryPlayer player);
        public abstract void NotifyWentOutOfRange(InventoryPlayer player);


        public void NotifyCursorEnterTriggerer()
        {
            cursorIcon.Enable();
        }

        public void NotifyCursorExitTriggerer()
        {
            InventorySettingsManager.instance.noActionCursor.Enable();
        }
    }
}
