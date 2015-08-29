using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Devdog.InventorySystem.UI;
using UnityEngine;

namespace Devdog.InventorySystem.UI
{
    using Devdog.InventorySystem.Dialogs;

    /// <summary>
    /// Convenience methods to use with Unity UI
    /// </summary>
    [AddComponentMenu("InventorySystem/UI Helpers/Inventory Action helper")]
    public partial class InventoryActionHelper : MonoBehaviour
    {
        private Sprite wrapperMarkedStartSprite;
        private Color wrapperMarkedStartColor;


        private static InventoryUIItemWrapperBase _markedWrapper;
        private InventoryUIItemWrapperBase markedWrapper
        {
            get { return _markedWrapper; }
            set
            {
                _markedWrapper = value;
                if (_markedWrapper != null && _markedWrapper.item != null)
                {
                    var button = _markedWrapper.gameObject.GetComponent<UnityEngine.UI.Button>();
                    if (button != null && button.targetGraphic != null)
                    {
                        var image = button.targetGraphic as UnityEngine.UI.Image;
                        if (image != null)
                        {
                            image.sprite = button.spriteState.highlightedSprite;
                            image.color = Color.yellow; // TODO: Make dynamic!
                        }
                    }
                }
            }
        }

        private static bool canUseWrapper
        {
            get
            {
                if (InventoryUIUtility.CanReceiveInput(InventoryUIUtility.currentlySelectedWrapper) == false)
                    return false;

                return InventoryUIUtility.currentlySelectedWrapper != null;
            }
        }

        public void TriggerUseCurrentlySelectedWrapper()
        {
            if (canUseWrapper)
                InventoryUIUtility.currentlySelectedWrapper.TriggerUse();

            ClearMarkedWrapper(); // Slot could be cleared, clear marker as well
        }

        public void TriggerDropCurrentlySelectedWrapper()
        {
            if (canUseWrapper)
                InventoryUIUtility.currentlySelectedWrapper.TriggerDrop();

            ClearMarkedWrapper(); // Slot is cleared, clear marker as well
        }

        public void TriggerUnstackCurrentlySelectedWrapper()
        {
            if (canUseWrapper)
                InventoryUIUtility.currentlySelectedWrapper.TriggerUnstack(InventoryUIUtility.currentlySelectedWrapper.itemCollection);

            ClearMarkedWrapper(); // Slot could be cleared, clear marker as well
        }

        public void TriggerContextMenuCurrentlySelectedWrapper()
        {
            if (canUseWrapper)
                InventoryUIUtility.currentlySelectedWrapper.TriggerContextMenu();

            ClearMarkedWrapper(); // Slot could be cleared, clear marker as well
        }

        /// <summary>
        /// Marking can be used to temp "select" a wrapper. You can then later read the temp selected / marked wrapper.
        /// </summary>
        public void MarkCurrentlySelectedWrapper()
        {
            if (canUseWrapper)
            {
                if (InventoryUIUtility.currentlySelectedWrapper.itemCollection.canDragInCollection == false)
                    return;

                var button = InventoryUIUtility.currentlySelectedWrapper.gameObject.GetComponent<UnityEngine.UI.Button>();
                if (button != null && button.targetGraphic != null)
                {
                    var img = button.targetGraphic as UnityEngine.UI.Image;
                    if (img != null)
                        wrapperMarkedStartSprite = img.sprite;

                    wrapperMarkedStartColor = button.targetGraphic.color;
                }
            }

            markedWrapper = InventoryUIUtility.currentlySelectedWrapper;
        }

        /// <summary>
        /// Move the previously marked wrapper to the currently / newly selected wrapper.
        /// This can cause a move, merge, or swap. (depending on the new location)
        /// 
        /// Note: If markedWrapper is null it will be set using this method.
        /// </summary>
        public void MoveCurrentlySelectedWrapperToMarkedWrapper()
        {
            if (canUseWrapper == false)
                return;

            if (markedWrapper == null || markedWrapper.item == null)
            {
                MarkCurrentlySelectedWrapper();
                return;
            }

            var newWrapper = InventoryUIUtility.currentlySelectedWrapper;
            if (newWrapper == null)
                return; // No new location selected.
            
            // Move it (move, merge or swap)
            markedWrapper.itemCollection.SwapOrMerge(markedWrapper.index, newWrapper.itemCollection, newWrapper.index);

            ClearMarkedWrapper();
        }

        private void ClearMarkedWrapper()
        {
            if (canUseWrapper == false)
                return;

            if (markedWrapper == null)
                return;

            // Reset the marked wrapper's original sprite.
            var button = markedWrapper.gameObject.GetComponent<UnityEngine.UI.Button>();
            if (button != null && button.targetGraphic != null)
            {
                var image = button.targetGraphic as UnityEngine.UI.Image;
                if (image != null)
                    image.sprite = wrapperMarkedStartSprite;

                button.targetGraphic.color = wrapperMarkedStartColor;
            }

            markedWrapper = null;
        }


        public void SelectFirstWrapperOfCollection(ItemCollectionBase collection)
        {
            if (collection.items.Length == 0)
            {
                Debug.LogWarning("Collection has no items, can't select first item.");
                return;
            }

            collection[0].Select();
        }
    }
}
