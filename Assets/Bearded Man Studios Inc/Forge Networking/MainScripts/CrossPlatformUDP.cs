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



using System;
using System.Collections.Generic;

#if NETFX_CORE
using Windows.Networking.Sockets;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Storage.Streams;
using Windows.Foundation;
using System.Runtime.InteropServices.WindowsRuntime;
#else
using System.Net.Sockets;
using System.ComponentModel;
using System.Net;
using System.Threading;
#endif

namespace BeardedManStudios.Network
{
	public class CrossPlatformUDP : NetWorker
	{
		/// <summary>
		/// The amount of time in milliseconds that must be waited before sending another ping
		/// </summary>
		public int MINIMUM_PING_WAIT_TIME = 1500;

		/// <summary>
		/// This is a percentage (between 0 and 1) to drop packets for testing
		/// </summary>
		public float packetDropSimulationChance = 0.0f;

		/// <summary>
		/// This is a time in milliseconds to delay packet reads by for testing
		/// </summary>
		public int networkLatencySimulationTime = 0;

		/// <summary>
		/// This is a list of the packets that are currently being delayed for testing
		/// </summary>
		private List<object> latencySimulationPackets = new List<object>();

		/// <summary>
		/// The thread that is responsible for managing the latency behaviors
		/// </summary>
		private Thread latencyThread = null;

#if NETFX_CORE
		//private DatagramSocket writeClient = null;
		private DatagramSocket readClient = null;
#else
		private CachedUdpClient readClient = null;
#endif

#if NETFX_CORE
		private Task reliableWorker = null;
#else
		private IPEndPoint groupEP = null;
		private BackgroundWorker readWorker = null;
#endif
		/// <summary>
		/// NetworkUDP response delegate
		/// </summary>
		/// <param name="endpoint">The endpoint for the NetworkUDP</param>
		/// <param name="stream">The stream of data receieved</param>
		public delegate void NetworkUDPMessageEvent(string endpoint, NetworkingStream stream);

		/// <summary>
		/// The event to hook into for when data is sent
		/// </summary>
		new public event NetworkMessageEvent dataSent
		{
			add
			{
				dataSentInvoker += value;
			}
			remove
			{
				dataSentInvoker -= value;
			}
		} NetworkMessageEvent dataSentInvoker;	// Because iOS is stupid - Multi-cast function pointer.

		/// <summary>
		/// The event to hook into for when the UDP data is read
		/// </summary>
		public event NetworkUDPMessageEvent udpDataRead
		{
			add
			{
				udpDataReadInvoker += value;
			}
			remove
			{
				udpDataReadInvoker -= value;
			}
		} NetworkUDPMessageEvent udpDataReadInvoker;	// Because iOS is stupid - Multi-cast function pointer.

		public delegate void PingEvent(string ipAndPort);
		public event PingEvent pingEvent
		{
			add
			{
				pingEventInvoker += value;
			}
			remove
			{
				pingEventInvoker -= value;
			}
		} PingEvent pingEventInvoker;

		/// <summary>
		/// Dictionary of all the client sockets on the CrossPlatformUDP (NetWorker)
		/// </summary>
		public Dictionary<string, NetworkingPlayer> clientSockets = new Dictionary<string, NetworkingPlayer>();

		/// <summary>
		/// Add a client to the CrossPlatformUDP (NetWorker)
		/// </summary>
		/// <param name="ip">Ip address of the player to add</param>
		/// <param name="player">Player we are adding</param>
		private void AddClient(string ip, NetworkingPlayer player)
		{
			if (!clientSockets.ContainsKey(ip))
			{
				clientSockets.Add(ip, player);

				if (Players == null)
					Players = new List<NetworkingPlayer>(clientSockets.Values);
				else
					Players.Add(player);
			}
		}

		private object removalMutex = new Object();

		// TODO: Optomize the following
		private Dictionary<NetworkingPlayer, Dictionary<uint, Dictionary<int, KeyValuePair<DateTime, Header[]>>>> multiPartPending = new Dictionary<NetworkingPlayer, Dictionary<uint, Dictionary<int, KeyValuePair<DateTime, Header[]>>>>();
		private Dictionary<NetworkingPlayer, Dictionary<uint, Dictionary<int, KeyValuePair<DateTime, Header[]>>>> reliableMultiPartPending = new Dictionary<NetworkingPlayer, Dictionary<uint, Dictionary<int, KeyValuePair<DateTime, Header[]>>>>();
		private NetworkingPlayer server = null;

		/// <summary>
		/// A list of all the Reliable Packets Cache being stored
		/// </summary>
		public Dictionary<uint, KeyValuePair<DateTime, KeyValuePair<NetworkingPlayer, List<BMSByte>>>> reliablePacketsCache = new Dictionary<uint, KeyValuePair<DateTime, KeyValuePair<NetworkingPlayer, List<BMSByte>>>>();

		private static Dictionary<uint, int> packetGroupIds = new Dictionary<uint, int>();
		private const int PACKET_GROUP_TIMEOUT = 5000;						// Milliseconds
		private DateTime previousPingTime;
		private bool sendNewPing = true;
		private double previousServerPing { get; set; }						// Milliseconds
		private static int plusPingTime = 20;
		private const int PAYLOAD_SIZE = 1024;
		private const int MAX_PACKET_SIZE = PAYLOAD_SIZE + sizeof(uint)		// update id
														 + sizeof(int)		// group id
														 + sizeof(ushort)	// group packet count
														 + sizeof(ushort);	// current group order id

		/// <summary>
		/// Whether or not this CrossPlatformUDP (NetWorker) is the server
		/// </summary>
		new public bool IsServer { get; private set; }

		private static Dictionary<string, uint> updateidentifiers = new Dictionary<string, uint>();

#if NETFX_CORE
		private IPEndPointWinRT hostEndpoint = null;
#else
		private IPEndPoint hostEndpoint = null;
#endif

#if !NETFX_CORE
		private BackgroundWorker reliableWorker = null;
#endif
		private static object reliableCacheMutex = new Object();

		private NetworkingStream readStream = new NetworkingStream();
		private NetworkingStream writeStream = new NetworkingStream();

		private class Header
		{
			public uint updateId;
			public int packetGroupId;
			public ushort packetCount;
			public ushort packetOrderId;
			public bool reliable;
			public BMSByte payload = new BMSByte();

			public Header(uint u, int g, ushort c, ushort o, bool r)
			{
				updateId = u;
				packetGroupId = g;
				packetCount = c;
				packetOrderId = o;
				reliable = r;
			}

			public Header(Header other)
			{
				Clone(other);
			}

			public void Clone(Header other)
			{
				updateId = other.updateId;
				packetGroupId = other.packetGroupId;
				packetCount = other.packetCount;
				packetOrderId = other.packetOrderId;
				reliable = other.reliable;
				payload.Clone(other.payload);
			}

			public void Clone(uint u, int g, ushort c, ushort o, bool r)
			{
				updateId = u;
				packetGroupId = g;
				packetCount = c;
				packetOrderId = o;
				reliable = r;
			}

			public void SetPayload(BMSByte b)
			{
				payload.Clone(b);
			}
		}

		/// <summary>
		/// Constructor for the CrossPlatformUDP (NetWorker)
		/// </summary>
		/// <param name="isServer">If this is the server</param>
		/// <param name="maxConnections">Maximum allowed connections on this CrossPlatformUDP (NetWorker)</param>
		/// <param name="isSimpleType">If this is a simple Quick UDP</param>
		public CrossPlatformUDP(bool isServer, int maxConnections, bool isSimpleType = false)
		{
			MaxConnections = maxConnections;
			IsServer = isServer;
			Players = new List<NetworkingPlayer>();
		}
		~CrossPlatformUDP() { Disconnect(); }

		public static ulong resendCount = 0;
#if NETFX_CORE
		private async void ResendReliableWorker()
#else
		private void ResendReliableWorker(object sender, DoWorkEventArgs e)
#endif
		{
			while (true)
			{
				if (networkLatencySimulationTime > 0)
					Thread.Sleep(networkLatencySimulationTime * 3);
#if NETFX_CORE
				if (!Connected)
					return;
#else
				if (reliableWorker.CancellationPending)
					return;
#endif

				if (!IsServer && sendNewPing && Connected && (DateTime.Now - previousPingTime).TotalMilliseconds >= MINIMUM_PING_WAIT_TIME)
				{
					sendNewPing = false;
					previousPingTime = DateTime.Now;
					Ping();
				}

				if (reliablePacketsCache.Count == 0)
				{
#if NETFX_CORE
					await Task.Delay(TimeSpan.FromMilliseconds(ThreadSpeed));
#else
					Thread.Sleep(ThreadSpeed);
#endif

					continue;
				}

				lock (reliableCacheMutex)
				{
					List<uint> keys = new List<uint>(reliablePacketsCache.Keys);
					foreach (uint key in keys)
					{
						if ((DateTime.Now - reliablePacketsCache[key].Key).TotalMilliseconds > previousServerPing + plusPingTime)
						{
							if (reliablePacketsCache[key].Value.Key == null)
								Write(key, null, true, reliablePacketsCache[key].Value.Value);
							else
								Write(key, reliablePacketsCache[key].Value.Key, null, true, reliablePacketsCache[key].Value.Value);

							reliablePacketsCache[key] = new KeyValuePair<DateTime, KeyValuePair<NetworkingPlayer, List<BMSByte>>>(DateTime.Now, reliablePacketsCache[key].Value);
						}
					}
				}

#if NETFX_CORE
				await Task.Delay(ThreadSpeed);
#else
				Thread.Sleep(ThreadSpeed);
#endif
			}
		}

#if NETFX_CORE
		private Task timeoutTask = null;
#endif

#if NETFX_CORE
		/// <summary>
		/// Connect with the CrossPlatformUDP (NetWorker) to a ip and port
		/// </summary>
		/// <param name="hostAddress">Ip address of the connection</param>
		/// <param name="port">Port of the connection</param>
		public async override void Connect(string hostAddress, ushort port)
		{
#else
		private Thread connector;

		/// <summary>
		/// Connect with the CrossPlatformUDP (NetWorker) to a ip and port
		/// </summary>
		/// <param name="hostAddress">Ip address of the connection</param>
		/// <param name="port">Port of the connection</param>
		public override void Connect(string hostAddress, ushort port)
		{
			Host = hostAddress;

			if (!IsServer && !Networking.IsBareMetal)
				SocketPolicyServer.CheckWebplayer(hostAddress);

			string localIp = "127.0.0.1";

			previousPingTime = DateTime.Now;

			connector = new Thread(new ParameterizedThreadStart(ThreadedConnect));
			connector.Start(new object[] { hostAddress, port, localIp });

			previousServerPing = 100;
		}

		private void LatencySimulator()
		{
			if (!IsServer)
				return;

			try
			{
				while (true)
				{
					if (latencySimulationPackets.Count > 0)
					{
						double time = (DateTime.Now - (DateTime)((object[])latencySimulationPackets[0])[0]).TotalMilliseconds;
						string endpoint = (string)((object[])latencySimulationPackets[0])[1];
						BMSByte bytes = (BMSByte)((object[])latencySimulationPackets[0])[2];
						bytes.ResetPointer();

						if (Math.Round(networkLatencySimulationTime - time) > 0)
							Thread.Sleep((int)Math.Round(networkLatencySimulationTime - time));

						lastReadTime = DateTime.Now;

						PacketReceived(endpoint, bytes);

						latencySimulationPackets.RemoveAt(0);
					}
					else
						Thread.Sleep(10);
				}
			}
			catch (Exception e)
			{
				UnityEngine.Debug.LogException(e);
			}
		}

#if NETFX_CORE
		private void ThreadedConnect(object hostAndPort)
#else
		private void ThreadedConnect(object hostAndPort)
#endif
		{
#if NETFX_CORE
			await System.Threading.Tasks.Task.Delay(System.TimeSpan.FromMilliseconds(50));
#else
			Thread.Sleep(50);
#endif

			string hostAddress = (string)((object[])hostAndPort)[0];
			ushort port = (ushort)((object[])hostAndPort)[1];

			//UnityEngine.Debug.Log("Connecting to address: " + hostAddress + " with port " + port);

#endif
			try
			{
				if (IsServer)
				{
#if NETFX_CORE
					readClient = new DatagramSocket();
					await readClient.BindServiceNameAsync(port.ToString());
#else
					//if (1 == 1)//useNat)
					//	NatHolePunch.RegisterNat(ref readClient, port);
					//else
					readClient = new CachedUdpClient(port);
					readClient.EnableBroadcast = true;
					Port = port;

					groupEP = new IPEndPoint(IPAddress.Any, 0);

#endif
					Me = new NetworkingPlayer(ServerPlayerCounter++, "127.0.0.1", readClient, "SERVER");
				}
				else
				{
					ushort serverPort = port;
					bool triedOnce = false;

#if NETFX_CORE
					readClient = new DatagramSocket();
					readClient.MessageReceived += ReadAsync;

					HostName serverHost = new HostName(hostAddress);

					await readClient.ConnectAsync(serverHost, serverPort.ToString());

					Port = port;

					if (!IsServer)
					{
						lastReadTime = DateTime.Now;
						timeoutTask = new Task(TimeoutCheck);
						timeoutTask.RunSynchronously();
					}
#else
					for (; ; port++)
					{
						try
						{
							readClient = new CachedUdpClient(port);
							hostEndpoint = new IPEndPoint(IPAddress.Parse(hostAddress), serverPort);
							break;
						}
						catch
						{
							if (triedOnce && port == serverPort)
								throw new Exception("Running UDP locally, the system looped all the way around and back to port " + serverPort + " and found no open ports to run on.");

							triedOnce = true;
						}
					}

					Port = port;
#endif
				}

				if (IsServer)
					OnConnected();

				if (previousServerPing == 0)
				{
#if NETFX_CORE
					// TODO:  Do a correct ping for this platform
					previousServerPing = 75;			
#endif
				}

#if NETFX_CORE
				reliableWorker = Task.Run((Action)ResendReliableWorker);
#else
				readWorker = new BackgroundWorker();
				readWorker.WorkerSupportsCancellation = true;
				readWorker.DoWork += ReadAsync;
				readWorker.RunWorkerAsync();

				reliableWorker = new BackgroundWorker();
				reliableWorker.WorkerSupportsCancellation = true;
				reliableWorker.DoWork += ResendReliableWorker;
				reliableWorker.RunWorkerAsync(hostAddress);
#endif

				if (!IsServer)
				{
					lock (writersBlockMutex)
					{
						if (updateidentifiers == null)
							updateidentifiers = new Dictionary<string, uint>();

						BMSByte tmp = new BMSByte();
						ObjectMapper.MapBytes(tmp, "connect", port);

						writeStream.SetProtocolType(Networking.ProtocolType.ReliableUDP);
						writeStream.Prepare(this, NetworkingStream.IdentifierType.None, null, tmp);
						Write("BMS_INTERNAL_Udp_Connect", writeStream, true);
					}
				}

				if (IsServer && networkLatencySimulationTime > 0.0f)
				{
					latencyThread = new Thread(LatencySimulator);
					latencyThread.Start();
				}
			}
			catch (Exception e)
			{
				OnError(e);
			}
		}

		/// <summary>
		/// Executes buffered RPC's on the servers
		/// </summary>
		public override void GetNewPlayerUpdates()
		{
			Me = new NetworkingPlayer(Uniqueidentifier, "127.0.0.1", null, string.Empty);

			lock (writersBlockMutex)
			{
				BMSByte tmp = new BMSByte();
				ObjectMapper.MapBytes(tmp, "update");

				writeStream.SetProtocolType(Networking.ProtocolType.ReliableUDP);
				writeStream.Prepare(this, NetworkingStream.IdentifierType.None, null, tmp);
				Write("BMS_INTERNAL_Udp_New_Player", writeStream, true);
				//System.Diagnostics.Debug.WriteLine("wrote player update request");
			}
		}

#if NETFX_CORE
		private void Disconnect(string id, DatagramSocket player, string reason = null)
#else
		private void Disconnect(string id, IPEndPoint endpoint, string reason = null)
#endif
		{
			if (!string.IsNullOrEmpty(reason))
			{
				lock (writersBlockMutex)
				{
					BMSByte tmp = new BMSByte();
					ObjectMapper.MapBytes(tmp, reason);

					writeStream.SetProtocolType(Networking.ProtocolType.UDP);
					writeStream.Prepare(this, NetworkingStream.IdentifierType.Disconnect, null, tmp);
#if NETFX_CORE
					WriteAndClose(id, player, writeStream);
#else
					WriteAndClose(id, endpoint, writeStream);
#endif
				}
			}
		}

		/// <summary>
		/// Disconnect a player on this CrossPlatformUDP (NetWorker)
		/// </summary>
		/// <param name="player">Player to disconnect</param>
		public override void Disconnect(NetworkingPlayer player, string reason = null)
		{
			base.Disconnect(player, reason);

			if (!string.IsNullOrEmpty(reason))
			{
#if NETFX_CORE
				Disconnect("BMS_INTERNAL_Udp_DC_Player_Reason", (DatagramSocket)player.SocketEndpoint, reason);
#else
				Disconnect("BMS_INTERNAL_Udp_DC_Player_Reason", (IPEndPoint)player.SocketEndpoint, reason);
#endif
			}

			lock (removalMutex)
			{
				if (clientSockets.ContainsKey(player.Ip))
				{
					clientSockets.Remove(player.Ip);
					Players.Remove(player);
				}

				if (multiPartPending.ContainsKey(player))
					multiPartPending.Remove(player);

				if (reliableMultiPartPending.ContainsKey(player))
					reliableMultiPartPending.Remove(player);

				OnPlayerDisconnected(player);
				CleanUDPRPCForPlayer(player);
			}
		}

		private void DisconnectCleanup(bool timeout = false)
		{
			//if (IsServer)
			//	NatHolePunch.DeRegisterNat();

#if !NETFX_CORE
			if (connector != null)
				connector.Abort();

			if (readWorker != null)
				readWorker.CancelAsync();

			if (latencyThread != null)
				latencyThread.Abort();
#endif

			if (reliableWorker != null)
			{
#if NETFX_CORE
				// TODO:  Make sure this is properly killed
				reliableWorker.Wait();
#else
				reliableWorker.CancelAsync();
#endif
			}

#if NETFX_CORE
			//if (!IsServer)
			//{
			//	if (writeClient != null)
			//		writeClient.Dispose();
			//}
#endif

			if (readClient != null)
			{
#if NETFX_CORE
				readClient.Dispose();
#else
				readClient.Close();
#endif
			}

			if (multiPartPending != null)
				multiPartPending.Clear();

			if (clientSockets != null)
				clientSockets.Clear();

			if (timeout)
				OnTimeoutDisconnected();
			else
				OnDisconnected();
		}

		public override void TimeoutDisconnect()
		{
			lock (writersBlockMutex)
			{
				BMSByte tmp = new BMSByte();
				ObjectMapper.MapBytes(tmp, "disconnect");

				writeStream.SetProtocolType(Networking.ProtocolType.ReliableUDP);
				writeStream.Prepare(this, NetworkingStream.IdentifierType.None, null, tmp);
				Write("BMS_INTERNAL_Udp_Disconnect", writeStream, true);
			}

			DisconnectCleanup(true);
		}

		/// <summary>
		/// Disconnect this CrossPlatformUDP (NetWorker)
		/// </summary>
		public override void Disconnect()
		{
			lock (writersBlockMutex)
			{
				BMSByte tmp = new BMSByte();
				ObjectMapper.MapBytes(tmp, "disconnect");

				writeStream.SetProtocolType(Networking.ProtocolType.ReliableUDP);
				writeStream.Prepare(this, NetworkingStream.IdentifierType.None, null, tmp);
				Write("BMS_INTERNAL_Udp_Disconnect", writeStream, true);
			}

			DisconnectCleanup();
		}

		private static object updateidentifiersMutex = new Object();
		private static void TestUniqueIdentifiers(string updateStringId)
		{
			lock (updateidentifiersMutex)
			{
				if (!updateidentifiers.ContainsKey(updateStringId))
					updateidentifiers.Add(updateStringId, (uint)updateidentifiers.Count);
			}
		}

		private List<BMSByte> PreparePackets(string updateStringId, NetworkingStream stream, bool reliable, NetworkingPlayer player = null)
		{
			TestUniqueIdentifiers(updateStringId);
			return PreparePackets(updateidentifiers[updateStringId], stream.Bytes, reliable);
		}

		private List<BMSByte> PreparePackets(uint updateId, NetworkingStream stream, bool reliable, NetworkingPlayer player = null)
		{
			return PreparePackets(updateId, stream.Bytes, reliable);
		}

		private object packetBufferMutex = new Object();
		private List<BMSByte> packetBufferList = new List<BMSByte>();
		private List<BMSByte> PreparePackets(uint updateId, BMSByte bytes, bool reliable, NetworkingPlayer player = null)
		{
			lock (packetBufferMutex)
			{
				ushort packetCount = (ushort)Math.Ceiling((float)bytes.Size / PAYLOAD_SIZE);

				if (packetBufferList.Count < packetCount)
					for (int i = 0; i < packetCount - packetBufferList.Count; i++)
						packetBufferList.Add(new BMSByte());

				if (!packetGroupIds.ContainsKey(updateId))
					packetGroupIds.Add(updateId, 0);
				else
					packetGroupIds[updateId]++;

				for (ushort i = 0; i < packetCount; i++)
				{
					packetBufferList[i].Clear();
					packetBufferList[i].Append(new byte[1] { 0 });

					//packet.AddRange(ObjectMapper.MapBytes(updateId, packetGroupId, packetCount, i, reliable ? (byte)1 : (byte)0));
					ObjectMapper.MapBytes(packetBufferList[i], updateId);
					ObjectMapper.MapBytes(packetBufferList[i], packetGroupIds[updateId]);
					ObjectMapper.MapBytes(packetBufferList[i], packetCount);
					ObjectMapper.MapBytes(packetBufferList[i], i);
					ObjectMapper.MapBytes(packetBufferList[i], reliable ? (byte)1 : (byte)0);

					if (bytes.Size - (i * PAYLOAD_SIZE) > PAYLOAD_SIZE)
						packetBufferList[i].BlockCopy(bytes.byteArr, bytes.StartIndex() + i * PAYLOAD_SIZE, PAYLOAD_SIZE);
					else
						packetBufferList[i].BlockCopy(bytes.byteArr, bytes.StartIndex() + i * PAYLOAD_SIZE, bytes.Size - (i * PAYLOAD_SIZE));
				}

				lock (reliableCacheMutex)
				{
					if (reliable)
					{
						List<BMSByte> packets = new List<BMSByte>();

						foreach (BMSByte b in packetBufferList)
							packets.Add(new BMSByte().Clone(b));

						if (!reliablePacketsCache.ContainsKey(updateId))
							reliablePacketsCache.Add(updateId, new KeyValuePair<DateTime, KeyValuePair<NetworkingPlayer, List<BMSByte>>>(DateTime.Now, new KeyValuePair<NetworkingPlayer, List<BMSByte>>(player, packets)));
						else
							reliablePacketsCache[updateId] = new KeyValuePair<DateTime, KeyValuePair<NetworkingPlayer, List<BMSByte>>>(DateTime.Now, new KeyValuePair<NetworkingPlayer, List<BMSByte>>(player, packets));
					}
				}
			}

			return packetBufferList;
		}

		private static Header packetHeader = null;
		private static Header GetPacketHeader(ref BMSByte buffer)
		{
			if (packetHeader == null)
			{
				packetHeader = new Header(
					BitConverter.ToUInt32(buffer.byteArr, buffer.StartIndex()),
					BitConverter.ToInt32(buffer.byteArr, buffer.StartIndex(sizeof(uint))),
					BitConverter.ToUInt16(buffer.byteArr, buffer.StartIndex(sizeof(uint) + sizeof(int))),
					BitConverter.ToUInt16(buffer.byteArr, buffer.StartIndex(sizeof(uint) + sizeof(int) + sizeof(ushort))),
					buffer.byteArr[buffer.StartIndex(sizeof(uint) + sizeof(int) + sizeof(ushort) + sizeof(ushort))] == 1
				);
			}
			else
			{
				packetHeader.Clone(
					BitConverter.ToUInt32(buffer.byteArr, buffer.StartIndex()),
					BitConverter.ToInt32(buffer.byteArr, buffer.StartIndex(sizeof(uint))),
					BitConverter.ToUInt16(buffer.byteArr, buffer.StartIndex(sizeof(uint) + sizeof(int))),
					BitConverter.ToUInt16(buffer.byteArr, buffer.StartIndex(sizeof(uint) + sizeof(int) + sizeof(ushort))),
					buffer.byteArr[buffer.StartIndex(sizeof(uint) + sizeof(int) + sizeof(ushort) + sizeof(ushort))] == 1
				);
			}

			// The last addition is the size of the payload as it is not needed in this UDP implementation (if first packet)
			if (packetHeader.packetOrderId == 0)
				buffer.RemoveStart(sizeof(uint) + sizeof(int) + sizeof(ushort) + sizeof(ushort) + sizeof(byte) + sizeof(int));
			else
				buffer.RemoveStart(sizeof(uint) + sizeof(int) + sizeof(ushort) + sizeof(ushort) + sizeof(byte));

			if (buffer.Size <= 0)
				return null;

			packetHeader.SetPayload(buffer);

			return packetHeader;
		}

		/// <summary>
		/// Write the data on a given CrossPlatformUDP(NetWorker) from a ip, port, id, data and reliability
		/// </summary>
		/// <param name="hostAddress">Ip address of the host</param>
		/// <param name="port">Port of the host</param>
		/// <param name="updateidentifier">Unique update identifier to be used</param>
		/// <param name="data">Data to send</param>
		/// <param name="reliable">If this is a reliable send</param>
		[Obsolete("Static write methods on the UDP library are no longer supported")]
		public static void Write(string hostAddress, ushort port, string updateidentifier, BMSByte data, bool reliable)
		{
			throw new NotImplementedException("This function is no longer supported");
			//Write(hostAddress, port, updateidentifier, new NetworkingStream(Networking.ProtocolType.QuickUDP).Prepare(null,
			//	NetworkingStream.IdentifierType.None, null, data
			//), reliable);
		}

		/// <summary>
		/// Write the data on a given CrossPlatformUDP(NetWorker) from a ip, port, id, networking stream and reliability
		/// </summary>
		/// <param name="hostAddress">Ip address of the host</param>
		/// <param name="port">Port of the host</param>
		/// <param name="updateidentifier">Unique update identifier to be used</param>
		/// <param name="stream">Data to send</param>
		/// <param name="reliable">If this is a reliable send</param>
		public static void Write(string hostAddress, ushort port, string updateidentifier, NetworkingStream stream, bool reliable)
		{
			throw new NotImplementedException("This method has become obsolete");
			//#if NETFX_CORE
			//			DatagramSocket client = new DatagramSocket();
			//			HostName serverHost = new HostName(hostAddress);

			//			Task tConnect = Task.Run(async () =>
			//			{
			//				await client.ConnectAsync(serverHost, port.ToString());
			//			});

			//			tConnect.Wait();

			//			DataWriter writer = new DataWriter(client.OutputStream);
			//			//uint length = writer.MeasureString(message);
			//			writer.WriteBuffer(stream.Bytes.byteArr.AsBuffer(), (uint)stream.Bytes.StartIndex(), (uint)stream.Bytes.Size);
			//			// Try to store (send?) synchronously

			//			Task tWrite = Task.Run(async () =>
			//			{
			//				await writer.StoreAsync();
			//			});
			//			tWrite.Wait();

			//			writer.DetachStream();
			//			writer.Dispose();
			//#else
			//			CachedUdpClient client = new CachedUdpClient(hostAddress, port);

			//			List<BMSByte> packets = PreparePackets(updateidentifier, stream, reliable);

			//			foreach (BMSByte packet in packets)
			//				client.Send(packet.Compress().byteArr, packet.Size, hostAddress, port);

			//			client.Close();
			//#endif
		}

#if NETFX_CORE
		private static NetworkingStream gotResponse = null;
#endif
		public static NetworkingStream WriteAndGetResponse(string hostAddress, ushort port, string updateidentifier, NetworkingStream stream)
		{
			// TODO:  Implement
			return null;
		}

		public override void Write(NetworkingPlayer player, NetworkingStream stream)
		{
			// TODO:  Implement
		}

#if NETFX_CORE
		private void WriteAndClose(string id, DatagramSocket targetSocket, NetworkingStream stream, bool reliable = false, List<BMSByte> packets = null)
#else
		private void WriteAndClose(string id, IPEndPoint endpoint, NetworkingStream stream, bool reliable = false, List<BMSByte> packets = null)
#endif
		{
			if (packets == null)
				packets = PreparePackets(id, stream, reliable);

			foreach (BMSByte packet in packets)
			{
				try
				{
#if NETFX_CORE
					Task tWrite = Task.Run(async () =>
					{
						DataWriter writer = new DataWriter(targetSocket.OutputStream);
						//uint length = writer.MeasureString(message);
						writer.WriteBuffer(packet.byteArr.AsBuffer(), (uint)packet.StartIndex(), (uint)packet.Size);
						await writer.StoreAsync();

						writer.DetachStream();
						writer.Dispose();
						targetSocket.Dispose();
					});

					tWrite.Wait();
#else
					readClient.Send(packet.Compress().byteArr, packet.Size, endpoint);
#endif
				}
				catch
				{
#if NETFX_CORE
					targetSocket.Dispose();
#endif
				}
			}
		}

		private void MakeRawReliable(string reliableStringId, NetworkingPlayer player, BMSByte data)
		{
			TestUniqueIdentifiers(reliableStringId);

			lock (reliableCacheMutex)
			{
				if (!reliablePacketsCache.ContainsKey(updateidentifiers[reliableStringId]))
					reliablePacketsCache.Add(updateidentifiers[reliableStringId], new KeyValuePair<DateTime, KeyValuePair<NetworkingPlayer, List<BMSByte>>>(DateTime.Now, new KeyValuePair<NetworkingPlayer, List<BMSByte>>(player, new List<BMSByte>() { data })));
				else
					reliablePacketsCache[updateidentifiers[reliableStringId]] = new KeyValuePair<DateTime, KeyValuePair<NetworkingPlayer, List<BMSByte>>>(DateTime.Now, new KeyValuePair<NetworkingPlayer, List<BMSByte>>(player, new List<BMSByte>() { data }));
			}
		}

		public override void WriteRaw(NetworkingPlayer player, BMSByte data, string reliableId = "")
		{
			if (data[0] != 1)
				data.InsertRange(0, new byte[] { 1 });

			if (!string.IsNullOrEmpty(reliableId))
				MakeRawReliable(reliableId, player, data);

			try
			{
#if NETFX_CORE
					Task tWrite = Task.Run(async () =>
					{
						DataWriter writer = null;
						if (IsServer)
							writer = new DataWriter(((DatagramSocket)player.SocketEndpoint).OutputStream);
						else
							writer = new DataWriter(readClient.OutputStream);

						//uint length = writer.MeasureString(message);
						writer.WriteBuffer(data.byteArr.AsBuffer(), (uint)data.StartIndex(), (uint)data.Size);
						// Try to store (send?) synchronously
						await writer.StoreAsync();
						
						writer.DetachStream();
						writer.Dispose();
					});

					tWrite.Wait();
#else
				if (IsServer)
					readClient.Send(data.Compress().byteArr, data.Size, (IPEndPoint)player.SocketEndpoint);
				else
					readClient.Send(data.Compress().byteArr, data.Size, hostEndpoint);
#endif
			}
			catch
			{
				Disconnect(player);
			}
		}

		public override void WriteRaw(BMSByte data, bool relayToServer = true, string reliableId = "")
		{
			if (data[0] != 1)
				data.InsertRange(0, new byte[] { 1 });

			if (!string.IsNullOrEmpty(reliableId))
				MakeRawReliable(reliableId, server, data);

			if (IsServer)
			{
				if (relayToServer)
				{
					// Make the server send the raw data to itself
					OnRawDataRead(Me, data);

					// The above function moves the pointer so swap it back
					data.MoveStartIndex(-1);
				}

				foreach (KeyValuePair<string, NetworkingPlayer> kv in clientSockets)
				{
					try
					{
#if NETFX_CORE
						Task tWrite = Task.Run(async () =>
						{
							DataWriter writer = new DataWriter(((DatagramSocket)kv.Value.SocketEndpoint).OutputStream);
							//uint length = writer.MeasureString(message);
							writer.WriteBuffer(data.byteArr.AsBuffer(), (uint)data.StartIndex(), (uint)data.Size);
							// Try to store (send?) synchronously
							await writer.StoreAsync();

							writer.DetachStream();
							writer.Dispose();
						});

						tWrite.Wait();
#else
						readClient.Send(data.Compress().byteArr, data.Size, (IPEndPoint)kv.Value.SocketEndpoint);
#endif
					}
					catch
					{
						Disconnect(kv.Value);
					}
				}
			}
			else
			{
				if (readClient != null)
				{
#if NETFX_CORE
					Task tWrite = Task.Run(async () =>
					{
						DataWriter writer = new DataWriter(readClient.OutputStream);
						//uint length = writer.MeasureString(message);
						writer.WriteBuffer(data.byteArr.AsBuffer(), (uint)data.StartIndex(), (uint)data.Size);
						// Try to store (send?) synchronously
						await writer.StoreAsync();

						writer.DetachStream();
						writer.Dispose();
					});

					tWrite.Wait();
#else
					readClient.Send(data.Compress().byteArr, data.Size, hostEndpoint);
#endif
				}

				// TODO:  RawDataSent
				//if (stream != null && dataSentInvoker != null)
				//	dataSentInvoker(stream);
			}
		}

		public object writersBlockMutex = new object();

		public void Ping(object endpoint = null, IPEndPoint overrideHost = null)
		{
			if (IsServer && endpoint == null && overrideHost == null)
				return;

			byte[] ping = new byte[1] { 3 };
			try
			{
#if NETFX_CORE
					Task tWrite = Task.Run(async () =>
					{
						DataWriter writer = null;
						if (IsServer)
							writer = new DataWriter(((DatagramSocket)endpoint).OutputStream);
						else
							writer = new DataWriter(readClient.OutputStream);
						
						writer.WriteBuffer(ping.AsBuffer(), (uint)0, (uint)ping.Length);
						await writer.StoreAsync();
						
						writer.DetachStream();
						writer.Dispose();
					});

					tWrite.Wait();
#else
				if (overrideHost != null)
					readClient.Send(ping, ping.Length, overrideHost);
				else
				{
					if (IsServer)
						readClient.Send(ping, ping.Length, (IPEndPoint)endpoint);
					else
						readClient.Send(ping, ping.Length, hostEndpoint);
				}
#endif
			}
			catch
			{
				if (!IsServer)
					Disconnect();
			}
		}

		/// <summary>
		/// Write the data on a given CrossPlatformUDP(NetWorker) from a id, player, networking stream and reliability
		/// </summary>
		/// <param name="updateidentifier">Unique update identifier to be used</param>
		/// <param name="player">Player to be written to server</param>
		/// <param name="stream">The stream of data to be written</param>
		/// <param name="reliable">If this is a reliable send</param>
		/// <param name="packets">Extra parameters being sent</param>
		public override void Write(uint updateidentifier, NetworkingPlayer player, NetworkingStream stream, bool reliable = false, List<BMSByte> packets = null)
		{
			if (packets == null)
				packets = PreparePackets(updateidentifier, stream, reliable, player);

			foreach (BMSByte packet in packets)
			{
				try
				{
#if NETFX_CORE
					Task tWrite = Task.Run(async () =>
					{
						DataWriter writer = null;
						if (IsServer)
							writer = new DataWriter(((DatagramSocket)player.SocketEndpoint).OutputStream);
						else
							writer = new DataWriter(readClient.OutputStream);
						//uint length = writer.MeasureString(message);
						writer.WriteBuffer(packet.byteArr.AsBuffer(), (uint)packet.StartIndex(), (uint)packet.Size);
						// Try to store (send?) synchronously
						await writer.StoreAsync();
						
						writer.DetachStream();
						writer.Dispose();
					});

					tWrite.Wait();
#else
					if (IsServer)
						readClient.Send(packet.Compress().byteArr, packet.Size, (IPEndPoint)player.SocketEndpoint);
					else
						readClient.Send(packet.Compress().byteArr, packet.Size, hostEndpoint);
#endif
				}
				catch
				{
					Disconnect(player);
				}
			}
		}

		/// <summary>
		/// Write the data on a given CrossPlatformUDP(NetWorker) from a id and networking stream
		/// </summary>
		/// <param name="updateidentifier">Unique update identifier to be used</param>
		/// <param name="stream">The stream of data to be written</param>
		/// <param name="reliable">If this is a reliable send</param>
		/// <param name="packets">Extra parameters being sent</param>
		public override void Write(uint updateidentifier, NetworkingStream stream, bool reliable = false, List<BMSByte> packets = null)
		{
			if (IsServer)
			{
				if (stream != null)
				{
					if (stream.Sender == null)
						stream.AssignSender(Me);

					if (stream.identifierType == NetworkingStream.IdentifierType.RPC && (stream.Receivers == NetworkReceivers.AllBuffered || stream.Receivers == NetworkReceivers.OthersBuffered))
						ServerBufferRPC(updateidentifier, stream);

					if (clientSockets.Count == 0 || stream.Receivers == NetworkReceivers.Server)
						return;

					if (packets == null)
						packets = PreparePackets(updateidentifier, stream, reliable);
				}
				else if (packets == null)
					throw new NetworkException("There is no message being sent on this request");

				foreach (KeyValuePair<string, NetworkingPlayer> kv in clientSockets)
				{
					if (stream != null)
					{
						if ((stream.Receivers == NetworkReceivers.Others || stream.Receivers == NetworkReceivers.OthersBuffered || stream.Receivers == NetworkReceivers.OthersProximity) && kv.Value == stream.Sender)
							continue;

						if (stream.Receivers == NetworkReceivers.OthersProximity || stream.Receivers == NetworkReceivers.AllProximity)
						{
							// If the receiver is out of range, do not update them with the message
							if (UnityEngine.Vector3.Distance(stream.Sender.Position, kv.Value.Position) > ProximityMessagingDistance)
								continue;
						}
					}

					foreach (BMSByte packet in packets)
					{
						try
						{
#if NETFX_CORE
							Task tWrite = Task.Run(async () =>
							{
								DataWriter writer = new DataWriter(((DatagramSocket)kv.Value.SocketEndpoint).OutputStream);
								//uint length = writer.MeasureString(message);
								writer.WriteBuffer(packet.byteArr.AsBuffer(), (uint)packet.StartIndex(), (uint)packet.Size);
								// Try to store (send?) synchronously
								await writer.StoreAsync();

								writer.DetachStream();
								writer.Dispose();
							});

							tWrite.Wait();
#else
							readClient.Send(packet.Compress().byteArr, packet.Size, (IPEndPoint)kv.Value.SocketEndpoint);
#endif
						}
						catch
						{
							Disconnect(kv.Value);
						}
					}
				}
			}
			else
			{
				if (packets == null)
					packets = PreparePackets(updateidentifier, stream, reliable);

				if (readClient != null)
				{
					foreach (BMSByte packet in packets)
					{
#if NETFX_CORE
						Task tWrite = Task.Run(async () =>
						{
							DataWriter writer = new DataWriter(readClient.OutputStream);
							//uint length = writer.MeasureString(message);
							writer.WriteBuffer(packet.byteArr.AsBuffer(), (uint)packet.StartIndex(), (uint)packet.Size);
							// Try to store (send?) synchronously
							await writer.StoreAsync();

							writer.DetachStream();
							writer.Dispose();
						});

						tWrite.Wait();
#else
						readClient.Send(packet.Compress().byteArr, packet.Size, hostEndpoint);
#endif
					}
				}

				if (stream != null && dataSentInvoker != null)
					dataSentInvoker(stream);
			}
		}

		/// <summary>
		/// Write the data on a given CrossPlatformUDP(NetWorker) from a id, player and networking stream
		/// </summary>
		/// <param name="updateidentifier">Unique update identifier to be used</param>
		/// <param name="player">Player to be written to server</param>
		/// <param name="stream">The stream of data to be written</param>
		/// <param name="reliable">If this is a reliable send</param>
		/// <param name="packets">Extra parameters being sent</param>
		public override void Write(string updateidentifier, NetworkingPlayer player, NetworkingStream stream, bool reliable = false, List<BMSByte> packets = null)
		{
			lock (writersBlockMutex)
			{
				if (!updateidentifiers.ContainsKey(updateidentifier))
					updateidentifiers.Add(updateidentifier, (uint)updateidentifiers.Count);

				Write(updateidentifiers[updateidentifier], player, stream, reliable, packets);
			}
		}

		/// <summary>
		/// Write the data on a given CrossPlatformUDP(NetWorker) from a id and networking stream
		/// </summary>
		/// <param name="updateidentifier">Unique update identifier to be used</param>
		/// <param name="stream">The stream of data to be written</param>
		/// <param name="reliable">If this is a reliable send</param>
		public override void Write(string updateidentifier, NetworkingStream stream, bool reliable = false)
		{
			if (!updateidentifiers.ContainsKey(updateidentifier))
				updateidentifiers.Add(updateidentifier, (uint)updateidentifiers.Count);

			Write(updateidentifiers[updateidentifier], stream, reliable);
		}

		// Obsolete
		public override void Write(NetworkingStream stream)
		{
			throw new NetworkException(4, "This method requires an updateidentifier, use the other Write method if unsure Write(id, stream)");
		}

		private byte[] NetworkInstantiateForBareMetal = new byte[] { 109, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
		private object rpcMutex = new object();
		private void ReadStream(string endpoint, NetworkingStream stream)
		{
			if (IsServer)
			{
				if (clientSockets.ContainsKey(endpoint))
					OnDataRead(clientSockets[endpoint], stream);
			}
			else
				OnDataRead(null, stream);

			if (stream.identifierType == NetworkingStream.IdentifierType.RPC)
			{
				if (!Networking.IsBareMetal)
				{
					lock (rpcMutex)
					{
						new NetworkingStreamRPC(stream);
					}
				}
				else
				{
					bool found = true;
					for (int i = 16; i < 16 + NetworkInstantiateForBareMetal.Length; i++)
					{
						if (stream.Bytes.byteArr[i] != NetworkInstantiateForBareMetal[i - 16])
						{
							found = false;
							break;
						}
					}

					if (found)
					{
						int current = 38;
						foreach (byte b in BitConverter.GetBytes(SimpleNetworkedMonoBehavior.GenerateUniqueId()))
							stream.Bytes.byteArr[current++] = b;
					}
				}
			}
		}

		private void StreamCompleted(string endpoint, Header header, NetworkingStream stream)
		{
			ReadStream(endpoint, stream);

			if (IsServer)
				RelayStream(header.updateId, stream);
		}

		private BMSByte readMessageBuffer = new BMSByte();
		private void ReadMessageFromPlayer(string endpoint, Header header, NetworkingPlayer sender, ref Dictionary<NetworkingPlayer, Dictionary<uint, Dictionary<int, KeyValuePair<DateTime, Header[]>>>> target, NetworkingStream stream)
		{
			if (!target[sender].ContainsKey(header.updateId))
				target[sender].Add(header.updateId, new Dictionary<int, KeyValuePair<DateTime, Header[]>>());

			if (!target[sender][header.updateId].ContainsKey(header.packetGroupId))
				target[sender][header.updateId].Add(header.packetGroupId, new KeyValuePair<DateTime, Header[]>(DateTime.Now, new Header[header.packetCount]));

			bool alreadyDone = true;
			foreach (Header head in target[sender][header.updateId][header.packetGroupId].Value)
			{
				if (head == null || head.payload == null || head.payload.Size == 0)
				{
					alreadyDone = false;
					break;
				}
			}

			if (alreadyDone)
				return;

			if (target[sender][header.updateId][header.packetGroupId].Value[header.packetOrderId] == null)
				target[sender][header.updateId][header.packetGroupId].Value[header.packetOrderId] = new Header(header);
			else
				target[sender][header.updateId][header.packetGroupId].Value[header.packetOrderId].Clone(header);

			bool done = true;
			readMessageBuffer.Clear();
			foreach (Header head in target[sender][header.updateId][header.packetGroupId].Value)
			{
				if (head == null || head.payload == null || head.payload.Size == 0)
				{
					done = false;
					break;
				}
				else
					readMessageBuffer.BlockCopy(head.payload.byteArr, stream.Bytes.StartIndex(), stream.Bytes.Size);
			}

			if (done)
			{
				if (stream == null)
					stream = new NetworkingStream(header.reliable ? Networking.ProtocolType.ReliableUDP : Networking.ProtocolType.UDP).Consume(this, sender, readMessageBuffer);

				CurrentStreamOwner = stream.Sender;

				StreamCompleted(endpoint, header, stream);

				// TODO:  Possibility that this is an older packet than the last, need to do better sorting
				//target[sender][header.updateId].Remove(header.packetGroupId);

				if (!header.reliable)
					multiPartPending[sender][header.updateId].Remove(header.packetGroupId);

				// TODO:  Write this message to all of the other clients
			}
			else
				target[sender][header.updateId][header.packetGroupId] = new KeyValuePair<DateTime, Header[]>(DateTime.Now, target[sender][header.updateId][header.packetGroupId].Value);
		}

		private BMSByte writeBuffer = new BMSByte();
		private BMSByte rawBuffer = new BMSByte();
		private void PacketReceived(string endpoint, BMSByte bytes)
		{
#if NETFX_CORE
			if (bytes.Size == 1)
			{
				Task tWrite = Task.Run(async () =>
				{
					DataWriter writer = new DataWriter(readClient.OutputStream);
					writer.WriteBuffer((new byte[1] { 1 }).AsBuffer(), 0, 1);
					await writer.StoreAsync();

					writer.DetachStream();
					writer.Dispose();
				});

				tWrite.Wait();
				return;
			}
#else
			if (bytes.Size == 1)
			{
				if (bytes.byteArr[0] == 3)
				{
					if (IsServer)
					{
						// TODO:  Implement ping for WinRT
#if !NETFX_CORE
						if (pingEventInvoker != null)
							pingEventInvoker(endpoint);

						if (clientSockets.ContainsKey(endpoint))
							Ping(clientSockets[endpoint].SocketEndpoint);
						else if (pingEventInvoker == null)
							Ping(null, groupEP);
#endif
					}
					else
					{
						previousServerPing = (DateTime.Now - previousPingTime).TotalMilliseconds;
						sendNewPing = true;
					}

					return;
				}

				readClient.Send(new byte[1] { 1 }, 1, groupEP);
				return;
			}
#endif
			bytes.MoveStartIndex(1);

			NetworkingPlayer sender = null;
			if (bytes.byteArr[0] != 0)
			{
				if (!IsServer)
					sender = server;
				else
				{
					if (clientSockets.ContainsKey(endpoint))
					{
						sender = clientSockets[endpoint];
						sender.Ping();
					}
				}

				rawBuffer.Clone(bytes);

				switch (rawBuffer.byteArr[0])
				{
					case 1:	// User raw write
						OnRawDataRead(sender, rawBuffer);
						if (IsServer) RelayRawStream(sender, bytes);
						break;
					case 2:	// Scene load raw write
						// The server is never told what scene to load
						if (IsServer) return;
						string sceneName = Encryptor.Encoding.GetString(rawBuffer.byteArr, 1, rawBuffer.Size - 1);
						BeardedManStudios.Network.Unity.MainThreadManager.Run(() => { UnityEngine.Application.LoadLevel(sceneName); });
						break;
					case 3:	// Cache request
						if (IsServer)
						{
							if (clientSockets.ContainsKey(endpoint))
							{
								sender = clientSockets[endpoint];
								Cache.NetworkRead(rawBuffer, sender);
							}
						}
						else
							Cache.NetworkRead(rawBuffer);
						break;
				}

				return;
			}

			readStream.Reset();
			Header header = GetPacketHeader(ref bytes);

			if (header == null)
				return;

			if (header.packetCount == 1)
			{
				if (header.reliable && clientSockets.ContainsKey(endpoint))
				{
					if (reliableMultiPartPending.ContainsKey(clientSockets[endpoint]))
					{
						if (reliableMultiPartPending[clientSockets[endpoint]].ContainsKey(header.updateId))
						{
							if (reliableMultiPartPending[clientSockets[endpoint]][header.updateId].ContainsKey(header.packetGroupId))
							{
								// This packet has already been read
								return;
							}
						}
					}
				}

				readStream.SetProtocolType(header.reliable ? Networking.ProtocolType.ReliableUDP : Networking.ProtocolType.UDP);
				if (readStream.Consume(this, null, header.payload) == null)
					if (!Networking.IsBareMetal)
						return;

				if (readStream.identifierType == NetworkingStream.IdentifierType.Player)
				{
					OnConnected();
					lock (writersBlockMutex)
					{
						writeBuffer.Clear();
						ObjectMapper.MapBytes(writeBuffer, "received");
						writeStream.SetProtocolType(Networking.ProtocolType.UDP);
						writeStream.Prepare(this, NetworkingStream.IdentifierType.None, null, writeBuffer);
						Write(header.updateId, server, writeStream, false);
					}
					return;
				}
			}

			if (!IsServer)
			{
				if (server == null)
				{
					server = new NetworkingPlayer(0, endpoint, hostEndpoint, "SERVER");

					reliableMultiPartPending.Add(server, new Dictionary<uint, Dictionary<int, KeyValuePair<DateTime, Header[]>>>());
					multiPartPending.Add(server, new Dictionary<uint, Dictionary<int, KeyValuePair<DateTime, Header[]>>>());
				}

				sender = server;

				if (readStream.Ready && readStream.identifierType == NetworkingStream.IdentifierType.Disconnect)
				{
					DisconnectCleanup();
					OnDisconnected(ObjectMapper.Map<string>(readStream));
					return;
				}
			}
			else
			{
				if (readStream.Ready)
				{
					// New player
					if (readStream.Bytes.Size < 22 && ObjectMapper.Compare<string>(readStream, "connect"))
					{
						lock (removalMutex)
						{
							// TODO:  In the future a player can connect with a pre-determined name
							string name = string.Empty;

#if NETFX_CORE
							DatagramSocket newConnection = new DatagramSocket();
							HostName serverHost = new HostName(endpoint);
							
							Task tConnect = Task.Run(async () =>
							{
								// Try to connect asynchronously
								if (endpoint == "127.0.0.1")
									await newConnection.ConnectAsync(serverHost, (port + 1).ToString());
								else
									await newConnection.ConnectAsync(serverHost, port.ToString());
							});
							tConnect.Wait();
#else
							//string[] hostPort = endpoint.Split(CachedUdpClient.HOST_PORT_CHARACTER_SEPARATOR);
							//string host = hostPort[0];

							// Remove connect from bytes
							ObjectMapper.Map<string>(readStream);

							// Remove the sender port from the stream
							ObjectMapper.Map<ushort>(readStream);
							//ushort senderPort = ObjectMapper.Map<ushort>(readStream);
#endif

							if (Connections >= MaxConnections)
							{
#if NETFX_CORE
								Disconnect("BMS_INTERNAL_DC_Max_Players", newConnection, "Max Players Reached On Server");
#else
								Disconnect("BMS_INTERNAL_DC_Max_Players", groupEP, "Max Players Reached On Server");
#endif

								return;
							}

							if (!clientSockets.ContainsKey(endpoint))
							{
#if NETFX_CORE
								sender = new NetworkingPlayer(ServerPlayerCounter++, endpoint, newConnection, name);
#else
								sender = new NetworkingPlayer(ServerPlayerCounter++, endpoint, new IPEndPoint(groupEP.Address, groupEP.Port), name);
#endif

								AddClient(endpoint, sender);

								reliableMultiPartPending.Add(sender, new Dictionary<uint, Dictionary<int, KeyValuePair<DateTime, Header[]>>>());
								multiPartPending.Add(sender, new Dictionary<uint, Dictionary<int, KeyValuePair<DateTime, Header[]>>>());

								OnPlayerConnected(sender);
							}
							else
							{
#if NETFX_CORE
								sender = new NetworkingPlayer(clientSockets[endpoint].NetworkId, endpoint, newConnection, name);
#else
								sender = new NetworkingPlayer(clientSockets[endpoint].NetworkId, endpoint, new IPEndPoint(groupEP.Address, groupEP.Port), name);
#endif

								reliableMultiPartPending.Remove(clientSockets[endpoint]);
								multiPartPending.Remove(clientSockets[endpoint]);

								// TODO:  Check why this isn't reset on disconnect
								clientSockets[endpoint] = sender;

								reliableMultiPartPending.Add(sender, new Dictionary<uint, Dictionary<int, KeyValuePair<DateTime, Header[]>>>());
								multiPartPending.Add(sender, new Dictionary<uint, Dictionary<int, KeyValuePair<DateTime, Header[]>>>());
							}

							lock (writersBlockMutex)
							{
								writeBuffer.Clear();
								ObjectMapper.MapBytes(writeBuffer, "received");
								writeStream.SetProtocolType(Networking.ProtocolType.UDP);
								writeStream.Prepare(this, NetworkingStream.IdentifierType.None, null, writeBuffer);
								Write(header.updateId, sender, writeStream, false);
							}

							lock (writersBlockMutex)
							{
								writeBuffer.Clear();
								ObjectMapper.MapBytes(writeBuffer, sender.NetworkId);
								writeStream.SetProtocolType(Networking.ProtocolType.ReliableUDP);
								writeStream.Prepare(this, NetworkingStream.IdentifierType.Player, null, writeBuffer);
								Write("BMS_INTERNAL_Set_Player_Id", sender, writeStream, true);
							}

							return;
						}
					}
					else
					{
						if (!clientSockets.ContainsKey(endpoint))
						{
							// This is a connect and write
							if (udpDataReadInvoker != null)
								udpDataReadInvoker(endpoint, readStream);

							return;
						}
					}
				}

				foreach (KeyValuePair<string, NetworkingPlayer> player in clientSockets)
				{
					if (player.Key == endpoint)
					{
						sender = player.Value;
						sender.Ping();
					}
					else
					{
						if ((DateTime.Now - player.Value.LastPing).TotalSeconds > player.Value.InactiveTimeoutSeconds)
						{
							Disconnect(player.Value, "Player timed out");
						}
					}
				}

				if (readStream.Ready)
				{
					// TODO:  These need to be done better since there are many of them
					if (readStream.Bytes.Size < 22)
					{
						try
						{
							if (ObjectMapper.Compare<string>(readStream, "update"))
								UpdateNewPlayer(sender);

							if (ObjectMapper.Compare<string>(readStream, "disconnect"))
							{
								// TODO:  If this eventually sends something to the player they will not exist
								Disconnect(sender);
								return;
							}
						}
						catch
						{
							throw new NetworkException(12, "Mal-formed defalut communication");
						}
					}
				}
			}

			if (readStream.Ready)
			{
				readStream.AssignSender(sender);
			}

			if (reliablePacketsCache.Count > 0 && readStream.Ready)
			{
				lock (reliableCacheMutex)
				{
					if (readStream.Bytes.Size < 22)
					{
						if (ObjectMapper.Compare<string>(readStream, "received"))
						{
							reliablePacketsCache.Remove(header.updateId);
							reliableMultiPartPending[sender].Remove(header.updateId);
							return;
						}
					}
				}
			}

			if (!IsServer && Uniqueidentifier == 0)
				return;

			if (header.reliable)
			{
				if (IsServer)
				{
					if (clientSockets.ContainsKey(endpoint))
					{
						lock (writersBlockMutex)
						{
							writeBuffer.Clear();
							ObjectMapper.MapBytes(writeBuffer, "received");

							writeStream.SetProtocolType(Networking.ProtocolType.ReliableUDP);
							writeStream.Prepare(this, NetworkingStream.IdentifierType.None, null, writeBuffer);
							Write(header.updateId, clientSockets[endpoint], writeStream, false);
						}
					}
				}
				else
				{
					lock (writersBlockMutex)
					{
						writeBuffer.Clear();
						ObjectMapper.MapBytes(writeBuffer, "received");

						writeStream.SetProtocolType(Networking.ProtocolType.ReliableUDP);
						writeStream.Prepare(this, NetworkingStream.IdentifierType.None, null, writeBuffer);
						Write(header.updateId, writeStream, false);
					}
				}

				ReadMessageFromPlayer(endpoint, header, sender, ref reliableMultiPartPending, readStream);
			}
			else
				ReadMessageFromPlayer(endpoint, header, sender, ref multiPartPending, readStream);
		}

		private string incomingEndpoint = string.Empty;
		private BMSByte readBuffer = new BMSByte();
#if NETFX_CORE
		byte[] readBytes = new byte[0];
		private async void ReadAsync(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
#else
		private void ReadAsync(object eventSender, DoWorkEventArgs e)
#endif
		{
#if NETFX_CORE
			DataReader reader = args.GetDataReader();

			readBytes = new byte[reader.UnconsumedBufferLength];

			reader.ReadBytes(readBytes);

			readBuffer.Clone(readBytes);

			if (!IsServer)
				lastReadTime = DateTime.Now;

			PacketReceived(sender.Information.LocalAddress.RawName, readBuffer);

			//ReadStream(args.RemoteAddress.DisplayName, Convert.ToUInt16(args.RemotePort), new NetworkingStream(protocolType).Consume(bytes));
#else
			if (!IsServer)
			{
				lastReadTime = DateTime.Now;
				Thread timeout = new Thread(TimeoutCheck);
				timeout.Start();
			}

			try
			{
				while (true)
				{
					if (readWorker.CancellationPending)
						return;

					readBuffer = readClient.Receive(ref groupEP, ref incomingEndpoint);

					if (packetDropSimulationChance > 0)
					{
						if (new System.Random().NextDouble() <= packetDropSimulationChance)
						{
							UnityEngine.Debug.Log("Packet Dropped!");
							continue;
						}
					}

					if (IsServer && networkLatencySimulationTime > 0 && clientSockets.ContainsKey(incomingEndpoint) && alreadyUpdated.Contains(clientSockets[incomingEndpoint]))
					{
						BMSByte tmp = new BMSByte().Clone(readBuffer);
						tmp.ResetPointer();

						latencySimulationPackets.Add(new object[] { DateTime.Now, incomingEndpoint, tmp });

						continue;
					}

					if (!IsServer)
						lastReadTime = DateTime.Now;

					PacketReceived(incomingEndpoint, readBuffer);
				}
			}
			catch (SocketException ex)
			{
				if (ex.ErrorCode != 10004)
				{
					// TODO:  In the master server capture this error and see who it is, then remove them from the hosts list

					if (Networking.IsBareMetal)
						Console.WriteLine(ex.Message + " | " + ex.StackTrace);
					else
					{
						// The host forcefully disconnected
						if (ex.ErrorCode == 10054)
						{
							OnDisconnected("The connection has been forcefully closed");
						}
#if UNITY_EDITOR
						else
						{

							UnityEngine.Debug.LogException(ex);
							UnityEngine.Debug.LogError("Error Code:" + ex.ErrorCode);
						}
#endif
					}
				}
			}
#if BMS_TRY_READ
			catch (Exception ex)
			{
				if (Networking.IsBareMetal)
					Console.WriteLine(ex.Message + " | " + ex.StackTrace);
				else
					UnityEngine.Debug.LogException(ex);
			}
#endif
#endif
		}
	}
}