#if PLY_GAME

using System;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem;
using Devdog.InventorySystem.Models;
using plyCommon;
using plyGame;
using UnityEngine;

namespace Devdog.InventorySystem.Integration.plyGame
{
    public partial class plyGameEquippableInventoryItem : EquippableInventoryItem
    {
        public plyGameAttributeModifierModel[] plyAttributes = new plyGameAttributeModifierModel[0];

        public Actor actor
        {
            get
            {
                return InventoryPlayerManager.instance.currentPlayer.GetComponent<Actor>();
            }
        }

        public override LinkedList<InventoryItemInfoRow[]> GetInfo()
        {
            var info = base.GetInfo();

            var attributes = new InventoryItemInfoRow[plyAttributes.Length];
            for (int i = 0; i < plyAttributes.Length; i++)
            {
                var a = actor.actorClass.attributes.FirstOrDefault(attribute => attribute.id.Value == plyAttributes[i].id.Value);
                if (a != null)
                    attributes[i] = new InventoryItemInfoRow(a.def.screenName, plyAttributes[i].toAdd.ToString(), plyAttributes[i].color, plyAttributes[i].color);
            }

            info.AddAfter(info.First, attributes.ToArray());

            return info;
        }

        public override bool CanEquip()
        {
            bool can = base.CanEquip();
            if (can == false)
                return false;

            if (actor.actorClass.currLevel < requiredLevel)
            {
                InventoryManager.instance.lang.itemCannotBeUsedLevelToLow.Show(name, description, requiredLevel);
                return false;
            }

            return true;
        }


        public override bool Equip(InventoryEquippableField equipSlot, CharacterUI equipToCollection)
        {
            bool equipped = base.Equip(equipSlot, equipToCollection);
            if (equipped == false)
                return false;

            SetPlyGameValues(1.0f);
            return true;
        }


        public override bool UnEquip(bool wasUsed)
        {
            bool unequipped = base.UnEquip(wasUsed);
            if (unequipped == false)
                return false;
            
            SetPlyGameValues(-1.0f);
            return true;
        }


        protected virtual void SetPlyGameValues(float multiplier)
        {
            foreach (var attr in plyAttributes)
            {
                var a = actor.actorClass.attributes.FirstOrDefault(attribute => attribute.id.Value == attr.id.Value);
                if (a != null)
                {
                    a.ChangeSimpleBonus((int)(attr.toAdd * multiplier));
                }
            }
        }
    }
}

#endif