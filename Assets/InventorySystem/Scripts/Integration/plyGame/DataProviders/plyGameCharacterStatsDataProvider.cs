#if PLY_GAME

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.Integration.plyGame;
using Devdog.InventorySystem.Models;
using plyCommon;
using plyGame;
using UnityEngine;

namespace Devdog.InventorySystem
{
    public class plyGameCharacterStatsDataProvider : ICharacterStatDataProvider
    {


        private plyCharacterUI characterUI { get; set; }
        protected virtual List<ActorAttribute> plyAttributes
        {
            get
            {
                if(InventoryPlayerManager.instance.currentPlayer == null)
                    return new List<ActorAttribute>();

                var actor = InventoryPlayerManager.instance.currentPlayer.GetComponent<Actor>();
                if (actor == null || actor.actorClass == null)
                    return new List<ActorAttribute>();

                return actor.actorClass.attributes;
            }
        }



        public plyGameCharacterStatsDataProvider(plyCharacterUI characterUI)
        {
            this.characterUI = characterUI;
        }


        public Dictionary<string, List<InventoryCharacterStat>> Prepare(Dictionary<string, List<InventoryCharacterStat>> appendTo)
        {
            // Get the properties
            foreach (var stat in ItemManager.instance.plyAttributes)
            {
                if (appendTo.ContainsKey(stat.category) == false)
                    appendTo.Add(stat.category, new List<InventoryCharacterStat>());

                var plyStat = GetPlyAttribute(stat.ID);
                if (plyStat == null)
                {
                    Debug.Log("Plystat not found ");
                    continue;
                }
                
                // Already in list
                if (appendTo[stat.category].FirstOrDefault(o => o.statName == plyStat.def.screenName) != null)
                    continue;
                
                appendTo[stat.category].Add(new InventoryCharacterStat(InventoryPlayerManager.instance.currentPlayer, plyStat.def.screenName, "{0}", plyStat.Value, 9999f, true));
            }

            return appendTo;
        }

        protected ActorAttribute GetPlyAttribute(UniqueID id)
        {
            var a = plyAttributes.FirstOrDefault(o => o.id.Value.ToString() == id.Value.ToString());
            if (a == null || a.def == null)
                return null;

            return a;
        }
    }
}

#endif