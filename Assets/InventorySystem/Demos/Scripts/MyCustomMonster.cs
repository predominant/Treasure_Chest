using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Devdog.InventorySystem.Demo
{
    [RequireComponent(typeof(SelectableObjectInfo))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(ObjectTriggerer))]
    [RequireComponent(typeof(LootableObject))]
    public partial class MyCustomMonster : MonoBehaviour, IObjectTriggerUser
    {
        public bool droppedLoot { get; protected set; }

        public float walkSpeed = 4.0f;
        public float walkRadius = 10.0f;

        public bool useLootWindow = true;

        public GameObject corpseParticleEffectPrefab;

        [NonSerialized]
        private WaitForSeconds waitTime = new WaitForSeconds(4.0f);

        [NonSerialized]
        private Vector3 aimPosition;

        [NonSerialized]
        private NavMeshAgent agent;

        public LootableObject lootable { get; protected set; }

        public ISelectableObjectInfo selectableObjectInfo { get; protected set; }

        public ObjectTriggerer triggerer { get; protected set; }


        public bool isDead
        {
            get { return selectableObjectInfo.health <= 0; }
        }

        public void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.speed = walkSpeed;

            selectableObjectInfo = (ISelectableObjectInfo)GetComponent(typeof(ISelectableObjectInfo));

            droppedLoot = false;

            triggerer = GetComponent<ObjectTriggerer>();
            triggerer.window = InventoryManager.instance.loot.window;
            triggerer.handleWindowDirectly = false; // We're handling it.
            triggerer.enabled = false; // Only need it once the monster dies


            lootable = gameObject.GetComponent<LootableObject>();
            if (lootable == null)
            {
                Debug.LogWarning("No lootable object found on MyCustomMonster (manually added one at runtime to prevent errors)", gameObject);
                lootable = gameObject.AddComponent<LootableObject>();
            }
            
            StartCoroutine(_ChooseNewLocation());
        }

        private void OnRemovedItem(InventoryItemBase item, uint itemid, uint slot, uint amount)
        {
            lootable.items[slot] = null;
        }

        private void OnLootWindowHide()
        {
            if (useLootWindow)
            {
                InventoryManager.instance.loot.window.OnHide -= OnLootWindowHide; // No longer need this.
                UnSelected();
            }
        }

        protected void UnSelected()
        {
            InventoryManager.instance.loot.OnRemovedItem -= OnRemovedItem; // Un-register callback
        }

        private IEnumerator _ChooseNewLocation()
        {
            while (true)
            {
                ChooseNewLocation();

                yield return waitTime;
            }
        }

        public virtual void ChooseNewLocation()
        {
            if (isDead)
                return;

            aimPosition = UnityEngine.Random.insideUnitCircle * walkRadius;
            agent.SetDestination(transform.position + aimPosition);
        }

        public void OnMouseDown()
        {          
            var character = InventoryPlayerManager.instance.currentPlayer.characterCollection;
            int dmg = 0;

            if (character != null)
                dmg = 40 + (int)character.characterStats["Default"].FirstOrDefault(o => o.statName == "Strength").currentValue;
            else
                dmg = 40;

            Debug.Log("Damage dealt: " + dmg);
            selectableObjectInfo.ChangeHealth(-dmg);
            selectableObjectInfo.Select(InventoryPlayerManager.instance.currentPlayer);

            if (isDead)
                Die(); // Ah it died!
        }

        protected virtual void Die()
        {
            if (!isDead || droppedLoot)
                return; // not actually dead?

            Debug.Log("You killed it!");

            if (corpseParticleEffectPrefab != null)
            {
                var copy = GameObject.Instantiate<GameObject>(corpseParticleEffectPrefab);
                copy.transform.SetParent(transform);
                copy.transform.localPosition = Vector3.zero;
            }

            if (useLootWindow)
            {
                triggerer.enabled = true;
            }

            droppedLoot = true;

            agent.Stop();

            StartCoroutine(SinkIntoGround());

            DropLoot();
        }

        protected virtual IEnumerator SinkIntoGround()
        {
            yield return new WaitForSeconds(4.0f * ((useLootWindow) ? 2.0f : 1.0f));
            agent.enabled = false; // To allow for sinking
            float timer = 0.0f;

            while (timer < 3.0f)
            {
                yield return null;

                transform.Translate(0, -1.0f * Time.deltaTime, 0.0f);
                timer += Time.deltaTime;
            }

            // Remove if we still have this one selected.
            if (InventoryManager.instance.selectableObjectInfo.currentSelectableObject == selectableObjectInfo)
                selectableObjectInfo.UnSelect();

            Destroy(gameObject); // And clean up.
        }

        public virtual void DropLoot()
        {
            if (useLootWindow)
                return; // Nope, using a loot window

            foreach (var item in lootable.items)
            {
                InventoryItemBase dropItem = Instantiate<InventoryItemBase>(item);
                GameObject drop = dropItem.Drop(transform.position);

                if (drop != null)
                {
                    Rigidbody body = drop.GetComponent<Rigidbody>();
                    if (body != null)
                        body.velocity = new Vector3(UnityEngine.Random.Range(-1f, 1f), 3f, UnityEngine.Random.Range(-1f, 1f));

                }
            }
        }
    }
}
