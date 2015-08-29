#if PLAYMAKER

using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;


namespace Devdog.InventorySystem.Integration.PlayMaker
{
    [ActionCategory("Inventory Pro")]
    [HutongGames.PlayMaker.Tooltip("Get the item count from a given collection.")]
    public class GetItemCount : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        public InventoryItemBase item;

        [UIHint(UIHint.Variable)]
        public ItemCollectionBase collection;

        [UIHint(UIHint.Variable)]
        public FsmVar result;

        public override void Reset()
        {

        }

        public override void OnEnter()
        {
            result.intValue = (int)collection.GetItemCount(item.ID);

            Finish();
        }
    }
}

#endif