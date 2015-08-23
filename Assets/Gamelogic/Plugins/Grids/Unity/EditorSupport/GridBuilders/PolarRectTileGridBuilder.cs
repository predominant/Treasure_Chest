using System;
using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		Class for building a polar rect grid in the Unity editor. 

		This component should (generally) not be accessed directly. Instead, add your 
		own component that inherits from GridBebaviour, and access the grid and map
		through there.

		@version1_8

		@ingroup UnityComponents
	*/
	[AddComponentMenu("Gamelogic/GridBuilders/Tile Grids/Polar Rect Grid Builder")]
	public class PolarRectTileGridBuilder : PolarTileGridBuilder<RectPoint>
	{
		#region Types

		[Serializable]
		public enum Shape
		{
			Rectangle,
		}

		[Serializable]
		public enum MapType
		{
			Rect
		}

		#endregion

		#region Fields
		[SerializeField]
		[Tooltip("The shape that the grid will be built in.")] 
		private Shape shape = Shape.Rectangle;

		[SerializeField]
		[Tooltip("The map to use with your grid.")] 
		private MapType mapType = MapType.Rect;

		#endregion

		#region Properties

		/**
			Returns the grid builder attached to the same game object as this
			grid behaviour.

			(It's provided, but you will mostly need only the Grid and Map.)
		*/
		/*public TileGridBuilder<RectPoint> GridBuilder
		{
			get { return GetComponent<TileGridBuilder<RectPoint>>(); }
		}*/

		public new WrappedGrid<TileCell, RectPoint> Grid
		{
			get { return (WrappedGrid<TileCell, RectPoint>) grid; }
		}

		public new IMap3D<RectPoint> Map
		{
			get { return base.Map; }
		}

		public Shape GridShape
		{
			get { return shape; }
		}

		#endregion

		#region Implementation

		protected override void InitGrid()
		{
			int width = Dimensions.X;
			int height = Dimensions.Y;

			switch (shape)
			{
				case Shape.Rectangle:
					base.Grid = RectGrid<TileCell>.HorizontallyWrappedParallelogram(width, height);
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected override IPolarMap<RectPoint> CreatePolarMap()
		{
			switch (mapType)
			{
				case MapType.Rect:
					return new PolarRectMap(Vector2.zero, polarGridProperties.innerRadius, polarGridProperties.outerRadius, Dimensions);
				default:
					throw new ArgumentOutOfRangeException();
			}
			
		}

		protected override Func<RectPoint, int> GetColorFunc(int x0, int x1, int y1)
		{
			return (point => point.GetColor(x0, x1, y1));
		}

		#endregion
	}
}