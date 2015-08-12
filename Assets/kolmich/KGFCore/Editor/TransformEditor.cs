using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Transform))]
public class TransformEditor : Editor
{
	public override void OnInspectorGUI()
	{
		Transform aTarget = (Transform)target;
		
		GUILayout.BeginHorizontal();
		{
			if (GUILayout.Button("x",GUILayout.Width(20)))
			{
				aTarget.localPosition = Vector3.zero;
				EditorUtility.SetDirty(aTarget);
			}
			Vector3 aValue = EditorGUILayout.Vector3Field("Position",aTarget.localPosition);
			if (aValue != aTarget.localPosition)
			{
				aTarget.localPosition = aValue;
				EditorUtility.SetDirty(aTarget);
			}
		}
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		{
			if (GUILayout.Button("x",GUILayout.Width(20)))
			{
				aTarget.localEulerAngles = Vector3.zero;
				EditorUtility.SetDirty(aTarget);
			}
			Vector3 aValue = EditorGUILayout.Vector3Field("Rotation",aTarget.localEulerAngles);
			if (aValue != aTarget.localEulerAngles)
			{
				aTarget.localEulerAngles = aValue;
				EditorUtility.SetDirty(aTarget);
			}
		}
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		{
			if (GUILayout.Button("x",GUILayout.Width(20)))
			{
				aTarget.localScale = Vector3.one;
				EditorUtility.SetDirty(aTarget);
			}
			Vector3 aValue = EditorGUILayout.Vector3Field("Scale",aTarget.localScale);
			if (aValue != aTarget.localScale)
			{
				aTarget.localScale = aValue;
				EditorUtility.SetDirty(aTarget);
			}
		}
		GUILayout.EndHorizontal();
	}
}
