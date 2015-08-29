using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Devdog.InventorySystem.Models;
using Devdog.InventorySystem.UI;

namespace Devdog.InventorySystem
{
    [AddComponentMenu("InventorySystem/UI Wrappers/UI Wrapper")]
    public partial class InventoryUIItemWrapper : InventoryUIItemWrapperBase, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerUpHandler, IPointerDownHandler // , IPointerEnterHandler, IPointerExitHandler
    {

        #region Variables


        /// <summary>
        /// The icon that was set when the object started.
        /// If null, the default will be chosen.
        /// </summary>
        [NonSerialized]
        private Sprite startIcon;

        #region UI Elements

        public UnityEngine.UI.Text amountText;
        public UnityEngine.UI.Text itemName;
        //public Text keyCombinationText;
        public UnityEngine.UI.Image icon;

        public UIShowValueModel cooldownVisualizer = new UIShowValueModel();
        //public UnityEngine.UI.Image cooldownImage;

        public override Material material
        {
            get
            {
                return icon.material;
            }
            set
            {
                icon.material = value;
            }
        }

        //public Button button;

        #endregion


    
        public virtual bool isEmpty
        {
            get
            {
                return item == null;
            }
        }

        /// <summary>
        /// Converts the rect transform's rect to screen space (adds the position to it)
        /// </summary>
        public virtual Rect screenSpaceRect
        {
            get
            {
                var rectTransform = GetComponent<RectTransform>();
                var pos = rectTransform.position;
                var r = new Rect(rectTransform.rect);
                
                r.x += pos.x;
                r.y += pos.y;

                r.width += InventorySettingsManager.instance.onPointerUpInsidePadding.x;
                r.height += InventorySettingsManager.instance.onPointerUpInsidePadding.y;

                r.x -= (InventorySettingsManager.instance.onPointerUpInsidePadding.x / 2);
                r.y -= (InventorySettingsManager.instance.onPointerUpInsidePadding.y / 2);

                return r;
            }
        }



        [NonSerialized]
        protected static InventoryUIItemWrapperBase pointerDownOnUIElement;

        private bool isPressingButton
        {
            get { return InventoryUIUtility.currentlyHoveringWrapper == this && (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2)); }
        }

        private bool isLongPress
        {
            get
            {
                return lastDownTime != 0.0f && Time.timeSinceLevelLoad - InventorySettingsManager.instance.mobileLongPressTime > lastDownTime;                
            }
        }

        private bool isDoubleTap
        {
            get { return lastDownTime != 0.0f && Time.timeSinceLevelLoad < lastDownTime + InventorySettingsManager.instance.mobileDoubleTapTime; }
        }


        /// <summary>
        /// Last time the button was pressed, used to determine long presses.
        /// </summary>
        protected static float lastDownTime { get; set; }


        #endregion



        public virtual void Awake()
        {
            startIcon = icon.sprite;
        }


        public virtual void Update()
        {
            RepaintCooldown();

            if (isPressingButton && InventoryUIDragUtility.isDraggingItem == false && isLongPress)
                OnLongTap(new PointerEventData(EventSystem.current), InventoryActionInput.EventType.All); // Long press for mobile
        }

        public virtual void OnDisable()
        {
            // Force end it if the original is disabled / destroyed.
            if (InventoryUIDragUtility.isDraggingItem)
                InventoryUIDragUtility.OnEndDrag(new PointerEventData(EventSystem.current));

        }

        #region Button handler UI events

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (itemCollection == null || item == null || itemCollection.canDragInCollection == false)
                return;

            if (item != null && itemCollection.canDragInCollection)
            {
                InventoryUIDragUtility.OnBeginDrag(this, eventData);
            }
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (itemCollection == null || item == null || itemCollection.canDragInCollection == false)
                return;


            InventoryUIDragUtility.OnDrag(eventData);

            //if (eventData.button == PointerEventData.InputButton.Left || (eventData.button == PointerEventData.InputButton.Right && InventorySettingsManager.instance.unstackItemButton == PointerEventData.InputButton.Right))
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (itemCollection == null || item == null || itemCollection.canDragInCollection == false)
                return;

            var lookup = InventoryUIDragUtility.OnEndDrag(eventData);
            if (lookup == null)
                return; // No drag handler.

            if (lookup.endOnWrapper)
            {
                // Place on a slot
                if (InventorySettingsManager.instance.useUnstackDrag && lookup.endItemCollection.useReferences == false && lookup.endItemCollection.canUnstackItemsInCollection)
                {
                    if (InventorySettingsManager.instance.unstackKeys.AllPressed(eventData, InventoryActionInput.EventType.All))
                    {
                        TriggerUnstack(lookup.endItemCollection, lookup.endIndex);
                        return; // Stop the rest otherwise we'll do 2 actions at once.                        
                    }
                }

                lookup.startItemCollection.SwapOrMerge((uint)lookup.startIndex, lookup.endItemCollection, (uint)lookup.endIndex);
            }
            else if (lookup.startItemCollection.useReferences)
            {
                lookup.startItemCollection.SetItem((uint)lookup.startIndex, null);
                lookup.startItemCollection[lookup.startIndex].Repaint();
            }
            else if (InventoryUIUtility.isHoveringUIElement == false)
            {
                TriggerDrop();
            }
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (itemCollection == null)
                return;

            pointerDownOnUIElement = InventoryUIUtility.currentlyHoveringWrapper;
            if (pointerDownOnUIElement == null)
                return;


            //////////// Ugly hack, because input modules will take over soon.
            bool tapped = OnTap(eventData, InventoryActionInput.EventType.OnPointerDown); // Mobile version of OnPointerUp
            if (tapped)
            {
                return;
            }

            if (InventorySettingsManager.instance.useContextMenu && InventorySettingsManager.instance.triggerContextMenuKeys.AllPressed(eventData, InventoryActionInput.EventType.OnPointerDown))
            {
                if (item != null)
                {
                    TriggerContextMenu();
                    return;
                }
            }

            if (isDoubleTap)
            {
                OnDoubleTap(eventData, InventoryActionInput.EventType.OnPointerDown);
                return;
            }

            lastDownTime = Time.timeSinceLevelLoad;
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            // Started on a UI element?
            if (pointerDownOnUIElement == null)
                return;

            pointerDownOnUIElement = null;

            // Cursor still inside the button on Pointer up?
            var canvas = gameObject.GetComponentInParent<Canvas>();
            if(canvas.renderMode != RenderMode.WorldSpace)
            {
                if (screenSpaceRect.Contains(eventData.position) == false)
                    return;
            }

            if (InventorySettingsManager.instance.useContextMenu || itemCollection == null)
                return;

            bool tapped = OnTap(eventData, InventoryActionInput.EventType.OnPointerUp); // Mobile version of OnPointerUp
            if (tapped)
            {
                return;
            }

            if (isDoubleTap)
            {
                OnDoubleTap(eventData, InventoryActionInput.EventType.OnPointerUp);
                return;
            }

            if (item != null && InventoryUIDragUtility.isDraggingItem == false)
            {
                // Check if we're trying to unstack
                if (itemCollection.useReferences == false && InventorySettingsManager.instance.useUnstackClick && itemCollection.canUnstackItemsInCollection)
                {
                    if (InventorySettingsManager.instance.unstackKeys.AllPressed(eventData, InventoryActionInput.EventType.OnPointerUp))
                    {
                        TriggerUnstack(itemCollection);
                        return; // Stop the rest otherwise we'll do 2 actions at once.
                    }
                }

                // Use the item
                if (InventorySettingsManager.instance.useItemKeys.AllPressed(eventData, InventoryActionInput.EventType.OnPointerUp))
                    TriggerUse();

            }
        }

        /// <summary>
        /// Check if mobile input is valid.
        /// </summary>
        /// <param name="tap"></param>
        /// <param name="eventData"></param>
        /// <returns>True if an action was taken, false if no action was taken.</returns>
        protected virtual bool CheckMobileInput(InventoryActionInput.MobileUIActions tap, InventoryActionInput.EventType eventUsed, PointerEventData eventData)
        {
            if (InventorySettingsManager.instance.unstackKeys.AllPressed(tap, eventUsed, eventData))
            {
                TriggerUnstack(itemCollection);
                return true;
            }

            if (InventorySettingsManager.instance.useItemKeys.AllPressed(tap, eventUsed, eventData))
            {
                TriggerUse();
                return true;
            }

            if (InventorySettingsManager.instance.triggerContextMenuKeys.AllPressed(tap, eventUsed, eventData))
            {
                if (InventorySettingsManager.instance.useContextMenu)
                {
                    TriggerContextMenu();
                    return true;
                }
            }

            return false; // No action taken
        }

        public virtual bool OnTap(PointerEventData eventData, InventoryActionInput.EventType eventUsed)
        {
            return CheckMobileInput(InventoryActionInput.MobileUIActions.SingleTap, eventUsed, eventData);
        }

        public virtual bool OnDoubleTap(PointerEventData eventData, InventoryActionInput.EventType eventUsed)
        {
            return CheckMobileInput(InventoryActionInput.MobileUIActions.DoubleTap, eventUsed, eventData);
        }

        public virtual bool OnLongTap(PointerEventData eventData, InventoryActionInput.EventType eventUsed)
        {
            return CheckMobileInput(InventoryActionInput.MobileUIActions.LongTap, eventUsed, eventData);
        }

        //public virtual void OnPointerEnter(PointerEventData eventData)
        //{
        //    InventoryUIUtility.CursorEnterWrapper(this, eventData);
        //}

        //public virtual void OnPointerExit(PointerEventData eventData)
        //{
        //    InventoryUIUtility.CursorExitWrapper(this, eventData);
        //}

        #endregion

        #region Triggers

        public override void TriggerContextMenu()
        {
            if (item == null)
                return;

            var contextMenu = InventoryManager.instance.contextMenu;

            // Show context menu
            contextMenu.ClearMenuOptions();

            var itemList = item.GetUsabilities();
            itemList = itemCollection.GetExtraItemUsabilities(itemList);
            foreach (var i in itemList)
            {
                contextMenu.AddMenuOption(i.actionName, item, i.useItemCallback);
            }

            contextMenu.window.Show();
        }

        // <inheritdoc />
        public override void TriggerUnstack(ItemCollectionBase toCollection, int toIndex = -1)
        {
            if (item == null || itemCollection.useReferences || itemCollection.canUnstackItemsInCollection == false)
                return;

            if (item.currentStackSize > 1)
            {
                var m = InventorySettingsManager.instance;
                if(m.useUnstackDialog)
                {
                    var d = InventoryManager.instance.lang.unstackDialog;
                    InventoryManager.instance.unstackDialog.ShowDialog(itemCollection.transform, d.title, d.message, 1, (int)item.currentStackSize - 1, item,
                        (int val) =>
                        {
                            if (toIndex != -1)
                                itemCollection.UnstackSlot(index, toCollection, (uint) toIndex, (uint)val, true);
                            else
                                itemCollection.UnstackSlot(index, (uint)val, true);
                        },
                        (int val) =>
                        {
                            // Canceled


                        });
                }
                else
                {
                    if(toIndex != -1)
                        itemCollection.UnstackSlot(index, toCollection, (uint)toIndex, (uint)Mathf.Floor(item.currentStackSize / 2), true);                    
                    else
                        itemCollection.UnstackSlot(index, (uint)Mathf.Floor(item.currentStackSize / 2), true);
                }
            }
        }

        public override void TriggerDrop(bool useRaycast = true)
        {
            if (item == null || itemCollection.canDropFromCollection == false)
                return;
            
            if(item.isDroppable == false)
            {
                InventoryManager.instance.lang.itemCannotBeDropped.Show(item.name, item.description);
                return;
            }

            Vector3 dropPosition = InventoryPlayerManager.instance.currentPlayer.transform.position;
            if (InventorySettingsManager.instance.dropAtMousePosition)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, InventorySettingsManager.instance.maxDropDistance, InventorySettingsManager.instance.layersWhenDropping))
                {
                    dropPosition = hit.point;
                }
                else
                {
                    return; // Couldn't drop item
                }
            }

            var s = InventorySettingsManager.instance;
            if (s.showConfirmationDialogWhenDroppingItem && InventoryManager.instance.confirmationDialog == null)
            {
                Debug.LogError("Trying to drop item with confimration dialog, but dialog is not assigned, please check your settings.");
                return;
            }

            if (s.showConfirmationDialogWhenDroppingItem && s.showConfirmationDialogMinRarity.ID <= item.rarity.ID)
            {
                // Not on a button, drop it
                var tempItem = item; // Capture list stuff
                var msg = InventoryManager.instance.lang.confirmationDialogDrop;
                InventoryManager.instance.confirmationDialog.ShowDialog(itemCollection.transform, msg.title, msg.message, item,
                    (dialog) =>
                    {
                        tempItem.Drop(dropPosition);
                    },
                    (dialog) =>
                    {
                        //Debug.Log("No clicked");
                    });
            }
            else
            {
                item.Drop(dropPosition);
            }
        }

        public override void TriggerUse()
        {
            if (item == null)
                return;

            // Avoid reference using something from other collection that doesn't allow it.
            if (itemIsReference && item.itemCollection.canUseItemsFromReference == false)
                return;
            
            item.Use();
        }
    
        #endregion

        public override void Select()
        {
            var btn = gameObject.GetComponent<UnityEngine.UI.Button>();
            if(btn != null)
                btn.Select();

        }


        /// <summary>
        /// Repaints the item icon and amount.
        /// </summary>
        public override void Repaint()
        {
            if (item != null)
            {
                if (amountText != null)
                {
                    // Only show when we have more then 1 item.
                    if (item.currentStackSize > 1)
                        amountText.text = item.currentStackSize.ToString();
                    else
                        amountText.text = string.Empty;
                }

                if (itemName != null)
                    itemName.text = item.name;

                if(icon != null)
                    icon.sprite = item.icon;
            }
            else
            {
                if (amountText != null)
                    amountText.text = string.Empty;

                if (itemName != null)
                    itemName.text = string.Empty;

                if(icon != null)
                    icon.sprite = startIcon;
            }

            //RepaintCooldown(); // Already called by update loop
        }

        public virtual void RepaintCooldown()
        {
            if (item != null)
            {
                if(item.isInCooldown)
                {
                    cooldownVisualizer.Repaint((1.0f - item.cooldownFactor) * item.cooldownTime, item.cooldownTime);
                    return;
                }
            }


            cooldownVisualizer.Repaint(0.0f, 1.0f);
        }
    }
}