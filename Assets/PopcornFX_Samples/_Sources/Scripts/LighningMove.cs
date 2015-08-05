using UnityEngine;
using System.Collections;

public class LighningMove : MonoBehaviour {

	public GameObject 	movableObject;
	public float		speed = 10.0f;
	public Texture 		tex;
	
	void OnGUI()
	{
		GUI.DrawTexture(new Rect(50, 50, tex.width/2.0f, tex.height/2.0f), tex);
	}

	void Update () {

		Vector3 translation = Vector3.zero;

		if (Input.GetKey(KeyCode.LeftArrow))
			translation += new Vector3(-1, 0,0);
		if (Input.GetKey(KeyCode.RightArrow))
			translation += new Vector3(1, 0,0);
		if (Input.GetKey(KeyCode.UpArrow))
			translation += new Vector3(0, 1,0);
		if (Input.GetKey(KeyCode.DownArrow))
			translation += new Vector3(0, -1,0);

		translation = translation.normalized;

		if (movableObject != null)
		{
			movableObject.transform.Translate(translation * speed * Time.deltaTime);
		}
	}
}
