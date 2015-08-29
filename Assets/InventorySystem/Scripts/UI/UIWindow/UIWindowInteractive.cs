using System;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

namespace Devdog.InventorySystem.UI
{
    /// <summary>
    /// Any window that you want to hide or show through key combination or a helper.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [AddComponentMenu("InventorySystem/UI Helpers/UI Interactive Window")]
    public partial class UIWindowInteractive : UIWindow
    {
        #region Variables 


        /// <summary>
        /// Keys to toggle this window
        /// </summary>
        public KeyCode[] keyCombination;


        public bool keysDown
        {
            get
            {
                if (keyCombination.Length == 0)
                    return false;

                foreach (var keyCode in keyCombination)
                {
                    if (Input.GetKeyDown(keyCode))
                        return true;

                }

                return false;
                //return keyCombination.Any(key => Input.GetKeyDown(key));
            }
        }


        #endregion


        public virtual void Update()
        {
            if (keysDown)
            {
                if (InventoryUIUtility.CanReceiveInput(gameObject))
                    Toggle();

            }
        }
    }
}