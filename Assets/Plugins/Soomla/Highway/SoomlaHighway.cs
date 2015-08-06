/*
 * Copyright (C) 2012-2015 Soomla Inc. - All Rights Reserved
 *
 *   Unauthorized copying of this file, via any medium is strictly prohibited
 *   Proprietary and confidential
 *
 *   Written by Refael Dakar <refael@soom.la>
 */
 
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Soomla.Highway
{
	public class SoomlaHighway
	{
#if UNITY_IOS && !UNITY_EDITOR
		[DllImport ("__Internal")]
		private static extern int soomlaHighway_initialize(string gameKey, string envKey);
		[DllImport ("__Internal")]
		private static extern int soomlaHighway_setHighwayUrl(string url);
		[DllImport ("__Internal")]
		private static extern int soomlaHighway_setServicesUrl(string url);
		[DllImport ("__Internal")]
		private static extern int soomlaHighway_start();
#endif

		public static void Initialize() {
			SoomlaUtils.LogDebug (TAG, "SOOMLA/UNITY Initializing Highway");
#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaClass jniSoomlaHighwayClass = new AndroidJavaClass("com.soomla.highway.SoomlaHighway")) {

				AndroidJavaObject jniSoomlaHighwayInstance = jniSoomlaHighwayClass.CallStatic<AndroidJavaObject>("getInstance");
				jniSoomlaHighwayInstance.Call("initialize", HighwaySettings.HighwayGameKey, HighwaySettings.HighwayEnvKey);

				// Uncomment this and change the URL for testing
//				using(AndroidJavaClass jniConfigClass = new AndroidJavaClass("com.soomla.highway.HighwayConfig")) {
//					AndroidJavaObject jniConfigObject = jniConfigClass.CallStatic<AndroidJavaObject>("getInstance");
//					jniConfigObject.Call("setUrls", "http://example.com", "http://example.com");
//				}

				jniSoomlaHighwayInstance.Call("start");
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
#elif UNITY_IOS && !UNITY_EDITOR
			soomlaHighway_initialize(HighwaySettings.HighwayGameKey, HighwaySettings.HighwayEnvKey);

			// Uncomment this and change the URL for testing
//			soomlaHighway_setHighwayUrl("http://example.com");
//			soomlaHighway_setServicesUrl("http://example.com");

			soomlaHighway_start();
#else
			SoomlaUtils.LogError(TAG, "Highway only works on Android or iOS devices !");
			UnityEditor.EditorApplication.isPlaying = false;
#endif
		}

		/// <summary> Class Members </summary>

		private const string TAG = "SOOMLA SoomlaHighway";

	}
}
