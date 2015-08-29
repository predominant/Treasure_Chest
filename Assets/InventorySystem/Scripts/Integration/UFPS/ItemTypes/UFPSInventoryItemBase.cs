#if UFPS

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Devdog.InventorySystem;
using Devdog.InventorySystem.Models;

#if UFPS_MULTIPLAYER
using Devdog.InventorySystem.Integration.UFPS.Multiplayer;
#endif


namespace Devdog.InventorySystem.Integration.UFPS
{
    [RequireComponent(typeof(ObjectTriggererItemUFPS))]
    public partial class UFPSInventoryItemBase : EquippableInventoryItem
    {
        public bool useUFPSItemData = true;


        protected vp_PlayerEventHandler eventHandler
        {
            get
            {
                return InventoryPlayerManager.instance.currentPlayer.GetComponent<vp_PlayerEventHandler>();
            }
        }
        protected vp_PlayerInventory ufpsInventory
        {
            get
            {
                return InventoryPlayerManager.instance.currentPlayer.GetComponent<vp_PlayerInventory>();
            }
        }

        private ObjectTriggererItemUFPS _objectTriggererItemUfps;
        public ObjectTriggererItemUFPS objectTriggererItemUfps
        {
            get
            {
                if (_objectTriggererItemUfps == null)
                    _objectTriggererItemUfps = GetComponent<ObjectTriggererItemUFPS>();

                return _objectTriggererItemUfps;
            }
        }

        protected virtual void Awake()
        {
#if UFPS_MULTIPLAYER
            InventoryMPUFPSPickupManager.instance.RegisterPickup(this);
#endif
        }

        public override bool PickupItem()
        {
            bool pickedUp = base.PickupItem();
            if (pickedUp)
                transform.position = Vector3.zero; // Reset position to avoid the user from looting it twice when reloading (reloading temp. enables the item)

            return pickedUp;
        }
    }
}

#endif