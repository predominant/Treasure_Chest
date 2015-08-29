using System;
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Net;
using System.Text.RegularExpressions;


namespace Devdog.InventorySystem.Editors
{
    public class InventoryProNewsEditor : EditorWindow
    {
        private Vector2 scrollPos { get; set; }

        private Texture2D _texture;
        public Texture2D texture
        {
            get
            {
                if(_texture == null)
                    _texture = (Texture2D)AssetDatabase.LoadMainAssetAtPath("Assets/InventorySystem/EditorStyles/InventoryProAboutHeader.png");

                return _texture;
            }
        }

        private const string NewsUri = @"http://devdog.nl/unity/unity-editor-news/";
        private static string[] news;


        [MenuItem("Tools/Inventory Pro/News", false, 5)] // Always at bottom
        public static void ShowWindow()
        {
            var window = GetWindow<InventoryProNewsEditor>(true, "Inventory Pro news", true);
            window.minSize = new Vector2(400, 500);
            window.maxSize = new Vector2(400, 500);

            FetchNews();
        }

        private static void FetchNews()
        {
            // Grab news from web
            using (var client = new WebClient())
            {
                client.DownloadStringCompleted += DownloadedString;
                client.DownloadStringAsync(new Uri(NewsUri));
            }
        }

        private static void DownloadedString(object sender, DownloadStringCompletedEventArgs e)
        {
            string noHtml = Regex.Replace(e.Result, @"<[^>]+>|&nbsp;", "").Trim(); // Remove html tags
            noHtml = Regex.Replace(noHtml, @"\s{2,}", " "); // Remove double spaces in string

            //Debug.Log("Fetched data from web + " + noHtml);
            news = noHtml.Split(new[] { "---" }, StringSplitOptions.None);
        }

        public void OnGUI()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            
            //GUILayout.Label(texture);
            GUI.DrawTexture(new Rect(0, 0, 400, 181), texture, ScaleMode.ScaleAndCrop, false);
            GUI.BeginGroup(new Rect(0, 190, 400, 300));

            if (GUILayout.Button("Refresh news"))
            {
                FetchNews();
                Repaint();
            }

            if (news != null)
            {
                GUILayout.BeginVertical();

                foreach (var message in news)
                {
                    GUILayout.BeginHorizontal(InventoryEditorStyles.boxStyle, GUILayout.MaxWidth(400));
                    GUILayout.Label(message, InventoryEditorStyles.labelStyle);
                    GUILayout.Space(10);
                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();
            }

            GUI.EndGroup();
            GUILayout.EndScrollView();
        }
    }
}