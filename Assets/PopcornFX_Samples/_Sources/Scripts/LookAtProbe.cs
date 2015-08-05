using UnityEngine;
using System.Collections;

public class LookAtProbe : MonoBehaviour {

	GameObject probe;

	// Use this for initialization
	void Start () {
		probe = GameObject.Find("Probe(Clone)");
	}
	
	// Update is called once per frame
	void Update () {
		if (probe == null)
			probe = GameObject.Find("Probe(Clone)");
		if (probe != null)
			this.GetComponent<Camera>().transform.LookAt(probe.transform);
	}
}
