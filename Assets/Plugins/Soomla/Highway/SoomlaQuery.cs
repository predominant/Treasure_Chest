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

namespace Soomla.Query
{
	/// <summary>
	/// Represents a manager class which is in charge of retrieving leaderboard
	/// information from the server
	/// </summary>
	public class SoomlaQuery {
		#if UNITY_IOS  && !UNITY_EDITOR
		[DllImport ("__Internal")]
		private static extern int soomlaQuery_queryFriendsStates(int providerId, string friendsListJson, out bool outResult);
		#endif

		/// <summary>
		/// Fetches the friends' state from the server.
		/// The friends' state contains relevant information on completed levels
		/// and highscores for the provided list of users.
		/// </summary>
		/// <returns><c>true</c>, if the operation has started, <c>false</c> if it cannot.</returns>
		/// <param name="providerId">The social provider ID for which to get the friends'
		/// state</param>
		/// <param name="friendsProfileIds">a List of friends' profile IDs in the social
		/// network provided</param>
		public static bool QueryFriendsStates(int providerId, IList<string> friendsProfileIds) {
			SoomlaUtils.LogDebug (TAG, "SOOMLA/UNITY Quering Friends States");

			JSONObject friendsProfileIdsJSON = new JSONObject(JSONObject.Type.ARRAY);
			foreach (string profileId in friendsProfileIds) {
				friendsProfileIdsJSON.Add(profileId);
			}
			bool result = false;

			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaClass jniSoomlaLeaderboardClass = new AndroidJavaClass("com.soomla.highway.unity.SoomlaQuery")) {

				result = jniSoomlaLeaderboardClass.CallStatic<bool>("unityQueryFriendsStates", providerId, friendsProfileIdsJSON.ToString());
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
			#elif UNITY_IOS && !UNITY_EDITOR
			soomlaQuery_queryFriendsStates(providerId, friendsProfileIdsJSON.ToString(), out result);
			#endif

			return result;
		}

		private const string TAG = "SOOMLA SoomlaQuery";
	}

}
