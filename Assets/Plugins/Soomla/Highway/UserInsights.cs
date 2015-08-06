/*
 * Copyright (C) 2012-2015 Soomla Inc. - All Rights Reserved
 *
 *   Unauthorized copying of this file, via any medium is strictly prohibited
 *   Proprietary and confidential
 *
 *   Written by Refael Dakar <refael@soom.la>
 */
 
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Soomla.Insights
{
	/// <summary>
	/// Represents insights for a user
	/// </summary>
	public class UserInsights {

		public UserInsights(JSONObject userInsightsJSON) {
			JSONObject payInsightsJSON = null;
			try {
				payInsightsJSON = userInsightsJSON["pay"];
			} catch (Exception e) {
				SoomlaUtils.LogError(TAG, "An error occurred while trying to fetch user pay insights when generating. error: " + e.Message);
			}
			
			PayInsights = new PayInsights(payInsightsJSON);
		}
		
		public PayInsights PayInsights;

		private const string TAG = "SOOMLA UserInsights";

	}

	public class PayInsights
	{
		public Dictionary<Genre, int> PayRankByGenre;
		
		public PayInsights(JSONObject payInsightsJSON) {
			PayRankByGenre = new Dictionary<Genre, int>();
			
			if (payInsightsJSON == null || payInsightsJSON.keys.Count==0) {
				return;
			}
			
			try {
				JSONObject payRankByGenre = payInsightsJSON["payRankByGenre"];
				
				foreach(string key in payRankByGenre.keys) {
					try {
						PayRankByGenre[(Genre)Convert.ToInt32(key)] = (int)payRankByGenre[key].n;
					} catch (Exception e) {
						SoomlaUtils.LogError(TAG, "An error occurred when trying to generate PayInsights for key: " + key + " error: " + e.Message);
					}
				}
			} catch (Exception e) {
				SoomlaUtils.LogError(TAG, "An error occurred when trying to generate PayInsights. couldn't get payRankByGenre. error: " + e.Message);
			}
		}
		
		private const string TAG = "SOOMLA PayInsights";
	}
}