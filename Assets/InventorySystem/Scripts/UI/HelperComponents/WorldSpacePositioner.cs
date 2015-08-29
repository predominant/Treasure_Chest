using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Devdog.InventorySystem.UI
{
    [RequireComponent(typeof(ObjectTriggerer))]
    [AddComponentMenu("InventorySystem/UI Helpers/World Space Positioner")]
    public partial class WorldSpacePositioner : MonoBehaviour
    {

        public Vector3 windowPositionOffset;
        public Vector3 windowRotationOffset;

        private ObjectTriggerer triggerer { get; set; }

        public void Start()
        {
            triggerer = GetComponent<ObjectTriggerer>();
            triggerer.OnTriggerUse += PositionNow;
        }

        private void PositionNow(InventoryPlayer player)
        {
            if (triggerer.window != null)
            {
                triggerer.window.transform.position = transform.position;
                triggerer.window.transform.rotation = transform.rotation;
                triggerer.window.transform.Rotate(windowRotationOffset);

                //+(transform.forward * windowPositionOffset.z)
                triggerer.window.transform.Translate(windowPositionOffset);
            }
        }
    }
}
