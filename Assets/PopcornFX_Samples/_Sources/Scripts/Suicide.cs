using UnityEngine;
using System.Collections;

public class Suicide : MonoBehaviour {

	public float life = 5.0f;
	private float timer = 0.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if (timer >= life)
		{
			Destroy(this.gameObject);
		}
	}
}
