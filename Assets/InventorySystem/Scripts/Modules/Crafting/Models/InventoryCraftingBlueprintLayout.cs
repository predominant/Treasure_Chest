using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devdog.InventorySystem.Models
{
    [System.Serializable]
    public partial class InventoryCraftingBlueprintLayout
    {
        [System.Serializable]
        public partial class Row
        {
            [System.Serializable]
            public partial class Column
            {
                public int index;
                public InventoryItemBase item;
                public int amount = 0;
            }

            public Column this[int i]
            {
                get
                {
                    return columns[i];
                }
                set
                {
                    columns[i] = value;
                }
            }

            public int index;
            public Column[] columns = new Column[0];
        }

        public int ID;
        public bool enabled = true;
        public Row[] rows = new Row[0];


        public Row this[int i]
        {
            get
            {
                return rows[i];
            }
            set
            {
                rows[i] = value;
            }
        }
    }
}