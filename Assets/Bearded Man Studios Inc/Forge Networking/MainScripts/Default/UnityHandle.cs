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

using BeardedManStudios.Network;

public class UnityHandle : MonoBehaviour
{
	/// <summary>
	/// Port started on
	/// </summary>
	public static ushort StartOnPort { get; private set; }
	
	/// <summary>
	/// Maximum allowed players
	/// </summary>
	public static int MaxPlayers { get; private set; }

	/// <summary>
	/// ID
	/// </summary>
	public static string ID { get; private set; }

	/// <summary>
	/// Room name on
	/// </summary>
	public static string RoomName { get; private set; }

	/// <summary>
	/// Arbitor port connected to
	/// </summary>
	public static ushort ArbitorPort { get; private set; }

	/// <summary>
	/// Protocol type used
	/// </summary>
	public static Networking.TransportationProtocolType protocol = Networking.TransportationProtocolType.UDP;

	/// <summary>
	/// If we should move to the next scene on start
	/// </summary>
	public bool moveToNextSceneOnStart = true;

	private void Awake()
	{
		string[] args = Environment.GetCommandLineArgs();
		bool allowWebplayerConnection = true;

		bool loadedRoom = false;
		for (int i = 0; i < args.Length; i++)
		{
			if (i == args.Length - 1 || string.IsNullOrEmpty(args[i + 1]))
				break;

			string next = args[i + 1];

			if (args[i] == "-room")
			{
				RoomName = next;
				Application.LoadLevel(RoomName);
				loadedRoom = true;

				if (!Application.isLoadingLevel)
				{
					loadedRoom = false;
					// TODO:  Add an error to the logging system
					Application.Quit();
				}
			}
			else if (args[i] == "-arbitor")
			{
				ushort port;
				if (!ushort.TryParse(next, out port))
				{
					// TODO:  Add an error to the logging system
					Application.Quit();
				}

				ArbitorPort = port;
			}
			else if (args[i] == "-port")
			{
				ushort port;
				if (!ushort.TryParse(next, out port))
				{
					// TODO:  Add an error to the logging system
					Application.Quit();
				}

				StartOnPort = port;
			}
			else if (args[i] == "-players")
			{
				int maxPlayers;
				if (!int.TryParse(next, out maxPlayers))
				{
					// TODO:  Add an error to the logging system
					Application.Quit();
				}

				MaxPlayers = maxPlayers;
			}
			else if (args[i] == "-protocol")
			{
				if (next == "tcp")
					protocol = Networking.TransportationProtocolType.TCP;
				else if (next == "udp")
					protocol = Networking.TransportationProtocolType.UDP;
			}
			else if (args[i] == "-identifier")
				ID = next;
			else if (args[i] == "-nowebplayer")
				allowWebplayerConnection = false;
		}

		// TODO:  Allow the room thing to set the protocol type
		if (loadedRoom)
			Networking.Host(StartOnPort, protocol, MaxPlayers, allowWebplayerConnection);
	}

	private void Start()
	{
		if (moveToNextSceneOnStart)
			Application.LoadLevel(Application.loadedLevel + 1);
	}

	/// <summary>
	/// Teleport to the given room with a scene name, ip address and port.
	/// </summary>
	/// <param name="scene">Scene to load</param>
	/// <param name="host">IP address of the host</param>
	/// <param name="port">Port of the host</param>
	public static void TeleportToRoom(string scene, string host, ushort port)
	{
		Application.LoadLevel(scene);

		Networking.Disconnect();
		Networking.Connect(host, port, Networking.TransportationProtocolType.TCP);
	}
}
#endif