

using System;
using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		Class for building a Cairo grid in the Unity editor.

		This component should (generally) not be accessed directly. Instead, add your 
		own component that inherits from GridBebaviour, and access the grid and map
		through there.

		@version1_8
		@ingroup Unity Components
	*/
	[AddComponentMenu("Gamelogic/GridBuilders/Tile Grids/Cairo Grid Builder")]
	public class CairoTileGridBuilder : TileGridBuilder<CairoPoint>
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
			Cairo,
			Custom
		}

		#endregion

		#region Fields

		[SerializeField]
		[Tooltip("The shape that the grid will be built in.")]
		private Shape shape = Shape.Hexagon;

		[SerializeField]
		[Tooltip("The map to use with your grid.")]
		private MapType mapType = MapType.Cairo;

		#endregion

		#region Properties

		public new CairoGrid<TileCell> Grid
		{
			get { return (CairoGrid<TileCell>) base.Grid; }
		}

		public new IMap3D<CairoPoint> Map
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
					base.Grid = CairoGrid<TileCell>.Rectangle(rectDimensions.X, rectDimensions.Y);
					break;

				case Shape.Parallelogram:
					base.Grid = CairoGrid<TileCell>.Parallelogram(rectDimensions.X, rectDimensions.Y);
					break;

				case Shape.FatRectangle:
					base.Grid = CairoGrid<TileCell>.ThinRectangle(rectDimensions.X, rectDimensions.Y);
					break;

				case Shape.ThinRectangle:
					base.Grid = CairoGrid<TileCell>.FatRectangle(rectDimensions.X, rectDimensions.Y);
					break;

				case Shape.Hexagon:
					base.Grid = CairoGrid<TileCell>.Hexagon(Size);
					break;

				case Shape.UpTriangle:
					//base.Grid = CairoGrid<SpriteCell>.UpTriangle(Size);
					break;

				case Shape.DownTriangle:
					//base.Grid = CairoGrid<SpriteCell>.DownTriangle(Size);
					break;

				case Shape.Diamond:
					//base.Grid = CairoGrid<SpriteCell>.Diamond(Size);
					break;

				case Shape.Single:
					base.Grid = CairoGrid<TileCell>.Single();
					break;

				case Shape.Custom:
					var shapeBuilder = GetComponent<CustomGridBuilder>();
					base.Grid = shapeBuilder.MakeGrid<TileCell, CairoPoint>();
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected override WindowedMap<CairoPoint> CreateWindowedMap()
		{
			WindowedMap<CairoPoint> windowedHexMap;

			float cellWidth;
			float cellHeight;
			Vector2 cellDimensions;

			switch (mapType)
			{
				case MapType.Cairo:
					cellWidth = CellPrefab.Dimensions.x;
					cellHeight = CellPrefab.Dimensions.y;
					cellDimensions = new Vector2(cellWidth, cellHeight);

					windowedHexMap = new CairoMap(cellDimensions.HadamardMul(CellSpacingFactor))
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



		protected override Func<CairoPoint, int> GetColorFunc(int x0, int x1, int y1)
		{
			return (point => point.GetColor(x0, x1, y1));
		}

		#endregion
	}
}