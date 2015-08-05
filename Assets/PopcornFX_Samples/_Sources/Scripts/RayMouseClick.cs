using UnityEngine;
using System.Collections;

public class RayMouseClick : MonoBehaviour {

	public Texture tex;

	void OnGUI()
	{
		GUI.DrawTexture(new Rect(50, 50, tex.width/2.0f, tex.height/2.0f), tex);
	}

	void Update () {
		if (Input.GetMouseButtonDown(0)) 
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit = new RaycastHit();
			if(Physics.Raycast(ray,out hit, 500))
			{
				GameObject newFx = Resources.Load ("BlastFX") as GameObject;
				newFx.transform.position = hit.point;
				Instantiate(newFx);
			}
		}
	}
}
