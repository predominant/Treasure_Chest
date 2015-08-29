using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem
{

    [System.Serializable]
    public class InventoryItemFilters
    {
        public enum FiltersMatchType
        {
            MatchAll,
            MatchAny
        }

        public InventoryItemFilter[] filters = new InventoryItemFilter[0];
        public FiltersMatchType matchType = FiltersMatchType.MatchAll;




        public bool IsItemAbidingFilters(InventoryItemBase item)
        {
            switch (matchType)
            {
                case FiltersMatchType.MatchAll:

                    if (filters.Any(filter => filter.IsItemAbidingFilter(item) == false))
                    {
                        return false;
                    }

                    return true;

                case FiltersMatchType.MatchAny:

                    if (filters.Any(filter => filter.IsItemAbidingFilter(item)))
                    {
                        return true;
                    }

                    return false;

                default:
                    Debug.LogWarning("Type " + matchType + " not found");
                    break;
            }

            return false;
        }
    }
}
