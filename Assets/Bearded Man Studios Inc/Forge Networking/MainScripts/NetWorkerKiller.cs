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
using UnityEngine;

namespace BeardedManStudios.Network.Unity
{
	public class NetWorkerKiller : MonoBehaviour
	{
		private static NetWorkerKiller instance = null;

		public static void Create()
		{
			if (instance != null)
				return;

			instance = new GameObject("NetWorker Authority").AddComponent<NetWorkerKiller>();
			DontDestroyOnLoad(instance.gameObject);
		}

		/// <summary>
		/// Get the instance of the NetWorkerKiller
		/// </summary>
		public static NetWorkerKiller Instance
		{
			get
			{
				if (instance == null)
					Create();

				return instance;
			}
			private set
			{
				instance = value;
			}
		}

		/// <summary>
		/// Get a list of all the NetWorkers
		/// </summary>
		public static List<NetWorker> NetWorkers { get; private set; }

		/// <summary>
		/// Add a NetWorker to this list
		/// </summary>
		/// <param name="netWorker"></param>
		public static void AddNetWorker(NetWorker netWorker)
		{
			if (Instance == null || NetWorkers == null)
				NetWorkers = new List<NetWorker>();

			if (!NetWorkers.Contains(netWorker))
				NetWorkers.Add(netWorker);
		}

		/// <summary>
		/// Clean all the sockets and connections
		/// </summary>
		private void OnApplicationQuit()
		{
			if (NetWorkers == null || NetWorkers.Count == 0)
				return;

			foreach (NetWorker socket in NetWorkers)
			{
				if (socket.Connected)
					socket.Disconnect();
			}

			if (NetworkingManager.SubtleInstance != null)
				Networking.Disconnect();

			HTTP.Close();
		}
	}
}