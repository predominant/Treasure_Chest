using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.Editors;
using UnityEditor.Callbacks;


namespace Devdog.InventorySystem.Editors
{
    [CustomEditor(typeof(InventoryItemDatabase), true)]
    public class InventoryItemDatabaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {

            DrawDefaultInspector();

            if (GUILayout.Button("Open editor"))
            {
                InventoryEditorUtility.selectedDatabase = (InventoryItemDatabase) target;
                InventoryMainEditor.ShowWindow();
            }
        }
    }
}