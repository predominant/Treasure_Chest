using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Devdog.InventorySystem.UI;

namespace Devdog.InventorySystem
{
    [RequireComponent(typeof(UIWindowInteractive))]
    public partial class SettingsMenuUI : MonoBehaviour
    {
        /// <summary>
        /// When true, each time the "keyCode" is pressed a window is hidden. When there are no longer any interactive windows visible the settings menu will be shown.
        /// When false, all windows will be hidden.
        /// </summary>
        public bool hideSingleWindowAtATime = true;

        /// <summary>
        /// When the settings menu is closed should the previously hidden windows be restored?
        /// </summary>
        public bool restoreWindowsAfterClose = true;

        [NonSerialized]
        private UIWindowInteractive[] hiddenWindows = new UIWindowInteractive[0];

        [NonSerialized]
        private UIWindowInteractive[] interactiveWindowsInScene = new UIWindowInteractive[0];

        private UIWindowInteractive _window;
        public UIWindowInteractive window
        {
            get
            {
                if (_window == null)
                    _window = GetComponent<UIWindowInteractive>();

                return _window;
            }
            set { _window = value; }
        }


        public void Start()
        {
            interactiveWindowsInScene = Resources.FindObjectsOfTypeAll<UIWindowInteractive>();

            //window.OnShow += HideInteractiveWindows;
            //if(restoreWindowsAfterClose)
            //    window.OnHide += RestoreInteractiveWindows;

            window.enabled = false; // Activate it once all windows are hidden.
        }

        public void Update()
        {
            if (window.keysDown)
            {
                HideInteractiveWindows();
            }
        }


        public virtual void HideInteractiveWindows()
        {
            // Show menu, hide current interactive hiddenWindows
            var l = new List<UIWindowInteractive>(interactiveWindowsInScene.Length);
            foreach (var w in interactiveWindowsInScene)
            {
                if (w.isVisible)
                {
                    w.Hide();
                    l.Add(w);

                    if (hideSingleWindowAtATime)
                        return;

                }
            }

            hiddenWindows = l.ToArray();
            window.Show();
        }

        public virtual void RestoreInteractiveWindows()
        {
            foreach (var w in hiddenWindows)
            {
                if(w.isVisible == false)
                    w.Show();

            }
        }
    }
}
