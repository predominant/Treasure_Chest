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



#if !NETFX_CORE
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;
using System.Threading;
#endif

namespace BeardedManStudios.Network
{
	public class DefaultClientTCP : TCPProcess
	{
#if NETFX_CORE
		public override void Connect(string hostAddress, ushort port) { }
		public override void Disconnect() { }
		public override void TimeoutDisconnect() { }
		public override void Disconnect(NetworkingPlayer player, string reason = "") { }
		public override void Write(NetworkingStream stream) { }
		public override void Write(NetworkingPlayer player, NetworkingStream stream) { }
#else
		private NetworkStream netStream = null;
		private TcpClient client = null;

		private Thread readWorker = null;

		private NetworkingStream writeStream = new NetworkingStream();
		private NetworkingStream readStream = new NetworkingStream();

		~DefaultClientTCP() { Disconnect(); }

		private Thread connector;

		/// <summary>
		/// Connect to a Ip Address with a supplied port
		/// </summary>
		/// <param name="hostAddress">Ip Address to connect to</param>
		/// <param name="port">Port to connect from</param>
		public override void Connect(string hostAddress, ushort port)
		{
			Host = hostAddress;

#if UNITY_WEBPLAYER
			if (UnityEngine.Application.isWebPlayer)
				UnityEngine.Security.PrefetchSocketPolicy(hostAddress, 843);	// TODO:  Make this configurable
#endif

			//UnityEngine.Debug.Log("Connecting to address: " + hostAddress + " with port " + port);

			connector = new Thread(new ParameterizedThreadStart(ThreadedConnect));
			connector.Start(new object[] { hostAddress, port });
		}

		private void ThreadedConnect(object hostAndPort)
		{
			string hostAddress = (string)((object[])hostAndPort)[0];
			ushort port = (ushort)((object[])hostAndPort)[1];

			try
			{
				// Create a TcpClient. 
				// The client requires a TcpServer that is connected 
				// to the same address specified by the server and port 
				// combination.
				client = new TcpClient(hostAddress, (int)port);

				// Get a client stream for reading and writing. 
				// Stream stream = client.GetStream();
				netStream = client.GetStream();

				readWorker = new System.Threading.Thread(new System.Threading.ThreadStart(ReadAsync));
				readWorker.Start();
			}
			catch
			{
				throw new NetworkException(1, "Host is invalid or not found");
			}
		}

		/// <summary>
		/// Disconnect from the server
		/// </summary>
		public override void Disconnect()
		{
			BMSByte tmp = new BMSByte();
			ObjectMapper.MapBytes(tmp, "disconnect");

			writeStream.SetProtocolType(Networking.ProtocolType.TCP);
			writeStream.Prepare(this, NetworkingStream.IdentifierType.Disconnect, null, tmp, NetworkReceivers.Server);

			Write(writeStream);

			if (readWorker != null)
				readWorker.Abort();

			if (netStream != null)
				netStream.Close();

			if (client != null)
				client.Close();

			OnDisconnected();
		}

		public override void TimeoutDisconnect()
		{
			// TODO:  Implement
			OnTimeoutDisconnected();
		}

		public override void Write(NetworkingPlayer player, NetworkingStream stream)
		{
			throw new NetworkException(11, "This is a method planned for the future and has not been implemented yet.");
		}

		/// <summary>
		/// Write to the server with a Networking Stream
		/// </summary>
		/// <param name="stream">Networking Stream to write</param>
		public override void Write(NetworkingStream stream)
		{
			if (!Connected)
				throw new NetworkException(5, "The network could not be written to because no connection has been opened");

			if (!netStream.CanWrite)
				return;

			// Send the message to the connected TcpServer. 
			netStream.Write(stream.Bytes.byteArr, stream.Bytes.StartIndex(), stream.Bytes.Size);
			OnDataSent(stream);
		}

		/// <summary>
		/// Get all the new player updates
		/// </summary>
		public override void GetNewPlayerUpdates()
		{
			Me = new NetworkingPlayer(Uniqueidentifier, "127.0.0.1", null, string.Empty);

			BMSByte tmp = new BMSByte();
			ObjectMapper.MapBytes(tmp, "update");

			writeStream.SetProtocolType(Networking.ProtocolType.TCP);
			writeStream.Prepare( this, NetworkingStream.IdentifierType.None, null, tmp, NetworkReceivers.Server);

			Write(writeStream);
		}

		private void StreamReceived(StreamRead obj)
		{
			StreamRead streamRead = obj;

			readStream.Consume(this, null, streamRead.bytes);

			if (readStream.identifierType == NetworkingStream.IdentifierType.Player)
				OnConnected();

			DataRead(null, readStream);

			if (readStream.identifierType == NetworkingStream.IdentifierType.Disconnect)
			{
				OnDisconnected(ObjectMapper.Map<string>(readStream));
				Disconnect();
			}
		}

		private void ReadAsync()
		{
			try
			{
				while (true)
				{
					if (!netStream.CanRead)
						break;

					if (netStream.DataAvailable)
					{
						do
						{
							readBuffer = ReadBuffer(netStream);

							if (readBuffer.Size > 0)
								StreamReceived(streamRead.Prepare(0, readBuffer));
						} while (backBuffer.Size > 0);
					}

					Thread.Sleep(ThreadSpeed);
				}
			}
			catch (Exception e)
			{
				UnityEngine.Debug.LogException(e);
				Disconnect();
			}
		}
#endif
	}
}