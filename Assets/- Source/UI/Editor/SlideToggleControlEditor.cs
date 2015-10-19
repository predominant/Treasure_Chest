using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(SlideToggleControl))]
[CanEditMultipleObjects]
public class SlideToggleControlEditor : Editor {

	SerializedProperty _onToggle;
	SerializedProperty _toggleButton;
	SerializedProperty _toggleImage;
	SerializedProperty _textProxy;
	SerializedProperty _onText;
	SerializedProperty _offText;
	SerializedProperty _onToggleSprite;
	SerializedProperty _onPosition;
	SerializedProperty _offToggleSprite;
	SerializedProperty _offPosition;
	SerializedProperty _startOn;
	SerializedProperty _slideEase;
	SerializedProperty _slideTime;

	void OnEnable() {
		_toggleButton = serializedObject.FindProperty("_toggleButton");
		_toggleImage = serializedObject.FindProperty("_toggleImage");
		_textProxy = serializedObject.FindProperty("_textProxy");
		_onText = serializedObject.FindProperty("_onText");
		_offText = serializedObject.FindProperty("_offText");
		_onToggleSprite = serializedObject.FindProperty("_onToggleSprite");
		_onPosition = serializedObject.FindProperty("_onPosition");
		_offToggleSprite = serializedObject.FindProperty("_offToggleSprite");
		_offPosition = serializedObject.FindProperty("_offPosition");
		_startOn = serializedObject.FindProperty("_startOn");
		_slideEase = serializedObject.FindProperty("_slideEase");
		_slideTime = serializedObject.FindProperty("_slideTime");
	}

	public override void OnInspectorGUI() {
		serializedObject.Update();

		// Button
		if (_toggleButton.objectReferenceValue == null) {
			EditorGUILayout.LabelField("You must provide a button reference.", EditorStyles.boldLabel);
		}

		EditorGUILayout.PropertyField( _toggleButton, new GUIContent("Toggle Button") );
		if (_toggleButton.objectReferenceValue == null) { return; }
		EditorGUILayout.Space();

		// Image
		if (_toggleButton.objectReferenceValue == null) {
			EditorGUILayout.LabelField("You must provide an image reference.", EditorStyles.boldLabel);
		}

		EditorGUILayout.PropertyField(_toggleImage, new GUIContent("Toggle Image"));
		if (_toggleImage.objectReferenceValue == null) { return; }
		EditorGUILayout.Space();

		// Text
		EditorGUILayout.PropertyField( _textProxy, new GUIContent("State Proxy Text") );
		if (_textProxy.objectReferenceValue != null) {
			EditorGUILayout.PropertyField(_onText, new GUIContent("On Text"));
			EditorGUILayout.PropertyField(_offText, new GUIContent("Off Text"));
		}

		EditorGUILayout.Space();

		// Sprites
		if (_onToggleSprite.objectReferenceValue == null || _offToggleSprite.objectReferenceValue == null) {
			EditorGUILayout.LabelField("You must provide both on and off state sprites.", EditorStyles.boldLabel);
		}

		EditorGUILayout.PropertyField( _onToggleSprite, new GUIContent("On Sprite") );
		EditorGUILayout.PropertyField( _offToggleSprite, new GUIContent("Off Sprite") );
		if (_onToggleSprite.objectReferenceValue == null || _offToggleSprite.objectReferenceValue == null) { return; }
		EditorGUILayout.Space();

		// Toggle State
		EditorGUILayout.PropertyField( _startOn, new GUIContent("Start On") );
		//EditorGUILayout.PropertyField( _onToggle, new GUIContent("On Toggle Event") );
		EditorGUILayout.Space();

		// Positions
		_onPosition.vector3Value = EditorGUILayout.Vector3Field("On Position", _onPosition.vector3Value, new GUILayoutOption[] {} );
		_offPosition.vector3Value = EditorGUILayout.Vector3Field("Off Position", _offPosition.vector3Value, new GUILayoutOption[] {} );
		EditorGUILayout.Space();

		// Tweening
		EditorGUILayout.PropertyField( _slideEase, new GUIContent("Ease") );
		EditorGUILayout.PropertyField( _slideTime, new GUIContent("Time") );

		serializedObject.ApplyModifiedProperties();
	}
}