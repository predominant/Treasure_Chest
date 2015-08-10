using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;
using uGUIPanelManager;

public class uGUIMenu : Editor {

	[MenuItem ("GameObject/UI/[uGUI]AddManagedPanel")]
	static void AddMangedPanel (MenuCommand command) 
	{
		GameObject go = Selection.activeGameObject;

		if (go != null)
		{
			go.AddComponent<uGUIManagedPanel>();
		}
		else
		{
			Debug.LogError("Select a Panel first!");
		}


	}

	[MenuItem ("GameObject/UI/[uGUI]AddPanelSwitcher")]
	static void AddPanelSwitcher (MenuCommand command) 
	{
		GameObject go = Selection.activeGameObject;
		if (go != null)
		{
			go.AddComponent<uGUIPanelSwitcher>();
		}
		else
		{
			Debug.LogError("Select a Button first!");
		}
		
	}
}
