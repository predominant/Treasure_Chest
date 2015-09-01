using UnityEngine;
using System.Collections;
using UnityEditor;
using uGUIPanelManager;

[CustomEditor(typeof(uGUIManager))]
public class uGUIManager_Editor : Editor
{

	protected uGUIManager manager; 
	protected uGUIManager Manager
	{
		get
		{
			if (manager == null)
			{
				manager = (uGUIManager)target;
			}
			return manager;
		}
	}
	
	
	
	public virtual void Awake()
	{


	}


	
	public override void OnInspectorGUI()
	{
		if (GUILayout.Button("Search Managed Panels"))
		{
			Manager.SearchPaneles();
		}

		Manager.Settings.useFixedDeltaTime = EditorGUILayout.Toggle("use fixed deltaTime:", Manager.Settings.useFixedDeltaTime);
		Manager.Settings.showMainPanelInstantOnSceneLoad = EditorGUILayout.Toggle("show main panel instant on load scene:", Manager.Settings.showMainPanelInstantOnSceneLoad);

		ShowPanelsEdit();
	}

	public void ShowPanelsEdit()
	{
		foreach (uGUIManagedPanel mp in Manager.managedPanels)
		{
			GUILayout.BeginVertical("Box");
			bool oldState = mp.panelState != PanelState.Hide || mp.CachedGameObject.activeSelf;
			GUILayout.BeginHorizontal();
			GUILayout.Space(10);
			bool newState = EditorGUILayout.Foldout((mp.panelState != PanelState.Hide || mp.CachedGameObject.activeSelf), mp.name);

			GUILayout.EndHorizontal();

			if (oldState != newState)
			{
				if (newState)
				{
					mp.CachedGameObject.SetActive(true);
					mp.panelState = PanelState.Show;
				}
				else
				{
					mp.CachedGameObject.SetActive(false);
				}
				uGUIManagedPanels_Editor.MoveToPanelState(mp);

			}

			if (newState)
			{
				uGUIManagedPanels_Editor.MangedPanelGUI(mp);
			}

			GUILayout.EndVertical();

		}
	}


	public static void GUIColor(bool state, Color ifTrue, Color ifFalse)
	{
		if (state)
		{
			GUI.color = ifTrue;
		}
		else
		{
			GUI.color = ifFalse;
		}
	}
}
