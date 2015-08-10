using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;

namespace UnityEditor.UI
{
	[CanEditMultipleObjects, CustomEditor(typeof(UILoadingBar), true)]
	public class UILoadingBarEditor : Editor {
		
		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			
			EditorGUILayout.Separator();
			EditorGUILayout.PropertyField(base.serializedObject.FindProperty("m_Type"), new GUIContent("Type"), true);
			if ((UILoadingBar.Type)base.serializedObject.FindProperty("m_Type").enumValueIndex == UILoadingBar.Type.Normal)
			{
				EditorGUILayout.PropertyField(base.serializedObject.FindProperty("m_TargetImage"), new GUIContent("Target Image"), true);
			}
			else
			{
				EditorGUILayout.PropertyField(base.serializedObject.FindProperty("m_TargetTransform"), new GUIContent("Target Transform"), true);
				EditorGUILayout.PropertyField(base.serializedObject.FindProperty("m_TransformWidth"), new GUIContent("Max Width"), true);
			}
			
			EditorGUILayout.Separator();
			EditorGUILayout.PropertyField(base.serializedObject.FindProperty("m_TextComponent"), new GUIContent("Target Text"), true);
			
			EditorGUILayout.Separator();
			EditorGUILayout.PropertyField(base.serializedObject.FindProperty("m_IsDemo"), new GUIContent("Is Demo"), true);
			if (base.serializedObject.FindProperty("m_IsDemo").boolValue)
			{
				EditorGUILayout.PropertyField(base.serializedObject.FindProperty("m_Duration"), new GUIContent("Tween Duration"), true);
				EditorGUILayout.PropertyField(base.serializedObject.FindProperty("m_LoadScene"), new GUIContent("Load Scene"), true);
			}
			
			EditorGUILayout.Separator();
			EditorGUILayout.PropertyField(base.serializedObject.FindProperty("m_FillAmount"), new GUIContent("Fill Amount"), true);
			
			EditorGUILayout.Separator();
			EditorGUILayout.PropertyField(base.serializedObject.FindProperty("m_OnChange"), new GUIContent("On Change"), true);
			
			base.serializedObject.ApplyModifiedProperties();
		}
	}
}