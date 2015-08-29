#if PLAYMAKER

using UnityEngine;
using System.Collections;
using Devdog.InventorySystem.Dialogs;
using Devdog.InventorySystem.Models;
using HutongGames.PlayMaker;
using Devdog.InventorySystem.UI;

namespace Devdog.InventorySystem.Integration.PlayMaker
{

    [ActionCategory("Inventory Pro")]
    [HutongGames.PlayMaker.Tooltip("Show or hide a dialog")]
    public class UIShowHideUIWindow : FsmStateAction
    {
        public FsmBool show;
        public UIWindow window;

        public override void Reset()
        {

        }

        public override void OnEnter()
        {
            if (show.Value)
                window.Show();
            else
                window.Hide();
        }
    }
}

#endif