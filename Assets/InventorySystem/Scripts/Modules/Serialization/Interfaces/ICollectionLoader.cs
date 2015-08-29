using System;
using System.Collections.Generic;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem
{
    public interface ICollectionLoader
    {
        void LoadItems(ItemCollectionBase collection, Uri loadLocation, Action<byte[]> callback);
    }
}