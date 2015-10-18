using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(Node))]
public class NodeEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.DrawDefaultInspector();

		Node node = target as Node;

		serializedObject.Update();

		NodeData data = node.NodeData;

		////////////////////////////////////////////////////////////
		// Node Data
		////////////////////////////////////////////////////////////
		EditorGUILayout.Space();
		EditorGUILayout.LabelField( "- Node Data", EditorStyles.boldLabel );

		// Experience
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField( "Experience", EditorStyles.boldLabel );
		EditorGUILayout.LabelField( data.Experience.ToString() );
		EditorGUILayout.EndHorizontal();

		// Energy Cost
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField( "Energy Cost", EditorStyles.boldLabel );
		EditorGUILayout.LabelField( data.EnergyCost.ToString() );
		EditorGUILayout.EndHorizontal();

		// Cooldown
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField( "Cooldown", EditorStyles.boldLabel );
		EditorGUILayout.LabelField( data.Experience.ToString() );
		EditorGUILayout.EndHorizontal();

		serializedObject.ApplyModifiedProperties();
	}
}