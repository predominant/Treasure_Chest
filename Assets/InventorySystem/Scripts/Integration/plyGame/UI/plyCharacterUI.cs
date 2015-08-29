#if PLY_GAME

using UnityEngine;
using System.Collections;

namespace Devdog.InventorySystem.Integration.plyGame
{
    [AddComponentMenu("InventorySystem/Integration/plyGame/Windows/plyCharacter")]
    public class plyCharacterUI : CharacterUI
    {
        public override void Start()
        {
            base.Start();

            StartCoroutine(WaitAndDoPlyGame());
        }

        protected override void SetDefaultDataProviders()
        {
            base.SetDefaultDataProviders();
        }

        protected virtual IEnumerator WaitAndDoPlyGame()
        {
            yield return new WaitForEndOfFrame(); // Needed for plyGame for some reason...
            yield return new WaitForEndOfFrame();


            // Add the plyGame specific data provider
            statsDataProviders.Add(new plyGameCharacterStatsDataProvider(this));

            PrepareCharacterStats();
            //UpdateCharacterStats();
        }
    }
}

#endif