using UnityEngine;
using System.Collections;

public class ThrusterStart : MonoBehaviour {

	public Texture 		tex1;
	public Texture 		tex2;
	public Texture 		tex3;

	void OnGUI()
	{
		GUI.DrawTexture(new Rect(50, 50, tex1.width/2.0f, tex1.height/2.0f), tex1);
		GUI.DrawTexture(new Rect(50 + tex1.width/2.0f, 50, tex2.width/2.0f, tex2.height/2.0f), tex2);
		GUI.DrawTexture(new Rect(50, 50 + tex1.height/2, tex3.width/2.0f, tex3.height/2.0f), tex3);
	}

	void SpawnProbe()
	{
		GameObject newProbe = Resources.Load ("Probe") as GameObject;
		newProbe.transform.position = new Vector3(0,10,0);
		newProbe.GetComponent<ProbeController>().spawner = this;
		Instantiate(newProbe);
	}

	void Start () {
		SpawnProbe();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
