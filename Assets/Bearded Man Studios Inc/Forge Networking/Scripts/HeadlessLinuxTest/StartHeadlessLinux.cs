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

using UnityEngine;
using BeardedManStudios.Network;

#if UNITY_EDITOR && !UNITY_WEBPLAYER
using System.Collections;
#endif

namespace BeardedManStudios.Network.Unity
{
	public class StartHeadlessLinux : MonoBehaviour
	{
		public string host = "127.0.0.1";																		// IP address
		public int port = 15937;																				// Port number
		public Networking.TransportationProtocolType protocolType = Networking.TransportationProtocolType.UDP;	// Communication protocol
		public int playerCount = 31;																			// Maximum player count -- excluding this server
		public string sceneName = "Game";																		// Scene to load
		public bool proximityBasedUpdates = false;																// Only send other player updates if they are within range
		public float proximityDistance = 5.0f;																	// The range for the players to be updated within

		private NetWorker socket = null;																		// The initial connection socket

		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}

		public void Start()
		{
			StartServer();
		}

		/// <summary>
		/// This method is called when the host server button is clicked
		/// </summary>
		public void StartServer()
		{
			// Create a host connection
			socket = Networking.Host((ushort)port, protocolType, playerCount, false);
			Go();
		}

		private void Go()
		{
			if (proximityBasedUpdates)
				socket.MakeProximityBased(proximityDistance);

			socket.serverDisconnected += delegate(string reason)
			{
				MainThreadManager.Run(() =>
				{
					Debug.Log("The server kicked you for reason: " + reason);
					Application.Quit();
#if UNITY_EDITOR
					UnityEditor.EditorApplication.isPlaying = false;
#endif
				});
			};

			if (socket.Connected)
				MainThreadManager.Run(LoadScene);
			else
				socket.connected += LoadScene;
		}

		private void LoadScene()
		{
			Networking.SetPrimarySocket(socket);
			Application.LoadLevel(sceneName);
		}
	}
}