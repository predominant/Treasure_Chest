#if PLAYMAKER

using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;


namespace Devdog.InventorySystem.Integration.PlayMaker
{
    [ActionCategory("Inventory Pro")]
    [HutongGames.PlayMaker.Tooltip("Set items for a vendor, replaces the old items that were in the collection before it.")]
    public class SetItemsVendor : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        public InventoryItemBase[] items;

        [UIHint(UIHint.Variable)]
        public VendorTriggerer vendor;

        public override void Reset()
        {

        }

        public override void OnEnter()
        {
            vendor.items = items;
            Finish();
        }
    }
}

#endif