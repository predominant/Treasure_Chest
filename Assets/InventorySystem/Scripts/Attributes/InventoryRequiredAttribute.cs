using System;
using UnityEngine;

namespace Devdog.InventorySystem
{
    [AttributeUsage(AttributeTargets.Field)]
    public class InventoryRequiredAttribute : PropertyAttribute
    {

        public InventoryRequiredAttribute()
        {
            
        }
    }
}
