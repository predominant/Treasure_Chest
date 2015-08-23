using System;
using UnityEditor;
using UnityEngine;

namespace Gamelogic.Grids.Editor.Internal
{
	[InitializeOnLoad]
	public static class GLEditorExtensions
	{
		private static readonly EditorApplication.HierarchyWindowItemCallback hiearchyItemCallback;

		private static Texture2D gridIcon;
		private static Texture2D cellIcon;

		private static Texture2D GridIcon
		{
			get
			{
				if (gridIcon == null)
				{
					gridIcon = (Texture2D) Resources.Load("GridsEditor/grid");
				}
				return gridIcon;
			}
		}

		private static Texture2D CellIcon
		{
			get
			{
				if (cellIcon == null)
				{
					cellIcon = (Texture2D) Resources.Load("GridsEditor/cell");
				}
				return cellIcon;
			}
		}

		// constructor
		static GLEditorExtensions()
		{
			hiearchyItemCallback = DrawHierarchyIcon;
			EditorApplication.hierarchyWindowItemOnGUI =
				(EditorApplication.HierarchyWindowItemCallback)
					Delegate.Combine(EditorApplication.hierarchyWindowItemOnGUI, hiearchyItemCallback);
		}

		private static void DrawHierarchyIcon(int instanceID, Rect selectionRect)
		{
			var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

			if (gameObject == null)
				return;

			var rect = new Rect(selectionRect.x + selectionRect.width - 18f, selectionRect.y, 16f, 16f);

			var view = gameObject.GetComponent(typeof (IGridBuilderBase));

			if (view != null)
			{
				GUI.DrawTexture(rect, GridIcon);
			}

			view = gameObject.GetComponent(typeof (ICell));

			if (view != null)
			{
				GUI.DrawTexture(rect, CellIcon);
			}
		}
	}
}