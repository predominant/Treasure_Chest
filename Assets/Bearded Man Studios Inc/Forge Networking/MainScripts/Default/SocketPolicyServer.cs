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
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BeardedManStudios.Network
{
	public class SocketPolicyServer
	{
		const string PolicyFileRequest = "<policy-file-request/>";
		static byte[] request = Encoding.UTF8.GetBytes(PolicyFileRequest);
		private byte[] policy;
		private static SocketPolicyServer server = null;

		private Socket listen_socket;
		private Thread runner;

		/// <summary>
		/// If the Socket Policy Server is busy
		/// </summary>
		public bool Busy { get; private set; }

		private AsyncCallback accept_cb;

		class Request
		{
			public Request(Socket s)
			{
				Socket = s;
				// the only answer to a single request (so it's always the same length)
				Buffer = new byte[request.Length];
				Length = 0;
			}

			public Socket Socket { get; private set; }
			public byte[] Buffer { get; set; }
			public int Length { get; set; }
		}

		private SocketPolicyServer(string xml)
		{
			// transform the policy to a byte array (a single time)
			policy = Encoding.UTF8.GetBytes(xml);
		}

		/// <summary>
		/// Start the Socket Policy Server
		/// </summary>
		public void Start()
		{
			if (Busy)
				return;

			try
			{
				listen_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				listen_socket.Bind(new IPEndPoint(IPAddress.Any, 843));
				listen_socket.Listen(500);
				listen_socket.Blocking = false;
				Busy = true;
			}
			catch (SocketException se)
			{
				// Most common mistake: port 843 is not user accessible on unix-like operating systems
				if (se.SocketErrorCode == SocketError.AccessDenied)
				{
					Console.WriteLine("NOTE: must be run as root since the server listen to port 843");
				}
				else
				{
					Console.WriteLine(se);
				}
			}

			runner = new Thread(new ThreadStart(RunServer));
			runner.Start();
		}

		void RunServer()
		{
			accept_cb = new AsyncCallback(OnAccept);
			listen_socket.BeginAccept(accept_cb, null);

			while (true) Thread.Sleep(1000000);  // Just sleep until we're aborted.
		}

		void OnAccept(IAsyncResult ar)
		{
			Console.WriteLine("incoming connection");
			Socket accepted = null;
			try
			{
				accepted = listen_socket.EndAccept(ar);
			}
			catch
			{
			}
			finally
			{
				if (!stopped)
					listen_socket.BeginAccept(accept_cb, null);
			}

			if (accepted == null || stopped)
				return;

			accepted.Blocking = true;

			Request request = new Request(accepted);
			accepted.BeginReceive(request.Buffer, 0, request.Length, SocketFlags.None, new AsyncCallback(OnReceive), request);
		}

		void OnReceive(IAsyncResult ar)
		{
			Request r = (ar.AsyncState as Request);
			Socket socket = r.Socket;
			try
			{
				r.Length += socket.EndReceive(ar);

				// compare incoming data with expected request
				for (int i = 0; i < r.Length; i++)
				{
					if (r.Buffer[i] != request[i])
					{
						// invalid request, close socket
						socket.Close();
						return;
					}
				}

				if (r.Length == request.Length)
				{
					Console.WriteLine("got policy request, sending response");
					// request complete, send policy
					socket.BeginSend(policy, 0, policy.Length, SocketFlags.None, new AsyncCallback(OnSend), socket);
				}
				else
				{
					// continue reading from socket
					socket.BeginReceive(r.Buffer, r.Length, request.Length - r.Length, SocketFlags.None,
						new AsyncCallback(OnReceive), ar.AsyncState);
				}
			}
			catch
			{
				// if anything goes wrong we stop our connection by closing the socket
				socket.Close();
			}
		}

		void OnSend(IAsyncResult ar)
		{
			Socket socket = (ar.AsyncState as Socket);
			try
			{
				socket.EndSend(ar);
			}
			catch
			{
				// whatever happens we close the socket
			}
			finally
			{
				socket.Close();
			}
		}

		private bool stopped = false;

		/// <summary>
		/// Stop the Socket Policy Server
		/// </summary>
		public void Stop()
		{
			runner.Abort();
			listen_socket.Close();
			stopped = true;
		}

		const string AllPolicy =

	@"<?xml version='1.0'?>
<cross-domain-policy>
        <allow-access-from domain=""*"" to-ports=""*"" />
</cross-domain-policy>";

		const string LocalPolicy =

	@"<?xml version='1.0'?>
<cross-domain-policy>
	<allow-access-from domain=""*"" to-ports=""4500-4550"" />
</cross-domain-policy>";

		/// <summary>
		/// Type of XML Type for the Socket Policy Server
		/// </summary>
		public enum XMLType
		{
			All,
			Local,
			File
		}

		/// <summary>
		/// Begin the Socket Policy Server
		/// </summary>
		/// <param name="xmlType">XML Type to be used</param>
		/// <param name="fileLocation">Location of the policy file</param>
		public static void Begin(XMLType xmlType = XMLType.All, string fileLocation = "")
		{
			string policy = null;
			switch (xmlType)
			{
				case XMLType.All:
					policy = AllPolicy;
					break;
				case XMLType.Local:
					policy = LocalPolicy;
					break;
				case XMLType.File:
					if (fileLocation.Length < 2)
					{
						UnityEngine.Debug.LogError("Missing policy file name");
						throw new NetworkException("Missing policy file name");
					}

					if (!File.Exists(fileLocation))
					{
						UnityEngine.Debug.LogError("Could not find policy file '" + fileLocation + "'.");
						throw new NetworkException("Could not find policy file '" + fileLocation + "'.");
					}
					using (StreamReader sr = new StreamReader(fileLocation))
					{
						policy = sr.ReadToEnd();
					}
					break;
			}

			server = new SocketPolicyServer(policy);
			server.Start();
		}

		/// <summary>
		/// End the Socket Policy Server
		/// </summary>
		public static void End()
		{
			if (server != null)
				server.Stop();
		}

		public static void CheckWebplayer(string hostAddress)
		{
#if UNITY_WEBPLAYER
			if (UnityEngine.Application.isWebPlayer)
				UnityEngine.Security.PrefetchSocketPolicy(hostAddress, 843); // TODO:  Make this configurable
#endif
		}
	}
}
#endif