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
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
            EditorUtility.SetDirty(Manager.Settings);
        }
    }

    public void ShowPanelsEdit()
    {
        foreach (uGUIManagedPanel mp in Manager.managedPanels)
        {

            GUILayout.BeginVertical("Box");
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (mp.panelState == PanelState.Show)
            {
                GUI.contentColor = Color.green;
            }
            GUILayout.Label(mp.name);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUI.contentColor = Color.white;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Settings", GUILayout.Width(70)))
            {
                Selection.activeGameObject = mp.CachedGameObject;
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Show", GUILayout.Width(50)))
            {
                mp.panelState = PanelState.Show;
                uGUIManagedPanels_Editor.MoveToPanelState(mp);
            }
            GUILayout.Space(10);
            if (GUILayout.Button("Hide", GUILayout.Width(50)))
            {
                mp.panelState = PanelState.Hide;
                uGUIManagedPanels_Editor.MoveToPanelState(mp);
            }
            GUILayout.EndHorizontal();

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
