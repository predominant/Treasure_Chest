//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2014 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		Provides methods for drawing Gizmos in
		for grids in the Unity editor.

		@version1_8
	*/
	public static class GLGizmos
	{
		public static readonly GUIStyle DefaultLabelStyle;
		public static readonly GUIStyle AlternativeLabelStyle;
		public static readonly Color BackgroundColor = new Color(42/255f, 192/255f, 217/255f, 0.5f);

#if UNITY_EDITOR
		static GLGizmos()
		{
			DefaultLabelStyle = new GUIStyle
			{
				normal =
				{
					background = EditorGUIUtility.whiteTexture,
					textColor = Color.white
				},

				margin = new RectOffset(0, 0, 0, 0),
				padding = new RectOffset(0, 0, 0, 0),
				alignment = TextAnchor.MiddleCenter,
				border = new RectOffset(6, 6, 6, 6),
				fontSize = 12
			};

			AlternativeLabelStyle = new GUIStyle
			{
				normal =
				{
					background = EditorGUIUtility.whiteTexture,
					textColor = Color.black
				},

				margin = new RectOffset(0, 0, 0, 0),
				padding = new RectOffset(0, 0, 0, 0),
				alignment = TextAnchor.MiddleCenter,
				border = new RectOffset(6, 6, 6, 6),
				fontSize = 12
			};
		}
#endif

		private static Texture2D MakeTexture(int width, int height, Color color)
		{
			var pixels = new Color[width*height];

			for (int i = 0; i < pixels.Length; ++i)
			{
				pixels[i] = color;
			}

			var texture = new Texture2D(width, height);

			texture.SetPixels(pixels);
			texture.Apply();

			return texture;
		}

		public static void Label(Vector3 position, string label)
		{
#if UNITY_EDITOR
			if (string.IsNullOrEmpty(label)) return;
			var color = GUI.color;
			GUI.color = Color.white;

			var backgroundColor = GUI.backgroundColor;
			GUI.backgroundColor = BackgroundColor;

			Handles.Label(position, label, DefaultLabelStyle);

			GUI.backgroundColor = backgroundColor;
			GUI.color = color;
#endif
		}

		public static void LabelWB(Vector3 position, string label)
		{
#if UNITY_EDITOR
			if (string.IsNullOrEmpty(label)) return;
			var color = GUI.color;
			GUI.color = Color.white;

			var backgroundColor = GUI.backgroundColor;
			GUI.backgroundColor = new Color(1, 1, 1, 0.5f); ;

			Handles.Label(position, label, AlternativeLabelStyle);

			GUI.backgroundColor = backgroundColor;
			GUI.color = color;
#endif
		}
	}
}
