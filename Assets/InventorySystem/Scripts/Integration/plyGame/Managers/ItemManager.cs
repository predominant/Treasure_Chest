#if PLY_GAME

using System;
using System.Collections.Generic;
using Devdog.InventorySystem.Integration.plyGame;
using UnityEngine;

namespace Devdog.InventorySystem
{
    public partial class ItemManager
    {
        public plyGameAttributeDatabaseModel[] plyAttributes
        {
            get { return itemDatabase.plyAttributes; }
            set { itemDatabase.plyAttributes = value; }
        }
    }
}


#endif