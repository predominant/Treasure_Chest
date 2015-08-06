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
using System.Runtime.InteropServices;
using System.Collections.Generic;

public class BluetoothAdapterWrapper
{

#if UNITY_ANDROID
	private const string bluetoothAdapterDLL = "com.UnityBluetoothBridge.Adapter"; //NO DLL for other platforms'
	private static AndroidJavaClass bluetoothAdapter;
#elif UNITY_IPHONE
	private const string bluetoothAdapterDLL = " "; //NO DLL for other platforms
#else
	private const string bluetoothAdapterDLL = " "; //NO DLL for other platforms
#endif

	public static bool StartDiscovery()
	{
		SetBluetoothAdapter();

		if (IsAdapterNull())
			return false;

#if UNITY_ANDROID
		return bluetoothAdapter.CallStatic<bool>("StartDiscovery");
#else
		return false;
#endif
	}

	public static bool CancelDiscovery()
	{
		SetBluetoothAdapter();

		if (IsAdapterNull())
			return false;

#if UNITY_ANDROID
		return bluetoothAdapter.CallStatic<bool>("CancelDiscovery");
#else
		return false;
#endif
	}

	public static bool CheckBluetoothAddress(string address)
	{
		SetBluetoothAdapter();

		if (IsAdapterNull())
			return false;

#if UNITY_ANDROID
		return bluetoothAdapter.CallStatic<bool>("CheckBluetoothAddress", address);
#else
		return false;
#endif
	}

	public static bool IsDiscovering()
	{
		SetBluetoothAdapter();

		if (IsAdapterNull())
			return false;

#if UNITY_ANDROID
		return bluetoothAdapter.CallStatic<bool>("IsDiscovering");
#else
		return false;
#endif
	}

	public static bool IsEnabled()
	{
		SetBluetoothAdapter();

		if (IsAdapterNull())
			return false;

#if UNITY_ANDROID
		return bluetoothAdapter.CallStatic<bool>("IsEnabled");
#else
		return false;
#endif
	}

	public static bool SetName(string name)
	{
		SetBluetoothAdapter();

		if (IsAdapterNull())
			return false;

#if UNITY_ANDROID
		return bluetoothAdapter.CallStatic<bool>("SetName", name);
#else
		return false;
#endif
	}

	public static bool Disable()
	{
		SetBluetoothAdapter();

		if (IsAdapterNull())
			return false;

#if UNITY_ANDROID
		return bluetoothAdapter.CallStatic<bool>("Disable");
#else
		return false;
#endif
	}

	public static bool Enable()
	{
		SetBluetoothAdapter();

		if (IsAdapterNull())
			return false;

#if UNITY_ANDROID
		return bluetoothAdapter.CallStatic<bool>("Enable");
#else
		return false;
#endif
	}

	public static string GetAddress()
	{
		SetBluetoothAdapter();

		if (IsAdapterNull())
			return string.Empty;

#if UNITY_ANDROID
		return bluetoothAdapter.CallStatic<string>("GetAddress");
#else
		return string.Empty;
#endif
	}

	public static BluetoothDeviceWrapper[] GetBondedDevices()
	{
		SetBluetoothAdapter();

		if (IsAdapterNull())
			return null;

		List<BluetoothDeviceWrapper> returnedDevices = new List<BluetoothDeviceWrapper>();
#if UNITY_ANDROID
		AndroidJavaObject[] devices = bluetoothAdapter.CallStatic<AndroidJavaObject[]>("GetBondedDevices");

		foreach (AndroidJavaObject jObj in devices)
		{
			returnedDevices.Add(new BluetoothDeviceWrapper(jObj));
		}
#endif
		return returnedDevices.ToArray();
	}

	public static string GetName()
	{
		SetBluetoothAdapter();

		if (IsAdapterNull())
			return string.Empty;

#if UNITY_ANDROID
		return bluetoothAdapter.CallStatic<string>("GetName");
#else
		return string.Empty;
#endif
	}

	public static int GetProfileConnectionState(int profile)
	{
		SetBluetoothAdapter();

		if (IsAdapterNull())
			return -1;

#if UNITY_ANDROID
		return bluetoothAdapter.CallStatic<int>("GetProfileConnectionState", profile);
#else
		return -1;
#endif
	}

	public static BluetoothDeviceWrapper GetRemoteDevice(string address)
	{
		SetBluetoothAdapter();

		if (IsAdapterNull())
			return null;

#if UNITY_ANDROID
		return new BluetoothDeviceWrapper(bluetoothAdapter.CallStatic<AndroidJavaObject>("GetProfileConnectionState", address));
#else
		return null;
#endif
	}

	public static int GetScanMode()
	{
		SetBluetoothAdapter();

		if (IsAdapterNull())
			return -1;

#if UNITY_ANDROID
		return bluetoothAdapter.CallStatic<int>("GetScanMode");
#else
		return -1;
#endif
	}

	public static int GetState()
	{
		SetBluetoothAdapter();

		if (IsAdapterNull())
			return -1;

#if UNITY_ANDROID
		return bluetoothAdapter.CallStatic<int>("GetState");
#else
		return -1;
#endif
	}

	private static void SetBluetoothAdapter()
	{
#if UNITY_ANDROID
		if (bluetoothAdapter != null)
			return;

		bluetoothAdapter = new AndroidJavaClass(bluetoothAdapterDLL); 
#else
#endif
	}

	private static bool IsAdapterNull()
	{
#if UNITY_ANDROID
		return bluetoothAdapter == null;
#else
		return true;
#endif
	}
}