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

namespace BeardedManStudios.Network
{
	public class HostInfo
	{
		/// <summary>
		/// Ip address of the HostInfo
		/// </summary>
		public string ipAddress = string.Empty;

		public string IpAddress { get { return ipAddress.Split('+')[0]; } }

		/// <summary>
		/// Port being used
		/// </summary>
		public ushort port = 0;

		/// <summary>
		/// The name that the server owner designated for this server
		/// </summary>
		public string name = string.Empty;

		/// <summary>
		/// The type of game that this server is running
		/// </summary>
		public string gameType = string.Empty;

		/// <summary>
		/// The type of connection that the server is running
		/// </summary>
		public string connectionType = "udp";

		/// <summary>
		/// The name of the scene that the server is currently on
		/// </summary>
		public string sceneName = string.Empty;

		/// <summary>
		/// Player count of all connected players
		/// </summary>
		public int connectedPlayers = 0;

		/// <summary>
		/// Maximum allowed players
		/// </summary>
		public int maxPlayers = 0;

		/// <summary>
		/// Extra string for loading screen texts and what not
		/// </summary>
		public string comment = string.Empty;

		/// <summary>
		/// The password that is used for the server
		/// </summary>
		public string password = string.Empty;

		/// <summary>
		/// Last ping sent
		/// </summary>
		public DateTime lastPing;

		public int PingTime { get; private set; }

		/// <summary>
		/// Default constructor
		/// </summary>
		public HostInfo()
		{
			Ping();
		}

		/// <summary>
		/// Set the ping to current time
		/// </summary>
		public void Ping()
		{
			lastPing = DateTime.Now;
		}

		public void SetPing(int pingTime)
		{
			PingTime = pingTime;
		}
	}
}