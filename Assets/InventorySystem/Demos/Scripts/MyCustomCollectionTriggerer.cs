using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem;
using UnityEngine;

namespace Devdog.InventorySystem.Demo
{
    [RequireComponent(typeof(ObjectTriggerer))]
    public class MyCustomCollectionTriggerer : MonoBehaviour
    {
        /// <summary>
        /// The items inside this local chest
        /// </summary>
        //[NonSerialized]
        public InventoryItemBase[] myItems = new InventoryItemBase[10]; // Room for 10 items
        
        public void Awake()
        {
            // Create instance objects.
            for (int i = 0; i < myItems.Length; i++)
            {
                if (myItems[i] != null)
                {
                    myItems[i] = GameObject.Instantiate<InventoryItemBase>(myItems[i]);
                    myItems[i].transform.SetParent(transform);
                    myItems[i].gameObject.SetActive(false);
                }
            }


            // The triggererHandler component, that is always there because of RequireComponent
            var triggerer = GetComponent<ObjectTriggerer>();

            // The collection we want to place the items into.
            var collection = triggerer.window.GetComponent<ItemCollectionBase>();

            // When the trigger is used by the player, behavior can be modified in the inspector.
            triggerer.OnTriggerUse += (player) =>
            {
                // When the user has triggered this object, set the items in the window
                collection.SetItems(myItems, true);
                collection.SyncActions(myItems); // All actions done in the UI window should be synced to this item array.

                // And done!
            };

            triggerer.OnTriggerUnUse += (player) =>
            {
                collection.StopSyncActions(myItems); // Stop syncing actions
            };
        }
    }
}