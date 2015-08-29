using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem;
using UnityEngine;


namespace Devdog.InventorySystem.UI
{
    /// <summary>
    /// Used to visualize the triggering process (pickup, using, etc) for in-range items.
    /// </summary>
    [RequireComponent(typeof(UIWindow))]
    [AddComponentMenu("InventorySystem/UI Helpers/Object trigger Range UI")]
    public partial class ObjectTriggererRangeUI : MonoBehaviour
    {
        /// <summary>
        /// Bind an KeyCode to a sprite.
        /// </summary>
        [System.Serializable]
        public class KeyIconBinding
        {
            public KeyCode keyCode;
            public Sprite sprite;
        }


        public UnityEngine.UI.Image imageIcon;
        public UnityEngine.UI.Text shortcutText;
        public bool moveToTriggererLocation = true;

        public KeyIconBinding[] keyIconBindings = new KeyIconBinding[0];



        public UIWindow window { get; protected set; }

        public virtual void Awake()
        {
            window = GetComponent<UIWindow>();
        }

        public virtual void Start()
        {
            InventoryPlayerManager.instance.OnPlayerChanged += OnPlayerChanged;
            OnPlayerChanged(null, InventoryPlayerManager.instance.currentPlayer);
        }

        public void Update()
        {
            if(moveToTriggererLocation && InventoryPlayerManager.instance.currentPlayer != null)
                UpdatePosition(InventoryPlayerManager.instance.currentPlayer.rangeHelper.bestTriggerer);
        }
        
        private void OnPlayerChanged(InventoryPlayer oldPlayer, InventoryPlayer newPlayer)
        {
            newPlayer.rangeHelper.OnChangedBestTriggerer += BestTriggererChanged;

            // First time
            Repaint(newPlayer.rangeHelper.bestTriggerer);
        }

        private void BestTriggererChanged(ObjectTriggererBase old, ObjectTriggererBase newBest)
        {
            if (newBest != null)
            {
                window.Show();
                Repaint(newBest);
            }
            else
            {
                window.Hide();
            }
        }

        protected virtual Sprite GetIcon(KeyCode keyCode)
        {
            var binding = keyIconBindings.FirstOrDefault(o => o.keyCode == keyCode);
            if (binding == null)
            {
                Debug.LogWarning("Couldn't find binding for keycode : " + keyCode, transform);
                return null;
            }

            return binding.sprite;
        }
        
        protected virtual void UpdatePosition(ObjectTriggererBase triggerer)
        {
            if(triggerer != null)
                transform.position = Camera.main.WorldToScreenPoint(triggerer.transform.position);
        }

        public virtual void Repaint(ObjectTriggererBase triggerer)
        {
            if(window.isVisible == false)
                window.Show();
            
            Sprite icon = null;
            if (triggerer != null)
                icon = GetIcon(triggerer.triggerKeyCode);

            if (imageIcon != null && imageIcon.sprite != icon)
                imageIcon.sprite = icon;

            if (shortcutText != null)
            {
                if (triggerer != null)
                {
                    if (shortcutText.text != triggerer.triggerKeyCode.ToString())
                    {
                        shortcutText.text = triggerer.triggerKeyCode.ToString();
                    }
                    else
                        shortcutText.text = "";
                }
            }
        }
    }    
}
