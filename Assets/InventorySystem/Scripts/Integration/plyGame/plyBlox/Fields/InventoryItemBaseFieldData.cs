#if PLY_GAME

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem;
using plyBloxKit;
using plyGame;

namespace Devdog.InventorySystem.Integration.plyGame.plyBlox
{
    public class InventoryItemBaseFieldData : IBlockValidate
    {
        public InventoryItemBase item;

        public override string ToString()
        {
            if (item == null) return "-invalid-";
            return string.IsNullOrEmpty(item.name) ? "-invalid-" : item.name;
        }

        public bool IsValid()
        {
            return item != null && string.IsNullOrEmpty(item.name) == false;
        }

        public InventoryItemBaseFieldData Copy()
        {
            var c = new InventoryItemBaseFieldData();
            c.item = item;
            return c;
        }
    }
}

#endif