using UnityEngine;
using System.Collections.Generic;

namespace Soomla.Profile {

	/// <summary>
	/// This class represents a single page of pagable data
	/// received from the social provider
	/// </summary>
	public class SocialPageData<T> {
		/// <summary>
		/// All data items received for this page.
		/// </summary>
		public List<T> PageData;

		/// <summary>
		/// The page number
		/// </summary>
		public int PageNumber;

		/// <summary>
		/// Are there more pages of contacts?
		/// </summary>
		public bool HasMore;
	}
}