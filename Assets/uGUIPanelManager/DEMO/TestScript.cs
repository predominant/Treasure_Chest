using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void CallBeforeShow()
	{
		Debug.Log ("CallBeforeShow");
	}

	public void CallAfterShow()
	{
		Debug.Log ("CallAfterShow");
	}

	public void CallBeforeHide()
	{
		Debug.Log ("CallBeforeHide");
	}

	public void CallAfterHide()
	{
		Debug.Log ("CallAfterHide");
	}
}
