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

public class BluetoothServerSocketWrapper
{
#if UNITY_ANDROID
	private const string bluetoothServerSocketDLL = "com.UnityBluetoothBridge.ServerSocket"; //NO DLL for other platforms'
	private static AndroidJavaClass bluetoothServerSocket;
#elif UNITY_IPHONE
	private const string bluetoothServerSocketDLL = " "; //NO DLL for other platforms
#else
	private const string bluetoothServerSocketDLL = " "; //NO DLL for other platforms
#endif

	public static void StartListener(string address, string uuid)
	{
		SetBluetoothServerSocket();

		if (IsServerSocketNull())
			return;

#if UNITY_ANDROID
		bluetoothServerSocket.Call("StartListener", address, uuid);
#else
#endif
	}

	public static BluetoothSocketWrapper Accept() //NOT TESTED
	{
		SetBluetoothServerSocket();

		if (IsServerSocketNull())
			return null;

#if UNITY_ANDROID
		return new BluetoothSocketWrapper(bluetoothServerSocket.Call<AndroidJavaClass>("Accept"));
#else
#endif

		return null;
	}

	public static BluetoothSocketWrapper Accept(int timeout)
	{
		SetBluetoothServerSocket();

		if (IsServerSocketNull())
			return null;

#if UNITY_ANDROID
		return new BluetoothSocketWrapper(bluetoothServerSocket.Call<AndroidJavaClass>("Accept", timeout));
#else
#endif

		return null;
	}

	public static void Close()
	{
		SetBluetoothServerSocket();

		if (IsServerSocketNull())
			return;

#if UNITY_ANDROID
		bluetoothServerSocket.Call("Close");
#else
#endif
	}

	private static void SetBluetoothServerSocket()
	{
#if UNITY_ANDROID
		if (bluetoothServerSocket != null)
			return;

		bluetoothServerSocket = new AndroidJavaClass(bluetoothServerSocketDLL); 
#else
#endif
	}

	private static bool IsServerSocketNull()
	{
#if UNITY_ANDROID
		return bluetoothServerSocket == null;
#else
		return true;
#endif
	}
}
