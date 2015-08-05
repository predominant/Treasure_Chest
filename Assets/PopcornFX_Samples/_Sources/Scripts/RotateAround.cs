using UnityEngine;
using System.Collections;

public class RotateAround : MonoBehaviour {

	public Transform	center;
	public float		speed = 25;
	public Texture 		tex;
	
	void OnGUI()
	{
		GUI.DrawTexture(new Rect(50, 50, tex.width/2.0f, tex.height/2.0f), tex);
	}
	
	void Update () {
	
		float angle = 0.0f;

		if (Input.GetKey(KeyCode.LeftArrow))
			angle += speed;
		if (Input.GetKey(KeyCode.RightArrow))
			angle -= speed;

		this.transform.RotateAround(center.position, Vector3.up, angle * Time.deltaTime);

	}
}
