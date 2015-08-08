using UnityEngine;
using System.Collections;

public class Light_animation : MonoBehaviour {
	public float minIntense=0.5f;
	public float maxIntense=1.2f;
	public float timeVariations=0.1f;
	private float cont=0.0f;
	private float curIntense;
	private float lastIntense;
	private float lightIntensity;
	// Use this for initialization
	void Start () {
		curIntense=GetComponent<Light>().intensity;
		lastIntense=GetComponent<Light>().intensity;
		lightIntensity=GetComponent<Light>().intensity;
	}
	
	// Update is called once per frame
	void Update () {
		if (cont < timeVariations){
			cont+=Time.deltaTime;
		}
		else{
			lastIntense=curIntense;
			curIntense=Random.Range(minIntense,maxIntense);
			cont=0f;
		}
		//LerpLight(lastIntense,curIntense);
		lightIntensity = Mathf.Lerp(lastIntense,curIntense, timeVariations*Time.deltaTime);
		GetComponent<Light>().intensity=lightIntensity;
	}
}
