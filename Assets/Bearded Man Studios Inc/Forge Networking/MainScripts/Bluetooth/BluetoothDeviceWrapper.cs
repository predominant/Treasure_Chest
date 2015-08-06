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

public class BluetoothDeviceWrapper
{
#if UNITY_ANDROID
	private AndroidJavaObject bluetoothDevice;
#elif UNITY_IPHONE
#else
#endif

#if UNITY_ANDROID
	public BluetoothDeviceWrapper(AndroidJavaObject bluetoothDevice)
	{
		this.bluetoothDevice = bluetoothDevice;
	}
#endif

	public bool CreateBond()
	{
#if UNITY_ANDROID
		return bluetoothDevice.Call<bool>("createBond");
#else
		return false;
#endif
	}

	public int DescribeContents()
	{
#if UNITY_ANDROID
		return bluetoothDevice.Call<int>("describeContents");
#else
		return -1;
#endif
	}

	public bool FetchUuidsWithSdp()
	{
#if UNITY_ANDROID
		return bluetoothDevice.Call<bool>("FetchUuidsWithSdp");
#else
		return false;
#endif
	}

	public string GetAddress()
	{
#if UNITY_ANDROID
		return bluetoothDevice.Call<string>("getAddress");
#else
		return string.Empty;
#endif
	}

	public int GetBondState()
	{
#if UNITY_ANDROID
		return bluetoothDevice.Call<int>("getBondState");
#else
		return -1;
#endif
	}

	public string GetName()
	{
#if UNITY_ANDROID
		return bluetoothDevice.Call<string>("getName");
#else
		return string.Empty;
#endif
	}

	new public int GetType()
	{
#if UNITY_ANDROID
		return bluetoothDevice.Call<int>("getType");
#else
		return -1;
#endif
	}

	public int HashCode()
	{
#if UNITY_ANDROID
		return bluetoothDevice.Call<int>("hashCode");
#else
		return -1;
#endif
	}

	public bool SetPairingConfirmation(bool confirm)
	{
#if UNITY_ANDROID
		return bluetoothDevice.Call<bool>("setPairingConfirmation", confirm);
#else
		return false;
#endif
	}

	public bool SetPin(byte[] pin)
	{
#if UNITY_ANDROID
		return bluetoothDevice.Call<bool>("setPin", pin);
#else
		return false;
#endif
	}

	public string toString()
	{
#if UNITY_ANDROID
		return bluetoothDevice.Call<string>("toString");
#else
		return string.Empty;
#endif
	}
}