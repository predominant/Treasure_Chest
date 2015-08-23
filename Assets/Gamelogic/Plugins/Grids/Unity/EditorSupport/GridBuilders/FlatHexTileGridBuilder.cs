using System;
using UnityEngine;



namespace Gamelogic.Grids
{
	/**
		Class for building a flat hex grid in the Unity editor. 
		This builder can also make flat brick grids.

		This component should (generally) not be accessed directly. Instead, add your 
		own component that inherits from GridBebaviour, and access the grid and map
		through there.

		@version1_8
		
		@ingroup UnityComponents
	*/
	[AddComponentMenu("Gamelogic/GridBuilders/Tile Grids/Flat Hex Grid Builder")]
	public class FlatHexTileGridBuilder : TileGridBuilder<FlatHexPoint>
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
			LeftTriangle,
			RightTriangle,
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
		private Shape shape = Shape.FatRectangle;

		[SerializeField]
		[Tooltip("The map to use with your grid.")] 
		private MapType mapType = MapType.Hex;

		#endregion

		#region Properties

		public new FlatHexGrid<TileCell> Grid
		{
			get { return (FlatHexGrid<TileCell>) base.Grid; }
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
			VectorPoint rectDimensions = Dimensions;

			switch (shape)
			{
				case Shape.Rectangle:
					base.Grid = FlatHexGrid<TileCell>.Rectangle(rectDimensions.X, rectDimensions.Y);
					break;
				case Shape.Parallelogram:
					base.Grid = FlatHexGrid<TileCell>.Parallelogram(rectDimensions.X, rectDimensions.Y);
					break;
				case Shape.FatRectangle:
					base.Grid = FlatHexGrid<TileCell>.FatRectangle(rectDimensions.X, rectDimensions.Y);
					break;
				case Shape.ThinRectangle:
					base.Grid = FlatHexGrid<TileCell>.ThinRectangle(rectDimensions.X, rectDimensions.Y);
					break;
				case Shape.Hexagon:
					base.Grid = FlatHexGrid<TileCell>.Hexagon(Size);
					break;
				case Shape.LeftTriangle:
					base.Grid = FlatHexGrid<TileCell>.LeftTriangle(Size);
					break;
				case Shape.RightTriangle:
					base.Grid = FlatHexGrid<TileCell>.RightTriangle(Size);
					break;
				case Shape.Diamond:
					base.Grid = FlatHexGrid<TileCell>.Diamond(Size);
					break;
				case Shape.Single:
					base.Grid = FlatHexGrid<TileCell>.Single();
					break;
				case Shape.Custom:
					var shapeBuilder = GetComponent<CustomGridBuilder>();
					base.Grid = shapeBuilder.MakeGrid<TileCell, FlatHexPoint>();
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected override WindowedMap<FlatHexPoint> CreateWindowedMap()
		{
			WindowedMap<FlatHexPoint> windowedHexMap;

			float cellWidth;
			float cellHeight;
			Vector2 cellDimensions;

			switch (mapType)
			{
				case MapType.Hex:
					cellWidth = CellPrefab.Dimensions.x;
					cellHeight = CellPrefab.Dimensions.y;
					cellDimensions = new Vector2(cellWidth, cellHeight);

					windowedHexMap = new FlatHexMap(cellDimensions.HadamardMul(CellSpacingFactor))
						.WithWindow(CenterRect);
					break;
				case MapType.Brick:

					cellWidth = CellPrefab.Dimensions.x;
					cellHeight = CellPrefab.Dimensions.y;
					cellDimensions = new Vector2(cellWidth, cellHeight);

					windowedHexMap = new FlatBrickMap(cellDimensions.HadamardMul(CellSpacingFactor))
						.WithWindow(CenterRect);
					break;
				case MapType.Custom:
					windowedHexMap = GetComponent<CustomMapBuilder>().CreateWindowedMap<FlatHexPoint>();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return windowedHexMap;
		}

		protected override Func<FlatHexPoint, int> GetColorFunc(int x0, int x1, int y1)
		{
			return (point => point.GetColor(x0, x1, y1));
		}

		#endregion
	}
}