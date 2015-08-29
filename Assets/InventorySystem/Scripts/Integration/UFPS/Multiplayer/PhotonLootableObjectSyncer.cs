#if UFPS_MULTIPLAYER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Photon;

namespace Devdog.InventorySystem.Integration.UFPS
{
    [UnityEngine.RequireComponent(typeof(LootableObject))]
    [UnityEngine.RequireComponent(typeof(PhotonView))]
    [UnityEngine.AddComponentMenu("InventorySystem/Integration/UFPS/PhotonLootableObjectSyncer")]
    public partial class PhotonLootableObjectSyncer : Photon.MonoBehaviour
    {


        private LootableObject lootable { get; set; }
        private ObjectTriggerer triggerer { get; set; }


        public virtual void Awake()
        {
            lootable = GetComponent<LootableObject>();
            lootable.OnLootedItem += LootableOnOnLootedItem;

            triggerer = GetComponent<ObjectTriggerer>();
            triggerer.OnTriggerUse += (player) => { photonView.RPC("OnTriggerUse", PhotonTargets.Others); };
            triggerer.OnTriggerUnUse += (player) => { photonView.RPC("OnTriggerUnUse", PhotonTargets.Others); };
        }

        [PunRPC]
        private void OnTriggerUse()
        {
            triggerer.DoVisuals();
        }

        [PunRPC]
        private void OnTriggerUnUse()
        {
            triggerer.UndoVisuals();
        }

        /// <summary>
        /// Photon.MonoBehaviour action
        /// </summary>
        protected virtual void OnJoinedRoom()
        {
            if (PhotonNetwork.isMasterClient)
            {
                var itemIDs = new int[lootable.items.Length];
                for (int i = 0; i < itemIDs.Length; i++)
                {
                    itemIDs[i] = (int)lootable.items[i].ID;
                }

                // Master defines the items to be looted, and sends it to the clients.
                var result = string.Join(",", itemIDs.Select(x => x.ToString()).ToArray()); // Concat as a string, because photon is being bitchy about int arrays
                photonView.RPC("SetLootableItemsClients", PhotonTargets.AllBufferedViaServer, result);
            }
        }


        [PunRPC]
        private void SetLootableItemsClients(string itemIDsString, PhotonMessageInfo info)
        {
            if (itemIDsString == "")
            {
                lootable.items = new InventoryItemBase[0];
                return;
            }

            string[] itemIDs = itemIDsString.Split(',');

            UnityEngine.Debug.Log("Received message from server to set items for lootable: " + itemIDs.Length);
            var items = new InventoryItemBase[itemIDs.Length];

            for (int i = 0; i < itemIDs.Length; i++)
            {
                var item = UnityEngine.Object.Instantiate<InventoryItemBase>(ItemManager.instance.items[int.Parse(itemIDs[i])]);
                item.gameObject.SetActive(false);
                item.transform.SetParent(transform);
                items[i] = item;
            }

            lootable.items = items; // Set the lootable items for this object.
        }

        private void LootableOnOnLootedItem(InventoryItemBase item, uint itemId, uint slot, uint amount)
        {
            photonView.RPC("LootableObjectItemRemoved", PhotonTargets.OthersBuffered, new object[] { (int)slot });
        }

        [PunRPC]
        private void LootableObjectItemRemoved(int slot)
        {
            lootable.items[slot] = null;
            UnityEngine.Debug.Log("Item looted from other client");
            
            // Currently checking this object out.
            lootable.lootWindow.SetItems(lootable.items, true);
        }
    }
}

#endif