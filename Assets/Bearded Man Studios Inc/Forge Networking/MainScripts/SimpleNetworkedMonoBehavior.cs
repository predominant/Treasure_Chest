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

using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace BeardedManStudios.Network
{
	[AddComponentMenu("Forge Networking/Simple Networked MonoBehavior")]
	public class SimpleNetworkedMonoBehavior : MonoBehaviour, INetworkingSerialized
	{
		/// <summary>
		/// Determine if the initial setup has been done or not
		/// </summary>
		private static bool initialSetup = false;

		/// <summary>
		/// A list of all of the current networked behaviors
		/// </summary>
		public static Dictionary<ulong, SimpleNetworkedMonoBehavior> networkedBehaviors = new Dictionary<ulong, SimpleNetworkedMonoBehavior>();

		/// <summary>
		/// A temporary stack to initialize all of the Networked Behaviors on the Awake (of the scene, not each object)
		/// </summary>
		private static List<SimpleNetworkedMonoBehavior> stackPool = new List<SimpleNetworkedMonoBehavior>();

		/// <summary>
		/// Determines if all of the networked objects that are default to the scene have been set up
		/// </summary>
		private static bool stackPoolDone = false;

		/// <summary>
		/// A number that is used in assigning the unique id for a networked object
		/// </summary>
		public static ulong ObjectCounter { get; private set; }

		/// <summary>
		/// A list of actions that are to be called on the main thread for this object
		/// </summary>
		private static List<Action<object[]>> callOnMainThread = new List<Action<object[]>>();

		/// <summary>
		/// A list of arguments that are to be sent to the call on main thread method.  This should match 1:1
		/// </summary>
		private static List<object[]> callOnMainThreadArguments = new List<object[]>();

		/// <summary>
		/// A mutex object to prevent thread race conditions when adding, calling and removing main thread calls
		/// </summary>
		private static object callOnMainThreadMutex = new object();

		/// <summary>
		/// Determine if the object calling this boolean is the owner of this networked object
		/// </summary>
		public bool IsOwner { get; protected set; }

		/// <summary>
		/// The player id who currently owns this networked object
		/// </summary>
		public ulong OwnerId { get; protected set; }

		/// <summary>
		/// Used to map BRPC methods to integers for bandwidth compression
		/// </summary>
		protected Dictionary<int, MethodInfo> RPCs { get; set; }

		/// <summary>
		/// A list of RPC (remote methods) that are pending to be called for this object
		/// </summary>
		protected List<NetworkingStreamRPC> rpcStack = new List<NetworkingStreamRPC>();

		/// <summary>
		/// The sender of the RPC (player who requested the RPC to be called)
		/// </summary>
		protected NetworkingPlayer CurrentRPCSender { get; set; }

		// TODO:  Optimization when an object is removed there needs to be a way to replace its spot
		/// <summary>
		/// The Network ID of this Simple Networked Monobehavior
		/// </summary>
		public ulong NetworkedId { get; private set; }

		/// <summary>
		/// If this object has been setup on the network
		/// </summary>
		public bool IsSetup { get; private set; }

		/// <summary>
		/// The owning socket (Net Worker) for this Simple Networked Monobehavior
		/// </summary>
		public NetWorker OwningNetWorker { get; protected set; }

		/// <summary>
		/// This is used by the server in order to easily associate this object to its owning player
		/// </summary>
		public NetworkingPlayer OwningPlayer { get; protected set; }

		/// <summary>
		/// The stream that is re-used for the RPCs that are being sent from this object
		/// </summary>
		private NetworkingStream rpcNetworkingStream = new NetworkingStream();

		/// <summary>
		/// The main cached buffer the RPC network stream
		/// </summary>
		private BMSByte getStreamBuffer = new BMSByte();

		/// <summary>
		/// Used to queue up methods that are to be called on the main thread for this object
		/// </summary>
		/// <param name="action">The method that is to be called on the main thread</param>
		/// <param name="arguments">The arguments that are to be passed to the method</param>
		protected static void CallOnMainThread(Action<object[]> action, object[] arguments = null)
		{
			lock (callOnMainThreadMutex)
			{
				callOnMainThread.Add(action);
				callOnMainThreadArguments.Add(arguments);
			}
		}

		/// <summary>
		/// Get a generated Unique ID for the next simple networked mono behavior or its derivitive
		/// </summary>
		/// <returns>A Unique unsigned long ID</returns>
		public static ulong GenerateUniqueId()
		{
			if (Networking.IsBareMetal && ObjectCounter == 0)
				ObjectCounter++;

			return ++ObjectCounter;
		}

		/// <summary>
		/// Locate a Simple Networked Monobehavior given a ID
		/// </summary>
		/// <param name="id">ID of the Simple Networked Monobehavior</param>
		/// <returns>The Simple Networked Monobehavior found or <c>null</c> if not found</returns>
		public static SimpleNetworkedMonoBehavior Locate(ulong id)
		{
			if (networkedBehaviors.ContainsKey(id))
				return networkedBehaviors[id];

			return null;
		}

		/// <summary>
		/// Destroy a Simple Networked Monobehavior or any of its derivitives with the given Network ID
		/// </summary>
		/// <param name="networkId">Network ID to be destroyed</param>
		/// <returns><c>True</c> if the network behavoir was destroy, otherwise <c>False</c></returns>
		public static bool NetworkDestroy(ulong networkId)
		{
			// Make sure the object exists on the network before calling destroy
			SimpleNetworkedMonoBehavior behavior = Locate(networkId);

			if (behavior == null)
				return false;

			// Destroy the object from the scene and remove it from the lookup
			GameObject.Destroy(behavior.gameObject);
			networkedBehaviors.Remove(networkId);
			return true;
		}

		// TODO:  Test this to see if it is needed any longer it may be depricated
		private void OnDestroy()
		{
			stackPoolDone = false;
		}

		protected virtual void Awake()
		{
			if (stackPool == null && !stackPoolDone)
				stackPool = new List<SimpleNetworkedMonoBehavior>();

			RPCs = new Dictionary<int, MethodInfo>();
#if NETFX_CORE
			IEnumerable<MethodInfo> methods = this.GetType().GetRuntimeMethods();
#else
			MethodInfo[] methods = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#endif
			foreach (MethodInfo method in methods)
			{
#if NETFX_CORE
				if (method.GetCustomAttribute<BRPC>() != null)
#else
				if (method.GetCustomAttributes(typeof(BRPC), true).Length != 0)
#endif
				{
					RPCs.Add(RPCs.Count, method);
				}
			}

			if (stackPool != null)
				stackPool.Add(this);
		}

		/// <summary>
		/// An initial setup to make sure that a networking manager exists before running any core logic
		/// </summary>
		private static void Initialize()
		{
			if (NetworkingManager.Instance == null)
				Instantiate(UnityEngine.Resources.Load<GameObject>("BeardedManStudios/Networking Manager"));

			NetworkingManager.Instance.Populate();

			initialSetup = true;
		}

		private void ThrowNetworkerException()
		{
#if UNITY_EDITOR
			Debug.Log("Try using the Forge Quick Start Menu and setting the \"Scene Name\" on the \"Canvas\" to the scene you are loading. Then running from that scene.");
#endif

			throw new NetworkException("The NetWorker doesn't exist. Is it possible that a connection hasn't been made?");
		}

		protected virtual void Start()
		{
			if (!initialSetup)
				Initialize();
		}

		/// <summary>
		/// A start method that is called after the object has been setup on the network
		/// </summary>
		protected virtual void NetworkStart() { IsSetup = true; }

		/// <summary>
		/// Setup the Simple Networked Monobehavior stack with a NetWorker
		/// </summary>
		/// <param name="owningSocket">The NetWorker to be setup with</param>
		public static void SetupObjects(NetWorker owningSocket)
		{
			foreach (SimpleNetworkedMonoBehavior stack in stackPool)
			{
				if (stack != NetworkingManager.Instance)
					continue;

				stack.Setup(owningSocket, owningSocket.IsServer, GenerateUniqueId(), 0);
			}

			foreach (SimpleNetworkedMonoBehavior stack in stackPool.OrderBy(s => s.GetType().Name))
			{
				if (stack == NetworkingManager.Instance)
					continue;

				// Automatically set this to be the server initially
				stack.Setup(owningSocket, owningSocket.IsServer, GenerateUniqueId(), 0);
			}

			stackPoolDone = true;
			stackPool.Clear();
			stackPool = null;
		}

		/// <summary>
		/// Setup the Simple Networked Monobehavior stack with a NetWorker, owner, network id, and owner id
		/// </summary>
		/// <param name="owningSocket">The NetWorker to be setup with</param>
		/// <param name="isOwner">If this object is the owner</param>
		/// <param name="networkId">The NetworkID for this Simple Networked Monobehavior</param>
		/// <param name="ownerId">The OwnerID for this Simple Networked Monobehavior</param>
		public virtual void Setup(NetWorker owningSocket, bool isOwner, ulong networkId, ulong ownerId)
		{
			if (owningSocket == null)
				ThrowNetworkerException();

			OwningNetWorker = owningSocket;
			IsOwner = isOwner;
			OwnerId = ownerId;
			NetworkedId = networkId;
			networkedBehaviors.Add(NetworkedId, this);

			if (OwningNetWorker.IsServer)
			{
				foreach (NetworkingPlayer player in OwningNetWorker.Players)
				{
					if (ownerId == player.NetworkId)
					{
						OwningPlayer = player;
						break;
					}
				}
			}
			else if (OwningNetWorker.Me != null && ownerId == OwningNetWorker.Me.NetworkId)
				OwningPlayer = OwningNetWorker.Me;

			NetworkStart();
		}

		protected virtual void Update()
		{
			// If there are any pending RPC calls, then do them now on the main thread
			if (rpcStack.Count != 0)
			{
				NetworkingStreamRPC stream = rpcStack[0];
				rpcStack.RemoveAt(0);

				foreach (KeyValuePair<int, MethodInfo> rpc in RPCs)
				{
					if (stream == null)
						return;

					if (rpc.Value.Name == stream.MethodName)
					{
						CurrentRPCSender = stream.Sender;
						rpc.Value.Invoke(this, stream.Arguments);
						CurrentRPCSender = null;
						return;
					}
				}

				throw new NetworkException(13, "Invoked network method " + stream.MethodName + " not found or not marked with [BRPC]");
			}

			// If there are any pending main thread calls, then do them now
			lock (callOnMainThreadMutex)
			{
				if (callOnMainThread.Count > 0)
				{
					for (int i = 0; i < callOnMainThread.Count; i++)
						callOnMainThread[i](callOnMainThreadArguments[i]);

					callOnMainThread.Clear();
					callOnMainThreadArguments.Clear();
				}
			}
		}

		int tmpRPCMapId = 0;
		/// <summary>
		/// To Invoke an RPC on a given Networking Stream RPC
		/// </summary>
		/// <param name="stream">Networking Stream RPC to read from</param>
		public void InvokeRPC(NetworkingStreamRPC stream)
		{
			tmpRPCMapId = ObjectMapper.Map<int>(stream);
			if (!RPCs.ContainsKey(tmpRPCMapId))
				return;

			stream.SetName(RPCs[tmpRPCMapId].Name);

			List<object> args = new List<object>();

			MethodInfo invoke = null;
			foreach (KeyValuePair<int, MethodInfo> m in RPCs)
			{
				if (m.Value.Name == stream.MethodName)
				{
					invoke = m.Value;
					break;
				}
			}

			int start = stream.Bytes.StartIndex(stream.ByteReadIndex);
			ParameterInfo[] pars = invoke.GetParameters();
			foreach (ParameterInfo p in pars)
				args.Add(ObjectMapper.Map(p.ParameterType, (NetworkingStream)stream));

			stream.SetArguments(args.ToArray());

			if (OwningNetWorker.IsServer && stream.MethodName == NetworkingStreamRPC.INSTANTIATE_METHOD_NAME)
				stream.Arguments[1] = stream.SetupInstantiateId(stream, start);

			rpcStack.Add(stream);
		}

		/// <summary>
		/// Creates a network stream for the method with the specified string name and returns the method info
		/// </summary>
		/// <param name="methodName">The name of the method to call from this class</param>
		/// <param name="receivers">The players on the network that will be receiving RPC</param>
		/// <param name="arguments">The list of arguments that will be sent for the RPC</param>
		/// <returns></returns>
		private MethodInfo GetStreamRPC(string methodName, NetworkReceivers receivers, params object[] arguments)
		{
			foreach (KeyValuePair<int, MethodInfo> rpc in RPCs)
			{
				if (rpc.Value.Name == methodName)
				{
					getStreamBuffer.Clear();
					ObjectMapper.MapBytes(getStreamBuffer, rpc.Key);

					if (arguments != null && arguments.Length > 0)
						ObjectMapper.MapBytes(getStreamBuffer, arguments);

					bool buffered = receivers == NetworkReceivers.AllBuffered || receivers == NetworkReceivers.OthersBuffered;

					rpcNetworkingStream.SetProtocolType(OwningNetWorker is CrossPlatformUDP ? Networking.ProtocolType.UDP : Networking.ProtocolType.TCP);
					rpcNetworkingStream.Prepare(OwningNetWorker, NetworkingStream.IdentifierType.RPC, this, getStreamBuffer, receivers, buffered);

					return rpc.Value;
				}
			}

			throw new NetworkException(14, "No method marked with [BRPC] was found by the name " + methodName);
		}

		/// <summary>
		/// Used for the server to call an RPC method on a NetWorker(Socket) on a particular player
		/// </summary>
		/// <param name="methodName">Method(Function) name to call</param>
		/// <param name="socket">The NetWorker(Socket) being used</param>
		/// <param name="player">The NetworkingPlayer who will execute this RPC</param>
		/// <param name="arguments">The RPC function parameters to be passed in</param>
		public void AuthoritativeRPC(string methodName, NetWorker socket, NetworkingPlayer player, bool runOnServer, params object[] arguments)
		{
			MethodInfo rpc = GetStreamRPC(methodName, NetworkReceivers.All, arguments);

			if (socket is CrossPlatformUDP)
				((CrossPlatformUDP)socket).Write("BMS_INTERNAL_Rpc_" + methodName, player, rpcNetworkingStream, true);
			else
				socket.Write(player, rpcNetworkingStream);

			if (socket.IsServer && runOnServer)
			{
				CallOnMainThread(delegate(object[] args)
				{
					rpc.Invoke(this, arguments);
				});
			}
		}

		/// <summary>
		/// Call an RPC method on a NetWorker(Socket) with receivers and arguments
		/// </summary>
		/// <param name="methodName">Method(Function) name to call</param>
		/// <param name="socket">The NetWorker(Socket) being used</param>
		/// <param name="receivers">Who shall receive the RPC</param>
		/// <param name="arguments">The RPC function parameters to be passed in</param>
		public void RPC(string methodName, NetWorker socket, NetworkReceivers receivers, params object[] arguments)
		{
			MethodInfo rpc = GetStreamRPC(methodName, receivers, arguments);

			if (socket is CrossPlatformUDP)
				((CrossPlatformUDP)socket).Write("BMS_INTERNAL_Rpc_" + methodName, rpcNetworkingStream, true);
			else
				socket.Write(rpcNetworkingStream);

			if (socket.IsServer && receivers != NetworkReceivers.Others && receivers != NetworkReceivers.OthersBuffered && receivers != NetworkReceivers.OthersProximity)
			{
				CallOnMainThread(delegate(object[] args)
				{
					rpc.Invoke(this, arguments);
				});
			}
		}

		/// <summary>
		/// Call an Unreliable RPC method on a NetWorker(Socket) with receivers and arguments
		/// </summary>
		/// <param name="methodName">Method(Function) name to call</param>
		/// <param name="socket">The NetWorker(Socket) being used</param>
		/// <param name="receivers">Who shall receive the RPC</param>
		/// <param name="arguments">The RPC function parameters to be passed in</param>
		public void URPC(string methodName, NetWorker socket, NetworkReceivers receivers, params object[] arguments)
		{
			MethodInfo rpc = GetStreamRPC(methodName, receivers, arguments);

			if (socket is CrossPlatformUDP)
				((CrossPlatformUDP)socket).Write("BMS_INTERNAL_Rpc_" + methodName, rpcNetworkingStream, false);
			else
				socket.Write(rpcNetworkingStream);

			if (socket.IsServer && receivers != NetworkReceivers.Others && receivers != NetworkReceivers.OthersBuffered && receivers != NetworkReceivers.OthersProximity)
				rpc.Invoke(this, arguments);
		}

		/// <summary>
		/// Call an RPC method with arguments
		/// </summary>
		/// <param name="methodName">Method(Function) name to call</param>
		/// <param name="arguments">Extra parameters passed in</param>
		public void RPC(string methodName, params object[] arguments)
		{
			RPC(methodName, OwningNetWorker, NetworkReceivers.All, arguments);
		}

		/// <summary>
		/// Call an RPC method with a receiver and arguments
		/// </summary>
		/// <param name="methodName">Method(Function) name to call</param>
		/// <param name="rpcMode">Who shall receive the RPC</param>
		/// <param name="arguments">Extra parameters passed in</param>
		public void RPC(string methodName, NetworkReceivers rpcMode, params object[] arguments)
		{
			RPC(methodName, OwningNetWorker, rpcMode, arguments);
		}

		/// <summary>
		/// Call an Unreliable RPC method with a receiver and arguments
		/// </summary>
		/// <param name="methodName">Method(Function) name to call</param>
		/// <param name="rpcMode">Who shall receive the RPC</param>
		/// <param name="arguments">Extra parameters passed in</param>
		public void URPC(string methodName, NetworkReceivers rpcMode, params object[] arguments)
		{
			URPC(methodName, OwningNetWorker, rpcMode, arguments);
		}

		/// <summary>
		/// Call an RPC method with a NetWorker(Socket) and arguments
		/// </summary>
		/// <param name="methodName">Method(Function) name to call</param>
		/// <param name="socket">The NetWorker(Socket) being used</param>
		/// <param name="arguments">Extra parameters passed in</param>
		public void RPC(string methodName, NetWorker socket, params object[] arguments)
		{
			RPC(methodName, socket, NetworkReceivers.All, arguments);
		}

		/// <summary>
		/// Serialize the Simple Networked Monobehavior
		/// </summary>
		/// <returns></returns>
		public virtual BMSByte Serialized() { return null; }

		/// <summary>
		/// Deserialize the Networking Stream
		/// </summary>
		/// <param name="stream">Stream to be deserialized</param>
		public virtual void Deserialize(NetworkingStream stream) { }

		/// <summary>
		/// Used to do final cleanup when disconnecting. This gets called currently only on application quit
		/// </summary>
		public virtual void Disconnect() { }

		protected virtual void OnApplicationQuit() { Disconnect(); }
	}
}