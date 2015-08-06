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
namespace Soomla.Profile
{
	/// <summary>
	/// A string enumeration of available social providers. Currently, the only Provider available 
	/// with SOOMLA is Facebook, but in the future more providers will be supported. 
	/// </summary>
	public sealed class Provider
	{
		private readonly string name;

		public static readonly Provider FACEBOOK = new Provider ("facebook");
		public static readonly Provider GOOGLE = new Provider ("google");
		public static readonly Provider TWITTER = new Provider ("twitter");

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the social provider.</param>
		private Provider(string name){
			this.name = name;
		}

		//// <summary>
		/// Converts this provider into a string. 
		/// </summary>
		/// <returns>A string representation of the current <c>Provider</c>.</returns>
		public override string ToString(){
			return name;
		}

		//// <summary>
		/// Converts this provider into an int. 
		/// </summary>
		/// <returns>A int representation of the current <c>Provider</c>.</returns>
		public int toInt(){
			switch(name) {
			case("facebook"):
				return 0;
			case("google"):
				return 2;
			case("twitter"):
				return 5;
			default:
				return -1;
			}
		}

		/// <summary>
		/// Converts the given string into a <c>Provider</c>
		/// </summary>
		/// <returns>The string.</returns>
		/// <param name="providerTypeStr">The string to convert into a <c>Provider</c>.</param>
		public static Provider fromString(string providerTypeStr) {
			switch(providerTypeStr) {
			case("facebook"):
				return FACEBOOK;
			case("google"):
				return GOOGLE;
			case("twitter"):
				return TWITTER;
			default:
				return null;
			}
		}

		/// <summary>
		/// Converts the given int into a <c>Provider</c>
		/// </summary>
		/// <returns>The int.</returns>
		/// <param name="providerTypeInt">The string to convert into a <c>Provider</c>.</param>
		public static Provider fromInt(int providerTypeInt) {
			switch(providerTypeInt) {
			case 0:
				return FACEBOOK;
			case 2:
				return GOOGLE;
			case 5:
				return TWITTER;
			default:
				return null;
			}
		}
	}
}

