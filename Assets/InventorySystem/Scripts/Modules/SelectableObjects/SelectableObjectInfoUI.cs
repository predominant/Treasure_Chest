using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Devdog.InventorySystem.UI;

namespace Devdog.InventorySystem
{
    [RequireComponent(typeof(UIWindow))]
    public partial class SelectableObjectInfoUI : MonoBehaviour
    {
        [SerializeField]
        private Text objectName;

        [Header("Health")]
        [SerializeField]
        private Slider objectHealthSlider;
        [SerializeField]
        private RectTransform healthContainer; // Parent of health
        [SerializeField]
        private Text objectHealth;
        [SerializeField]
        private Text maxObjectHealth;


        protected ISelectableObjectInfo _currentSelectableObject;
        public ISelectableObjectInfo currentSelectableObject
        {
            get { return _currentSelectableObject; }
            set
            {
                _currentSelectableObject = value;
                Repaint();
            }
        }

        public UIWindow window
        {
            get { return GetComponent<UIWindow>(); }
        }


        public void Repaint()
        {
            if (currentSelectableObject != null)
                window.Show();
            else
            {
                window.Hide();
                return;
            }

            if(objectName != null)
                objectName.text = currentSelectableObject.name;

            if (currentSelectableObject.useHealth)
            {
                healthContainer.gameObject.SetActive(true);

                if (objectHealthSlider != null)
                {
                    objectHealthSlider.value = currentSelectableObject.healthFactor;
                }

                if (objectHealth != null)
                {
                    objectHealth.text = currentSelectableObject.health.ToString();
                }

                if (maxObjectHealth != null)
                {
                    maxObjectHealth.text = currentSelectableObject.maxHealth.ToString();
                }
            }
            else
            {
                healthContainer.gameObject.SetActive(false);
            }
        }
    }
}