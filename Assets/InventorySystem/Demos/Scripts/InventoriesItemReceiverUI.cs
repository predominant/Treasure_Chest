using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem;

namespace Devdog.InventorySystem.Demo
{
    public class InventoriesItemReceiverUI : MonoBehaviour
    {
        private class ItemHolder
        {
            public InventoryItemBase item { get; set; }
            public uint stackSize;
        }

        public InventoryUIItemWrapper wrapperPrefab;
        public AnimationClip slideAnimation;

        public float offsetTimerSeconds = 0.2f;

        private InventoryPool<InventoryUIItemWrapper> pool { get; set; }
        private Queue<ItemHolder> queue { get; set; }
        private WaitForSeconds destroyTimer { get; set; }
        private WaitForSeconds offsetTimer { get; set; }


        public IEnumerator Start()
        {
            pool = new InventoryPool<InventoryUIItemWrapper>(wrapperPrefab, 8);
            queue = new Queue<ItemHolder>(8);
            destroyTimer = new WaitForSeconds(slideAnimation.length - 0.025f);
            offsetTimer = new WaitForSeconds(offsetTimerSeconds);

            foreach (var inv in InventoryManager.GetLootToCollections())
            {
                inv.OnAddedItem += (items, amount, cameFromCollection) =>
                {
                    if (cameFromCollection == false)
                    {
                        queue.Enqueue(new ItemHolder() { item = items.FirstOrDefault(), stackSize = amount});
                    }
                };
            }

            while (true)
            {
                if (queue.Count > 0)
                {
                    ShowItem(queue.Peek().item, queue.Peek().stackSize);
                    queue.Dequeue(); // Remove it
                }

                yield return offsetTimer;
            }
        }

        private void ShowItem(InventoryItemBase item, uint amount)
        {
            if (item != null)
            {
                var inst = pool.Get();

                inst.transform.SetParent(transform);
                inst.transform.localPosition = Vector3.zero;
                inst.transform.SetSiblingIndex(0);

                inst.icon.sprite = item.icon;
                inst.amountText.text = amount.ToString();
                
                // No repaint, manually handling items

                inst.GetComponent<Animator>().Play(slideAnimation.name);
                StartCoroutine(DestroyItem(inst));
            }
        }

        private IEnumerator DestroyItem(InventoryUIItemWrapper inst)
        {
            yield return destroyTimer;

            pool.Destroy(inst);            
        }
    }   
}