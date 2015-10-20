using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using GameSparks;
using GameSparks.Core;
using GameSparks.Api;
using GameSparks.Api.Requests;
using Devdog.InventorySystem;

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
	public float Exp			{ get { return m_Exp;			} set { if( value != Exp )			{ SetExperience(value);		if( null != ExperienceChanged )		{ ExperienceChanged(value); }	} } }
	public string Nickname		{ get { return m_Nickname;		} set { if( value != Nickname )		{ m_Nickname = value;		if( null != NicknameChanged )		{ NicknameChanged(value); }		} } }
	public float Energy			{ get { return m_Energy;		} set { if( value != Energy )		{ m_Energy = value;			if( null != EnergyChanged )			{ EnergyChanged(value); }		} } }
	public float MaxEnergy		{ get { return m_MaxEnergy;		} set { if( value != MaxEnergy )	{ m_MaxEnergy = value;		if( null != MaxEnergyChanged )		{ MaxEnergyChanged(value); }	} } }
	public float EnergyRegen	{ get { return m_EnergyRegen;	} set { if( value != EnergyRegen )	{ m_EnergyRegen = value;	if( null != EnergyRegenChanged )	{ EnergyRegenChanged(value); }	} } }
	#endregion

	#region Server Stored Data
	private uint m_Level		= uint.MaxValue;	// "Level": 4
	private float m_Exp	= -1f;				// "Experience"
	private string m_Nickname	= string.Empty;		// "Nickname": "The Master",
	private float m_Energy		= -1f;				// "Energy": 100,
	private float m_MaxEnergy	= -1f;				// "MaxEnergy": 100,
	private float m_EnergyRegen	= -1f;				// "EnergyRegen": 0.2,
	#endregion

	#region Public Variables
	public static Player instance = null;
	#endregion

	#region Protected Variables
	public Inventory m_Inventory = null;
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
		instance = this;
	}

	void Start()
	{
		GameObject goItemManager = GameObject.FindGameObjectsWithTag("GameManager").FirstOrDefault( (go) => go.name == "Item Manager" );
		if( null == goItemManager )
			Debug.LogError( "Failed to find Item Manager game object");
		else
			m_Inventory = new Inventory( goItemManager.GetComponent<InventoryManager>() );

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
	}

	void OnLevelWasLoaded(int level)
	{
		Net_SyncData();

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

	void FixedUpdate()
	{
		Energy += Time.fixedDeltaTime * EnergyRegen;
		Energy = Mathf.Clamp( Energy, 0f, MaxEnergy );
	}
	#endregion

	#region Mutators
	private void SetExperience(float exp)
	{
		// Try a simple next-level search (most cases)
		if( exp > m_Exp &&
			Level < Experience.Table.Length - 1 &&
			exp > Experience.Table[Level] &&
			exp < Experience.Table[Level+1]
			)
		{
			m_Exp = exp;
			Level += 1;
			return;
		}

		m_Exp = exp;

		// Is the exp zero'd out?
		if( m_Exp == 0f )
			Level = 0;
		else
		{
			if( m_Exp > Experience.Table.Last() )
				Level = (uint)(Experience.Table.Length - 1);
			else
				for( int i = 0; i < Experience.Table.Length; ++i )
					if( Experience.Table[i] > m_Exp )
					{
						Level = (uint)i;
						break;
					}
		}
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
					Exp = (float)tExperience;
					Nickname = tNickname;
					Energy = (float)tEnergy;
					MaxEnergy = (float)tMaxEnergy;
					EnergyRegen = (float)tEnergyRegen;
				}
			});
	}
	#endregion
}