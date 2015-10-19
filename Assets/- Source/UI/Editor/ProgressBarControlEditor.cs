using UnityEditor;
using UnityEngine;
using System.Collections;
using DG.Tweening;

[CustomEditor(typeof(ProgressBarControl))]
[CanEditMultipleObjects]
public class ProgressBarControlEditor : Editor
{

	SerializedProperty _fillBar;
	SerializedProperty _orientation;
	SerializedProperty _easeType;
	SerializedProperty _easeTime;
	SerializedProperty _scale;
	SerializedProperty _track;
	SerializedProperty _minWidth;
	SerializedProperty _maxWidth;
	SerializedProperty _minHeight;
	SerializedProperty _maxHeight;
	SerializedProperty _scalingTween;
	SerializedProperty _valueCur;
	SerializedProperty _valueMin;
	SerializedProperty _valueMax;
	SerializedProperty _zeroAdjustedValue;
	SerializedProperty _inverseRangeRatio;

	SerializedProperty _useMarginWidth;
	SerializedProperty _useMarginHeight;
	SerializedProperty _marginWidthMin;
	SerializedProperty _marginWidthMax;
	SerializedProperty _marginHeightMin;
	SerializedProperty _marginHeightMax;

	void OnEnable()
	{
		_fillBar = serializedObject.FindProperty("_fillBar");
		_orientation = serializedObject.FindProperty("_orientation");
		_easeType = serializedObject.FindProperty("_easeType");
		_easeTime = serializedObject.FindProperty("_easeTime");
		_track = serializedObject.FindProperty("_track");
		_minWidth = serializedObject.FindProperty("_minWidth");
		_maxWidth = serializedObject.FindProperty("_maxWidth");
		_minHeight = serializedObject.FindProperty("_minHeight");
		_maxHeight = serializedObject.FindProperty("_maxHeight");
		_valueCur = serializedObject.FindProperty("_valueCur");
		_valueMin = serializedObject.FindProperty("_valueMin");
		_valueMax = serializedObject.FindProperty("_valueMax");

		_useMarginWidth = serializedObject.FindProperty("_useMarginWidth");
		_useMarginHeight = serializedObject.FindProperty("_useMarginHeight");
		_marginWidthMin = serializedObject.FindProperty("_marginWidthMin");
		_marginWidthMax = serializedObject.FindProperty("_marginWidthMax");
		_marginHeightMin = serializedObject.FindProperty("_marginHeightMin");
		_marginHeightMax = serializedObject.FindProperty("_marginHeightMax");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		// Track Bar
		if (_track.objectReferenceValue == null)
		{
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("You must provide a track bar reference.", EditorStyles.boldLabel);
			EditorGUILayout.LabelField("This is the 'empty' area filled up by the fill bar.", EditorStyles.boldLabel);
		}

		EditorGUILayout.PropertyField(_track, new GUIContent("Track Bar"));
		if (_track.objectReferenceValue == null)
		{
			serializedObject.ApplyModifiedProperties();
			return;
		}

		EditorGUILayout.Space();

		// Fill Bar
		if (_fillBar.objectReferenceValue == null)
		{
			EditorGUILayout.LabelField("You must provide a fill bar reference. This is the element which will be scaled", EditorStyles.boldLabel);
		}

		EditorGUILayout.PropertyField(_fillBar, new GUIContent("Fill Bar"));
		if (_fillBar.objectReferenceValue == null)
		{
			serializedObject.ApplyModifiedProperties();
			return;
		}

		EditorGUILayout.Space();

		EditorGUILayout.LabelField("Bar Configuration", EditorStyles.boldLabel);
		//EditorGUILayout.PropertyField(_orientation, new GUIContent("Orientation"));
		EditorGUILayout.PropertyField(_easeType, new GUIContent("Ease Type"));
		EditorGUILayout.PropertyField(_easeTime, new GUIContent("Ease Time"));
		EditorGUILayout.Space();

		EditorGUILayout.LabelField("Sizing", EditorStyles.boldLabel);

		_useMarginWidth.boolValue = EditorGUILayout.Toggle(new GUIContent("Use Margin Width"), _useMarginWidth.boolValue);
		if (_useMarginWidth.boolValue)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PropertyField(_marginWidthMin, new GUIContent("Margin Width Min"));
			EditorGUILayout.PropertyField(_marginWidthMax, new GUIContent("Margin Width Max"));
			EditorGUILayout.EndHorizontal();
		}

		_useMarginHeight.boolValue = EditorGUILayout.Toggle(new GUIContent("Use Margin Height"), _useMarginHeight.boolValue);
		if (_useMarginHeight.boolValue)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PropertyField(_marginHeightMin, new GUIContent("Margin Height Min"));
			EditorGUILayout.PropertyField(_marginHeightMax, new GUIContent("Margin Height Max"));
			EditorGUILayout.EndHorizontal();
		}

		// Populate bounds based on track bar size and fill margins
		if (_useMarginWidth.boolValue)
		{
			_minWidth.floatValue = _marginWidthMin.floatValue;
			_maxWidth.floatValue = (_track.objectReferenceValue as RectTransform).rect.width - _marginWidthMax.floatValue;
		}
		else
		{
			_minWidth.floatValue = 0f;
			_maxWidth.floatValue = (_fillBar.objectReferenceValue as RectTransform).rect.width;
		}

		if (_useMarginHeight.boolValue)
		{
			_minHeight.floatValue = _marginHeightMin.floatValue;
			_maxHeight.floatValue = (_track.objectReferenceValue as RectTransform).rect.height + _marginHeightMax.floatValue;
		}
		else
		{
			_minHeight.floatValue = 0f;
			_maxHeight.floatValue = (_fillBar.objectReferenceValue as RectTransform).rect.height;
		}

		EditorGUILayout.Space();


		EditorGUILayout.LabelField("Value Settings", EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PropertyField(_valueMin, new GUIContent("Min Value"));
		EditorGUILayout.PropertyField(_valueMax, new GUIContent("Max Value"));
		EditorGUILayout.EndHorizontal();
		_valueCur.floatValue = EditorGUILayout.Slider(new GUIContent("Current Value"), _valueCur.floatValue, _valueMin.floatValue, _valueMax.floatValue);

		serializedObject.ApplyModifiedProperties();
	}
}