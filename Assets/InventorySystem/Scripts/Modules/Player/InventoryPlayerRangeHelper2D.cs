using System;
using Devdog.InventorySystem.Models;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.InventorySystem
{
    //[RequireComponent(typeof(CircleCollider2D))]
    //[RequireComponent(typeof(Rigidbody2D))] // Needed, otherwise collider calls (onEnter, onExit) get called on parent.
    [AddComponentMenu("InventorySystem/Player/Inventory player range helper 2D")]
    public partial class InventoryPlayerRangeHelper2D : InventoryPlayerRangeHelper
    {
        public CircleCollider2D circleCollider { get; set; }
        public new Rigidbody2D rigidbody2D { get; set; }

        protected override void GetComponents()
        {
            player = GetComponentInParent<InventoryPlayer>();

            circleCollider = GetComponent<CircleCollider2D>();
            if (circleCollider == null)
                circleCollider = gameObject.AddComponent<CircleCollider2D>();

            circleCollider.isTrigger = true;
            circleCollider.radius = InventorySettingsManager.instance.useObjectDistance;

            rigidbody2D = GetComponent<Rigidbody2D>();
            if (rigidbody2D == null)
                rigidbody2D = gameObject.AddComponent<Rigidbody2D>();

            rigidbody2D.isKinematic = true;
        }

        public void OnTriggerEnter2D(Collider2D col)
        {
            if (enabled == false)
                return;

            var c = col.gameObject.GetComponent<ObjectTriggererBase>();
            if (c != null)
            {
                c.NotifyCameInRange(player);
                triggerersInRange.Add(c);
            }

        }

        public void OnTriggerExit2D(Collider2D col)
        {
            if (enabled == false)
                return;

            var c = col.gameObject.GetComponent<ObjectTriggererBase>();
            if (c != null)
            {
                c.NotifyWentOutOfRange(player);
                triggerersInRange.Remove(c);
            }
        }
    }
}
