using System;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem
{
    /// <summary>
    /// An abstract base class for collections. When creating a new collection, extend from this class.
    /// </summary>
    public partial class ItemCollectionBase : MonoBehaviour, IEnumerable, IEnumerable<InventoryUIItemWrapperBase>
    {
        #region Delegates

        /// <summary>
        /// Called when an item was added to this collection
        /// </summary>
        /// <param name="item">A list of the item that was added, all slots where the item was stored. If the item doesn't fit 1 slot it's broken up into multiple slots.</param>
        /// <param name="amount">The amount of items that have been added (stack size).</param>
        /// <param name="cameFromCollection">Did the item come from another collection or is it looted?</param>
        public delegate void AddedItem(IEnumerable<InventoryItemBase> items, uint amount, bool cameFromCollection);

        /// <summary>
        /// Called whenever the currency changes inside of this collection.
        /// </summary>
        /// <param name="amountBefore"></param>
        /// <param name="lookup"></param>
        public delegate void CurrencyChanged(float amountBefore, InventoryCurrencyLookup lookup);

        /// <summary>
        /// Called when an item is dropped from this collection.
        /// </summary>
        /// <param name="item">The item that has been dropped.</param>
        /// <param name="slot">Slot from where the item was dropped.</param>
        /// <param name="droppedObj">The actual 3D object that got dropped. This can either be the "DropObject" that has been set in the cateogry or the actual model.</param>
        public delegate void DroppedItem(InventoryItemBase item, uint slot, GameObject droppedObj);

        /// <summary>
        /// Called when an reference is removed from this collection. Note that it will only be called if "Use references" is enabled in the inspector.
        /// </summary>
        /// <param name="item">The reference that was removed.</param>
        /// <param name="slot">The slot from where the reference was removed.</param>
        public delegate void RemovedReference(InventoryItemBase item, uint slot);

        /// <summary>
        /// Called when an item was removed from this collection, either by dropping or selling.
        /// <b>Item is not fired when an item of a stack is used, only the last item, as this destroys the stack.</b>
        /// </summary>
        /// <param name="item">The item that was removed, note that it can be null if the item was destroyed in the process.</param>
        /// <param name="itemID">ItemID of the item that was removed from the inventory. ID instead of an actual item, because it could have been destroyed in the process.</param>
        /// <param name="slot">The slot from where the item was removed.</param>
        /// <param name="amount">Amount of items that were removed of this type.</param>
        public delegate void RemovedItem(InventoryItemBase item, uint itemID, uint slot, uint amount);

        /// <summary>
        /// Called when a stack is un-stacked, aka broken down into 2 smaller stacks.
        /// </summary>
        /// <param name="startSlot">The original location where the unstack started</param>
        /// <param name="endSlot">The 2nd slot used to store the 2nd stack.</param>
        /// <param name="amount">Amount of items that were unstacked</param>
        public delegate void UnstackedItem(ItemCollectionBase fromCollection, uint startSlot, ItemCollectionBase toCollection, uint endSlot, uint amount);


        /// <summary>
        /// Called when 2 slots of the same type are merged together into a single slot.
        /// </summary>
        /// <param name="fromCollection"></param>
        /// <param name="fromSlot"></param>
        /// <param name="toCollection"></param>
        /// <param name="toSlot"></param>
        public delegate void MergedSlots(ItemCollectionBase fromCollection, uint fromSlot, ItemCollectionBase toCollection, uint toSlot);

        /// <summary>
        /// Called when an item was used from this collection.
        /// </summary>
        /// <param name="item">The item that was used, note that it can be null if the item was destroyed in the process.</param>
        /// <param name="itemID">ID of the item used.</param>
        /// <param name="slot">Slot of the item used.</param>
        /// <param name="amount">Amount of items used.</param>
        public delegate void UsedItem(InventoryItemBase item, uint itemID, uint slot, uint amount);


        /// <summary>
        /// Called when a reference is used inside this collection.
        /// </summary>
        /// <param name="actualItem">The item used (original item) that the reference is bound to.</param>
        /// <param name="itemID"></param>
        /// <param name="referenceSlot"></param>
        /// <param name="amountUsed"></param>
        public delegate void UsedReference(InventoryItemBase actualItem, uint itemID, uint referenceSlot, uint amountUsed);


        /// <summary>
        /// Tried to add an item to the collection, but it was full, item was denied.
        /// </summary>
        /// <param name="item">Item tried to add</param>
        public delegate void AddedItemCollectionFull(InventoryItemBase item, bool cameFromCollection);

        /// <summary>
        /// Tried to unstack an item, but the collection was full. Item not unstacked.
        /// </summary>
        /// <param name="slot">Slot attempted to unstack. items[slot].item returns the actual item.</param>
        public delegate void UnstackedItemCollectionFull(uint slot);

        /// <summary>
        /// Called when this collection is resized.
        /// </summary>
        /// <param name="fromSize">Old size</param>
        /// <param name="toSize">New size (can be smaller than fromSize)</param>
        public delegate void CollectionResized(uint fromSize, uint toSize);

        /// <summary>
        /// Called when the collection is sorted.
        /// </summary>
        public delegate void Sorted();

        /// <summary>
        /// Called when gold was added / removed from this collection.
        /// </summary>
        /// <param name="amountAdded">Positive value if gold was added, negative if gold was removed.</param>
        public delegate void GoldChanged(float amountAdded);

        /// <summary>
        /// Called when 2 items were swapped inside this collection.
        /// Called when 2 items were swapped between 2 different collections.
        /// </summary>
        /// <param name="fromCollection">Start collection</param>
        /// <param name="fromSlot">Start slot</param>
        /// <param name="toCollection">End collection</param>
        /// <param name="toSlot">End slot</param>
        public delegate void SwappedItems(ItemCollectionBase fromCollection, uint fromSlot, ItemCollectionBase toCollection, uint toSlot);


        /// <summary>
        /// Returns true if the item can be added to the collection, and false when the item cannot be added.
        /// Allows you to add your own conditions to collections.
        /// </summary>
        /// <param name="item"></param>
        public delegate bool CanAddItemDelegate(InventoryItemBase item);

        #endregion


        #region Variables 

        /// <summary>
        /// Cache of the items / visuals.
        /// </summary>
        [SerializeField]
        private InventoryUIItemWrapperBase[] _items;

        /// <summary>
        /// Cache of the items / visuals.
        /// Only set this if you know what you're doing...
        /// </summary>
        public InventoryUIItemWrapperBase[] items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
            }
        }
        
        public InventoryCurrencyLookupContainer currenciesContainer { get; protected set; }
        

        /// <summary>
        /// Amount of items / slots in this collection, empty items are also counted.
        /// </summary>
        public uint collectionSize
        {
            get
            {
                return (uint)_items.Length;
            }
        }

        /// <summary>
        /// Easy accessor, also prevents users from changing the InventoryUIItemWrapper object.
        /// </summary>
        /// <param name="i">Index</param>
        /// <returns></returns>
        public InventoryUIItemWrapperBase this[uint i]
        {
            get
            {
                return items[i];
            }
        }
        public InventoryUIItemWrapperBase this[int i]
        {
            get
            {
                return items[i];
            }
        }

        /// <summary>
        /// The name of this collection used in notifications or custom code
        /// </summary>
        public string collectionName = "MyCollection";

        ///// <summary>
        ///// Does this collection belong to a player?
        ///// </summary>
        //public InventoryPlayer[] belongsToPlayers;

        /// <summary>
        /// Use weight calculations??
        /// </summary>
        public bool restrictByWeight = false;

        /// <summary>
        /// Restrict this collection to a max amount of weight
        /// </summary>
        //[Range(0.0f, 999.0f)]
        public float restrictMaxWeight = 100.0f;


        /// <summary>
        /// If true references will be used, and the objects won't be moved. Useful for skill bars and ... well references.
        /// </summary>
        public bool useReferences = false;

        /// <summary>
        /// Can this collection contain currencies?
        /// </summary>
        public bool canContainCurrencies = true;

        /// <summary>
        /// Can an item be dropped directly from this collection?
        /// Auto disabled if useReferences is set to true.
        /// </summary>
        public bool canDropFromCollection = false;

        /// <summary>
        /// Does the player or a player carry this collection around?
        /// </summary>
        public bool canUseItemsFromReference = false;

        /// <summary>
        /// Can an item be used directly from this collection?
        /// </summary>
        public bool canUseFromCollection = false;

        /// <summary>
        /// Can items be dragged inside the collection?
        /// </summary>
        public bool canDragInCollection = true;

        /// <summary>
        /// Can items be stored into this collection / moved into?
        /// </summary>
        public bool canPutItemsInCollection = true;

        /// <summary>
        /// Can items be stacked in this collection, if false items will be broken up in stacks of 1.
        /// </summary>
        public bool canStackItemsInCollection = true;

        /// <summary>
        /// Can items be unstacked inside this collection?
        /// </summary>
        public bool canUnstackItemsInCollection = true;
        
        /// <summary>
        /// Manually create the collection, either through code or the inspector.
        /// </summary>
        public bool manuallyDefineCollection = false;
        

        /// <summary>
        /// A list of all synced collections. When a action is taken in this collection, it will be re-produced in the collections in the list.
        /// </summary>
        [NonSerialized]
        private List<CollectionToCollectionSyncer> _syncedCollections = new List<CollectionToCollectionSyncer>();

        /// <summary>
        /// A list of all synced arrays. When a action is taken in this collection, it will be re-produced on the arrays in the list.
        /// </summary>
        [NonSerialized]
        private List<CollectionToArraySyncer> _syncedArrays = new List<CollectionToArraySyncer>();


            /// <summary>
        /// The container that holds the collection.
        /// </summary>
        [InventoryRequired]
        public RectTransform container;

        /// <summary>
        /// Item button prefab, used to create UI elements.
        /// </summary>
        [Tooltip("The prefab used to create UI elements, if left empty the default will be chosen from the settings.")]
        public GameObject itemButtonPrefab;

        ///// <summary>
        ///// Only allow items of a certain type. If the item does not match the type, it won't be storable in this collection.
        ///// </summary>
        //[HideInInspector]
        //public string[] _onlyAllowTypes;
        //public System.Type[] onlyAllowTypes
        //{
        //    get
        //    {
        //        var types = new System.Type[_onlyAllowTypes.Length];
        //        for (int i = 0; i < _onlyAllowTypes.Length; i++)
        //        {
        //            types[i] = System.Type.GetType(_onlyAllowTypes[i]);
        //        }

        //        return types;
        //    }
        //}

        public InventoryItemFilters filters = new InventoryItemFilters();


        /// <summary>
        /// The parent of the items in this collection
        /// </summary>
        protected Transform containerItemsParent;

        /// <summary>
        /// The max size of the collection, this many empty item slots will be created.
        /// </summary>
        public virtual uint initialCollectionSize { get; set; }



        private List<CanAddItemDelegate> _canAddItemToCollectionConditionals = new List<CanAddItemDelegate>();
        /// <summary>
        /// Returns true if the item can be used, and false when the item cannot be used.
        /// Allows you to add your own conditions to items.
        /// </summary>
        public List<CanAddItemDelegate> canAddItemToCollectionConditionals
        {
            get { return _canAddItemToCollectionConditionals; }
            protected set { _canAddItemToCollectionConditionals = value; }
        }


        #endregion


        #region Hookable events

        /// <summary>
        /// Occurs when an item is added to the collection.
        /// </summary>
        public event AddedItem OnAddedItem;

        /// <summary>
        /// Occurs when the currency inside this collection is changed.
        /// </summary>
        public event CurrencyChanged OnCurrencyChanged;

        /// <summary>
        /// Occurs when an item is dropped from a collection.
        /// <b>Event is NOT fired when an item is used!</b>
        /// </summary>
        public event DroppedItem OnDroppedItem;

        /// <summary>
        /// Occurs when an item inside this collection is used, some items reduce in stackSize when used ( Consumable for example ).
        /// </summary>
        public event UsedItem OnUsedItem;

        /// <summary>
        /// Occurs when a reference inside this collection is used, some items reduce in stackSize when used ( Consumable for example ).
        /// </summary>
        public event UsedReference OnUsedReference;

        /// <summary>
        /// Occurs when an reference is removed from a collection.
        /// </summary>
        public event RemovedReference OnRemovedReference;

        /// <summary>
        /// Occurs when an item is removed from a collection, either dropped, stored, sold, swapped, last item in stack used, etc.
        /// <b>Item is not fired when an item of a stack is used, only the last item, as this destroys the stack.</b>
        /// Using itemID instead of object, because the object could have been destroyed in the process.
        /// Does not fire for references, use OnRemovedFerence instead
        /// </summary>
        public event RemovedItem OnRemovedItem;

        /// <summary>
        /// When an item is unstacked, the event is fired, if the inventory was full, no event will be fired.
        /// </summary>
        public event UnstackedItem OnUnstackedItem;

        /// <summary>
        /// Fired when 2 slots of the same type are merged into a single stack
        /// </summary>
        public event MergedSlots OnMergedSlots;

        /// <summary>
        /// Occurs when you try to add an item to the collection but there is no space for it.
        /// </summary>
        public event AddedItemCollectionFull OnAddedItemCollectionFull;

        /// <summary>
        /// Fired when an item is being unstacked but there is no place for the unstacked item, because the collection is full.
        /// </summary>
        public event UnstackedItemCollectionFull OnUnstackedItemCollectionFull;

        /// <summary>
        /// Occurs when the collection is resized (whens slots are added or removed).
        /// </summary>
        public event CollectionResized OnResized;

        /// <summary>
        /// Occurs when the collection is sorted.
        /// The collection is sorted using the collectionSorter,
        /// you can create your own sorting behavior by creating a custom class and implementing IInventoryCollectionSorter.
        /// </summary>
        public event Sorted OnSorted;

        /// <summary>
        /// When 2 items are swapped the event is fired.
        /// Note: The event is fired on both collections, if it is swapped within the same collection only that collection will be called once.
        /// toCollection is the final position where the item was stored.
        /// </summary>
        public event SwappedItems OnSwappedItems;

        #endregion


        #region Methods


        /// <summary>
        /// Unity monobehaviour method, creates the collection.
        /// </summary>
        public virtual void Awake()
        {
            var a = new GameObject(GetType().Name);
            containerItemsParent = a.transform;
            containerItemsParent.SetParent(InventoryManager.instance.collectionObjectsParent);

            currenciesContainer = new InventoryCurrencyLookupContainer(true);
            RegisterCurrencyLookupEvents();

            Awake2();
            Awake3();
            Awake4();
            Awake5();
            Awake6();
            Awake7();
            Awake8();
            Awake9();

            FillUI();
        }

        private void RegisterCurrencyLookupEvents()
        {
            foreach (var currency in currenciesContainer.lookups)
            {
                var c = currency; // Capture list and all..
                c.OnCurrencyChanged += (before, after) =>
                    { NotifyCurrencyChanged(before, c); };
            }
        }


        /// <summary>
        /// These Awake..n() methods can be overriden in partial classes for integrations, etc.
        /// </summary>
        partial void Awake2();
        partial void Awake3();
        partial void Awake4();
        partial void Awake5();
        partial void Awake6();
        partial void Awake7();
        partial void Awake8();
        partial void Awake9();


        public virtual void Start()
        {
            Start2();
            Start3();
            Start4();
            Start5();
            Start6();
            Start7();
            Start8();
            Start9();
        }

        /// <summary>
        /// These Start..n() methods can be overriden in partial classes for integrations, etc.
        /// </summary>
        partial void Start2();
        partial void Start3();
        partial void Start4();
        partial void Start5();
        partial void Start6();
        partial void Start7();
        partial void Start8();
        partial void Start9();

        /// <summary>
        /// Unity MonoBehaviour method
        /// </summary>
        public virtual void OnDestroy()
        {
            OnDestroy2();
            OnDestroy3();
            OnDestroy4();
            OnDestroy5();
            OnDestroy6();
            OnDestroy7();
            OnDestroy8();
            OnDestroy9();
        }

        /// <summary>
        /// These OnDestroy..n() methods can be overriden in partial classes for integrations, etc.
        /// </summary>
        partial void OnDestroy2();
        partial void OnDestroy3();
        partial void OnDestroy4();
        partial void OnDestroy5();
        partial void OnDestroy6();
        partial void OnDestroy7();
        partial void OnDestroy8();
        partial void OnDestroy9();


        /// <summary>
        /// Creates all visual wrapper objects and parents them to create a displayable collection.
        /// </summary>
        protected virtual void FillUI()
        {
            if (manuallyDefineCollection == false)
            {
                items = new InventoryUIItemWrapperBase[initialCollectionSize];

                // Fill the container on startup, can add / remove later on
                for (uint i = 0; i < initialCollectionSize; i++)
                {
                    items[i] = CreateUIItem<InventoryUIItemWrapperBase>(i, itemButtonPrefab != null ? itemButtonPrefab : InventorySettingsManager.instance.itemButtonPrefab);
                }
            }
            else
            {
                for (uint i = 0; i < items.Length; i++)
                {
                    items[i].itemCollection = this;
                    items[i].index = i;
                }
            }
        }

        /// <summary>
        /// Create a single UI item (wrapper).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="i"></param>
        /// <param name="prefab"></param>
        /// <returns></returns>
        protected T CreateUIItem<T>(uint i, GameObject prefab) where T : InventoryUIItemWrapperBase
        {
            T item = GameObject.Instantiate<GameObject>(prefab).GetComponent<T>();

            item.transform.SetParent(container);
            InventoryUtility.ResetTransform(item.transform);
            
            item.itemCollection = this;
            item.index = i;
        
            return item;
        }

        #endregion


        #region Notifications from other objects

        /// <summary>
        /// Is notified when an item is dropped, this fires of the events.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="itemID">ID of the item dropped</param>
        /// <param name="amount">Amount of items dropped</param>
        public void NotifyItemDropped(InventoryItemBase item, uint itemID, uint amount, GameObject droppedObj)
        {
            // References just disappear, can't drop them
            if (useReferences)
            {
                if (OnRemovedReference != null)
                    OnRemovedReference(items[item.index].item, item.index);

                return;
            }
                
            if (OnDroppedItem != null)
                OnDroppedItem(item, item.index, droppedObj);

            NotifyItemRemoved(item, itemID, item.index, amount);



            item.itemCollection.SetItem(item.index, null);
            items[item.index].Repaint();

            // Item is no longer in a collection, remove it.
            item.itemCollection = null;
            item.index = 0;
        }

        /// <summary>
        /// Is notified when a reference is used from this collection.
        /// </summary>
        /// <param name="actualItem"></param>
        /// <param name="itemID"></param>
        /// <param name="referenceSlot"></param>
        /// <param name="amountUsed"></param>
        public void NotifyReferenceUsed(InventoryItemBase actualItem, uint itemID, uint referenceSlot, uint amountUsed)
        {
            if (OnUsedReference != null)
                OnUsedReference(actualItem, itemID, referenceSlot, amountUsed);
        }

        public void NotifyResized(uint oldSize, uint newSize)
        {
            if (OnResized != null)
                OnResized(oldSize, newSize);
        }

        /// <summary>
        /// Handles events whenever an item is used.
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="slot"></param>
        public void NotifyItemUsed(InventoryItemBase item, uint itemID, uint slot, uint amountUsed)
        {
            // Item removed itself or someone else did.
            if (items[slot].item == null)
            {
                //SetItem(slot, null);
                //NotifyItemRemoved(itemID, slot, 0); // Let the NotifyItemUsed() handle the update.
            }
            else
            {
                if (items[slot].item.currentStackSize == 0)
                {
                    SetItem(slot, null);
                    NotifyItemRemoved(item, itemID, slot, 0); // Let the NotifyItemUsed() handle the update.
                }
            }

            if (OnUsedItem != null)
                OnUsedItem(item, itemID, slot, amountUsed);


            if (item != null)
            {
                if (item.itemCollection.useReferences)
                {
                    // Repaint the original item
                    item.itemCollection.NotifyReferenceUsed(item, item.ID, item.index, amountUsed);

                    if (amountUsed > 0)
                        item.itemCollection[item.index].Repaint();
                }

                if (item.currentStackSize <= 0)
                {
                    SetItem(item.index, null);
                }
            }

            if(amountUsed > 0)
                items[slot].Repaint();
        }

        public void NotifySorted()
        {
            if (OnSorted != null)
                OnSorted();
        }

        public void NotifyItemRemoved(InventoryItemBase item, uint itemID, uint slot, uint amount)
        {
            if (OnRemovedItem != null)
                OnRemovedItem(item, itemID, slot, amount);
        }

        public void NotifyItemAdded(InventoryItemBase item, uint amount, bool cameFromCollection)
        {
            NotifyItemAdded(new []{ item }, amount, cameFromCollection);
        }

        public virtual void NotifyItemAdded(IEnumerable<InventoryItemBase> item, uint amount, bool cameFromCollection)
        {
            if (OnAddedItem != null)
                OnAddedItem(item, amount, cameFromCollection);
        }

        private void NotifyCurrencyChanged(float before, InventoryCurrencyLookup inventoryCurrencyLookup)
        {
            //Debug.Log("Currency changed", transform);
            if (OnCurrencyChanged != null)
                OnCurrencyChanged(before, inventoryCurrencyLookup); // Call the event on this collection.

        }

        public void NotifyItemSwapped(ItemCollectionBase fromCollection, uint fromSlot, ItemCollectionBase toCollection, uint toSlot)
        {
            if (fromCollection.OnSwappedItems != null)
                fromCollection.OnSwappedItems(fromCollection, fromSlot, toCollection, toSlot);

            // Already fired what we needed, so return, else fire it on the other collection as well.
            if (fromCollection == toCollection)
                return;
        
            if(toCollection.OnSwappedItems != null)
                toCollection.OnSwappedItems(fromCollection, fromSlot, toCollection, toSlot);        
        }

        public void NotifyAddedItemCollectionFull(InventoryItemBase item, bool cameFromCollection)
        {
            // Inventory is full, got no space for that item
            if (OnAddedItemCollectionFull != null)
                OnAddedItemCollectionFull(item, cameFromCollection);
        }

        public void NotifyUnstackedItemCollectionFull(uint slot)
        {
            // Inventory is full, got no space for that item
            if (OnUnstackedItemCollectionFull != null)
                OnUnstackedItemCollectionFull(slot);
        }

        public void NotifyUnstackedItem(uint slot, ItemCollectionBase toCollection, uint toSlot, uint amount)
        {
            if (OnUnstackedItem != null)
                OnUnstackedItem(this, slot, toCollection, toSlot, amount);
        }

        private void NotifyMergedSlots(ItemCollectionBase fromCollection, uint fromSlot, ItemCollectionBase toCollection, uint toSlot)
        {
            if (OnMergedSlots != null)
                OnMergedSlots(fromCollection, fromSlot, toCollection, toSlot);
        }

        #endregion

        #region Actions

        /// <summary>
        /// Override the Use() method on all items inside this collection.
        /// Note that this method will be called even if canUseFromCollection is false.
        /// </summary>
        /// <returns>Return true if the useMethod() is overriden, false if not and the default item based action will be performed.</returns>
        public virtual bool OverrideUseMethod(InventoryItemBase item)
        {
            return false;
        }


        /// <summary>
        /// Resize to the new size
        /// </summary>
        /// <param name="newSize"></param>
        /// <param name="force"></param>
        /// <returns>True when resized, false if failed</returns>
        public virtual bool Resize(uint newSize, bool force = false)
        {
            uint oldSize = (uint)items.Length;
            if (oldSize == newSize)
                return true; // Nothing to resize

            if (newSize > oldSize)
            {
                // Add slots
                return AddSlots(newSize - oldSize);
            }
            else
            {
                // Remove slots
                return RemoveSlots(oldSize - newSize, force);
            }
        }


        /// <summary>
        /// Add extra slots to the collection.
        /// </summary>
        /// <returns>True if slots were added, false if not.</returns>
        public virtual bool AddSlots(uint amount, bool fireEvents = true)
        {
            uint oldSize = (uint)items.Length;
            uint newSize = oldSize + amount;

            var currentItems = new InventoryUIItemWrapperBase[newSize];
            for (uint i = 0; i < oldSize; i++)
            {
                currentItems[i] = items[i];
            }
            for (uint i = oldSize; i < newSize; i++)
            {
                currentItems[i] = CreateUIItem<InventoryUIItemWrapperBase>(i, itemButtonPrefab != null ? itemButtonPrefab : InventorySettingsManager.instance.itemButtonPrefab);
            }

            items = currentItems;

            if (fireEvents)
                NotifyResized(oldSize, newSize);

            return true;
        }

        public virtual bool CanRemoveSlots(uint amount)
        {
            if (items.Length - (int)amount < 0)
                return false;

            uint oldSize = (uint)items.Length;
            uint newSize = oldSize - amount;

            for (uint i = newSize; i < oldSize; i++)
            {
                if (items[i].item != null)
                    return false;
            }

            return true;
        }


        /// <summary>
        /// Remove slots from the collection, slots are removed at the end, if however the slots are not empty the function will not do anything and return false.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="force">When force is true, slots will be removed even if CanRemoveSlots returns false</param>
        /// <returns>True if slots were removed, false if the slots were not empty and could not be removed.</returns>
        public virtual bool RemoveSlots(uint amount, bool force = false, bool fireEvents = true)
        {
            if (CanRemoveSlots(amount) == false && force == false)
                return false;

            uint oldSize = (uint)items.Length;
            uint newSize = oldSize - amount;

            var currentItems = new InventoryUIItemWrapperBase[newSize];
            for (uint i = 0; i < newSize; i++)
            {
                currentItems[i] = items[i];
            }

            // Remove the old
            for (uint i = newSize; i < oldSize; i++)
            {
                Destroy(items[i].gameObject);
            }

            items = currentItems;

            if(fireEvents)
                NotifyResized(oldSize, newSize);

            return true;
        }


        /// <summary>
        /// Sync an item array with the items in this collection.
        /// </summary>
        /// <param name="arrToSync"></param>
        public void SyncActions(InventoryItemBase[] arrToSync)
        {
            _syncedArrays.Add(new CollectionToArraySyncer(this, arrToSync));
        }

        /// <summary>
        /// Sync a collection with this one. For example when collection A moves an object, it will also be moved in collection B.
        /// </summary>
        /// <param name="collectionToSync"></param>
        public void SyncActions(ItemCollectionBase collectionToSync, bool twoWaySyncing)
        {
            _syncedCollections.Add(new CollectionToCollectionSyncer(this, collectionToSync));
            if (twoWaySyncing)
                _syncedCollections.Add(new CollectionToCollectionSyncer(collectionToSync, this)); // And the other way around
        }

        /// <summary>
        /// Stop syncing actions.
        /// </summary>
        /// <param name="arrToSync"></param>
        public void StopSyncActions(InventoryItemBase[] arrToSync)
        {
            var a = _syncedArrays.FirstOrDefault(o => o.toArr.Equals(arrToSync));
            if (a != null)
            {
                a.UnRegister();
                _syncedArrays.Remove(a);
            }
        }

        public void StopSyncActions(ItemCollectionBase collectionToSync)
        {
            var a = _syncedCollections.FirstOrDefault(o => o.fromCollection == collectionToSync);
            if (a != null)
            {
                a.UnRegister();
                _syncedCollections.Remove(a);                
            }
        }


        /// <summary>
        /// Check if a given item type is allowed inside this collection
        /// </summary>
        /// <returns></returns>
        protected virtual bool VerifyFilters(InventoryItemBase item)
        {
            return filters.IsItemAbidingFilters(item);
        }

        protected virtual bool VerifyCustomConditionals(InventoryItemBase item)
        {
            foreach (var canAdd in canAddItemToCollectionConditionals)
            {
                if (canAdd(item) == false)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// How many items can be stored in collection with itemID
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual uint CanAddItemCount(InventoryItemBase itemToAdd)
        {
            if (canPutItemsInCollection == false)
                return 0;

            if (VerifyFilters(itemToAdd) == false)
                return 0;

            if (VerifyCustomConditionals(itemToAdd) == false)
                return 0;

            int weightLimit = 99999;
            if (restrictByWeight && itemToAdd.weight != 0.0f) // avoid dividing by 0.0f
            {
                float weightSpace = restrictMaxWeight - GetWeight();
                weightLimit = Mathf.FloorToInt(weightSpace / itemToAdd.weight);
            }
            
            uint amount = 0;
            foreach (var item in items)
            {
                if (item.item != null && item.item.ID == itemToAdd.ID)
                    amount += item.item.maxStackSize - item.item.currentStackSize;
                else if (item.item == null)
                    amount += itemToAdd.maxStackSize;
            }

            return (uint)Mathf.Min(amount, weightLimit);
        }


        /// <summary>
        /// Checks if an item can be placed inside the collection or not, checks if it's full, and checks item types.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool CanAddItem(InventoryItemBase item)
        {
            return CanAddItemCount(item) >= item.currentStackSize;
        }

        /// <summary>
        /// Add an item to the inventory, will handle stacking, placement and repainting.
        /// </summary>
        /// <param name="item">The item to store</param>
        /// <param name="storedItems">The items that were stored, item might be broken up into stacks</param>
        /// <param name="repaint">Should items be repainted? In most cases true will suit best.</param>
        /// <param name="fireEvents">Should events be fired such as OnAddedItem? Leave to true unless you know what you're doing... Don't say I didn't warn you...</param>
        /// <returns>Returns true if the item was placed, false if not.</returns>
        public virtual bool AddItem(InventoryItemBase item, ICollection<InventoryItemBase> storedItems = null, bool repaint = true, bool fireEvents = true)
        {
            if (canPutItemsInCollection == false)
                return false;

            if (item is CurrencyInventoryItem)
            {
                var currency = item as CurrencyInventoryItem;
                return currency.PickupItem();
            }

            // Instantiate the object if it's not an instance object (A prefab for example...)
            if (item.IsInstanceObject() == false)
                item = GameObject.Instantiate<InventoryItemBase>(item); // Create an instance object            

            if(storedItems == null)
                storedItems = new List<InventoryItemBase>(4); // For events and such

            uint placedCount = 0;
            bool cameFromCollection = item.itemCollection != null;

            bool placed = false;
            bool stacked = false;
            int slot = -1; // -1 if it is not placed
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].item == null)
                {
                    // Place in new slot

                    //items[i] = item;
                    bool set = SetItem((uint)i, item);
                    if (set)
                    {
                        placed = true;
                        stacked = false;
                        placedCount = item.currentStackSize;
                        slot = i;
                        storedItems.Add(item);
                        break; // All done
                    }
                }
                else if (items[i].item.ID == item.ID && canStackItemsInCollection)
                {
                    // Stack it

                    if (items[i].item.currentStackSize + item.currentStackSize <= items[i].item.maxStackSize)
                    {
                        // We can stack it
                        items[i].item.currentStackSize += item.currentStackSize;
                        placed = true;
                        stacked = true;
                        placedCount = item.currentStackSize;
                        slot = i;
                        storedItems.Add(items[i].item); // The stack we've added our items to
                        break;
                    }
                    else if (item.currentStackSize > item.maxStackSize)
                    {
						// Add to a collection, but still need to store more, so no break;
                        uint added = item.maxStackSize - items[i].item.currentStackSize;
                        placedCount += added;
                        items[i].item.currentStackSize = item.maxStackSize; // Max out stack
                        item.currentStackSize -= added;
                        storedItems.Add(items[i].item); // The stack we've added our items to
                    }
                }
            }

            if (placed)
            {
                if (stacked)
                {
                    // No longer need the object, items that are stacked are also dropped as stacks.
                    Destroy(item.gameObject);
                }
                else
                {
                    item.gameObject.SetActive(false);
                    item.transform.SetParent(containerItemsParent); // Keep things organized in the hierarchy
                }

                if (fireEvents)
                    NotifyItemAdded(storedItems, placedCount, cameFromCollection);

                if (repaint)
                    items[slot].Repaint();
            }
            else
            {
                InventoryManager.instance.lang.collectionFull.Show(item.name, item.description, collectionName);

                if (fireEvents)
                    NotifyAddedItemCollectionFull(item, cameFromCollection);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Convenience function to add an item and remove it from it's original collection.
        /// If the object was not previously in a collection this will throw an error...
        /// </summary>
        /// <param name="item"></param>
        /// <param name="storedItems">The items that were stored, item might be broken up into stacks</param>
        /// <returns>True if stored, false is not stored.</returns>
        public virtual bool AddItemAndRemove(InventoryItemBase item, ICollection<InventoryItemBase> storedItems = null, bool repaint = true)
        {
            var oldCollection = item.itemCollection;
            uint oldIndex = item.index;

            bool added = AddItem(item, storedItems, repaint);
            if (added)
            {
                oldCollection[oldIndex].item = null;
                oldCollection.NotifyItemRemoved(item, item.ID, oldIndex, item.currentStackSize);
                oldCollection[oldIndex].Repaint();

                return true;
            }

            return false;
        }

        ///// <summary>
        ///// Add a list of items in 1 go
        ///// </summary>
        ///// <param name="itemsToAdd">The items to add</param>
        ///// <param name="storedItems">The items that were stored, item might be broken up into stacks</param>
        ///// <param name="repaint"></param>
        //public virtual void AddItems(IEnumerable<InventoryItemBase> itemsToAdd, ICollection<InventoryItemBase> storedItems = null, bool repaint = true)
        //{
        //    foreach (var item in itemsToAdd)
        //    {
        //        AddItem(item, storedItems, repaint);
        //    }

        //    NotifyItemsAdded(itemsToAdd);
        //}

        /// <summary>
        /// Remove items from this collection.
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="amountToRemove"></param>
        /// <returns>The amount of item that were removed, note that when there are 4 items in this collection and you try to remove 10, 4 will be removed and the value 4 (amount of items removed) is returned.</returns>
        public virtual uint RemoveItem(uint itemID, uint amountToRemove)
        {
            uint removed = 0;
            var items = FindAll(itemID);
            foreach (var item in items)
            {
                if (removed + item.currentStackSize <= amountToRemove)
                {
                    // Take some of the stack or all if it's available

                    //item.itemCollection.SetItem(item.index, null);
                    item.itemCollection[item.index].item = null;
                    item.itemCollection.NotifyItemRemoved(item, item.ID, item.index, (uint)item.currentStackSize);
                    item.itemCollection[item.index].Repaint();

                    Destroy(item.gameObject); // Item is no longer needed
                    removed += item.currentStackSize;
                }
                else if (removed < amountToRemove)
                {
                    // Remove that's left
                    // Going over, take just a few of the stack
                    uint toRemove = amountToRemove - removed;
                    item.currentStackSize -= toRemove;
                    item.itemCollection[item.index].Repaint();
                    removed += toRemove;
                    break; // We're done our stack is complete
                }
            }

            return removed;
        }



        #region Currencies

        public InventoryCurrencyLookup GetCurrencyByID(uint currencyID)
        {
            return currenciesContainer.lookups.FirstOrDefault(o => o.currency.ID == currencyID);
        }

        public float CanAddCurrencyCount(uint currencyID)
        {
            float total = 0.0f;

            var currency = GetCurrencyByID(currencyID);
            if (currency != null)
                total += currency.CanAddCount();

            return total;
        }

        public float CanRemoveCurrencyCount(uint currencyID)
        {
            return CanRemoveCurrencyCount(currencyID, true);
        }

        /// <summary>
        /// The total amount of currency that can be removed.
        /// </summary>
        /// <param name="currencyID"></param>
        /// <param name="allowCurrencyConversions">When true all possible conversions will be considered. For example 1 gold is worth 100 silver. If you try to remove 101 silver, gold will be converted down to silver.</param>
        /// <returns></returns>
        public float CanRemoveCurrencyCount(uint currencyID, bool allowCurrencyConversions)
        {
            float total = 0.0f;

            var currency = GetCurrencyByID(currencyID);
            if (currency != null)
                total += currency.CanRemoveCount(allowCurrencyConversions);

            return total;
        }

        public bool CanAddCurrency(float amount, uint currencyID)
        {
            if (canContainCurrencies == false)
                return false;

            return CanAddCurrencyCount(currencyID) >= amount;
        }

        public bool CanRemoveCurrency(float amount, uint currencyID, bool allowCurrencyConversions)
        {
            if (canContainCurrencies == false)
                return false;

            return CanRemoveCurrencyCount(currencyID, allowCurrencyConversions) >= amount;
        }

        /// <summary>
        /// Add currency to this collection.
        /// Note: Currency is auto. converted if it's exceeding the conversion restrictions.
        /// </summary>
        /// <param name="amount">The amount to add</param>
        /// <param name="currencyID">The currencyID (type) of currency to add.</param>
        /// <returns></returns>
        public bool AddCurrency(float amount, uint currencyID)
        {
            if (CanAddCurrency(amount, currencyID) == false)
                return false;

            var currency = GetCurrencyByID(currencyID);
            if (currency != null)
            {
                currency.amount += amount;
                return true;
            }

            Debug.Log("Can't add that currency to this colleciton...");
            return false;
        }

        public bool RemoveCurrency(float amount, uint currencyID, bool allowCurrencyConversions)
        {
            if (CanRemoveCurrency(amount, currencyID, allowCurrencyConversions) == false)
                return false;

            var currency = GetCurrencyByID(currencyID);
            if (currency != null)
            {
                currency.amount -= amount;
                return true;
            }

            Debug.Log("Can't remove that currency from this colleciton...");
            return false;
        }

        public bool SetCurrencyAmount(float amount, uint currencyID)
        {
            if (canContainCurrencies == false)
                return false;

            var currency = GetCurrencyByID(currencyID);
            if (currency != null)
            {
                currency.amount = amount;
                return true;
            }

            return false;
        }


        #endregion


        /// <summary>
        /// Finds the first empty slot in this collection
        /// </summary>
        /// <returns>The index of the first empty slot, and -1 if no slot found (when collection is full)</returns>
        public virtual int FindFirstEmptySlot()
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].item == null)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Unstack an item slot to the first empty slot
        /// Unstacks in the current collection
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="amount"></param>
        /// <param name="repaint"></param>
        /// <returns>Returns true if unstacked, false if failed</returns>
        public virtual bool UnstackSlot(uint slot, uint amount, bool repaint = true)
        {
            // References can never be unstacked / stacked.
            if (useReferences || canUnstackItemsInCollection == false)
                return false;

            int firstEmptySlot = FindFirstEmptySlot();
            if (firstEmptySlot != -1)
                return UnstackSlot(slot, this, (uint) firstEmptySlot, amount, repaint);

            NotifyUnstackedItemCollectionFull(slot);

            return false;
        }


        /// <summary>
        /// Unstack an item slot to a given slot
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="toCollection"></param>
        /// <param name="toSlot"></param>
        /// <param name="amount"></param>
        /// <param name="repaint"></param>
        /// <returns>Returns true if unstacked, false if failed</returns>
        public virtual bool UnstackSlot(uint slot, ItemCollectionBase toCollection, uint toSlot, uint amount, bool repaint = true)
        {
            // References can never be unstacked / stacked.
            if (useReferences || canUnstackItemsInCollection == false)
                return false;

            if (toCollection[toSlot].item != null)
            {
                if (toCollection[toSlot].item.ID != items[slot].item.ID)
                    return false; // Different item

                if (toCollection[toSlot].item.currentStackSize + amount > toCollection[toSlot].item.maxStackSize)
                {
                    Debug.Log("Trying to unstack into a filled slot, but stack amount would override max stack size.");
                    return false;                    
                }
            }

            if (items[slot].item == null || amount > items[slot].item.currentStackSize)
                return false; // Trying to remove to many items, or trying to remove from a stack that doesn't exist...

            // Remove from old stack
            items[slot].item.currentStackSize -= amount;
            bool itemSet = false;

            if (toCollection[toSlot].item != null)
            {
                // Unstack onto new slot
                toCollection[toSlot].item.currentStackSize += amount;
                itemSet = true;
            }
            else
            {
                // Unstack onto empty slot
                var copy = GameObject.Instantiate<InventoryItemBase>(items[slot].item);
                copy.currentStackSize = (uint)amount;

                itemSet = toCollection.SetItem(toSlot, copy);
                if (itemSet)
                {
                    copy.gameObject.SetActive(false);
                    copy.transform.SetParent(containerItemsParent); // Keep things organized in the hierarchy
                }
                else
                {
                    Destroy(copy.gameObject); // Couldn't set, no longer need it
                    return false;
                }
            }

            if (itemSet)
            {
                // Notify collection
                NotifyUnstackedItem(slot, toCollection, toSlot, amount);

                // Notify item
                items[slot].item.NotifyItemUnstacked(toSlot, amount);

                if (repaint)
                {
                    items[slot].Repaint();
                    toCollection[toSlot].Repaint();
                }

                return true; // All done                
            }

            // Inventory is full, got no space for that item
            NotifyUnstackedItemCollectionFull(slot);

            return false;
        }


        /// <summary>
        /// Swap 2 slots, useful for re-aranging elements in the UI.
        /// </summary>
        /// <param name="slot1">Index 1</param>
        /// <param name="slot2">Index 2</param>
        protected virtual bool SwapSlots(uint fromSlot, ItemCollectionBase toCollection, uint toSlot, bool repaint = true, bool fireEvents = true)
        {
            ItemCollectionBase fromCollection = this;

            if (fromCollection.CanSetItem(fromSlot, toCollection[toSlot].item) == false || toCollection.CanSetItem(toSlot, fromCollection[fromSlot].item) == false)
                return false;

            var tempTo = toCollection.items[toSlot].item;
            uint fromTempAmount = fromCollection[fromSlot].item != null ? fromCollection[fromSlot].item.currentStackSize : 0; // TODO: fromCollection item shouldn't be empty!
            //uint toTempAmount = toCollection[toSlot].item.currentStackSize;

            // Move to first item to toCollection
            bool moved = fromCollection.MoveItem(fromCollection[fromSlot].item, fromSlot, toCollection, toSlot, true, false); // Moves the item into toCollection[toSlot]
            if (moved == false)
                return false;

            // Moving to another collection, so we're actually removing one in this collection
            if (fromCollection != toCollection)
            {
                if (toCollection.useReferences == false && fromCollection.useReferences == false && fireEvents)
                {
                    // if tempTo == null then the slot we're moving to didn't have an object before, so then one won't be removed.
                    if (tempTo != null)
                        toCollection.NotifyItemRemoved(tempTo, tempTo.ID, toSlot, tempTo.currentStackSize);

                    if (toCollection[toSlot].item != null)
                        toCollection.NotifyItemAdded(toCollection[toSlot].item, fromTempAmount, true);
                }
            }

            // And move the temp item to fromCollection, and it's swapped :)
            if ((toCollection.useReferences == false && fromCollection.useReferences == false) || (toCollection.useReferences && fromCollection.useReferences))
                toCollection.MoveItem(tempTo, toSlot, fromCollection, fromSlot, false, false);

            // Moving to another collection, so we're actually removing one in this collection
            if (fromCollection != toCollection)
            {
                if (fromCollection.useReferences == false && toCollection.useReferences == false && fireEvents)
                {
                    if (toCollection[toSlot].item != null)
                        fromCollection.NotifyItemRemoved(toCollection[toSlot].item, toCollection[toSlot].item.ID, fromSlot, toCollection[toSlot].item.currentStackSize);

                    // if temp == null then the slot we're moving to didn't have an object before, so then one won't be removed.
                    if (tempTo != null)
                        fromCollection.NotifyItemAdded(fromCollection[fromSlot].item, fromCollection[fromSlot].item.currentStackSize, true);
                }
            }

            //Debug.Log("Swapped slots " + fromSlot + " in " + fromCollection.ToString() + " with " + toSlot + " in " + toCollection.ToString());

            if (repaint)
            {
                fromCollection.items[fromSlot].Repaint();
                toCollection.items[toSlot].Repaint();
            }

            if(fireEvents && (fromCollection.useReferences == false && toCollection.useReferences == false))
                NotifyItemSwapped(fromCollection, fromSlot, toCollection, toSlot);

            return true;
        }

        /// <summary>
        /// Determines whether this 2 slots can be merged / stacked together.
        /// </summary>
        /// <returns><c>true</c> if this instance can merge the specified slot1 slot2; otherwise, <c>false</c>.</returns>
        /// <param name="slot1">Index 1.</param>
        /// <param name="slot2">Index 2.</param>
        public virtual bool CanMergeSlots(uint slot1, ItemCollectionBase collection2, uint slot2)
        {
            ItemCollectionBase collection1 = this;

            if (canStackItemsInCollection == false)
                return false;

            // If slots are empty they cannot be merged.
            if (collection1.items[slot1].item == null || collection2.items[slot2].item == null)
                return false;

            if (collection2.canPutItemsInCollection == false)
                return false;

            // References cannot be merged.
            if (collection1.useReferences || collection2.useReferences)
                return false;

            // Same item ?
            if (collection1.items[slot1].item.ID == collection2.items[slot2].item.ID)
            {
                return collection1.items[slot1].item.currentStackSize + collection2.items[slot2].item.currentStackSize <= collection1.items[slot1].item.maxStackSize; // Does it fit?
            }

            return false;
        }

        /// <summary>
        /// Merge the specified slot1 and slot2.
        /// Slot 1 will be the new slot -> Slot 2 is moved to slot 1, and slot 2 is cleared.
        /// </summary>
        /// <param name="slot1">Index 1.</param>
        /// <param name="slot2">Index 2.</param>
        protected virtual bool MergeSlots(uint slot1, ItemCollectionBase collection2, uint slot2, bool repaint = true, bool fireEvents = true)
        {
            ItemCollectionBase collection1 = this;

            if (CanMergeSlots(slot1, collection2, slot2))
            {
                uint tempStackSize = collection1.items[slot1].item.currentStackSize;
                var tempItemCollection1 = collection1.items[slot1].item;
                collection2.items[slot2].item.currentStackSize += collection1.items[slot1].item.currentStackSize;
                collection1[slot1].item = null;

                // Moving to a different collection
                if(collection1 != collection2)
                {
                    collection2.NotifyItemAdded(collection2[slot2].item, tempStackSize, true); // Items are moved to collection2 so it gains the stack of the other
                    collection1.NotifyItemRemoved(tempItemCollection1, tempItemCollection1.ID, slot1, tempStackSize); // Item in collection1 is moved to collection2, so it loses an object.

                    Debug.Log("Merged an item in another collection " + collection2.ToString() + " gained " + tempStackSize + " items of ID: " + collection2[slot2].item.ID + " and removed " + tempStackSize + " of " + collection1.ToString() + " with item ID: " + collection2[slot2].item.ID + 
                              "\nThere are now " + collection2.GetItemCount(collection2[slot2].item.ID) + " items in " + collection2.ToString() + " and " + collection1.GetItemCount(collection2[slot2].item.ID) + " in " + collection1.ToString());
                }

                if (fireEvents)
                    NotifyMergedSlots(collection1, slot1, collection2, slot2);

                if (repaint)
                {
                    collection2.items[slot2].Repaint();
                    collection1.items[slot1].Repaint();
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Merges the slots if possible, if not it will swap them out.
        /// </summary>
        /// <param name="slot1">Slot1.</param>
        /// <param name="slot2">Slot2.</param>
        /// <returns>True if the item was swapped or merged / False if the action could not be completed.</returns>
        public virtual bool SwapOrMerge(uint slot1, ItemCollectionBase handler2, uint slot2, bool repaint = true)
        {
            ItemCollectionBase handler1 = this;

            // Not actually moving anything...
            if (handler1 == handler2 && slot1 == slot2)
                return false;

            if (handler2.canPutItemsInCollection == false)
                return false;

            if (CanMergeSlots(slot1, handler2, slot2))
                return MergeSlots(slot1, handler2, slot2, repaint);
            else
                return SwapSlots(slot1, handler2, slot2, repaint);
        }

        /// <summary>
        /// Can an item be placed in a slot, does type checking auto.
        /// </summary>
        /// <param name="slot">The slot we're trying to set</param>
        /// <param name="item">The item we're trying to set</param>
        /// <returns></returns>
        public virtual bool CanSetItem(uint slot, InventoryItemBase item)
        {
            if (item == null)
                return true;

            if (canPutItemsInCollection == false)
                return false;

            if (restrictByWeight && GetWeight() + item.weight * item.currentStackSize > restrictMaxWeight)
                return false; // To much weight

            if (VerifyFilters(item) == false)
                return false;

            return true;
        }

        /// <summary>
        /// <b>DOES NOT FIRE ADD / REMOVE EVENTS!</b>
        /// This function can be overridden to add custom behavior whenever an object is placed in the inventory.
        /// This happens when 2 items are swapped, items are merged, anytime an object is put in a slot.
        /// <b>Does not handle repainting</b>
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="item"></param>
        /// <returns>Returns true if the item was set, false if not.</returns>
        public virtual bool SetItem(uint slot, InventoryItemBase item)
        {
            if (CanSetItem(slot, item) == false)
                return false;

            items[slot].item = item;
			items[slot].itemCollection = this;
                
            return true;
        }
        public bool SetItem(uint slot, InventoryItemBase item, bool repaint)
        {
            bool s = SetItem(slot, item);
            if(repaint)
                items[slot].Repaint();
                
            return s;
        }
        

        /// <summary>
        /// Set an array of items, fails if the collection is smaller than the amount of items setting.
        /// Note: This removed all items from the collection and replaces it with the given array.
        /// Note2: This ignores any restrictions of type and collection settings.
        /// </summary>
        /// <param name="toSet">Items to set, the remainder will be set to null</param>
        /// <param name="setParent">Move the transforms to the new position</param>
        public virtual void SetItems(InventoryItemBase[] toSet, bool setParent, bool repaint = true)
        {
            Resize((uint)toSet.Length);

            for (int i = 0; i < toSet.Length; i++)
            {
                if (items[i].item != null)
                    NotifyItemRemoved(items[i].item, items[i].item.ID, items[i].item.index, items[i].item.currentStackSize);

                items[i].item = toSet[i];
                if (items[i].item != null)
                {
                    bool cameFromCollection = items[i].itemCollection != null;
                    items[i].itemCollection = this;

                    NotifyItemAdded(items[i].item, items[i].item.currentStackSize, cameFromCollection);

                    if (setParent)
                    {
                        items[i].item.transform.SetParent(containerItemsParent);
                        items[i].item.gameObject.SetActive(false);
                    }
                }

                if (repaint)
                    items[i].Repaint();
            }

            for (int i = toSet.Length; i < items.Length; i++)
            {
                if (items[i].item != null)
                    NotifyItemRemoved(items[i].item, items[i].item.ID, items[i].item.index, items[i].item.currentStackSize);

                items[i].item = null;

                if (repaint)
                    items[i].Repaint();
            }
        }

        /// <summary>
        /// Item usabilities added to all items in this collection, none by default.
        /// </summary>
        public virtual IList<InventoryItemUsability> GetExtraItemUsabilities(IList<InventoryItemUsability> basic)
        {
            return basic;
        }


        /// <summary>
        /// Sorts the inventory and handles the required repaint.
        /// To change sorting behavior implement the IInventorySorter.
        /// </summary>
        public virtual void SortCollection()
        {
            var list = new List<InventoryItemBase>(items.Length);
            for (uint i = 0; i < items.Length; i++)
            {
                if (items[i].item != null)
                {
                    list.Add(items[i].item);
                    //NotifyItemRemoved(items[i].item.ID, i, items[i].item.currentStackSize);
                    items[i].item = null;
                }
            }
        

            var tempItems = InventorySettingsManager.instance.collectionSorter.Sort(list);
        

            // Add all items (they're already cleared)
            foreach (var item in tempItems)
                AddItem(item, null, false, false);

            // Repaint all UI elements
            foreach(var item in items)
                item.Repaint();

            NotifySorted();

            Debug.Log("Sorting collection: " + this.ToString());
        }

        /// <summary>
        /// Places an object in a slot and handles repaint, this method can easilly be overwritten for extra functinoality when placing an item.
        /// </summary>
        /// <param name="fromSlot"></param>
        /// <param name="toCollection"></param>
        /// <param name="toSlot"></param>
        /// <param name="doRepaint"></param>
        public virtual bool MoveItem(InventoryItemBase item, uint fromSlot, ItemCollectionBase toCollection, uint toSlot, bool clearOld, bool doRepaint = true)
        {
            // Not actually moving anything.
            if (this == toCollection && fromSlot == toSlot)
                return false;

            if (toCollection.CanSetItem(toSlot, item) == false)
                return false;

            // Only remove the item if we're not dealing with references.
            // If we're not using reference move the item from 1 place to the next.
            if (useReferences && toCollection.useReferences == false)
            {
                items[fromSlot].item = null;

                if (doRepaint)
                    items[fromSlot].Repaint();
            
                return true;
            }

            // Move to reference, without clearing it.
            if(useReferences == false && toCollection.useReferences)
            {
                bool set = toCollection.SetItem(toSlot, item);
                if (set == false)
                    return false;

                if (doRepaint)
                    toCollection[toSlot].Repaint();
            
                return true;
            }

            // Ignore the repaint request, references have to be repainted...
            if (useReferences && toCollection.useReferences)
            {
                toCollection.SetItem(toSlot, item); // Make a copy, that's it

                if (doRepaint)
                    toCollection[toSlot].Repaint(); // Ignore doRepaint, and do it anyway

                return true;
            }

            // Just simply move it, all that's left
            toCollection.SetItem(toSlot, item);
            if (clearOld)
                items[fromSlot].item = null;
                //SetItem(fromSlot, null);

            if (doRepaint)
            {
                toCollection[toSlot].Repaint();
                items[fromSlot].Repaint();
            }

            return true;
        }

        /// <summary>
        /// Find returns the first item in the collection that matches the ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The first object / stack that matches the ID. Returns null if the object is not found.</returns>
        public InventoryItemBase Find(uint id)
        {
            foreach(var item in items)
            {
                if (item.item != null && item.item.ID == id)
                    return item.item;
            }

            return null;
        }

        /// <summary>
        /// Find returns an array of all items that match the ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>An array of all the items that match the ID.Returns empty array if no objects are found.</returns>
        public InventoryItemBase[] FindAll(uint id)
        {
            var list = new List<InventoryItemBase>(items.Length);
            foreach (var item in items)
            {
                if (item.item != null && item.item.ID == id)
                    list.Add(item.item);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Find an item by category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public InventoryItemBase Find(InventoryItemCategory category)
        {
            foreach (var item in items)
            {
                if (item.item != null && item.item.category.ID == category.ID)
                    return item.item;
            }

            return null;
        }

        /// <summary>
        /// Find all items in a category.
        /// </summary>
        /// <param name="category"></param>
        /// <returns>All items in the given category.</returns>
        public InventoryItemBase[] FindAll(InventoryItemCategory category)
        {
            var list = new List<InventoryItemBase>(items.Length);
            foreach (var item in items)
            {
                if (item.item != null && item.item.category.ID == category.ID)
                    list.Add(item.item);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Counting how many items there are with the given ID.
        /// Lookup is O(N) (Linear performance)
        /// </summary>
        /// <param name="id">item.ID</param>
        /// <returns>The amount of items in the collection or a given ID.</returns>
        public uint GetItemCount(uint id)
        {
            return (uint)FindAll(id).Sum(i => i.currentStackSize);
        }

        /// <summary>
        /// Get the amount of empty slots in the collection
        /// </summary>
        /// <returns>Amount of empty slots</returns>
        public uint GetEmptySlotsCount()
        {
            return (uint)items.Sum(o => o.item == null ? 1 : 0);
        }


        /// <summary>
        /// Get the total weight of all items inside this collection
        /// </summary>
        /// <returns></returns>
        public virtual float GetWeight()
        {
            return items.Sum(o => o.item == null ? 0.0f : o.item.weight * o.item.currentStackSize);
        }

        #endregion


        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        public IEnumerator<InventoryUIItemWrapperBase> GetEnumerator()
        {
            return ((IEnumerable<InventoryUIItemWrapperBase>) items).GetEnumerator();
        }

        public override string ToString()
        {
            return collectionName;
        }


    }
}