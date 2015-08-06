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



#if BMS_DEBUGGING
#define BMS_DEBUGGING_UNITY
#endif

using UnityEngine;

using System.Collections.Generic;
using System;
using System.Text;

namespace BeardedManStudios.Network
{
	public class ObjectMapper
	{
		/// <summary>
		/// Map a type of object from a networking stream to a object
		/// </summary>
		/// <param name="type">Type of object to map</param>
		/// <param name="stream">Networking Stream to be used</param>
		/// <returns>Returns the mapped object</returns>
		public static object Map(Type type, NetworkingStream stream)
		{
			object obj = null;

			if (type == typeof(string))
				obj = MapString(stream);
			else if (type == typeof(Vector2))
				obj = MapVector2(stream);
			else if (type == typeof(Vector3))
				obj = MapVector3(stream);
			else if (type == typeof(Vector4) || type == typeof(Color) || type == typeof(Quaternion))
				obj = MapVector4(type, stream);
			else
				obj = MapBasicType(type, stream);

			return obj;
		}

		/// <summary>
		/// Compares a type of object to the Networking Stream
		/// </summary>
		/// <typeparam name="T">Value type to get out of it</typeparam>
		/// <param name="stream">Stream to be used</param>
		/// <param name="o">Object being compared with</param>
		/// <returns>Returns the type of comparison passed</returns>
		public static bool Compare<T>(NetworkingStream stream, object o)
		{
#if BMS_DEBUGGING_UNITY
			UnityEngine.Debug.Log("[ObjectMapper Stream Bytes] " + (stream != null ? stream.Bytes.Count.ToString() : string.Empty));
			UnityEngine.Debug.Log("[ObjectMapper Stream NetworkID] " + (stream != null ? stream.NetworkId.ToString() : string.Empty));
			UnityEngine.Debug.Log("[ObjectMapper Object] " + o);
#endif

			stream.StartPeek();
			object obj = null;

			if (typeof(T) == typeof(string))
			{
#if BMS_DEBUGGING_UNITY
				UnityEngine.Debug.Log("[ObjectMapper Stream MapString()]");
#endif
				obj = MapString(stream);
			}
			else if (typeof(T) == typeof(Vector3))
			{
#if BMS_DEBUGGING_UNITY
				UnityEngine.Debug.Log("[ObjectMapper Stream Vector3()]");
#endif
				obj = MapVector3(stream);
			}
			else if (typeof(T) == typeof(Vector4) || typeof(T) == typeof(Color) || typeof(T) == typeof(Quaternion))
			{
#if BMS_DEBUGGING_UNITY
				UnityEngine.Debug.Log("[ObjectMapper Stream Vector4(), Color(), Quaternion()]");
#endif
				obj = MapVector4(typeof(T), stream);
			}
			else
			{
#if BMS_DEBUGGING_UNITY
				UnityEngine.Debug.Log("[ObjectMapper Stream Basic]");
#endif
				obj = MapBasicType(typeof(T), stream);
			}

			stream.StopPeek();

			return Equals(o, obj);
		}

		/// <summary>
		/// Get a mapped value out of the Networking Stream
		/// </summary>
		/// <typeparam name="T">Value to get out of it</typeparam>
		/// <param name="stream">Networking Stream to be used</param>
		/// <returns>Returns a mapped value from the Networking Stream</returns>
		public static T Map<T>(NetworkingStream stream)
		{
			object obj = null;

			if (typeof(T) == typeof(string))
				obj = MapString(stream);
			else if (typeof(T) == typeof(Vector2))
				obj = MapVector2(stream);
			else if (typeof(T) == typeof(Vector3))
				obj = MapVector3(stream);
			else if (typeof(T) == typeof(Vector4) || typeof(T) == typeof(Color) || typeof(T) == typeof(Quaternion))
				obj = MapVector4(typeof(T), stream);
			else
				obj = MapBasicType(typeof(T), stream);

			return (T)obj;
		}

		/// <summary>
		/// Get a mapped basic type of object from the Networking Stream
		/// </summary>
		/// <param name="type">Type of object to be mapped</param>
		/// <param name="stream">Networking Stream to be used</param>
		/// <returns>Returns a mapped object of the given type</returns>
		public static object MapBasicType(Type type, NetworkingStream stream)
		{
			if (type == typeof(byte))
				return (byte)stream.Read(ref readBytes, sizeof(byte))[0];
			else if (type == typeof(char))
				return stream.Read(ref readBytes, sizeof(byte));
			else if (type == typeof(short))
				return BitConverter.ToInt16(stream.Read(ref readBytes, sizeof(short)), 0);
			else if (type == typeof(ushort))
				return BitConverter.ToUInt16(stream.Read(ref readBytes, sizeof(short)), 0);
			else if (type == typeof(bool))
				return BitConverter.ToBoolean(stream.Read(ref readBytes, sizeof(bool)), 0);
			else if (type == typeof(int))
				return BitConverter.ToInt32(stream.Read(ref readBytes, sizeof(int)), 0);
			else if (type == typeof(uint))
				return BitConverter.ToUInt32(stream.Read(ref readBytes, sizeof(int)), 0);
			else if (type == typeof(float))
				return BitConverter.ToSingle(stream.Read(ref readBytes, sizeof(float)), 0);
			else if (type == typeof(long))
				return BitConverter.ToInt64(stream.Read(ref readBytes, sizeof(long)), 0);
			else if (type == typeof(ulong))
				return BitConverter.ToUInt64(stream.Read(ref readBytes, sizeof(long)), 0);
			else if (type == typeof(double))
				return BitConverter.ToDouble(stream.Read(ref readBytes, sizeof(double)), 0);
			else
				throw new NetworkException(11, "The type " + type.ToString() + " is not allowed to be sent over the network (yet)");
		}

		/// <summary>
		/// Map a string from the Networking Stream
		/// </summary>
		/// <param name="stream">Networking Stream to be used</param>
		/// <returns>Returns a string out of the Networking Stream</returns>
		public static object MapString(NetworkingStream stream)
		{
#if BMS_DEBUGGING_UNITY
			UnityEngine.Debug.Log("[ObjectMapper MapString Stream Bytes] " + stream.Bytes);
			UnityEngine.Debug.Log("[ObjectMapper MapString Stream NetworkId] " + stream.NetworkId);
#endif
			int length = BitConverter.ToInt32(stream.Read(ref readBytes, sizeof(int)), 0);
#if BMS_DEBUGGING_UNITY
			UnityEngine.Debug.Log("[ObjectMapper Length] " + length);
#endif

			if (length <= 0)
				return string.Empty;

#if NETFX_CORE
			return Encoding.UTF8.GetString(stream.Read(ref readBytes, length), 0, length);
#else
			if (length > stream.Bytes.Size - sizeof(int))
			{
#if BMS_DEBUGGING_UNITY
				UnityEngine.Debug.Log("[ObjectMapper (Attempted to read a string that doesn't exist)]");
#endif
				return string.Empty;
				//throw new NetworkException(12, "Attempted to read a string that doesn't exist");
			}

			return Encoding.UTF8.GetString(stream.Read(ref readBytes, length), 0, length);
#endif
		}

		private static float x, y, z, w;
		private static byte[] readBytes = new byte[0];

		/// <summary>
		/// Get a Vector2 out of a Networking Stream
		/// </summary>
		/// <param name="stream">Networking Stream to be used</param>
		/// <returns>A Vector2 out of the Networking Stream</returns>
		public static object MapVector2(NetworkingStream stream)
		{
			x = BitConverter.ToSingle(stream.Read(ref readBytes, sizeof(float)), 0);
			y = BitConverter.ToSingle(stream.Read(ref readBytes, sizeof(float)), 0);
			return new Vector2(x, y);
		}

		/// <summary>
		/// Get a Vector3 out of a Networking Stream
		/// </summary>
		/// <param name="stream">Networking Stream to be used</param>
		/// <returns>A Vector3 out of the Networking Stream</returns>
		public static object MapVector3(NetworkingStream stream)
		{
			x = BitConverter.ToSingle(stream.Read(ref readBytes, sizeof(float)), 0);
			y = BitConverter.ToSingle(stream.Read(ref readBytes, sizeof(float)), 0);
			z = BitConverter.ToSingle(stream.Read(ref readBytes, sizeof(float)), 0);

			return new Vector3(x, y, z);
		}

		/// <summary>
		/// Get a Vector4 out of a Networking Stream
		/// </summary>
		/// <param name="type">Type of object to be mapped</param>
		/// <param name="stream">Networking Stream to be used</param>
		/// <returns>A type of Vector4 (Vector4/Color/Quaternion) out of the Networking Stream</returns>
		public static object MapVector4(Type type, NetworkingStream stream)
		{
			x = BitConverter.ToSingle(stream.Read(ref readBytes, sizeof(float)), 0);
			y = BitConverter.ToSingle(stream.Read(ref readBytes, sizeof(float)), 0);
			z = BitConverter.ToSingle(stream.Read(ref readBytes, sizeof(float)), 0);
			w = BitConverter.ToSingle(stream.Read(ref readBytes, sizeof(float)), 0);

			if (type == typeof(Vector4))
				return new Vector4(x, y, z, w);
			else if (type == typeof(Color))
				return new Color(x, y, z, w);
			else// if (type == typeof(Quaternion))
				return new Quaternion(x, y, z, w);
		}

		/// <summary>
		/// Get a byte[] out of a Networking Stream
		/// </summary>
		/// <param name="args">Arguments passed through to get mapped</param>
		/// <returns>A byte[] of the mapped arguments</returns>
		public static BMSByte MapBytes(BMSByte bytes, params object[] args)
		{
			foreach (object o in args)
			{
				Type type = o.GetType();

				if (type == typeof(string))
				{
					// TODO:  Need to make custom string serialization to binary
					bytes.Append(BitConverter.GetBytes(((string)o).Length));
					bytes.Append(Encoding.UTF8.GetBytes((string)o));
				}
				else if (type == typeof(byte))
					bytes.BlockCopy<byte>(o, 1);
				else if (type == typeof(char))
					bytes.Append(BitConverter.GetBytes((char)o));	// TODO:  See if the same thing happens with a char as a byte
				else if (type == typeof(bool))
					bytes.Append(BitConverter.GetBytes((bool)o));
				else if (type == typeof(short))
					bytes.Append(BitConverter.GetBytes((short)o));
				else if (type == typeof(ushort))
					bytes.Append(BitConverter.GetBytes((ushort)o));
				else if (type == typeof(int))
					bytes.Append(BitConverter.GetBytes((int)o));
				else if (type == typeof(uint))
					bytes.Append(BitConverter.GetBytes((uint)o));
				else if (type == typeof(long))
					bytes.Append(BitConverter.GetBytes((long)o));
				else if (type == typeof(ulong))
					bytes.Append(BitConverter.GetBytes((ulong)o));
				else if (type == typeof(float))
					bytes.Append(BitConverter.GetBytes((float)o));
				else if (type == typeof(double))
					bytes.Append(BitConverter.GetBytes((double)o));
				else if (type == typeof(Vector2))
				{
					bytes.Append(BitConverter.GetBytes(((Vector2)o).x));
					bytes.Append(BitConverter.GetBytes(((Vector2)o).y));
				}
				else if (type == typeof(Vector3))
				{
					bytes.Append(BitConverter.GetBytes(((Vector3)o).x));
					bytes.Append(BitConverter.GetBytes(((Vector3)o).y));
					bytes.Append(BitConverter.GetBytes(((Vector3)o).z));
				}
				else if (type == typeof(Vector4))
				{
					bytes.Append(BitConverter.GetBytes(((Vector4)o).x));
					bytes.Append(BitConverter.GetBytes(((Vector4)o).y));
					bytes.Append(BitConverter.GetBytes(((Vector4)o).z));
					bytes.Append(BitConverter.GetBytes(((Vector4)o).w));
				}
				else if (type == typeof(Color))
				{
					bytes.Append(BitConverter.GetBytes(((Color)o).r));
					bytes.Append(BitConverter.GetBytes(((Color)o).g));
					bytes.Append(BitConverter.GetBytes(((Color)o).b));
					bytes.Append(BitConverter.GetBytes(((Color)o).a));
				}
				else if (type == typeof(Quaternion))
				{
					bytes.Append(BitConverter.GetBytes(((Quaternion)o).x));
					bytes.Append(BitConverter.GetBytes(((Quaternion)o).y));
					bytes.Append(BitConverter.GetBytes(((Quaternion)o).z));
					bytes.Append(BitConverter.GetBytes(((Quaternion)o).w));
				}
				else if (type == typeof(Animator))
				{
					// TODO:  Serialize animator properties
				}
				else
					throw new NetworkException(11, "The type " + type.ToString() + " is not allowed to be sent over the network (yet)");
			}

			return bytes;
		}
	}
}