#if UFPS

using UnityEngine;
using System.Collections;
using Devdog.InventorySystem;
using Devdog.InventorySystem.Models;
using Devdog.InventorySystem.UI;


namespace Devdog.InventorySystem.Integration.UFPS
{
    [AddComponentMenu("InventorySystem/Integration/UFPS/Inventory UFPS Input Controller")]
    public partial class InventoryUFPSInputController : MonoBehaviour, IInventoryPlayerController
    {
        [InventoryRequired]
        public vp_FPInput input;

        [InventoryRequired]
        public vp_SimpleCrosshair crosshair;
        public bool hideCrosshairOnBlockingWindow = true;

        /// <summary>
        /// Close the windows when you click on the world.
        /// </summary>
        public bool closeWindowsWhenClickWorld;

        /// <summary>
        /// Auto hide the cursor when windows are shown / hidden
        /// </summary>
        public bool hideCursorOnNoBlockingWindows = true;

        private static int windowCounter;
        private static bool registered = true;
        private static UIWindow[] windows;
        private static float lastWindowShownTime = 0.0f;

        // Start, to make sure all Awakes are done.
        public virtual void Start()
        {
            if (hideCursorOnNoBlockingWindows)
                Cursor.visible = false;

            windows = Resources.FindObjectsOfTypeAll<UIWindow>();
            foreach (var w in windows)
            {
                var window = w; // Capture list and all...
                if (window.blockUFPSInput)
                {
                    if (window.isVisible)
                        windowCounter++;

                    window.OnShow += () =>
                    {
                        lastWindowShownTime = Time.time;
                        windowCounter++;

                        if (windowCounter > 0 && registered)
                            SetActive(false);
                    };

                    window.OnHide += () =>
                    {
                        windowCounter--;

                        if (windowCounter == 0 && registered == false)
                            SetActive(true);
                    };
                }
            }

            if (windowCounter > 0)
            {
                SetActive(false);
                //Cursor.visible = true;
            }
        }

        public void Update()
        {
            // Auto close window when movement is pressed.
            if (vp_Input.GetAxisRaw("Horizontal") != 0.0f || vp_Input.GetAxisRaw("Vertical") != 0.0f)
            {
                if (Time.time > lastWindowShownTime + 0.4f)
                {
                    HideAllWindows();
                }
            }

            if (closeWindowsWhenClickWorld)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (InventoryUIUtility.isHoveringUIElement == false)
                    {
                        input.MouseCursorBlocksMouseLook = true;
                        HideAllWindows();
                    }
                }
            }
        }


        public virtual void SetActive(bool set)
        {
            input.enabled = set;

            registered = set;
            vp_Utility.LockCursor = set;

            if (hideCursorOnNoBlockingWindows)
                Cursor.visible = !set;

            if (hideCrosshairOnBlockingWindow)
                crosshair.enabled = set;
        }

        protected virtual void HideAllWindows()
        {
            if (windowCounter == 0)
                return;

            foreach (var window in windows)
            {
                if (window.isVisible && window.blockUFPSInput)
                    window.Hide();
            }

            windowCounter = 0;
        }
    }
}

#else

using UnityEngine;
using System.Collections;
using Devdog.InventorySystem;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem.Integration.UFPS
{
    public class InventoryUFPSInputController : MonoBehaviour
    {
        // No UFPS, No fun stuff...
    }
}

#endif