#if PLAYMAKER

using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;


namespace Devdog.InventorySystem.Integration.PlayMaker
{
    [ActionCategory("Inventory Pro")]
    [HutongGames.PlayMaker.Tooltip("Check if the given item can be added to the collection.")]
    public class CanAddItemToCollection : FsmStateAction
    {
        public InventoryItemBase item;
        public ItemCollectionBase collection;

        [UIHint(UIHint.Variable)]
        public FsmVar result;

        public override void Reset()
        {

        }

        public override void OnEnter()
        {
            result.boolValue = collection.CanAddItem(item);

            Finish();
        }
    }
}

#endif