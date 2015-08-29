using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Devdog.InventorySystem.Demo
{
    public partial class InventoryLookAtPlayer : MonoBehaviour
    {
        public Vector3 rotationOffset;

        public void Update()
        {
            if (InventoryPlayerManager.instance.currentPlayer == null)
                return;

            transform.LookAt(InventoryPlayerManager.instance.currentPlayer.transform, Vector3.up);
            transform.Rotate(rotationOffset);            
        }

    }
}
