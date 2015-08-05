using UnityEngine;
using System.Collections;

public class EffectButton : MonoBehaviour {

	public PKFxFX Fx;

	void OnGUI()
	{
		if (Fx != null)
		{
			if (GUI.Button(new Rect(10, Screen.height - 30 - 10, 90, 30), "Restart FX"))
			{
				Fx.StopEffect();
				Fx.StartEffect();
			}
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
