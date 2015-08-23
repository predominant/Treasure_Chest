using System;


using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		Class for building a flat rhomb grid in the Unity editor. 

		This component should (generally) not be accessed directly. Instead, add your 
		own component that inherits from GridBebaviour, and access the grid and map
		through there.

		@version1_8

		@ingroup UnityComponents
	*/
	[AddComponentMenu("Gamelogic/GridBuilders/Tile Grids/Flat Rhomb Grid Builder")]
	public class FlatRhombTileGridBuilder : TileGridBuilder<FlatRhombPoint>
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
			Single,
			SingleGroup,
			Custom
			//Star,	
		}

		[Serializable]
		public enum MapType
		{
			Rhomb,
			Custom
		}

		#endregion

		#region Fields

		[SerializeField]
		[Tooltip("The shape that the grid will be built in.")] 
		private Shape shape = Shape.FatRectangle;

		[SerializeField]
		[Tooltip("The map to use with your grid.")] 
		private MapType mapType = MapType.Rhomb;

		#endregion

		#region Properties

		public new FlatRhombGrid<TileCell> Grid
		{
			get { return (FlatRhombGrid<TileCell>) base.Grid; }
		}

		public new IMap3D<FlatRhombPoint> Map
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
					base.Grid = FlatRhombGrid<TileCell>.Rectangle(rectDimensions.X, rectDimensions.Y);
					break;
				case Shape.Parallelogram:
					base.Grid = FlatRhombGrid<TileCell>.Parallelogram(rectDimensions.X, rectDimensions.Y);
					break;
				case Shape.FatRectangle:
					base.Grid = FlatRhombGrid<TileCell>.ThinRectangle(rectDimensions.X, rectDimensions.Y);
					break;
				case Shape.ThinRectangle:
					base.Grid = FlatRhombGrid<TileCell>.FatRectangle(rectDimensions.X, rectDimensions.Y);
					break;
				case Shape.Hexagon:
					base.Grid = FlatRhombGrid<TileCell>.Hexagon(Size);
					break;
				case Shape.Single:
					base.Grid = FlatRhombGrid<TileCell>.Single();
					break;
				case Shape.SingleGroup:
					base.Grid = FlatRhombGrid<TileCell>.SingleGroup();
					break;
				case Shape.Custom:
					var shapeBuilder = GetComponent<CustomGridBuilder>();
					base.Grid = shapeBuilder.MakeGrid<TileCell, FlatRhombPoint>();
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected override WindowedMap<FlatRhombPoint> CreateWindowedMap()
		{
			WindowedMap<FlatRhombPoint> windowedHexMap;

			float cellWidth;
			float cellHeight;
			Vector2 cellDimensions;

			switch (mapType)
			{
				case MapType.Rhomb:
					cellWidth = CellPrefab.Dimensions.x/80*69;
					cellHeight = CellPrefab.Dimensions.x;
					cellDimensions = new Vector2(cellWidth, cellHeight);

					windowedHexMap = new FlatRhombMap(cellDimensions.HadamardMul(CellSpacingFactor))
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

		protected override Func<FlatRhombPoint, int> GetColorFunc(int x0, int x1, int y1)
		{
			return (point => point.GetColor(x0, x1, y1));
		}

		#endregion
	}
}