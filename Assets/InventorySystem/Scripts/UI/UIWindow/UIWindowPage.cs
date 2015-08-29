using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace Devdog.InventorySystem.UI
{
    using System.Linq;

    /// <summary>
    /// A page inside an UIWindow. When a tab is clicked the insides of the window are changed, this is a page.
    /// </summary>
    [AddComponentMenu("InventorySystem/UI Helpers/UIWindowPage")]
    public partial class UIWindowPage : UIWindow
    {

        [Header("Page specific")]
        public bool isDefaultPage = true;

        [SerializeField]
        protected bool _isEnabled = true;
        public bool isEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;

                if(isEnabled)
                {
                    myButton.enabled = false;

                }
                else
                {
                    Hide();
                    myButton.enabled = false;
                }
            }
        }

        /// <summary>
        /// The button that "triggers" this page. leave empty if there is no button.
        /// </summary>
        public UnityEngine.UI.Button myButton;

        /// <summary>
        /// Container that olds the items, if any.
        /// </summary>
        public RectTransform itemContainer;


        private UIWindow _windowParent;
        public UIWindow windowParent
        {
            get
            {
                if (_windowParent == null)
                    _windowParent = transform.parent.GetComponentInParent<UIWindow>();

                if (_windowParent == null)
                    Debug.LogWarning("No UIWindow found in parents (UIWindowPage can only be used as a child of UIWindow)", transform);

                return _windowParent;
            }
        }
        
        protected override void LevelStart()
        {
            base.LevelStart();

            windowParent.pages.Add(this);
            if (isDefaultPage)
                windowParent.currentPage = this;

            windowParent.OnShow += () =>
            {
                if (isDefaultPage && windowParent.pages.Sum(o => o.isVisible ? 1 : 0) == 0)
                    Show(true);
            };
            windowParent.OnHide += () =>
            {
                if(isVisible)
                    Hide(true);
            };
        }

        public override void Show(bool fireEvents)
        {
            if(isEnabled == false)
            {
                Debug.LogWarning("Trying to show a disabled UIWindowPage");
                return;
            }

            if (isVisible)
                return;

            base.Show(fireEvents);
            windowParent.currentPage = this;
            //windowParent.HideOtherPages(this);
            
            // Regular event is notified in base.Show()
            if (fireEvents)
            {
                windowParent.NotifyChildShown(this);
            }
        }

        protected override IEnumerator _Show(float waitTime, bool fireEvents = true)
        {
            windowParent.HideOtherPages(this, waitTime); // Already fires events.
            return base._Show(waitTime, fireEvents);
        }
    }
}