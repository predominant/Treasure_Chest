//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using Gamelogic.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Gamelogic.Grids.Editor.Internal
{
	/**
		The base class of grid editors. This class implements the core logic.
	*/
	public class SimpleGridEditor<TGridBuilder, TPoint> : GLEditor<TGridBuilder>
		where TGridBuilder : MonoBehaviour, ITileGrid<TPoint>
		where TPoint : IGridPoint<TPoint> //, IVectorPoint<TPoint>
	{
		private const string HelpURL = @"http://gamelogic.co.za/grids/documentation-contents/";

		private GLSerializedProperty updateTypeProp;
		private GLSerializedProperty isInterActiveProp;
		private GLSerializedProperty cellPrefabProp;
		private GLSerializedProperty shapeProp;
		private GLSerializedProperty dimensionsProp;
		private GLSerializedProperty sizeProp;

		private GLSerializedProperty polarGridPropertiesProp;
		private GLSerializedProperty neighborSetupProp;
		private GLSerializedProperty pointListProp;
		private GLSerializedProperty poissonDiskPropertiesProp;

		private GLSerializedProperty mapTypeProp;
		private GLSerializedProperty planeProp;
		private GLSerializedProperty alignmentProp;
		private GLSerializedProperty cellSpacingFactorProp;
		private GLSerializedProperty useColorProp;
		private GLSerializedProperty colorsProp;
		private GLSerializedProperty colorFunctionProp;

		private ReorderableList colorList;

		public void OnEnable()
		{
			updateTypeProp = FindProperty("updateType");
			isInterActiveProp = FindProperty("isInteractive");
			cellPrefabProp = FindProperty("cellPrefab");
			shapeProp = FindProperty("shape");
			dimensionsProp = FindProperty("dimensions");
			sizeProp = FindProperty("size");

			neighborSetupProp = HasProperty("neighborSetup") ? FindProperty("neighborSetup") : null;
			polarGridPropertiesProp = HasProperty("polarGridProperties") ? FindProperty("polarGridProperties") : null;
			pointListProp = HasProperty("pointList") ? FindProperty("pointList") : null;
			poissonDiskPropertiesProp = HasProperty("poissonDiskProperties") ? FindProperty("poissonDiskProperties") : null;
			mapTypeProp = FindProperty("mapType");
			planeProp = FindProperty("plane");
			alignmentProp = FindProperty("alignment");
			cellSpacingFactorProp = FindProperty("cellSpacingFactor");
			useColorProp = FindProperty("useColor");
			colorsProp = FindProperty("colors");
			colorFunctionProp = FindProperty("colorFunction");

			colorList = new ReorderableList(serializedObject, colorsProp.SerializedProperty, true, true, true, true);
			
			colorList.drawHeaderCallback += rect => GUI.Label(rect, "Colors");

			colorList.drawElementCallback += (rect, index, active, focused) =>
			{
				rect.height = 16;
				rect.y += 2;
				if (index >= colorsProp.SerializedProperty.arraySize) return;
				var color = colorsProp.SerializedProperty.GetArrayElementAtIndex(index).colorValue;
				
				color = EditorGUI.ColorField(rect, color);

				colorsProp.SerializedProperty.GetArrayElementAtIndex(index).colorValue = color;
			};

		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.BeginHorizontal();
			
			if (GUILayout.Button("Help", EditorStyles.miniButton, GUILayout.MaxWidth(80)))
			{
				Application.OpenURL(HelpURL);
			}

			if (updateTypeProp.enumValueIndex == (int) UpdateType.EditorManual)
			{
				if (GUILayout.Button("Build Grid", EditorStyles.miniButton))
				{
					Target.__UpdatePresentation(true);
				}
			}

			EditorGUILayout.EndHorizontal();

			AddField(updateTypeProp);
			AddField(isInterActiveProp);

			Splitter();
			AddField(cellPrefabProp);

			Splitter();
			AddField(shapeProp);

			var shape = shapeProp.enumValueIndex;

			if (ShowDimensions(shape))
			{
				AddField(dimensionsProp);
			}
			else if (ShowSize(shape))
			{
				AddField(sizeProp);
			}

			if (IsCustomShape(shape))
			{
				var customGridBuilder = Target.GetComponent<CustomGridBuilder>();

				if (customGridBuilder == null)
				{
					AddLabel("", "No custom grid builder attached to this component.");
				}
				else
				{
					AddLabel("Custom Shape", customGridBuilder.GetType().ToString().SplitCamelCase());
				}
			}

			if (neighborSetupProp != null)
			{
				AddField(neighborSetupProp);
			}

			if (polarGridPropertiesProp != null)
			{
				AddField(polarGridPropertiesProp);
			}

			if (pointListProp != null)
			{
				AddField(pointListProp);
			}

			if (poissonDiskPropertiesProp != null)
			{
				AddField(poissonDiskPropertiesProp);
			}

			Splitter();
			AddField(mapTypeProp);
			AddField(planeProp);
			AddField(alignmentProp);
			AddField(cellSpacingFactorProp);

			Splitter();
			AddField(useColorProp);

			if (useColorProp.boolValue)
			{
				//AddField(colorsProp);

				colorList.DoLayoutList();

				AddField(colorFunctionProp);
			}

			if (GUI.changed)
			{
				CheckPositive(sizeProp);
				CheckDimensions(dimensionsProp);
				CheckColorFunction(colorFunctionProp);
				serializedObject.ApplyModifiedProperties();

				Target.__UpdatePresentation(false);
			}
		}

		protected virtual bool IsCustomShape(int shape)
		{
			AddLabel("Custom Shape", "Not implemented");

			return false;
		}

		protected virtual bool ShowSize(int shape)
		{
			AddLabel("Size", "Not implemented");

			return false;
		}

		protected virtual bool ShowDimensions(int shape)
		{
			AddLabel("Dimensions", "Not implemented");

			return false;
		}

		private static void CheckPositive(GLSerializedProperty property)
		{
			if (property.intValue < 1)
			{
				property.intValue = 1;
			}
		}

		private static void CheckNonNegative(GLSerializedProperty property)
		{
			if (property.intValue < 0)
			{
				property.intValue = 0;
			}
		}

		private static void CheckDimensions(GLSerializedProperty property)
		{
			CheckPositive(property.FindPropertyRelative("x"));
			CheckPositive(property.FindPropertyRelative("y"));
		}

		private static void CheckColorFunction(GLSerializedProperty property)
		{
			CheckPositive(property.FindPropertyRelative("x0"));
			CheckNonNegative(property.FindPropertyRelative("x1"));
			CheckPositive(property.FindPropertyRelative("y1"));
		}
	}
}
