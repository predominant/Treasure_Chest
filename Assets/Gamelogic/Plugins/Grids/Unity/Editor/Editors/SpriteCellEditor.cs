//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) Gamelogic (Pty) Ltd            //
//----------------------------------------------//

using UnityEditor;
using UnityEngine;

namespace Gamelogic.Grids.Editor.Internal
{
	[CustomEditor(typeof (SpriteCell), false)]
	[CanEditMultipleObjects]
	public class SpriteCellEditor : GLEditor<SpriteCell>
	{
		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			var spritesProperty = FindProperty("sprites");

			AddField(spritesProperty);


			if (GUI.changed)
			{
				serializedObject.ApplyModifiedProperties();

				foreach (var t in Targets)
				{
					t.__UpdatePresentation(false);
					EditorUtility.SetDirty(t);
				}
			}
		}
	}
}
