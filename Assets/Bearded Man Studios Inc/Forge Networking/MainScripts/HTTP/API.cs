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
using SimpleJSON;
using System;
using System.Text;

namespace BeardedManStudios.Network.API
{
	public class API
	{
		private static string apiVersion = "v1.0";
		private static string applicationId = string.Empty;
		private static string privateKey = string.Empty;
		private static string accessKey = string.Empty;
		private static string handle = string.Empty;
		private static string endpoint = string.Empty;
		private static bool initialized = false;
		public bool Initialized { get { return initialized; } }

		protected string URL { get { return "http://api.forgearcade.com/" + apiVersion + "/" + handle + "/" + endpoint + "/"; } }

		protected API(string handle, string endpoint)
		{
			API.handle = handle;
			API.endpoint = endpoint;
		}

		protected void ChangeEndpoint(string endpoint)
		{
			API.endpoint = endpoint;
		}

		public static void Initialize(string apiVersion, string applicationId, string privateKey, string accessKey)
		{
			if (initialized)
				return;

			if (string.IsNullOrEmpty(apiVersion.Trim()) || string.IsNullOrEmpty(applicationId.Trim()) || string.IsNullOrEmpty(privateKey.Trim()) || string.IsNullOrEmpty(accessKey.Trim()))
				return;

			API.apiVersion = apiVersion;
			API.applicationId = applicationId;
			API.privateKey = privateKey;
			API.accessKey = accessKey;

			initialized = true;
		}

		protected void Post(string method, Action<object> response, params object[] objects)
		{
			if (string.IsNullOrEmpty(apiVersion))
			{
				Debug.LogError("An api version was not provided");
				return;
			}

			if (string.IsNullOrEmpty(applicationId))
			{
				Debug.LogError("An application id was not provided");
				return;
			}

			if (string.IsNullOrEmpty(privateKey))
			{
				Debug.LogError("An private key was not provided");
				return;
			}

			if (string.IsNullOrEmpty(accessKey))
			{
				Debug.LogError("An access key was not provided");
				return;
			}

			JSONClass payload = new JSONClass();

			var args = new JSONArray();

			foreach (object obj in objects)
			{
				if (obj is bool)
					args.Add(new JSONData((bool)obj));
				else if (obj is double)
					args.Add(new JSONData((double)obj));
				else if (obj is float)
					args.Add(new JSONData((float)obj));
				else if (obj is int || obj is byte || obj is short || obj is ushort)
					args.Add(new JSONData(Convert.ToInt32(obj)));
				else if (obj is string)
					args.Add(new JSONData((string)obj));
				else if (obj is JSONNode)
					args.Add((JSONNode)obj);
			}

			payload.Add("params", args);
			payload.Add("accessKey", accessKey);

			string iv;
			string encrypt = Encryptor.Encrypt(privateKey, payload.ToString(), out iv);

			payload = new JSONClass();
			payload.Add("applicationId", new JSONData(applicationId));
			payload.Add("iv", iv);
			payload.Add("payload", encrypt);

			HTTP request = new HTTP(URL + method);
			request.contentType = HTTP.ContentType.JSON;
			request.Post(response, payload.ToString());
		}

		protected string PostSynchronous(string method, Action<string> response, params object[] objects)
		{
			if (string.IsNullOrEmpty(apiVersion))
			{
				Debug.LogError("An api version was not provided");
				return string.Empty;
			}

			if (string.IsNullOrEmpty(applicationId))
			{
				Debug.LogError("An application id was not provided");
				return string.Empty;
			}

			if (string.IsNullOrEmpty(privateKey))
			{
				Debug.LogError("An private key was not provided");
				return string.Empty;
			}

			if (string.IsNullOrEmpty(accessKey))
			{
				Debug.LogError("An access key was not provided");
				return string.Empty;
			}

			JSONClass payload = new JSONClass();

			var args = new JSONArray();

			foreach (object obj in objects)
			{
				if (obj is bool)
					args.Add(new JSONData((bool)obj));
				else if (obj is double)
					args.Add(new JSONData((double)obj));
				else if (obj is float)
					args.Add(new JSONData((float)obj));
				else if (obj is int || obj is byte || obj is short || obj is ushort)
					args.Add(new JSONData(Convert.ToInt32(obj)));
				else if (obj is string)
					args.Add(new JSONData((string)obj));
				else if (obj is JSONNode)
					args.Add((JSONNode)obj);
			}

			payload.Add("params", args);
			payload.Add("accessKey", accessKey);

			string iv;
			string encrypt = Encryptor.Encrypt(privateKey, payload.ToString(), out iv);

			payload = new JSONClass();
			payload.Add("applicationId", new JSONData(applicationId));
			payload.Add("iv", iv);
			payload.Add("payload", encrypt);

			HTTP request = new HTTP(URL + method);
			request.contentType = HTTP.ContentType.JSON;

			object postResponse = request.PostSynchronous(payload.ToString());

			if (postResponse is Exception)
				throw (Exception)postResponse;

			return (string)postResponse;
		}
	}
}