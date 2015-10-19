using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using GameSparks;
using GameSparks.Core;
using GameSparks.Api;
using GameSparks.Api.Requests;

public class Player : MonoBehaviour
{
	public string m_StartLocatorName = "Player Start";

	protected AIPath m_Path = null;
	protected Seeker m_Seeker = null;

	#region Events
	public Action<uint> LevelChanged;
	public Action<float> ExperienceChanged;	
	public Action<string> NicknameChanged;
	public Action<float> EnergyChanged;
	public Action<float> MaxEnergyChanged;
	public Action<float> EnergyRegenChanged;
	#endregion

	#region Properties
	public uint Level			{ get { return m_Level;			} set { if( value != Level )		{ m_Level = value;			if( null != LevelChanged )			{ LevelChanged(value); }		} } }
	public float Experience		{ get { return m_Experience;	} set { if( value != Experience )	{ m_Experience = value;		if( null != ExperienceChanged )		{ ExperienceChanged(value); }	} } }
	public string Nickname		{ get { return m_Nickname;		} set { if( value != Nickname )		{ m_Nickname = value;		if( null != NicknameChanged )		{ NicknameChanged(value); }		} } }
	public float Energy			{ get { return m_Energy;		} set { if( value != Energy )		{ m_Energy = value;			if( null != EnergyChanged )			{ EnergyChanged(value); }		} } }
	public float MaxEnergy		{ get { return m_MaxEnergy;		} set { if( value != MaxEnergy )	{ m_MaxEnergy = value;		if( null != MaxEnergyChanged )		{ MaxEnergyChanged(value); }	} } }
	public float EnergyRegen	{ get { return m_EnergyRegen;	} set { if( value != EnergyRegen )	{ m_EnergyRegen = value;	if( null != EnergyRegenChanged )	{ EnergyRegenChanged(value); }	} } }
	#endregion

	#region Server Stored Data
	private uint m_Level		= 0;			// "Level": 4
	private float m_Experience	= 0f;			// "Experience"
	private string m_Nickname	= string.Empty;	// "Nickname": "The Master",
	private float m_Energy		= 100f;			// "Energy": 100,
	private float m_MaxEnergy	= 100f;			// "MaxEnergy": 100,
	private float m_EnergyRegen	= 0f;			// "EnergyRegen": 0.2,
	#endregion

	#region Unity Events
	void Awake()
	{
		GameObject[] playerObjs = GameObject.FindGameObjectsWithTag( "Player" );
		if( playerObjs.Length > 1 )
		{
			Debug.LogWarning( "Duplicate 'Player' detected - destroying" );
			Destroy(gameObject);
			return;
		}
		
		DontDestroyOnLoad( gameObject );
	}

	void Start()
	{
		m_Path = GetComponent<AIPath>();
		m_Seeker = GetComponent<Seeker>();

		// Find the start locator and set our position/orientation
		if( !string.IsNullOrEmpty( m_StartLocatorName ) )
		{
			GameObject[] locators = GameObject.FindGameObjectsWithTag( "Locator" );
			if( locators.Length > 0 )
			{
				GameObject startLocator = locators.FirstOrDefault( (i) => { return i.name == m_StartLocatorName; } );

				if( null != startLocator )
				{
					transform.position = startLocator.transform.position;
					transform.rotation = startLocator.transform.rotation;
				}
			}
		}

		Net_SyncData();
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
		GameObject locator = null;
		foreach( GameObject locObj in locators )
			if( locObj.name == transition.LocatorName )
				locator = locObj;

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
	#endregion

	#region Server Actions
	public void Net_SyncData()
	{
		new LogEventRequest()
			.SetEventKey("GET_PLAYER_INFO")
			.Send((response) =>
			{
				if (response.HasErrors)
				{
					Debug.Log("[GS] GET_PLAYER_INFO failed: " + response.Errors.JSON);
				}
				else
				{
					int? tLevel			= response.ScriptData.GetInt("Level");
					float? tExperience	= response.ScriptData.GetFloat("Experience");
					string tNickname	= response.ScriptData.GetString("Nickname");
					float? tEnergy		= response.ScriptData.GetFloat("Energy");
					float? tMaxEnergy	= response.ScriptData.GetFloat("MaxEnergy");
					float? tEnergyRegen	= response.ScriptData.GetFloat("EnergyRegen");

					Level = (uint)tLevel;
					Experience = (float)tExperience;
					Nickname = tNickname;
					Energy = (float)tEnergy;
					MaxEnergy = (float)tMaxEnergy;
					EnergyRegen = (float)EnergyRegen;
				}
			});
	}

	#endregion
}