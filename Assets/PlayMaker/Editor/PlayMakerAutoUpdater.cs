using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HutongGames.PlayMakerEditor
{
    /// <summary>
    /// Try to fix common update problems automatically
    /// </summary>
    [InitializeOnLoad]
    public class PlayMakerAutoUpdater
    {
        private const string version = "1.7.8.3";
        static List<string> changeList = new List<string>();

        private static readonly BuildTarget[] standardPlatforms =
        {
            BuildTarget.Android,
            BuildTarget.BlackBerry,
            BuildTarget.StandaloneLinux,
            BuildTarget.StandaloneLinux64,
            BuildTarget.StandaloneLinuxUniversal,
            BuildTarget.StandaloneOSXIntel,
            BuildTarget.StandaloneOSXIntel64,
            BuildTarget.StandaloneOSXUniversal,
            BuildTarget.StandaloneWindows,
            BuildTarget.StandaloneWindows64,
            BuildTarget.WebPlayer,
            BuildTarget.WebPlayerStreamed,
            BuildTarget.iOS
        };

        // static constructor called on load
        static PlayMakerAutoUpdater()
        {
            if (ShouldUpdate())
            {
                // Can't call assetdatabase here, so use update callback
                EditorApplication.update -= RunAutoUpdate;
                EditorApplication.update += RunAutoUpdate;
            }
        }

        static bool ShouldUpdate()
        {
            if (string.IsNullOrEmpty(Application.dataPath)) return false;
            if (EditorPrefs.GetString("LastAutoUpdate", "") != GetUpdateSignature())  
            {
                // save auto update settings
                // so we don't get caught in infinite loop when re-importing
                EditorPrefs.SetString("LastAutoUpdate", GetUpdateSignature());
                return true;
            }
            return false;
        }

        // Get a unique signature for this update to avoid repeatedly updating the same project
        // NOTE: might be a better way to do this. Currently doesn't catch project changes like imports...
        static string GetUpdateSignature()
        {
            return Application.unityVersion + "__" + Application.dataPath + "__" + version;
        }

        // Check pre-requisites for auto updating
        // e.g., Unity 5 version of Playmaker is imported
        static bool CheckRequirements()
        {
            // If project doesn't have this folder user hasn't updated Playmaker for Unity5
            if (!AssetDatabase.IsValidFolder("Assets/Plugins/PlayMaker"))
            {
                EditorUtility.DisplayDialog("PlayMaker AutoUpdater",
                    "Please import Playmaker 1.7.8 for Unity 5."+
                    "\n\nTo get the latest version, update in the Unity Asset Store "+
                    "or download from Hutong Games Store."+
                    "\n\nNOTE: You can run the AutoUpdater from PlayMaker > Tools > Run AutoUpdater", "OK");
                return false;
            }
            return true;
        }

        public static void RunAutoUpdate()
        {
            //Debug.Log("PlayMaker AutoUpdater " + version);
            EditorApplication.update -= RunAutoUpdate;

            if (!CheckRequirements())
            {
                Debug.Log("PlayMaker AutoUpdate: Failed to update.");
                return;
            }

            if (NeedsUpdate())
            {
                if (EditorUtility.DisplayDialog("PlayMaker",
                    "PlayMaker AutoUpdater would like to move PlayMaker.dll " +
                    "from Assets/PlayMaker to Assets/Plugins/PlayMaker" +
                    "\n\nNOTE: You can run the AutoUpdater from PlayMaker > Tools > Run AutoUpdater",
                    "OK", "Cancel"))
                {
                    DoUpdate();
                }
            }
        }

        static bool NeedsUpdate()
        {
            return AssetImporter.GetAtPath("Assets/PlayMaker/PlayMaker.dll") != null;
        }

        static void DoUpdate()
        {
            MovePlayMakerDll();

            // plugin settings need to be set (note doing this before move doesn't seem to take)
            if (!FixPlayMakerDllSettings("Assets/Plugins/PlayMaker/PlayMaker.dll"))
            {
                Debug.LogWarning("Failed to change Assets/Plugins/PlayMaker/PlayMaker.dll settings.");
            }

            ReportChanges();
        }

        static bool FixPlayMakerDllSettings(string pluginPath)
        {
            var pluginImporter = (PluginImporter)AssetImporter.GetAtPath(pluginPath);
            if (pluginImporter != null)
            {
                FixPlayMakerDllSettings(pluginImporter);
                return true;
            }

            return false;
        }
        
        static void FixPlayMakerDllSettings(PluginImporter pluginImporter)
        {
            LogChange("Fixed Plugin Settings: " + pluginImporter.assetPath);

            pluginImporter.SetCompatibleWithAnyPlatform(false);
            pluginImporter.SetCompatibleWithEditor(true);
            SetCompatiblePlatforms(pluginImporter, standardPlatforms);
            AssetDatabase.Refresh();
        }

        static void SetCompatiblePlatforms(PluginImporter pluginImporter, IEnumerable<BuildTarget> platforms)
        {
            foreach (var platform in platforms)
            {
                pluginImporter.SetCompatibleWithPlatform(platform, true);
            }
        }

        static void MovePlayMakerDll()
        {
            LogChange("Moved PlayMaker.dll from Assets/PlayMaker to Assets/PlayMaker/Plugins");

            AssetDatabase.DeleteAsset("Assets/Plugins/PlayMaker/PlayMaker.dll");
            AssetDatabase.Refresh();
            AssetDatabase.MoveAsset("Assets/PlayMaker/PlayMaker.dll", "Assets/Plugins/PlayMaker/PlayMaker.dll");
            AssetDatabase.Refresh();
        }

        static void LogChange(string change)
        {
            //Debug.Log("PlayMaker AutoUpdate: " + change);
            changeList.Add(change);
        }

        static void ReportChanges()
        {
            if (changeList.Count > 0)
            {
                var changeLog = "PlayMaker AutoUpdater Changes:";
                foreach (var change in changeList)
                {
                    changeLog += "\n" + change;
                }
                Debug.Log(changeLog);
            }
            else
            {
                Debug.Log("PlayMaker AutoUpdater: No changes made");
            }
        }

    }
}