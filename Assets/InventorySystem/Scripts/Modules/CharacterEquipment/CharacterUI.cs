using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;
using System.Linq;
using System.Reflection;
using Devdog.InventorySystem;
using Devdog.InventorySystem.Models;
using Devdog.InventorySystem.UI;

namespace Devdog.InventorySystem
{
    [AddComponentMenu("InventorySystem/Windows/Character")]
    [RequireComponent(typeof(UIWindow))]
    public partial class CharacterUI : ItemCollectionBase, ICollectionPriority, IInventoryDragAccepter
    {
        #region Eventns

        //public delegate void StatsChanged(Dictionary<string, List<InventoryCharacterStat>> stats);
        public delegate void StatChanged(InventoryCharacterStat stat);

        //public event StatsChanged OnStatsChanged;
        public event StatChanged OnStatChanged;

        #endregion


        /// <summary>
        /// The container where the generated stats will be placed.
        /// </summary>
        public RectTransform statsContainer;


        [Range(0, 100)]
        [SerializeField]
        private int _collectionPriority;
        public int collectionPriority
        {
            get { return _collectionPriority; }
            set { _collectionPriority = value; }
        }

        public bool isSharedCollection = false;


        [Header("UI Prefabs")]
        public InventoryEquipStatRowUI statusRowPrefab;
        public InventoryEquipStatCategoryUI statusCategoryPrefab;
        

        public InventoryEquippableField[] equipSlotFields { get; private set; }
        public Dictionary<string, List<InventoryCharacterStat>> characterStats { get; protected set; }

        [NonSerialized]
        protected InventoryPool<InventoryEquipStatRowUI> rowsPool;

        [NonSerialized]
        protected InventoryPool<InventoryEquipStatCategoryUI> categoryPool;



        public List<ICharacterStatDataProvider> statsDataProviders { get; set; } 


        private UIWindow _window;
        public UIWindow window
        {
            get
            {
                if (_window == null)
                    _window = GetComponent<UIWindow>();

                return _window;
            }
            protected set { _window = value; }
        }

        public override uint initialCollectionSize
        {
            get
            {
                return (uint)equipSlotFields.Length;
            }
        }

        public override void Awake()
        {
            equipSlotFields = new InventoryEquippableField[items.Length];
            for (int i = 0; i < items.Length; i++)
                equipSlotFields[i] = items[i].gameObject.GetComponent<InventoryEquippableField>();

            
            characterStats = new Dictionary<string, List<InventoryCharacterStat>>(16);
            statsDataProviders = new List<ICharacterStatDataProvider>(2);


            base.Awake();
            SetDefaultDataProviders();
            PrepareCharacterStats();


            if (isSharedCollection)
                InventoryManager.AddEquipCollection(this, collectionPriority);

            if (statusRowPrefab != null)
                rowsPool = new InventoryPool<InventoryEquipStatRowUI>(statusRowPrefab, 32);

            if (statusCategoryPrefab != null)
                categoryPool = new InventoryPool<InventoryEquipStatCategoryUI>(statusCategoryPrefab, 8);




            OnAddedItem += (itemsAdded, amount, cameFromCollection) =>
            {
                foreach (var item in itemsAdded)
                    ((EquippableInventoryItem)item).NotifyItemEquipped(equipSlotFields[item.index]);

                //RepaintStats();
                //UpdateCharacterStats();
            };
            OnRemovedItem += (item, itemID, slot, amount) =>
            {
                ((EquippableInventoryItem)item).NotifyItemUnEquipped();

                //RepaintStats();
                //UpdateCharacterStats();
            };
            OnSwappedItems += (collection, slot, toCollection, toSlot) =>
            {
                if (collection == this)
                {
                    //if (toCollection[toSlot].item != null)
                    //((EquippableInventoryItem)toCollection[toSlot].item).HandleLocks(equipSlotFields[slot], collection, this);
                }
                else
                {
                    if (toCollection[toSlot].item != null)
                        ((EquippableInventoryItem)toCollection[toSlot].item).HandleLocks(equipSlotFields[toSlot], collection, this);
                }
            };


            window.OnShow += RepaintAllStats;
        }


        public override void Start()
        {
            base.Start();

            //UpdateCharacterStats();


           
        }


        protected virtual void SetDefaultDataProviders()
        {
            statsDataProviders.Add(new CharacterStatsPropertiesDataProvider(this));
            statsDataProviders.Add(new CharacterStatsDataProvider(this));
        }

        //public void NotifyStatsChanged()
        //{
        //    if (OnStatsChanged != null)
        //        OnStatsChanged(characterStats);

        //    RepaintStats();
        //}

        public void NotifyStatChanged(InventoryCharacterStat stat)
        {
            if (OnStatChanged != null)
                OnStatChanged(stat);

            RepaintStat(stat);
        }


        public bool AcceptsDragItem(InventoryItemBase item)
        {
            return item is EquippableInventoryItem;
            //return onlyAllowTypes.Contains(item.GetType());
        }

        /// <summary>
        /// Called by the InventoryDragAccepter, when an item is dropped on the window / a specific location, this method is called to add a custom behavior.
        /// </summary>
        /// <param name="item"></param>
        public bool AcceptDragItem(InventoryItemBase item)
        {
            if (AcceptsDragItem(item) == false)
                return false;

            var equippable = (EquippableInventoryItem) item;
            var bestSlot = equippable.GetBestEquipSlot(this);
            return equippable.Equip(bestSlot, this);
        }


        /// <summary>
        /// Get all slots where this item can be equipped.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Returns indices where the item can be equipped. collection[index] ... </returns>
        public InventoryEquippableField[] GetEquippableSlots(EquippableInventoryItem item)
        {
            var equipSlots = new List<InventoryEquippableField>(4);
            foreach (var field in equipSlotFields)
            {
                foreach (var type in field.equipTypes)
                {
                    if (item.equipType.ID == type.ID)
                    {
                        equipSlots.Add(field);
                    }
                }
            }

            return equipSlots.ToArray();
        }


        /// <summary>
        /// Convenience method to grab a stat from this character.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public InventoryCharacterStat GetStat(string category, string name)
        {
            if (characterStats.ContainsKey(category) == false)
            {
                return null;
            }

            return characterStats[category].FirstOrDefault(o => o.statName == name);
        }

        /// <summary>
        /// Gets the categories and stat rows, but doesn't fill them yet.
        /// </summary>
        protected virtual void PrepareCharacterStats()
        {
            characterStats.Clear();


            foreach (var dataProvider in statsDataProviders)
                dataProvider.Prepare(characterStats);

        }

        protected virtual void ClearCharacterStats()
        {
            foreach (var category in characterStats)
            {
                foreach (var stat in category.Value)
                {
                    stat.Reset();
                }
            }
        }


        ///// <summary>
        ///// Show the status
        ///// </summary>
        //public virtual void ResetCharacterStats(bool repaint = true)
        //{
        //    ClearCharacterStats();

        //    foreach (var dataProvider in statsDataProviders)
        //        dataProvider.Prepare(characterStats);

        //    // Call events before the repaint to allow modification in the events, without needing a 2nd repaint.
        //    NotifyCalculatedStats(characterStats);

        //    if (repaint)
        //        RepaintStats();
        //}

        public virtual void RepaintAllStats()
        {
            if (window.isVisible == false || statusRowPrefab == null || statusCategoryPrefab == null)
                return;

            // Get rid of the old
            categoryPool.DestroyAll();
            rowsPool.DestroyAll();

            // Maybe make a pool for the items? See some spikes...
            foreach (var stat in characterStats)
            {
                // Maybe make a pool for the items? See some spikes...
                if (stat.Value.Count(o => o.showInUI) == 0)
                    continue; // No items to display in this category.

                // stat.Key is category
                // stat.Value is all items in category 
                var cat = categoryPool.Get();
                cat.SetCategory(stat.Key);
                cat.transform.SetParent(statsContainer);
                cat.transform.localPosition = new Vector3(cat.transform.localPosition.x, cat.transform.localPosition.y, 0.0f);

                foreach (var s in stat.Value)
                {
                    if (s.showInUI == false)
                        continue;

                    var obj = rowsPool.Get();
                    obj.Repaint(s.statName, s.ToString());

                    obj.transform.SetParent(cat.container);
                    obj.transform.localPosition = Vector3.zero; // UI Layout will handle it.
                }
            }
        }

        /// <summary>
        /// Repaint a single stat.
        /// </summary>
        /// <param name="stat"></param>
        public virtual void RepaintStat(InventoryCharacterStat stat)
        {
            if (window.isVisible == false || statusRowPrefab == null || statusCategoryPrefab == null)
                return;

            foreach (var row in rowsPool)
            {
                if (row.statName.text == stat.statName)
                {
                    row.Repaint(stat.statName, stat.ToString());
                }
            }
        }


        public override void SetItems(InventoryItemBase[] toSet, bool setParent, bool repaint = true)
        {
            base.SetItems(toSet, setParent, repaint);

            //UpdateCharacterStats(window.isVisible);
        }

        public override bool CanSetItem(uint slot, InventoryItemBase item)
        {
            bool set = base.CanSetItem(slot, item);
            if (set == false)
                return false;

            if (item == null)
                return true;

            var equippable = item as EquippableInventoryItem;
            if (equippable == null)
                return false; // Can't equip this item type, only Equippable and anything that inherits from Equippable.

            var slots = GetEquippableSlots(equippable);
            if (slots.Length == 0)
                return false;

            // An acceptable slot?
            foreach (var s in slots)
            {
                if (s.index == slot)
                    return true;
            }
            
            return false;
        }

        public override bool CanMergeSlots(uint slot1, ItemCollectionBase collection2, uint slot2)
        {
            return false;
        }
        public override bool SwapOrMerge(uint slot1, ItemCollectionBase handler2, uint slot2, bool repaint = true)
        {
            return SwapSlots(slot1, handler2, slot2, repaint);
        }
    }
}