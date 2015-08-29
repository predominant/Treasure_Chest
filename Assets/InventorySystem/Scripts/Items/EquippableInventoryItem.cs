using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem
{
    /// <summary>
    /// Used to represent items that can be equipped by the user, this includes armor, weapons, etc.
    /// </summary>
    public partial class EquippableInventoryItem : InventoryItemBase
    {
        #region Events

        /// <summary>
        /// Called by the collection once the item is successfully equipped.
        /// </summary>
        /// <param name="equippedTo"></param>
        public delegate void Equipped(InventoryEquippableField equippedTo);

        /// <summary>
        /// Called when the item is equipped visually / bound to a mesh.
        /// Useful if you wish to remove a custom component whenever the item is equipped.
        /// </summary>
        /// <param name="binder"></param>
        public delegate void EquippedVisually(InventoryPlayerEquipTypeBinder binder, InventoryPlayer player);

        /// <summary>
        /// Called by the collection once the item is successfully un-equipped
        /// </summary>
        public delegate void UnEquipped();



        /// <summary>
        /// Called by the collection once the item is successfully equipped.
        /// </summary>
        /// <param name="equippedTo"></param>
        public event Equipped OnEquipped;

        /// <summary>
        /// Called when the item is equipped visually / bound to a mesh.
        /// Useful if you wish to remove a custom component whenever the item is equipped.
        /// </summary>
        public event EquippedVisually OnEquippedVisually;


        /// <summary>
        /// Called when the item is unequipped visually / removed from the equipment mesh
        /// </summary>
        public event EquippedVisually OnUnEquippedVisually;


        /// <summary>
        /// Called by the collection once the item is successfully un-equipped
        /// </summary>
        public event UnEquipped OnUnEquipped;

        #endregion




        /// <summary>
        /// Equip type ID's, Manage in InventoryManager/Item Editor/Equip types
        /// </summary>
        [SerializeField]
        [HideInInspector]
        public int _equipType;
        public InventoryEquipType equipType
        {
            get
            {
                return ItemManager.instance.equipTypes[_equipType];
            }
        }


        public AudioClip playOnEquip;

        /// <summary>
        /// Consider this item for visual equipment?
        /// </summary>
        public bool equipVisually = true;

        /// <summary>
        /// The position / offset used when equipping the item visually
        /// </summary>
        public Vector3 equipPosition;

        /// <summary>
        /// The rotation of the item when equipping the item visually
        /// </summary>
        public Quaternion equipRotation;


        /// <summary>
        /// Is the item currently equipped or not?
        /// </summary>
        public bool isEquipped { get; protected set; }


        /// <summary>
        /// Called by the collection once the item is successfully equipped.
        /// </summary>
        /// <param name="equipSlot"></param>
        public void NotifyItemEquipped(InventoryEquippableField equipSlot)
        {
            isEquipped = true;

            SetItemProperties(InventoryPlayerManager.instance.currentPlayer, InventoryItemUtility.SetItemPropertiesAction.Use);

            if (OnEquipped != null)
                OnEquipped(equipSlot);

            if (playOnEquip != null)
                InventoryUtility.AudioPlayOneShot(playOnEquip);
        }
        
        /// <summary>
        /// Called when the item is equipped visually / bound to a mesh.
        /// Useful if you wish to remove a custom component whenever the item is equipped.
        /// </summary>
        /// <param name="binder"></param>
        public void NotifyItemEquippedVisually(InventoryPlayerEquipTypeBinder binder, InventoryPlayer player)
        {


            if (OnEquippedVisually != null)
                OnEquippedVisually(binder, player);
        }

        /// <summary>
        /// Called when the item is equipped visually / bound to a mesh.
        /// Useful if you wish to remove a custom component whenever the item is equipped.
        /// </summary>
        /// <param name="binder"></param>
        public void NotifyItemUnEquippedVisually(InventoryPlayerEquipTypeBinder binder, InventoryPlayer player)
        {
            if (OnUnEquippedVisually != null)
                OnUnEquippedVisually(binder, player);
        }

        /// <summary>
        /// Called by the collection once the item is successfully un-equipped
        /// </summary>
        public void NotifyItemUnEquipped()
        {
            isEquipped = false;

            SetItemProperties(InventoryPlayerManager.instance.currentPlayer, InventoryItemUtility.SetItemPropertiesAction.UnUse);

            if (OnUnEquipped != null)
                OnUnEquipped();
        }

        

        protected virtual void SetItemProperties(InventoryPlayer player, InventoryItemUtility.SetItemPropertiesAction action)
        {
            // Just to be safe...
            foreach (var property in properties)
            {
                property.actionEffect = InventoryItemProperty.ActionEffect.Add;
            }

            InventoryItemUtility.SetItemProperties(player, properties, action);
        }


        public override GameObject Drop(Vector3 location, Quaternion rotation)
        {
            // Currently in a equip to collection?
            if (isEquipped)
            {
                if (itemCollection != null)
                    itemCollection.SetItem(index, null);

                NotifyItemUnEquipped(); // Actually using set item, exception where collection doesn't handle this
            }

            return base.Drop(location, rotation);
        }


        public override int Use()
        {
            int used = base.Use();
            if (used < 0)
                return used;

            if (isEquipped)
            {
                bool unequipped = UnEquip(true);
                if (unequipped)
                    return 1; // Used from inside the character, move back to inventory.

                return 0; // Couldn't un-unequip
            }

            var equipTo = GetBestEquipToCollection();
            if (equipTo == null)
            {
                Debug.LogWarning("No equip collection found for item " + name + ", forgot to assign the character collection to the player?", transform);
                return -2; // No equip to collections found
            }

            var equipSlot = GetBestEquipSlot(equipTo);
            if (equipSlot == null)
            {
                Debug.LogWarning("No suitable equip slot found for item " + name, transform);    
                return -2; // No slot found
            }

            bool equipped = Equip(equipSlot, equipTo);
            if (equipped)
                return 1;

            return -2;
        }

        protected virtual CharacterUI GetBestEquipToCollection()
        {
            CharacterUI last = null;
            foreach (var col in InventoryManager.GetEquipToCollections())
            {
                var best = GetBestEquipSlot(col);
                if (best != null && (best.itemWrapper.item == null || last == null))
                {
                    last = col;
                }
            }

            return last;
        }

        /// <summary>
        /// Verifies if the item can be equipped or not.
        /// This is validated after CanSetItem, so the item can be rejected before it gets here, if it doesn't match onlyAllowTypes.
        /// </summary>
        /// <returns>True if the item can be equipped, false if not.</returns>
        public virtual bool CanEquip()
        {
            return true;
        }

        /// <summary>
        /// Equip the item to the given collection
        /// </summary>
        /// <param name="equipSlot">Equip to slot</param>
        /// <param name="equipTo">Collection to equip to</param>
        /// <returns></returns>
        public virtual bool Equip(InventoryEquippableField equipSlot, CharacterUI equipTo)
        {
            if (CanEquip() == false)
                return false;

            bool handled = HandleLocks(equipSlot, itemCollection, equipTo);
            if (handled == false)
                return false; // Other items cannot be unequipped

            // There was already an item in this slot, un-equip that one first
            if (equipTo[equipSlot.index].item != null)
            {
                var item = equipTo[equipSlot.index].item as EquippableInventoryItem;
                bool unEquipped = item.UnEquip(false);
                if (unEquipped == false)
                    return false;
            }

            // The values before the collection / slot changed.
            uint prevIndex = index;
            var fromCollection = itemCollection;

            // Equip the item -> Will swap as merge is not possible
            bool canSet = equipTo.CanSetItem(equipSlot.index, this);
            if (canSet)
            {
                NotifyItemUsed(1, true); // Events still valid, handling before the set

                equipTo.SetItem(equipSlot.index, this);
                if(fromCollection != null)
                    fromCollection.SetItem(prevIndex, null);

                equipTo.NotifyItemAdded(this, currentStackSize, itemCollection != null);
                if (fromCollection != null)
                {
                    fromCollection.NotifyItemRemoved(this, ID, prevIndex, currentStackSize);
                    fromCollection[prevIndex].Repaint();
                }

                //NotifyItemEquipped(equipSlot); // Collection defines when an item is equipped or unequipped
                equipTo[equipSlot.index].Repaint();
                //fromCollection.NotifyItemUsed(this, ID, prevIndex, 1);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Unequip this item
        /// </summary>
        /// <param name="wasUsed">Was the item used or forced out by a blocking item or swap?
        /// Set to true if the action was continuously initiated, false if it happened through some other action.</param>
        /// <returns>True if successfully unequipped, false if couldn't un-equip</returns>
        public virtual bool UnEquip(bool wasUsed)
        {
            uint prevIndex = index;
            var fromCollection = itemCollection;

            bool added = InventoryManager.CanAddItem(this);
            if (added == false)
                return false;

            // Was the item actually used or was it swapped / forced out?
            if (wasUsed)
            {
                NotifyItemUsed(1, false); // NOTE: Collection changed, collection - event no longer valid!
                fromCollection.NotifyItemUsed(this, ID, prevIndex, 1);                
            }

            InventoryManager.AddItemAndRemove(this); // Already verified if possible

            // NotifyItemUnEquipped(); // Collection defines events

            return true;
        }

        public virtual InventoryEquippableField GetBestEquipSlot(CharacterUI equipCollection)
        {
            var equipSlots = equipCollection.GetEquippableSlots(this);
            if (equipSlots.Length == 0)
            {
                Debug.LogWarning("No suitable equip slot found for item " + name, gameObject);
                return null;
            }

            InventoryEquippableField equipSlot = equipSlots[0];
            foreach (var e in equipSlots)
            {
                if (equipCollection[e.index].item == null)
                {
                    equipSlot = e; // Prefer an empty slot over swapping a filled one.
                }
            }

            return equipSlot;
        }

        /// <summary>
        /// Some item's require multiple slots, for example a 2 handed item forces the left handed item to be empty.
        /// </summary>
        /// <returns>true if items were removed, false if items were not removed.</returns>
        public virtual bool HandleLocks(InventoryEquippableField equipSlot, ItemCollectionBase usedFromCollection, CharacterUI equipTo)
        {
            var toBeRemoved = new List<uint>(8);

            // Loop through things we want to block
            foreach (var blockType in equipType.blockTypes)
            {
                // Check every slot against this block type
                foreach (var field in equipTo.equipSlotFields)
                {
                    var item = equipTo[field.index].item;
                    if(item != null)
                    {
                        var eq = (EquippableInventoryItem)item;

                        if(eq.equipType.ID == blockType && field.index != equipSlot.index)
                        {
                            toBeRemoved.Add(field.index);
                            bool canAdd = InventoryManager.CanAddItem(eq);
                            if (canAdd == false)
                                return false;
                        }
                    }
                }
            }
            
            foreach (uint i in toBeRemoved)
            {
                var item = equipTo[i].item as EquippableInventoryItem;
                bool added = InventoryManager.AddItemAndRemove(item);
                if (added == false)
                {
                    Debug.LogError("Item could not be saved, even after check, please report this bug + stacktrace.");
                    return false;
                }
            }

            return true;
        }
    }
}