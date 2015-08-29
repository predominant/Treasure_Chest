using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;


namespace Devdog.InventorySystem.Editors
{
    public class IntegrationHelperEditor : EditorWindow
    {

        private BuildTargetGroup[] allTargets;
        private Vector2 scrollPos = new Vector2();
        private Color grayishColor;

        [MenuItem("Tools/Inventory Pro/Integrations", false, 0)]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow<IntegrationHelperEditor>(true, "Integrations", true);
        }

        public void OnEnable()
        {
            allTargets = new BuildTargetGroup[]
            {
                BuildTargetGroup.Standalone,
                BuildTargetGroup.WebPlayer,
                BuildTargetGroup.iOS,
                BuildTargetGroup.PS3,
                BuildTargetGroup.XBOX360,
                BuildTargetGroup.Android,
                BuildTargetGroup.GLESEmu,
                BuildTargetGroup.WebGL,
                BuildTargetGroup.WSA,
                BuildTargetGroup.WP8,
                BuildTargetGroup.BlackBerry,
                BuildTargetGroup.Tizen,
                BuildTargetGroup.PSP2,
                BuildTargetGroup.PS4,
                BuildTargetGroup.PSM,
                BuildTargetGroup.XboxOne,
                BuildTargetGroup.SamsungTV
            };

            grayishColor = new Color(0.7f, 0.7f, 0.7f, 1.0f);

        }


        public void OnGUI()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);

            ShowIntegration("Easy Save 2", "Easy save is a fast and easy tool to load and save almost any data type.", "https://www.assetstore.unity3d.com/en/#!/content/768", "EASY_SAVE_2");
            ShowIntegration("PlayMaker", "Quickly make gameplay prototypes, A.I. behaviors, animation graphs, interactive objects, cut-scenes, walkthroughs", "https://www.assetstore.unity3d.com/en/#!/content/368", "PLAYMAKER");
            //ShowIntegration("iCode", "Quickly make gameplay prototypes, A.I. behaviors, interactions between GameObjects and more. ", "https://www.assetstore.unity3d.com/en/#!/content/13761", "ICODE");
            ShowIntegration("Behavior Designer", "Behavior trees are used by AAA studios to create a lifelike AI. With Behavior Designer, you can bring the power of behaviour trees to Unity!", "https://www.assetstore.unity3d.com/en/#!/content/15277", "BEHAVIOR_DESIGNER");
            //ShowIntegration("Dialogue System", "Dialogue System for Unity makes it easy to add interactive dialogue to your game. It's a complete, robust solution including a visual node-based editor, dialogue UIs, cutscenes, quests, save/load, and more.", "https://www.assetstore.unity3d.com/en/#!/content/11672", "DIALOGUE_SYSTEM");

            EditorGUILayout.BeginVertical(InventoryEditorStyles.boxStyle);
            ShowIntegration("UFPS (Ultimate FPS)", "Featuring the SMOOTHEST CONTROLS and the most POWERFUL FPS CAMERA available for Unity, Ultimate FPS is an awesome script pack for achieving that special AAA FPS feeling.", "https://www.assetstore.unity3d.com/en/#!/content/2943", "UFPS", false);
            ShowIntegration("UFPS Multiplayer", "UFPS Multiplayer", "https://www.assetstore.unity3d.com/en/#!/content/33752", "UFPS_MULTIPLAYER", false);
            EditorGUILayout.EndVertical();
            GUILayout.Space(10);

            ShowIntegration("plyGame", "plyGame has been designed with ease of use in mind and provides the components and editors you need to create games of various genres, from RPGs and Visual Novels to Action games.", "https://www.assetstore.unity3d.com/en/#!/content/9694", "PLY_GAME");
            

            GUILayout.EndScrollView();

        }

        private void ShowIntegration(string name, string description, string link, string defineName, bool showBox = true)
        {
            if(showBox)
                EditorGUILayout.BeginVertical(InventoryEditorStyles.boxStyle);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Toggle(IsEnabled(defineName), name))
            {
                EnableIntegration(defineName);
            }
            else
            {
                DisableIntegration(defineName);
            }
            if (GUILayout.Button("View in Asset store", EditorStyles.toolbarButton))
                Application.OpenURL(link);

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUI.color = grayishColor;
            EditorGUILayout.LabelField(description, InventoryEditorStyles.labelStyle);
            GUI.color = Color.white;

            if(showBox)
                EditorGUILayout.EndVertical();

            GUILayout.Space(10);
        }

        private bool IsEnabled(string name)
        {
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone).Contains(name);
        }

        private void DisableIntegration(string name)
        {
            if (IsEnabled(name) == false) // Already disabled
                return;

            foreach (var target in allTargets)
            {
                string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
                string[] items = symbols.Split(';');
                var l = new List<string>(items);
                l.Remove(name);

                PlayerSettings.SetScriptingDefineSymbolsForGroup(target, string.Join(";", l.ToArray()));
            }
        }

        private void EnableIntegration(string name)
        {
            if (IsEnabled(name)) // Already enabled
                return;

            foreach (var target in allTargets)
            {
                string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
                string[] items = symbols.Split(';');
                var l = new List<string>(items);
                l.Add(name);

                PlayerSettings.SetScriptingDefineSymbolsForGroup(target, string.Join(";", l.ToArray()));
            }
        }
    }
}