using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Devdog.InventorySystem.Dialogs;
using Devdog.InventorySystem.UI;


namespace Devdog.InventorySystem
{
    using Devdog.InventorySystem.Models;

    using UnityEngine.Serialization;

    /// <summary>
    /// Used to trigger a physical object such as vendor, treasure chests etc.
    /// </summary>
    [AddComponentMenu("InventorySystem/Triggers/Object triggererHandler")]
    public partial class ObjectTriggerer : ObjectTriggererBase
    {
        #region Events

        public delegate void TriggerUse(InventoryPlayer player);
        public delegate void TriggerUnUse(InventoryPlayer player);

        public event TriggerUse OnTriggerUse;
        public event TriggerUnUse OnTriggerUnUse;

        #endregion

        [SerializeField]
        private bool _triggerMouseClick = true;
        public override bool triggerMouseClick
        {
            get { return _triggerMouseClick; }
            set { _triggerMouseClick = value; }

        }

        [SerializeField]
        private KeyCode _triggerKeyCode = KeyCode.None;
        public override KeyCode triggerKeyCode
        {
            get { return _triggerKeyCode; }
            set { _triggerKeyCode = value; }
        }

        public override InventoryCursorIcon cursorIcon
        {
            get { return InventorySettingsManager.instance.useCursor; }
            set { InventorySettingsManager.instance.useCursor = value; }
        }

        public override Sprite uiIcon
        {
            get { return InventorySettingsManager.instance.objectTriggererFPSUseSprite; }
            set { InventorySettingsManager.instance.objectTriggererFPSUseSprite = value; }
        }

        /// <summary>
        /// When true the window will be triggered directly, if false, a 2nd party will have to handle it through events.
        /// </summary>
        [HideInInspector]
        [NonSerialized]
        public bool handleWindowDirectly = true;

        /// <summary>
        /// Toggle when triggered
        /// </summary>
        public bool toggleWhenTriggered = true;

        /// <summary>
        /// Only required if handling the window directly
        /// </summary>
        [Header("The window")]
        [FormerlySerializedAs("window")]
        [SerializeField]
        private UIWindow _window;

        public UIWindow window
        {
            get
            {
                return _window;
            }
            set
            {
                _window = value;
            }
        }


        [Header("Animations & Audio")]
        public AnimationClip useAnimation;
        public AnimationClip unUseAnimation;

        public AudioClip useAudioClip;
        public AudioClip unUseAudioClip;


        public Animator animator { get; protected set; }



        /// <summary>
        /// Check if there's a other component on this object that can handle the triggerer.
        /// (Called by editor, don't remove)
        /// </summary>
        public bool isControlledByOther
        {
            get
            {
                return GetComponent(typeof(IObjectTriggerUser)) != null;
            }
        }

        protected static ObjectTriggerer previousTriggerer { get; set; }


        public override void Awake()
        {
            base.Awake();

            isActive = false;
            animator = GetComponent<Animator>();
        }

        private void WindowOnHide()
        {
            UnUse();
        }

        private void WindowOnShow()
        {

        }

        public override void NotifyCameInRange(InventoryPlayer player)
        {
            
        }

        public override void NotifyWentOutOfRange(InventoryPlayer player)
        {
            UnUse();
        }


        public override void OnMouseDown()
        {
            if (enabled == false)
                return;

            if (triggerMouseClick && InventoryUIUtility.isHoveringUIElement == false)
            {
                if (toggleWhenTriggered)
                    Toggle();
                else
                    Use();
            }
        }


        public override bool Toggle(InventoryPlayer player, out bool removeSource, bool fireEvents = true)
        {
            if (window != null && window.isVisible && isActive)
            {
                removeSource = false; // Never destroy an ObjectTriggerer, they're static.
                return UnUse(player, fireEvents);
            }
            else
            {
                return Use(player, out removeSource, fireEvents);
            }
        }

        public void DoVisuals()
        {
            if (useAnimation != null && animator != null)
                animator.Play(useAnimation.name);

            if (useAudioClip != null)
                InventoryUtility.AudioPlayOneShot(useAudioClip);
        }

        public void UndoVisuals()
        {
            if (unUseAnimation != null && animator != null)
                animator.Play(unUseAnimation.name);

            if (unUseAudioClip != null)
                InventoryUtility.AudioPlayOneShot(unUseAudioClip);
        }


        public override bool Use(out bool removeSource, bool fireEvents = true)
        {
            return Use(InventoryPlayerManager.instance.currentPlayer, out removeSource, fireEvents);
        }

        public override bool Use(InventoryPlayer player, out bool removeSource, bool fireEvents = true)
        {
            removeSource = false; // ObjectTriggers are "static" and aren't moved / changed after usage.

            if (enabled == false || inRange == false)
                return false;

            if (isActive)
                return true;

            if (previousTriggerer != null)
            {
                previousTriggerer.UnUse(player, fireEvents);
            }

            if (window != null)
            {
                window.OnShow += WindowOnShow;
                window.OnHide += WindowOnHide;

                if (handleWindowDirectly && fireEvents)
                {
                    if (toggleWhenTriggered)
                        window.Toggle();
                    else if (window.isVisible == false)
                        window.Show();
                }
            }
            else
            {
                Debug.LogWarning("Triggerer has no window", transform);
            }

            DoVisuals();

            isActive = true;

            if (OnTriggerUse != null && fireEvents)
                OnTriggerUse(player);

            InventoryTriggererManager.instance.NotifyTriggererUsed(this);
            previousTriggerer = this;

            return true;
        }

        public override bool UnUse(bool fireEvents = true)
        {
            return UnUse(InventoryPlayerManager.instance.currentPlayer, fireEvents);
        }

        public override bool UnUse(InventoryPlayer player, bool fireEvents = true)
        {
            if (enabled == false || inRange == false)
                return false;

            if (isActive == false)
                return true;

            if (handleWindowDirectly && fireEvents && window != null)
            {
                window.Hide();
            }

            UndoVisuals();


            isActive = false;

            if (window != null)
            {
                window.OnShow -= WindowOnShow;
                window.OnHide -= WindowOnHide;
            }

            if (OnTriggerUnUse != null && fireEvents)
                OnTriggerUnUse(player);

            InventoryTriggererManager.instance.NotifyTriggererUnUsed(this);

            previousTriggerer = null;
            return true;
        }
    }
}