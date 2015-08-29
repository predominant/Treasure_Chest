using System;
using System.Collections.Generic;
using Devdog.InventorySystem.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Devdog.InventorySystem
{
    [RequireComponent(typeof (ItemManager))]
    [RequireComponent(typeof (InventorySettingsManager))]
    [RequireComponent(typeof (InventoryTriggererManager))]
    [RequireComponent(typeof (InventoryPlayerManager))]
    [RequireComponent(typeof (InventoryManager))]
    [AddComponentMenu("InventorySystem/Managers/Inventory Input Manager")]
    public partial class InventoryInputManager : MonoBehaviour
    {
        public delegate void ItemWrapperChanged(InventoryUIItemWrapperBase wrapper);
        public delegate void HoveringTriggererChanged(ObjectTriggererBase triggerer);

        public event ItemWrapperChanged OnCursorEnterWrapper;
        public event ItemWrapperChanged OnCursorExitWrapper;

        //public event HoveringTriggererChanged OnCursorEnterTriggerer;
        //public event HoveringTriggererChanged OnCursorExitTriggerer;


        [Tooltip("Set up a vertical input axis for navigating the UI like: DPadVertical")]
        public string controllerVerticalInputAxis = "";

        [Tooltip("Set up a horizontal input axis for navigating the UI like: DPadHorizontal")]
        public string controllerHorizontalInputAxis = "";


        private float _controllerVerticalInputDelta { get; set; }
        private float _controllerHorizontalInputDelta { get; set; }

        public ObjectTriggererBase currentlyHoveringTriggerer
        {
            set
            {
                InventoryTriggererManager.instance.currentlyHoveringTriggerer = value;
            }
        }


        private static InventoryUIItemWrapperBase _currentlyHoveringWrapper;

        public InventoryUIItemWrapperBase currentlyHoveringWrapper
        {
            get
            {
                return _currentlyHoveringWrapper;
            }
            set
            {
                // Setting it to null, notify the previous object.
                if (_currentlyHoveringWrapper != null && _currentlyHoveringWrapper != value)
                {
                    NotifyCursorExitWrapper(_currentlyHoveringWrapper);
                }

                var before = _currentlyHoveringWrapper;
                _currentlyHoveringWrapper = value;

                if (before != _currentlyHoveringWrapper && value != null)
                {
                    NotifyCursorEnterWrapper(_currentlyHoveringWrapper);
                }
            }
        }

        public static List<RaycastResult> cursorRaycastUIResults { get; private set; }
        private static Vector3 prevMouseOrTouchPosition { get; set; }
        private static int raycastLooper { get; set; }
        private static PointerEventData raycastPointerEventData { get; set; }


        public GameObject limitInputTo { get; set; }
        private static InventoryInputManager _instance;
        public static InventoryInputManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<InventoryInputManager>();
                }

                return _instance;
            }
        }

        private static float cursorNotMovedTimer { get; set; }
        private const float ForceCheckTimer = 0.1f;


        static InventoryInputManager()
        {
            cursorRaycastUIResults = new List<RaycastResult>(8);
            raycastPointerEventData = new PointerEventData(EventSystem.current);
        }

        public void Awake()
        {
            _instance = this;
        }

        public void Update()
        {
            CheckRaycastsWhenCursorMoves();
            CheckControllerInput();

            if (currentlyHoveringWrapper != null)
            {
                if (currentlyHoveringWrapper.gameObject.activeInHierarchy == false)
                {
                    currentlyHoveringWrapper = null;
                }
            }
        }

        private void CheckRaycastsWhenCursorMoves()
        {
            var newPosition = InventoryUIUtility.mouseOrTouchPosition;
            if (newPosition != prevMouseOrTouchPosition || cursorNotMovedTimer > ForceCheckTimer)
            {
                // Cursor moved
                ManuallyCheckHoveringTriggerers(newPosition);
                ManuallyCheckHoveringUIItems(newPosition);

                cursorNotMovedTimer = 0.0f;
            }

            prevMouseOrTouchPosition = newPosition;
            cursorNotMovedTimer += Time.deltaTime;
        }


        /// <summary>
        /// When the cursor enters an item
        /// </summary>
        private void NotifyCursorEnterWrapper(InventoryUIItemWrapperBase wrapper)
        {
            if (OnCursorEnterWrapper != null)
                OnCursorEnterWrapper(wrapper);

        }

        /// <summary>
        /// When the cursor exits an item
        /// </summary>
        /// <param name="wrapper"></param>
        /// <param name="eventData"></param>
        private void NotifyCursorExitWrapper(InventoryUIItemWrapperBase wrapper)
        {
            if (OnCursorExitWrapper != null)
                OnCursorExitWrapper(wrapper);

        }


        #region Manual cursor raycasting

        private void ManuallyCheckHoveringTriggerers(Vector2 pos)
        {
            currentlyHoveringTriggerer = GetHoveringTriggerer(pos);
        }

        private void ManuallyCheckHoveringUIItems(Vector2 pos)
        {
            cursorRaycastUIResults = GetAllHoveringUIObjects(pos);
            currentlyHoveringWrapper = GetWrapperFromRaycastResult(cursorRaycastUIResults);
        }

        private static InventoryUIItemWrapperBase GetWrapperFromRaycastResult(List<RaycastResult> results)
        {
            raycastLooper = 0;
            for (raycastLooper = 0; raycastLooper < results.Count; raycastLooper++)
            {
                // GetComponent<>() causes GC in the editor, but no in final build version.
                var wrapper = results[raycastLooper].gameObject.GetComponent<InventoryUIItemWrapperBase>();
                if (wrapper != null)
                    return wrapper;
            }

            return null;
        }

        /// <summary>
        /// Cast a ray to test if Input.mousePosition is over any UI object in EventSystem.current.
        /// This is a replacement for IsPointerOverGameObject() which does not work on Android
        /// </summary>
        private static List<RaycastResult> GetAllHoveringUIObjects(Vector2 pos)
        {
            cursorRaycastUIResults.Clear();

            raycastPointerEventData = new PointerEventData(EventSystem.current)
            {
                position = pos
            };

            if (EventSystem.current != null)
                EventSystem.current.RaycastAll(raycastPointerEventData, cursorRaycastUIResults);

            return cursorRaycastUIResults;
        }


        /// <summary>
        /// Cast a ray to test if Input.mousePosition is over any UI object in EventSystem.current.
        /// This is a replacement for IsPointerOverGameObject() which does not work on Android
        /// </summary>
        private static ObjectTriggererBase GetHoveringTriggerer(Vector2 pos)
        {
            if (Camera.main == null)
            {
                //Debug.LogWarning("No camera in scene is marked with MainCamera tag.");
                return null;
            }

            RaycastHit hitInfo;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(pos), out hitInfo, Camera.main.farClipPlane))
            {
                var triggerer = hitInfo.transform.gameObject.GetComponent<ObjectTriggererBase>();
                if (triggerer != null)
                {
                    if (InventoryUIUtility.isHoveringUIElement)
                        return null;

                    if (triggerer.inRange == false)
                        return null;

                    return triggerer;
                }
            }
            
            return null;
        }

        #endregion


        #region Controller input

        private void CheckControllerInput()
        {
            if (InventoryUIUtility.CanReceiveInput(InventoryUIUtility.currentlySelectedGameObject) == false)
                return;

            if (controllerVerticalInputAxis == "" || controllerHorizontalInputAxis == "")
                return;

            var controllerVerticalInput = Input.GetAxis(controllerVerticalInputAxis);
            var controllerHorizontalInput = Input.GetAxis(controllerHorizontalInputAxis);

            var current = EventSystem.current;
            if (current == null || current.currentSelectedGameObject == null)
                return;

            // Already did something last frame (same as OnDown, assuring it only fires once, untill key is released)
            if (_controllerVerticalInputDelta != 0.0f || _controllerHorizontalInputDelta != 0.0f)
            {
                _controllerVerticalInputDelta = controllerVerticalInput;
                _controllerHorizontalInputDelta = controllerHorizontalInput;
                return;
            }

            // To avoid constant checking
            if (controllerVerticalInput == 0.0f && controllerHorizontalInput == 0.0f)
                return;

            if (current.currentSelectedGameObject == null)
                return;

            var selectable = current.currentSelectedGameObject.GetComponent<Selectable>();
            if (controllerVerticalInput != 0.0f)
            {
                if (controllerVerticalInput < 0)
                {
                    var down = selectable.FindSelectableOnDown();
                    if (down != null)
                        down.Select();
                }
                else
                {
                    var up = selectable.FindSelectableOnUp();
                    if (up != null)
                        up.Select();
                }
            }
            else if (controllerHorizontalInput != 0.0f)
            {
                if (controllerHorizontalInput < 0)
                {
                    var left = selectable.FindSelectableOnLeft();
                    if (left != null)
                        left.Select();
                }
                else
                {
                    var right = selectable.FindSelectableOnRight();
                    if (right != null)
                        right.Select();
                }
            }

            _controllerVerticalInputDelta = controllerVerticalInput;
            _controllerHorizontalInputDelta = controllerHorizontalInput;
        }


        #endregion
    }
}
