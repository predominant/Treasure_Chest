using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Devdog.InventorySystem.Models
{
    [Serializable]
    public partial class InventoryItemFilter
    {
        public enum RestrictionType
        {
            [AllowedFilters(FilterType.NotEqual | FilterType.Equal)]
            Type,

            [AllowedFilters(FilterType.NotEqual | FilterType.Equal)]
            Category,

            [AllowedFilters(FilterType.NotEqual | FilterType.Equal | FilterType.GreatherThan | FilterType.LessThan)]
            Rarity,

            [AllowedFilters(FilterType.NotEqual | FilterType.Equal | FilterType.GreatherThan | FilterType.LessThan)]
            Properties,

            [AllowedFilters(FilterType.NotEqual | FilterType.Equal)]
            Droppable,

            [AllowedFilters(FilterType.NotEqual | FilterType.Equal)]
            Sellable,

            [AllowedFilters(FilterType.NotEqual | FilterType.Equal)]
            Storable,

            [AllowedFilters(FilterType.NotEqual | FilterType.Equal | FilterType.GreatherThan | FilterType.LessThan)]
            Weight
        }

        [Flags]
        public enum FilterType
        {
            Equal,
            NotEqual,
            GreatherThan,
            LessThan
        }

        [AttributeUsage(AttributeTargets.Field)]
        public class AllowedFiltersAttribute : Attribute
        {

            public AllowedFiltersAttribute(FilterType types)
            {
                
            }
        }


        public RestrictionType restrictionType;
        public FilterType filterType;


        public string stringValue;
        public bool boolValue;
        public float floatValue;
        public int intValue;

        public Type typeValue
        {
            get
            {
                return Type.GetType(stringValue);
            }
        }


        public InventoryItemCategory categoryValue
        {
            get
            {
                return ItemManager.instance.itemCategories.First(o => o.ID == intValue);
            }
        }


        public InventoryItemRarity rarityValue
        {
            get
            {
                return ItemManager.instance.itemRarities.First(o => o.ID == intValue);
            }
        }


        public InventoryItemProperty propertyValue
        {
            get
            {
                return ItemManager.instance.properties.First(o => o.ID == intValue);
            }
        }


        public InventoryItemFilter()
            : this(RestrictionType.Type, FilterType.Equal)
        {

        }

        public InventoryItemFilter(RestrictionType restrictionType, FilterType filterType)
        {
            this.restrictionType = restrictionType;
            this.filterType = filterType;
        }


        public bool IsItemAbidingFilter(InventoryItemBase item)
        {
            switch (restrictionType)
            {
                case RestrictionType.Type:
                    return VerifyFilter(item, typeValue, filterType);
                case RestrictionType.Category:
                    return VerifyFilter(item.category.ID, intValue, filterType);
                case RestrictionType.Rarity:
                    return VerifyFilter(item.rarity.ID, intValue, filterType);
                case RestrictionType.Properties:
                    return VerifyFilter(item.properties, propertyValue, filterType);
                case RestrictionType.Droppable:
                    return VerifyFilterEqualNotEqual(item.isDroppable, boolValue, filterType);
                case RestrictionType.Sellable:
                    return VerifyFilterEqualNotEqual(item.isSellable, boolValue, filterType);
                case RestrictionType.Storable:
                    return VerifyFilterEqualNotEqual(item.isStorable, boolValue, filterType);
                case RestrictionType.Weight:
                    return VerifyFilter(item.weight, floatValue, filterType);
                default:
                    Debug.LogWarning("Type " + restrictionType + " not found");
                    break;
            }

            return false;
        }

        private bool VerifyFilter(InventoryItemProperty[] properties, InventoryItemProperty property, FilterType filterType)
        {
            switch (filterType)
            {
                case FilterType.Equal:
                    return properties.Any(o => o.ID == property.ID);
                case FilterType.NotEqual:
                    return properties.All(o => o.ID != property.ID);
                case FilterType.LessThan:
                    var prop = properties.FirstOrDefault(o => o.ID == property.ID);
                    if (prop == null)
                        return true; // None is also considered less than... Use Equal instead.

                    return prop.floatValue < floatValue;
                case FilterType.GreatherThan:
                    var prop2 = properties.FirstOrDefault(o => o.ID == property.ID);
                    if (prop2 == null)
                        return false;

                    return prop2.floatValue > floatValue;
                default:

                    break;
            }

            return false;
        }

        public bool VerifyFilterEqualNotEqual(object a, object b, FilterType filterType)
        {
            if (filterType == FilterType.Equal)
                return a.Equals(b);

            if (filterType == FilterType.NotEqual)
                return !a.Equals(b);

            //Debug.LogWarning("Only equal and not equal are supported for this check (" + a.ToString() + " given)");
            return false;
        }

        public bool VerifyFilter(InventoryItemBase item, System.Type type, FilterType filterType)
        {
            return VerifyFilterEqualNotEqual(item.GetType(), type, filterType);
        }
        public bool VerifyFilter(InventoryItemBase item, InventoryItemCategory category, FilterType filterType)
        {
            return VerifyFilterEqualNotEqual(item.category, category, filterType);
        }
        public bool VerifyFilter(int a, int b, FilterType filterType)
        {
            bool equality = VerifyFilterEqualNotEqual(a, b, filterType);
            if (equality)
                return equality;

            if (filterType == FilterType.GreatherThan)
                return a > b;

            if (filterType == FilterType.LessThan)
                return a < b;

            return false;
        }

        public bool VerifyFilter(float a, float b, FilterType filterType)
        {
            bool equality = VerifyFilterEqualNotEqual(a, b, filterType);
            if (equality)
                return equality;

            if (filterType == FilterType.GreatherThan)
                return a > b;

            if (filterType == FilterType.LessThan)
                return a < b;

            return false;
        }
    }
}
