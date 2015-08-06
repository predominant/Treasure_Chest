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

namespace Soomla.Query
{
	/// <summary>
	/// Represents a friend's state in the game.
	/// It contains all relevant information to create a leaderboard between a player
	/// and his friends.
	/// </summary>
	public class FriendState {
		/// <summary>
		/// The profile ID of the user in the social network
		/// </summary>
		public string ProfileId;
		/// <summary>
		/// a Map of worlds having levels completed in them by the user.
		/// It maps between the world ID and a completed inner world ID.
		/// </summary>
		public Dictionary<string, string> LastCompletedWorlds = new Dictionary<string, string>();
		/// <summary>
		/// a Map of records made by the user.
		/// It maps between score ID and the highest record done by the user.
		/// </summary>
		public Dictionary<string, double> Records = new Dictionary<string, double>();

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="jsonObject">a Friend state for a user, represented by a JSON</param>
		public FriendState(JSONObject jsonObject) {
			if (jsonObject[USER_PROFILE_ID]) {
				ProfileId = jsonObject[USER_PROFILE_ID].str;
			}
			else {
				ProfileId = "";
			}
			
			if (jsonObject[USER_LAST_COMPLETED_WORLDS]) {
				JSONObject lastCompletedWorldIdsJSON = jsonObject[USER_LAST_COMPLETED_WORLDS];
				foreach (string worldId in lastCompletedWorldIdsJSON.keys) {
					LastCompletedWorlds.Add(worldId, lastCompletedWorldIdsJSON[worldId].str);
				}
			}

			if (jsonObject[USER_RECORDS]) {
				JSONObject recordsJSON = jsonObject[USER_RECORDS];
				foreach (string scoreId in recordsJSON.keys) {
					Records.Add(scoreId, recordsJSON[scoreId].n);
				}
			}
		}

		private const string USER_PROFILE_ID = "profileId";
		private const string USER_LAST_COMPLETED_WORLDS = "lastCompletedWorlds";
		private const string USER_RECORDS = "records";
	}
}
