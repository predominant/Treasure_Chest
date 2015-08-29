#if PLY_GAME

using System;
using System.Collections.Generic;
using System.Linq;
using plyBloxKit;
using UnityEngine;
using Devdog.InventorySystem.UI;

namespace Devdog.InventorySystem.Integration.plyGame.plyBlox
{
    [plyBlock("Inventory Pro", "UI", "Toggle window", BlockType.Action, ReturnValueType = typeof(UIToggleUIWindow))]
    public class UIToggleUIWindow : plyBlock
    {
        [plyBlockField("Window", ShowName = true, ShowValue = true, DefaultObject = typeof (UIWindow), EmptyValueName = "-error-", SubName = "UIWindow", Description = "The window you wish to manipulate.")]
        public UIWindow window;


        public override void Created()
        {
            blockIsValid = window != null;
        }

        public override BlockReturn Run(BlockReturn param)
        {
            window.Toggle();

            return BlockReturn.OK;
        }
    }
}

#endif