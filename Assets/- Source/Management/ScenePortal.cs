using UnityEngine;
using System.Collections;

public class ScenePortal : MonoBehaviour
{
	public string m_To = string.Empty;
	public string m_LocatorName = string.Empty;

	SceneManager.SceneTransition ToSceneTransition()
	{
		return new SceneManager.SceneTransition( Application.loadedLevelName, m_To, m_LocatorName );
	}

	public void ChangeScene()
	{
		SceneManager.instance.ChangeScene( ToSceneTransition() );
	}

	public void OnTriggerEnter( Collider collider )
	{
		if( collider.gameObject.tag != "Player" )
			return;

		ChangeScene();
	}
}