#if PLAYMAKER

using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;


namespace Devdog.InventorySystem.Integration.PlayMaker
{
    [ActionCategory("Inventory Pro")]
    [HutongGames.PlayMaker.Tooltip("Check if a given collection contains an item")]
    public class CollectionContainsItem : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        public InventoryItemBase item;

        [UIHint(UIHint.Variable)]
        public ItemCollectionBase collection;

        [UIHint(UIHint.Variable)]
        public FsmBool result;

        public override void Reset()
        {

        }

        public override void OnEnter()
        {
            result.Value = collection.GetItemCount(item.ID) > 0;

            Finish();
        }
    }
}

#endif