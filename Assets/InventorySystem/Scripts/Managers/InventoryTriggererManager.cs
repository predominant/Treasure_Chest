using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.UI;
using UnityEngine;

namespace Devdog.InventorySystem
{
    [AddComponentMenu("InventorySystem/Managers/Inventory triggerer manager")]
    [RequireComponent(typeof(InventorySettingsManager))]
    [RequireComponent(typeof(InventoryManager))]
    [RequireComponent(typeof(InventoryPlayerManager))]
    [RequireComponent(typeof(ItemManager))]
    [RequireComponent(typeof(InventoryInputManager))]
    public partial class InventoryTriggererManager : MonoBehaviour
    {
        #region Events

        public delegate void NewBestTriggerer(ObjectTriggererBase old, ObjectTriggererBase newBest);
        public delegate void ChangedTriggerer(ObjectTriggererBase triggerer);

        public event NewBestTriggerer OnChangedBestTriggerer;
        public event ChangedTriggerer OnCursorEnterTriggerer;
        public event ChangedTriggerer OnCursorExitTriggerer;

        #endregion

        private ObjectTriggererBase _currentlyHoveringTriggerer;
        private bool isHoveringTriggerer; // Triggerers can get destroyed, causing the Exit not to fire.
        public ObjectTriggererBase currentlyHoveringTriggerer
        {
            get { return _currentlyHoveringTriggerer; }
            set
            {
                // Setting it to null, notify the previous object ( Triggerers can get destroyed, causing the Exit not to fire, hence the manual isHoveringTriggerer check. )
                if ((_currentlyHoveringTriggerer != null && _currentlyHoveringTriggerer != value) || (isHoveringTriggerer && _currentlyHoveringTriggerer == null))
                {
                    NotifyCursorExitTriggerer(_currentlyHoveringTriggerer);
                }

                var before = _currentlyHoveringTriggerer;
                _currentlyHoveringTriggerer = value;

                if (before != _currentlyHoveringTriggerer && value != null)
                {
                    NotifyCursorEnterTriggerer(_currentlyHoveringTriggerer);
                }
            }
        }


        private ObjectTriggererBase _bestTriggerer;
        public ObjectTriggererBase bestTriggerer
        {
            get { return _bestTriggerer; }
            set
            {
                var before = _bestTriggerer;
                _bestTriggerer = value;

                if (before != value)
                {
                    NotifyNewBestTriggerer(before, value);
                }
            }
        }


        protected static InventorySettingsManager settings { get; set; }
        private static InventoryTriggererManager _instance;
        public static InventoryTriggererManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<InventoryTriggererManager>();
                }

                return _instance;
            }
        }

        public void Awake()
        {
            _instance = this;
            settings = InventorySettingsManager.instance;
        }


        #region Notifies

        protected void NotifyNewBestTriggerer(ObjectTriggererBase before, ObjectTriggererBase after)
        {
            // New best triggerer   
            if (OnChangedBestTriggerer != null)
                OnChangedBestTriggerer(before, after);

        }

        public void NotifyCursorEnterTriggerer(ObjectTriggererBase triggerer)
        {
            triggerer.NotifyCursorEnterTriggerer();
            isHoveringTriggerer = true;

            // New best triggerer
            if (OnCursorEnterTriggerer != null)
                OnCursorEnterTriggerer(triggerer);

        }

        public void NotifyCursorExitTriggerer(ObjectTriggererBase triggerer)
        {
            triggerer.NotifyCursorExitTriggerer();
            isHoveringTriggerer = false;

            if (OnCursorExitTriggerer != null)
                OnCursorExitTriggerer(triggerer);

        }

        public void NotifyTriggererUsed(ObjectTriggererBase triggerer)
        {

        }

        public void NotifyTriggererUnUsed(ObjectTriggererBase triggerer)
        {

        }

        #endregion

    }
}
