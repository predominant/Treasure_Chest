/*
 * Copyright (C) 2012-2015 Soomla Inc. - All Rights Reserved
 *
 *   Unauthorized copying of this file, via any medium is strictly prohibited
 *   Proprietary and confidential
 *
 *   Written by Refael Dakar <refael@soom.la>
 */
 
using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

namespace Soomla.Insights
{
	/// <summary>
	/// Represents a manager class which is in charge of fetching insights.
	/// </summary>
	public class SoomlaInsights {
		#if UNITY_IOS  && !UNITY_EDITOR
		[DllImport ("__Internal")]
		private static extern int soomlaInsights_initialize();
		[DllImport ("__Internal")]
		private static extern int soomlaInsights_refreshInsights();
		[DllImport ("__Internal")]
		private static extern int soomlaInsights_getUserInsights(out string outResult);
		#endif

		/// <summary>
		/// Initializes insights
		/// </summary>
		public static void Initialize() {
			SoomlaUtils.LogDebug (TAG, "SOOMLA/UNITY Initializing Insights");
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaClass jniSoomlaInsightsClass = new AndroidJavaClass("com.soomla.insights.SoomlaInsights")) {

				AndroidJavaObject jniSoomlaInsightsInstance = jniSoomlaInsightsClass.CallStatic<AndroidJavaObject>("getInstance");
				jniSoomlaInsightsInstance.Call("initialize");
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
			#elif UNITY_IOS && !UNITY_EDITOR
			soomlaInsights_initialize();
			#endif
		}

		/// <summary>
		/// Refreshing SoomlaInsights with SOOMLA GROW
		/// </summary>
		public static void RefreshInsights() {
			SoomlaUtils.LogDebug (TAG, "SOOMLA/UNITY Refreshing Insights");
			
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaClass jniSoomlaInsightsClass = new AndroidJavaClass("com.soomla.insights.SoomlaInsights")) {

				AndroidJavaObject jniSoomlaInsightsInstance = jniSoomlaInsightsClass.CallStatic<AndroidJavaObject>("getInstance");
				jniSoomlaInsightsInstance.Call("refreshInsights");
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);

			#elif UNITY_IOS && !UNITY_EDITOR
			soomlaInsights_refreshInsights();
			#endif
		}

		public static void I_SyncWithNative() {
			#if !UNITY_EDITOR
			JSONObject json = null;
			#if UNITY_ANDROID
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaClass jniSoomlaInsightsUnity = new AndroidJavaClass("com.soomla.highway.unity.SoomlaInsights")) {
				string userInsightsJSON = jniSoomlaInsightsUnity.CallStatic<string>("getUserInsights");
				json = new JSONObject(userInsightsJSON);
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
			
			#elif UNITY_IOS
			string userInsightsJSON = "{}";
			soomlaInsights_getUserInsights(out userInsightsJSON);
			json = new JSONObject(userInsightsJSON);
			#endif
			if (json != null) {
				UserInsights = new UserInsights(json);
			}
			#endif
		}

		public static UserInsights UserInsights = null;

		private const string TAG = "SOOMLA SoomlaInsights";
	}
}
