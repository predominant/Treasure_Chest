using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.Models;
using UnityEditor;
using Devdog.InventorySystem.UI;

namespace Devdog.InventorySystem.Editors
{
    [CustomEditor(typeof(UIShowStat), true)]
    public class UIShowStatEditor : Editor
    {
        private SerializedProperty statCategory;
        private SerializedProperty statName;

        private SerializedProperty useCurrentPlayer;
        private SerializedProperty player;

        private SerializedProperty visualizer;
        
        private Dictionary<string, List<InventoryCharacterStat>> stats = new Dictionary<string, List<InventoryCharacterStat>>();

        public virtual void OnEnable()
        {

            statCategory = serializedObject.FindProperty("statCategory");
            statName = serializedObject.FindProperty("statName");

            useCurrentPlayer = serializedObject.FindProperty("useCurrentPlayer");
            player = serializedObject.FindProperty("player");

            visualizer = serializedObject.FindProperty("visualizer");



            UpdateStats();
        }

        private void UpdateStats()
        {
            stats.Clear();


            var p = Resources.FindObjectsOfTypeAll<InventoryPlayer>();
            if (p.Length > 0)
            {
                var itemDatabase = InventoryEditorUtility.GetItemDatabase(true, false);

                // Get the properties
                foreach (var property in itemDatabase.properties)
                {
                    if (property.useInStats == false)
                        continue;

                    if (stats.ContainsKey(property.category) == false)
                        stats.Add(property.category, new List<InventoryCharacterStat>());

                    // Check if it's already in the list
                    if (stats[property.category].FirstOrDefault(o => o.statName == property.name) != null)
                        continue;

                    stats[property.category].Add(new InventoryCharacterStat(p[0], property));
                }

                // Get the equip stats
                foreach (var equipStat in itemDatabase.equipStats)
                {
                    if (stats.ContainsKey(equipStat.category) == false)
                        stats.Add(equipStat.category, new List<InventoryCharacterStat>());

                    stats[equipStat.category].Add(new InventoryCharacterStat(p[0], equipStat.name, "{0}", 0.0f, 9999f, equipStat.show));
                }
            }
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

//            var t = (UIShowStat)target;
            bool categoryFaulty = false;
            bool statNameFaulty = false;

            if (stats.ContainsKey(statCategory.stringValue) == false || statCategory.stringValue == "")
            {
                categoryFaulty = true;
            }
            else
            {
                if (stats[statCategory.stringValue].FirstOrDefault(o => o.statName == statName.stringValue) == null || statName.stringValue == "")
                {
                    statNameFaulty = true;
                }
            }

            if (categoryFaulty)
                GUI.color = Color.yellow;

            EditorGUILayout.PropertyField(statCategory);
            GUI.color = Color.white;

            if (statNameFaulty)
                GUI.color = Color.yellow;

            EditorGUILayout.PropertyField(statName);
            GUI.color = Color.white;

            if (categoryFaulty)
                EditorGUILayout.HelpBox("Category does not exist in properties or equip stats, are you adding it through code (runtime)?", MessageType.Warning);

            if(statNameFaulty)
                EditorGUILayout.HelpBox("Stat name does not exist in properties or equip stats, are you adding it through code (runtime)?", MessageType.Warning);

            
            EditorGUILayout.PropertyField(useCurrentPlayer);
            if (useCurrentPlayer.boolValue == false)
                EditorGUILayout.PropertyField(player);


            EditorGUILayout.PropertyField(visualizer, true);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}