#if UFPS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Devdog.InventorySystem.Integration.UFPS
{
    using Devdog.InventorySystem.Integration.UFPS.Multiplayer;

    [UnityEngine.AddComponentMenu("InventorySystem/Integration/UFPS/ObjectTriggererItemUFPS")]
    public class ObjectTriggererItemUFPS : vp_ItemPickup
    {

        private ObjectTriggererItem objectTriggerer { get; set; }
        //protected bool itemInInventory { get; set; }

        protected override void Awake()
        {
            objectTriggerer = GetComponent<ObjectTriggererItem>();
            objectTriggerer.OnPickup += ObjectTriggererOnPickup;

            m_Item = new vp_ItemPickup.ItemSection();

            var equippable = GetComponent<EquippableUFPSInventoryItem>();
            if (equippable != null)
                m_Item.Type = equippable.itemType;
            else
            {
                var unitType = GetComponent<UnitTypeUFPSInventoryItem>();
                if (unitType != null)
                    m_Item.Type = unitType.unitType;
            }

        }

        protected override void OnEnable()
        {
            base.OnEnable();

        }

        //protected virtual void OnDisable()
        //{
        //    objectTriggerer.OnPickup -= ObjectTriggererOnPickup;
        //}


        protected virtual void ObjectTriggererOnPickup(InventoryPlayer inventoryPlayer)
        {
            // Player picked up the item (Triggered by event)

#if UFPS_MULTIPLAYER

            vp_MPNetworkPlayer player;
            if (!vp_MPNetworkPlayer.Players.TryGetValue(InventoryPlayerManager.instance.currentPlayer.transform, out player))
                return;

            InventoryMPUFPSPickupManager.instance.photonView.RPC("InventoryPlayerPickedUpItem", PhotonTargets.Others, ID, player.ID, transform.position, transform.rotation);

#endif

        }


        protected override void OnTriggerEnter(Collider col)
        {
            //base.OnTriggerEnter(col);
        }


        /// <summary>
        /// Directly gives it to the player, bypasses server check.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="amount"></param>
        public void TryGiveToPlayer(Collider col, int amount, bool fireEvents = true)
        {
            //m_Depleted = false;
            this.Amount = amount;


            if (ItemType == null)
                return;

            //if (!Collider.enabled)
            //    return;

            // only do something if the trigger is still active
            //if (m_Depleted)
            //    return;

            // see if the colliding object was a valid recipient
            if ((m_Recipient.Tags.Count > 0) && !m_Recipient.Tags.Contains(col.gameObject.tag))
                return;

            bool result = false;

            int prevAmount = vp_TargetEventReturn<vp_ItemType, int>.SendUpwards(col, "GetItemCount", m_Item.Type);


            if (ItemType == typeof(vp_ItemType))
                result = vp_TargetEventReturn<vp_ItemType, int, bool>.SendUpwards(col, "TryGiveItem", m_Item.Type, ID);
            else if (ItemType == typeof(vp_UnitBankType))
                result = vp_TargetEventReturn<vp_UnitBankType, int, int, bool>.SendUpwards(col, "TryGiveUnitBank", (m_Item.Type as vp_UnitBankType), Amount, ID);
            else if (ItemType == typeof(vp_UnitType))
                result = vp_TargetEventReturn<vp_UnitType, int, bool>.SendUpwards(col, "TryGiveUnits", (m_Item.Type as vp_UnitType), Amount);
            else if (ItemType.BaseType == typeof(vp_ItemType))
                result = vp_TargetEventReturn<vp_ItemType, int, bool>.SendUpwards(col, "TryGiveItem", m_Item.Type, ID);
            else if (ItemType.BaseType == typeof(vp_UnitBankType))
                result = vp_TargetEventReturn<vp_UnitBankType, int, int, bool>.SendUpwards(col, "TryGiveUnitBank", (m_Item.Type as vp_UnitBankType), Amount, ID);
            else if (ItemType.BaseType == typeof(vp_UnitType))
                result = vp_TargetEventReturn<vp_UnitType, int, bool>.SendUpwards(col, "TryGiveUnits", (m_Item.Type as vp_UnitType), Amount);

            if (fireEvents)
            {
                if (result == true)
                {
                    m_PickedUpAmount = (vp_TargetEventReturn<vp_ItemType, int>.SendUpwards(col, "GetItemCount", m_Item.Type) - prevAmount); // calculate resulting amount given
                    OnSuccess(col.transform);
                }
                else
                {
                    OnFail(col.transform);
                }
            }
        }


        //protected override void OnSuccess(Transform recipient)
        //{
        //    base.OnSuccess(recipient);

        //    objectTriggerer.PickupItem();
        //}
    }
}

#endif