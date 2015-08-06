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
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;

namespace Soomla.Dlc
{
	/// <summary>
	/// This class provides DLC functionality to the highway package.
	/// This mainly means contacting the server and synchronizing asset packages
	/// defined by the game developer (including downloading and deleting).
	/// </summary>
	public class SoomlaDLC {

		#if UNITY_IOS  && !UNITY_EDITOR
		[DllImport ("__Internal")]
		private static extern int soomlaDlc_initialize();
		[DllImport ("__Internal")]
		private static extern int soomlaDlc_checkSyncedPackagesStatus();
		[DllImport ("__Internal")]
		private static extern int soomlaDlc_checkPackageStatus(string packageId);
		[DllImport ("__Internal")]
		private static extern int soomlaDlc_startSync(string packageId, out bool outResult);
		[DllImport ("__Internal")]
		private static extern int soomlaDlc_getFilePath(string packageId, string fileName, out IntPtr filePathP);
		[DllImport ("__Internal")]
		private static extern int soomlaDlc_getFilesPathsInPackage(string packageId, out IntPtr json);
		#endif

		/// <summary>
		/// Initializes the DLC service, this method will check for updates on
		/// all the synced packages on the device and return a state via
		/// the <c>OnDLCPackagesStatusUpdate</c>
		/// </summary>
		public static void Initialize() {
			SoomlaUtils.LogDebug (TAG, "SOOMLA/UNITY Initializing DLC");
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaClass jniSoomlaDLCClass = new AndroidJavaClass("com.soomla.dlc.SoomlaDLC")) {

				AndroidJavaObject jniSoomlaDLCInstance = jniSoomlaDLCClass.CallStatic<AndroidJavaObject>("getInstance");
				jniSoomlaDLCInstance.Call("initialize");
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
			#elif UNITY_IOS && !UNITY_EDITOR
			soomlaDlc_initialize();
			#endif
		}

		/// <summary>
		/// Checks all synced packages' status against the server, the result will
		/// be returned via <c>OnDLCPackagesStatusUpdate</c> event.
		/// Deleted packages from the server will be deleted from the device.
		/// </summary>
		public static void CheckSyncedPackagesStatus() {
			SoomlaUtils.LogDebug (TAG, "SOOMLA/UNITY Checking synced packages status");
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaClass jniSoomlaDLCClass = new AndroidJavaClass("com.soomla.dlc.SoomlaDLC")) {

				AndroidJavaObject jniSoomlaDLCInstance = jniSoomlaDLCClass.CallStatic<AndroidJavaObject>("getInstance");
				jniSoomlaDLCInstance.Call("checkSyncedPackagesStatus");
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
			#elif UNITY_IOS && !UNITY_EDITOR
			soomlaDlc_checkSyncedPackagesStatus();
			#endif
		}

		/// <summary>
		/// Checks the status of a single package, the result will be returned via
		/// <c>OnDLCPackagesStatusUpdate</c> event
		/// </summary>
		/// <param name="packageId">The ID of a defined package for the game to check.</param>
		public static void CheckPackageStatus(string packageId) {
			SoomlaUtils.LogDebug (TAG, "SOOMLA/UNITY Checking package status " + packageId);
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaClass jniSoomlaDLCClass = new AndroidJavaClass("com.soomla.dlc.SoomlaDLC")) {
				AndroidJavaObject jniSoomlaDLCInstance = jniSoomlaDLCClass.CallStatic<AndroidJavaObject>("getInstance");
				jniSoomlaDLCInstance.Call("checkPackageStatus", packageId);
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
			#elif UNITY_IOS && !UNITY_EDITOR
			soomlaDlc_checkPackageStatus(packageId);
			#endif
		}

		/// <summary>
		/// Starts synchronizing the provided DLC package (only one sync at a time).
		/// </summary>
		/// <returns><c>true</c>, if the syncing process has started, <c>false</c> otherwise.</returns>
		/// <param name="packageId">The ID of a defined package for the game.</param>
		public static bool StartSync(string packageId) {
			SoomlaUtils.LogDebug (TAG, "SOOMLA/UNITY Starting DLC sync for package " + packageId);
			bool result = false;
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaClass jniSoomlaDLCClass = new AndroidJavaClass("com.soomla.dlc.SoomlaDLC")) {

				AndroidJavaObject jniSoomlaDLCInstance = jniSoomlaDLCClass.CallStatic<AndroidJavaObject>("getInstance");
				result = jniSoomlaDLCInstance.Call<bool>("startSync", packageId);
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
			#elif UNITY_IOS && !UNITY_EDITOR
			soomlaDlc_startSync(packageId, out result);
			#endif
			return result;
		}

		/// <summary>
		/// Get a full path to a file in a package
		/// </summary>
		/// <returns>a Full path for the supplied filename in a package, which might
		/// not exist on the device itself.</returns>
		/// <param name="packageId">The ID of a defined package for the game.</param>
		/// <param name="fileName">The filename which may or may not exist in the package.</param>
		public static string GetFilePath(string packageId, string fileName) {
			SoomlaUtils.LogDebug (TAG, "SOOMLA/UNITY Getting full file path for " + fileName + " in package " + packageId);
			string result = null;
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaClass jniSoomlaDLCClass = new AndroidJavaClass("com.soomla.dlc.SoomlaDLC")) {

				AndroidJavaObject jniSoomlaDLCInstance = jniSoomlaDLCClass.CallStatic<AndroidJavaObject>("getInstance");
				result = jniSoomlaDLCInstance.Call<string>("getPathToFile", packageId, fileName);
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
			#elif UNITY_IOS && !UNITY_EDITOR
			IntPtr p = IntPtr.Zero;
			soomlaDlc_getFilePath(packageId, fileName, out p);

			result = Marshal.PtrToStringAnsi(p);
			Marshal.FreeHGlobal(p);
			SoomlaUtils.LogDebug(TAG, "Got file-path: " + result);
			#endif

			return result;
		}

		/// <summary>
		/// Gets a list of full paths to files in a package.
		/// </summary>
		/// <returns>Ta List of full paths to files currently in the package.</returns>
		/// <param name="packageId">The ID of a defined package for the game.</param>
		public static IList<string> GetFilesPathsInPackage(string packageId) {
			SoomlaUtils.LogDebug (TAG, "SOOMLA/UNITY Getting full file paths for package " + packageId);
			string result = null;
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJNI.PushLocalFrame(100);
			// NOTE: this is in the bridge!
			using(AndroidJavaClass jniSoomlaDLCClass = new AndroidJavaClass("com.soomla.highway.unity.SoomlaDLC")) {

				result = jniSoomlaDLCClass.CallStatic<string>("unityGetFilesPathsInPackage", packageId);
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
			#elif UNITY_IOS && !UNITY_EDITOR
			IntPtr p = IntPtr.Zero;
			soomlaDlc_getFilesPathsInPackage(packageId, out p);

			result = Marshal.PtrToStringAnsi(p);
			Marshal.FreeHGlobal(p);
			SoomlaUtils.LogDebug(TAG, "Got file-paths JSON: " + result);
			#endif

			List<string> filePaths = new List<string>();
			if (result != null) {
				JSONObject filePathsJSON = new JSONObject(result);
				foreach (var filePathJSON in filePathsJSON.list) {
					filePaths.Add(filePathJSON.str);
				}
			}

			return filePaths;
		}

		private const string TAG = "SOOMLA SoomlaDLC";
	}
}
