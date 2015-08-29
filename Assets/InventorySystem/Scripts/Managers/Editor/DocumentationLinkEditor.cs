using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;


namespace Devdog.InventorySystem.Editors
{
    public class DocumentationLinkEditor : EditorWindow
    {


        [MenuItem("Tools/Inventory Pro/Documentation", false, 99)] // Always at bottom
        public static void ShowWindow()
        {
            Application.OpenURL("http://devdog.nl/unity/documentation");
        }
    }
}