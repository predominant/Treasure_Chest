namespace Devdog.InventorySystem.Editors
{
    using System;
    using System.Collections.Generic;

    public class InventoryEditorUitlity
    {
        public static int FindIndex<T>(IEnumerable<T> items, Func<T, bool> predicate)
        {
            int retVal = 0;
            foreach (var item in items)
            {
                if (predicate(item)) return retVal;
                retVal++;
            }
            return -1;
        }
    }
}