using UnityEngine;
using System.Collections;

public class MyCameraController : MonoBehaviour
{
	private KGFOrbitCam itsKGFOrbitCam;	//reference to the orbitcam		
	
	public void Start()
	{
		itsKGFOrbitCam = KGFAccessor.GetObject<KGFOrbitCam>();	//get the KGFOrbitcam by using the KGFAccessor class
		
		if(itsKGFOrbitCam != null)
		{
			float anOffset = itsKGFOrbitCam.GetEnviromentCollisionOffset(); //get the layermask for collisions
			Debug.Log("camera is located: "+anOffset+" units in front of the collision");
		}
	}
}
