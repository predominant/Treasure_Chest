using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem
{
    public class CharacterStatsDataProvider : ICharacterStatDataProvider
    {
        private CharacterUI characterUI { get; set; }

        public CharacterStatsDataProvider(CharacterUI characterUI)
        {
            this.characterUI = characterUI;
        }


        public Dictionary<string, List<InventoryCharacterStat>> Prepare(Dictionary<string, List<InventoryCharacterStat>> appendTo)
        {
            // Get the equip stats
            foreach (var equipStat in ItemManager.instance.equipStats)
            {
                if (appendTo.ContainsKey(equipStat.category) == false)
                    appendTo.Add(equipStat.category, new List<InventoryCharacterStat>());

                appendTo[equipStat.category].Add(new InventoryCharacterStat(InventoryPlayerManager.instance.currentPlayer, equipStat.name, "{0}", 0.0f, 9999f, equipStat.show));
            }

            return appendTo;
        }
    }
}
