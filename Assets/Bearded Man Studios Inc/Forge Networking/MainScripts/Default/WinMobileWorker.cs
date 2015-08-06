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
using System.Collections.Generic;
using Windows.Networking;
using Windows.Storage.Streams;
#endif

namespace BeardedManStudios.Network
{
	public abstract class WinMobileWorker : NetWorker
	{
		public WinMobileWorker() : base() { }
		public WinMobileWorker(int maxConnections) : base(maxConnections) { }
#if NETFX_CORE
		protected HostName serverHost;

		abstract protected void ConnectAsync(string hostAddress, ushort port);
		abstract protected void SendAsync(NetworkingStream stream);
		protected virtual void SendAsync(NetworkingPlayer player, NetworkingStream stream) { }
		abstract protected void ReadAsync();

		~WinMobileWorker() { Disconnect(); }

		public override void Connect(string hostAddress, ushort port)
		{
			if (Connected)
				throw new NetworkException("The client is already connected to the socket, multiple connections are not allowed. Check implementation to only try to connect if !Connected.");

			ConnectAsync(hostAddress, port);
		}

		public override void Disconnect()
		{
			Connected = false;
			OnDisconnected();
		}

		public override void Disconnect(NetworkingPlayer player, string reason = "")
		{
			base.Disconnect(player);
		}

		protected void ErrorDisconnect(string message)
		{
			OnError(new NetworkException(message));
			Disconnect();
		}

		public override void Write(NetworkingPlayer player, NetworkingStream stream)
		{
			SendAsync(player, stream);
		}

		public override void Write(NetworkingStream stream)
		{
			if (!Connected)
				throw new NetworkException("You must first be connected to a network before you can send packets.");

			SendAsync(stream);
		}
#endif
	}
}