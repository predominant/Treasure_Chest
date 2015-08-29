using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.Models;
using UnityEngine;

namespace Devdog.InventorySystem
{
    public partial class InventoryPlayerEquipHelper
    {
        public enum EquipHandlerType
        {
            /// <summary>
            /// Replace the equip location's mesh with the new model.
            /// </summary>
            Replace,

            /// <summary>
            /// Make the model a child of the equip location.
            /// </summary>
            MakeChild
        }

        /// <summary>
        /// Called when the item is equipped visually / bound to a mesh.
        /// Useful if you wish to remove a custom component whenever the item is equipped.
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="item"></param>
        public delegate void EquippedVisually(InventoryPlayerEquipTypeBinder binder, EquippableInventoryItem item);
        public event EquippedVisually OnEquippedVisually;
        public event EquippedVisually OnUnEquippedVisually;


        protected InventoryPlayer player;

        public InventoryPlayerEquipHelper(InventoryPlayer player)
        {
            this.player = player;


            if (player.characterCollection == null)
                return;

            player.characterCollection.OnAddedItem += (items, amount, collection) =>
            {
                var item = items.FirstOrDefault();
                if (item is EquippableInventoryItem)
                    EquipItemVisually((EquippableInventoryItem)item, player.characterCollection.equipSlotFields[item.index]);
            };
            player.characterCollection.OnRemovedItem += (item, id, slot, amount) =>
            {
                if (item is EquippableInventoryItem)
                    UnEquipItemVisually((EquippableInventoryItem)item, player.characterCollection.equipSlotFields[slot]);
            };
            player.characterCollection.OnSwappedItems += (fromCollection, fromSlot, toCollection, toSlot) =>
            {
                if (fromCollection is CharacterUI == false || toCollection is CharacterUI == false)
                    return;

                // Items are already swapped here...

                var fromItem = (EquippableInventoryItem)fromCollection[fromSlot].item;
                var fromCol = (CharacterUI)fromCollection;

                var toItem = (EquippableInventoryItem)toCollection[toSlot].item;
                var toCol = (CharacterUI)toCollection;


                UnEquipItemVisually(toItem, fromCol.equipSlotFields[fromSlot]);

                // Remove from old position
                if (fromItem != null)
                {
                    UnEquipItemVisually(fromItem, toCol.equipSlotFields[toSlot]);
                }

                EquipItemVisually(toItem, toCol.equipSlotFields[toSlot]);

                if (fromItem != null)
                    EquipItemVisually(fromItem, fromCol.equipSlotFields[fromSlot]);
            };
        }


        private void NotifyItemEquippedVisually(InventoryPlayerEquipTypeBinder binder, EquippableInventoryItem item)
        {
            if (OnEquippedVisually != null)
                OnEquippedVisually(binder, item);

            item.NotifyItemEquippedVisually(binder, player);
        }

        private void NotifyItemUnEquippedVisually(InventoryPlayerEquipTypeBinder binder, EquippableInventoryItem item)
        {
            if (OnUnEquippedVisually != null)
                OnUnEquippedVisually(binder, item);

            item.NotifyItemUnEquippedVisually(binder, player);
        }




        public virtual InventoryPlayerEquipTypeBinder FindEquipLocation(EquippableInventoryItem item, InventoryEquippableField equippableField)
        {
            return player.equipLocations.FirstOrDefault(o => o.equipTransform != null && o.associatedField == equippableField);
        }

        protected void SetLayerRecursive(GameObject obj, int layer)
        {
            obj.layer = layer;
            foreach (Transform t in obj.transform)
            {
                SetLayerRecursive(t.gameObject, layer);
            }
        }

        public virtual void EquipItemVisually(EquippableInventoryItem item, InventoryEquippableField equippableField)
        {
            if (item.equipVisually == false)
                return;

            var equipLocation = FindEquipLocation(item, equippableField);

            if (equipLocation != null)
            {
                var copy = GameObject.Instantiate<EquippableInventoryItem>(item);

                // Remove the default components, to prevent the user from looting an equipped item.
                Object.Destroy(copy.gameObject.GetComponent<ObjectTriggererItem>());
                Object.Destroy(copy.gameObject.GetComponent<InventoryItemBase>());

                var rigid = copy.gameObject.GetComponent<Rigidbody>();
                if (rigid != null)
                    rigid.isKinematic = true;

                var cols = copy.gameObject.GetComponentsInChildren<Collider>(true);
                foreach (var col in cols)
                    col.isTrigger = true;

                copy.gameObject.SetActive(true);
                SetLayerRecursive(copy.gameObject, InventorySettingsManager.instance.equipmentLayer);
                HandleEquipType(copy, equipLocation);
                return;
            }

            //Debug.LogWarning("No suitable visual equip location found", item.transform);
        }


        public virtual void HandleEquipType(EquippableInventoryItem copy, InventoryPlayerEquipTypeBinder binder)
        {
            switch (binder.equipHandlerType)
            {
                case EquipHandlerType.MakeChild:

                    copy.transform.SetParent(binder.equipTransform);
                    copy.transform.localPosition = copy.equipPosition;
                    copy.transform.localRotation = copy.equipRotation;

                    break;
                case EquipHandlerType.Replace:

                    copy.transform.SetParent(binder.equipTransform.parent); // Same level
                    copy.transform.SetSiblingIndex(binder.equipTransform.GetSiblingIndex());
                    binder.equipTransform.gameObject.SetActive(false); // Swap the item by disabling the original

                    copy.transform.localPosition = copy.equipPosition;
                    copy.transform.localRotation = copy.equipRotation;

                    break;
                default:
                    Debug.LogWarning("No visual equip handler found for item " + copy.name, copy);
                    return; // Return to avoid the notify event handler
            }

            binder.currentItem = copy.gameObject;
            NotifyItemEquippedVisually(binder, copy);
        }

        public void UnEquipItemVisually(EquippableInventoryItem item, InventoryEquippableField equippableField)
        {
            var equipLocation = FindEquipLocation(item, equippableField);
            UnEquipItemVisually(item, equipLocation);
        }

        public virtual void UnEquipItemVisually(EquippableInventoryItem item, InventoryPlayerEquipTypeBinder binder)
        {
            if (binder != null && binder.currentItem != null)
            {
                switch (binder.equipHandlerType)
                {
                    case EquipHandlerType.MakeChild:

                        UnityEngine.Object.Destroy(binder.currentItem);
                        break;
                    case EquipHandlerType.Replace:

                        binder.equipTransform.gameObject.SetActive(true); // Re-enable the original
                        break;
                    default:
                        Debug.LogWarning("No visual unequip handler found for item");
                        return; // Return to avoid the notify event handler
                }

                NotifyItemUnEquippedVisually(binder, item);
            }
        }


    }
}
