using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using GameSparks;
using GameSparks.Core;
using GameSparks.Api;
using GameSparks.Api.Requests;

public static class ServerDataProxy
{
	// Cached server information on node data entries
	// [JobType][NodeTypeName]
	private static Dictionary<Job.Type, Dictionary<string, NodeData>> m_NodeDataEntries = new Dictionary<Job.Type, Dictionary<string, NodeData>>();

	// Node info request callbacks
	// TODO: Something better than this
	private static Dictionary<Job.Type, Dictionary<string, List<Action<NodeData>>>> m_NodeDataRequestCallbacks = new Dictionary<Job.Type,Dictionary<string,List<Action<NodeData>>>>();

	public static void GetNodeData( Job.Type _jobType, string _nodeTypeName, Action<NodeData> _callback )
	{
		Dictionary<string, NodeData> nodeTypeContainer = null;
		
		if( !m_NodeDataEntries.TryGetValue( _jobType, out nodeTypeContainer ) )
		{
			nodeTypeContainer = new Dictionary<string, NodeData>();
			m_NodeDataEntries.Add(_jobType, nodeTypeContainer );
		}

		NodeData nodeData;
		if( nodeTypeContainer.TryGetValue( _nodeTypeName, out nodeData ) )
			_callback(nodeData);

		else
		{
			// Determine if we are currently requesting this node type data by if it has an active entry in the request cb container
			bool isRequesting = false;
			if (m_NodeDataRequestCallbacks.ContainsKey(_jobType))
				if (m_NodeDataRequestCallbacks[_jobType].ContainsKey(_nodeTypeName))
					isRequesting = true;

			// If we are currently requesting, we can simply add our cb to this list
			if( isRequesting )
				m_NodeDataRequestCallbacks[_jobType][_nodeTypeName].Add(_callback);

			// If we are not currently requesting, we have to check if the node type exists. Add if not.
			// We know that the node subtype cannot exist or it would pass the isRequesting check, so we add by default.
			// All that's left now is to register ourselves to the cb list and request data
			else
			{
				if (!m_NodeDataRequestCallbacks.ContainsKey(_jobType))
					m_NodeDataRequestCallbacks.Add(_jobType, new Dictionary<string, List<Action<NodeData>>>());

				m_NodeDataRequestCallbacks[_jobType].Add(_nodeTypeName, new List<Action<NodeData>>());
				m_NodeDataRequestCallbacks[_jobType][_nodeTypeName].Add(_callback);

				Net_RequestNodeData(_jobType, _nodeTypeName);
			}
		}
	}

	private static void Net_RequestNodeData( Job.Type _jobType, string _nodeTypeName )
	{
		if( !GS.Available )
		{
			Debug.LogError("!!Unhandled behaviour!! Requesting node data without an active GS connection" );
		}

		new LogEventRequest()
			.SetEventKey("NODE_DATA")
			.SetEventAttribute("JobType", _jobType.ToString())
			.SetEventAttribute("NodeType", _nodeTypeName)
			.Send((response) =>
			{
				if (response.HasErrors)
					Debug.Log("Node info net request failed: " + response.Errors.JSON);
				else
				{
					NodeData data;

					int? experience = response.ScriptData.GetInt("Experience");
					int? energyCost = response.ScriptData.GetInt("EnergyCost");
					float? cooldown = response.ScriptData.GetFloat("Cooldown");

					Debug.Log("Node Data [" + (_jobType).ToString() + "][" + _nodeTypeName + "]: Experience: " + experience + ", EnergyCost: " + energyCost + ", Cooldown: " + cooldown);

					data.Experience = (int)experience;
					data.EnergyCost = (int)energyCost;
					data.Cooldown = (float)cooldown;

					// Cache this node data information
					m_NodeDataEntries[_jobType][_nodeTypeName] = data;

					// Fire this information to all of our callbacks
					m_NodeDataRequestCallbacks[_jobType][_nodeTypeName].ForEach( cb => cb(data) );
				}
			});
	}
}