using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Devdog.InventorySystem.Demo
{
    public class ObjectTriggererHandlerEventTester : MonoBehaviour
    {

        public void Start()
        {
            InventoryTriggererManager.instance.OnCursorEnterTriggerer += (triggerer) => Debug.Log("Cursor entered triggerer");
            InventoryTriggererManager.instance.OnCursorExitTriggerer += (triggerer) => Debug.Log("Cursor exited triggerer");
            InventoryTriggererManager.instance.OnChangedBestTriggerer += (before, after) => Debug.Log("Changed best triggerer");
        }
    }
}
