using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Devdog.InventorySystem.Dialogs
{
    public partial class ItemIntValDialog : IntValDialog
    {
        public InventoryUIItemWrapperBase wrapper;
        protected InventoryItemBase inventoryItem { get; set; }

        public override void ShowDialog(Transform caller, string title, string description, int minValue, int maxValue, IntValDialogCallback yesCallback, IntValDialogCallback noCallback)
        {
            base.ShowDialog(caller, title, description, minValue, maxValue, yesCallback, noCallback);

            if(wrapper != null && inventoryItem != null)
            {
                wrapper.item = inventoryItem;
                wrapper.Repaint();
            }
        }


        public override void ShowDialog(Transform caller, string title, string description, int minValue, int maxValue, InventoryItemBase item, IntValDialogCallback yesCallback, IntValDialogCallback noCallback)
        {
            // Don't call base class going directly to this.ShowDialog()
            inventoryItem = item;
            ShowDialog(caller, string.Format(title, item.name, item.description), string.Format(description, item.name, item.description), minValue, maxValue, yesCallback, noCallback);
        }
    }
}