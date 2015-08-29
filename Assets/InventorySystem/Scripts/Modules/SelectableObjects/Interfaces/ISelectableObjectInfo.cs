using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devdog.InventorySystem
{
    public interface ISelectableObjectInfo
    {
        string name { get; set; }
        float health { get; set; }
        float maxHealth { get; set; }
        
        float healthFactor { get; }
        bool useHealth { get; set; }


        void ChangeHealth(float changeBy, bool fireEvents = true);
        void Select(InventoryPlayer selectedByPlayer);
        void UnSelect();
    }
}
