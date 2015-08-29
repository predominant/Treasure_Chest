using System;
using System.Collections.Generic;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem
{
    public interface ICollectionSaver
    {
        /// <summary>
        /// Save the item data
        /// </summary>
        /// <param name="collection">Collection to save</param>
        /// <param name="serializedData">Serialized item data</param>
        /// <param name="callback">The callback when saving is either failed or completed, can by async.</param>
        void SaveItems(ItemCollectionBase collection, Uri saveLocation, byte[] serializedData, Action<bool> callback);
    }
}
