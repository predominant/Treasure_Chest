using UnityEngine;
using System.Collections;

public class TPSlider : MonoBehaviour {

	float 			slider1Value = 0.5f;
	float			slider0Value = 1.0f;
	public PKFxFX	TP;	

	void OnGUI()
	{
		GUI.Label(new Rect(10, Screen.height / 2 - (2 * 30), Screen.width / 4, Screen.height / 10), "Global power :");
		slider0Value = GUI.HorizontalSlider(new Rect(10, Screen.height / 2 - 30, Screen.width / 4, 30), slider0Value, 0.0f, 1.0f);
		GUI.Label(new Rect(10, Screen.height / 2, Screen.width / 4, Screen.height / 10), "Startup animation :");
		slider1Value = GUI.HorizontalSlider(new Rect(10, Screen.height / 2 + 30, Screen.width / 4, 30), slider1Value, 0.0f, 3.0f);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (slider1Value <= 1.0f)
		{
			TP.SetAttribute(new PKFxManager.Attribute("TeleportStartup", slider0Value));
			TP.SetAttribute(new PKFxManager.Attribute("TeleportStartup1", slider1Value));
			TP.SetAttribute(new PKFxManager.Attribute("TeleportStartup2", 0));
			TP.SetAttribute(new PKFxManager.Attribute("TeleportStartup3", 0));
		}
		else if (slider1Value <= 2.0f)
		{
			TP.SetAttribute(new PKFxManager.Attribute("TeleportStartup", slider0Value));
			TP.SetAttribute(new PKFxManager.Attribute("TeleportStartup1", 1));
			TP.SetAttribute(new PKFxManager.Attribute("TeleportStartup2", slider1Value - 1.0f));
			TP.SetAttribute(new PKFxManager.Attribute("TeleportStartup3", 0));
		}
		else if (slider1Value <= 3.0f)
		{
			TP.SetAttribute(new PKFxManager.Attribute("TeleportStartup", slider0Value));
			TP.SetAttribute(new PKFxManager.Attribute("TeleportStartup1", 1));
			TP.SetAttribute(new PKFxManager.Attribute("TeleportStartup2", 1));
			TP.SetAttribute(new PKFxManager.Attribute("TeleportStartup3", slider1Value - 2.0f));
		}
	}
}
