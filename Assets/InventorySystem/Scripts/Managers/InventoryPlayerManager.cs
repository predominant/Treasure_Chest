using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem;
using UnityEngine;

namespace Devdog.InventorySystem
{
    [RequireComponent(typeof(InventorySettingsManager))]
    [RequireComponent(typeof(InventoryManager))]
    [RequireComponent(typeof(InventoryTriggererManager))]
    [RequireComponent(typeof(ItemManager))]
    [RequireComponent(typeof(InventoryInputManager))]
    [AddComponentMenu("InventorySystem/Managers/Inventory Player manager")]
    public class InventoryPlayerManager : MonoBehaviour
    {
        #region Delegates and Events

        public delegate void PlayerChanged(InventoryPlayer oldPlayer, InventoryPlayer newPlayer);
        public event PlayerChanged OnPlayerChanged;

        #endregion
        

        [NonSerialized]
        private List<InventoryPlayer> _players = new List<InventoryPlayer>(1);

        [NonSerialized]
        private InventoryPlayer _currentPlayer;

        public InventoryPlayer currentPlayer
        {
            get
            {
                return _currentPlayer;
            }
            set
            {
                InventoryPlayer oldPlayer = _currentPlayer;
                if (_currentPlayer != null)
                {
                    // Remove old
                    foreach (var inv in _currentPlayer.inventoryCollections)
                        InventoryManager.RemoveInventoryCollection(inv);

                    if (_currentPlayer.characterCollection != null)
                        InventoryManager.RemoveEquipCollection(_currentPlayer.characterCollection);
                }

                _currentPlayer = value;
                if (_currentPlayer != null)
                {
                    foreach (var inv in _currentPlayer.inventoryCollections)
                    {
                        var i = inv as ICollectionPriority;
                        if (i != null)
                            InventoryManager.AddInventoryCollection(inv, i.collectionPriority);
                        else
                            InventoryManager.AddInventoryCollection(inv, 50);
                    }

                    if (_currentPlayer.characterCollection != null)
                    {
                        var eq = _currentPlayer.characterCollection as ICollectionPriority;
                        if (eq != null)
                            InventoryManager.AddEquipCollection(_currentPlayer.characterCollection, eq.collectionPriority);
                        else
                            InventoryManager.AddEquipCollection(_currentPlayer.characterCollection, 50);
                    }

                    //instance.character = _currentPlayer.associatedCharacterUI;
                    //instance.skillbar = _currentPlayer.skillbar;
                }

                if (OnPlayerChanged != null)
                    OnPlayerChanged(oldPlayer, _currentPlayer);
            }
        }

        private static InventoryPlayerManager _instance;
        public static InventoryPlayerManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<InventoryPlayerManager>();
                }

                return _instance;
            }
        }

        public void Awake()
        {
            _instance = this;
        }


        public static void AddPlayer(InventoryPlayer player)
        {
            if (instance.currentPlayer == null)
                instance.currentPlayer = player;

            instance._players.Add(player);
        }

        public static void RemovePlayer(InventoryPlayer player)
        {
            instance._players.Remove(player);
        }
    }
}
