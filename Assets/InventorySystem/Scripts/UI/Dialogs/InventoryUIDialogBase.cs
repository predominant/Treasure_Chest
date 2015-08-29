using UnityEngine;
using System.Collections;
using Devdog.InventorySystem.Dialogs;
using UnityEngine.UI;
using Devdog.InventorySystem.UI;

namespace Devdog.InventorySystem.Dialogs
{

    public delegate void InventoryUIDialogCallback(InventoryUIDialogBase dialog);

    /// <summary>
    /// The abstract base class used to create all dialogs. If you want to create your own dialog, extend from this class.
    /// </summary>
    [RequireComponent(typeof (Animator))]
    [RequireComponent(typeof (UIWindow))]
    public abstract partial class InventoryUIDialogBase : MonoBehaviour
    {
        [Header("UI")] public Text titleText;
        public Text descriptionText;

        public UnityEngine.UI.Button yesButton;
        public UnityEngine.UI.Button noButton;

        /// <summary>
        /// The item that should be selected by default when the dialog opens.
        /// </summary>
        [Header("Behavior")]
        public bool disableSelectOnOpenDialogOnMobile = true;
        public Selectable selectOnOpenDialog;

        /// <summary>
        /// When enabled the window will be positioned on top of the caller's window.
        /// </summary>
        public bool positionOnTopOfCaller;

        /// <summary>
        /// Disables the items defined in InventorySettingsManager.disabledWhileDialogActive if set to true.
        /// </summary>
        public bool disableElementsWhileActive = true;

        protected CanvasGroup canvasGroup { get; set; }
        protected Animator animator { get; set; }
        public UIWindow window { get; protected set; }

        public UIWindow dialogCallerWindow { get; protected set; }

        private Transform _dialogCaller;
        public Transform dialogCaller
        {
            get { return _dialogCaller; }
            set
            {
                _dialogCaller = value;
                if(dialogCaller != null)
                    dialogCallerWindow = _dialogCaller.GetComponent<UIWindow>();
            }
        }

        public virtual void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();

            animator = GetComponent<Animator>();
            window = GetComponent<UIWindow>();


            window.OnShow += () =>
            {
                SetEnabledWhileActive(false); // Disable other UI elements

                if (Application.isMobilePlatform && disableSelectOnOpenDialogOnMobile == false)
                {
                    if (selectOnOpenDialog != null)
                        selectOnOpenDialog.Select();

                }
            };
            window.OnHide += () =>
            {
                SetEnabledWhileActive(true); // Enable other UI elements
            };
        }

        public virtual void Update()
        {
            
        }


        private void OnDialogCallerWindowHidden()
        {
            if(dialogCallerWindow != null)
                dialogCallerWindow.OnHide -= OnDialogCallerWindowHidden;

            if (window.isVisible && dialogCallerWindow != null && dialogCallerWindow.isVisible == false)
                window.Hide();
        }

        public void Toggle()
        {
            window.Toggle();
            SetEnabledWhileActive(!window.isVisible);
        }

        /// <summary>
        /// Disables elements of the UI when a dialog is active. Useful to block user actions while presented with a dialog.
        /// </summary>
        /// <param name="enabled">Should the items be disabled?</param>
        protected virtual void SetEnabledWhileActive(bool enabled)
        {
            if (disableElementsWhileActive == false)
                return;

            if(enabled)
                InventoryInputManager.instance.limitInputTo = null;
            else
                InventoryInputManager.instance.limitInputTo = gameObject;


            foreach (var item in InventorySettingsManager.instance.disabledWhileDialogActive)
            {
                var group = item.gameObject.GetComponent<CanvasGroup>();
                if (group == null)
                    group = item.gameObject.AddComponent<CanvasGroup>();

                group.blocksRaycasts = enabled;
                group.interactable = enabled;
            }
        }


        /// <summary>
        /// Called when a dialog is shown
        /// </summary>
        /// <param name="dialogCaller">The gameObject that is responsible for opening this dialog.</param>
        protected virtual void NotifyDialogShown(Transform dialogCaller)
        {
            this.dialogCaller = dialogCaller;

            if (dialogCallerWindow != null)
                dialogCallerWindow.OnHide += OnDialogCallerWindowHidden;

            //if (InventorySettingsManager.instance.isUIWorldSpace)
            //{
                //transform.SetParent(dialogCaller.transform);
                //transform.SetSiblingIndex(9999); // Make sure it's the last object, so it's drawn on top.

            if (positionOnTopOfCaller && dialogCaller != null)
            {
                transform.position = dialogCaller.position + (-dialogCaller.forward * 0.5f);
                transform.rotation = dialogCaller.rotation;
            }
            //}
        }
    }
}