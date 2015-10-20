using UnityEngine;
using System.Collections;
using System;
using GameSparks;
using GameSparks.Core;
using GameSparks.Api;
using GameSparks.Api.Requests;

#region Data Structures
public struct NodeData 
{
	public float Experience;
	public int EnergyCost;
	public float Cooldown;	// Minutes

	public long CooldownTicks { get { return (int)(Cooldown * 60 * 1000 * 10000); } } // Minutes -> Seconds -> Milliseconds -> 100 nanos (DateTime tick)
}
#endregion

public abstract class Node : MonoBehaviour, Interactable
{
	#region Enums
	public enum Type
	{
		MiningOre = 0,
		MiningQuarry,
		Crop,
	}
	#endregion

	#region Events
	public event Action AvailableTimeChanged;
	#endregion

	#region Properties
	Transform Interactable.InteractLocator { get { return m_InteractLocator; } }

	public abstract Job.Type JobType { get; }

	public abstract string NodeTypeName { get; }
	
	public bool IsAvailable { get { return DateTime.UtcNow >= m_AvailableTime; } }

	public DateTime AvailableTime
	{
		get { return m_AvailableTime; }
		set
		{
			if( m_AvailableTime == value )
				return;

			m_AvailableTime = value;
			if( null != AvailableTimeChanged )
				AvailableTimeChanged();
		}
	}

	public float SecondsToAvailable
	{
		get { return (float)(AvailableTime - DateTime.UtcNow).TotalSeconds; }
	}

	public NodeData NodeData { get { return m_NodeData; } }
	#endregion

	#region Public Variables
	// Server node UID for this node group
	public int m_ServerGroupNodeUID = 0;
	public DateTime m_AvailableTime = DateTime.MinValue;
	public Transform m_InteractLocator = null;
	#endregion

	#region Private Variables
	protected bool m_AwaitingResponse = false;
	protected NodeData m_NodeData;
	#endregion

	#region Overrides
	void Interactable.HandleInteraction()
	{
		if (!IsAvailable)
			return;

		if (GS.Available)
		{
			if (m_AwaitingResponse)
				return;

			m_AwaitingResponse = true;

			new LogEventRequest()
				.SetEventKey("NODE_COLLECT")
				.SetEventAttribute("JobType", JobType.ToString() )
				.SetEventAttribute("NodeType", NodeTypeName )
				.Send((response) =>
				{
					if (response.HasErrors)
					{
						Debug.Log("Node collection net request failed");
					}
					else
					{
						string itemType = response.ScriptData.GetString("Type");
						int? itemAmt = response.ScriptData.GetInt("Amount");
						int? expAmt = response.ScriptData.GetInt("Experience");
						DateTime collectTime = DateTime.UtcNow;
						AvailableTime = collectTime.AddTicks( m_NodeData.CooldownTicks );

						Debug.Log( "Item: " + itemType.ToString() + ", Yield: " + itemAmt + ", Experience: " + expAmt );
						Debug.Log( "Next collection time in " + (m_AvailableTime - collectTime).ToString() );

						Player.instance.Exp += (float)expAmt;
					}

					m_AwaitingResponse = false;
				});
		}
	}
	#endregion

	#region Unity Events
	public void Start()
	{
		ServerDataProxy.GetNodeData( JobType, NodeTypeName, (d) => OnDataReceived(d) );
	}
	#endregion

	#region Server Events
	void OnDataReceived( NodeData data )
	{
		m_NodeData = data;
	}
	#endregion
}