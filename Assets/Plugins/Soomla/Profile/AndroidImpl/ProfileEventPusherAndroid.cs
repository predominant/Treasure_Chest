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
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Soomla.Profile {

	//TODO: add invite push
	public class ProfileEventPusherAndroid : Soomla.Profile.ProfileEvents.ProfileEventPusher {

#if UNITY_ANDROID && !UNITY_EDITOR

		// event pushing back to native (when using FB Unity SDK)
		protected override void _pushEventLoginStarted(Provider provider, string payload) {
			if (SoomlaProfile.IsProviderNativelyImplemented(provider)) return;
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaClass jniSoomlaProfile = new AndroidJavaClass("com.soomla.profile.unity.ProfileEventHandler")) {
				ProfileJNIHandler.CallStaticVoid(jniSoomlaProfile, "pushEventLoginStarted", provider.ToString(), payload);
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}
		protected override void _pushEventLoginFinished(UserProfile userProfile, string payload) { 
			if (SoomlaProfile.IsProviderNativelyImplemented(userProfile.Provider)) return;
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaClass jniSoomlaProfile = new AndroidJavaClass("com.soomla.profile.unity.ProfileEventHandler")) {
				ProfileJNIHandler.CallStaticVoid(jniSoomlaProfile, "pushEventLoginFinished", userProfile.toJSONObject().print(), payload);
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}
		protected override void _pushEventLoginFailed(Provider provider, string message, string payload) {
			if (SoomlaProfile.IsProviderNativelyImplemented(provider)) return;
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaClass jniSoomlaProfile = new AndroidJavaClass("com.soomla.profile.unity.ProfileEventHandler")) {
				ProfileJNIHandler.CallStaticVoid(jniSoomlaProfile, "pushEventLoginFailed", provider.ToString(), message, payload);
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}
		protected override void _pushEventLoginCancelled(Provider provider, string payload) { 
			if (SoomlaProfile.IsProviderNativelyImplemented(provider)) return;
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaClass jniSoomlaProfile = new AndroidJavaClass("com.soomla.profile.unity.ProfileEventHandler")) {
				ProfileJNIHandler.CallStaticVoid(jniSoomlaProfile, "pushEventLoginCancelled", provider.ToString(), payload);
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}
		protected override void _pushEventLogoutStarted(Provider provider) { 
			if (SoomlaProfile.IsProviderNativelyImplemented(provider)) return;
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaClass jniSoomlaProfile = new AndroidJavaClass("com.soomla.profile.unity.ProfileEventHandler")) {
				ProfileJNIHandler.CallStaticVoid(jniSoomlaProfile, "pushEventLogoutStarted", provider.ToString());
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}
		protected override void _pushEventLogoutFinished(Provider provider) {
			if (SoomlaProfile.IsProviderNativelyImplemented(provider)) return;
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaClass jniSoomlaProfile = new AndroidJavaClass("com.soomla.profile.unity.ProfileEventHandler")) {
				ProfileJNIHandler.CallStaticVoid(jniSoomlaProfile, "pushEventLogoutFinished", provider.ToString());
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}
		protected override void _pushEventLogoutFailed(Provider provider, string message) {
			if (SoomlaProfile.IsProviderNativelyImplemented(provider)) return;
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaClass jniSoomlaProfile = new AndroidJavaClass("com.soomla.profile.unity.ProfileEventHandler")) {
				ProfileJNIHandler.CallStaticVoid(jniSoomlaProfile, "pushEventLogoutFailed",
				                                 provider.ToString(), message);
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}
		protected override void _pushEventSocialActionStarted(Provider provider, SocialActionType actionType, string payload) { 
			if (SoomlaProfile.IsProviderNativelyImplemented(provider)) return;
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaClass jniSoomlaProfile = new AndroidJavaClass("com.soomla.profile.unity.ProfileEventHandler")) {
				ProfileJNIHandler.CallStaticVoid(jniSoomlaProfile, "pushEventSocialActionStarted",
				                                 provider.ToString(), actionType.ToString(), payload);
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}
		protected override void _pushEventSocialActionFinished(Provider provider, SocialActionType actionType, string payload) {
			if (SoomlaProfile.IsProviderNativelyImplemented(provider)) return;
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaClass jniSoomlaProfile = new AndroidJavaClass("com.soomla.profile.unity.ProfileEventHandler")) {
				ProfileJNIHandler.CallStaticVoid(jniSoomlaProfile, "pushEventSocialActionFinished",
				                                 provider.ToString(), actionType.ToString(), payload);
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}
		protected override void _pushEventSocialActionCancelled(Provider provider, SocialActionType actionType, string payload) {
			if (SoomlaProfile.IsProviderNativelyImplemented(provider)) return;
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaClass jniSoomlaProfile = new AndroidJavaClass("com.soomla.profile.unity.ProfileEventHandler")) {
				ProfileJNIHandler.CallStaticVoid(jniSoomlaProfile, "pushEventSocialActionCancelled",
				                                 provider.ToString(), actionType.ToString(), payload);
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}
		protected override void _pushEventSocialActionFailed(Provider provider, SocialActionType actionType, string message, string payload) { 
			if (SoomlaProfile.IsProviderNativelyImplemented(provider)) return;
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaClass jniSoomlaProfile = new AndroidJavaClass("com.soomla.profile.unity.ProfileEventHandler")) {
				ProfileJNIHandler.CallStaticVoid(jniSoomlaProfile, "pushEventSocialActionFailed", 
				                                 provider.ToString(), actionType.ToString(), message, payload);
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}

		protected override void _pushEventGetContactsStarted (Provider provider, bool fromStart, string payload) {
			if (SoomlaProfile.IsProviderNativelyImplemented(provider)) return;
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaClass jniSoomlaProfile = new AndroidJavaClass("com.soomla.profile.unity.ProfileEventHandler")) {
				ProfileJNIHandler.CallStaticVoid(jniSoomlaProfile, "pushEventGetContactsStarted", 
				                                 provider.ToString(), fromStart, payload);
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}

		protected override void _pushEventGetContactsFinished (Provider provider, SocialPageData<UserProfile> contactsPage, string payload) {
			if (SoomlaProfile.IsProviderNativelyImplemented(provider)) return;
			List<JSONObject> profiles = new List<JSONObject>();
			foreach (var profile in contactsPage.PageData) {
				profiles.Add(profile.toJSONObject());
			}
			JSONObject contacts = new JSONObject(profiles.ToArray());

			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaClass jniSoomlaProfile = new AndroidJavaClass("com.soomla.profile.unity.ProfileEventHandler")) {
				ProfileJNIHandler.CallStaticVoid(jniSoomlaProfile, "pushEventGetContactsFinished", 
				                                 provider.ToString(), contacts.ToString(), payload, contactsPage.HasMore);
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}

		protected override void _pushEventGetContactsFailed (Provider provider, string message, bool fromStart, string payload) {
			if (SoomlaProfile.IsProviderNativelyImplemented(provider)) return;
			AndroidJNI.PushLocalFrame(100);
			using(AndroidJavaClass jniSoomlaProfile = new AndroidJavaClass("com.soomla.profile.unity.ProfileEventHandler")) {
				ProfileJNIHandler.CallStaticVoid(jniSoomlaProfile, "pushEventGetContactsFailed", 
				                                 provider.ToString(), message, fromStart, payload);
			}
			AndroidJNI.PopLocalFrame(IntPtr.Zero);
		}

#endif
	}
}
