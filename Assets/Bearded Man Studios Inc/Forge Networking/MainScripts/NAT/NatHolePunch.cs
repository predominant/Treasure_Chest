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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BeardedManStudios.Network
{
	public class NatHolePunch
	{
		private const ushort PROXY_PORT = 37915;

		public static void RegisterNat(ref CachedUdpClient readClient, ushort port, string proxyHost = "beardedmanstudios.com", ushort proxyPort = PROXY_PORT)
		{
			readClient = new CachedUdpClient(proxyHost, proxyPort);
			IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
			//readClient.Connect(proxyHost, proxyPort);

			try
			{
				int tryCount = 30;
				while (readClient.Available == 0)
				{
					readClient.Send(new byte[1] { 1 }, 1);
					Thread.Sleep(1000);

					if (--tryCount <= 0)
						throw new Exception("Unable to contact proxy host");
				}

				string tmp = string.Empty;
				readClient.Receive(ref endpoint, ref tmp);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}

		public static void DeRegisterNat(string proxyHost = "beardedmanstudios.com", ushort proxyPort = PROXY_PORT)
		{
			UdpClient udpClient = new UdpClient(proxyHost, proxyPort);
			IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);

			try
			{
				int tryCount = 30;
				while (udpClient.Available == 0)
				{
					udpClient.Send(new byte[1] { 0 }, 1);
					Thread.Sleep(1000);

					if (--tryCount <= 0)
						throw new Exception("Unable to contact proxy host");
				}

				udpClient.Receive(ref endpoint);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}

		public static int GetNattedServer(string natHostAddress, string proxyHost = "beardedmanstudios.com", ushort proxyPort = PROXY_PORT)
		{
			UdpClient udpClient = new UdpClient(PROXY_PORT);
			udpClient.Connect(proxyHost, proxyPort);
			IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, PROXY_PORT);

			byte[] hostBytes = Encoding.UTF8.GetBytes(natHostAddress);

			try
			{
				int tryCount = 30;
				while (udpClient.Available == 0)
				{
					udpClient.Send(hostBytes, hostBytes.Length);
					Thread.Sleep(500);

					if (--tryCount <= 0)
						throw new Exception("Unable to contact proxy host");
				}

				byte[] message = udpClient.Receive(ref endpoint);

				if (message.Length > 0)
					return BitConverter.ToInt32(message, 0);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}

			return -1;
		}
	}
}
#endif