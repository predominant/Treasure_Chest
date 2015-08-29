#if PLY_GAME

using System;
using System.Collections.Generic;
using System.Linq;
using plyBloxKit;
using UnityEngine;

namespace Devdog.InventorySystem.Integration.plyGame.plyBlox
{
    [plyBlock("Inventory Pro", "Actions", "Use Object triggererHandler", BlockType.Action)]
    public class UseObjectTriggerer : plyBlock
    {
        [plyBlockField("Triggerer", ShowName = true, ShowValue = true, DefaultObject = typeof(ObjectTriggerer), EmptyValueName = "-error-", SubName = "Object Triggerer", Description = "The triggererHandler you wish to use / unuse")]
        public ObjectTriggerer triggerer;

        [plyBlockField("Action", ShowName = true, ShowValue = true, DefaultObject = typeof(Bool_Value), SubName = "Action", Description = "The action on the triggererHandler")]
        public Bool_Value action;

        public override void Created()
        {
            blockIsValid = (triggerer != null);

            if (triggerer == null)
                Log(LogType.Error, "Triggerer is empty");
        }

        public override BlockReturn Run(BlockReturn param)
        {
            bool removeSource;
            if (action.value)
                triggerer.Use(out removeSource);
            else
                triggerer.UnUse();

            return BlockReturn.OK;
        }
    }
}

#endif