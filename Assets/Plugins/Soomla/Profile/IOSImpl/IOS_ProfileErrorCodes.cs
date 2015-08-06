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

namespace Soomla.Profile {

	/// <summary>
	/// This class provides error codes for each of the errors available in iOS-profile. 
	/// </summary>
	public static class IOS_ProfileErrorCodes {
		public static int NO_ERROR = 0;
		public static int EXCEPTION_PROVIDER_NOT_FOUND = -301;
		public static int EXCEPTION_USER_PROFILE_NOT_FOUND = -302;

		/// <summary>
		/// Checks the error code and throws the relevant exception.
		/// </summary>
		/// <param name="error">Error code.</param>
		public static void CheckAndThrowException(int error) {
			if (error == EXCEPTION_PROVIDER_NOT_FOUND) {
				Debug.Log("SOOMLA/UNITY Got ProviderNotFoundException exception from 'extern C'");
				throw new ProviderNotFoundException();
			} 
			
			if (error == EXCEPTION_USER_PROFILE_NOT_FOUND) {
				Debug.Log("SOOMLA/UNITY Got UserProfileNotFoundException exception from 'extern C'");
				throw new UserProfileNotFoundException();
			} 
		}
	}
}

