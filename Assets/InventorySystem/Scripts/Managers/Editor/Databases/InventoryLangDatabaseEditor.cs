using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Devdog.InventorySystem.Editors
{
    [CustomEditor(typeof(InventoryLangDatabase), true)]
    public class InventoryLangDatabaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {

            DrawDefaultInspector();

            if (GUILayout.Button("Open main editor"))
            {
                InventoryEditorUtility.selectedLangDatabase = (InventoryLangDatabase)target;
                InventoryMainEditor.ShowWindow();
            }
        }
    }
}