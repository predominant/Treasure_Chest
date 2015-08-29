#if PLY_GAME

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.Integration.plyGame.plyBlox;
using Devdog.InventorySystem.Models;
using plyGame;
using UnityEngine;

namespace Devdog.InventorySystem.Integration.plyGame
{
    [AddComponentMenu("InventorySystem/Integration/plyGame/ply Inventory player")]
    public partial class plyInventoryPlayer : InventoryPlayer
    {
        protected virtual List<ActorAttribute> plyAttributes
        {
            get
            {
                if (InventoryPlayerManager.instance.currentPlayer == null)
                    return new List<ActorAttribute>();

                var actor = InventoryPlayerManager.instance.currentPlayer.GetComponent<Actor>();
                if (actor == null || actor.actorClass == null)
                    return new List<ActorAttribute>();

                return actor.actorClass.attributes;
            }
        }

        private Actor _actor;
        public Actor actor
        {
            get
            {
                if (_actor == null)
                    _actor = gameObject.GetComponent<Actor>();

                return _actor;
            }
        }



        public override void Awake()
        {
            // Pass the data to plyBlox
            gameObject.AddComponent<InventoriesCollectionEventsProxy>();
            gameObject.AddComponent<CharactersCollectionEventsProxy>();
            gameObject.AddComponent<VendorCollectionEventsProxy>();
            gameObject.AddComponent<CraftingCollectionEventsProxy>();

            StartCoroutine(WaitAndCallBaseAwake());
        }

        private IEnumerator WaitAndCallBaseAwake()
        {
            yield return new WaitForEndOfFrame(); // plyGame runs before general Awake because it instantiates the player...
            yield return new WaitForEndOfFrame();

            base.Awake();


            var attributes = this.plyAttributes;
            foreach (var attr in attributes)
            {
                var a = actor.actorClass.attributes.FirstOrDefault(attribute => attribute.id.Value == attr.id.Value);
                if (a != null)
                {
                    a.RegisterChangeListener(AttributeChangeCallback);
                }
            }
        }

        private void AttributeChangeCallback(object sender, object[] args)
        {
            var attr = (ActorAttribute) sender;

            //float to = (float)args[0];
            //float from = (float)args[1];

            var invProAttr = ItemManager.instance.plyAttributes.FirstOrDefault(o => o.ID == attr.id);

            var player = InventoryPlayerManager.instance.currentPlayer;
            if (player != null && player.characterCollection != null && invProAttr != null)
            {
                var stat = player.characterCollection.GetStat(invProAttr.category, attr.def.screenName);
                if (stat != null)
                {
                    stat.ChangeCurrentValueRaw(attr.Value - attr.lastValue);
                }
            }
        }
    }
}

#endif