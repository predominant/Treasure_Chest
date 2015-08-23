using UnityEditor;
using UnityEngine;

namespace Gamelogic.Grids.Editor
{
	[CustomPropertyDrawer(typeof(ColorFunction))]
	public class ColorFunctionPropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			EditorGUIUtility.LookLikeControls(20, position.width/3 - 20);

			var cellWidth = position.width/3;
			var x0Rect = new Rect(position.x, position.y, cellWidth - 2, position.height);
			var x1Rect = new Rect(position.x + cellWidth, position.y, cellWidth- 2, position.height);
			var y1Rect = new Rect(position.x + 2 * cellWidth, position.y, cellWidth, position.height);

			EditorGUI.PropertyField(x0Rect, property.FindPropertyRelative("x0"));
			EditorGUI.PropertyField(x1Rect, property.FindPropertyRelative("x1"));
			EditorGUI.PropertyField(y1Rect, property.FindPropertyRelative("y1"));

			EditorGUI.EndProperty();
		}
	}
}