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
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Collections.Generic;
#endif

namespace BeardedManStudios.Network
{
	public class DefaultServerTCP : TCPProcess
	{
#if NETFX_CORE
		public bool RelayToAll { get; set; }
		public DefaultServerTCP(int maxConnections) : base(maxConnections) { }
		public override void Connect(string hostAddress, ushort port) { }
		public override void Disconnect() { }
		public override void TimeoutDisconnect() { }
		public override void Disconnect(NetworkingPlayer player, string reason = "") { }
		public void WriteTo(NetworkingPlayer player, NetworkingStream stream) { }
		public override void Write(NetworkingStream stream) { }
		public override void Write(NetworkingPlayer player, NetworkingStream stream) { }
#else
		private TcpListener tcpListener = null;
		private IPAddress ipAddress = null;
		private BackgroundWorker listenWorker = null;
		private BackgroundWorker readWorker = null;

		private NetworkingStream staticWriteStream = new NetworkingStream();
		private NetworkingStream writeStream = new NetworkingStream();
		private NetworkingStream readStream = new NetworkingStream();

		/// <summary>
		/// Should the messages be relayed to all
		/// </summary>
		public bool RelayToAll { get; set; }

		private object removalMutex = new Object();

		/// <summary>
		/// Constructor with a given Maximum allowed connections
		/// </summary>
		/// <param name="maxConnections">The Maximum connections allowed</param>
		public DefaultServerTCP(int maxConnections) : base(maxConnections) { RelayToAll = true; }
		~DefaultServerTCP() { Disconnect(); }

		private Thread connector;

		/// <summary>
		/// Host to a Ip Address with a supplied port
		/// </summary>
		/// <param name="hostAddress">Ip Address to host from</param>
		/// <param name="port">Port to allow connections from</param>
		public override void Connect(string hostAddress, ushort port)
		{
			Host = hostAddress;
			//UnityEngine.Debug.Log("Connecting to address: " + hostAddress + " with port " + port);

			connector = new Thread(new ParameterizedThreadStart(ThreadedConnect));
			connector.Start(new object[] { hostAddress, port });
		}

		private void ThreadedConnect(object hostAndPort)
		{
			string hostAddress = (string)((object[])hostAndPort)[0];
			ushort port = (ushort)((object[])hostAndPort)[1];

			// Create an instance of the TcpListener class.
			tcpListener = null;
			if (string.IsNullOrEmpty(hostAddress) || hostAddress == "127.0.0.1" || hostAddress == "localhost")
				ipAddress = IPAddress.Any;
			else
				ipAddress = IPAddress.Parse(hostAddress);

			try
			{
				// Set the listener on the local IP address 
				// and specify the port.
				tcpListener = new TcpListener(ipAddress, (int)port);
				tcpListener.Start();

				Players = new List<NetworkingPlayer>();
				Me = new NetworkingPlayer(ServerPlayerCounter++, "127.0.0.1", tcpListener, "SERVER");

				listenWorker = new BackgroundWorker();
				listenWorker.WorkerSupportsCancellation = true;
				listenWorker.WorkerReportsProgress = true;
				listenWorker.DoWork += Listen;
				listenWorker.ProgressChanged += listenWorker_ProgressChanged;
				listenWorker.RunWorkerCompleted += WorkCompleted;
				listenWorker.RunWorkerAsync(tcpListener);

				readWorker = new BackgroundWorker();
				readWorker.WorkerSupportsCancellation = true;
				//readWorker.WorkerReportsProgress = true;
				//readWorker.ProgressChanged += StreamReceived;
				readWorker.DoWork += ReadAsync;
				readWorker.RunWorkerAsync();

				OnConnected();
			}
			catch (Exception e)
			{
				UnityEngine.Debug.LogException(e);
				Disconnect();
			}
		}

		/// <summary>
		/// Disconnet a player from the server
		/// </summary>
		/// <param name="player">Player to be removed from the server</param>
		public override void Disconnect(NetworkingPlayer player, string reason = null)
		{
			lock (removalMutex)
			{
				base.Disconnect(player);

				if (Players.Contains(player))
					Players.Remove(player);

				CleanRPCForPlayer(player);
			}
		}

		/// <summary>
		/// Disconnect the server
		/// </summary>
		public override void Disconnect()
		{
			if (Players != null)
			{
				lock (Players)
				{
					foreach (NetworkingPlayer player in Players)
						((TcpClient)player.SocketEndpoint).Close();

					Players.Clear();
				}
			}

			if (listenWorker != null)
				listenWorker.CancelAsync();

			if (readWorker != null)
				readWorker.CancelAsync();

			tcpListener.Stop();

			OnDisconnected();
		}

		public override void TimeoutDisconnect()
		{
			// TODO:  Implement
		}

		private void WriteAndClose(TcpClient targetSocket, NetworkingStream stream)
		{
			targetSocket.GetStream().Write(stream.Bytes.byteArr, stream.Bytes.StartIndex(), stream.Bytes.Size);
			targetSocket.Close();
		}

		/// <summary>
		/// Write the Players data and Networking stream sent to the server
		/// </summary>
		/// <param name="player">Player to write from</param>
		/// <param name="stream">Networking Stream to be used</param>
		public override void Write(NetworkingPlayer player, NetworkingStream stream)
		{
			((TcpClient)player.SocketEndpoint).GetStream().Write(stream.Bytes.byteArr, stream.Bytes.StartIndex(), stream.Bytes.Size);
		}

		/// <summary>
		/// Write the Networking Stream to the server
		/// </summary>
		/// <param name="stream">Networking Stream to be used</param>
		public override void Write(NetworkingStream stream)
		{
			// TODO:  Find out if this was a relay
			if (stream.identifierType == NetworkingStream.IdentifierType.RPC && (stream.Receivers == NetworkReceivers.AllBuffered || stream.Receivers == NetworkReceivers.OthersBuffered))
				ServerBufferRPC(stream);

			if (stream.Receivers == NetworkReceivers.Server)
				return;

			for (int i = 0; i < Players.Count; i++)
			{
				if ((stream.Receivers == NetworkReceivers.Others || stream.Receivers == NetworkReceivers.OthersBuffered) && Players[i] == stream.Sender)
					continue;

				if (!((TcpClient)Players[i].SocketEndpoint).Connected)
				{
					Disconnect(Players[i]);
					continue;
				}

				((TcpClient)Players[i].SocketEndpoint).GetStream().Write(stream.Bytes.byteArr, stream.Bytes.StartIndex(), stream.Bytes.Size);
			}
		}

		private void StreamReceived(StreamRead streamRead)//object sender, ProgressChangedEventArgs e)
		{
			//StreamRead streamRead = (StreamRead)e.UserState;

			readStream.Consume(this, Players[streamRead.clientIndex], streamRead.bytes);

			DataRead(Players[streamRead.clientIndex], readStream);

			if (ObjectMapper.Compare<string>(readStream, "update"))
				UpdateNewPlayer(Players[streamRead.clientIndex]);

			if (RelayToAll)
				RelayStream(readStream);
		}

		private void ReadAsync(object sender, DoWorkEventArgs e)
		{
			while (true)
			{
				try
				{
					if (readWorker.CancellationPending)
					{
						e.Cancel = true;
						break;
					}

					lock (removalMutex)
					{
						for (int i = 0; i < Players.Count; i++)
						{
							if (!((TcpClient)Players[i].SocketEndpoint).Connected)
							{
								Disconnect(Players[i]);

								Thread.Sleep(ThreadSpeed);

								continue;
							}


							if (((TcpClient)Players[i].SocketEndpoint).GetStream().DataAvailable)
							{
								do
								{
									readBuffer = ReadBuffer(((TcpClient)Players[i].SocketEndpoint).GetStream());

									if (readBuffer != null && readBuffer.Size > 0)
										StreamReceived(streamRead.Prepare(i, readBuffer));
								} while (backBuffer.Size > 0);
							}
						}
					}

					Thread.Sleep(ThreadSpeed);
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.LogException(ex);
				}
			}
		}

		private BMSByte writeBuffer = new BMSByte();

		private void Listen(object sender, DoWorkEventArgs e)
		{
			while (true)
			{
				if (listenWorker.CancellationPending)
				{
					e.Cancel = true;
					break;
				}

				TcpListener tcpListener = (TcpListener)e.Argument;

				try
				{
					for (int i = 0; i < Players.Count; i++)
						if (!((TcpClient)Players[i].SocketEndpoint).Connected)
							Players.RemoveAt(i--);

					// Create a TCP socket.
					// If you ran this server on the desktop, you could use 
					// Socket socket = tcpListener.AcceptSocket()
					// for greater flexibility.
					TcpClient tcpClient = tcpListener.AcceptTcpClient();

					if (Connections >= MaxConnections)
					{
						lock (writeMutex)
						{
							writeBuffer.Clear();
							ObjectMapper.MapBytes(writeBuffer, "Max Players Reached On Server");

							staticWriteStream.SetProtocolType(Networking.ProtocolType.TCP);
							WriteAndClose(tcpClient, staticWriteStream.Prepare(
								this, NetworkingStream.IdentifierType.Disconnect, null, writeBuffer));
						}

						return;
					}

					// TODO:  Set the name
					string name = string.Empty;

					NetworkingPlayer player = new NetworkingPlayer(ServerPlayerCounter++, tcpClient.Client.RemoteEndPoint.ToString(), tcpClient, name);

					lock (Players)
					{
						Players.Add(player);
					}

					listenWorker.ReportProgress(0, player);
				}
				catch (NetworkException exception)
				{
					UnityEngine.Debug.LogException(exception);
					Disconnect();
				}
			}
		}

		private void listenWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			lock (writeMutex)
			{
				writeBuffer.Clear();
				ObjectMapper.MapBytes(writeBuffer, ((NetworkingPlayer)e.UserState).NetworkId);

				writeStream.SetProtocolType(Networking.ProtocolType.TCP);
				Write((NetworkingPlayer)e.UserState, writeStream.Prepare(this,
					NetworkingStream.IdentifierType.Player, null, writeBuffer));

				OnPlayerConnected((NetworkingPlayer)e.UserState);
			}
		}

		private void WorkCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			Disconnect();
		}
#endif
	}
}