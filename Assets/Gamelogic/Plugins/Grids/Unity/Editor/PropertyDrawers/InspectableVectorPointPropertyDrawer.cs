using UnityEditor;
using UnityEngine;

namespace Gamelogic.Grids.Editor
{
	[CustomPropertyDrawer(typeof(InspectableVectorPoint))]
	public class InspectableVectorPointPropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			EditorGUIUtility.LookLikeControls(15, position.width/2 - 15);

			var halfWidth = position.width/2;
			var xRect = new Rect(position.x, position.y, halfWidth - 2, position.height);
			var yRect = new Rect(position.x + halfWidth, position.y, halfWidth, position.height);
		
			EditorGUI.PropertyField(xRect, property.FindPropertyRelative("x"));
			EditorGUI.PropertyField(yRect, property.FindPropertyRelative("y"));

			EditorGUI.EndProperty();
		}
	}
}