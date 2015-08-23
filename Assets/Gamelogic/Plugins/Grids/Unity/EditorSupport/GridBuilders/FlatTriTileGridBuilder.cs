

using System;
using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		Class for building a flat tri grid in the Unity editor. 

		This component should (generally) not be accessed directly. Instead, add your 
		own component that inherits from GridBebaviour, and access the grid and map
		through there.

		@version1_8

		@ingroup UnityComponents
	*/
	[AddComponentMenu("Gamelogic/GridBuilders/Tile Grids/Flat Tri Grid Builder")]
	public class FlatTriTileGridBuilder : TileGridBuilder<FlatTriPoint>
	{
		#region Types

		[Serializable]
		public enum Shape
		{
			Rectangle,
			//ThinRectangle,
			//FatRectangle,
			Parallelogram,
			UpTriangle,
			DownTriangle,
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
		private Shape shape = Shape.Parallelogram;

		[SerializeField]
		[Tooltip("The map to use with your grid.")]
		private MapType mapType = MapType.Tri;

		#endregion

		#region Properties

		public new FlatTriGrid<TileCell> Grid
		{
			get { return (FlatTriGrid<TileCell>) base.Grid; }
		}

		public new IMap3D<FlatTriPoint> Map
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
					base.Grid = FlatTriGrid<TileCell>.Rectangle(rectDimensions.X, rectDimensions.Y);
					break;
				case Shape.Parallelogram:
					base.Grid = FlatTriGrid<TileCell>.ParallelogramXY(rectDimensions.X, rectDimensions.Y);
					break;
				case Shape.UpTriangle:
					base.Grid = FlatTriGrid<TileCell>.UpTriangle(size);
					break;
				case Shape.DownTriangle:
					base.Grid = FlatTriGrid<TileCell>.DownTriangle(size);
					break;
				case Shape.Hexagon:
					base.Grid = FlatTriGrid<TileCell>.Hexagon(size);
					break;
				case Shape.Star:
					base.Grid = FlatTriGrid<TileCell>.Star(size);
					break;
				case Shape.Single:
					base.Grid = FlatTriGrid<TileCell>.Single();
					break;
				case Shape.SingleGroup:
					base.Grid = FlatTriGrid<TileCell>.SingleGroup();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected override WindowedMap<FlatTriPoint> CreateWindowedMap()
		{
			WindowedMap<FlatTriPoint> windowedHexMap;

			switch (mapType)
			{
				case MapType.Tri:
					windowedHexMap = new FlatTriMap(cellPrefab.Dimensions)
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

		protected override Func<FlatTriPoint, int> GetColorFunc(int x0, int x1, int y1)
		{
			return (point => point.GetColor(x0, x1, y1));
		}

		#endregion
	}
}