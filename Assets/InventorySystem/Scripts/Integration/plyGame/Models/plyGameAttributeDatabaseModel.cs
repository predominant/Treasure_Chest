#if PLY_GAME

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using plyCommon;
using UnityEngine;

namespace Devdog.InventorySystem.Integration.plyGame
{
    [System.Serializable]
    public partial class plyGameAttributeDatabaseModel
    {
        [SerializeField]
        public UniqueID ID;
        public bool show;
        public string category;

        public plyGameAttributeDatabaseModel(UniqueID id, string category, bool show)
        {
            this.ID = id;
            this.category = category;
            this.show = show;
        }
    }
}

#endif