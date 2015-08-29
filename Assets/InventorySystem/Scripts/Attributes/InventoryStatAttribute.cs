using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.InventorySystem.Models
{
    public partial class InventoryStatAttribute : Attribute
    {
        public string name;

        public InventoryStatAttribute()
        {

        }
        public InventoryStatAttribute(string name)
        {
            this.name = name;
        }
    }
}