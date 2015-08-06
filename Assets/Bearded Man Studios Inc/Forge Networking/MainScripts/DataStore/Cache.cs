using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;

#if NETFX_CORE
using System.Threading.Tasks;
#else
using System.ComponentModel;
#endif

namespace BeardedManStudios.Network
{
	/// <summary>
	/// The main class for managing and communicating data from the server cache
	/// </summary>
	public static class Cache
	{
		/// <summary>
		/// The memory cache for the data
		/// </summary>
		private static Dictionary<string, object> memory = new Dictionary<string, object>();

		/// <summary>
		/// The main socket for communicating the cache back and forth
		/// </summary>
		public static NetWorker Socket { get { return Networking.PrimarySocket; } }

		// TODO:  Possibly make this global
		/// <summary>
		/// The set of types that are allowed and a byte mapping to them
		/// </summary>
		private static Dictionary<byte, Type> typeMap = new Dictionary<byte, Type>()
		{
			{ 0, typeof(byte) },
			{ 1, typeof(char) },
			{ 2, typeof(short) },
			{ 3, typeof(ushort) },
			{ 4, typeof(int) },
			{ 5, typeof(uint) },
			{ 6, typeof(long) },
			{ 7, typeof(ulong) },
			{ 8, typeof(bool) },
			{ 9, typeof(float) },
			{ 10, typeof(double) },
			{ 11, typeof(string) },
			{ 12, typeof(Vector2) },
			{ 13, typeof(Vector3) },
			{ 14, typeof(Vector4) },
			{ 15, typeof(Quaternion) },
			{ 16, typeof(Color) }
		};

		/// <summary>
		/// The current id that the callback stack is on
		/// </summary>
		private static int responseHookIncrementer = 0;

		/// <summary>
		/// The main callback stack for when requesting data
		/// </summary>
		private static Dictionary<int, Action<object>> responseHooks = new Dictionary<int, Action<object>>();

		/// <summary>
		/// Called when the network as interpreted that a cache message has been sent
		/// </summary>
		/// <param name="bytes">The data that was received</param>
		/// <param name="player">The player that requested data from the cache</param>
		public static void NetworkRead(BMSByte bytes, NetworkingPlayer player = null)
		{
			// This is a read
			if (bytes[1] == 0)
			{
				byte type = bytes.GetBasicType<byte>(2);
				int responseHookId = bytes.GetBasicType<int>(3);
				string key = Encryptor.Encoding.GetString(bytes.byteArr, 3 + sizeof(int), bytes.Size - 3 - sizeof(int));

				UnityEngine.Debug.Log(key);

				object obj = Get(key);

				UnityEngine.Debug.Log(obj);

				// TODO:  Let the client know it is null
				if (obj == null)
					return;

				BMSByte data = new BMSByte(true);
				data.Append(new byte[] { 3 });
				data.Append(new byte[] { 2 });
				data.Append(new byte[] { type });
				data.Append(BitConverter.GetBytes(responseHookId));
				ObjectMapper.MapBytes(data, obj);

				Socket.WriteRaw(player, data, "BMS_INTERNAL_Cache_" + key);
			}
			// This is a write
			else if (bytes[1] == 1)
			{
				// TODO:  Implement the writing feature for clients later
			}
			// This is a response from the server
			else if (bytes[1] == 2)
			{
				byte type = bytes.GetBasicType<byte>(2);
				int responseHookId = bytes.GetBasicType<int>(3);
				object obj = null;
				int offset = 3 + sizeof(int);


				if (typeMap[type] == typeof(Vector2))
					obj = bytes.GetVector2(offset);
				else if (typeMap[type] == typeof(Vector3))
					obj = bytes.GetVector3(offset);
				else if (typeMap[type] == typeof(Vector4))
					obj = bytes.GetVector4(offset);
				else if (typeMap[type] == typeof(Color))
					obj = bytes.GetColor(offset);
				else if (typeMap[type] == typeof(Quaternion))
					obj = bytes.GetQuaternion(offset);
				else if (typeMap[type] == typeof(string))
					obj = bytes.GetString(offset);
				else
					obj = bytes.GetBasicType(typeMap[type], offset);

				if (responseHooks.ContainsKey(responseHookId))
				{
					responseHooks[responseHookId](obj);
					responseHooks.Remove(responseHookId);
				}
			}
		}

		/// <summary>
		/// Get an object from cache
		/// </summary>
		/// <typeparam name="T">The type of object to store</typeparam>
		/// <param name="key">The name variable used for storing the desired object</param>
		/// <returns>Return object from key otherwise return the default value of the type or null</returns>
		private static T Get<T>(string key)
		{
			if (!Socket.IsServer)
				return default(T);

			if (!memory.ContainsKey(key))
				return default(T);

			if (memory[key].GetType() == typeof(T))
				return (T)memory[key];

			return default(T);
		}

		/// <summary>
		/// Used on the server to get an object at a given key from cache
		/// </summary>
		/// <param name="key">The key to be used in the dictionary lookup</param>
		/// <returns>The object at the given key in cache otherwise null</returns>
		private static object Get(string key)
		{
			if (!Socket.IsServer)
				return null;

			if (memory.ContainsKey(key))
				return memory[key];

			return null;
		}

		/// <summary>
		/// Get an object from cache
		/// </summary>
		/// <param name="key">The name variable used for storing the desired object</param>
		/// <returns>The string data at the desired key or null</returns>
		public static void Request<T>(string key, Action<object> callback)
		{
			if (callback == null)
				throw new NetworkException("A callback is needed when requesting data from the server");

			if (Socket.IsServer)
			{
				callback(Get<T>(key));
				return;
			}

			responseHooks.Add(responseHookIncrementer, callback);

			byte[] keyBin = Encryptor.Encoding.GetBytes(key);
			BMSByte data = new BMSByte(true);
			data.Append(new byte[] { 3 });
			data.Append(new byte[] { 0 });

			foreach (KeyValuePair<byte, Type> kv in typeMap)
			{
				if (typeof(T) == kv.Value)
				{
					data.Append(new byte[] { kv.Key });
					break;
				}
			}

			data.Append(BitConverter.GetBytes(responseHookIncrementer));
			data.Append(keyBin);

			Socket.WriteRaw(data, reliableId: "BMS_INTERNAL_Cache_" + key);
			responseHookIncrementer++;
		}

		/// <summary>
		/// Inserts a NEW key/value into cache
		/// </summary>
		/// <typeparam name="T">The serializable type of object</typeparam>
		/// <param name="key">The name variable used for storing the specified object</param>
		/// <param name="value">The object that is to be stored into cache</param>
		/// <returns>True if successful insert or False if the key already exists</returns>
		public static bool Insert<T>(string key, T value)
		{
			if (!Socket.IsServer)
				throw new NetworkException("The Cache insert method is not yet supported for clients");

			if (!memory.ContainsKey(key))
				return false;

			memory.Add(key, value);

			return true;
		}

		/// <summary>
		/// Inserts a new key/value or updates a key's value in cache
		/// </summary>
		/// <typeparam name="T">The serializable type of object</typeparam>
		/// <param name="key">The name variable used for storing the specified object</param>
		/// <param name="value">The object that is to be stored into cache</param>
		public static void Set<T>(string key, T value)
		{
			if (!Socket.IsServer)
				throw new NetworkException("The Cache insert method is not yet supported for clients");

			if (!memory.ContainsKey(key))
				memory.Add(key, value);
			else
				memory[key] = value;
		}
	}
}