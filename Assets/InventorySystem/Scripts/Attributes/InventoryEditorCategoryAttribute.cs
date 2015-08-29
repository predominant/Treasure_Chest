using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devdog.InventorySystem
{
    [AttributeUsage(AttributeTargets.Field)]
    public class InventoryEditorCategoryAttribute : Attribute
    {
        public string name { get; protected set; }


        public InventoryEditorCategoryAttribute(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
