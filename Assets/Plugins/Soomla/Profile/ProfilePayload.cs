/// Copyright (C) 2012-2014 Soomla Inc.
///
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
///
///      http://www.apache.org/licenses/LICENSE-2.0
///
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
using System;
using UnityEngine;
namespace Soomla.Profile
{
	/// <summary>
	/// This class represents the profile payload which is based on
	/// the user payload and additional information.
	/// </summary>
	internal static class ProfilePayload
	{
		public static JSONObject ToJSONObj(string userPayload, string rewardId = "")
		{
			JSONObject obj = new JSONObject(JSONObject.Type.OBJECT);
			obj.AddField(USER_PAYLOAD, userPayload);
			obj.AddField(REWARD_ID, rewardId);
			return obj;
		}

		public static string GetUserPayload(JSONObject profilePayloadJson)
		{
			return profilePayloadJson != null && profilePayloadJson[USER_PAYLOAD] != null ? profilePayloadJson[USER_PAYLOAD].str : "";
		}

		public static string GetRewardId(JSONObject profilePayloadJson)
		{
			return profilePayloadJson != null && profilePayloadJson[REWARD_ID] != null ? profilePayloadJson[REWARD_ID].str : "";
		}
	
		private const string USER_PAYLOAD = "userPayload";
		private const string REWARD_ID = "rewardId";
	}
}

