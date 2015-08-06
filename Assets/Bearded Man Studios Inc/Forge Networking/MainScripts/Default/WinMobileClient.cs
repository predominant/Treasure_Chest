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
using Windows.Networking;
using Windows.Networking.Sockets;

using System;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using Windows.Foundation;
using System.Collections.Generic;
#endif

namespace BeardedManStudios.Network
{
	public class WinMobileClient : WinMobileWorker
	{
#if !NETFX_CORE
		public WinMobileClient() : base() { }
		public override void Connect(string hostAddress, ushort port) { }
		public override void Disconnect() { }
		public override void TimeoutDisconnect() { }
		public override void Write(NetworkingPlayer player, NetworkingStream stream) { }
		public override void Write(NetworkingStream stream) { }
#else
		private StreamSocket socket;
		
		public WinMobileClient() : base()
		{
			socket = new StreamSocket();
			Unity.NetWorkerKiller.AddNetWorker(this);
		}
		~WinMobileClient() { Disconnect(); }
		
		protected override async void ConnectAsync(string hostAddress, ushort port)
		{
			Host = hostAddress;
			try
			{
				serverHost = new HostName(hostAddress);

				await socket.ConnectAsync(serverHost, port.ToString());

				Connected = true;
				OnConnected();
				ReadAsync();
			}
			catch (Exception e)
			{
				// If this is an unknown status,
				// it means that the error is fatal and retry will likely fail.
				if (SocketError.GetStatus(e.HResult) == SocketErrorStatus.Unknown)
					throw;
				
				ErrorDisconnect(e.Message);
			}
		}

		public async void ConnectAndWrite(string hostAddress, ushort port, NetworkingStream stream)
		{
			try
			{
				serverHost = new HostName(hostAddress);

				await socket.ConnectAsync(serverHost, port.ToString());

				Connected = true;
				OnConnected();
				
				SendAsync(stream);
				Disconnect();
			}
			catch (Exception e)
			{
				ErrorDisconnect(e.Message);
			}
		}

		public NetworkingStream ConnectAndRead(string hostAddress, ushort port, NetworkingStream stream)
		{
			try
			{
				serverHost = new HostName(hostAddress);

				// Try to connect asynchronously
				socket.ConnectAsync(serverHost, port.ToString());

				Connected = true;
				OnConnected();
				SendAsync(stream);

				byte[] bytes = null;

				Task tReadResponse = Task.Run(async () =>
				{
					DataReader reader = new DataReader(socket.InputStream);
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
				});

				tReadResponse.Wait();

				Disconnect();

				BMSByte tmp = new BMSByte();
				tmp.Clone(bytes);
				return new NetworkingStream(Networking.ProtocolType.TCP).Consume(this, null, tmp);
			}
			catch (Exception e)
			{
				ErrorDisconnect(e.Message);
			}

			return null;
		}
		
		public override void Disconnect()
		{
			if (socket != null)
			{
				socket.Dispose();
				socket = null;
			}
			
			base.Disconnect();
		}

		public override void TimeoutDisconnect()
		{
			// TODO:  Implement
		}

		protected override async void SendAsync(NetworkingStream stream)
		{
			try
			{
				DataWriter writer = new DataWriter(socket.OutputStream);
				//uint length = writer.MeasureString(message);
				writer.WriteBytes(stream.Bytes.byteArr);
				// Try to store (send?) synchronously
				await writer.StoreAsync();

				OnDataSent(stream);

				writer.DetachStream();
				writer.Dispose();
			}
			catch (Exception e)
			{
				// If this is an unknown status, 
				// it means that the error is fatal and retry will likely fail.
				if (SocketError.GetStatus(e.HResult) == SocketErrorStatus.Unknown)
					throw;
				
				ErrorDisconnect(e.Message);
			}
		}

		private BMSByte readBuffer = new BMSByte();
		protected override async void ReadAsync()
		{
			int milliseconds = 0;

			DataReader reader = new DataReader(socket.InputStream);
			//reader.InputStreamOptions = InputStreamOptions.Partial;
			while (true)
			{
				milliseconds = 0;

				try
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

					//UnityEngine.Debug.LogError(bytes.Length);

					readBuffer.Clone(bytes);
					NetworkingStream stream = new NetworkingStream().Consume(this, null, readBuffer);

					DataRead(null, stream);

					if (stream.identifierType == NetworkingStream.IdentifierType.Disconnect)
					{
						OnDisconnected(ObjectMapper.Map<string>(stream));
						Disconnect();
					}
				}
				catch (Exception e)
				{
					// If this is an unknown status,
					// it means that the error is fatal and retry will likely fail.
					if (SocketError.GetStatus(e.HResult) == SocketErrorStatus.Unknown)
						throw;
					
					ErrorDisconnect(e.Message);
				}

				await Task.Delay(ThreadSpeed);
			}
		}
#endif
	}
}