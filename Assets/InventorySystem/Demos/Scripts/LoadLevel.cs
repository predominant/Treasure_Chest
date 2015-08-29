using UnityEngine;
using System.Collections;

namespace Devdog.InventorySystem.Demo
{
    public class LoadLevel : MonoBehaviour
    {
        public void LoadALevel(string level)
        {
            Application.LoadLevel(level);
        }
    }
}