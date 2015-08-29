#if PLY_GAME

using System;
using System.Collections.Generic;
using System.Linq;
using plyBloxKit;
using UnityEngine;

namespace Devdog.InventorySystem.Integration.plyGame.plyBlox
{
    [plyBlock("Inventory Pro", "Actions", "Toggle Object triggererHandler", BlockType.Action)]
    public class ToggleObjectTriggerer : plyBlock
    {
        [plyBlockField("Triggerer", ShowName = true, ShowValue = true, DefaultObject = typeof(ObjectTriggerer), EmptyValueName = "-error-", SubName = "Object Triggerer", Description = "The triggererHandler you wish to use / unuse")]
        public ObjectTriggerer triggerer;

        public override void Created()
        {
            blockIsValid = (triggerer != null);

            if (triggerer == null)
                Log(LogType.Error, "Triggerer is empty");
        }

        public override BlockReturn Run(BlockReturn param)
        {
            triggerer.Toggle();
            return BlockReturn.OK;
        }
    }
}

#endif