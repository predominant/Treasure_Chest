using UnityEditor;
using UnityEngine;

namespace Gamelogic.Editor
{
	/// <summary>
	/// A property drawer for fields marked with the InspectorFlags Attribute.
	/// </summary>
	[Version(1, 4, 3)]
	[CustomPropertyDrawer(typeof(InspectorFlags))]
	public class FlagsDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position,
			SerializedProperty prop,
			GUIContent label)
		{
			prop.intValue = EditorGUI.MaskField(position, label, prop.intValue, prop.enumNames);
		}
	}
}