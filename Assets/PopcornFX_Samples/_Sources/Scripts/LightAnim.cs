using UnityEngine;
using System.Collections;

public class LightAnim : MonoBehaviour {

	public Light l;
	public float maxIntensity = 2.0f;
	public float duration = 1.5f;

	private float timer = 0.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (l != null)
		{
			if (timer <= 0.2f)
				l.intensity = Mathf.Lerp(0.0f, 1.4f, timer/0.2f);
			else
				l.intensity = Random.Range(0.1f, (1.0f - timer/duration) * maxIntensity);
		}
		timer += Time.deltaTime;
	}
}
