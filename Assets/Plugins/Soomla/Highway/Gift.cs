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

namespace Soomla.Gifting
{
	/// <summary>
	/// Represents a gift from one user to the other
	/// </summary>
	public class Gift {
		/// <summary>
		/// The Gift's ID, only when received from server
		/// </summary>
		public string ID;
		/// <summary>
		/// User's UID from which the gift originated
		/// </summary>
		public string FromUID;
		/// <summary>
		/// Social provider ID of the user to which the gift was sent
		/// </summary>
		public int ToProvider;
		/// <summary>
		/// The receiving user's ID on the provider social provider
		/// </summary>
		public string ToProfileId;
		/// <summary>
		/// Payload for the gift
		/// </summary>
		public GiftPayload Payload = null;
		
		public Gift(JSONObject jsonObject) {
			if (jsonObject[GIFT_ID]) {
				ID = jsonObject[GIFT_ID].str;
			}
			else {
				ID = "";
			}

			if (jsonObject[FROM_UID]) {
				FromUID = jsonObject[FROM_UID].str;
			}
			else {
				FromUID = "";
			}

			if (jsonObject[TO_PROVIDER]) {
				ToProvider = System.Convert.ToInt32(jsonObject[TO_PROVIDER].n);
			}
			else {
				ToProvider = 0;
			}

			if (jsonObject[TO_PROFILE_ID]) {
				ToProfileId = jsonObject[TO_PROFILE_ID].str;
			}
			else {
				ToProfileId = "";
			}

			if (jsonObject[PAYLOAD]) {
				Payload = new GiftPayload(jsonObject[PAYLOAD]);
			}
			else {
				Payload = null;
			}
		}

		private const string GIFT_ID = "giftId";
		private const string FROM_UID = "fromUid";
		private const string TO_PROVIDER = "toProvider";
		private const string TO_PROFILE_ID = "toProfileId";
		private const string PAYLOAD = "payload";
	}
}