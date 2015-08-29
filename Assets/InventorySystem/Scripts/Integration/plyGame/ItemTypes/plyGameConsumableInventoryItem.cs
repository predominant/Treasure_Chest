#if PLY_GAME

using System;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem;
using plyCommon;
using plyGame;
using UnityEngine;

namespace Devdog.InventorySystem.Integration.plyGame
{
    public partial class plyGameConsumableInventoryItem : InventoryItemBase
    {
        public plyGameAttributeModifierModel[] plyAttributes = new plyGameAttributeModifierModel[0];
        public AudioClip audioClipWhenUsed;
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
                if(a != null)
                    attributes[i] = new InventoryItemInfoRow(a.def.screenName, plyAttributes[i].toAdd.ToString(), plyAttributes[i].color, plyAttributes[i].color);
            }

            info.AddAfter(info.First, attributes.ToArray());

            return info;
        }

        public override int Use()
        {
            int used = base.Use();
            if (used < 0)
                return used;

            if (actor.actorClass.currLevel < requiredLevel)
            {
                InventoryManager.instance.lang.itemCannotBeUsedLevelToLow.Show(name, description, requiredLevel);
                return -1;
            }
            
            SetItemProperties(InventoryPlayerManager.instance.currentPlayer, InventoryItemUtility.SetItemPropertiesAction.Use);

            if(audioClipWhenUsed != null)
                InventoryUtility.AudioPlayOneShot(audioClipWhenUsed);

            currentStackSize--;
            NotifyItemUsed(1, true);
            return 1;
        }

        protected virtual void SetItemProperties(InventoryPlayer player, InventoryItemUtility.SetItemPropertiesAction action)
        {
            InventoryItemUtility.SetItemProperties(player, properties, action);
            SetPlyGameValues(player);
        }

        protected virtual void SetPlyGameValues(InventoryPlayer player)
        {
            foreach (var attr in plyAttributes)
            {
                var a = actor.actorClass.attributes.FirstOrDefault(attribute => attribute.id.Value == attr.id.Value);
                if (a != null)
                {
                    //var i = ItemManager.instance.plyAttributes.FirstOrDefault(o => o.ID == attr.id);
                    //var stat = player.characterCollection.GetStat(i.category, a.def.screenName);

                    if (attr.addToBonus)
                    {
                        a.ChangeSimpleBonus(attr.toAdd);
                        //stat._maxValue += attr.toAdd; // Done in plyInventoryPlayer Callback
                    }
                    else
                    {
                        a.SetConsumableValue(a.ConsumableValue + attr.toAdd, gameObject);
                        //stat.currentValue += attr.toAdd; // Done in plyInventoryPlayer Callback
                    }
                }
            }

            //Debug.Log("TODO: Ply stats still need fixing"); // TODO: FINISH!

            //foreach (var equipToCollection in InventoryManager.GetEquipToCollections())
            //{
            //    equipToCollection.UpdateCharacterStats();
            //}
        }
    }
}

#endif