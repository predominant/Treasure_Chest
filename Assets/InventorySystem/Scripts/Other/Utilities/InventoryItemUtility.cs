using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devdog.InventorySystem.Models;
using UnityEngine;

namespace Devdog.InventorySystem
{
    public class InventoryItemUtility
    {
        public enum SetItemPropertiesAction
        {
            Use,
            UnUse
        }


        public static void SetItemProperties(InventoryPlayer player, InventoryItemProperty[] properties, SetItemPropertiesAction action)
        {
            float multiplier = 1.0f;
            switch (action)
            {
                case SetItemPropertiesAction.Use:
                    multiplier = 1.0f;
                    break;
                case SetItemPropertiesAction.UnUse:
                    multiplier = -1f;
                    break;
                default:
                    Debug.LogWarning("Action " + action + " not found (Going with default use)");
                    break;
            }

            // Use the item's properties.
            if (player != null)
            {
                foreach (var property in properties)
                {
                    var stat = player.characterCollection.GetStat(property.category, property.name);
                    if (stat != null)
                    {
                        switch (property.actionEffect)
                        {
                            case InventoryItemProperty.ActionEffect.Add:

                                if (property.isFactor)
                                {
                                    //if (property.increaseMax)
                                    //    stat.ChangeFactorMax((property.floatValue - 1.0f) * multiplier, true);
                                    //else
                                        stat.ChangeFactorMax((property.floatValue - 1.0f) * multiplier, true);
                                }
                                else
                                {
                                    //if(property.increaseMax)
                                    //    stat.ChangeMaxValueRaw(property.floatValue * multiplier, true);
                                    //else
                                        stat.ChangeMaxValueRaw(property.floatValue * multiplier, true);
                                }

                                break;
                            case InventoryItemProperty.ActionEffect.Restore:

                                if (property.isFactor)
                                    stat.ChangeCurrentValueRaw((stat.currentValue * (property.floatValue - 1.0f)) * multiplier);
                                else
                                    stat.ChangeCurrentValueRaw(property.floatValue * multiplier);

                                break;
                            default:
                                Debug.LogWarning("Action effect" + property.actionEffect + " not found.");
                                break;
                        }

                        //player.characterCollection.NotifyStatChanged(stat); // Done by the stat itself.
                    }
                    else
                    {
                        Debug.LogWarning("Stat based on property: " + property.name + " not found on player.");
                    }
                }
            }
        }
    }
}
