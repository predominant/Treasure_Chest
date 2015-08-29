using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace Devdog.InventorySystem.Editors
{
    [CustomEditor(typeof(ItemCollectionBase), true)]
    [CanEditMultipleObjects()]
    public class ItemCollectionBaseEditor : InventoryEditorBase
    {
        private ItemCollectionBase item;
        private SerializedObject serializer;

        private SerializedProperty collectionName;
        private SerializedProperty restrictByWeight;
        private SerializedProperty restrictMaxWeight;
        private SerializedProperty itemButtonPrefab;

        private SerializedProperty items;
        private SerializedProperty filters;

        private SerializedProperty useReferences;
        private SerializedProperty canContainCurrencies;
        private SerializedProperty canUseItemsFromReference;
        private SerializedProperty canDropFromCollection;
        private SerializedProperty canUseFromCollection;
        private SerializedProperty canDragInCollection;
        private SerializedProperty canPutItemsInCollection;
        private SerializedProperty canStackItemsInCollection;
        private SerializedProperty canUnstackItemsInCollection;
        private SerializedProperty manuallyDefineCollection;
        private SerializedProperty container;

        private static ItemManager itemManager;

        // Script selector
        private UnityEditorInternal.ReorderableList manualItemsList;

        public override void OnEnable()
        {
            base.OnEnable();

            item = (ItemCollectionBase)target;
            //serializer = new SerializedObject(target);
            serializer = serializedObject;
            
            collectionName = serializer.FindProperty("collectionName");
            restrictByWeight = serializer.FindProperty("restrictByWeight");
            restrictMaxWeight = serializer.FindProperty("restrictMaxWeight");
            itemButtonPrefab = serializer.FindProperty("itemButtonPrefab");

            items = serializer.FindProperty("_items");
            filters = serializer.FindProperty("filters");
            useReferences = serializer.FindProperty("useReferences");
            canContainCurrencies = serializer.FindProperty("canContainCurrencies");
            canDropFromCollection = serializer.FindProperty("canDropFromCollection");
            canUseItemsFromReference = serializer.FindProperty("canUseItemsFromReference");
            canUseFromCollection = serializer.FindProperty("canUseFromCollection");
            canDragInCollection = serializer.FindProperty("canDragInCollection");
            canPutItemsInCollection = serializer.FindProperty("canPutItemsInCollection");
            canStackItemsInCollection = serializer.FindProperty("canStackItemsInCollection");
            canUnstackItemsInCollection = serializer.FindProperty("canUnstackItemsInCollection");

            manuallyDefineCollection = serializer.FindProperty("manuallyDefineCollection");
            container = serializer.FindProperty("container");

            itemManager = Editor.FindObjectOfType<ItemManager>();
            if (itemManager == null)
                Debug.LogError("No item manager found in scene, cannot edit item.");


            manualItemsList = new UnityEditorInternal.ReorderableList(serializer, items, true, true, true, true);
            manualItemsList.drawHeaderCallback += rect =>
            {
                EditorGUI.LabelField(rect, "Select items");
            };
            manualItemsList.drawElementCallback += (rect, index, active, focused) =>
            {
                rect.height = 16;
                rect.y += 2;

                EditorGUI.PropertyField(rect, items.GetArrayElementAtIndex(index));
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            OnCustomInspectorGUI();
            serializedObject.ApplyModifiedProperties();
        }


        protected override void OnCustomInspectorGUI(params CustomOverrideProperty[] extraOverride)
        {
            base.OnCustomInspectorGUI(extraOverride);
            
            GUILayout.Label("General settings", InventoryEditorStyles.titleStyle);
            EditorGUILayout.BeginVertical(InventoryEditorStyles.boxStyle);

            //GUILayout.Label("General", InventoryEditorStyles.titleStyle);
            EditorGUILayout.PropertyField(collectionName);
            EditorGUILayout.PropertyField(useReferences);

            GUILayout.Label("UI", InventoryEditorStyles.titleStyle);
            EditorGUILayout.PropertyField(itemButtonPrefab);
            EditorGUILayout.PropertyField(container);
            //EditorGUILayout.PropertyField(onlyAllowTypes);


            #region Manually define collection

            EditorGUILayout.PropertyField(manuallyDefineCollection);
            if (manuallyDefineCollection.boolValue)
            {
                manualItemsList.DoLayoutList();

                if (GUILayout.Button("Scan for wrappers"))
                {
                    var wrappers = item.gameObject.GetComponentsInChildren<InventoryUIItemWrapperBase>();
                    manualItemsList.list = wrappers;
                    item.items = wrappers;
                    GUI.changed = true;
                }
            }

            #endregion

            EditorGUILayout.EndVertical();


            // Draws remaining items
            GUILayout.Label("Collection specific", InventoryEditorStyles.titleStyle);
            EditorGUILayout.BeginVertical(InventoryEditorStyles.boxStyle);

            var doNotDrawList = new List<string>()
            {
                "m_Script",
                "collectionName",
                "restrictByWeight",
                "restrictMaxWeight",
                "itemButtonPrefab",
                "_items",
                "useReferences",
                "canContainCurrencies",
                "canDropFromCollection",
                "canUseFromCollection",
                "canDragInCollection",
                "canPutItemsInCollection",
                "canStackItemsInCollection",
                "canUnstackItemsInCollection",
                "manuallyDefineCollection",
                "container",
                "onlyAllowTypes",
                "canUseItemsFromReference",
                "filters"
            };

            foreach (var extra in extraOverride)
            {
                extra.action();
                doNotDrawList.Add(extra.serializedName);
            }

            DrawPropertiesExcluding(serializer, doNotDrawList.ToArray());
            EditorGUILayout.EndVertical();


            GUILayout.Label("Restrictions", InventoryEditorStyles.titleStyle);
            EditorGUILayout.BeginVertical(InventoryEditorStyles.boxStyle);
            EditorGUILayout.PropertyField(filters);

            GUILayout.Label("Other", InventoryEditorStyles.titleStyle);
            EditorGUILayout.PropertyField(restrictByWeight);
            if (restrictByWeight.boolValue)
                EditorGUILayout.PropertyField(restrictMaxWeight);

            EditorGUILayout.PropertyField(canContainCurrencies);
            EditorGUILayout.PropertyField(canDropFromCollection);
            EditorGUILayout.PropertyField(canUseFromCollection);
            GUI.enabled = canUseFromCollection.boolValue;
            EditorGUILayout.PropertyField(canUseItemsFromReference);
            GUI.enabled = true;

            EditorGUILayout.PropertyField(canDragInCollection);
            EditorGUILayout.PropertyField(canPutItemsInCollection);
            EditorGUILayout.PropertyField(canStackItemsInCollection);
            EditorGUILayout.PropertyField(canUnstackItemsInCollection);

            EditorGUILayout.EndVertical();

        }
    }
}