using UnityEngine;
using System.Collections;

public class PlayerCameraBinding : MonoBehaviour
{
	protected KGFOrbitCam m_CameraSettings = null;
	protected Transform m_CameraLocator = null;

	void Start()
	{
		m_CameraSettings = GetComponent<KGFOrbitCam>();
		GameObject player = GameObject.FindGameObjectWithTag( "Player" );
		m_CameraLocator = player.transform.Find( "CameraLocator" );

		m_CameraSettings.SetTarget( m_CameraLocator.gameObject );
	}
}