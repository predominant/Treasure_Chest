using UnityEngine;
using UnityEditor;
using System;

namespace Devdog.InventorySystem.Editors
{
    [CustomEditor(typeof(ObjectTriggerer), true)]
    [CanEditMultipleObjects]
    public class ObjectTriggererEditor : Editor
    {

        private SerializedProperty window;

        private static Color outOfRangeColor;
        private static Color inRangeColor;

        public virtual void OnEnable()
        {
            window = serializedObject.FindProperty("_window");

            outOfRangeColor = new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, 0.2f);
            inRangeColor = new Color(Color.green.r, Color.green.g, Color.green.b, 0.3f);
        }


        public void OnSceneGUI()
        {
            var settings = InventoryEditorUtility.GetSettingsManager();
            if (settings == null)
                return;

            var triggerer = (ObjectTriggerer)target;

            var discColor = outOfRangeColor;
            if (Application.isPlaying && triggerer.inRange)
            {
                discColor = inRangeColor;
            }

            Handles.color = discColor;
            var euler = triggerer.transform.rotation.eulerAngles;
            euler.x += 90.0f;
            Handles.DrawSolidDisc(triggerer.transform.position, Vector3.up, settings.useObjectDistance);

            discColor.a += 0.2f;
            Handles.color = discColor;
            Handles.CircleCap(0, triggerer.transform.position, Quaternion.Euler(euler), settings.useObjectDistance);
        }



        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Draws remaining items
            //EditorGUILayout.BeginVertical("box");
            DrawPropertiesExcluding(serializedObject, new string[]
            {
                "window"
            });
            //EditorGUILayout.EndVertical();

            var o = ((ObjectTriggerer) target).gameObject.GetComponent(typeof (IObjectTriggerUser));
            if (o != null)
            {
                //if(window.objectReferenceValue != o)
                window.objectReferenceValue = o;

                GUI.enabled = false;
            }

            EditorGUILayout.PropertyField(window);
            GUI.enabled = true;

            if(o != null)
                EditorGUILayout.HelpBox("Window is managed by " + o.GetType().Name, MessageType.Info);

            
            serializedObject.ApplyModifiedProperties();
        }
    }
}