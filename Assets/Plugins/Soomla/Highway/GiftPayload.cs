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
	/// Represents a gift payload which provides information of actual gift value
	/// </summary>
	public class GiftPayload {
		/// <summary>
		/// The associated virtual item ID to give as a part of the gifting process
		/// </summary>
		public string AssociatedItemId;
		/// <summary>
		/// How much of associated virtual item should be given as a part of the gifting process
		/// </summary>
		public int ItemsAmount;

		public GiftPayload(JSONObject jsonObject) {
			if (jsonObject[ASSOCIATED_ITEM_ID]) {
				AssociatedItemId = jsonObject[ASSOCIATED_ITEM_ID].str;
			}
			else {
				AssociatedItemId = "";
			}
			
			if (jsonObject[ITEMS_AMOUNT]) {
				ItemsAmount = System.Convert.ToInt32(jsonObject[ITEMS_AMOUNT].n);
			}
			else {
				ItemsAmount = 0;
			}
		}

		private const string ASSOCIATED_ITEM_ID = "associatedItemId";
		private const string ITEMS_AMOUNT = "itemsAmount";
	}
}