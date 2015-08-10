using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using uGUIPanelManager;

[CustomEditor(typeof(uGUIPanelSwitcher))]
public class uGUIPanelSwitcher_Editor : Editor
{

    protected uGUIPanelSwitcher switcher; 
    protected uGUIPanelSwitcher Switcher
    {
        get
        {
            if (switcher == null)
            {
                switcher = (uGUIPanelSwitcher)target;
            }
            return switcher;
        }
    }

	
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        int index = GetPanelIndex(Switcher.panelName);

        GUILayout.Label("Panel:");
        if (index < uGUIManager.instance.managedPanels.Length)
        {
            int newindex = EditorGUILayout.Popup(index, GetPanelNameArray());
            if (index != newindex)
            {
                Switcher.panelName = uGUIManager.instance.managedPanels [newindex].name;
            }
        }

        if (Switcher.toggle)
        {
            Switcher.targetState = (PanelState)EditorGUILayout.EnumPopup("Toggle between:", Switcher.targetState);
            Switcher.toggleState = (PanelState)EditorGUILayout.EnumPopup("and:", Switcher.toggleState);
        }
        else
        {
            Switcher.targetState = (PanelState)EditorGUILayout.EnumPopup("Action:", Switcher.targetState);
        }

        Switcher.toggle = GUILayout.Toggle(Switcher.toggle, "Toggle");
        Switcher.additional = GUILayout.Toggle(Switcher.additional, "additional");
        Switcher.queued = GUILayout.Toggle(Switcher.queued, "Queued");
        Switcher.instant = GUILayout.Toggle(Switcher.instant, "Instant");

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(Switcher.gameObject);
        }

    }

    private int GetPanelIndex(string panelName)
    {
        for (int i = 0; i< uGUIManager.instance.managedPanels.Length; i++)
        {
            if (uGUIManager.instance.managedPanels [i].name == panelName)
            {
                return i;
            }
        }
        if (uGUIManager.instance.managedPanels.Length > 0)
        {
            switcher.panelName = uGUIManager.instance.managedPanels [0].name;
        }
        return 0;
    }

    private string[] GetPanelNameArray()
    {
        string[] names = new string[uGUIManager.instance.managedPanels.Length];
        for (int i = 0; i< uGUIManager.instance.managedPanels.Length; i++)
        {
            names [i] = uGUIManager.instance.managedPanels [i].name;	
        }
        return names;
    }
}
