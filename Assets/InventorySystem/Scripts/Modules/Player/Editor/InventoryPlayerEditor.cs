using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem.Editors
{
    [CustomEditor(typeof(InventoryPlayer), true)]
    [CanEditMultipleObjects()]
    public class InventoryPlayerEditor : InventoryEditorBase
    {
        private static InventoryItemDatabase itemDatabase;
        private SerializedProperty equipLocations;

        private SerializedProperty dynamicallyFindUIElements;
        private SerializedProperty characterUIPath;
        private SerializedProperty inventoryPaths;
        private SerializedProperty skillbarPath;


        private SerializedProperty characterCollection;
        private SerializedProperty inventoryCollections;
        private SerializedProperty skillbarCollection;


        private UnityEditorInternal.ReorderableList list;
        private InventoryPlayer tar;

        private bool IsOutOfSync()
        {
            var equipSlotFields = tar.characterCollection.container.GetComponentsInChildren<InventoryEquippableField>(true);
            return equipSlotFields.Length != equipLocations.arraySize;
        }

        private void ReSync()
        {
            var newList = new List<InventoryPlayerEquipTypeBinder>();
            var equipSlotFields = tar.characterCollection.container.GetComponentsInChildren<InventoryEquippableField>(true);

            for (int i = 0; i < equipSlotFields.Length; i++)
            {
                InventoryPlayerEquipTypeBinder toAdd = new InventoryPlayerEquipTypeBinder(equipSlotFields[i], null, InventoryPlayerEquipHelper.EquipHandlerType.MakeChild);

                // Find in old data
                var found = tar.equipLocations.FirstOrDefault(o => o.associatedField == equipSlotFields[i]);
                if (found != null)
                {
                    toAdd = found;
                    //toAdd = new InventoryPlayerEquipTypeBinder(t.equipLocations[i].associatedField, t.equipLocations[i].equipTransform);
                }

                newList.Add(toAdd); 
            }


            tar.equipLocations = newList.ToArray();
            GUI.changed = true; // To save

            list.list = tar.equipLocations; // Update list
            Repaint();
        }

        public override void OnEnable()
        {
            base.OnEnable();

            tar = (InventoryPlayer)target;
            equipLocations = serializedObject.FindProperty("equipLocations");

            dynamicallyFindUIElements = serializedObject.FindProperty("dynamicallyFindUIElements");
            characterUIPath = serializedObject.FindProperty("characterUIPath");
            inventoryPaths = serializedObject.FindProperty("inventoryPaths");
            skillbarPath = serializedObject.FindProperty("skillbarPath");

            characterCollection = serializedObject.FindProperty("characterCollection");
            inventoryCollections = serializedObject.FindProperty("inventoryCollections");
            skillbarCollection = serializedObject.FindProperty("skillbarCollection");


            itemDatabase = InventoryEditorUtility.GetItemDatabase(true, false);
            if (itemDatabase == null)
                Debug.LogError("No item database found in scene, cannot edit item.");


            list = new UnityEditorInternal.ReorderableList(serializedObject, equipLocations, false, true, false, false);
            list.elementHeight = 66;
            list.drawHeaderCallback += rect =>
            {
                EditorGUI.LabelField(rect, "Equipment binding");
            };
            list.drawElementCallback += (rect, index, active, focused) =>
            {
                rect.height = 16;
                rect.y += 2;

                var r3 = rect;
                r3.width = 25;

                rect.x += 30;
                rect.width -= 30;
                var r = rect;
                var r2 = rect;
                r2.width -= EditorGUIUtility.labelWidth;
                r2.x += EditorGUIUtility.labelWidth;

                r3.x += 8;
                r3.y += 24;
                var t = equipLocations.GetArrayElementAtIndex(index).FindPropertyRelative("equipTransform").objectReferenceValue as Transform;
                if (t != null && t.IsChildOf(((InventoryPlayer) target).transform))
                {
                    EditorGUI.Toggle(r3, GUIContent.none, true, "VisibilityToggle");
                }
                else
                {
                    EditorGUI.Toggle(r3, GUIContent.none, false, "VisibilityToggle");                    
                }
                


                GUI.enabled = false;
                EditorGUI.ObjectField(r, equipLocations.GetArrayElementAtIndex(index).FindPropertyRelative("associatedField"), new GUIContent("Associated field"));
                GUI.enabled = true;

                rect.y += 18;
                EditorGUI.PropertyField(rect, equipLocations.GetArrayElementAtIndex(index).FindPropertyRelative("equipTransform"));

                if (t != null && t.IsChildOf(((InventoryPlayer)target).transform) == false)
                {
                    rect.y += 18;
                    EditorGUI.HelpBox(rect, "Equip transform has to be a child of this character.", MessageType.Error);
                }

                rect.y += 18;
                EditorGUI.PropertyField(rect, equipLocations.GetArrayElementAtIndex(index).FindPropertyRelative("equipHandlerType"));
            };


            if (tar.characterCollection != null && tar.characterCollection.container != null)
            {
                if (IsOutOfSync())
                {
                    ReSync();
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            OnCustomInspectorGUI();

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();

            if (tar.gameObject.GetComponentsInChildren<InventoryPlayerRangeHelper>(true).Length == 0)
            {
                EditorGUILayout.HelpBox("No range helper found on player", MessageType.Warning);

                GUI.color = Color.green;
                if (GUILayout.Button("Add helper"))
                {
                    tar.AddRangeHelper();
                }
                GUI.color = Color.white;
            }

            if (tar.characterCollection == null)
            {
                EditorGUILayout.HelpBox("This player isn't attached to a CharacterUI, disabling visual equipment / mesh binding.", MessageType.Warning);
                return;
            }

            if (tar.characterCollection.container == null)
            {
                EditorGUILayout.HelpBox("This player is attached to a CharacterUI, but the CharacterUI's container is null.\nEquip locations cannot be scanned..", MessageType.Warning);
                return;
            }


            //if (GUILayout.Button("This player is associated with " + tar.characterCollection.name + "(" + tar.characterCollection.collectionName + ")", InventoryEditorStyles.boxStyle))
            //{
            //    Selection.activeObject = tar.characterCollection.gameObject;
            //}

            if (GUILayout.Button("Force rescan"))
                ReSync();                


            list.DoLayoutList();


            serializedObject.ApplyModifiedProperties();
        }


        protected override void OnCustomInspectorGUI(params CustomOverrideProperty[] extraOverride)
        {
            base.OnCustomInspectorGUI(extraOverride);

            if (itemDatabase == null)
                return;


            EditorGUILayout.PropertyField(dynamicallyFindUIElements);
            if (dynamicallyFindUIElements.boolValue)
            {
                EditorGUILayout.PropertyField(characterUIPath, true);
                EditorGUILayout.PropertyField(inventoryPaths, true);
                EditorGUILayout.PropertyField(skillbarPath, true);
            }
            
            if(dynamicallyFindUIElements.boolValue == false || EditorApplication.isPlaying)
            {
                GUI.enabled = !EditorApplication.isPlaying;

                EditorGUILayout.PropertyField(characterCollection, true);
                EditorGUILayout.PropertyField(inventoryCollections, true);
                EditorGUILayout.PropertyField(skillbarCollection, true);

                GUI.enabled = true;
            }


            // Draws remaining items
            DrawPropertiesExcluding(serializedObject, new []
            {
                "m_Script",
                "equipLocations",
                "dynamicallyFindUIElements",
                "characterUIPath",
                "inventoryPaths",
                "skillbarPath",
                "characterCollection",
                "inventoryCollections",
                "skillbarCollection"
            });
        }
    }
}