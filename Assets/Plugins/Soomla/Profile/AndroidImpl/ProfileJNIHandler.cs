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

namespace Soomla.Profile {

	/// <summary>
	/// This class uses JNI and provides functions that call SOOMLA's android-profile.
	/// </summary>
	public static class ProfileJNIHandler {

#if UNITY_ANDROID

		/// <summary>
		/// Calls android-profile static function that returns void and receives a params arguments. 
		/// </summary>
		public static void CallStaticVoid(AndroidJavaClass jniObject, string method, params object[] args) {
			if(!Application.isEditor){
				jniObject.CallStatic(method, args);

				checkExceptions();
			}
		}

		/// <summary>
		/// Calls android-profile static function that has a return value and receives a string argument. 
		/// </summary>
		/// <param name="jniObject">A type-less instance of any Java class.</param>
		/// <param name="method">The method to call in android-profile.</param>
		/// <param name="args">The method's arguments.</param>
		/// <returns>Return value of the function called.</returns>
		public static T CallStatic<T>(AndroidJavaClass jniObject, string method, params object[] args) {
			if (!Application.isEditor) {
				T retVal = jniObject.CallStatic<T>(method, args);

				checkExceptions();

				return retVal;
			}
			
			return default(T);
		}

		/// <summary>
		/// Throws one of the exceptions (<c>UserProfileNotFoundException</c> or <c>ProviderNotFoundException</c> if needed. 
		/// </summary>
		public static void checkExceptions ()
		{
			IntPtr jException = AndroidJNI.ExceptionOccurred();
			if (jException != IntPtr.Zero) {
				AndroidJNI.ExceptionClear();
				
				AndroidJavaClass jniExceptionClass = new AndroidJavaClass("com.soomla.profile.exceptions.UserProfileNotFoundException");
				if (AndroidJNI.IsInstanceOf(jException, jniExceptionClass.GetRawClass())) {
					Debug.Log("SOOMLA/UNITY Caught UserProfileNotFoundException!");
					
					throw new UserProfileNotFoundException();
				}
				
				jniExceptionClass.Dispose();
				jniExceptionClass = new AndroidJavaClass("com.soomla.profile.exceptions.ProviderNotFoundException");
				if (AndroidJNI.IsInstanceOf(jException, jniExceptionClass.GetRawClass())) {
					Debug.Log("SOOMLA/UNITY Caught ProviderNotFoundException!");
					
					throw new ProviderNotFoundException();
				}

				jniExceptionClass.Dispose();
				
				Debug.Log("SOOMLA/UNITY Got an exception but can't identify it!");
			}
		}
#endif
	}
}

