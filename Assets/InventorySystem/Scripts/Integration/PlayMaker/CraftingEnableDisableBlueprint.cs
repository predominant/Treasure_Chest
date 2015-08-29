#if PLAYMAKER

using UnityEngine;
using System.Collections;
using Devdog.InventorySystem.Models;
using HutongGames.PlayMaker;

namespace Devdog.InventorySystem.Integration.PlayMaker
{

    [ActionCategory("Inventory Pro")]
    [HutongGames.PlayMaker.Tooltip("Enable or disable a blueprint")]
    public class CraftingEnableDisableBlueprint : FsmStateAction
    {
        public FsmInt blueprintID;
        public FsmBool learned;


        public override void Reset()
        {

        }

        public override void OnEnter()
        {
            foreach (var cat in ItemManager.instance.craftingCategories)
            {
                foreach (var b in cat.blueprints)
                {
                    if (b.ID == (uint) blueprintID.Value)
                    {
                        b.playerLearnedBlueprint = learned.Value;
                        Finish();

                        return;
                    }
                }
            }


            Debug.LogWarning("Error, can't set blueprint with ID " + blueprintID.Value);
        }
    }
}

#endif