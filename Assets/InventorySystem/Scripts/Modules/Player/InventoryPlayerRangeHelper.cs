using System;
using Devdog.InventorySystem.Models;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.InventorySystem
{
    using Devdog.InventorySystem.UI;

    //[RequireComponent(typeof(SphereCollider))]
    //[RequireComponent(typeof(Rigidbody))] // Needed, otherwise collider calls (onEnter, onExit) get called on parent.
    [AddComponentMenu("InventorySystem/Player/Inventory player range helper")]
    public partial class InventoryPlayerRangeHelper : MonoBehaviour
    {
        public delegate void NewBestTriggerer(ObjectTriggererBase old, ObjectTriggererBase newBest);
        public event NewBestTriggerer OnChangedBestTriggerer;
        
        public InventoryPlayer player { get; set; }
        public SphereCollider sphereCollider { get; set; }
        public new Rigidbody rigidbody { get; set; }


        public new bool enabled
        {
            get;
            set;
        }


        /// <summary>
        /// All triggerers in range of this player, list is maintaned by InventoryPlayerRangeHelper.cs
        /// </summary>
        [HideInInspector, NonSerialized]
        public List<ObjectTriggererBase> triggerersInRange = new List<ObjectTriggererBase>(32);
        

        private ObjectTriggererBase _bestTriggerer;
        public ObjectTriggererBase bestTriggerer
        {
            get { return _bestTriggerer; }
            protected set
            {
                var before = _bestTriggerer;
                _bestTriggerer = value;

                if (before != value)
                {
                    // New best triggerer
                    if (OnChangedBestTriggerer != null)
                        OnChangedBestTriggerer(before, value);

                }

                // InventoryTriggererManager handles all triggering for the current player.
                InventoryTriggererManager.instance.bestTriggerer = value;
            }
        }

        /// <summary>
        /// How often should we check if there's an object / item in-front of us that can be triggered?
        /// </summary>
        public const float UpdateBestTriggererInterval = 0.2f;


        public void Awake()
        {
            this.enabled = true;
            this.gameObject.layer = 2; // Ignore raycasts layer.
            GetComponents();

            InvokeRepeating("UpdateBestTriggerer", UpdateBestTriggererInterval, UpdateBestTriggererInterval);
        }

        protected virtual void GetComponents()
        {
            player = GetComponentInParent<InventoryPlayer>();

            sphereCollider = GetComponent<SphereCollider>();
            if (sphereCollider == null)
                sphereCollider = gameObject.AddComponent<SphereCollider>();

            sphereCollider.isTrigger = true;
            sphereCollider.radius = InventorySettingsManager.instance.useObjectDistance;
            

            rigidbody = GetComponent<Rigidbody>();
            if (rigidbody == null)
                rigidbody = gameObject.AddComponent<Rigidbody>();

            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
        }

        public void Update()
        {
            if (bestTriggerer == null || enabled == false)
                return;

            if (Input.GetKeyDown(bestTriggerer.triggerKeyCode) && bestTriggerer.triggerKeyCode != KeyCode.None)
            {
                bool removeSource;
                bestTriggerer.Toggle(InventoryPlayerManager.instance.currentPlayer, out removeSource, true);
                if (removeSource)
                {
                    triggerersInRange.Remove(bestTriggerer);
                    bestTriggerer = null;
                }
            }
        }


        public void UpdateBestTriggerer()
        {
            // Check for Triggerers in range.
            if(enabled)
                bestTriggerer = GestBestTriggerer();

        }

        /// <summary>
        /// Get the best triggerable object / item.
        /// Current setup uses distance and Dot product of player to item angle for triggerable priority checking.
        /// </summary>
        /// <returns></returns>
        public virtual ObjectTriggererBase GestBestTriggerer()
        {
            if (InventorySettingsManager.instance.triggererHandlerType == InventorySettingsManager.TriggererKeyCodeHandlerType.RaycastFromCamera)
            {
                return GetBestTriggererRaycast(Camera.main);
            }

            if (InventorySettingsManager.instance.triggererHandlerType == InventorySettingsManager.TriggererKeyCodeHandlerType.FindBestInRange)
            {
                return GetBestTriggererInRange();
            }

            return null;
        }

        public virtual ObjectTriggererBase GetBestTriggererRaycast(Camera camera)
        {
            if (camera == null)
                return null; // No camera in the scene

            var settings = InventorySettingsManager.instance;


            // Raycast from center of screen
            Debug.DrawRay(camera.transform.position, (camera.transform.forward * settings.useObjectDistance), Color.red, 0.2f);


            RaycastHit hit;
            if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, settings.useObjectDistance))
            {
                var triggerer = hit.transform.GetComponent<ObjectTriggererBase>();
                if (triggerer != null)
                {
                    return triggerer;
                }
            }

            return null;
        }

        public virtual ObjectTriggererBase GetBestTriggererInRange()
        {
            float bestCheck = -999.0f;
            ObjectTriggererBase closestTriggerer = null;
            foreach (var item in triggerersInRange)
            {
                if (item == null)
                {
                    continue;
                }

                var toPlayerVec = item.transform.position - transform.position;
                var dist = Vector3.Magnitude(toPlayerVec);

                float inFrontFactor = Mathf.Clamp01(Vector3.Dot(transform.forward, toPlayerVec / dist));
                inFrontFactor *= 0.2f; // Item infront has 20% effect on making the best decision

                float final = (InventorySettingsManager.instance.useObjectDistance - dist) * inFrontFactor;
                final = Mathf.Abs(final);

                if (final > bestCheck)
                {
                    closestTriggerer = item;
                    bestCheck = final;
                    //Debug.LogWarning("Found a better one: infront: " + inFrontFactor + " total factor: " + final, item.transform);
                }
            }

            triggerersInRange.RemoveAll(o => o == null);
            return closestTriggerer;
        }


        public void OnTriggerEnter(Collider col)
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

        public void OnTriggerExit(Collider col)
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
