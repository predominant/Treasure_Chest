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



#if NETFX_CORE
using System;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using System.Collections.Generic;
#endif

namespace BeardedManStudios.Network
{
	public class WinMobileServer : WinMobileWorker
	{
#if !NETFX_CORE
		public WinMobileServer(int maxConnections) : base(maxConnections) { }
		public override void Connect(string hostAddress, ushort port) { }
		public override void Disconnect() { }
		public override void TimeoutDisconnect() { }
		public override void Write(NetworkingPlayer player, NetworkingStream stream) { }
		public override void Write(NetworkingStream stream) { }
#else
		private StreamSocketListener socket = null;

		public WinMobileServer(int maxConnections) : base(maxConnections) { }
		~WinMobileServer() { Disconnect(); }

		public override void Connect(string hostAddress, ushort port)
		{
			Host = hostAddress;
			Players = new List<NetworkingPlayer>();
			ConnectAsync(string.Empty, port);
		}

		public override void Disconnect()
		{
			socket.Dispose();
			socket = null;
		}

		public override void TimeoutDisconnect()
		{
			// TODO:  Implement
		}

		private void ClientDisconnected(NetworkingPlayer player)
		{
			OnPlayerDisconnected(player);
			Players.Remove(player);
		}

		private void ClientDisconnected(int index)
		{
			OnPlayerDisconnected(Players[index]);
			Players.RemoveAt(index);
		}

		private async void WriteAndClose(StreamSocket targetSocket, NetworkingStream stream)
		{
			try
			{
				DataWriter writer = new DataWriter(targetSocket.OutputStream);
				writer.WriteBytes(stream.Bytes.byteArr);

				// Send synchronously
				await writer.StoreAsync();

				OnDataSent(stream);

				writer.DetachStream();
				writer.Dispose();
				targetSocket.Dispose();
			}
			catch (Exception e)
			{
				targetSocket.Dispose();
				//Networking.Error(e.Message);
			}
		}

		protected override async void SendAsync(NetworkingPlayer player, NetworkingStream stream)
		{
			try
			{
				DataWriter writer = new DataWriter(((StreamSocket)player.SocketEndpoint).OutputStream);
				writer.WriteBytes(stream.Bytes.byteArr);

				// Send synchronously
				await writer.StoreAsync();

				OnDataSent(stream);

				writer.DetachStream();
				writer.Dispose();
			}
			catch (Exception e)
			{
				ClientDisconnected(player);
				//Networking.Error(e.Message);
			}
		}

		protected override async void SendAsync(NetworkingStream stream)
		{
			if (stream.Receivers == NetworkReceivers.Server)
				return;

			for (int i = 0; i < Players.Count; i++)
			{
				if ((stream.Receivers == NetworkReceivers.Others || stream.Receivers == NetworkReceivers.OthersBuffered) && Players[i] == stream.Sender)
					continue;

				try
				{
					DataWriter writer = new DataWriter(((StreamSocket)Players[i].SocketEndpoint).OutputStream);
					writer.WriteBytes(stream.Bytes.byteArr);

					// Send synchronously
					await writer.StoreAsync();

					OnDataSent(stream);

					writer.DetachStream();
					writer.Dispose();
				}
				catch (Exception e)
				{
					ClientDisconnected(i);
					//Networking.Error(e.Message);
				}
			}
		}

		private BMSByte readBuffer = new BMSByte();
		protected override async void ReadAsync()
		{
			int milliseconds = 0;

			int i = 0;
			while (true)
			{
				milliseconds = 0;

				try
				{
					while (true)
					{
						for (i = i; i < Players.Count; i++)
						{
							// TODO:  Cache these objects to save on garbage collection
							DataReader reader = new DataReader(((StreamSocket)Players[i].SocketEndpoint).InputStream);
							
							if (reader.UnconsumedBufferLength > 0)
							{
								byte[] bytes = null;
								uint messageSize = await reader.LoadAsync(sizeof(uint));
								if (messageSize != sizeof(uint))
								{
									Disconnect();

									// socket was closed
									return;
								}

								bytes = new byte[messageSize];
								reader.ReadBytes(bytes);
								messageSize = BitConverter.ToUInt32(bytes, 0);
								await reader.LoadAsync(messageSize);

								bytes = new byte[messageSize];

								// TODO:  This may read the first 4 bytes again for the size, make sure it doesn't
								reader.ReadBytes(bytes);

								readBuffer.Clone(bytes);
								NetworkingStream stream = new NetworkingStream().Consume(this, Players[i], readBuffer);

								DataRead(Players[i], stream);

								// Write what was read to all the clients
								Write(new NetworkingStream(stream.ProtocolType).Prepare(this, stream.identifierType, stream.NetworkedBehavior, stream.Bytes));
							}
						}

						i = 0;
					}
				}
				catch (Exception e)
				{
					ClientDisconnected(i);
					//Networking.Error(e.Message);
				}

				await Task.Delay(ThreadSpeed);
			}
		}

		protected override async void ConnectAsync(string ip, ushort port)
		{
			socket = new StreamSocketListener();
			socket.Control.QualityOfService = SocketQualityOfService.Normal;
			socket.ConnectionReceived += ConnectionReceived;

			// Binding listener to a port
			try
			{
				await socket.BindServiceNameAsync(port.ToString());
				Connected = true;

				OnConnected();
			}
			catch (Exception e)
			{
				ErrorDisconnect(e.Message);
			}
		}

		private async void ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
		{
			BMSByte tmp = new BMSByte();

			if (Connections >= MaxConnections)
			{
				ObjectMapper.MapBytes(tmp, "Max Players Reached On Server");

				WriteAndClose(args.Socket, new NetworkingStream(Networking.ProtocolType.TCP).Prepare(this,
					NetworkingStream.IdentifierType.Disconnect, null, tmp));
				
				return;
			}

			// TODO:  Setup name
			string name = string.Empty;

			NetworkingPlayer player = new NetworkingPlayer(ServerPlayerCounter, args.Socket.Information.RemoteAddress.CanonicalName, args.Socket, name);
			Players.Add(player);
			OnPlayerConnected(player);

			tmp.Clear();
			ObjectMapper.MapBytes(tmp, player.NetworkId);

			Write(player, new NetworkingStream(Networking.ProtocolType.TCP).Prepare(this,
				NetworkingStream.IdentifierType.Player, null, tmp));
		}
#endif
	}
}