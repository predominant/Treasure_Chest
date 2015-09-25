using UnityEngine;
using System.Collections;
using System.Linq;

public class Player : MonoBehaviour
{
	protected AIPath m_Path = null;
	protected Seeker m_Seeker = null;

	void Awake()
	{
		GameObject[] playerObjs = GameObject.FindGameObjectsWithTag( "Player" );
		if( playerObjs.Length > 1 )
		{
			Debug.LogWarning( "Duplicate 'Player' detected - destroying" );
			DestroyImmediate(gameObject);
			return;
		}
		
		DontDestroyOnLoad( gameObject );
	}

	void Start()
	{
		m_Path = GetComponent<AIPath>();
		m_Seeker = GetComponent<Seeker>();
	}

	void OnLevelWasLoaded(int level)
	{
		SceneManager.SceneTransition transition = SceneManager.instance.NewestTransition;

		if( null == transition )
			return;

		// Find list of locators
		GameObject[] locators = GameObject.FindGameObjectsWithTag( "Locator" );
		if( locators.Length == 0 )
		{
			Debug.LogWarning( "No locator in scene" );
			return;
		}

		// Find a locator of this name
		GameObject locator = locators.First( (i) => { return i.name == transition.LocatorName; } );
		if( null == locator )
		{
			Debug.LogWarning( "No Locator of name " + transition.LocatorName );
			return;
		}

		transform.position = locator.transform.position;
		transform.rotation = locator.transform.rotation;

		// Reset our path follower
		m_Path.target = transform;
		m_Path.SearchPath();
	}

	public void OnTriggerEnter(Collider collider)
	{
		//switch( collider.gameObject.tag )
		//{ 
		//	case "Portal":
		//		break;
		//}
	}
}