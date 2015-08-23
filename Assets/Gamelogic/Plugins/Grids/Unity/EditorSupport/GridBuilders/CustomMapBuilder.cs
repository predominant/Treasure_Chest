using UnityEngine;

namespace Gamelogic.Grids
{
	/*
		@ingroup UnityEditorSupport
	*/
	/// <summary>
	/// Inherit from this class to implement custom map for an 
	/// grid builder.
	/// This class should return a map. To have the grid builder 
	/// used this map, you should set the Map to Custom in the
	/// editor, and attach this script to the same game object
	/// as the grid builder.
	/// </summary>
	[Version(1, 8)]
	public abstract class CustomMapBuilder : MonoBehaviour
	{
		/// <summary>
		/// Returns a custom map windowed map.
		/// </summary>
		/// <typeparam name="TPoint">The type of the point.</typeparam>
		/// <returns></returns>
		/// <remarks>
		/// Implementors: Check the type; if it is "your" type, return the map.
		/// Otherwise return null.
		/// </remarks>
		public virtual WindowedMap<TPoint> CreateWindowedMap<TPoint>()
			where TPoint : IGridPoint<TPoint>
		{
			return null;
		}
	}
}