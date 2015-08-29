using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using Devdog.InventorySystem.Models;

namespace Devdog.InventorySystem.Editors
{
    [CustomEditor(typeof(EquippableInventoryItem), true)]
    public class EquippableInventoryItemEditor : InventoryItemBaseEditor
    {
        protected SerializedProperty equipType;
        protected SerializedProperty equipVisually;
        protected SerializedProperty equipPosition;
        protected SerializedProperty equipRotation;


        private static EquippableInventoryItem currentlyPositionedEquippable { get; set; }
        private static InventoryPlayerEquipTypeBinder currenctlyPositionedBinder { get; set; }
        private static InventoryPlayer currentInventoryPlayer { get; set; }

        public override void OnEnable()
        {
            base.OnEnable();
            equipType = serializedObject.FindProperty("_equipType");
            equipVisually = serializedObject.FindProperty("equipVisually");
            equipPosition = serializedObject.FindProperty("equipPosition");
            equipRotation = serializedObject.FindProperty("equipRotation");
        }

        protected override void PropertiesDrawElement(Rect rect, int index, bool isactive, bool isfocused, bool drawRestore, bool drawPercentage)
        {
            base.PropertiesDrawElement(rect, index, isactive, isfocused, false, drawPercentage);
        }

        protected override void OnCustomInspectorGUI(params CustomOverrideProperty[] extraOverride)
        {

            var l = new List<CustomOverrideProperty>(extraOverride);
            l.Add(new CustomOverrideProperty("_equipType", () =>
            {

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Equip type", GUILayout.Width(EditorGUIUtility.labelWidth - 5));

                EditorGUILayout.BeginVertical();
                EditorGUILayout.HelpBox("Edit types in the Equip editor", MessageType.Info);
                equipType.intValue = EditorGUILayout.Popup(equipType.intValue, InventoryEditorUtility.equipTypesStrings);                
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
            }));
            l.Add(new CustomOverrideProperty("equipVisually", () =>
            {
                EditorGUILayout.PropertyField(equipVisually);
            }));
            l.Add(new CustomOverrideProperty("equipPosition", () =>
            {
                GUI.enabled = equipVisually.boolValue;

                if (currentlyPositionedEquippable == null)
                {
                    if (GUILayout.Button("Position now"))
                    {
                        currentInventoryPlayer = GetPlayer();
                        if (currentInventoryPlayer != null)
                        {
                            var t = (EquippableInventoryItem)target;
                            var equipFields = GetEquippableSlots(t, currentInventoryPlayer);
                            currenctlyPositionedBinder = currentInventoryPlayer.equipHelper.FindEquipLocation(t, GetBestEquipSlot(t, equipFields, currentInventoryPlayer));
                            if (currenctlyPositionedBinder == null)
                            {
                                Debug.Log("No suitable equip location found");
                                return;
                            }

                            currentlyPositionedEquippable = (EquippableInventoryItem)PrefabUtility.InstantiatePrefab(t);
                            //currentInventoryPlayer.equipHelper.HandleEquipType(currentlyPositionedEquippable, currenctlyPositionedBinder);
                        }
                    }
                }
                else
                {
                    GUI.color = Color.green;
                    if (GUILayout.Button("Save state"))
                    {
                        var obj = (GameObject)PrefabUtility.GetPrefabParent(currentlyPositionedEquippable.gameObject);
                        var equippable = obj.GetComponent<EquippableInventoryItem>();

                        equippable.equipPosition = currentlyPositionedEquippable.transform.localPosition;
                        equippable.equipRotation = currentlyPositionedEquippable.transform.localRotation;

                        equippable.gameObject.SetActive(true);
                        EditorUtility.SetDirty(equippable);
                        GUI.changed = true;
                        AssetDatabase.SaveAssets(); // Save it

                        DestroyImmediate(currentlyPositionedEquippable.gameObject); // Get rid of positioning object

                        //SceneView.currentDrawingSceneView.SetSceneViewFiltering(false);
                    }
                    GUI.color = Color.white;
                }

                if (currentlyPositionedEquippable != null)
                    GUI.enabled = false;

                GUILayout.Space(5);
                EditorGUILayout.BeginHorizontal(InventoryEditorStyles.boxStyle);

                EditorGUILayout.PropertyField(equipPosition);
                GUILayout.Space(20);
                equipRotation.quaternionValue = ToQuat(EditorGUILayout.Vector4Field("Equip Rotation", ToVec4(equipRotation.quaternionValue)));

                EditorGUILayout.EndHorizontal();
                GUILayout.Space(10);

                GUI.enabled = true;
            }));
            l.Add(new CustomOverrideProperty("equipRotation", null));

            if (currentlyPositionedEquippable != null && currentlyPositionedEquippable.transform.parent == null)
            {
                currentInventoryPlayer.equipHelper.HandleEquipType(currentlyPositionedEquippable, currenctlyPositionedBinder);

                Selection.activeObject = currentlyPositionedEquippable.gameObject;
                if (SceneView.currentDrawingSceneView != null)
                    SceneView.currentDrawingSceneView.LookAt(currentlyPositionedEquippable.transform.position);
            }

            base.OnCustomInspectorGUI(l.ToArray());
        }

        private InventoryPlayer GetPlayer()
        {
            var playersInScene = FindObjectsOfType<InventoryPlayer>();
            if (playersInScene.Length == 0)
            {
                Debug.LogWarning("No players found in scene to position model on");
                return null;
            }

            if (playersInScene.Length > 1)
            {
                Debug.LogWarning("Currently only supporting 1 player at a time (returning first player)");
            }

            playersInScene[0].equipHelper = new InventoryPlayerEquipHelper(playersInScene[0]); // Needed for equipment, not created in editor...
            return playersInScene[0];
        }


        public InventoryEquippableField GetBestEquipSlot(EquippableInventoryItem item, InventoryEquippableField[] fields, InventoryPlayer player)
        {
            if (fields.Length > 0)
                return fields[0];

            return null;
        }

        public InventoryEquippableField[] GetEquippableSlots(EquippableInventoryItem item, InventoryPlayer player)
        {
            if (player == null || player.characterCollection == null || player.characterCollection.container == null)
                return new InventoryEquippableField[0];

            var equipSlots = new List<InventoryEquippableField>(4);
            foreach (var field in player.characterCollection.container.GetComponentsInChildren<InventoryEquippableField>(true))
            {
                foreach (var type in field._equipTypes)
                {
                    if (item._equipType == type)
                    {
                        equipSlots.Add(field);
                    }
                }
            }

            return equipSlots.ToArray();
        }

        private Vector4 ToVec4(Quaternion quat)
        {
            return new Vector4(quat.x, quat.y, quat.z, quat.w);
        }

        private Quaternion ToQuat(Vector4 vec)
        {
            return new Quaternion(vec.x, vec.y, vec.z, vec.w);
        }
    }
}