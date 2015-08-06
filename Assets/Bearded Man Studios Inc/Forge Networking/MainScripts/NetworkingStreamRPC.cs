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
using System.Reflection;

namespace BeardedManStudios.Network
{
	public class NetworkingStreamRPC : NetworkingStream
	{
		/// <summary>
		/// Default method name for Network Instantiation
		/// </summary>
		public const string INSTANTIATE_METHOD_NAME = "NetworkInstantiate";

		/// <summary>
		/// The Method name for the NetworkingStream
		/// </summary>
		public string MethodName { get; private set; }

		/// <summary>
		/// Arguments passed through the Networkingstream
		/// </summary>
		public object[] Arguments { get; private set; }

		/// <summary>
		/// Target for the NetworkingStream
		/// </summary>
		public NetworkedMonoBehavior Target { get; private set; }

		/// <summary>
		/// Basic constructor with a Protocol Type
		/// </summary>
		/// <param name="protocolType"></param>
		public NetworkingStreamRPC(Networking.ProtocolType protocolType) : base(protocolType) { }

		/// <summary>
		/// Constructor for the NetworkingStream with a passed in stream
		/// </summary>
		/// <param name="stream">The stream passed in to be used</param>
		public NetworkingStreamRPC(NetworkingStream stream)
		{
			// TODO:  Check for null NetworkedBehavior or if it is the base class
			if (ReferenceEquals(stream.NetworkedBehavior, null))
				return;

#if NETFX_CORE
			IEnumerable<PropertyInfo> properties = stream.GetType().GetRuntimeProperties();
#else
			PropertyInfo[] properties = stream.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#endif
			foreach (PropertyInfo property in properties)
			{
				if (property.CanRead && property.CanWrite)
					property.SetValue(this, property.GetValue(stream, null), null);
			}

			NetworkedBehavior.InvokeRPC(this);
		}

		/// <summary>
		/// Set the method name for the Networking Stream
		/// </summary>
		/// <param name="methodName">Method name to be set to</param>
		public void SetName(string methodName)
		{
			MethodName = methodName;
		}

		/// <summary>
		/// Set the arguments for the Networking Stream
		/// </summary>
		/// <param name="arguments">Arguments to be set for the Networking Stream</param>
		public void SetArguments(object[] arguments)
		{
			Arguments = arguments;
		}

		private byte[] idReplacer = new byte[8];

		/// <summary>
		/// Check the argument update with the stream and start index
		/// </summary>
		/// <param name="stream">Stream to be updated</param>
		/// <param name="start">Start index</param>
		public ulong SetupInstantiateId(NetworkingStream stream, int start)
		{
			if (MethodName == INSTANTIATE_METHOD_NAME)
			{
				idReplacer = BitConverter.GetBytes(SimpleNetworkedMonoBehavior.GenerateUniqueId());

				for (int i = 0; i < idReplacer.Length; i++)
					stream.Bytes.byteArr[start + sizeof(ulong) + i] = idReplacer[i];
			}

			return SimpleNetworkedMonoBehavior.ObjectCounter;
		}
	}
}