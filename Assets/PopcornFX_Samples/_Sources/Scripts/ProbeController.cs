using UnityEngine;
using System.Collections;

public class ProbeController : MonoBehaviour {

	public Rigidbody 		rb;
	public ThrusterStart	spawner;
	private float			damage = 0.0f;

	void OnCollisionEnter(Collision cl)
	{
		if (cl.relativeVelocity.magnitude > 4.2)
			damage += cl.relativeVelocity.magnitude;
	}

	// Use this for initialization
	void Start () {
		rb = this.gameObject.GetComponent<Rigidbody>();
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (Input.GetKey("space"))
			rb.AddForce(this.transform.up * 50);
		if (Input.GetKey(KeyCode.LeftArrow))
		    rb.AddRelativeTorque(0,0,10);
	    if (Input.GetKey(KeyCode.RightArrow))
		    rb.AddRelativeTorque(0,0,-10);
		if (Input.GetKey(KeyCode.UpArrow))
			rb.AddRelativeTorque(10,0,0);
		if (Input.GetKey(KeyCode.DownArrow))
			rb.AddRelativeTorque(-10,0,0);
		if (Input.GetMouseButtonDown(0))
			damage = 10;
		if (damage > 8.0f)
		{
			GameObject newFx = Resources.Load ("BlastFX") as GameObject;
			newFx.transform.position = this.transform.position;
			Instantiate(newFx);

			spawner.Invoke("SpawnProbe", 2);
			
			this.GetComponentInChildren<PKFxFX>().StopEffect();
			Destroy(this.gameObject);
		}
	}
}
