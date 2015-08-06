using UnityEditor;
using UnityEngine;
using System.Collections;

namespace BeardedManStudios.Network.Unity
{
	[CustomEditor(typeof(Autostart), true)]
	public class Autostart_Editor : Editor
	{
		private Autostart Target { get { return (Autostart)target; } }
		public enum Platform
		{
			Windows,
			Mac,
			Linux
		}

		public Platform platform = Platform.Windows;

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			platform = (Platform)EditorGUILayout.EnumPopup("Build Platform", platform);

			if (GUILayout.Button("Compile and Run"))
				Build();

			Repaint();
		}

		private void Build()
		{
			string path = EditorUtility.SaveFolderPanel("Choose Build Location", "", "");

			if (string.IsNullOrEmpty(path))
			{
				Debug.LogError("Aborted because the path was not specified");
				return;
			}

			string[] levels = new string[2];
			for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
			{
				if (EditorBuildSettings.scenes[i].path.Contains(EditorApplication.currentScene))
					levels[0] = EditorBuildSettings.scenes[i].path;
				else if (EditorBuildSettings.scenes[i].path.Contains(Target.sceneName))
					levels[1] = EditorBuildSettings.scenes[i].path;
			}

			if (levels[0] == null || levels[1] == null)
			{
				Debug.LogError("Aborted because the current scene and the target scene were not in the build settings");
				return;
			}

			path += "/Autobuild";

			if (System.IO.Directory.Exists(path))
				System.IO.Directory.Delete(path, true);

			System.IO.Directory.CreateDirectory(path);

			BuildTarget target = BuildTarget.StandaloneWindows;
			string extension = ".exe";

			if (platform == Platform.Mac)
			{
				target = BuildTarget.StandaloneOSXIntel;
				extension = ".app";
			}
			else if (platform == Platform.Linux)
			{
				target = BuildTarget.StandaloneLinux;
				extension = ".so";
			}

			Target.executePath = path + "/autobuild" + extension;

			ResolutionDialogSetting resolutionDialog = PlayerSettings.displayResolutionDialog;

			PlayerSettings.runInBackground = true;
			PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;

			BuildPipeline.BuildPlayer(levels, Target.executePath, target, BuildOptions.None);

			PlayerSettings.displayResolutionDialog = resolutionDialog;

			EditorApplication.isPlaying = true;
		}
	}
}