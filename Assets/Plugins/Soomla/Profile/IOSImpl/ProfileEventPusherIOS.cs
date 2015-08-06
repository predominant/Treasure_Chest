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

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Soomla.Profile {

	//TODO: add invite push
	public class ProfileEventPusherIOS : Soomla.Profile.ProfileEvents.ProfileEventPusher {
#if UNITY_IOS && !UNITY_EDITOR

		// event pushing back to native (when using FB Unity SDK)
		[DllImport ("__Internal")]
		private static extern void soomlaProfile_PushEventLoginStarted(string provider, string payload);
		[DllImport ("__Internal")]
		private static extern void soomlaProfile_PushEventLoginFinished(string userProfileJson, string payload);
		[DllImport ("__Internal")]
		private static extern void soomlaProfile_PushEventLoginFailed(string provider, string message, string payload);
		[DllImport ("__Internal")]
		private static extern void soomlaProfile_PushEventLoginCancelled(string provider, string payload);
		[DllImport ("__Internal")]
		private static extern void soomlaProfile_PushEventLogoutStarted(string provider);
		[DllImport ("__Internal")]
		private static extern void soomlaProfile_PushEventLogoutFinished(string provider);
		[DllImport ("__Internal")]
		private static extern void soomlaProfile_PushEventLogoutFailed(string provider, string message);
		[DllImport ("__Internal")]
		private static extern void soomlaProfile_PushEventSocialActionStarted(string provider, string actionType, string payload);
		[DllImport ("__Internal")]
		private static extern void soomlaProfile_PushEventSocialActionFinished(string provider, string actionType, string payload);
		[DllImport ("__Internal")]
		private static extern void soomlaProfile_PushEventSocialActionCancelled(string provider, string actionType, string payload);
		[DllImport ("__Internal")]
		private static extern void soomlaProfile_PushEventSocialActionFailed(string provider, string actionType, string message, string payload);
		[DllImport ("__Internal")]
		private static extern void soomlaProfile_PushEventGetContactsStarted(string provider, bool fromStart, string payload);
		[DllImport ("__Internal")]
		private static extern void soomlaProfile_PushEventGetContactsFinished(string provider, string userProfilesJson, string payload, bool hasNext);
		[DllImport ("__Internal")]
		private static extern void soomlaProfile_PushEventGetContactsFailed(string provider, string message, bool fromStart, string payload);


		// event pushing back to native (when using FB Unity SDK)
		protected override void _pushEventLoginStarted(Provider provider, string payload) {
			if (SoomlaProfile.IsProviderNativelyImplemented(provider)) return;
			soomlaProfile_PushEventLoginStarted(provider.ToString(), payload);
		}

		protected override void _pushEventLoginFinished(UserProfile userProfile, string payload) { 
			if (SoomlaProfile.IsProviderNativelyImplemented(userProfile.Provider)) return;
			soomlaProfile_PushEventLoginFinished(userProfile.toJSONObject().print(), payload);
		}
		protected override void _pushEventLoginFailed(Provider provider, string message, string payload) {
			if (SoomlaProfile.IsProviderNativelyImplemented(provider)) return;
			soomlaProfile_PushEventLoginFailed(provider.ToString(), message, payload);
		}
		protected override void _pushEventLoginCancelled(Provider provider, string payload) { 
			if (SoomlaProfile.IsProviderNativelyImplemented(provider)) return;
			soomlaProfile_PushEventLoginCancelled(provider.ToString(), payload);
		}
		protected override void _pushEventLogoutStarted(Provider provider) { 
			if (SoomlaProfile.IsProviderNativelyImplemented(provider)) return;
			soomlaProfile_PushEventLogoutStarted(provider.ToString());
		}
		protected override void _pushEventLogoutFinished(Provider provider) { 
			if (SoomlaProfile.IsProviderNativelyImplemented(provider)) return;
			soomlaProfile_PushEventLogoutFinished(provider.ToString());
		}
		protected override void _pushEventLogoutFailed(Provider provider, string message) {
			if (SoomlaProfile.IsProviderNativelyImplemented(provider)) return;
			soomlaProfile_PushEventLogoutFailed(provider.ToString(), message);
		}
		protected override void _pushEventSocialActionStarted(Provider provider, SocialActionType actionType, string payload) { 
			if (SoomlaProfile.IsProviderNativelyImplemented(provider)) return;
			soomlaProfile_PushEventSocialActionStarted(provider.ToString(), actionType.ToString(), payload);
		}
		protected override void _pushEventSocialActionFinished(Provider provider, SocialActionType actionType, string payload) {
			if (SoomlaProfile.IsProviderNativelyImplemented(provider)) return;
			soomlaProfile_PushEventSocialActionFinished(provider.ToString(), actionType.ToString(), payload);
		}
		protected override void _pushEventSocialActionCancelled(Provider provider, SocialActionType actionType, string payload) {
			if (SoomlaProfile.IsProviderNativelyImplemented(provider)) return;
			soomlaProfile_PushEventSocialActionCancelled(provider.ToString(), actionType.ToString(), payload);
		}
		protected override void _pushEventSocialActionFailed(Provider provider, SocialActionType actionType, string message, string payload) { 
			if (SoomlaProfile.IsProviderNativelyImplemented(provider)) return;
			soomlaProfile_PushEventSocialActionFailed(provider.ToString(), actionType.ToString(), message, payload);
		}
		protected override void _pushEventGetContactsStarted (Provider provider, bool fromStart, string payload) {
			if (SoomlaProfile.IsProviderNativelyImplemented(provider)) return;
			soomlaProfile_PushEventGetContactsStarted(provider.ToString(), fromStart, payload);
		}
		protected override void _pushEventGetContactsFinished (Provider provider, SocialPageData<UserProfile> contactsPage, string payload) {
			if (SoomlaProfile.IsProviderNativelyImplemented(provider)) return;
			List<JSONObject> profiles = new List<JSONObject>();
			foreach (var profile in contactsPage.PageData) {
				profiles.Add(profile.toJSONObject());
			}
			JSONObject contacts = new JSONObject(profiles.ToArray());

			soomlaProfile_PushEventGetContactsFinished(provider.ToString(), contacts.ToString(), payload, contactsPage.HasMore);
		}
		protected override void _pushEventGetContactsFailed (Provider provider, string message, bool fromStart, string payload) {
			if (SoomlaProfile.IsProviderNativelyImplemented(provider)) return;
			soomlaProfile_PushEventGetContactsFailed(provider.ToString(), message, fromStart, payload);
		}
#endif
	}
}
