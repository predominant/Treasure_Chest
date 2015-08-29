using UnityEngine;
using System;
using System.Collections.Generic;

namespace Devdog.InventorySystem.Models
{
    [System.Serializable]
    public partial class InventoryCraftingBlueprint
    {
        /// <summary>
        /// The unique ID of this object. Note that this is NOT the index inside the category.
        /// </summary>
        [HideInInspector]
        public int ID;

        /// <summary>
        /// The index inside the category.
        /// </summary>
        [HideInInspector]
        public int indexInCategory;

        ///// <summary>
        ///// The category, convenience property.
        ///// </summary>
        //public InventoryCraftingCategory category
        //{
        //    get
        //    {
        //        return ItemManager.instance.craftingCategories[categoryID];
        //    }
        //}

        /// <summary>
        /// Use the name of the item instead of a custom crafting name
        /// </summary>
        public bool useItemResultNameAndDescription = true;


        /// <summary>
        /// Name of the blueprint.
        /// </summary>
        public string name
        {
            get
            {
                if (useItemResultNameAndDescription)
                    return itemResult != null ? itemResult.name : string.Empty;

                return customName;
            }
        }

        /// <summary>
        /// Description of the blueprint.
        /// </summary>
        public string description
        {
            get
            {
                if (useItemResultNameAndDescription)
                    return itemResult != null ? itemResult.description : string.Empty;

                return customDescription;
            }
        }


        /// <summary>
        /// Crafting name, ignored if useItemResultNameAndDescription = true
        /// </summary>
        [SerializeField]
        public string customName;

        /// <summary>
        /// Crafting description, ignored if useItemResultNameAndDescription = true
        /// </summary>
        [SerializeField]
        public string customDescription;

        /// <summary>
        /// The items required for this blueprint.
        /// </summary>
        public InventoryCraftingBlueprintItemRow[] requiredItems = new InventoryCraftingBlueprintItemRow[0];

        ///// <summary>
        ///// If you want you can override the itemCategory of the crafted item, in-case you want to display it differently.
        ///// If false the ItemCategory will be used from the itemResult.category
        ///// </summary>
        //public bool overrideItemCategory = false;

        ///// <summary>
        ///// The index of the item category.
        ///// </summary>
        //protected int itemCategoryID;

        ///// <summary>
        ///// The category of the final item. Crafting a weapon, bow, cloth, etc?
        ///// Can be modified in Item editor -> category editor
        ///// </summary>
        //public InventoryItemCategory itemCategory
        //{
        //    get
        //    {
        //        return ItemManager.instance.itemCategories[itemCategoryID];
        //    }
        //}

        /// <summary>
        /// Can we craft this item already? disable if you want to unlock it through code.
        /// </summary>
        public bool playerLearnedBlueprint = true;
    
        /// <summary>
        /// The price to craft this item once.
        /// </summary>
        public InventoryCurrencyLookup craftingCost;

        /// <summary>
        /// The success factor 0.0f will always fail, while 1.0f will always succeed.
        /// </summary>
        [Range(0.0f, 1.0f)]
        public float successChanceFactor = 1.0f;

        /// <summary>
        /// Theh item gained after crafting.
        /// </summary>
        public InventoryItemBase itemResult;

        /// <summary>
        /// Amount of items you get when craft succeeded.
        /// </summary>
        public int itemResultCount = 1;

        /// <summary>
        /// How many seconds does it take to craft the item?
        /// </summary>
        public float craftingTimeDuration = 0.0f;

        /// <summary>
        /// Returns a value of 0.. to 1 where 0 is 0% done and 1 is 100% done.
        /// </summary>
        public float craftingProgress
        {
            get
            {
                Debug.LogWarning("Not written yet!");
                return 0.0f;
            }
        }

        /// <summary>
        /// How much faster does crafting become after an item is created?
        /// </summary>
        public float craftingTimeSpeedupFactor = 1.0f;

        /// <summary>
        /// The maximum speed the crafting can be spedup?
        /// </summary>
        public float craftingTimeSpeedupMax = 1.0f;

        ///// <summary>
        ///// Should the layout be checked when crafting the item?
        ///// </summary>
        //public bool useLayouts = false;

        ///// <summary>
        ///// Is the layout also valid when mirrored horizontally?
        ///// </summary>
        //public bool checkMirroredLayoutHorizontal;

        ///// <summary>
        ///// Is the layout also valid when mirrored vertically?
        ///// </summary>
        //public bool checkMirroredLayoutVertical;

        /// <summary>
        /// Games like minecraft have a layout. Items have to be placed in a certain order to unlock the craft.
        /// [] #1 first is for all layouts
        /// [] #2 is for horizontal row in layout
        /// [] #3 is for vertical column in layout
        /// </summary>
        public InventoryCraftingBlueprintLayout[] blueprintLayouts = new InventoryCraftingBlueprintLayout[0];
    

        public virtual bool IsLayoutValid(InventoryItemBase[] items)
        {
            Debug.LogWarning("Not written yet!");
            return true;
        }

        public override string ToString()
        {
            return name;
        }
    }
}