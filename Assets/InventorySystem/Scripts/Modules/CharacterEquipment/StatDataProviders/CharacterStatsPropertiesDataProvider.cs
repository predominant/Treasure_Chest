using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem
{
    public class CharacterStatsPropertiesDataProvider : ICharacterStatDataProvider
    {
        private CharacterUI characterUI { get; set; }

        public CharacterStatsPropertiesDataProvider(CharacterUI characterUI)
        {
            this.characterUI = characterUI;
        }


        public Dictionary<string, List<InventoryCharacterStat>> Prepare(Dictionary<string, List<InventoryCharacterStat>> appendTo)
        {
            // Get the properties
            foreach (var property in ItemManager.instance.properties)
            {
                if (property.useInStats == false)
                    continue;

                if (appendTo.ContainsKey(property.category) == false)
                    appendTo.Add(property.category, new List<InventoryCharacterStat>());

                // Check if it's already in the list
                if (appendTo[property.category].FirstOrDefault(o => o.statName == property.name) != null)
                    continue;

                appendTo[property.category].Add(new InventoryCharacterStat(InventoryPlayerManager.instance.currentPlayer, property));
            }

            return appendTo;
        }
    }
}
