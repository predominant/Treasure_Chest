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
using System.Collections;
using System.IO;

public class BluetoothSocketWrapper
{
#if UNITY_ANDROID
	private const string bluetoothSocketDLL = "com.UnityBluetoothBridge.Socket"; //NO DLL for other platforms'
	private static AndroidJavaClass bluetoothSocket;
#elif UNITY_IPHONE
	private const string bluetoothSocketDLL = " "; //NO DLL for other platforms
#else
	private const string bluetoothSocketDLL = " "; //NO DLL for other platforms
#endif

#if UNITY_ANDROID
	public BluetoothSocketWrapper(AndroidJavaClass other)
#elif UNITY_IPHONE
	public BluetoothSocketWrapper()
#else
	public BluetoothSocketWrapper()
#endif
	{
#if UNITY_ANDROID
		bluetoothSocket = other;
#elif UNITY_IPHONE
#else
#endif
	}

	public static void Connect(string address, string uuid)
	{
		SetBluetoothSocket();

		if (IsSocketNull())
			return;

#if UNITY_ANDROID
		bluetoothSocket.Call("Connect", address, uuid);
#else
#endif
	}

	public static StreamWriter GetInputStream() //NOT TESTED TO WORK
	{
		SetBluetoothSocket();

		if (IsSocketNull())
			return null;

#if UNITY_ANDROID
		return null;
#else
		return null;
#endif
	}
	public static StreamReader GetOutputStream() //NOT TESTED TO WORK
	{
		SetBluetoothSocket();

		if (IsSocketNull())
			return null;

#if UNITY_ANDROID
		return null;
#else
		return null;
#endif
	}

	public static BluetoothDeviceWrapper GetRemoteDevice()
	{
		SetBluetoothSocket();

		if (IsSocketNull())
			return null;

#if UNITY_ANDROID
		return new BluetoothDeviceWrapper(bluetoothSocket.CallStatic<AndroidJavaObject>("GetRemoteDevice"));
#else
		return null;
#endif
	}

	public static bool IsConnected()
	{
		SetBluetoothSocket();

		if (IsSocketNull())
			return false;

#if UNITY_ANDROID
		return bluetoothSocket.CallStatic<bool>("IsConnected");
#else
		return false;
#endif
	}

	public static void Close()
	{
		SetBluetoothSocket();

		if (IsSocketNull())
			return;

#if UNITY_ANDROID
		bluetoothSocket.Call("Close");
#else
#endif
	}

	private static void SetBluetoothSocket()
	{
#if UNITY_ANDROID
		if (bluetoothSocket != null)
			return;

		bluetoothSocket = new AndroidJavaClass(bluetoothSocketDLL); 
#else
#endif
	}

	private static bool IsSocketNull()
	{
#if UNITY_ANDROID
		return bluetoothSocket == null;
#else
		return true;
#endif
	}
}