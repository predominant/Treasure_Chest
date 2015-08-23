

using System;
using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		Class for building a pointy tri grid in the Unity editor. 

		This component should (generally) not be accessed directly. Instead, add your 
		own component that inherits from GridBebaviour, and access the grid and map
		through there.

		@version1_8

		@ingroup UnityComponents
	*/
	[AddComponentMenu("Gamelogic/GridBuilders/Tile Grids/Pointy Tri Grid Builder")]
	public class PointyTriTileGridBuilder : TileGridBuilder<PointyTriPoint>
	{
		#region Types

		[Serializable]
		public enum Shape
		{
			Rectangle,
			ThinRectangle,
			FatRectangle,
			Parallelogram,
			LeftTriangle,
			RightTriangle,
			Hexagon,
			Star,
			Single,
			SingleGroup,
			Custom
		}

		[Serializable]
		public enum MapType
		{
			Tri,
			Custom
		}

		#endregion

		#region Fields

		private readonly Rect centerRect = new Rect(0, 0, 0, 0);

		[SerializeField]
		[Tooltip("The shape that the grid will be built in.")] 
		private Shape shape = Shape.FatRectangle;

		[SerializeField]
		[Tooltip("The map to use with your grid.")] 
		private MapType mapType = MapType.Tri;

		#endregion

		#region Properties

		public new PointyTriGrid<TileCell> Grid
		{
			get { return (PointyTriGrid<TileCell>) base.Grid; }
		}

		public new IMap3D<PointyTriPoint> Map
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
			VectorPoint rectDimensions = Dimensions;

			switch (shape)
			{
				case Shape.Rectangle:
					base.Grid = PointyTriGrid<TileCell>.Rectangle(rectDimensions.X, rectDimensions.Y);
					break;
				case Shape.Parallelogram:
					base.Grid = PointyTriGrid<TileCell>.ParallelogramXY(rectDimensions.X, rectDimensions.Y);
					break;
				case Shape.ThinRectangle:
					base.Grid = PointyTriGrid<TileCell>.ThinRectangle(rectDimensions.X, rectDimensions.Y);
					break;
				case Shape.FatRectangle:
					base.Grid = PointyTriGrid<TileCell>.FatRectangle(rectDimensions.X, rectDimensions.Y);
					break;
				case Shape.LeftTriangle:
					base.Grid = PointyTriGrid<TileCell>.LeftTriangle(size);
					break;
				case Shape.RightTriangle:
					base.Grid = PointyTriGrid<TileCell>.RightTriangle(size);
					break;
				case Shape.Hexagon:
					base.Grid = PointyTriGrid<TileCell>.Hexagon(size);
					break;
				case Shape.Star:
					base.Grid = PointyTriGrid<TileCell>.Star(size);
					break;
				case Shape.Single:
					base.Grid = PointyTriGrid<TileCell>.Single();
					break;
				case Shape.SingleGroup:
					base.Grid = PointyTriGrid<TileCell>.SingleGroup();
					break;
				case Shape.Custom:
					base.Grid = GetCustomGrid();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected override WindowedMap<PointyTriPoint> CreateWindowedMap()
		{
			WindowedMap<PointyTriPoint> windowedHexMap;

			switch (mapType)
			{
				case MapType.Tri:
					windowedHexMap = new PointyTriMap(cellPrefab.Dimensions)
						.WithWindow(centerRect);
					break;

				case MapType.Custom:
					windowedHexMap = GetCustomMap();
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			return windowedHexMap;
		}

		protected override Func<PointyTriPoint, int> GetColorFunc(int x0, int x1, int y1)
		{
			return (point => point.GetColor(x0, x1, y1));
		}

		#endregion
	}
}