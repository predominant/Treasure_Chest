#if UFPS_MULTIPLAYER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Devdog.InventorySystem.Integration.UFPS;

namespace Devdog.InventorySystem.Integration.UFPS.Multiplayer
{
    public partial class InventoryMPUFPSPickupManager : vp_MPPickupManager
    {
        public static InventoryMPUFPSPickupManager instance { get; protected set; }


        protected override void Awake()
        {
            instance = this;

            base.Awake();
        }

        public void RegisterPickup(UFPSInventoryItemBase item)
        {
            item.objectTriggererItemUfps.ID = (vp_Utility.PositionToID(item.objectTriggererItemUfps.transform.position) + ((int)item.ID * 100000));
            RegisterPickup(item.objectTriggererItemUfps);
        }

        protected override void RegisterPickup(vp_ItemPickup p)
        {
            base.RegisterPickup(p);
        }

        /// <summary>
        /// When a player has looted an item, it will be send to all other clients to get rid of the no longer valid object.
        /// </summary>
        [PunRPC]
        void InventoryPlayerPickedUpItem(int ufpsItemID, int ufpsPlayerID, Vector3 itemPosition, Quaternion itemRotation, PhotonMessageInfo info)
        {
            List<vp_ItemPickup> pickups;
            if (!vp_MPPickupManager.Instance.Pickups.TryGetValue(ufpsItemID, out pickups))
                return;

            if (pickups[0].gameObject != null)
                vp_Utility.Activate(pickups[0].gameObject, false);

            Debug.Log("Client looted item with UFPS ID: " + ufpsItemID);
            
            vp_MPNetworkPlayer player;
            if (!vp_MPNetworkPlayer.PlayersByID.TryGetValue(ufpsPlayerID, out player))
                return;

            if (player == null)
                return;

            if (player.Collider == null)
                return;

            foreach (vp_ItemPickup p in pickups)
            {
                var a = p as ObjectTriggererItemUFPS;
                a.TryGiveToPlayer(player.Collider, p.Amount, false);
                Debug.Log("Giving UFPS item to player - ItemID: " + a.ID);
            }
        }


        [PunRPC]
        private void InventoryDroppedObject(int itemID, int ufpsItemID, int amount, Vector3 position, Quaternion rotation, PhotonMessageInfo info)
        {
            List<vp_ItemPickup> pickups;
            if (!vp_MPPickupManager.Instance.Pickups.TryGetValue(ufpsItemID, out pickups))
                return;

            if (pickups[0].gameObject != null)
            {
                var item = pickups[0].gameObject.GetComponent<UFPSInventoryItemBase>();
                if (item == null)
                {
                    Debug.LogWarning("Non UFPS Item was dropped??", pickups[0].gameObject);
                    return;
                }

                item.transform.position = position;
                item.transform.rotation = rotation;


                if (item is EquippableUFPSInventoryItem)
                    ((EquippableUFPSInventoryItem)item).currentClipCount = (uint)amount;
                else if (item is UnitTypeUFPSInventoryItem)
                    ((UnitTypeUFPSInventoryItem)item).unitAmount = (uint)amount;


                item.gameObject.SetActive(true);
                var triggerer = item.gameObject.GetComponent<ObjectTriggererItemUFPS>();
                if (triggerer != null)
                {
                    triggerer.ID = ufpsItemID;
                    triggerer.Amount = amount;
                }

                Debug.Log("Client dropped item #" + item.ID + " (" + item.name + ") with " + amount + " bullets (UFPSID: " + ufpsItemID + ")", item);
            }
        }
    }
}

#endif