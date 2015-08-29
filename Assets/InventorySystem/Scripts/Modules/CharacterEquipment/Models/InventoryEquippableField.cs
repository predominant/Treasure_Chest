using System;
using UnityEngine;
using System.Collections;

namespace Devdog.InventorySystem.Models
{
    using System.Linq;

    [RequireComponent(typeof(InventoryUIItemWrapper))]
    [AddComponentMenu("InventorySystem/UI Helpers/Equippable field")]
    public partial class InventoryEquippableField : MonoBehaviour
    {
        /// <summary>
        /// Index of this field
        /// </summary>
        [HideInInspector]
        public uint index
        {
            get
            {
                if (itemWrapper == null)
                    itemWrapper = GetComponent<InventoryUIItemWrapperBase>();

                return itemWrapper.index;
            }
        }

        [SerializeField]
        [HideInInspector]
        public int[] _equipTypes;
        public InventoryEquipType[] equipTypes
        {
            get
            {
                InventoryEquipType[] types = new InventoryEquipType[_equipTypes.Length];
                for (int i = 0; i < types.Length; i++)
                {
                    types[i] = ItemManager.instance.equipTypes.FirstOrDefault(o => o.ID == _equipTypes[i]);
                }

                return types;
            }
        }

        public InventoryUIItemWrapperBase itemWrapper { get; private set; }
    }
}