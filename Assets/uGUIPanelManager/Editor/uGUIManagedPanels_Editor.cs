using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEditor;
using uGUIPanelManager;

[CustomEditor(typeof(uGUIManagedPanel))]
public class uGUIManagedPanels_Editor : Editor
{

	protected uGUIManagedPanel panel; 
	protected uGUIManagedPanel Panel
	{
		get
		{
			if (panel == null)
			{
				panel = (uGUIManagedPanel)target;
			}
			return panel;
		}
	}


	public override void OnInspectorGUI()
	{
        uGUIManagedPanels_Editor.MangedPanelGUI( Panel );
//		DrawDefaultInspector();
		if (GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}

	}

	public static void MangedPanelGUI(uGUIManagedPanel managedPanel)
	{
		uGUIManager_Editor.GUIColor(managedPanel.panelState == PanelState.Show, Color.green, Color.white);
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label(managedPanel.name);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Show"))
		{

			managedPanel.Show();
			MoveToPanelState(managedPanel);
		}
		uGUIManager_Editor.GUIColor(managedPanel.panelState == PanelState.Hide, Color.red, Color.white);
		if (GUILayout.Button("Hide"))
		{
			managedPanel.Hide();
		}
		GUI.color = Color.white;
		GUILayout.EndHorizontal();
		GUILayout.Space(20);
		managedPanel.isMainPanel = EditorGUILayout.ToggleLeft("IsMainPanel:", managedPanel.isMainPanel);
		GUILayout.Space(20);

		GUILayout.BeginVertical("Box");
		GUILayout.Label("Animate Panel:");
		if (managedPanel.CachedAnimator == null)
		{
			if (GUILayout.Button("AddAnimator (to replace basic Animations)"))
			{
				managedPanel.CachedGameObject.AddComponent<Animator>();
				if (AssetDatabase.CopyAsset("Assets/uGUIPanelManager/DefaultAnimator/DefaultPanelController.controller", "Assets/" + managedPanel.name + "_Controller.controller"))
				{
					AssetDatabase.Refresh();
					managedPanel.CachedAnimator.runtimeAnimatorController = (RuntimeAnimatorController)AssetDatabase.LoadAssetAtPath("Assets/" + managedPanel.name + "_Controller.controller", typeof(RuntimeAnimatorController));
				}

			}
			GUILayout.BeginVertical("Box");
			GUILayout.Label("Basic Animation:");

			PanelState oldState = managedPanel.panelState;
			managedPanel.panelState = (PanelState)EditorGUILayout.EnumPopup("State:", managedPanel.panelState);
			if (oldState != managedPanel.panelState)
			{
				MoveToPanelState(managedPanel);
			}

			EditorGUI.BeginChangeCheck();
			GUILayout.BeginVertical("Box");
			GUILayout.Label("Transform:");
			managedPanel.stateTransform [(int)managedPanel.panelState].pos = EditorGUILayout.Vector2Field("Position:", managedPanel.stateTransform [(int)managedPanel.panelState].pos);
			managedPanel.stateTransform [(int)managedPanel.panelState].rot = EditorGUILayout.Vector3Field("Rotation:", managedPanel.stateTransform [(int)managedPanel.panelState].rot);
			managedPanel.stateTransform [(int)managedPanel.panelState].scale = EditorGUILayout.Vector3Field("Scale:", managedPanel.stateTransform [(int)managedPanel.panelState].scale);
			GUILayout.EndVertical();
			if (EditorGUI.EndChangeCheck())
			{
				MoveToPanelState(managedPanel);
			}

			GUILayout.BeginVertical("Box");
			GUILayout.Label("Animation Settings:");
			managedPanel.duration = EditorGUILayout.FloatField("Duration:", managedPanel.duration);
			managedPanel.curve = EditorGUILayout.CurveField("Curve:", managedPanel.curve);
			GUILayout.EndVertical();

			GUILayout.EndVertical();

		}
		else
		{
			GUILayout.BeginVertical("Box");
			GUILayout.Label("Animator Trigger Names:");

			string[] stateNames = System.Enum.GetNames(typeof(PanelState));
			for (int i = 0; i < stateNames.Length; i++)
			{
				if (managedPanel.stateTransform [i].propertyname == "")
				{
					managedPanel.stateTransform [i].propertyname = stateNames [i];
				}

				managedPanel.stateTransform [i].propertyname = EditorGUILayout.TextField(stateNames [i], managedPanel.stateTransform [i].propertyname);
			}
			GUILayout.EndVertical();
		}
		GUILayout.EndVertical();


		GUILayout.BeginVertical("Box");

		if (managedPanel.CachedAnimator != null)
		{
			GUILayout.Label("Call Methods in State:");
			managedPanel.panelState = (PanelState)EditorGUILayout.EnumPopup("State:", managedPanel.panelState);
		}
		else
		{
			GUILayout.Label("Call Methods(THIS STATE):");
		}
//		managedPanel.stateTransform [(int)managedPanel.panelState].target = (GameObject)EditorGUILayout.ObjectField(managedPanel.stateTransform [(int)managedPanel.panelState].target, typeof(GameObject), true);
//		managedPanel.stateTransform [(int)managedPanel.panelState].callBeforeEnter = EditorGUILayout.TextField("Before Show:", managedPanel.stateTransform [(int)managedPanel.panelState].callBeforeEnter);
//		managedPanel.stateTransform [(int)managedPanel.panelState].callAfterEnter = EditorGUILayout.TextField("After Show:", managedPanel.stateTransform [(int)managedPanel.panelState].callAfterEnter);
//		managedPanel.stateTransform [(int)managedPanel.panelState].callBeforeLeave = EditorGUILayout.TextField("Before Hide:", managedPanel.stateTransform [(int)managedPanel.panelState].callBeforeLeave);
		//		managedPanel.stateTransform [(int)managedPanel.panelState].callAfterLeave = EditorGUILayout.TextField("After Hide:", managedPanel.stateTransform [(int)managedPanel.panelState].callAfterLeave);


		SerializedObject sobj = new SerializedObject(managedPanel);
		SerializedProperty prop = sobj.FindProperty("_stateTransform");
		EditorGUILayout.PropertyField(prop.GetArrayElementAtIndex((int)managedPanel.panelState).FindPropertyRelative("callBeforeEnterState"));
		EditorGUILayout.PropertyField(prop.GetArrayElementAtIndex((int)managedPanel.panelState).FindPropertyRelative("callAfterEnterState"));
		EditorGUILayout.PropertyField(prop.GetArrayElementAtIndex((int)managedPanel.panelState).FindPropertyRelative("callBeforeLeaveState"));
		EditorGUILayout.PropertyField(prop.GetArrayElementAtIndex((int)managedPanel.panelState).FindPropertyRelative("callAfterLeaveState"));


		GUILayout.EndVertical();
	}

	public static void MoveToPanelState(uGUIManagedPanel managedPanel)
	{
		managedPanel.CachedRectTransform.anchoredPosition = managedPanel.stateTransform [(int)managedPanel.panelState].pos;
		managedPanel.CachedRectTransform.localRotation = Quaternion.Euler(managedPanel.stateTransform [(int)managedPanel.panelState].rot);
		managedPanel.CachedRectTransform.localScale = managedPanel.stateTransform [(int)managedPanel.panelState].scale;
	}

}
