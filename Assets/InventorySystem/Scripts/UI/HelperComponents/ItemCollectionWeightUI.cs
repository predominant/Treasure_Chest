using UnityEngine;
using System.Collections;
using Devdog.InventorySystem;

namespace Devdog.InventorySystem.UI
{
    [AddComponentMenu("InventorySystem/UI Helpers/Item Collection Weight")]
    [RequireComponent(typeof(UIWindow))]
    [RequireComponent(typeof(ItemCollectionBase))]
    public class ItemCollectionWeightUI : MonoBehaviour
    {
        [Tooltip("The UI element that displays the weight of this collection")]
        public UnityEngine.UI.Text weightText;

        /// <summary>
        /// The format on how to display the value
        /// </summary>
        [Tooltip("The string format, \n{0} = The actual weight.\n{1} = Max weight")]
        public string format = "{0}/{1}kg";

        /// <summary>
        /// How much to round the final value by?
        /// </summary>
        public int roundAmount = 1;

        protected UIWindow window { get; set; }
        protected ItemCollectionBase collection { get; set; }

        // Use this for initialization
        public virtual void Awake()
        {
            window = GetComponent<UIWindow>();
            collection = GetComponent<ItemCollectionBase>();

            collection.OnAddedItem += (items, amount, fromCollection) => CollectionChanged();
            collection.OnRemovedItem += (item, id, slot, amount) => CollectionChanged();
            collection.OnUsedItem += (item, id, slot, amount) => CollectionChanged();

            window.OnShow += Repaint;
        }

        protected virtual void CollectionChanged()
        {
            if (window.isVisible)
                Repaint();
        }

        protected virtual void Repaint()
        {
            if (weightText != null)
                weightText.text = string.Format(format, System.Math.Round(collection.GetWeight(), roundAmount), collection.restrictMaxWeight);
        }
    }
}