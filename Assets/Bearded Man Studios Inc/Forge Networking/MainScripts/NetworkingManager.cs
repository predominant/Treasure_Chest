/*-----------------------------+------------------------------\
|                                                             |
|                        !!!NOTICE!!!                         |
|                                                             |
|  These libraries are under heavy development so they are    |
|  subject to make many changes as development continues.     |
|  For this reason, the libraries may not be well commented.  |
|  THANK YOU for supporting forge with all your feedback      |
|  suggestions, bug reports and comments!                     |
|                                                             |
|                               - The Forge Team              |
|                                 Bearded Man Studios, Inc.   |
|                                                             |
|  This source code, project files, and associated files are  |
|  copyrighted by Bearded Man Studios, Inc. (2012-2015) and   |
|  may not be redistributed without written permission.       |
|                                                             |
\------------------------------+-----------------------------*/



using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;

namespace BeardedManStudios.Network
{
	public class NetworkingManager : SimpleNetworkedMonoBehavior
	{
		/// <summary>
		/// The controlling socket of this NetworkingManager
		/// </summary>
		public static NetWorker ControllingSocket { get; set; }

		private static NetworkingManager instance = null;
		public static NetworkingManager SubtleInstance { get { return instance; } }

		/// <summary>
		/// The instance of the NetworkingManager
		/// </summary>
		public static NetworkingManager Instance { get { return instance; } private set { instance = value; } }

		/// <summary>
		/// All game objects to be instantiated whenever necessary
		/// </summary>
		public GameObject[] networkInstantiates = null;

		/// <summary>
		/// The list of all setup actions for when an object is instantiated
		/// </summary>
		public static List<System.Action> setupActions = new List<System.Action>();

		/// <summary>
		/// This is a list of the players that the client can access
		/// </summary>
		public List<NetworkingPlayer> Players { get { return OwningNetWorker.Players; } }

		private BMSByte playerPollData = new BMSByte();
		private Action<List<NetworkingPlayer>> pollPlayersCallback = null;

		private float previousTime = 0.0f;
		private float serverTime = 0.0f;
		private float lastTimeUpdate = 0.0f;
		public float ServerTime
		{
			get
			{
				if (OwningNetWorker.IsServer)
					return Time.time;

				return serverTime + (Time.time - lastTimeUpdate);
			}
		}

		public string resourcesDirectory = string.Empty;

		public int frameInterval = 100;

		public int CurrentFrame { get { return (int)(ServerTime * 1000 / frameInterval); } }

		/// <summary>
		/// The amount of time in seconds to update the time from the server
		/// </summary>
		public float updateTimeInterval = 1.0f;

		protected override void Awake()
		{
			instance = this;

			base.Awake();
		}

		public void Populate()
		{
			if (Networking.Sockets == null)
			{
#if UNITY_EDITOR
				Debug.Log("Try using the Forge Quick Start Menu and setting the \"Scene Name\" on the \"Canvas\" to the scene you are loading. Then running from that scene.");
#endif

				throw new NetworkException("The NetWorker doesn't exist. Is it possible that a connection hasn't been made?");
			}

			if (ControllingSocket == null)
				ControllingSocket = Networking.PrimarySocket != null ? Networking.PrimarySocket : Networking.Sockets.First().Value;

			OwningNetWorker = ControllingSocket;

			if (!OwningNetWorker.Connected)
			{
				Networking.connected += delegate()
				{
					SimpleNetworkedMonoBehavior.SetupObjects(OwningNetWorker);

					foreach (System.Action execute in setupActions)
						execute();
				};
			}
			else
			{
				SimpleNetworkedMonoBehavior.SetupObjects(OwningNetWorker);

				foreach (System.Action execute in setupActions)
					execute();
			}

			OwningNetWorker.AddCustomDataReadEvent("BMS_PP", PollPlayersResponse);
		}

		public static void Instantiate(NetworkReceivers receivers, string obj, Vector3 position, Quaternion rotation, int callbackCounter)
		{
			if (NetworkingManager.Instance.OwningNetWorker != null)
				NetworkingManager.Instance.RPC("NetworkInstantiate", receivers, NetworkingManager.Instance.OwningNetWorker.Uniqueidentifier, NetworkingManager.Instance.OwningNetWorker.IsServer ? SimpleNetworkedMonoBehavior.GenerateUniqueId() : 0, obj, position, rotation, callbackCounter);
			else
			{
				setupActions.Add(() =>
				{
					NetworkingManager.Instance.RPC("NetworkInstantiate", receivers, NetworkingManager.Instance.OwningNetWorker.Uniqueidentifier, NetworkingManager.Instance.OwningNetWorker.IsServer ? SimpleNetworkedMonoBehavior.GenerateUniqueId() : 0, obj, position, rotation, callbackCounter);
				});
			}
		}

		protected override void Start()
		{
			if (networkInstantiates != null)
			{
				foreach (GameObject obj in networkInstantiates)
				{
					foreach (GameObject obj2 in networkInstantiates)
					{
						if (obj != obj2 && obj.name == obj2.name)
							Debug.LogError("You have two or more objects in the Network Instantiate array with the name " + obj.name + ", these should be unique");
					}
				}
			}

			base.Start();
		}

		protected override void Update()
		{
			base.Update();

			if (!OwningNetWorker.IsServer)
				return;

			if (updateTimeInterval <= 0.01f)
				return;

			if (previousTime + updateTimeInterval <= Time.time)
			{
				URPC("UpdateServerTime", NetworkReceivers.Others, Time.time);
				previousTime = Time.time;
			}
		}

		private SimpleNetworkedMonoBehavior[] GetAllSimpleMonoBehaviors(GameObject o)
		{
			List<SimpleNetworkedMonoBehavior> behaviors = new List<SimpleNetworkedMonoBehavior>(o.GetComponents<SimpleNetworkedMonoBehavior>());

			for (int i = 0; i < o.transform.childCount; i++)
				behaviors.AddRange(GetAllSimpleMonoBehaviors(o.transform.GetChild(i).gameObject));
			
			return behaviors.ToArray();
		}

		[BRPC]
		private void NetworkInstantiate(ulong ownerId, ulong startNetworkId, string name, Vector3 position, Quaternion rotation, int callbackId = 0)
		{
			if (networkedBehaviors.ContainsKey(startNetworkId))
				return;

			GameObject o = NetworkingManager.Instance.PullObject(name);
			SimpleNetworkedMonoBehavior[] netBehaviors = GetAllSimpleMonoBehaviors(o);

			if (netBehaviors.Length == 0)
			{
				Debug.LogError("Instantiating on the network is only for objects that derive from BaseNetworkedMonoBehavior, " +
					"if object does not need to be serialized consider using a RPC with GameObject.Instantiate");

				return;
			}

			// If there are multiple network behaviors on this object then update the ids
			if (netBehaviors.Length > 1)
			{
				for (int i = 1; i < netBehaviors.Length; i++)
					SimpleNetworkedMonoBehavior.GenerateUniqueId();
			}

			netBehaviors = GetAllSimpleMonoBehaviors((GameObject.Instantiate(o, position, rotation) as GameObject));

			for (int i = 0; i < netBehaviors.Length; i++)
				netBehaviors[i].Setup(OwningNetWorker, OwningNetWorker.Uniqueidentifier == ownerId, startNetworkId + (ulong)i, ownerId);

			RPC("InitializeObject", NetworkReceivers.Server, startNetworkId, netBehaviors.Length);

			if (ownerId == OwningNetWorker.Me.NetworkId)
				Networking.RunInstantiateCallback(callbackId, netBehaviors[0].gameObject);
		}

		[BRPC]
		private void DestroyOnNetwork(ulong networkId)
		{
			SimpleNetworkedMonoBehavior.NetworkDestroy(networkId);
		}

		/// <summary>
		/// Get a game object of a given name in the NetworkingManager
		/// </summary>
		/// <param name="name">Name of the game object to pull</param>
		/// <returns>If the GameObject exists in the NetworkingManager, then it will return that. Otherwise <c>null</c></returns>
		public GameObject PullObject(string name)
		{
			foreach (GameObject obj in networkInstantiates)
				if (obj.name == name)
					return obj;

			GameObject find = Resources.Load<GameObject>(resourcesDirectory + "/" + name);

			if (find == null)
				Debug.LogError("GameObject with name " + name + " was not found in the lookup. Make sure the object is in the resources folder or in the Networking Manager \"Network Instantiates\" array in the inspector.");

			return find;
		}

		/// <summary>
		/// Get the latest list of players from the server
		/// </summary>
		/// <param name="callback">The method to call once the player list has been received</param>
		public void PollPlayerList(Action<List<NetworkingPlayer>> callback = null)
		{
			if (OwningNetWorker.IsServer)
			{
				if (callback != null)
				{
					List<NetworkingPlayer> tmp = new List<NetworkingPlayer>(OwningNetWorker.Players.ToArray());
					tmp.Insert(0, OwningNetWorker.Me);
					callback(tmp);
				}

				return;
			}

			pollPlayersCallback = callback;
			RPC("PollPlayers", NetworkReceivers.Server);
		}

		/// <summary>
		/// Set the player name for the current running client or server
		/// </summary>
		/// <param name="name">The name to be assigned</param>
		public void SetName(string newName)
		{
			RPC("SetPlayerName", NetworkReceivers.Server, newName);
		}

		[BRPC]
		private void SetPlayerName(string newName)
		{
			if (!OwningNetWorker.IsServer)
				return;

			if (CurrentRPCSender == null)
				OwningNetWorker.Me.Rename(newName);
			else
				CurrentRPCSender.Rename(newName);
		}

		[BRPC]
		private void InitializeObject(ulong startObjectId, int count)
		{
			if (!OwningNetWorker.IsServer)
				return;

			for (int i = 0; i < count; i++)
			{
				if (SimpleNetworkedMonoBehavior.networkedBehaviors.ContainsKey(startObjectId + (ulong)i))
				{
					if (SimpleNetworkedMonoBehavior.networkedBehaviors[startObjectId + (ulong)i] is NetworkedMonoBehavior)
						((NetworkedMonoBehavior)SimpleNetworkedMonoBehavior.networkedBehaviors[startObjectId + (ulong)i]).AutoritativeSerialize();
				}
			}
		}

		[BRPC]
		private void PollPlayers()
		{
			playerPollData.Clear();

			if (!OwningNetWorker.IsServer)
				return;

			ObjectMapper.MapBytes(playerPollData, OwningNetWorker.Players.Count + 1);

			// Send the server first
			ObjectMapper.MapBytes(playerPollData, OwningNetWorker.Me.NetworkId, OwningNetWorker.Me.Name);

			foreach (NetworkingPlayer player in OwningNetWorker.Players)
				ObjectMapper.MapBytes(playerPollData, player.NetworkId, player.Name);

			Networking.WriteCustom("BMS_PP", OwningNetWorker, playerPollData, OwningNetWorker.CurrentStreamOwner, true);
		}

		private void PollPlayersResponse(NetworkingPlayer sender, NetworkingStream stream)
		{
			int count = ObjectMapper.Map<int>(stream);
			List<NetworkingPlayer> playerList = new List<NetworkingPlayer>();

			for (int i = 0; i < count; i++)
				playerList.Add(new NetworkingPlayer(ObjectMapper.Map<ulong>(stream), string.Empty, string.Empty, ObjectMapper.Map<string>(stream)));

			if (pollPlayersCallback != null)
				pollPlayersCallback(playerList);

			OwningNetWorker.AssignPlayers(playerList);
		}

		[BRPC]
		private void UpdateServerTime(float time)
		{
			serverTime = time;
			lastTimeUpdate = Time.time;
		}
	}
}