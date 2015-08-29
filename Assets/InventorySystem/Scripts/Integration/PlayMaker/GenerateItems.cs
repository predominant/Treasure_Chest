#if PLAYMAKER

using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Devdog.InventorySystem.Integration.PlayMaker
{
    [ActionCategory("Inventory Pro")]
    [HutongGames.PlayMaker.Tooltip("Generate a bunch of items that can be stored in a collection.")]
    public class GenerateItems : FsmStateAction
    {
        public FsmInt minAmount, maxAmount;
        
        public InventoryItemBase[] items;

        protected IItemGenerator generator;

        public override void Reset()
        {

        }

        public override void Awake()
        {
            base.Awake();
            generator = new BasicItemGenerator();

#if UNITY_EDITOR
            var m = Editor.FindObjectOfType<ItemManager>();
            generator.SetItems(m.items);
#else
            generator.SetItems(ItemManager.instance.items);
#endif
            generator.Generate(minAmount.Value, maxAmount.Value);
        }


        public override void OnEnter()
        {
            // Do something with items

            Finish();
        }

        public override bool Event(FsmEvent fsmEvent)
        {
            return base.Event(fsmEvent);

        }
    }
}

#endif