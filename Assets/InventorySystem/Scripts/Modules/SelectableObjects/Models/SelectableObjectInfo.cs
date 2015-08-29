using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem;
using UnityEngine;

namespace Devdog.InventorySystem
{
    [RequireComponent(typeof(ObjectTriggerer))]
    public partial class SelectableObjectInfo : MonoBehaviour, ISelectableObjectInfo
    {
        [SerializeField]
        private string _name;
        public new string name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Should the health be displayed?
        /// </summary>
        [SerializeField]
        private bool _useHealth = true;
        public bool useHealth
        {
            get { return _useHealth; }
            set { _useHealth = value; }
        }


        /// <summary>
        /// THe current health
        /// </summary>
        [SerializeField]
        private float _health = 100;
        public float health
        {
            get { return _health; }
            set { _health = value; }
        }

        [SerializeField]
        private float _maxHealth = 100;
        public float maxHealth
        {
            get { return _maxHealth; }
            set { _maxHealth = value; }
        }


        public float healthFactor
        {
            get { return health/maxHealth; }
        }

        public bool isDead
        {
            get { return health <= 0; }
        }



        public void Awake()
        {
            var triggerer = gameObject.GetComponent<ObjectTriggerer>();
            triggerer.OnTriggerUse += Select;
            triggerer.OnTriggerUnUse += (player) => UnSelect();
        }

        public void Start()
        {
            health = maxHealth;
        }

        public void ChangeHealth(float changeBy, bool fireEvents = true)
        {
            health += changeBy;

            // TODO: Add some events
        }

        public void Select(InventoryPlayer player)
        {
            if (InventoryManager.instance.selectableObjectInfo != null)
                InventoryManager.instance.selectableObjectInfo.currentSelectableObject = this;

        }

        public void UnSelect()
        {
            if (InventoryManager.instance.selectableObjectInfo != null)
                InventoryManager.instance.selectableObjectInfo.currentSelectableObject = null;

        }
    }
}
