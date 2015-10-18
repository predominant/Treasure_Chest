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
	public float Cooldown;
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

	#region Properties
	public abstract Job.Type JobType { get; }

	public abstract string NodeTypeName { get; }
	
	public bool IsAvailable { get { return DateTime.UtcNow >= m_AvailableTime; } }
	public NodeData NodeData { get { return m_NodeData; } }
	#endregion

	#region Public Variables
	// Server node UID for this node group
	public int m_ServerGroupNodeUID = 0;
	public DateTime m_AvailableTime = DateTime.MinValue;
	#endregion

	#region Private Variables
	protected bool m_AwaitingResponse = false;
	protected NodeData m_NodeData;
	#endregion

	#region Overrides
	public virtual void HandleInteraction()
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
				.SetEventAttribute("SubType", NodeTypeName )
				.Send((response) =>
				{
					if (response.HasErrors)
						Debug.Log("Node collection net request failed");
					else
					{
						int? itemAmt = response.ScriptData.GetInt("Amount");
						int? itemType = response.ScriptData.GetInt("Type");
						int? expAmt = response.ScriptData.GetInt("Experience");

						Debug.Log("Item: " + ((Item.Type)(itemType)).ToString() + ", Yield: " + itemAmt + ", Experience: " + expAmt);
						GameObject.Destroy(this.gameObject);
					}
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