

using System;
using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		Class for building a polar pointy brick grid in the Unity editor. 

		This component should (generally) not be accessed directly. Instead, add your 
		own component that inherits from GridBebaviour, and access the grid and map
		through there.

		@version1_8

		@ingroup UnityComponents
	*/
	[AddComponentMenu("Gamelogic/GridBuilders/Tile Grids/Polar Pointy Brick Grid Builder")]
	public class PolarPointyBrickTileGridBuilder : PolarTileGridBuilder<PointyHexPoint>
	{
		#region Types

		[Serializable]
		public enum Shape
		{
			Parallelogram,
		}

		[Serializable]
		public enum MapType
		{
			Brick
		}

		#endregion

		#region Fields
		[SerializeField]
		[Tooltip("The shape that the grid will be built in.")]
		private Shape shape = Shape.Parallelogram;

		[SerializeField]
		[Tooltip("The map to use with your grid.")]
		private MapType mapType = MapType.Brick;

		#endregion

		#region Properties

		public new WrappedGrid<MeshTileCell, PointyHexPoint> Grid
		{
			get { return (WrappedGrid<MeshTileCell, PointyHexPoint>)base.Grid; }
		}

		public new IMap3D<PointyHexPoint> Map
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
					base.Grid = PointyHexGrid<TileCell>.HorizontallyWrappedParallelogram(width, height);
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected override IPolarMap<PointyHexPoint> CreatePolarMap()
		{
			switch (mapType)
			{
				case MapType.Brick:
					return new PolarPointyBrickMap(
						Vector2.zero, 
						polarGridProperties.innerRadius, 
						polarGridProperties.outerRadius,
				Dimensions);
				default:
					throw new ArgumentOutOfRangeException();
			}
			
		}

		protected override Func<PointyHexPoint, int> GetColorFunc(int x0, int x1, int y1)
		{
			return (point => point.GetColor(x0, x1, y1));
		}

		#endregion
	}
}