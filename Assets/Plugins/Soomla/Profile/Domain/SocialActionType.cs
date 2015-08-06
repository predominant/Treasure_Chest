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
/// 
using System;
namespace Soomla.Profile
{
	/// <summary>
	/// A string enumeration of social actions supported by social providers.
	/// </summary>
	public sealed class SocialActionType
	{
		private readonly string name;
		
		public static readonly SocialActionType UPDATE_STATUS = new SocialActionType ("UPDATE_STATUS");
		public static readonly SocialActionType UPDATE_STORY = new SocialActionType ("UPDATE_STORY");
		public static readonly SocialActionType UPLOAD_IMAGE = new SocialActionType ("UPLOAD_IMAGE");
		public static readonly SocialActionType GET_CONTACTS = new SocialActionType ("GET_CONTACTS");
		public static readonly SocialActionType GET_FEED = new SocialActionType ("GET_FEED");

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the social action.</param>
		private SocialActionType(string name){
			this.name = name;
		}

		/// <summary>
		/// Converts this social action into a string. 
		/// </summary>
		/// <returns>A string representation of the current <c>SocialActionType</c>.</returns>
		public override string ToString(){
			return name;
		}

		/// <summary>
		/// Converts the given string into a <c>SocialActionType</c>
		/// </summary>
		/// <returns>The string.</returns>
		/// <param name="actionTypeStr">The string to convert into a <c>SocialActionType</c>.</param>
		public static SocialActionType fromString(string actionTypeStr) {
			switch(actionTypeStr) {
			case("UPDATE_STATUS"):
				return UPDATE_STATUS;
			case("UPDATE_STORY"):
				return UPDATE_STORY;
			case("UPLOAD_IMAGE"):
				return UPLOAD_IMAGE;
			case("GET_CONTACTS"):
				return GET_CONTACTS;
			case("GET_FEED"):
				return GET_FEED;
			default:
				return null;
			}
		}

		/// <summary>
		/// Converts the given string into a <c>SocialActionType</c>
		/// </summary>
		/// <returns>The string.</returns>
		/// <param name="actionTypeInt">The string to convert into a <c>SocialActionType</c>.</param>
		public static SocialActionType fromInt(int actionTypeInt) {
			switch(actionTypeInt) {
			case 0:
				return UPDATE_STATUS;
			case 1:
				return UPDATE_STORY;
			case 2:
				return UPLOAD_IMAGE;
			case 3:
				return GET_CONTACTS;
			case 4:
				return GET_FEED;
			default:
				return null;
			}
		}
	}
}

