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
using UnityEngine.UI;

using BeardedManStudios.Network;
using BeardedManStudios.Network.API;
using BeardedManStudios.Network.Unity;

namespace BeardedManStudios.Forge.Examples
{
	public class StartGame : MonoBehaviour
	{
		public string host = "127.0.0.1";																		// IP address
		public int port = 15937;																				// Port number
		public Networking.TransportationProtocolType protocolType = Networking.TransportationProtocolType.UDP;	// Communication protocol
		public int playerCount = 31;																			// Maximum player count -- excluding this server
		public string sceneName = "Game";																		// Scene to load
		public string serverBrowserScene = "ForgeQuickServerBrowser";											// The server browser scene
		public bool proximityBasedUpdates = false;																// Only send other player updates if they are within range
		public float proximityDistance = 5.0f;																	// The range for the players to be updated within

		private NetWorker socket = null;																		// The initial connection socket

		public InputField ipAddressInput = null;																// The input label for the ip address for the client to connect to directly

		public float packetDropSimulationChance = 0.0f;															// A number between 0 and 1 where 0 is 0% and 1 is 100%
		public int networkLatencySimulationTime = 0;															// The amount of time in milliseconds to simulate network latency

		public string masterServerIp = string.Empty;															// If this has a value then it will register itself on the master server at this location

		/// <summary>
		/// Determine if the current system is within the "WinRT" ecosystem
		/// </summary>
		private bool IsWinRT
		{
			get
			{
#if UNITY_4_6
				return Application.platform == RuntimePlatform.MetroPlayerARM ||
					Application.platform == RuntimePlatform.MetroPlayerX86 ||
					Application.platform == RuntimePlatform.MetroPlayerX64;
#else
				return Application.platform == RuntimePlatform.WSAPlayerARM ||
					Application.platform == RuntimePlatform.WSAPlayerX86 ||
					Application.platform == RuntimePlatform.WSAPlayerX64;
#endif
			}
		}

		public void Start()
		{
			// Assign the text for the input to be whatever is set by default
			ipAddressInput.text = host;

			// These devices have no reason to fire off a firewall check as they are not behind a local firewall
#if !UNITY_IPHONE && !UNITY_WP_8_1 && !UNITY_ANDROID
			// Check to make sure that the user is allowing this connection through the local OS firewall
			Networking.InitializeFirewallCheck((ushort)port);
#endif
		}

		/// <summary>
		/// This method is called when a player connects or disconnects in order to update the player count on Arbiter
		/// </summary>
		/// <param name="player">The player that just connected or disconnected</param>
		private void UpdatePlayerCount(NetworkingPlayer player)
		{
			ForgeMasterServer.UpdateServer(masterServerIp, socket.Port, socket.Players.Count);
		}

		/// <summary>
		/// This method is called when the host server button is clicked
		/// </summary>
		public void StartServer()
		{
			// Create a host connection
			socket = Networking.Host((ushort)port, protocolType, playerCount, IsWinRT);

			if (socket is CrossPlatformUDP)
			{
				((CrossPlatformUDP)socket).packetDropSimulationChance = packetDropSimulationChance;
				((CrossPlatformUDP)socket).networkLatencySimulationTime = networkLatencySimulationTime;
			}

			if (!string.IsNullOrEmpty(masterServerIp))
			{
				socket.connected += delegate()
				{
					ForgeMasterServer.RegisterServer(masterServerIp, (ushort)port, playerCount, "My Awesome Game Name", "Deathmatch", "Thank you for your support!", sceneName: sceneName);
				};

				socket.playerConnected += UpdatePlayerCount;
				socket.playerDisconnected += UpdatePlayerCount;
			}

			Go();
		}

		public void ServerBrowser()
		{
			Application.LoadLevel(serverBrowserScene);
		}

		public void StartClient()
		{
			host = ipAddressInput.text;

			if (string.IsNullOrEmpty(host.Trim()))
			{
				Debug.Log("No ip address provided to connect to");
				return;
			}

			socket = Networking.Connect(host, (ushort)port, protocolType, IsWinRT);
			Go();
		}

		public void TCPLocal()
		{
			socket = Networking.Connect("127.0.0.1", (ushort)port, protocolType, IsWinRT);
			Go();
		}

		public void StartClientLan()
		{
#if !NETFX_CORE
			System.Net.IPEndPoint endpoint = Networking.LanDiscovery((ushort)port, 5000, protocolType, IsWinRT);
#else
			IPEndPointWinRT endpoint = Networking.LanDiscovery((ushort)port, 5000, protocolType, IsWinRT);
#endif
			if (endpoint == null)
			{
				Debug.Log("No server found on LAN");
				return;
			}

			string ipAddress = string.Empty;
			ushort targetPort = 0;

#if !NETFX_CORE
			ipAddress = endpoint.Address.ToString();
			targetPort = (ushort)endpoint.Port;
#else
			ipAddress = endpoint.ipAddress;
			targetPort = (ushort)endpoint.port;
#endif

			socket = Networking.Connect(ipAddress, targetPort, protocolType, IsWinRT);
			Go();
		}

		private void Go()
		{
			if (proximityBasedUpdates)
				socket.MakeProximityBased(proximityDistance);

			socket.serverDisconnected += delegate(string reason)
			{
				MainThreadManager.Run(() =>
				{
					Debug.Log("The server kicked you for reason: " + reason);
#if UNITY_EDITOR
					UnityEditor.EditorApplication.isPlaying = false;
#endif
				});
			};

			if (socket.Connected)
				MainThreadManager.Run(LoadScene);
			else
				socket.connected += LoadScene;
		}

		private void LoadScene()
		{
			Networking.SetPrimarySocket(socket);
			Application.LoadLevel(sceneName);
		}
	}
}