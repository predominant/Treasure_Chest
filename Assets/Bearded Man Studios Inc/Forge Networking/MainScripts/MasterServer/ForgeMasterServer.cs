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
using System.Linq;

#if !NETFX_CORE
using System.Net;
using System.Threading;
#endif
using System.Collections.Generic;

namespace BeardedManStudios.Network
{
	public class ForgeMasterServer : MonoBehaviour
	{
		private const string REGISTER_SERVER = "BMS_INTERNAL_Register_Server";
		private const string UNREGISTER_SERVER = "BMS_INTERNAL_UnRegister_Server";
		private const string UPDATE_SERVER = "BMS_INTERNAL_Update_Server";
		private const string GET_HOSTS = "BMS_INTERNAL_Get_Hosts";
		public const int COUNT_PER_PAGE = 15;

		public const ushort PORT = 15939;																		// Port number

		private static Action<HostInfo[]> requestHostsCallback = null;

		private int playerCount = 1024;																			// Maximum player count -- excluding this server

		private NetWorker socket = null;																		// The initial connection socket

		private List<HostInfo> hosts = new List<HostInfo>();
		
#if !NETFX_CORE
		private Thread pingThread = null;
#endif

		public int sleepTime = 2500;
		public int timeoutTime = 10000;

		private void PingHosts()
		{
			while (true)
			{
				for (int i = 0; i < hosts.Count; i++)
				{
					if ((DateTime.Now - hosts[i].lastPing).TotalMilliseconds > timeoutTime)
					{
						Debug.Log("Removing Host " + hosts[i].IpAddress + ":" + hosts[i].port);
						hosts.RemoveAt(i--);
					}
					else
						((CrossPlatformUDP)socket).Ping(null, new IPEndPoint(IPAddress.Parse(hosts[i].IpAddress), hosts[i].port));
				}
				
#if !NETFX_CORE
				Thread.Sleep(sleepTime);
#endif
			}
		}

		private void PingRecieved(string ip)
		{
			try
			{
				var host = hosts.First(h => h.IpAddress + "+" + h.port == ip);

				if (host != null)
				{
					host.lastPing = DateTime.Now;
					Debug.Log("Ping on host: " + host.IpAddress + ":" + host.port);
				}
			}
			catch
			{

			}
		}

		public void Start()
		{
			StartServer();
			Networking.PrimarySocket.AddCustomDataReadEvent(REGISTER_SERVER, RegisterServerRequest);
			Networking.PrimarySocket.AddCustomDataReadEvent(UNREGISTER_SERVER, UnRegisterServerRequest);
			Networking.PrimarySocket.AddCustomDataReadEvent(UPDATE_SERVER, UpdateServerRequest);
			Networking.PrimarySocket.AddCustomDataReadEvent(GET_HOSTS, GetHostsRequestToServer);

			((CrossPlatformUDP)Networking.PrimarySocket).pingEvent += PingRecieved;

#if !NETFX_CORE
			pingThread = new Thread(PingHosts);
			pingThread.Start();
#endif
		}

		private static void Request(string host, Action<NetWorker> call)
		{
			if (Networking.Sockets != null && Networking.Sockets.ContainsKey(PORT))
				Networking.Disconnect(PORT);

			NetWorker socket = Networking.Connect(host, PORT, Networking.TransportationProtocolType.UDP);
			socket.MasterServerFlag = true;

			socket.AddCustomDataReadEvent(REGISTER_SERVER, null);
			socket.AddCustomDataReadEvent(UNREGISTER_SERVER, null);
			socket.AddCustomDataReadEvent(UPDATE_SERVER, null);
			socket.AddCustomDataReadEvent(GET_HOSTS, GetHostsRequestToClient);

			if (socket.Connected)
				call(socket);
			else
				socket.connected += delegate() { call(socket); };
		}

		public static void RegisterServer(string host, ushort port, int maxPlayers, string name, string gameType = "", string comment = "", string password = "", string sceneName = "")
		{
			Action<NetWorker> call = delegate(NetWorker socket)
			{
				BMSByte data = new BMSByte();
				ObjectMapper.MapBytes(data, port, maxPlayers, name, gameType, comment, password, sceneName);
				Networking.WriteCustom(REGISTER_SERVER, socket, data, true, NetworkReceivers.Server);
			};

			Request(host, call);
		}

		public static void UnRegisterServer(string host, ushort port)
		{
			Action<NetWorker> call = delegate(NetWorker socket)
			{
				BMSByte data = new BMSByte();
				ObjectMapper.MapBytes(data, port);
				Networking.WriteCustom(UNREGISTER_SERVER, socket, data, true, NetworkReceivers.Server);
			};

			Request(host, call);
		}

		private void RegisterServerRequest(NetworkingPlayer sender, NetworkingStream stream)
		{
			ushort port = ObjectMapper.Map<ushort>(stream);
			int maxPlayers = ObjectMapper.Map<int>(stream);
			string name = ObjectMapper.Map<string>(stream);
			string gameType = ObjectMapper.Map<string>(stream);
			string comment = ObjectMapper.Map<string>(stream);
			string password = ObjectMapper.Map<string>(stream);
			string sceneName = ObjectMapper.Map<string>(stream);

			HostInfo host = null;

			if (hosts.Count > 0)
				host = hosts.First(h => h.IpAddress.Split('+')[0] == sender.Ip && h.port == port);

			if (host == null)
			{
				hosts.Add(new HostInfo() { ipAddress = sender.Ip, port = port, maxPlayers = maxPlayers, name = name, gameType = gameType, comment = comment, password = password, sceneName = sceneName, lastPing = DateTime.Now });
				Debug.Log("Registered a new server " + sender.Ip.Split('+')[0] + ":" + port);
			}
			else
			{
				host.name = name;
				host.maxPlayers = maxPlayers;
				host.gameType = gameType;
				host.comment = comment;
				host.password = password;
				host.sceneName = sceneName;
				host.lastPing = DateTime.Now;
				Debug.Log("Updated the registration of a server " + host.IpAddress+ ":" + host.port);
			}

			socket.Disconnect(sender, "Register Complete");
		}

		private void UnRegisterServerRequest(NetworkingPlayer sender, NetworkingStream stream)
		{
			ushort port = ObjectMapper.Map<ushort>(stream);

			for (int i = 0; i < hosts.Count; i++)
			{
				if (hosts[i].ipAddress == sender.Ip && hosts[i].port == port)
				{
					hosts.RemoveAt(i);
					break;
				}
			}

			socket.Disconnect(sender, "UnRegister Complete");
		}

		// TODO:  Support updating the master server on scene change to "Application.loadedLevelName"

		public static void UpdateServer(string host, ushort port, int playerCount)
		{
			Action<NetWorker> call = delegate(NetWorker socket)
			{
				BMSByte data = new BMSByte();
				ObjectMapper.MapBytes(data, port, playerCount);
				Networking.WriteCustom(UPDATE_SERVER, socket, data, true, NetworkReceivers.Server);
			};

			Request(host, call);
		}

		private void UpdateServerRequest(NetworkingPlayer sender, NetworkingStream stream)
		{
			ushort port = ObjectMapper.Map<ushort>(stream);

			var host = hosts.First(h => h.IpAddress == sender.Ip.Split('+')[0] && h.port == port);

			if (host == null)
			{
				socket.Disconnect(sender, "Host not found");
				return;
			}

			host.connectedPlayers = ObjectMapper.Map<int>(stream);
			socket.Disconnect(sender, "Update Complete");
			Debug.Log("Updated a server " + host.IpAddress + ":" + host.port);
		}

		public static void GetHosts(string host, ushort pageNumber, Action<HostInfo[]> callback)
		{
			requestHostsCallback = callback;

			Action<NetWorker> call = delegate(NetWorker socket)
			{
				BMSByte data = new BMSByte();
				ObjectMapper.MapBytes(data, pageNumber);
				Networking.WriteCustom(GET_HOSTS, socket, data, true, NetworkReceivers.Server);
			};

			Request(host, call);
		}

		private void GetHostsRequestToServer(NetworkingPlayer sender, NetworkingStream stream)
		{
			ushort pageNumber = ObjectMapper.Map<ushort>(stream);

			List<HostInfo> subList = new List<HostInfo>();
			for (int i = pageNumber * COUNT_PER_PAGE; i < COUNT_PER_PAGE; i++)
			{
				if (hosts.Count <= i)
					break;

				subList.Add(hosts[i]);
			}

			BMSByte data = new BMSByte();
			ObjectMapper.MapBytes(data, subList.Count);

			foreach (HostInfo host in hosts)
				ObjectMapper.MapBytes(data, host.IpAddress, host.port, host.maxPlayers, host.name, host.password, host.gameType, host.connectedPlayers, host.comment, host.sceneName);

			Networking.WriteCustom(GET_HOSTS, socket, data, sender, true);
		}

		private static void GetHostsRequestToClient(NetworkingPlayer sender, NetworkingStream stream)
		{
			int count = ObjectMapper.Map<int>(stream);

			List<HostInfo> hostList = new List<HostInfo>();
			for (int i = 0; i < count; i++)
			{
				hostList.Add(new HostInfo()
				{
					ipAddress = ObjectMapper.Map<string>(stream),
					port = ObjectMapper.Map<ushort>(stream),
					maxPlayers = ObjectMapper.Map<int>(stream),
					name = ObjectMapper.Map<string>(stream),
					password = ObjectMapper.Map<string>(stream),
					gameType = ObjectMapper.Map<string>(stream),
					connectedPlayers = ObjectMapper.Map<int>(stream),
					comment = ObjectMapper.Map<string>(stream),
					sceneName = ObjectMapper.Map<string>(stream)
				});
			}

			Networking.Disconnect(PORT);
			requestHostsCallback(hostList.ToArray());
		}

		/// <summary>
		/// This method is called when the host server button is clicked
		/// </summary>
		public void StartServer()
		{
			// Create a host connection
			socket = Networking.Host(PORT, Networking.TransportationProtocolType.UDP, playerCount, false);
			Networking.SetPrimarySocket(socket);
		}

		private void OnApplicationQuit()
		{
			pingThread.Abort();
		}
	}
}