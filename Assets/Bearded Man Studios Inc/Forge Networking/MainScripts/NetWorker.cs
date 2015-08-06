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



#if BMS_DEBUGGING
#define BMS_DEBUGGING_UNITY
#endif

using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;

#if !NETFX_CORE
using System.Net.Sockets;
using System.Threading;
#else
using Windows.Networking.Sockets;
using System.Collections.Generic;
#endif

namespace BeardedManStudios.Network
{
	public abstract class NetWorker
	{
		protected const int READ_THREAD_TIMEOUT = 50;
		public string Host { get; protected set; }
		public ushort Port { get; protected set; }

		private int threadSpeed = READ_THREAD_TIMEOUT;
		protected int ThreadSpeed { get { return threadSpeed; } set { threadSpeed = value; } }
		public void SetThreadSpeed(int speed) { ThreadSpeed = speed; }
		public NetworkingPlayer CurrentStreamOwner { get; protected set; }

		public bool MasterServerFlag { get; set; }

		/// <summary>
		/// The maximum connections allowed on this NetWorker(Socket)
		/// </summary>
		public int MaxConnections { get; protected set; }

		/// <summary>
		/// Current amount of connections
		/// </summary>
		public int Connections { get; private set; }

		/// <summary>
		/// Players conencted to this NetWorker(Socket)
		/// </summary>
		public virtual List<NetworkingPlayer> Players { get; protected set; }

		/// <summary>
		/// Assigns a list of players for a client
		/// </summary>
		public void AssignPlayers(List<NetworkingPlayer> playerList)
		{
			Players = playerList;
		}

		protected Dictionary<ulong, List<NetworkingStream>> rpcBuffer = new Dictionary<ulong, List<NetworkingStream>>();

		protected Dictionary<ulong, List<KeyValuePair<uint, NetworkingStream>>> udpRpcBuffer = new Dictionary<ulong, List<KeyValuePair<uint, NetworkingStream>>>();

		/// <summary>
		/// Basic event response delegate
		/// </summary>
		public delegate void BasicEvent();

		/// <summary>
		/// Basic event response delegate
		/// </summary>
		public delegate void PingReceived(HostInfo host, int time);

		/// <summary>
		/// Network Exception response delegate
		/// </summary>
		/// <param name="exception">Exception thrown</param>
		public delegate void NetworkErrorEvent(Exception exception);

		/// <summary>
		/// Network Message response delegate
		/// </summary>
		/// <param name="stream">Stream responded with</param>
		public delegate void NetworkMessageEvent(NetworkingStream stream);

		/// <summary>
		/// Direct Network Message response delegate
		/// </summary>
		/// <param name="player">Player responded with</param>
		/// <param name="stream">Stream responded with</param>
		public delegate void DirectNetworkMessageEvent(NetworkingPlayer player, NetworkingStream stream);

		/// <summary>
		/// Direct Raw Network Message response delegate
		/// </summary>
		/// <param name="data">Stream responded with</param>
		public delegate void DirectRawNetworkMessageEvent(NetworkingPlayer player, BMSByte data);

		/// <summary>
		/// Player connection response delegate
		/// </summary>
		/// <param name="player">Player who connected</param>
		public delegate void PlayerConnectionEvent(NetworkingPlayer player);

		/// <summary>
		/// String response delegate
		/// </summary>
		/// <param name="message">String message responded with</param>
		public delegate void StringResponseEvent(string message);

		/// <summary>
		/// Byte array response delegate
		/// </summary>
		/// <param name="bytes">Byte array responded with</param>
		public delegate void ByteResponseEvent(byte[] bytes);

		/// <summary>
		/// This will make it so that only players who are close to one another will get updates about each other
		/// </summary>
		public bool ProximityBasedMessaging { get; set; }

		/// <summary>
		/// This is the distance in Unity units of the range that players need to be in to get updates about each other
		/// </summary>
		public float ProximityMessagingDistance { get; set; }

		protected List<NetworkingPlayer> alreadyUpdated = new List<NetworkingPlayer>();

		/// <summary>
		/// This will turn on proximity based messaging, see ProximityBasedMessaging property of this class
		/// </summary>
		/// <param name="updateDistance">The distance in Unity units of the range that players need to be in to get updates about each other</param>
		public void MakeProximityBased(float updateDistance)
		{
			ProximityBasedMessaging = true;
			ProximityMessagingDistance = updateDistance;
		}

		/// <summary>
		/// This is the servers reference to it's own player (NOT used by client)
		/// </summary>
		public NetworkingPlayer Me { get; protected set; }

		/// <summary>
		/// The event to hook into for when a NetWorker(Socket) connects
		/// </summary>
		public event BasicEvent connected
		{
			add
			{
				connectedInvoker += value;
			}
			remove
			{
				connectedInvoker -= value;
			}
		}
		BasicEvent connectedInvoker;	// Because iOS doesn't have a JIT - Multi-cast function pointer.

#if NETFX_CORE
		protected async void OnConnected()
#else
		protected void OnConnected()
#endif
		{
#if NETFX_CORE
			await System.Threading.Tasks.Task.Delay(System.TimeSpan.FromMilliseconds(50));
#else
			System.Threading.Thread.Sleep(50);
#endif

			if (Networking.IsBareMetal)
			{
				if (connectedInvoker != null)
					connectedInvoker();

				Connected = true;
				Disconnected = false;
			}
			else
			{
				if (connectedInvoker != null)
				{
					try
					{
						// If there is not a MAIN_THREAD_MANAGER then throw the error and disconnect
						BeardedManStudios.Network.Unity.MainThreadManager.Run(delegate()
						{
							connectedInvoker();
						});

						Connected = true;
						Disconnected = false;
					}
#if UNITY_EDITOR
					catch (Exception e)
					{
						UnityEngine.Debug.LogException(e);
#else
					catch
					{
#endif
						Disconnect();
					}
				}
			}
		}

		/// <summary>
		/// The event to hook into for when a NetWorker(Socket) disconnects
		/// </summary>
		public event BasicEvent disconnected
		{
			add
			{
				disconnectedInvoker += value;
			}
			remove
			{
				disconnectedInvoker -= value;
			}
		} BasicEvent disconnectedInvoker;	// Because iOS doesn't have a JIT - Multi-cast function pointer.

		/// <summary>
		/// The event to hook into for when a server disconnects
		/// </summary>
		public event StringResponseEvent serverDisconnected
		{
			add
			{
				serverDisconnectedInvoker += value;
			}
			remove
			{
				serverDisconnectedInvoker -= value;
			}
		} StringResponseEvent serverDisconnectedInvoker;	// Because iOS doesn't have a JIT - Multi-cast function pointer.

		protected void OnDisconnected()
		{
			Connected = false;
			Disconnected = true;

			if (Networking.IsBareMetal)
			{
				if (disconnectedInvoker != null)
					disconnectedInvoker();
			}
			else
			{
				if (disconnectedInvoker != null)
					disconnectedInvoker();
			}
		}

		#region Timeout Disconnect
		protected DateTime lastReadTime;
		public int LastRead
		{
			get
			{
				return (int)(DateTime.Now - lastReadTime).TotalMilliseconds;
			}
		}
		public int ReadTimeout = 0;

#if NETFX_CORE
		protected async void TimeoutCheck()
#else
		protected void TimeoutCheck()
#endif
		{
			while (true)
			{
				if (ReadTimeout == 0)
				{
#if NETFX_CORE
					await System.Threading.Tasks.Task.Delay(System.TimeSpan.FromMilliseconds(3000));
#else
					System.Threading.Thread.Sleep(3000);
#endif
					continue;
				}

#if NETFX_CORE
				await System.Threading.Tasks.Task.Delay(System.TimeSpan.FromMilliseconds(ReadTimeout - LastRead + 1));
#else
				System.Threading.Thread.Sleep(ReadTimeout - LastRead + 1);
#endif

				if (LastRead >= ReadTimeout)
				{
					TimeoutDisconnect();
				}
			}
		}

		/// <summary>
		/// The event to hook into for when a NetWorker(Socket) disconnects
		/// </summary>
		public event BasicEvent timeoutDisconnected
		{
			add
			{
				timeoutDisconnectedInvoker += value;
			}
			remove
			{
				timeoutDisconnectedInvoker -= value;
			}
		} BasicEvent timeoutDisconnectedInvoker;	// Because iOS doesn't have a JIT - Multi-cast function pointer.

		protected void OnTimeoutDisconnected()
		{
			Connected = false;
			Disconnected = true;

			if (timeoutDisconnectedInvoker != null)
				timeoutDisconnectedInvoker();
		}
		#endregion

		protected void OnDisconnected(string reason)
		{
			Connected = false;
			Disconnected = true;

			if (serverDisconnectedInvoker != null) serverDisconnectedInvoker(reason);
		}

		/// <summary>
		/// The event to hook into for when this NetWorker(Socket) receives and error
		/// </summary>
		public event NetworkErrorEvent error
		{
			add
			{
				errorInvoker += value;
			}
			remove
			{
				errorInvoker -= value;
			}
		} NetworkErrorEvent errorInvoker;	// Because iOS doesn't have a JIT - Multi-cast function pointer.

		protected void OnError(Exception exception)
		{
			if (errorInvoker != null)
			{
				if (Networking.IsBareMetal)
					errorInvoker(exception);
				else
					BeardedManStudios.Network.Unity.MainThreadManager.Run(delegate() { errorInvoker(exception); });
			}
		}

		public void ThrowException(NetworkException exception) { OnError(exception); }

		/// <summary>
		/// The event to hook into for when this NetWorker(Socket) sends data
		/// </summary>
		public event NetworkMessageEvent dataSent
		{
			add
			{
				dataSentInvoker += value;
			}
			remove
			{
				dataSentInvoker -= value;
			}
		} NetworkMessageEvent dataSentInvoker;	// Because iOS doesn't have a JIT - Multi-cast function pointer.

		protected void OnDataSent(NetworkingStream stream) { if (dataSentInvoker != null) dataSentInvoker(stream); }

		/// <summary>
		/// The event to hook into for when this NetWorker(Socket) reads data
		/// </summary>
		public event DirectNetworkMessageEvent dataRead
		{
			add
			{
				dataReadInvoker += value;
			}
			remove
			{
				dataReadInvoker -= value;
			}
		} DirectNetworkMessageEvent dataReadInvoker;	// Because iOS doesn't have a JIT - Multi-cast function pointer.

		/// <summary>
		/// The event to hook into for when this NetWorker(Socket) reads raw data
		/// </summary>
		public event DirectRawNetworkMessageEvent rawDataRead
		{
			add
			{
				rawDataReadInvoker += value;
			}
			remove
			{
				rawDataReadInvoker -= value;
			}
		} DirectRawNetworkMessageEvent rawDataReadInvoker;	// Because iOS doesn't have a JIT - Multi-cast function pointer.

		protected void OnDataRead(NetworkingPlayer player, NetworkingStream stream)
		{
			if (dataReadInvoker != null) dataReadInvoker(player, stream);

			if (stream.identifierType == NetworkingStream.IdentifierType.Custom)
				OnCustomDataRead(stream.Customidentifier, player, stream);
		}

		protected void OnRawDataRead(NetworkingPlayer sender, BMSByte data)
		{
			if (rawDataReadInvoker != null)
			{
				data.MoveStartIndex(sizeof(byte));
				rawDataReadInvoker(sender, data);
			}
		}

		private uint currentCustomId = 0;
		private Dictionary<string, uint> customReadIdentifiers = new Dictionary<string, uint>();
		public Dictionary<string, uint> CustomReadIdentifiers { get { return customReadIdentifiers; } }
		private Dictionary<uint, string> customReadIdentifiersIds = new Dictionary<uint, string>();
		public Dictionary<uint, string> CustomReadIdentifiersIds { get { return customReadIdentifiersIds; } }
		private Dictionary<string, System.Action<NetworkingPlayer, NetworkingStream>> customDataRead = new Dictionary<string, System.Action<NetworkingPlayer, NetworkingStream>>();

		/// <summary>
		/// Add a custom event to the NetWorker(Socket) read event
		/// </summary>
		/// <param name="id">Unique identifier to pass with</param>
		/// <param name="action">Action to be added to the events to be called upon</param>
		public void AddCustomDataReadEvent(string id, System.Action<NetworkingPlayer, NetworkingStream> action)
		{
			if (!customDataRead.ContainsKey(id))
			{
				customReadIdentifiersIds.Add(currentCustomId, id);
				customReadIdentifiers.Add(id, currentCustomId++);
				customDataRead.Add(id, action);
			}
			else
				customDataRead[id] = action;
		}

		/// <summary>
		/// Remove a custom event from the NetWorker(Socket) read event
		/// </summary>
		/// <param name="id">Unique identifier to pass with</param>
		public void RemoveCustomDataReadEvent(string id) { if (customDataRead.ContainsKey(id)) customDataRead.Remove(id); }

		protected void OnCustomDataRead(string id, NetworkingPlayer player, NetworkingStream stream) { if (customDataRead.ContainsKey(id)) customDataRead[id](player, stream); }

		/// <summary>
		/// The event to hook into for when this NetWorker(Socket) recieves another player
		/// </summary>
		public event PlayerConnectionEvent playerConnected
		{
			add
			{
				playerConnectedInvoker += value;
			}
			remove
			{
				playerConnectedInvoker -= value;
			}
		} PlayerConnectionEvent playerConnectedInvoker;	// Because iOS doesn't have a JIT - Multi-cast function pointer.

		protected void OnPlayerConnected(NetworkingPlayer player)
		{
			if (playerConnectedInvoker != null)
			{
				if (Networking.IsBareMetal)
					playerConnectedInvoker(player);
				else
				{
					BeardedManStudios.Network.Unity.MainThreadManager.Run(delegate()
					{
						playerConnectedInvoker(player);
					});
				}
			}

			Connections++;
		}

		/// <summary>
		/// The event to hook into for when this NetWorker(Socket) disconnects another player
		/// </summary>
		public event PlayerConnectionEvent playerDisconnected
		{
			add
			{
				playerDisconnectedInvoker += value;
			}
			remove
			{
				playerDisconnectedInvoker -= value;
			}
		} PlayerConnectionEvent playerDisconnectedInvoker;	// Because iOS doesn't have a JIT - Multi-cast function pointer.

		protected void OnPlayerDisconnected(NetworkingPlayer player)
		{
			if (playerDisconnectedInvoker != null)
			{
				if (Networking.IsBareMetal)
					playerDisconnectedInvoker(player);
				else
				{
					BeardedManStudios.Network.Unity.MainThreadManager.Run(delegate()
					{
						playerDisconnectedInvoker(player);
					});
				}
			}
			
			Connections--;
		}

		/// <summary>
		/// Returns a value whether or not this NetWorker(Socket) is the server
		/// </summary>
		public bool IsServer
		{
			get
			{
				if (this is DefaultServerTCP)
					return true;

				if (this is WinMobileServer)
					return true;

				if (this is CrossPlatformUDP)
					return ((CrossPlatformUDP)this).IsServer;

				return false;
			}
		}

		protected class StreamRead
		{
			public int clientIndex = -1;
			public BMSByte bytes = new BMSByte();

			public StreamRead() { }

			public StreamRead Prepare(int c, BMSByte b)
			{
				clientIndex = c;
				bytes.Clone(b);

				return this;
			}
		}

		/// <summary>
		/// The player count on this NetWorker(Socket)
		/// </summary>
		public ulong ServerPlayerCounter { get; protected set; }

		/// <summary>
		/// Whether or not this NetWorker(Socket) is connected
		/// </summary>
		public bool Connected { get; protected set; }

		/// <summary>
		/// Whether or not this NetWorker(Socket) was once connected and now is disconnected
		/// </summary>
		public bool Disconnected { get; protected set; }

		/// <summary>
		/// The update identifier of the NetWorker(Socket)
		/// </summary>
		public ulong Uniqueidentifier { get; private set; }

		/// <summary>
		/// Constructor of the NetWorker(Socket)
		/// </summary>
		public NetWorker()
		{
			if (!Networking.IsBareMetal)
				Unity.NetWorkerKiller.AddNetWorker(this);
		}

		/// <summary>
		/// Constructor of the NetWorker(Socket) with a Maximum allowed connections count
		/// </summary>
		/// <param name="maxConnections">The maximum number of connections allowed on this NetWorker(Socket)</param>
		public NetWorker(int maxConnections)
		{
			MaxConnections = maxConnections;
			
			if (!Networking.IsBareMetal)
				Unity.NetWorkerKiller.AddNetWorker(this);
		}

		~NetWorker() { Disconnect(); }

		abstract public void Connect(string hostAddress, ushort port);

		abstract public void Disconnect();

		abstract public void TimeoutDisconnect();

		/// <summary>
		/// Disconnect a player on this NetWorker(Socket)
		/// </summary>
		/// <param name="player">Player to disconnect</param>
		public virtual void Disconnect(NetworkingPlayer player, string reason = null)
		{
			if (alreadyUpdated.Contains(player))
				alreadyUpdated.Remove(player);

			OnPlayerDisconnected(player);
		}


		abstract public void Write(NetworkingStream stream);

		abstract public void Write(NetworkingPlayer player, NetworkingStream stream);

		/// <summary>
		/// Write to the NetWorker(Socket) with a given Update Identifier and Network Stream
		/// </summary>
		/// <param name="updateidentifier">Unique update identifier to be used</param>
		/// <param name="stream">Network stream being written with</param>
		/// <param name="reliable">If this is a reliable send</param>
		public virtual void Write(string updateidentifier, NetworkingStream stream, bool reliable = false) { }

		/// <summary>
		/// Write to the NetWorker(Socket) with a given Update Identifier and Network Stream
		/// </summary>
		/// <param name="updateidentifier">Unique update identifier to be used</param>
		/// <param name="stream">Network stream being written with</param>
		/// <param name="reliable">If this is a reliable send</param>
		/// <param name="packets">Packets to send</param>
		public virtual void Write(uint updateidentifier, NetworkingStream stream, bool reliable = false, List<BMSByte> packets = null) { }

		/// <summary>
		/// Write to the NetWorker(Socket) with a given Update Identifier, Player, and Network Stream
		/// </summary>
		/// <param name="updateidentifier">Unique update identifier to be used</param>
		/// <param name="player">Player to write with</param>
		/// <param name="stream">Network stream being written with</param>
		/// <param name="reliable">If this is a reliable send</param>
		/// <param name="packets">Packets to send</param>
		public virtual void Write(string updateidentifier, NetworkingPlayer player, NetworkingStream stream, bool reliable = false, List<BMSByte> packets = null) { }

		/// <summary>
		/// Write to the NetWorker(Socket) with a given Update Identifier, Player, and Network Stream
		/// </summary>
		/// <param name="updateidentifier">Unique update identifier to be used</param>
		/// <param name="player">Player to write with</param>
		/// <param name="stream">Network stream being written with</param>
		/// <param name="reliable">If this is a reliable send</param>
		/// <param name="packets">Packets to send</param>
		public virtual void Write(uint updateidentifier, NetworkingPlayer player, NetworkingStream stream, bool reliable = false, List<BMSByte> packets = null) { }

		public virtual void WriteRaw(NetworkingPlayer player, BMSByte data, string reliableId = "") { }
		public virtual void WriteRaw(BMSByte data, bool relayToServer = true, string reliableId = "") { }

		private object rpcMutex = new object();

		/// <summary>
		/// Read the data of a player and data stream
		/// </summary>
		/// <param name="player">Player to read from</param>
		/// <param name="stream">Network stream being read from</param>
		public void DataRead(NetworkingPlayer player, NetworkingStream stream)
		{
#if BMS_DEBUGGING_UNITY
			UnityEngine.Debug.Log("[NetWorker DataRead Player IP] " + (player != null ? player.Ip : string.Empty));
			UnityEngine.Debug.Log("[NetWorker DataRead Player NetworkID] " + (player != null ? player.NetworkId.ToString() : string.Empty));
			UnityEngine.Debug.Log("[NetWorker DataRead Stream Bytes] " + ((stream != null && stream.Bytes != null) ? stream.Bytes.Count.ToString() : string.Empty));
			UnityEngine.Debug.Log("[NetWorker DataRead Stream NetworkID] " + (stream != null ? stream.NetworkId.ToString() : string.Empty));
#endif
			OnDataRead(player, stream);

			if (stream.identifierType == NetworkingStream.IdentifierType.RPC)
			{
#if BMS_DEBUGGING_UNITY
				UnityEngine.Debug.Log("[NetWorker DataRead New Stream RPC]");
#endif
				lock (rpcMutex)
				{
					new NetworkingStreamRPC(stream);
				}
			}
		}

		/// <summary>
		/// Get the new player updates
		/// </summary>
		public virtual void GetNewPlayerUpdates() { }

		protected void ServerBufferRPC(NetworkingStream stream)
		{
			if (stream.identifierType == NetworkingStream.IdentifierType.RPC && stream.BufferedRPC)
			{
				if (!rpcBuffer.ContainsKey(stream.RealSenderId))
					rpcBuffer.Add(stream.RealSenderId, new List<NetworkingStream>());

				NetworkingStream clonedStream = new NetworkingStream(stream.ProtocolType);
				clonedStream.Bytes.BlockCopy(stream.Bytes.byteArr, stream.Bytes.StartIndex(), stream.Bytes.Size);
				clonedStream.AssignSender(Me);

				rpcBuffer[stream.RealSenderId].Add(clonedStream);
			}
		}

		protected void RelayStream(NetworkingStream stream)
		{
			if (stream.Receivers == NetworkReceivers.Server)
				return;

			if (stream.identifierType == NetworkingStream.IdentifierType.RPC && stream.BufferedRPC)
			{
				NetworkingStream clonedStream = new NetworkingStream(stream.ProtocolType).PrepareFinal(this, stream.identifierType, stream.NetworkedBehaviorId, stream.Bytes, stream.Receivers, stream.BufferedRPC, stream.Customidentifier, senderId: stream.RealSenderId);
				clonedStream.AssignSender(stream.Sender);

				if (!rpcBuffer.ContainsKey(stream.RealSenderId))
					rpcBuffer.Add(stream.RealSenderId, new List<NetworkingStream>());

				rpcBuffer[stream.RealSenderId].Add(clonedStream);
			}

			writeStream.SetProtocolType(stream.ProtocolType);

			if (!Networking.IsBareMetal)
				writeStream.Prepare(this, stream.identifierType, stream.NetworkedBehavior, stream.Bytes, stream.Receivers, stream.BufferedRPC, stream.Customidentifier, stream.RealSenderId);
			else
				writeStream.PrepareFinal(this, stream.identifierType, stream.NetworkedBehaviorId, stream.Bytes, stream.Receivers, stream.BufferedRPC, stream.Customidentifier, stream.RealSenderId);

			// Write what was read to all the clients
			Write(writeStream);
		}

		protected void ServerBufferRPC(uint updateidentifier, NetworkingStream stream)
		{
			if (stream.Receivers != NetworkReceivers.AllBuffered && stream.Receivers != NetworkReceivers.OthersBuffered)
				return;

			if (stream.identifierType == NetworkingStream.IdentifierType.RPC && stream.BufferedRPC)
			{
#if BMS_DEBUGGING_UNITY
				UnityEngine.Debug.Log("[NetWorker ServerBuffering RPC]");
#endif
				if (!udpRpcBuffer.ContainsKey(stream.RealSenderId))
					udpRpcBuffer.Add(stream.RealSenderId, new List<KeyValuePair<uint, NetworkingStream>>());

				NetworkingStream clonedStream = new NetworkingStream(stream.ProtocolType);
				clonedStream.Bytes.BlockCopy(stream.Bytes.byteArr, stream.Bytes.StartIndex(), stream.Bytes.Size);
				clonedStream.AssignSender(Me);

				udpRpcBuffer[stream.RealSenderId].Add(new KeyValuePair<uint, NetworkingStream>(updateidentifier, clonedStream));
			}
		}

		protected void RelayRawStream(NetworkingPlayer sender, BMSByte bytes)
		{
			if (Networking.ControlledRaw)
				return;

			WriteRaw(bytes, false);
		}

		private NetworkingStream writeStream = new NetworkingStream();
		protected void RelayStream(uint updateidentifier, NetworkingStream stream)
		{
			if (stream.identifierType == NetworkingStream.IdentifierType.RPC && stream.BufferedRPC)
			{
				NetworkingStream recreatedStream = new NetworkingStream(stream.ProtocolType).PrepareFinal(this, stream.identifierType, stream.NetworkedBehaviorId, stream.Bytes, stream.Receivers, stream.BufferedRPC, stream.Customidentifier, stream.RealSenderId);
				recreatedStream.AssignSender(stream.Sender);

				if (!udpRpcBuffer.ContainsKey(stream.RealSenderId))
					udpRpcBuffer.Add(stream.RealSenderId, new List<KeyValuePair<uint, NetworkingStream>>());

				udpRpcBuffer[stream.RealSenderId].Add(new KeyValuePair<uint, NetworkingStream>(updateidentifier, recreatedStream));
			}

			writeStream.SetProtocolType(stream.ProtocolType);
			if (!Networking.IsBareMetal)
				writeStream.Prepare(this, stream.identifierType, stream.NetworkedBehavior, stream.Bytes, stream.Receivers, stream.BufferedRPC, stream.Customidentifier, stream.RealSenderId);
			else
				writeStream.PrepareFinal(this, stream.identifierType, stream.NetworkedBehaviorId, stream.Bytes, stream.Receivers, stream.BufferedRPC, stream.Customidentifier, stream.RealSenderId);

			Write(updateidentifier, writeStream);
		}
		
#if NETFX_CORE
		protected async void UpdateNewPlayer(NetworkingPlayer player)
#else
		protected void UpdateNewPlayer(NetworkingPlayer player)
#endif
		{
			if (alreadyUpdated.Contains(player))
				return;

			alreadyUpdated.Add(player);

			if (rpcBuffer.Count > 0)
			{
				foreach (KeyValuePair<ulong, List<NetworkingStream>> kv in rpcBuffer)
				{
					foreach (NetworkingStream stream in kv.Value)
					{
						Write(player, stream);
					}
				}
			}

			if (udpRpcBuffer.Count > 0)
			{
				foreach (KeyValuePair<ulong, List<KeyValuePair<uint, NetworkingStream>>> kv in udpRpcBuffer)
				{
					foreach (KeyValuePair<uint, NetworkingStream> stream in kv.Value)
					{
						Write(stream.Key, player, stream.Value, true);
					}
				}
			}
		}

		protected void CleanRPCForPlayer(NetworkingPlayer player)
		{
			if (rpcBuffer.ContainsKey(player.NetworkId))
				rpcBuffer.Remove(player.NetworkId);
		}

		protected void CleanUDPRPCForPlayer(NetworkingPlayer player)
		{
			if (udpRpcBuffer.ContainsKey(player.NetworkId))
				udpRpcBuffer.Remove(player.NetworkId);
		}

		/// <summary>
		/// Assign a unique id to this NetWorker(Socket)
		/// </summary>
		/// <param name="id">Unique ID to assign with</param>
		public void AssignUniqueId(ulong id)
		{
			Uniqueidentifier = id;
		}
	}
}