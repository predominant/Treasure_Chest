using System;
using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		Class for building a rectangular grid in the Unity editor.

		This component should (generally) not be accessed directly. Instead, add your 
		own component that inherits from GridBebaviour, and access the grid and map
		through there.

		@version1_8

		@ingroup UnityComponents
	*/
	[AddComponentMenu("Gamelogic/GridBuilders/Tile Grids/Rect Grid Builder")]
	public class RectTileGridBuilder : TileGridBuilder<RectPoint>
	{
		#region Types
		[Serializable]
		public enum Shape
		{
			Rectangle,
			Parallelogram,
			CheckerBoard,
			/**
				@version1_9
			*/
			Circle,
			Custom,

			//Star,
		}

		[Serializable]
		public enum MapType
		{
			Rect,
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
		private MapType mapType = MapType.Rect;

		[SerializeField]
		[Tooltip("Which cells to consider neighbors")] 
		private RectNeighborType neighborSetup = RectNeighborType.Main;
		#endregion

		#region Properties
		public new RectGrid<TileCell> Grid
		{
			get
			{

				return (RectGrid<TileCell>)base.Grid;
			}
		}

		public new IMap3D<RectPoint> Map
		{
			get { return base.Map; }
		}

		public Shape GridShape
		{
			get
			{
				return shape;
			}
		}
		#endregion

		#region Implementation
		protected override void InitGrid()
		{
			VectorPoint rectDimensions = Dimensions;

			switch (shape)
			{
				case Shape.Rectangle:
					base.Grid = RectGrid<TileCell>.Rectangle(rectDimensions.X, rectDimensions.Y);
					break;
				case Shape.Parallelogram:
					base.Grid = RectGrid<TileCell>.Parallelogram(rectDimensions.X, rectDimensions.Y);
					break;
				case Shape.CheckerBoard:
					base.Grid = RectGrid<TileCell>.CheckerBoard(rectDimensions.X, rectDimensions.Y);
					break;
				case Shape.Circle:
					base.Grid = RectGrid<TileCell>.BeginShape().Circle(size).EndShape();
					break;
				case Shape.Custom:
					base.Grid = GetCustomGrid();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			switch (neighborSetup)
			{
				case RectNeighborType.Main:
					((RectGrid<TileCell>)base.Grid).SetNeighborsMain();
					break;
				case RectNeighborType.Diagonals:
					((RectGrid<TileCell>)base.Grid).SetNeighborsDiagonals();
					break;
				case RectNeighborType.MainAndDiagonals:
					((RectGrid<TileCell>)base.Grid).SetNeighborsMainAndDiagonals();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected override WindowedMap<RectPoint> CreateWindowedMap()
		{
			WindowedMap<RectPoint> windowedHexMap;

			float cellWidth;
			float cellHeight;
			Vector2 cellDimensions;

			switch (mapType)
			{
				case MapType.Rect:
					cellWidth = CellPrefab.Dimensions.x;
					cellHeight = CellPrefab.Dimensions.y;
					cellDimensions = new Vector2(cellWidth, cellHeight);

					windowedHexMap = new RectMap(cellDimensions.HadamardMul(CellSpacingFactor))
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

		protected override Func<RectPoint, int> GetColorFunc(int x0, int x1, int y1)
		{
			return (point => point.GetColor(x0, x1, y1));
		}
		#endregion
	}
}