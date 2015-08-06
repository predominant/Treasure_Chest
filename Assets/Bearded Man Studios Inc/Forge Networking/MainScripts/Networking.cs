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

using System.Collections.Generic;

#if !NETFX_CORE
using System.Net;
using System.Net.Sockets;
using System;
#endif

namespace BeardedManStudios.Network
{
	public class Networking
	{
		/// <summary>
		/// The various types of protocols that are supported in the system - Mainly used internally
		/// </summary>
		public enum ProtocolType
		{
			QuickTCP = 0,
			TCP = 1,
			QuickUDP = 2,
			UDP = 3,
			ReliableUDP = 4,
			HTTP = 5
		}

		/// <summary>
		/// The different transporation protocols that are available in the system
		/// TCP:  http://en.wikipedia.org/wiki/Transmission_Control_Protocol
		/// UDP:  http://en.wikipedia.org/wiki/User_Datagram_Protocol
		/// </summary>
		public enum TransportationProtocolType
		{
			TCP = 1,
			UDP = 3
		}

		/// <summary>
		/// If true then the server will not automatically relay raw messages to clients
		/// </summary>
		public static bool ControlledRaw { get; set; }

		/// <summary>
		/// A dictionary of all the NetWorkers(Sockets) being used throughout the current process
		/// </summary>
		public static Dictionary<ushort, NetWorker> Sockets { get; private set; }

		/// <summary>
		/// Determine if a NetWorker(Socket) is connected on a given port
		/// </summary>
		/// <param name="port">The port number that is to be checked for a connection create by this system</param>
		/// <returns>True if there thi system has an established connection on the given port and false if there isn't a connection on that port</returns>
		public static bool IsConnected(ushort port) { if (!Sockets.ContainsKey(port)) return false; return Sockets[port].Connected; }

		/// <summary>
		/// Determine if a NetWorker(Socket) reference is connected, (Dumbly returns socket.Connected)
		/// </summary>
		/// <param name="socket">NetWorker(Socket) to be checked</param>
		/// <returns>True if the referenced NetWorker has established a connection</returns>
		public static bool IsConnected(NetWorker socket) { return socket.Connected; }

		/// <summary>
		/// Fires whenever a connection has been made by any of the sockets that are managed by this class (Though "Host" and "Connect")
		/// </summary>
		public static event NetWorker.BasicEvent connected
		{
			add
			{
				connectedInvoker += value;
			}
			remove
			{
				connectedInvoker -= value;
			}
		} static NetWorker.BasicEvent connectedInvoker;	// Because iOS doesn't have a JIT - Multi-cast function pointer.

		/// <summary>
		/// Fires whenever a ping has been recieved by the ping request <see cref="Networking.Ping" />
		/// </summary>
		public static event NetWorker.PingReceived pingReceived
		{
			add
			{
				pingReceivedInvoker += value;
			}
			remove
			{
				pingReceivedInvoker -= value;
			}
		} static NetWorker.PingReceived pingReceivedInvoker;	// Because iOS doesn't have a JIT - Multi-cast function pointer.

		/// <summary>
		/// A getter for the current primary socket.  Usually in games you will have one socket that does the main communication
		/// with the game and all of it's events, this would be the reference to that particular NetWorker(socket)
		/// </summary>
		public static NetWorker PrimarySocket { get; private set; }

		/// <summary>
		/// The type of protocol being used for the <see cref="Networking.PrimarySocket" /> object
		/// </summary>
		public static ProtocolType PrimaryProtocolType { get; private set; }

		/// <summary>
		/// Tell if the system is running under Bare Metal mode or not
		/// </summary>
		public static bool IsBareMetal { get; private set; }

		/// <summary>
		/// The list of callbacks that are fired for a network instantiate
		/// </summary>
		private static Dictionary<int, Action<GameObject>> instantiateCallbacks = new Dictionary<int, Action<GameObject>>();
		private static int callbackCounter = 1;

		/// <summary>
		/// Tell the system if we are currently running it as Bare Metal or not - Mainly internal
		/// </summary>
		/// <param name="isBearMetal">If this is a Bare Metal instance</param>
		public static void SetBareMetal(bool isBearMetal)
		{
			IsBareMetal = isBearMetal;
		}

		/// <summary>
		/// Used to assign the <see cref="Networking.PrimarySocket"/> object to the specified <see cref="NetWorker"/>
		/// </summary>
		/// <param name="netWorker">The NetWorker that will be the primary socket</param>
		public static void SetPrimarySocket(NetWorker netWorker)
		{
			PrimarySocket = netWorker;

			if (PrimarySocket is CrossPlatformUDP)
				PrimaryProtocolType = ProtocolType.UDP;
			else
				PrimaryProtocolType = ProtocolType.TCP;
		}

		/// <summary>
		/// Will setup a new server on this machine
		/// </summary>
		/// <param name="port">This is the port you want to bind the server to</param>
		/// <param name="comType">The particular transportation protocol <see cref="Networking.TransportationProtocolType"/> you wish to be used for this server</param>
		/// <param name="maxConnections">The maximum connections (players) allowed on the server at one point in time</param>
		/// <param name="winRT">If this is Windows Phone or Windows Store, this should be true, otherwise default to false</param>
		/// <param name="allowWebplayerConnection">Allow web player connections to server</param>
		/// <param name="relayToAll">Used to determine if messages should be relayed to client (normally true) - Mainly internal</param>
		/// <returns>The NetWorker server that was created (Which may not have established a connection yet <see cref="NetWorker.connected"/></returns>
		/// <example>
		/// public int port = 15937;																				// Port number
		/// public Networking.TransportationProtocolType protocolType = Networking.TransportationProtocolType.UDP;	// Communication protocol
		/// public int playerCount = 31;
		/// 
		/// #if NETFX_CORE && !UNITY_EDITOR
		///		private bool isWinRT = true;
		/// #else
		///		private bool isWinRT = false;
		/// #endif
		/// public void StartServer()
		/// {
		///		NetWorker socket = Networking.Host((ushort)port, protocolType, playerCount, isWinRT);	
		///	}
		/// </example>
		public static NetWorker Host(ushort port, TransportationProtocolType comType, int maxConnections, bool winRT = false, string overrideIP = null, bool allowWebplayerConnection = false, bool relayToAll = true)
		{
			Unity.MainThreadManager.Create();

			if (Sockets == null) Sockets = new Dictionary<ushort, NetWorker>();

			if (Sockets.ContainsKey(port) && Sockets[port].Connected)
				throw new NetworkException(8, "Socket has already been initialized on that port");

			if (comType == TransportationProtocolType.UDP)
				Sockets.Add(port, new CrossPlatformUDP(true, maxConnections));
			else
			{
				if (winRT)
					Sockets.Add(port, new WinMobileServer(maxConnections));
				else
				{
					Sockets.Add(port, new DefaultServerTCP(maxConnections));
					((DefaultServerTCP)Sockets[port]).RelayToAll = relayToAll;
				}
			}

			Sockets[port].connected += delegate()
			{
				Sockets[port].AssignUniqueId(0);

				if (connectedInvoker != null)
					connectedInvoker();
			};

			Sockets[port].Connect(overrideIP, port);

#if !NETFX_CORE
			// TODO:  Allow user to pass in the variables needed to pass into this begin function
			if (allowWebplayerConnection)
				SocketPolicyServer.Begin();
#endif

			return Sockets[port];
		}

		/// <summary>
		/// This will force a firewall request by the users machine to allow for
		/// network communications with this particular application
		/// </summary>
		/// <param name="port">Port to be allowed</param>
		public static void InitializeFirewallCheck(ushort port)
		{
			DefaultServerTCP socket = new DefaultServerTCP(1);
			socket.connected += socket.Disconnect;
			socket.Connect("127.0.0.1", port);
		}

		/// <summary>
		/// Create and connect a client to the specified server ip and port
		/// </summary>
		/// <param name="ip">The host (usually ip address or domain name) to connect to</param>
		/// <param name="port">The port for the particular server that this connection is attempting</param>
		/// <param name="comType">The transportation protocol type that is to be used <see cref="Networking.TransportationProtocolType"/></param>
		/// <param name="winRT">If this is Windows Phone or Windows Store, this should be true, otherwise default to false</param>
		/// <returns>The NetWorker client that was created (Which may not have established a connection yet <see cref="NetWorker.connected"/></returns>
		/// <example>
		/// public string host = "127.0.0.1";																		// IP address
		/// public int port = 15937;																				// Port number
		/// public Networking.TransportationProtocolType protocolType = Networking.TransportationProtocolType.UDP;	// Communication protocol
		/// 
		/// #if NETFX_CORE && !UNITY_EDITOR
		///		private bool isWinRT = true;
		/// #else
		///		private bool isWinRT = false;
		/// #endif
		/// public void StartServer()
		/// {
		///		NetWorker socket = Networking.Connect(host, (ushort)port, protocolType, isWinRT);	
		///	}
		/// </example>
		public static NetWorker Connect(string ip, ushort port, TransportationProtocolType comType, bool winRT = false)
		{
			Unity.MainThreadManager.Create();

			if (Sockets == null) Sockets = new Dictionary<ushort, NetWorker>();

			if (Sockets.ContainsKey(port))
			{
				if (Sockets[port].Connected)
					throw new NetworkException(8, "Socket has already been initialized on that port");
				else if (Sockets[port].Disconnected)
					Sockets.Remove(port);
				else
					return Sockets[port];	// It has not finished connecting yet
			}

			if (comType == TransportationProtocolType.UDP)
				Sockets.Add(port, new CrossPlatformUDP(false, 0));
			else
			{
				if (winRT)
					Sockets.Add(port, new WinMobileClient());
				else
					Sockets.Add(port, new DefaultClientTCP());
			}

			Sockets[port].connected += delegate()
			{
				if (connectedInvoker != null)
					connectedInvoker();
			};

			Sockets[port].Connect(ip, port);

			return Sockets[port];
		}

		/// <summary>
		/// Finds the first host on the network on the specified port number in the local area network and makes a connection to it
		/// </summary>
		/// <param name="port">The port to connect to</param>
		/// <param name="listenWaitTime">The time in milliseconds to wait for a discovery</param>
		/// <param name="protocol">The protocol type for the server</param>
		/// <param name="winRT">If this is Windows Phone or Windows Store, this should be true, otherwise default to false</param>
		/// <returns>The <see cref="NetWorker"/> that has been bound for this communication, null if none were found</returns>
#if NETFX_CORE
		public static IPEndPointWinRT LanDiscovery(ushort port, int listenWaitTime = 10000, TransportationProtocolType protocol = TransportationProtocolType.UDP, bool winRT = false)
#else
		public static IPEndPoint LanDiscovery(ushort port, int listenWaitTime = 10000, TransportationProtocolType protocol = TransportationProtocolType.UDP, bool winRT = false)
#endif
		{
#if !NETFX_CORE && !UNITY_WEBPLAYER
			UdpClient Client = new UdpClient();
			IPEndPoint foundEndpoint = new IPEndPoint(IPAddress.Any, 0);
			bool found = false;

			List<string> localSubNet = new List<string>();

			foreach (System.Net.NetworkInformation.NetworkInterface f in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
			{
				if (f.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up)
				{
					foreach (System.Net.NetworkInformation.GatewayIPAddressInformation d in f.GetIPProperties().GatewayAddresses)
					{
						if (d.Address.ToString() == "0.0.0.0") continue;

						if (d.Address.ToString().Contains("."))
							localSubNet.Add(d.Address.ToString().Remove(d.Address.ToString().LastIndexOf('.')));
					}
				}
			}

			foreach (string s in localSubNet)
				Client.Send(new byte[1], 1, new IPEndPoint(IPAddress.Parse(s + ".255"), port));

			int counter = 0;
			do
			{
				if (Client.Available != 0)
				{
					Client.Receive(ref foundEndpoint);
					found = true;
					break;
				}

				if (counter++ > listenWaitTime / 50)
					break;

				System.Threading.Thread.Sleep(50);
				foreach (string s in localSubNet)
					Client.Send(new byte[1], 1, new IPEndPoint(IPAddress.Parse(s + ".255"), port));
			} while (true);

			Client.Close();

			if (found)
				return foundEndpoint;
#elif NETFX_CORE
			// TODO:  Implement
#elif UNITY_WEBPLAYER
			Debug.LogError("Unable to find local at this time for webplayer");
#endif

			return null;
		}

		/// <summary>
		/// Disconnects a player on a given port
		/// </summary>
		/// <param name="port">Port to disconnect from</param>
		/// <param name="player">Player to disconnect</param>
		/// <exception cref="NetworkException">Thrown when there is not a <see cref="NetWorker"/> on the supplied port</exception>
		/// <exception cref="NetworkException">Thrown when the <see cref="NetWorker"/> on the specified port is not a server</exception>
		public static void Disconnect(ushort port, NetworkingPlayer player)
		{
			if (!Sockets.ContainsKey(port))
				throw new NetworkException("There isn't a server running using the specified port on this machine");

			if (!Sockets[port].IsServer)
				throw new NetworkException("Disconnecting players can only be managed by the server, the NetWorker on the specified port is not a server");

			Sockets[port].Disconnect(player);
		}

		/// <summary>
		/// Disconnect a player on a given NetWorker(Socket)
		/// </summary>
		/// <param name="socket">NetWorker(Socket) to be disconnected from</param>
		/// <param name="player">The player reference to disconnect</param>
		/// <exception cref="NetworkException">Thrown when the <see cref="NetWorker"/> on the specified port is not a server</exception>
		/// <example>
		/// // Disconnect the first player on the primary socket
		/// Networking.Disconnect(Networking.PrimarySocket, Networking.PrimarySocket.Players[0]);
		/// </example>
		public static void Disconnect(NetWorker socket, NetworkingPlayer player)
		{
			if (!socket.IsServer)
				throw new NetworkException("Disconnecting players can only be managed by the server, the NetWorker on the specified port is not a server");

			socket.Disconnect(player);
		}

		/// <summary>
		/// Disconnects (on this machine) either a client or a server on the specified port
		/// </summary>
		/// <param name="port">Port of the local server/client to be disconnected from</param>
		public static void Disconnect(ushort port)
		{
			if (Sockets[port] == null)
				return;

#if !NETFX_CORE
			if (!ReferenceEquals(NetworkingManager.Instance, null) && NetworkingManager.Instance.OwningNetWorker != null && NetworkingManager.Instance.OwningNetWorker.IsServer)
				SocketPolicyServer.End();
#endif

			try
			{
				Sockets[port].Disconnect();
			}
			catch { }

			Sockets[port] = null;
			Sockets.Remove(port);

			// TODO:  Go through all the NetworkedMonoBehaviors and clean them up
		}

		/// <summary>
		/// Disconnect the specified <see cref="NetWorker"/> (socket) and remove it from the <see cref="Networking.Sockets"/> lookup
		/// </summary>
		/// <param name="socket">The socket <see cref="NetWorker"/> to be shut down</param>
		public static void Disconnect(NetWorker socket)
		{
			ushort[] keys = new ushort[Sockets.Keys.Count];
			Sockets.Keys.CopyTo(keys, 0);
			for (int i = 0; i < keys.Length; i++)
			{
				if (Sockets[keys[i]] == socket)
				{
					socket.Disconnect();
					Sockets[keys[i]] = null;
					Sockets.Remove(keys[i]);
					break;
				}
			}
		}

		/// <summary>
		/// Disconnects all sockets and clients for this machine running under this system and
		/// removes them all from the lookup (calls <see cref="Networking.NetworkingReset"/>
		/// </summary>
		public static void Disconnect()
		{
#if !NETFX_CORE
			if (NetworkingManager.Instance != null && NetworkingManager.Instance.OwningNetWorker.IsServer)
				SocketPolicyServer.End();
#endif

			NetworkingReset();

			// TODO:  Go through all the NetworkedMonoBehaviors and clean them up
		}

		/// <summary>
		/// Writes a <see cref="NetworkingStream"/> to a particular <see cref="NetWorker"/> that is
		/// running on a particular port directly to a player (if the port is a server)
		/// </summary>
		/// <param name="port">The port that the <see cref="NetWorker"/> is listening on</param>
		/// <param name="player">The player that this server will be writing this message to</param>
		/// <param name="stream">The data stream that is to be written to the player</param>
		/// <exception cref="NetworkException">Thrown when there is not a <see cref="NetWorker"/> on the supplied port</exception>
		/// <exception cref="NetworkException">Thrown when the <see cref="NetWorker"/> on the specified port is not a server</exception>
		public static void Write(ushort port, NetworkingPlayer player, NetworkingStream stream)
		{
			if (!Sockets.ContainsKey(port))
				throw new NetworkException("There isn't a server running using the specified port on this machine");

			if (!Sockets[port].IsServer)
				throw new NetworkException("Writing to particular players can only be done by the server, the NetWorker on the specified port is not a server");

			Sockets[port].Write(player, stream);
		}

		/// <summary>
		/// Writes a <see cref="NetworkingStream"/> to a particular <see cref="NetWorker"/> directly to a player (if the port is a server)
		/// </summary>
		/// <param name="socket">NetWorker(Socket) to write with</param>
		/// <param name="player">Player to be written to server</param>
		/// <param name="stream">The stream of data to be written</param>
		/// <exception cref="NetworkException">Thrown when there is not a <see cref="NetWorker"/> on the supplied port</exception>
		/// <exception cref="NetworkException">Thrown when the <see cref="NetWorker"/> on the specified port is not a server</exception>
		public static void Write(NetWorker socket, NetworkingPlayer player, NetworkingStream stream)
		{
			if (!socket.IsServer)
				throw new NetworkException("Writing to particular players can only be done by the server, the NetWorker on the specified port is not a server");

			socket.Write(player, stream);
		}

		/// <summary>
		/// Writes a <see cref="NetworkingStream"/> to a particular <see cref="NetWorker"/> that is
		/// running on a particular port
		/// </summary>
		/// <param name="port">Port of the given NetWorker(Socket)</param>
		/// <param name="identifier">Unique identifier to be used</param>
		/// <param name="stream">The stream of data to be written</param>
		/// <param name="reliable">If this be a reliable UDP</param>
		public static void WriteUDP(ushort port, string identifier, NetworkingStream stream, bool reliable = false)
		{
			if (!Sockets.ContainsKey(port))
				throw new NetworkException("There isn't a server running using the specified port on this machine");

			Sockets[port].Write(identifier, stream, reliable);
		}

		/// <summary>
		/// Writes a <see cref="NetworkingStream"/> to a particular <see cref="NetWorker"/>
		/// </summary>
		/// <param name="socket">NetWorker(Socket) to write with</param>
		/// <param name="identifier">Unique identifier to be used</param>
		/// <param name="stream">The stream of data to be written</param>
		/// <param name="reliable">If this be a reliable UDP</param>
		public static void WriteUDP(NetWorker socket, string identifier, NetworkingStream stream, bool reliable = false)
		{
			socket.Write(identifier, stream, reliable);
		}

		/// <summary>
		/// Write to the TCP given a NetWorker(Socket) with a stream of data
		/// </summary>
		/// <param name="port">Port of the given NetWorker(Socket)</param>
		/// <param name="stream">The stream of data to be written</param>
		public static void WriteTCP(ushort port, NetworkingStream stream)
		{
			Sockets[port].Write(stream);
		}

		/// <summary>
		/// Write to the TCP given a NetWorker(Socket) with a stream of data
		/// </summary>
		/// <param name="socket">The NetWorker(Socket) to write with</param>
		/// <param name="stream">The stream of data to be written</param>
		public static void WriteTCP(NetWorker socket, NetworkingStream stream)
		{
			socket.Write(stream);
		}

		/// <summary>
		/// Write to the UDP given a ip and port with a identifier and a stream of data
		/// </summary>
		/// <param name="ip">IpAddress of the given NetWorker(Socket)</param>
		/// <param name="port">Port of the given NetWorker(Socket)</param>
		/// <param name="updateidentifier">Unique update identifier to be used</param>
		/// <param name="stream">The stream of data to be written</param>
		/// <param name="reliable">If this be a reliable UDP</param>
		[Obsolete("Static calls to the UDP library are no longer supported")]
		public static void WriteUDP(string ip, ushort port, string updateidentifier, NetworkingStream stream, bool reliable = false)
		{
			//CrossPlatformUDP.Write(ip, port, updateidentifier, stream, reliable);
		}

		private static bool ValidateNetworkedObject(string name, out SimpleNetworkedMonoBehavior netBehavior)
		{
			netBehavior = null;

			if (NetworkingManager.Instance == null)
			{
				Debug.LogError("The NetworkingManager object could not be found.");
				return false;
			}

			GameObject o = NetworkingManager.Instance.PullObject(name);

			if (o == null)
				return false;

			netBehavior = o.GetComponent<SimpleNetworkedMonoBehavior>();

			if (netBehavior == null)
			{
				Debug.LogError("Instantiating on the network is only for objects that derive from BaseNetworkedMonoBehavior, " +
					"if object does not need to be serialized consider using a RPC with GameObject.Instantiate");

				return false;
			}

			return true;
		}

		private static void CallInstantiate(string obj, NetworkReceivers receivers, Action<GameObject> callback = null)
		{
			SimpleNetworkedMonoBehavior netBehavior;
			if (ValidateNetworkedObject(obj, out netBehavior))
			{
				if (callback != null)
				{
					instantiateCallbacks.Add(callbackCounter, callback);

					NetworkingManager.Instantiate(receivers, obj, netBehavior.transform.position, netBehavior.transform.rotation, callbackCounter);
					callbackCounter++;

					if (callbackCounter == 0)
						callbackCounter++;
				}
				else
					NetworkingManager.Instantiate(receivers, obj, netBehavior.transform.position, netBehavior.transform.rotation, 0);
			}
		}

		private static void CallInstantiate(string obj, Vector3 position, Quaternion rotation, NetworkReceivers receivers, Action<GameObject> callback = null)
		{
			SimpleNetworkedMonoBehavior netBehavior;
			if (ValidateNetworkedObject(obj, out netBehavior))
			{
				if (callback != null)
				{
					instantiateCallbacks.Add(callbackCounter, callback);
					NetworkingManager.Instantiate(receivers, obj, position, rotation, callbackCounter);
					callbackCounter++;

					if (callbackCounter == 0)
						callbackCounter++;
				}
				else
					NetworkingManager.Instantiate(receivers, obj, position, rotation, 0);
			}
		}

		public static bool RunInstantiateCallback(int index, GameObject spawn)
		{
			if (instantiateCallbacks.ContainsKey(index))
			{
				instantiateCallbacks[index](spawn);
				instantiateCallbacks.Remove(index);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Instantiate an object on the network
		/// </summary>
		/// <param name="obj">Object to be instantiated by object name</param>
		/// <param name="receivers">Recipients will receive this instantiate call</param>
		public static void Instantiate(string obj, NetworkReceivers receivers, Action<GameObject> callback = null)
		{
			CallInstantiate(obj, receivers, callback: callback);
		}

		/// <summary>
		/// Instantiate an object on the network
		/// </summary>
		/// <param name="obj">Object to be instantiated by object name</param>
		/// <param name="position">Position of instantiated object</param>
		/// <param name="rotation">Rotation of instantiated object</param>
		/// <param name="receivers">Recipients will receive this instantiate call</param>
		public static void Instantiate(string obj, Vector3 position, Quaternion rotation, NetworkReceivers receivers, Action<GameObject> callback = null)
		{
			CallInstantiate(obj, position, rotation, receivers, callback);
		}

		/// <summary>
		/// Instantiate an object on the network
		/// </summary>
		/// <param name="obj">Object to be instantiated by object name</param>
		/// <param name="receivers">Recipients will receive this instantiate call (Default: All)</param>
		public static void Instantiate(GameObject obj, NetworkReceivers receivers = NetworkReceivers.All, Action<GameObject> callback = null)
		{
			CallInstantiate(obj.name, receivers, callback: callback);
		}

		/// <summary>
		/// Instantiate an object on the network
		/// </summary>
		/// <param name="obj">Object to be instantiated by object name</param>
		/// <param name="position">Position of instantiated object</param>
		/// <param name="rotation">Rotation of instantiated object</param>
		/// <param name="receivers">Recipients will receive this instantiate call (Default: All)</param>
		public static void Instantiate(GameObject obj, Vector3 position, Quaternion rotation, NetworkReceivers receivers = NetworkReceivers.All, Action<GameObject> callback = null)
		{
			CallInstantiate(obj.name, position, rotation, receivers, callback);
		}

		/// <summary>
		/// Instantiate an object on the network from the resources folder
		/// </summary>
		/// <param name="resourcePath">Location of the resource</param>
		/// <param name="receivers">Recipients will receive this instantiate call (Default: All)</param>
		public static void InstantiateFromResources(string resourcePath, NetworkReceivers receivers = NetworkReceivers.All, Action<GameObject> callback = null)
		{
			GameObject obj = Resources.Load<GameObject>(resourcePath);
			CallInstantiate(obj.name, obj.transform.position, obj.transform.rotation, receivers, callback);
		}

		/// <summary>
		/// Instantiate an object on the network from the resources folder
		/// </summary>
		/// <param name="resourcePath">Location of the resource</param>
		/// <param name="position">Position of instantiated object</param>
		/// <param name="rotation">Rotation of instantiated object</param>
		/// <param name="receivers">Recipients will receive this instantiate call (Default: All)</param>
		public static void InstantiateFromResources(string resourcePath, Vector3 position, Quaternion rotation, NetworkReceivers receivers = NetworkReceivers.All, Action<GameObject> callback = null)
		{
			GameObject obj = Resources.Load<GameObject>(resourcePath);
			CallInstantiate(obj.name, position, rotation, receivers, callback);
		}

		/// <summary>
		/// Destroy a simple networked object
		/// </summary>
		/// <param name="netBehavior">Networked behavior to destroy</param>
		public static void Destroy(SimpleNetworkedMonoBehavior netBehavior)
		{
			if (!netBehavior.IsOwner && !netBehavior.OwningNetWorker.IsServer)
				return;

			NetworkingManager.Instance.RPC("DestroyOnNetwork", netBehavior.NetworkedId);
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="id">Unique identifier to be used</param>
		/// <param name="port">Port to be written to</param>
		/// <param name="data">Data to send over</param>
		public static void WriteCustom(string id, ushort port, BMSByte data, NetworkReceivers recievers = NetworkReceivers.All)
		{
			WriteCustom(id, Sockets[port], data, false, recievers);
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="id">Unique identifier to be used</param>
		/// <param name="netWorker">The NetWorker(Socket) to write with</param>
		/// <param name="data">Data to send over</param>
		/// <param name="reliableUDP">If this be a reliable UDP</param>
		public static void WriteCustom(string id, NetWorker netWorker, BMSByte data, bool reliableUDP = false, NetworkReceivers recievers = NetworkReceivers.All)
		{
			if (netWorker is CrossPlatformUDP)
			{
				netWorker.Write(id, new NetworkingStream().Prepare(
					netWorker, NetworkingStream.IdentifierType.Custom, null, data, recievers, reliableUDP, id
				), reliableUDP);
			}
			else
			{
				netWorker.Write(new NetworkingStream().Prepare(
					netWorker, NetworkingStream.IdentifierType.Custom, null, data, recievers, reliableUDP, id
				));
			}
		}

		private static byte[] rawTypeIndicator = new byte[1] { 1 };
		/// <summary>
		/// Write a custom raw byte message with a 1 byte header across the network
		/// </summary>
		/// <param name="id"></param>
		/// <param name="netWorker"></param>
		/// <param name="data"></param>
		public static void WriteRaw(NetWorker netWorker, BMSByte data)
		{
			if (data == null)
			{
				netWorker.ThrowException(new NetworkException(1000, "The data being written can not be null"));
				return;
			}

			if (data.Size == 0)
			{
				netWorker.ThrowException(new NetworkException(1001, "The data being sent can't be empty"));
				return;
			}

			data.InsertRange(0, rawTypeIndicator);
			netWorker.WriteRaw(data);
		}

		/// <summary>
		/// Allows the server to send a raw message to a particular player
		/// </summary>
		/// <param name="netWorker"></param>
		/// <param name="targetPlayer"></param>
		/// <param name="data"></param>
		public static void WriteRaw(NetWorker netWorker, NetworkingPlayer targetPlayer, BMSByte data)
		{
			data.InsertRange(0, new byte[1] { 1 });
			netWorker.WriteRaw(targetPlayer, data);
		}

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="id">Unique identifier to be used</param>
		/// <param name="netWorker">The NetWorker(Socket) to write with</param>
		/// <param name="data">Data to send over</param>
		/// <param name="reliableUDP">If this be a reliable UDP</param>
		public static void WriteCustom(string id, NetWorker netWorker, BMSByte data, NetworkingPlayer target, bool reliableUDP = false)
		{
			if (!netWorker.IsServer)
				throw new NetworkException("Currently this overload of WriteCustom is only supported being called on the server.");

			if (netWorker is CrossPlatformUDP)
			{
				netWorker.Write(id, target, new NetworkingStream().Prepare(
					netWorker, NetworkingStream.IdentifierType.Custom, null, data, NetworkReceivers.Others, reliableUDP, id
				), reliableUDP);
			}
			else
			{
				netWorker.Write(target, new NetworkingStream().Prepare(
					netWorker, NetworkingStream.IdentifierType.Custom, null, data, NetworkReceivers.Others, reliableUDP, id
				));
			}
		}

#if !NETFX_CORE
		/// <summary>
		/// Get the local Ip address
		/// </summary>
		/// <returns>The Local Ip Address</returns>
		public static string GetLocalIPAddress()
		{
			IPHostEntry host;
			string localIP = "";
			host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (IPAddress ip in host.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork && ip.ToString().StartsWith("192."))
				{
					localIP = ip.ToString();
					break;
				}
			}

			return localIP;
		}
#endif

		/// <summary>
		/// To reset the network by clearing the sockets and disconnecting them if possible
		/// </summary>
		public static void NetworkingReset()
		{
			if (Sockets == null)
				return;

			ushort[] keys = new ushort[Sockets.Keys.Count];
			Sockets.Keys.CopyTo(keys, 0);
			foreach (ushort key in keys)
			{
				if (Sockets[key] != null && Sockets[key].Connected)
				{
					Sockets[key].Disconnect();
					Sockets[key] = null;
				}
			}

			Sockets.Clear();
		}

		#region Change Client Scene
		/// <summary>
		/// Tells the client to change their scene to the given scene.  This is often called
		/// after the server has changed to that scene to ensure that the server will always
		/// load up the scene before the client does
		/// </summary>
		/// <param name="netWorker"></param>
		/// <param name="sceneName">The name of the scene in which the client should load</param>
		public static void ChangeClientScene(NetWorker netWorker, string sceneName)
		{
			if (!netWorker.IsServer) throw new NetworkException("Only the server can call this method, the specified NetWorker is not a server");

			BMSByte data = new BMSByte();
			data.Clone(Encryptor.Encoding.GetBytes(sceneName));
			data.InsertRange(0, new byte[1] { 2 });
			netWorker.WriteRaw(data, false);
		}

		/// <summary>
		/// Tells the client to change their scene to the given scene.  This is often called
		/// after the server has changed to that scene to ensure that the server will always
		/// load up the scene before the client does
		/// </summary>
		/// <param name="netWorker">The current <see cref="NetWorker"/> that will be sending the message</param>
		/// <param name="targetPlayer">The particular player that will be receiving this message</param>
		/// <param name="sceneName">The name of the scene in which the client should load</param>
		public static void ChangeClientScene(NetWorker netWorker, NetworkingPlayer targetPlayer, string sceneName)
		{
			if (!netWorker.IsServer) throw new NetworkException("Only the server can call this method, the specified NetWorker is not a server");

			BMSByte data = new BMSByte();
			data.Clone(Encryptor.Encoding.GetBytes(sceneName));
			data.InsertRange(0, new byte[1] { 2 });
			netWorker.WriteRaw(targetPlayer, data);
		}

		/// <summary>
		/// Tells the client to change their scene to the given scene.  This is often called
		/// after the server has changed to that scene to ensure that the server will always
		/// load up the scene before the client does
		/// </summary>
		/// <param name="port">The port of the <see cref="NetWorker"/> that is to send the message</param>
		/// <param name="sceneName">The name of the scene in which the client should load</param>
		public static void ChangeClientScene(ushort port, string sceneName)
		{
			if (!Sockets.ContainsKey(port)) throw new NetworkException("There isn't a server running using the specified port on this machine");
			if (!Sockets[port].IsServer) throw new NetworkException("Only the server can call this method, the NetWorker on the specified port is not a server");

			BMSByte data = new BMSByte();
			data.Clone(Encryptor.Encoding.GetBytes(sceneName));
			data.InsertRange(0, new byte[1] { 2 });
			Sockets[port].WriteRaw(data, false);
		}

		/// <summary>
		/// Tells the client to change their scene to the given scene.  This is often called
		/// after the server has changed to that scene to ensure that the server will always
		/// load up the scene before the client does
		/// </summary>
		/// <param name="port">The port of the <see cref="NetWorker"/> that is to send the message</param>
		/// <param name="targetPlayer">The particular player that will be receiving this message</param>
		/// <param name="sceneName">The name of the scene in which the client should load</param>
		public static void ChangeClientScene(ushort port, NetworkingPlayer targetPlayer, string sceneName)
		{
			if (!Sockets.ContainsKey(port)) throw new NetworkException("There isn't a server running using the specified port on this machine");
			if (!Sockets[port].IsServer) throw new NetworkException("Writing to particular players can only be done by the server, the NetWorker on the specified port is not a server");

			BMSByte data = new BMSByte();
			data.Clone(Encryptor.Encoding.GetBytes(sceneName));
			data.InsertRange(0, new byte[1] { 2 });
			Sockets[port].WriteRaw(data, false);
		}
		#endregion

		/// <summary>
		/// Ping a particular host to get its response time in milliseconds
		/// </summary>
		/// <param name="host">The host object to ping</param>
		/// <param name="port">The port number that the server is running on</param>
		public static void Ping(HostInfo host, ushort pingFromPort = 35791)
		{
#if NETFX_CORE

#else
			System.Threading.Thread pingThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(ThreadPing));
			pingThread.Start(new object[] { host, pingFromPort });
#endif
		}

		private static void ThreadPing(object hostObj)
		{
#if NETFX_CORE

#else
			HostInfo host = (HostInfo)((object[])hostObj)[0];
			IPAddress address = IPAddress.Parse(host.ipAddress);
			UdpClient Client = new UdpClient();
			IPEndPoint ep = new IPEndPoint(address, host.port);

			Client.Send(new byte[1], 1, ep);
			System.DateTime start = System.DateTime.Now;
			int counter = 0;
			int maxTries = 50;

			do
			{
				if (Client.Available != 0)
				{
					Client.Receive(ref ep);

					if (ep.Address.ToString() == host.ipAddress && ep.Port == host.port)
						break;
				}

				if (counter++ >= maxTries)
					return;	// TODO:  Fire off a failed event

				System.Threading.Thread.Sleep(1000);
				Client.Send(new byte[1], 1, new IPEndPoint(address, host.port));
				start = System.DateTime.Now;
			} while (true);

			int time = (int)(System.DateTime.Now - start).TotalMilliseconds;

			host.SetPing(time);

			if (pingReceivedInvoker != null)
				pingReceivedInvoker(host, time);
#endif
		}
	}
}