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
using System.Text;

using UnityEngine;

namespace BeardedManStudios.Network
{
	public sealed class BMSByte
	{
		private static Dictionary<Type, Array> allowedTypes = new Dictionary<Type, Array>()
		{
			{ typeof(char), new char[1] },
			{ typeof(string), new string[1] },
			{ typeof(bool), new bool[1] },
			{ typeof(byte), new byte[1] },
			{ typeof(short), new short[1] },
			{ typeof(ushort), new ushort[1] },
			{ typeof(int), new int[1] },
			{ typeof(uint), new uint[1] },
			{ typeof(long), new long[1] },
			{ typeof(ulong), new ulong[1] },
			{ typeof(float), new float[1] },
			{ typeof(double), new double[1] },
		};

		private int index = 0;

		/// <summary>
		/// The starting byte for the internal byte array
		/// </summary>
		public int StartPointer { get; private set; }

		/// <summary>
		/// The interpreted size of the internal byte array
		/// </summary>
		public int Size { get; private set; }

		/// <summary>
		/// The internal byte array
		/// </summary>
		public byte[] byteArr = new byte[1];

		public BMSByte(bool raw = false) 
		{
			Size = 0;
			StartPointer = 0;

			if (raw)
				Append(new byte[] { 1 });
		}
		
		/// <summary>
		/// Manually set the interpreted size of the interal byte array (will resize if larger)
		/// </summary>
		/// <param name="newSize">The size to be resized to</param>
		public void SetSize(int newSize)
		{
			if (byteArr.Length < newSize)
				Array.Resize<byte>(ref byteArr, newSize);

			Size = newSize;
		}

		/// <summary>
		/// Get the starting index (virtual head) of the internal byte array
		/// </summary>
		/// <param name="offset">Adds this amount to the internal index to start from there</param>
		/// <returns></returns>
		public int StartIndex(int offset = 0)
		{
			return StartPointer + offset;
		}

		/// <summary>
		/// Moves the starting index (virtual head) manually
		/// </summary>
		/// <param name="offset">The amount to move the starting index (virtual head) by</param>
		public int MoveStartIndex(int offset)
		{
			StartPointer += offset;
			
			if (index < StartPointer)
				index = StartPointer;

			Size -= offset;

			return StartPointer;
		}

		/// <summary>
		/// Resets the internal byte array and assignes the passed byte array to it
		/// </summary>
		/// <param name="input">The bytes to be cloned into the internal byte array</param>
		public BMSByte Clone(byte[] input)
		{
			PointToStart();

			if (byteArr.Length <= input.Length)
				Array.Resize<byte>(ref byteArr, input.Length + 1);

			Buffer.BlockCopy(input, 0, byteArr, index, input.Length);
			Size = input.Length;
			PointToEnd();

			return this;
		}

		/// <summary>
		/// Resets the internal byte array and assignes the internal byte array of the passed in BMSByte to this ones internal byte array
		/// </summary>
		/// <param name="otherBytes">The other BMSByte that will have its internal byte array cloned to this one</param>
		/// <returns></returns>
		public BMSByte Clone(BMSByte otherBytes)
		{
			PointToStart();

			if (byteArr.Length <= otherBytes.Size)
				Array.Resize<byte>(ref byteArr, otherBytes.Size + 1);

			Buffer.BlockCopy(otherBytes.byteArr, otherBytes.StartIndex(), byteArr, StartIndex() + index, otherBytes.Size);

			Size = otherBytes.Size;
			PointToEnd();

			return this;
		}

		/// <summary>
		/// Adds the bytes to the end of the byte array and resizes when needed
		/// </summary>
		/// <param name="input">The bytes to be appended to the end of the internal byte array</param>
		public void Append(byte[] input)
		{
			if (byteArr.Length <= index + input.Length)
				Array.Resize<byte>(ref byteArr, index + input.Length + 1);

			Buffer.BlockCopy(input, 0, byteArr, index, input.Length);
			
			Size += input.Length;

			PointToEnd();
		}

		/// <summary>
		/// Copies the passed byte array starting at the start index and for a specified count to
		/// the end of the internal byte array and resizes when needed
		/// </summary>
		/// <param name="input">The byte array to be appended to the end of the internal byte array</param>
		/// <param name="start">What index to start copying from on the passed byte array</param>
		/// <param name="count">How many bytes to read from the start of the passed byte array</param>
		public void BlockCopy(byte[] input, int start, int count)
		{
			if (byteArr.Length <= index + count)
				Array.Resize<byte>(ref byteArr, count + Size + 1);

			Buffer.BlockCopy(input, start, byteArr, index, count);
			Size += count;
			PointToEnd();
		}

		/// <summary>
		/// Copies the passed object's bytes starting at the start index and for a specified count to
		/// the end of the internal byte array and resizes when needed
		/// </summary>
		/// <param name="obj">The object to be converted into bytes</param>
		/// <param name="count">The size in bytes to copy from the start of the passed object</param>
		public void BlockCopy<T>(object obj, int count)
		{
			if (byteArr.Length <= index + count)
				Array.Resize<byte>(ref byteArr, Size + count + 1);

			if (obj is string)
				Buffer.BlockCopy(Encoding.UTF8.GetBytes(((string)obj)), 0, byteArr, index, count);
			else
				Buffer.BlockCopy(GetArray(typeof(T), obj), 0, byteArr, index, count);

			Size = index + count;
			PointToEnd();
		}

		/// <summary>
		/// Removes a range of bytes from the internal byte array
		/// </summary>
		/// <param name="count">How many bytes to be removed from the start</param>
		public void RemoveStart(int count)
		{
			if (count > byteArr.Length)
				throw new Exception("Can't remove more than what is in the array");

			StartPointer += count;

			Size -= count;

			if (index < StartPointer)
				index = StartPointer;
		}

		/// <summary>
		/// Remove a range of bytes from the internal byte array, this will iterate
		/// and rearrange so using it often is not suggested, use RemoveStart when you can
		/// </summary>
		/// <param name="start">The start index for the internal byte array to begin removing bytes from</param>
		/// <param name="count">The amount of bytes to remove starting from the passed in start</param>
		public void RemoveRange(int start, int count)
		{
			if (start + count > byteArr.Length)
				throw new Exception("Can't remove more than what is in the array");

			for (int i = 0; i < byteArr.Length - count - start - 1; i++)
				byteArr[start + i] = byteArr[start + count + i];

			Size -= count;
			index -= count;
		}

		/// <summary>
		/// Compresses the byte array so that it is sent properly across the network
		/// This is often used just as you are sending the message
		/// </summary>
		/// <returns>Returns this BMSByte object</returns>
		public BMSByte Compress()
		{
			if (StartPointer == 0)
				return this;

			for (int i = 0; i < byteArr.Length - StartPointer - 1; i++)
				byteArr[i] = byteArr[StartPointer + i];
		
			StartPointer = 0;

			return this;
		}

		/// <summary>
		/// Inserts a set of bytes at a specified index into the internal byte array
		/// </summary>
		/// <param name="start">The index that the new byte array will be inserted from in the internal byte array</param>
		/// <param name="data">The bytes that will be inserted at the specified point</param>
		public void InsertRange(int start, byte[] data)
		{
			if (byteArr.Length <= Size + data.Length)
				Array.Resize<byte>(ref byteArr, data.Length + Size);

			for (int i = byteArr.Length - 1; i > start + data.Length - 1; i--)
				byteArr[i] = byteArr[i - data.Length];

			Size = index + data.Length - StartPointer;
			index = start;

			Buffer.BlockCopy(data, 0, byteArr, index, data.Length);

			PointToEnd();
		}

		/// <summary>
		/// Inserts a set of bytes at a specified index into the internal byte array
		/// </summary>
		/// <param name="start">The index that the new byte array will be inserted from in the internal byte array</param>
		/// <param name="data">The BMSByte object will be inserted at the specified point</param>
		public void InsertRange(int start, BMSByte data)
		{
			if (byteArr.Length <= Size + data.Size)
				Array.Resize<byte>(ref byteArr, data.Size + Size);

			for (int i = byteArr.Length - 1; i > start + data.Size - 1; i--)
				byteArr[i] = byteArr[i - data.Size];

			Size = index + data.Size - StartPointer;
			index = start;

			Buffer.BlockCopy(data.byteArr, data.StartIndex(), byteArr, index, data.Size);

			PointToEnd();
		}

		private void PointToEnd()
		{
			index = Size;
		}

		private void PointToStart()
		{
			index = 0;
			StartPointer = 0;
		}

		public void ResetPointer()
		{
			PointToStart();
		}

		/// <summary>
		/// Clears out the internal byte array
		/// </summary>
		public void Clear()
		{
			index = 0;
			Size = 0;
			StartPointer = 0;
		}

		private Array GetArray(Type type, object o)
		{
			if (type == typeof(byte))
			{
				((byte[])allowedTypes[typeof(byte)])[0] = (byte)o;
				return allowedTypes[typeof(byte)];
			}
			else if (type == typeof(char))
			{
				((char[])allowedTypes[typeof(char)])[0] = (char)o;
				return allowedTypes[typeof(char)];
			}
			else if (type == typeof(bool))
			{
				((bool[])allowedTypes[typeof(bool)])[0] = (bool)o;
				return allowedTypes[typeof(bool)];
			}
			else if (type == typeof(short))
			{
				((short[])allowedTypes[typeof(short)])[0] = (short)o;
				return allowedTypes[typeof(short)];
			}
			else if (type == typeof(ushort))
			{
				((ushort[])allowedTypes[typeof(ushort)])[0] = (ushort)o;
				return allowedTypes[typeof(ushort)];
			}
			else if (type == typeof(int))
			{
				((int[])allowedTypes[typeof(int)])[0] = (int)o;
				return allowedTypes[typeof(int)];
			}
			else if (type == typeof(uint))
			{
				((uint[])allowedTypes[typeof(uint)])[0] = (uint)o;
				return allowedTypes[typeof(uint)];
			}
			else if (type == typeof(long))
			{
				((long[])allowedTypes[typeof(long)])[0] = (long)o;
				return allowedTypes[typeof(long)];
			}
			else if (type == typeof(ulong))
			{
				((ulong[])allowedTypes[typeof(ulong)])[0] = (ulong)o;
				return allowedTypes[typeof(ulong)];
			}
			else if (type == typeof(float))
			{
				((float[])allowedTypes[typeof(float)])[0] = (float)o;
				return allowedTypes[typeof(float)];
			}
			else if (type == typeof(double))
			{
				((double[])allowedTypes[typeof(double)])[0] = (double)o;
				return allowedTypes[typeof(double)];
			}
			else
				throw new Exception("The type " + type.ToString() + " is not allowed");
		}

		private Array GetArray(object obj)
		{
			if (obj is byte)
			{
				((byte[])allowedTypes[typeof(byte)])[0] = (byte)obj;
				return allowedTypes[typeof(byte)];
			}
			else if (obj is char)
			{
				((char[])allowedTypes[typeof(char)])[0] = (char)obj;
				return allowedTypes[typeof(char)];
			}
			else if (obj is bool)
			{
				((bool[])allowedTypes[typeof(bool)])[0] = (bool)obj;
				return allowedTypes[typeof(bool)];
			}
			else if (obj is short)
			{
				((short[])allowedTypes[typeof(short)])[0] = (short)obj;
				return allowedTypes[typeof(short)];
			}
			else if (obj is ushort)
			{
				((ushort[])allowedTypes[typeof(ushort)])[0] = (ushort)obj;
				return allowedTypes[typeof(ushort)];
			}
			else if (obj is int)
			{
				((int[])allowedTypes[typeof(int)])[0] = (int)obj;
				return allowedTypes[typeof(int)];
			}
			else if (obj is uint)
			{
				((uint[])allowedTypes[typeof(uint)])[0] = (uint)obj;
				return allowedTypes[typeof(uint)];
			}
			else if (obj is long)
			{
				((long[])allowedTypes[typeof(long)])[0] = (long)obj;
				return allowedTypes[typeof(long)];
			}
			else if (obj is ulong)
			{
				((ulong[])allowedTypes[typeof(ulong)])[0] = (ulong)obj;
				return allowedTypes[typeof(ulong)];
			}
			else if (obj is float)
			{
				((float[])allowedTypes[typeof(float)])[0] = (float)obj;
				return allowedTypes[typeof(float)];
			}
			else if (obj is double)
			{
				((double[])allowedTypes[typeof(double)])[0] = (double)obj;
				return allowedTypes[typeof(double)];
			}
			else
				throw new Exception("The type " + obj.GetType().ToString() + " is not allowed");
		}

		private int GetSize(object obj)
		{
			if (obj is byte)
				return 1;
			else if (obj is char)
				return 1;
			else if (obj is bool)
				return sizeof(bool);
			else if (obj is short)
				return sizeof(short);
			else if (obj is ushort)
				return sizeof(ushort);
			else if (obj is int)
				return sizeof(int);
			else if (obj is uint)
				return sizeof(uint);
			else if (obj is long)
				return sizeof(long);
			else if (obj is ulong)
				return sizeof(ulong);
			else if (obj is float)
				return sizeof(float);
			else if (obj is double)
				return sizeof(double);
			else
				throw new Exception("The type " + obj.GetType().ToString() + " is not allowed");
		}

		public T GetBasicType<T>(int start)
		{
			Type type = typeof(T);

			if (type == typeof(byte))
				return (T)(object)byteArr[0];
			else if (type == typeof(char))
				return (T)(object)byteArr[0];
			else if (type == typeof(short))
				return (T)(object)BitConverter.ToInt16(byteArr, start);
			else if (type == typeof(ushort))
				return (T)(object)BitConverter.ToUInt16(byteArr, start);
			else if (type == typeof(bool))
				return (T)(object)BitConverter.ToBoolean(byteArr, start);
			else if (type == typeof(int))
				return (T)(object)BitConverter.ToInt32(byteArr, start);
			else if (type == typeof(uint))
				return (T)(object)BitConverter.ToUInt32(byteArr, start);
			else if (type == typeof(float))
				return (T)(object)BitConverter.ToSingle(byteArr, start);
			else if (type == typeof(long))
				return (T)(object)BitConverter.ToInt64(byteArr, start);
			else if (type == typeof(ulong))
				return (T)(object)BitConverter.ToUInt64(byteArr, start);
			else if (type == typeof(double))
				return (T)(object)BitConverter.ToDouble(byteArr, start);
			else
				throw new NetworkException(11, "The type " + typeof(T).ToString() + " is gettable from basic type, maybe try one of the other getters?");
		}

		public object GetBasicType(Type type, int start)
		{
			if (type == typeof(byte))
				return (object)byteArr[0];
			else if (type == typeof(char))
				return (object)byteArr[0];
			else if (type == typeof(short))
				return (object)BitConverter.ToInt16(byteArr, start);
			else if (type == typeof(ushort))
				return (object)BitConverter.ToUInt16(byteArr, start);
			else if (type == typeof(bool))
				return (object)BitConverter.ToBoolean(byteArr, start);
			else if (type == typeof(int))
				return (object)BitConverter.ToInt32(byteArr, start);
			else if (type == typeof(uint))
				return (object)BitConverter.ToUInt32(byteArr, start);
			else if (type == typeof(float))
				return (object)BitConverter.ToSingle(byteArr, start);
			else if (type == typeof(long))
				return (object)BitConverter.ToInt64(byteArr, start);
			else if (type == typeof(ulong))
				return (object)BitConverter.ToUInt64(byteArr, start);
			else if (type == typeof(double))
				return (object)BitConverter.ToDouble(byteArr, start);
			else
				throw new NetworkException(11, "The type " + type.ToString() + " is gettable from basic type, maybe try one of the other getters?");
		}

		public string GetString(int start)
		{
			int length = BitConverter.ToInt32(byteArr, start);

			if (length <= 0)
				return string.Empty;

#if NETFX_CORE
			return Encoding.UTF8.GetString(stream.Read(ref readBytes, length), 0, length);
#else
			return Encoding.UTF8.GetString(byteArr, start + sizeof(int), length);
#endif
		}
		
		public object GetVector2(int start)
		{
			float x = BitConverter.ToSingle(byteArr, start);
			float y = BitConverter.ToSingle(byteArr, start);
			return new Vector2(x, y);
		}

		public Vector3 GetVector3(int start)
		{
			float x = BitConverter.ToSingle(byteArr, start);
			float y = BitConverter.ToSingle(byteArr, start);
			float z = BitConverter.ToSingle(byteArr, start);

			return new Vector3(x, y, z);
		}

		public Vector4 GetVector4(int start)
		{
			float x = BitConverter.ToSingle(byteArr, start);
			float y = BitConverter.ToSingle(byteArr, start);
			float z = BitConverter.ToSingle(byteArr, start);
			float w = BitConverter.ToSingle(byteArr, start);

			return new Vector4(x, y, z, w);
		}

		public Color GetColor(int start)
		{
			Vector4 found = GetVector4(start);
			return new Color(found.x, found.y, found.z, found.w);
		}

		public Quaternion GetQuaternion(int start)
		{
			Vector4 found = GetVector4(start);
			return new Quaternion(found.x, found.y, found.z, found.w);
		}

		/// <summary>
		/// Support for indexer
		/// </summary>
		/// <param name="i">The index of the byte array to access</param>
		/// <returns>The byte at the specidifed index</returns>
		public byte this[int i]
		{
			get { return byteArr[i]; }
			set { byteArr[i] = value; }
		}
	}
}