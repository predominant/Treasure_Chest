using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Devdog.InventorySystem
{
    [AddComponentMenu("InventorySystem/Other/Sync collection")]
    public class SyncCollection : MonoBehaviour
    {
        public ItemCollectionBase toSyncFrom;
        public bool syncBothWays = true;

        public void Start()
        {
            toSyncFrom.SyncActions(GetComponent<ItemCollectionBase>(), syncBothWays);
        }
    }
}
