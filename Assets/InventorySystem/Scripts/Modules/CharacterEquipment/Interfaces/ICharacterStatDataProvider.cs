using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem
{
    public interface ICharacterStatDataProvider
    {

        /// <summary>
        /// Set the categories and properties, does not calculate anything.
        /// </summary>
        /// <param name="appendTo"></param>
        Dictionary<string, List<InventoryCharacterStat>> Prepare(Dictionary<string, List<InventoryCharacterStat>> appendTo);

        ///// <summary>
        ///// Calculate the stats and fill the previously defined slots by Prepare()
        ///// </summary>
        ///// <param name="appendTo"></param>
        //Dictionary<string, List<InventoryCharacterStat>> Calculate(Dictionary<string, List<InventoryCharacterStat>> appendTo);


    }
}
