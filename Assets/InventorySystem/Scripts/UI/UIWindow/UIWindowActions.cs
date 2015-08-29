using System;
using UnityEngine;
using System.Collections;

namespace Devdog.InventorySystem.UI
{
    [RequireComponent(typeof(UIWindow))]
    [AddComponentMenu("InventorySystem/UI Helpers/UI Window Actions")]
    public partial class UIWindowActions : MonoBehaviour
    {
        [System.Serializable]
        public class UIWindowActionEvent : UnityEngine.Events.UnityEvent
        {
            
        }

        public UIWindowActionEvent onShowActions = new UIWindowActionEvent();
        public UIWindowActionEvent onHideActions = new UIWindowActionEvent();


        public void Awake()
        {
            var window = GetComponent<UIWindow>();
            if (window == null)
            {
                Debug.LogWarning("UIWindowActions needs an UIWindow component!", transform);
                return;
            }

            window.OnShow += WindowOnShow;
            window.OnHide += WindowOnHide;
        }

        private void WindowOnShow()
        {
            onShowActions.Invoke();
        }

        private void WindowOnHide()
        {
            onHideActions.Invoke();
        }
    }
}