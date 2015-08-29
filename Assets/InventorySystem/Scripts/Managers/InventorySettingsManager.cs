// /**
// * Written By Joris Huijbregts
// * Some legal stuff --- Copyright!
// */
using UnityEngine;
using System.Collections;
using Devdog.InventorySystem.Dialogs;
using Devdog.InventorySystem.Models;
using UnityEngine.EventSystems;

namespace Devdog.InventorySystem
{
    [AddComponentMenu("InventorySystem/Managers/Inventory Settings manager")]
    [RequireComponent(typeof(ItemManager))]
    [RequireComponent(typeof(InventoryManager))]
    [RequireComponent(typeof(InventoryTriggererManager))]
    [RequireComponent(typeof(InventoryPlayerManager))]
    [RequireComponent(typeof(InventoryInputManager))]
    public partial class InventorySettingsManager : MonoBehaviour
    {
        public enum TriggererKeyCodeHandlerType
        {
            /// <summary>
            /// Don't use range checking, instead triggerers can only be triggered using the mouse / mobile tap or collision.
            /// </summary>
            None,

            /// <summary>
            /// Raycast from the camera to try and use triggerers (useful for FPS games)
            /// </summary>
            RaycastFromCamera,

            /// <summary>
            /// Find the best object in range based on the player's viewing direction and distance to the object (useful for controller based games / picking up in a radius)
            /// </summary>
            FindBestInRange
        }

        /// <summary>
        /// The default material used when a set of items is higher than 0
        /// </summary>
        [InventoryEditorCategory("UI")]
        public Material iconDefaultMaterial;

        /// <summary>
        /// The material used when a stack of references is 0 in size. Aka, when the stack is depleted.
        /// </summary>
        public Material iconDepletedMaterial;

        /// <summary>
        /// Extra padding for when the user releases the mouse.
        /// Example: When the user clicks an item and keeps the mouse button down, and moves the cursor outside of the button.
        /// The padding allows the user to release the button further outside of the button, and still trigger it.
        /// </summary>
        public Vector2 onPointerUpInsidePadding;

        /// <summary>
        /// The default UI Item.
        /// </summary>
        [InventoryRequired]
        //[Header("Prefabs")]
        public GameObject itemButtonPrefab;

        /// <summary>
        /// The root of the GUI.
        /// </summary>
        //[Header("Scene references")]
        [InventoryRequired]
        public Canvas guiRoot;

        /// <summary>
        /// The default collection sorter, sorts by category.
        /// </summary>
        public ICollectionSorter collectionSorter { get; set; }


        /// <summary>
        /// The layer used to equip items
        /// </summary>
        [InventoryEditorCategory("Items")]
        [Range(8, 31)]
        public int equipmentLayer = 25;
        
        /// <summary>
        /// The layer used when dropping, and when the item is in the world.
        /// </summary>
        [Range(8, 31)]
        public int itemWorldLayer = 26;

        /// <summary>
        /// Use the context menu or not?
        /// </summary>
        [InventoryEditorCategory("Context menu")]
        public bool useContextMenu;
        

        /// <summary>
        /// Drop an object at the cursor / mouse position position
        /// If false, the item will be dropped using the offset vector.
        /// </summary>
        [InventoryEditorCategory("Dropping")]
        public bool dropAtMousePosition = true;

        /// <summary>
        /// The offset by which an item is dropped into the world
        /// Only used when dropAtMousePosition = false;
        /// </summary>
        public Vector3 dropOffsetVector = new Vector3(0.0f, 0.0f, 0.0f);

        /// <summary>
        /// The maxmimum distance an item can be dropped
        /// </summary>
        public float maxDropDistance = 10.0f;

        /// <summary>
        /// Layers to consider when ray casting for item dropping.
        /// </summary>
        public LayerMask layersWhenDropping;

        /// <summary>
        /// When dropping an item, do you want it placed precisely on the ground?
        /// </summary>
        public bool dropItemRaycastToGround = false;



        /// <summary>
        /// Do you want to show a confirmation dialog, when an item is being dropped?
        /// </summary>
        [InventoryEditorCategory("Dialogs")]
        [Header("Confirmation dialog")]
        public bool showConfirmationDialogWhenDroppingItem = true;


        public int _showConfirmationDialogMinRarity = 0;
        /// <summary>
        /// The minimal rarity an item should have before the dialog appears?
        /// </summary>
        public InventoryItemRarity showConfirmationDialogMinRarity
        {
            get
            {
                if (ItemManager.instance.itemRarities.Length == 0)
                    return null;

                return ItemManager.instance.itemRarities[_showConfirmationDialogMinRarity];
            }
        }
        
        /// <summary>
        /// Disables these UI elements while the dialog is active, useful when you want to disable the inventory when the user sees a dialog box.
        /// </summary>
        [Header("Value dialog")]
        [Tooltip("Disables these UI elements while the dialog is active, useful when you want to disable the inventory when the user sees a dialog box")]
        public RectTransform[] disabledWhileDialogActive = new RectTransform[0];


        
        /// <summary>
        /// If true a dialog is displayed, if false the stack will be split in half.
        /// </summary>
        [Header("Unstack dialog")]
        public bool useUnstackDialog = true;


        /// <summary>
        /// How long should the LongTap time be on mobile devices?
        /// </summary>
        [InventoryEditorCategory("User actions")]
        [Header("Mobile actions")]
        public float mobileLongPressTime = 0.3f;

        /// <summary>
        /// How long 2 taps can be apart from one another to trigger the double tap event.
        /// </summary>
        public float mobileDoubleTapTime = 0.4f;


        /// <summary>
        /// The distance items can be used, and windows should be auto closed.
        /// </summary>
        [InventoryEditorCategory("Pickup & usage")]
        [Header("Use distance")]
        public float useObjectDistance = 10.0f;


        [Header("Cursor pickup & usage")]
        public InventoryCursorIcon pickupCursor = new InventoryCursorIcon();
        public InventoryCursorIcon useCursor = new InventoryCursorIcon();

        [Tooltip("The icon used when no action is active. (Fallback icon)")]
        public InventoryCursorIcon noActionCursor = new InventoryCursorIcon();


        [Header("FPS Pickup & usage")]
        public Sprite objectTriggererFPSPickupSprite;
        public Sprite objectTriggererFPSUseSprite;

        
        ///// <summary>
        ///// When the item is clicked, should it trigger?
        ///// </summary>
        [Header("Behavior")]
        [Tooltip("When the item is clicked, should it trigger?")]
        public bool itemTriggerMouseClick = true;

        /// <summary>
        /// How should triggerers be handled / triggered?
        /// </summary>
        [Tooltip("RaycastFromCamera: Draw a ray from the middle of the screen (useful for FPS games)\nFindBestInRange: Find the best object in range based on the player's viewing direction and distance to the object (useful for 3rd person based games)\nNone: Only allow triggers to be used by clicking or collision.")]
        public TriggererKeyCodeHandlerType triggererHandlerType = TriggererKeyCodeHandlerType.None;

        ///// <summary>
        ///// The key code used when trying to use an item (loot an item)
        ///// </summary>
        [Tooltip("The key code used when trying to use an item (loot an item).")]
        public KeyCode itemTriggererUseKeyCode = KeyCode.None;

        /////// <summary>
        /////// The key code used to pickup an item either when in range (if itemTriggerKeyCodeRange is enabled)
        /////// </summary>
        //[Tooltip("The key code used to pickup an item either when in range (if itemTriggerKeyCodeRange is enabled).")]
        //public KeyCode itemTriggererRangeKeyCode = KeyCode.None;

        ///// <summary>
        ///// When an item is in range, determine the best object and loot it. Mostly useful for controller based games.
        ///// </summary>
        //[Tooltip("When an item is in range, determine the best object and loot it. Mostly useful for controller based games.")]
        //public bool useItemTriggererRangeUse = false;

        /// <summary>
        /// Always trigger gold pickup on collision, even when itemTriggerOnPlayerCollision is off.
        /// </summary>
        [Tooltip("Always trigger gold pickup on collision, even when itemTriggerOnPlayerCollision is off.")]
        public bool alwaysTriggerGoldItemPickupOnPlayerCollision = false;

        /// <summary>
        /// Trigger the item when the player collides with it.
        /// </summary>
        [Tooltip("Trigger the item when the player collides with it.")]
        public bool itemTriggerOnPlayerCollision = false;


        /// <summary>
        /// Unstack when the user clicks the item + has all the required keys down.
        /// </summary>
        [InventoryEditorCategory("Input")]
        [Header("Unstack actions")]
        public bool useUnstackClick = true;

        /// <summary>
        /// Unstack when the user drags the item to a new slot while holding the required keys.
        /// </summary>
        public bool useUnstackDrag = true;

        /// <summary>
        /// The keys required to unstack
        /// </summary>
        [Header("Action inputs")]
        public InventoryActionInput unstackKeys = new InventoryActionInput(PointerEventData.InputButton.Right, InventoryActionInput.EventType.OnPointerUp, KeyCode.LeftShift);

        /// <summary>
        /// The keys used to "use" an item.
        /// </summary>
        public InventoryActionInput useItemKeys = new InventoryActionInput(PointerEventData.InputButton.Right, InventoryActionInput.EventType.OnPointerUp, KeyCode.None);

        /// <summary>
        /// Trigger the context menu using, the following button
        /// </summary>
        public InventoryActionInput triggerContextMenuKeys = new InventoryActionInput(PointerEventData.InputButton.Left, InventoryActionInput.EventType.OnPointerUp, KeyCode.None);


        private static InventorySettingsManager _instance;
        public static InventorySettingsManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<InventorySettingsManager>();
                }

                return _instance;
            }
        }

        public void Awake()
        {
            _instance = this;

            collectionSorter = new BasicCollectionSorter();
        }
    }
}