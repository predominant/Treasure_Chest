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
using System.Reflection;

namespace BeardedManStudios.Network
{
	sealed class NetRef<T>
	{
		/// <summary>
		/// The method for getting the value of the target object
		/// </summary>
		private readonly Func<T> getter;

		/// <summary>
		/// The method for setting the value of the target object
		/// </summary>
		private readonly Action<T> setter;

		/// <summary>
		/// The callback to be executed when the value of the target object has changed
		/// </summary>
		public readonly Action callback;

		/// <summary>
		/// The type of callers that will be allowed to invoke the callback when the value has changed
		/// </summary>
		public readonly NetworkCallers callbackCallers;

		public NetRef(Func<T> getter, Action<T> setter, Action callback, NetworkCallers callbackCallers)
		{
			this.getter = getter;
			this.setter = setter;
			this.callback = callback;
			this.callbackCallers = callbackCallers;

			PreviousValue = getter();
		}

		/// <summary>
		/// Tells if this value is currently lerping
		/// </summary>
		public bool Lerping { get; private set; }

		/// <summary>
		/// The object to lerp the value to
		/// </summary>
		public T LerpTo { get; private set; }

		/// <summary>
		/// The speed of the lerp for this object
		/// </summary>
		public float LerpT { get; set; }

		/// <summary>
		/// The previous value for this object
		/// </summary>
		public T PreviousValue { get; private set; }

		/// <summary>
		/// The current value of the target object
		/// </summary>
		public T Value { get { return getter(); } set { PreviousValue = value; setter(value); } }

		/// <summary>
		/// Used to determine if the value has changed at all
		/// </summary>
		public bool IsDirty { get { return !Equals(PreviousValue, Value); } }

		/// <summary>
		/// Used to setup the lerp for this object
		/// </summary>
		/// <param name="to">The target object to lerp to</param>
		public void Lerp(T to)
		{
			Lerping = true;
			LerpTo = to;
			PreviousValue = to;
		}

		/// <summary>
		/// Finalizes the setting of the object's value
		/// </summary>
		public void Clean() { PreviousValue = Value; }

		/// <summary>
		/// Forcefully assign the value to the lerp destination object
		/// </summary>
		public void AssignToLerp()
		{
			Value = LerpTo;
		}

		/// <summary>
		/// Call the callback that is attached to this value change
		/// </summary>
		/// <param name="sender">The object that has the value that is being watched</param>
		public void Callback(NetworkedMonoBehavior sender, bool overrideDirty = false)
		{
			// Only execute if the value has changed
			if (callback == null || (!IsDirty && !overrideDirty))
				return;

			switch (callbackCallers)
			{
				case NetworkCallers.Everyone:
					callback();
					break;
				case NetworkCallers.Others:
					if (!sender.IsOwner)
						callback();
					break;
				case NetworkCallers.Owner:
					if (sender.IsOwner)
						callback();
					break;
			}
		}
	}
}