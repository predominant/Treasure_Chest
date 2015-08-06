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
using UnityEngine;
using System.Collections.Generic;
using SimpleJSON;

namespace BeardedManStudios.Network.API
{
	public sealed class ServerAPI : API
	{
		private string LogExceptionURL { get { return "LogException"; } }
		private string LogExceptionBatchURL { get { return "LogExceptionBatch"; } }
		private string StartServerURL { get { return "StartServer"; } }
		private string StopServerURL { get { return "StopServer"; } }
		private string UpdateServerURL { get { return "UpdateServer"; } }
		private string GetHostsURL { get { return "GetHosts"; } }

		private static ServerAPI instance = null;
		public static ServerAPI Instance
		{
			get
			{
				if (instance == null)
					instance = new ServerAPI("Accounts", "Servers");

				return instance;
			}
		}

		private ServerAPI(string handle, string endpoint) : base(handle, endpoint) { }

		public void LogException(NetWorker netWorker, NetworkException exception, Action<string> callback)
		{
			if (!netWorker.Connected)
			{
				Debug.LogError("You must be connected to log an exception to the Arbiter");
				return;
			}

			Post(LogExceptionURL, delegate(object obj) { callback((string)obj); }, netWorker.Port, exception.Code, exception.Message, exception.StackTrace, Time.time);
		}

		public void LogExceptionBatch(NetWorker netWorker, NetworkException[] exceptions, Action<string> callback)
		{
			if (!netWorker.Connected)
			{
				Debug.LogError("You must be connected to log an exception to the Arbiter");
				return;
			}

			var args = new JSONArray();

			foreach (NetworkException exception in exceptions)
			{
				var ex = new JSONClass();
				ex.Add("code", new JSONData(exception.Code));
				ex.Add("message", new JSONData(string.IsNullOrEmpty(exception.Message) ? "" : exception.Message));
				ex.Add("trace", new JSONData(exception.StackTrace == null ? "" : exception.StackTrace));
				args.Add(ex);
			}

			Post(LogExceptionBatchURL, delegate(object obj) { callback((string)obj); }, 15937, args, Time.time);
		}

		public void ServerStarted(NetWorker netWorker, Action<string> callback, string name, string type, string password, string comment, int maxPlayers, string sceneName, int playerCount = 0)
		{
			if (!netWorker.Connected)
			{
				Debug.LogError("You must be connected to register a server on the Arbiter");
				return;
			}

			Post(StartServerURL, delegate(object obj) { callback((string)obj); }, netWorker.Port, name, type, password, comment, maxPlayers, playerCount, (netWorker is CrossPlatformUDP) ? "udp" : "tcp", sceneName);
		}

		public void ServerStopped(NetWorker netWorker, Action<string> callback)
		{
			if (netWorker.Port <= 0)
			{
				Debug.LogError("The supplied NetWorker must have at least been connected at some point.");
				return;
			}

			PostSynchronous(StopServerURL, callback, netWorker.Port);
		}

		public void UpdateServer(NetWorker netWorker, Action<string> callback, int playerCount, string sceneName)
		{
			if (!netWorker.Connected)
			{
				Debug.LogError("You must be connected to update a server on the Arbiter");
				return;
			}

			Post(UpdateServerURL, delegate(object obj) { callback((string)obj); }, netWorker.Port, playerCount, sceneName);
		}

		public void GetHosts(Action<HostInfo[]> callback, string gameType = "", string sceneName = "")
		{
			Post(GetHostsURL,
				delegate(object data)
				{
					JSONNode json = JSON.Parse((string)data);

					List<HostInfo> hosts = new List<HostInfo>();
					HostInfo toBeAdded;
					foreach (JSONClass obj in json["servers"].AsArray)
					{
						toBeAdded = new HostInfo()
						{
							ipAddress = obj["host"],
							port = (ushort)obj["port"].AsInt,
							connectionType = obj["connectionType"],
							sceneName = obj["sceneName"],
							name = obj["name"],
							gameType = obj["type"],
							maxPlayers = obj["maxPlayers"].AsInt,
							connectedPlayers = obj["currentPlayers"].AsInt,
							comment = obj["comment"],
							password = obj["password"],
							lastPing = new DateTime(1970, 1, 1) + TimeSpan.FromSeconds(obj["lastPing"].AsDouble)
						};

						if (string.IsNullOrEmpty(toBeAdded.password))
							toBeAdded.password = string.Empty;
						if (string.IsNullOrEmpty(toBeAdded.connectionType))
							toBeAdded.connectionType = string.Empty;
						if (string.IsNullOrEmpty(toBeAdded.sceneName))
							toBeAdded.sceneName = string.Empty;

						hosts.Add(toBeAdded);
					}

					callback(hosts.ToArray());
				}, gameType, sceneName);
		}
	}
}