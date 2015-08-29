#if PLAYMAKER

using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;


namespace Devdog.InventorySystem.Integration.PlayMaker
{
    [ActionCategory("Inventory Pro")]
    [HutongGames.PlayMaker.Tooltip("Use a given item.")]
    public class UseItem : FsmStateAction
    {
        public InventoryItemBase item;
        
        public override void Reset()
        {

        }

        public override void OnEnter()
        {
            item.Use();
            Finish();
        }
    }
}

#endif