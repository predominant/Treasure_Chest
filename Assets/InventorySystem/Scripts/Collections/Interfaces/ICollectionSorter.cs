using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Devdog.InventorySystem
{
    public interface ICollectionSorter
    {
        IList<InventoryItemBase> Sort(IList<InventoryItemBase> items);
    }
}