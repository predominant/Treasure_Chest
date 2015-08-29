using System;
using System.Collections.Generic;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem
{
    public interface ICollectionSerializer
    {
        byte[] SerializeItems(IList<InventoryItemSaveLookup> toSerialize);
        byte[] SerializeItemReferences(IList<InventoryItemReferenceSaveLookup> toSerialize);

        IList<InventoryItemSaveLookup> DeserializeItems(byte[] data);
        IList<InventoryItemReferenceSaveLookup> DeserializeItemReferences(byte[] data);
    }
}
