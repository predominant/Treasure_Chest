using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(EMTransition))]
public class EMTransitionEditor : Editor
{
	EMTransition _target;
	
	[MenuItem ("GameObject/UI/Easy Masking Transition")]
	static void EMTransition ()
	{
		string[] guids = AssetDatabase.FindAssets("Easy Masking Transition t:prefab");
		if(guids.Length <= 0) Debug.Log("The prefab called Easy Masking Transition was not found");
		else
		{
			string path = AssetDatabase.GUIDToAssetPath(guids[0]);
			Object prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));

			var canvas = FindCanvas();
			if (canvas == null) return;

			GameObject newEMTransition = (GameObject)Instantiate(prefab, Vector3.zero, Quaternion.identity);

			if(newEMTransition != null)
			{
				newEMTransition.name = "Easy Masking Transition";
				newEMTransition.transform.SetParent(canvas.transform);
				newEMTransition.transform.localScale = Vector3.one;      
				newEMTransition.GetComponent<RectTransform>().sizeDelta = new Vector2(1920.0f, 1080.0f);
				newEMTransition.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
				Selection.activeGameObject = newEMTransition;
			}
		}
	}

	void OnEnable()
	{
		_target = (EMTransition)target;
	}
	
	public override void OnInspectorGUI(){
		
		serializedObject.Update();

		_target.gradationTexture = (Texture2D)EditorGUILayout.ObjectField("Gradation Texture", _target.gradationTexture, typeof(Texture2D), false, GUILayout.Height(96));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_duration"), new GUIContent("Duration"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_playOnAwake"), new GUIContent("Play On Awake"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_flipAfterAnimation"), new GUIContent("Flip After Animation"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_flip"), new GUIContent("Flip"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_invert"), new GUIContent("Invert"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ignoreTimeScale"), new GUIContent("Ignore Time Scale"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_pingPong"), new GUIContent("Preview Mode"));

		EditorGUILayout.BeginHorizontal();
		_target.curve = EditorGUILayout.CurveField("Transition Curve", _target.curve);
		if(GUILayout.Button("Flip", GUILayout.Width(50)))
		{
			_target.FlipAnimationCurve();
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_threshold"), new GUIContent("Threshold"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("onTransitionStart"), new GUIContent("On Transition Start"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("onTransitionComplete"), new GUIContent("On Transition Complete"));

		if(GUI.changed)
		{
			EditorUtility.SetDirty (target);
		}

		serializedObject.ApplyModifiedProperties();
	}

	// Find Canvas in this project
	private static Canvas FindCanvas() {
		Canvas canvas;
		if (Selection.activeObject is GameObject) {
			canvas = ((GameObject) Selection.activeObject).GetComponent<Canvas>();
			if (canvas != null) {
				return canvas;
			}
		}
		
		canvas = GameObject.FindObjectOfType<Canvas>();
		if (canvas == null) {
			EditorUtility.DisplayDialog("Canvas not found!",
			                            "There's no Canvas in this scene. Please create new canvas using the menu item:\n\n"
			                            + "GameObject -> UI -> Canvas", "OK");
		}
		
		return canvas;
	}

}