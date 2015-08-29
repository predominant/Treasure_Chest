#if PLAYMAKER

using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;


namespace Devdog.InventorySystem.Integration.PlayMaker
{
    [ActionCategory("Inventory Pro")]
    [HutongGames.PlayMaker.Tooltip("Use a given item.")]
    public class OpenCloseTreasureChest : FsmStateAction
    {
        public LootableObject chest;
        public FsmBool open;
        
        public override void Reset()
        {

        }

        public override void OnEnter()
        {
            bool removeSource;
            if (open.Value)
                chest.triggerer.Use(out removeSource);
            else
                chest.triggerer.UnUse();

            Finish();
        }
    }
}

#endif