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



//using System.Collections.Generic;
//using System.ComponentModel;

//#if NETFX_CORE
//using System.Threading.Tasks;
//#endif

//namespace BeardedManStudios.Network
//{
//	public class ArbiterServer
//	{
//		private const ushort TCP_PORT = 19375;
//		private const ushort UDP_PORT = 19372;

//		public static string MasterServerIp { get; private set; }
//		public static string GameKey { get; private set; }
//		public static ushort MasterServerPort { get; private set; }
//		public static ushort MasterServerPortUDP { get; private set; }

//		public delegate void HostEvent(HostInfo[] hosts);
//		public event HostEvent receivedHostList
//		{
//			add
//			{
//				receivedHostListInvoker += value;
//			}
//			remove
//			{
//				receivedHostListInvoker -= value;
//			}
//		} HostEvent receivedHostListInvoker;	// Because iOS doesn't have a JIT - Multi-cast function pointer.

//		public static void SetupMasterServerConnection(string gameKey, string masterServerIp)
//		{
//			GameKey = gameKey;
//			MasterServerIp = masterServerIp;
//			MasterServerPort = TCP_PORT;
//			MasterServerPortUDP = UDP_PORT;
//		}

//		public static void SetupMasterServerConnection(string gameKey, string masterServerIp, ushort masterServerPort, ushort masterServerPortUDP)
//		{
//			GameKey = gameKey;
//			MasterServerIp = masterServerIp;
//			MasterServerPort = masterServerPort;
//			MasterServerPortUDP = masterServerPortUDP;
//		}

//		public static void RegisterHost(ushort gamePort, int maxPlayers, string comment = "")
//		{
//			if (string.IsNullOrEmpty(MasterServerIp))
//				throw new NetworkException(3, "SetupMasterServerConnection must first be called before being able to communicate with the master server");

//#if NETFX_CORE
//			Task tSend = Task.Run(() =>
//			{
//				WinMobileClient client = new WinMobileClient();

//				BMSByte tmp = new BMSByte();
//				ObjectMapper.MapBytes(tmp, "register", GameKey, gamePort, maxPlayers, comment);

//				client.ConnectAndWrite(MasterServerIp, MasterServerPort, new NetworkingStream().Prepare(client,
//					NetworkingStream.IdentifierType.None, null, tmp));
//			});
//			tSend.Wait();
//#else
//			DefaultClientTCP client = new DefaultClientTCP();

//			BMSByte tmp = new BMSByte();
//			ObjectMapper.MapBytes(tmp, "register", GameKey, gamePort, maxPlayers, comment);

//			client.ConnectAndWrite(MasterServerIp, MasterServerPort, new NetworkingStream().Prepare(client,
//				NetworkingStream.IdentifierType.None, null, tmp));
//#endif
//		}

//		public static void PingHost()
//		{
//			if (string.IsNullOrEmpty(MasterServerIp))
//				return;

//			BMSByte tmp = new BMSByte();
//			ObjectMapper.MapBytes(tmp, "ping", GameKey);

//			CrossPlatformUDP.Write(MasterServerIp, MasterServerPortUDP, "BMS_INTERNAL_Ping_Host", tmp, false);
//		}

//		public static HostInfo[] GetHosts()
//		{
//			if (string.IsNullOrEmpty(MasterServerIp))
//				throw new NetworkException(3, "SetupMasterServerConnection must first be called before you can communicate with the master server");

//			NetworkingStream response = null;
//#if NETFX_CORE
//			Task tSend = Task.Run(() =>
//			{
//				WinMobileClient client = new WinMobileClient();

//				BMSByte tmp = new BMSByte();
//				ObjectMapper.MapBytes(tmp, "hosts", GameKey);

//				response = client.ConnectAndRead(MasterServerIp, MasterServerPort, new NetworkingStream().Prepare(client,
//					NetworkingStream.IdentifierType.None, null, tmp));
//			});
//			tSend.Wait();
//#else
//			DefaultClientTCP client = new DefaultClientTCP();

//			BMSByte tmp = new BMSByte();
//			ObjectMapper.MapBytes(tmp, "hosts", GameKey);

//			response = client.ConnectAndRead(MasterServerIp, MasterServerPort, new NetworkingStream().Prepare(client,
//				NetworkingStream.IdentifierType.None, null, tmp));
//#endif

//			return ReadHostList(response);
//		}

//		private static HostInfo[] ReadHostList(NetworkingStream stream)
//		{
//			List<HostInfo> hosts = new List<HostInfo>();

//			if (stream == null)
//				return hosts.ToArray();

//			int hostCount = (int)ObjectMapper.Map<int>(stream);

//			if (hostCount > 4)
//				hosts = new List<HostInfo>(hostCount);

//			for (int i = 0; i < hostCount; i++)
//			{
//				HostInfo host = HostInfo.Deserialize(stream);
//				hosts.Add(host);
//			}

//			return hosts.ToArray();
//		}

//		public static void UnregisterHost(ushort port)
//		{
//			if (string.IsNullOrEmpty(MasterServerIp))
//				throw new NetworkException(3, "SetupMasterServerConnection must first be called before you can communicate with the master server");

//			BMSByte tmp = new BMSByte();
//			ObjectMapper.MapBytes(tmp, "unregister", GameKey, port);

//			CrossPlatformUDP.Write(MasterServerIp, MasterServerPortUDP, "BMS_INTERNAL_Unregister_Host", tmp, false);
//		}

//		public static ushort[] GetRoomListsForScene(string sceneName)
//		{
//			if (string.IsNullOrEmpty(MasterServerIp))
//				throw new NetworkException(3, "SetupMasterServerConnection must first be called before you can communicate with the master server");

//			NetworkingStream response = null;
//#if NETFX_CORE
//			Task tSend = Task.Run(() =>
//			{
//				WinMobileClient client = new WinMobileClient();

//				BMSByte tmp = new BMSByte();
//				ObjectMapper.MapBytes(tmp, "rooms", sceneName);

//				response = client.ConnectAndRead(MasterServerIp, MasterServerPort, new NetworkingStream(Networking.ProtocolType.QuickTCP).Prepare(client,
//					NetworkingStream.IdentifierType.None, null, tmp));
//			});
//			tSend.Wait();
//#else
//			DefaultClientTCP client = new DefaultClientTCP();

//			BMSByte tmp = new BMSByte();
//			ObjectMapper.MapBytes(tmp, "rooms", sceneName);

//			response = client.ConnectAndRead(MasterServerIp, MasterServerPort, new NetworkingStream(Networking.ProtocolType.QuickTCP).Prepare(client,
//				NetworkingStream.IdentifierType.None, null, tmp));
//#endif

//			if (response == null)
//				return null;

//			List<ushort> ports = new List<ushort>(response.Bytes.Size / sizeof(ushort));
//			while (response.ByteReadIndex < response.Bytes.Size)
//				ports.Add(ObjectMapper.Map<ushort>(response));

//			return ports.ToArray();
//		}

//		public static void JoinFirstOpenRoom(string sceneName)
//		{
//			ushort[] ports = GetRoomListsForScene(sceneName);

//			if (ports == null)
//				throw new NetworkException(2, "Could not communicate with host");

//			UnityHandle.TeleportToRoom(sceneName, MasterServerIp, ports[0]);
//		}
//	}
//}