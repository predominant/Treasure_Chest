using System;
using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		Class for building a diamond grid in the Unity editor.

		This component should (generally) not be accessed directly. Instead, add your 
		own component that inherits from GridBebaviour, and access the grid and map
		through there.

		@version1_8
	*/
	[AddComponentMenu("Gamelogic/GridBuilders/Tile Grids/Diamond Grid Builder")]
	public class DiamondTileGridBuilder : TileGridBuilder<DiamondPoint>
	{
		#region Types

		[Serializable]
		public enum Shape
		{
			Rectangle,
			Parallelogram,
			ThinRectangle,
			FatRectangle,
			Diamond,
			Single,
			Custom
		}

		[Serializable]
		public enum MapType
		{
			Diamond,
			Custom
		}

		#endregion

		#region Fields

		private readonly Rect centerRect = new Rect(0, 0, 0, 0);

		[SerializeField]
		[Tooltip("The shape that the grid will be built in.")]
		private Shape shape = Shape.Rectangle;

		[SerializeField]
		[Tooltip("The map to use with your grid.")]
		private MapType mapType = MapType.Diamond;

		[SerializeField]
		[Tooltip("Which cells to consider neighbors")]
		private RectNeighborType neighborSetup =
			RectNeighborType.Main;

		#endregion

		#region Properties

		public new DiamondGrid<TileCell> Grid
		{
			get { return (DiamondGrid<TileCell>) base.Grid; }
		}

		public new IMap3D<DiamondPoint> Map
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
					base.Grid = DiamondGrid<TileCell>.Rectangle(rectDimensions.X, rectDimensions.Y);
					break;

				case Shape.Parallelogram:
					base.Grid = DiamondGrid<TileCell>.Parallelogram(rectDimensions.X, rectDimensions.Y);
					break;

				case Shape.FatRectangle:
					base.Grid = DiamondGrid<TileCell>.FatRectangle(rectDimensions.X, rectDimensions.Y);
					break;

				case Shape.ThinRectangle:
					base.Grid = DiamondGrid<TileCell>.ThinRectangle(rectDimensions.X, rectDimensions.Y);
					break;

				case Shape.Diamond:
					base.Grid = DiamondGrid<TileCell>.Diamond(Size);
					break;

				case Shape.Single:
					base.Grid = DiamondGrid<TileCell>.Single();
					break;

				case Shape.Custom:
					var shapeBuilder = GetComponent<CustomGridBuilder>();
					base.Grid = shapeBuilder.MakeGrid<TileCell, DiamondPoint>();
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			switch (neighborSetup)
			{
				case RectNeighborType.Main:
					((DiamondGrid<TileCell>) base.Grid).SetNeighborsMain();
					break;
				case RectNeighborType.Diagonals:
					((DiamondGrid<TileCell>)base.Grid).SetNeighborsDiagonals();
					break;
				case RectNeighborType.MainAndDiagonals:
					((DiamondGrid<TileCell>)base.Grid).SetNeighborsMainAndDiagonals();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected override WindowedMap<DiamondPoint> CreateWindowedMap()
		{
			WindowedMap<DiamondPoint> windowedHexMap;

			float cellWidth;
			float cellHeight;
			Vector2 cellDimensions;

			switch (mapType)
			{
				case MapType.Diamond:
					cellWidth = CellPrefab.Dimensions.x;
					cellHeight = CellPrefab.Dimensions.y;
					cellDimensions = new Vector2(cellWidth, cellHeight);

					windowedHexMap = new DiamondMap(cellDimensions.HadamardMul(CellSpacingFactor))
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

		protected override Func<DiamondPoint, int> GetColorFunc(int x0, int x1, int y1)
		{
			return (point => point.GetColor(x0, x1, y1));
		}

		#endregion
	}
}