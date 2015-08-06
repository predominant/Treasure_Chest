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
using UnityEngine;
using System;
using System.Collections;
using System.Net.Sockets;
#endif

namespace BeardedManStudios.Network
{
	abstract public class TCPProcess : NetWorker
	{
		public TCPProcess() : base() { }
		public TCPProcess(int maxConnections) : base(maxConnections) { }

#if !NETFX_CORE
		protected StreamRead streamRead = new StreamRead();

		protected int previousSize = 0;
		protected BMSByte readBuffer = new BMSByte();
		protected BMSByte backBuffer = new BMSByte();

		protected object writeMutex = new System.Object();

		protected BMSByte ReadBuffer(NetworkStream stream)
		{
			int count = 0;
			while (stream.DataAvailable)
			{
				previousSize = backBuffer.Size;
				backBuffer.SetSize(backBuffer.Size + 1024);

				count += stream.Read(backBuffer.byteArr, previousSize, 1024);
				backBuffer.SetSize(previousSize + count);
			}

			int size = BitConverter.ToInt32(backBuffer.byteArr, 0);

			readBuffer.Clear();

			readBuffer.BlockCopy(backBuffer.byteArr, backBuffer.StartIndex(4), size);

			if (readBuffer.Size + 4 < backBuffer.Size)
				backBuffer.RemoveStart(size + 4);
			else
				backBuffer.Clear();

			return readBuffer;
		}

		abstract public override void Connect(string hostAddress, ushort port);
		abstract public override void Disconnect();
		abstract public override void TimeoutDisconnect();
		abstract public override void Write(NetworkingStream stream);
		abstract public override void Write(NetworkingPlayer player, NetworkingStream stream);
#endif
	}
}