using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Devdog.InventorySystem
{
    [AddComponentMenu("InventorySystem/UI Wrappers/UI Wrapper loot")]
    public partial class InventoryUIItemWrapperLoot : InventoryUIItemWrapper
    {

        public static bool hideWhenEmpty = true;


        public override void Update()
        {
            //base.Update();
        }


        #region Button handler UI events

   
        public override void OnPointerUp(PointerEventData eventData)
        {
            PickupItem();
        }

        public virtual void PickupItem()
        {
            Selectable below = null;
            Selectable above = null;

            // select element below or above 
            var btn = gameObject.GetComponentInChildren<Button>();
            if (btn != null)
            {
                below = btn.FindSelectableOnDown();
                above = btn.FindSelectableOnUp();
            }


            bool added = item.PickupItem();
            if (added)
            {
                var i = item;
                itemCollection.SetItem(index, null); // Remove from loot collection
                itemCollection.NotifyItemRemoved(i, i.ID, index, i.currentStackSize);

                if (below != null)
                    below.Select();
                else if (above != null)
                    above.Select();

                Repaint();
            }
        }

        #endregion


        public override void Repaint()
        {
            if (item == null)
            {
                gameObject.SetActive(false);
                return;
            }
            else
                gameObject.SetActive(true);


            base.Repaint();

            if (item != null)
            {
                if (hideWhenEmpty)
                    gameObject.SetActive(true);

                //itemName.text = item.name;
                if(item != null && item.rarity != null)
                    itemName.color = item.rarity.color;
            }
            else
            {
                if (hideWhenEmpty)
                    gameObject.SetActive(false);

                //itemName.text = string.Empty;
            }

        }

        public override void RepaintCooldown()
        {
            //base.RepaintCooldown();
        }
    }
}