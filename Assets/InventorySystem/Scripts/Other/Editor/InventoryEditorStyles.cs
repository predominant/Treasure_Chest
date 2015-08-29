using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Devdog.InventorySystem.Editors
{
    public static class InventoryEditorStyles
    {

        private static GUIStyle _boxStyle;
        public static GUIStyle boxStyle
        {
            get
            {
                if (_boxStyle == null)
                {
                    _boxStyle = new GUIStyle("HelpBox");
                    _boxStyle.padding = new RectOffset(10, 10, 10, 10);
                }

                return _boxStyle;
            }
        }


        private static GUIStyle _titleStyle;
        public static GUIStyle titleStyle
        {
            get
            {
                if (_titleStyle == null)
                {
                    _titleStyle = new GUIStyle("IN TitleText");
                    _titleStyle.alignment = TextAnchor.MiddleLeft;
                    _titleStyle.padding.left += 5;
                    _titleStyle.margin.top += 10;
                }

                return _titleStyle;
            }
        }


        private static GUIStyle _infoStyle;
        public static GUIStyle infoStyle
        {
            get
            {
                if (_infoStyle == null)
                {
                    _infoStyle = new GUIStyle(EditorStyles.label);
                    _infoStyle.wordWrap = true;
                    //_infoStyle.normal.textColor = new Color(0.6f, 0.4f, 0.1f);
                }

                return _infoStyle;
            }
        }


        private static GUIStyle _reorderableListStyle;
        public static GUIStyle reorderableListStyle
        {
            get
            {
                if (_reorderableListStyle == null)
                {
                    _reorderableListStyle = new GUIStyle();
                    _reorderableListStyle.padding = new RectOffset(5, 5, 5, 5);    
                }
                
                return _reorderableListStyle;
            }
        }



        private static GUIStyle _richTextArea;
        public static GUIStyle richTextArea
        {
            get
            {
                if (_richTextArea == null)
                {
                    _richTextArea = new GUIStyle(EditorStyles.textArea);
                    _richTextArea.richText = true;
                    _richTextArea.wordWrap = true;
                    _richTextArea.fixedHeight = 40.0f;
                    _richTextArea.stretchHeight = true;
                }

                return _richTextArea;
            }
        }


        private static GUIStyle _labelStyle;
        public static GUIStyle labelStyle
        {
            get
            {
                if (_labelStyle == null)
                {
                    _labelStyle = new GUIStyle(EditorStyles.label);
                    _labelStyle.wordWrap = true;
                }

                return _labelStyle;
            }
        }

        public static float labelWidth
        {
            get { return 200.0f; }
        }

        private static GUIStyle _toolbarStyle;
        public static GUIStyle toolbarStyle
        {
            get
            {
                if (_toolbarStyle == null)
                {
                    _toolbarStyle = new GUIStyle(EditorStyles.toolbarButton);
                    _toolbarStyle.fixedHeight = 40;                    
                }

                return _toolbarStyle;
            }
        }




        public static string SearchBar(string searchQuery, EditorWindow window, bool isSearching)
        {
            EditorGUILayout.BeginHorizontal();
            GUI.SetNextControlName("SearchField");
            string q = EditorGUILayout.TextField(searchQuery, (GUIStyle)"SearchTextField"); // , GUILayout.Width(width)
            if (isSearching)
            {
                if (GUILayout.Button("", (GUIStyle)"SearchCancelButton", GUILayout.Width(17)))
                {
                    q = ""; // Reset
                    if(window != null)
                        window.Repaint();
                }
            }
            else
            {
                GUILayout.Button("", (GUIStyle)"SearchCancelButtonEmpty", GUILayout.Width(17));
            }

            EditorGUILayout.EndHorizontal();

            return q;
        }
    }
}
