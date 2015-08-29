using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using Devdog.InventorySystem.UI;

namespace Devdog.InventorySystem.Editors
{
    [CustomEditor(typeof(UIWindow), true)]
    [CanEditMultipleObjects]
    public class UIWindowEditor : Editor
    {
        private SerializedProperty script;

        private SerializedProperty hideOnStart;
        private SerializedProperty resetPositionOnStart;
        //private SerializedProperty keyCombination;

        private SerializedProperty showAnimation;
        private SerializedProperty hideAnimation;
        private SerializedProperty showAudioClip;
        private SerializedProperty hideAudioClip;

        //private UnityEditorInternal.ReorderableList keyCombinationList;

        private bool isInCanvas = false;

        public virtual void OnEnable()
        {
            script = serializedObject.FindProperty("m_Script");

            hideOnStart = serializedObject.FindProperty("hideOnStart");
            resetPositionOnStart = serializedObject.FindProperty("resetPositionOnStart");

            showAnimation = serializedObject.FindProperty("_showAnimation");
            hideAnimation = serializedObject.FindProperty("_hideAnimation");
            showAudioClip = serializedObject.FindProperty("showAudioClip");
            hideAudioClip = serializedObject.FindProperty("hideAudioClip");

            var t = (UIWindow) target;
            isInCanvas = t.gameObject.GetComponentInParent<Canvas>() != null;
        }


        /// <summary>
        /// Unity method, draws the outlines of the UIWindow to visualize boundries.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="gizmoType"></param>
        [DrawGizmo(GizmoType.Selected | GizmoType.Active | GizmoType.NonSelected)]
        static void DrawOutlines(UIWindow window, GizmoType gizmoType)
        {
            var rectTransform = window.gameObject.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                if (EditorPrefs.HasKey("InventoryPro_UIWindowDebug") == false)
                    return;

                var color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.6f);
                Gizmos.color = color;
                Handles.color = color;


                Vector3[] corners = new Vector3[4];
                rectTransform.GetWorldCorners(corners);


                // Draw window name
                //Handles.DrawBezier(Vector3.zero, Vector3.one * 1000, Vector3.up * 500, Vector3.up * 500, Color.yellow, null, 2.0f);
                //Handles.ArrowCap(0, Vector3.one * 1000, Quaternion.Euler(90.0f, 0.0f, 0.0f), 100.0f);
                DrawWindowName(corners[1] + Vector3.up * 20, window.windowName);


                // Draw the bounding box
                Gizmos.DrawLine(corners[0], corners[1]);
                Gizmos.DrawLine(corners[1], corners[2]);
                Gizmos.DrawLine(corners[2], corners[3]);
                Gizmos.DrawLine(corners[3], corners[0]);



                Gizmos.color = Color.white;
                Handles.color = Color.white;
            }
        }

        protected static void DrawWindowName(Vector3 position, string name)
        {
            Handles.Label(position, name, "GUIEditor.BreadcrumbLeft");
        }


        [DrawGizmo(GizmoType.Selected | GizmoType.Active | GizmoType.NonSelected)]
        static void DrawButtonActions(UnityEngine.UI.Button button, GizmoType gizmoType)
        {   
            if (EditorPrefs.HasKey("InventoryPro_UIWindowDebug") == false)
                return;

            // No events
            if (button.onClick.GetPersistentEventCount() == 0)
                return;

            var rectTransform = button.gameObject.GetComponent<RectTransform>();
            if (rectTransform == null)
                return;

            
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            //bool clicked = Handles.Button(corners[2], Quaternion.identity, 20.0f, 10.0f, ButtonActionDrawMethod);
            //if (clicked)
            //{
            //    Debug.Log("Select window!!");
            //}
        }

        private static void ButtonActionDrawMethod(int controlid, Vector3 position, Quaternion rotation, float size)
        {
            //Gizmos.DrawCube(position + (Vector3.up * (size * 0.5f)), new Vector3(size * 3, size, 1.0f));

            //size = HandleUtility.GetHandleSize(position);
            DrawText("Testing", position + Vector3.up * (size * 0.5f), Color.yellow, 14);
        }
        private static void DrawText(String text, Vector3 pos, Color color, int fontSize = 0, float yOffset = 0)
        {
            GUIContent textContent = new GUIContent(text);

            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
            style.normal.textColor = color;
            //style.normal.background = Texture2D.whiteTexture;

            if (fontSize > 0)
                style.fontSize = fontSize;

            Vector2 textSize = style.CalcSize(textContent);
            Vector3 screenPoint = Camera.current.WorldToScreenPoint(pos);

            if (screenPoint.z > 0) // check, it is necessary that the text is not visible when the camera is pointed in the opposite direction relative to the object
            {
                var worldPosition = Camera.current.ScreenToWorldPoint(new Vector3(screenPoint.x - textSize.x * 0.5f, screenPoint.y + textSize.y * 0.5f + yOffset, screenPoint.z));
                Handles.Label(worldPosition, textContent, style);
            }
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var t = (UIWindow) target;
            if (isInCanvas == false)
            {
                isInCanvas = t.gameObject.GetComponentInParent<Canvas>() != null;

                EditorGUILayout.HelpBox("UIWindow's can only be used inside a canvas!", MessageType.Error);
                //GUI.enabled = false;
            }

            if (GUILayout.Button("Toggle debugging"))
            {
                if (EditorPrefs.HasKey("InventoryPro_UIWindowDebug") == false)
                {
                    EditorPrefs.SetBool("InventoryPro_UIWindowDebug", true);
                }
                else
                {
                    EditorPrefs.DeleteKey("InventoryPro_UIWindowDebug");
                }
            }

            //if (GUILayout.Button("Show"))
            //    t.Show();


            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(script);

            EditorGUILayout.PropertyField(hideOnStart);
            EditorGUILayout.PropertyField(resetPositionOnStart);
            //keyCombinationList.DoLayoutList();
            //EditorGUILayout.PropertyField(hideOnStart);

            // Draws remaining items
            DrawPropertiesExcluding(serializedObject, new string[]
            {
                "m_Script",
                "hideOnStart",
                "resetPositionOnStart",
                "keyCombination",
                "_showAnimation",
                "_hideAnimation",
                "showAudioClip",
                "hideAudioClip",
            });


            EditorGUILayout.PropertyField(showAnimation);
            EditorGUILayout.PropertyField(hideAnimation);
            EditorGUILayout.PropertyField(showAudioClip);
            EditorGUILayout.PropertyField(hideAudioClip);


            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}