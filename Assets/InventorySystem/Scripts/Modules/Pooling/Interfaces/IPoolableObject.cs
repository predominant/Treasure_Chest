using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.InventorySystem
{
    public interface IPoolableObject
    {
        /// <summary>
        /// Reset the object so it can be reused again in it's original state.
        /// </summary>
        void Reset();
    }
}