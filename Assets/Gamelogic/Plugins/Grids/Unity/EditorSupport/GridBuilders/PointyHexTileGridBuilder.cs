
using System;
using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		Class for building a pointy hex grid in the Unity editor. 
		This builder can also make pointy brick grids.


		This component should (generally) not be accessed directly. Instead, add your 
		own component that inherits from GridBebaviour, and access the grid and map
		through there.

		@version1_8

		@ingroup UnityComponents
	*/
	[AddComponentMenu("Gamelogic/GridBuilders/Tile Grids/Pointy Hex Grid Builder")]
	public class PointyHexTileGridBuilder : TileGridBuilder<PointyHexPoint>
	{
		#region Types

		[Serializable]
		public enum Shape
		{
			Rectangle,
			Parallelogram,
			FatRectangle,
			ThinRectangle,
			Hexagon,
			UpTriangle,
			DownTriangle,
			Diamond,
			Single,
			Custom
			//Star,
		}

		[Serializable]
		public enum MapType
		{
			Hex,
			Brick,
			Custom
		}

		#endregion

		#region Fields

		[SerializeField]
		[Tooltip("The shape that the grid will be built in.")]
		private Shape shape = Shape.Hexagon;

		[SerializeField]
		[Tooltip("The map to use with your grid.")]
		private MapType mapType = MapType.Hex;

		#endregion

		#region Properties

		public new PointyHexGrid<TileCell> Grid
		{
			get { return (PointyHexGrid<TileCell>) base.Grid; }
		}

		public new IMap3D<PointyHexPoint> Map
		{
			get { return base.Map; }
		}

		public Shape GridShape
		{
			get { return shape; }

			set { shape = value; }
		}

		#endregion

		#region Implementation

		protected override void InitGrid()
		{
			VectorPoint rectDimensions = Dimensions;

			switch (shape)
			{
				case Shape.Rectangle:
					base.Grid = PointyHexGrid<TileCell>.Rectangle(rectDimensions.X, rectDimensions.Y);
					break;

				case Shape.Parallelogram:
					base.Grid = PointyHexGrid<TileCell>.Parallelogram(rectDimensions.X, rectDimensions.Y);
					break;

				case Shape.FatRectangle:
					base.Grid = PointyHexGrid<TileCell>.FatRectangle(rectDimensions.X, rectDimensions.Y);
					break;

				case Shape.ThinRectangle:
					base.Grid = PointyHexGrid<TileCell>.ThinRectangle(rectDimensions.X, rectDimensions.Y);
					break;

				case Shape.Hexagon:
					base.Grid = PointyHexGrid<TileCell>.Hexagon(Size);
					break;

				case Shape.UpTriangle:
					base.Grid = PointyHexGrid<TileCell>.UpTriangle(Size);
					break;

				case Shape.DownTriangle:
					base.Grid = PointyHexGrid<TileCell>.DownTriangle(Size);
					break;

				case Shape.Diamond:
					base.Grid = PointyHexGrid<TileCell>.Diamond(Size);
					break;

				case Shape.Single:
					base.Grid = PointyHexGrid<TileCell>.Single();
					break;

				case Shape.Custom:
					base.Grid = GetCustomGrid();
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected override WindowedMap<PointyHexPoint> CreateWindowedMap()
		{
			WindowedMap<PointyHexPoint> windowedHexMap;

			float cellWidth;
			float cellHeight;
			Vector2 cellDimensions;

			switch (mapType)
			{
				case MapType.Hex:
					cellWidth = CellPrefab.Dimensions.x;
					cellHeight = CellPrefab.Dimensions.x/69*80;
					cellDimensions = new Vector2(cellWidth, cellHeight);

					windowedHexMap = new PointyHexMap(cellDimensions.HadamardMul(CellSpacingFactor))
						.WithWindow(CenterRect);
					break;

				case MapType.Brick:
					cellWidth = CellPrefab.Dimensions.x;
					cellHeight = CellPrefab.Dimensions.y;
					cellDimensions = new Vector2(cellWidth, cellHeight);

					windowedHexMap = new PointyBrickMap(cellDimensions.HadamardMul(CellSpacingFactor))
						.WithWindow(CenterRect);
					break;

				case MapType.Custom:
					windowedHexMap = GetCustomMap();
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			return windowedHexMap;
		}

		protected override Func<PointyHexPoint, int> GetColorFunc(int x0, int x1, int y1)
		{
			return (point => point.GetColor(x0, x1, y1));
		}

		#endregion
	}
}