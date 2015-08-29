#if PLAYMAKER

using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;


namespace Devdog.InventorySystem.Integration.PlayMaker
{
    [ActionCategory("Inventory Pro")]
    [HutongGames.PlayMaker.Tooltip("Use a given item.")]
    public class DropThisItem : FsmStateAction
    {

        protected InventoryItemBase item;

        public override void Awake()
        {
            base.Awake();

            item = this.Owner.GetComponent<InventoryItemBase>();
        }

        public override void Reset()
        {

        }

        public override void OnEnter()
        {
            item.itemCollection[item.index].TriggerDrop();
            Finish();
        }
    }
}

#endif