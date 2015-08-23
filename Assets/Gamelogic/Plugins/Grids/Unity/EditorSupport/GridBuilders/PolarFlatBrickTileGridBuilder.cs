

using System;
using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		Class for building a polar flat brick grid in the Unity editor. 

		This component should (generally) not be accessed directly. Instead, add your 
		own component that inherits from GridBebaviour, and access the grid and map
		through there.

		@version1_8

		@ingroup UnityComponents
	*/
	[AddComponentMenu("Gamelogic/GridBuilders/Tile Grids/Polar Flat Brick Grid Builder")]
	public class PolarFlatBrickTileGridBuilder : PolarTileGridBuilder<FlatHexPoint>
	{
		#region Types

		[Serializable]
		public enum Shape
		{
			Rectangle,
			Parallelogram
		}

		[Serializable]
		public enum MapType
		{
			Brick,
			Custom
		}

		#endregion

		#region Fields
		[SerializeField]
		[Tooltip("The shape that the grid will be built in.")]
		private Shape shape = Shape.Rectangle;

		[SerializeField]
		[Tooltip("The map to use with your grid.")]
		private MapType mapType = MapType.Brick;

		#endregion

		#region Properties

		public new WrappedGrid<TileCell, FlatHexPoint> Grid
		{
			get { return (WrappedGrid<TileCell, FlatHexPoint>)base.Grid; }
		}

		public new IMap3D<FlatHexPoint> Map
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
				case Shape.Parallelogram:
					base.Grid = FlatHexGrid<TileCell>.HorizontallyWrappedParallelogram(width, height);
					break;

				case Shape.Rectangle:
					base.Grid = FlatHexGrid<TileCell>.HorizontallyWrappedRectangle(width, height);
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected override IPolarMap<FlatHexPoint> CreatePolarMap()
		{
			switch (mapType)
			{
				case MapType.Brick:
					return new PolarFlatBrickMap(Vector2.zero, polarGridProperties.innerRadius, polarGridProperties.outerRadius,
				Dimensions);

				default:
					throw new ArgumentOutOfRangeException();
			}
			
		}

		protected override Func<FlatHexPoint, int> GetColorFunc(int x0, int x1, int y1)
		{
			return (point => point.GetColor(x0, x1, y1));
		}

		#endregion
	}
}