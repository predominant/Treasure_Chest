using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem;

namespace Devdog.InventorySystem.UI
{
    public partial class InventoryUIDragLookup
    {
        public int startIndex = -1;
        public ItemCollectionBase startItemCollection;

        public int endIndex = -1;
        public ItemCollectionBase endItemCollection;

        public bool endOnWrapper
        {
            get
            {
                return endItemCollection != null;
            }
        }


        public void Reset()
        {
            startIndex = -1;
            startItemCollection = null;

            endIndex = -1;
            endItemCollection = null;
        }
    }
}
