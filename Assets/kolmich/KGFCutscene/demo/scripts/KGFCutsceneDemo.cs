using UnityEngine;
using System.Collections;

public class KGFCutsceneDemo : MonoBehaviour
{
	public KGFCutscene itsCutscene;
	
	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(0,0,Screen.width,Screen.height));
		{
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Start Cutscene",GUILayout.Height(100)))
			{
				itsCutscene.StartCutscene();
			}
			
			if (GUILayout.Button("Stop Cutscene",GUILayout.Height(100)))
			{
				itsCutscene.StopCutscene();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
		}
		GUILayout.EndArea();
	}
}
