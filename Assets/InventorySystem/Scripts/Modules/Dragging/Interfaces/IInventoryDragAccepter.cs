using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem;

namespace Devdog.InventorySystem.UI
{
    public interface IInventoryDragAccepter
    {

        /// <summary>
        /// Check if the drag accepter can accept this specific item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool AcceptsDragItem(InventoryItemBase item);

        /// <summary>
        /// Accept the item
        /// </summary>
        /// <returns></returns>
        bool AcceptDragItem(InventoryItemBase item);
    }
}
