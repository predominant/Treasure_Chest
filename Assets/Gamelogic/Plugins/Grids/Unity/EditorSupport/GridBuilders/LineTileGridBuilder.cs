

using UnityEngine;
using System;

namespace Gamelogic.Grids
{
	/**
		Class for building a line grid in the Unity editor. 

		This component should (generally) not be accessed directly. Instead, add your 
		own component that inherits from GridBebaviour, and access the grid and map
		through there.

		@version1_8

		@ingroup UnityComponents
	*/
	[AddComponentMenu("Gamelogic/GridBuilders/Tile Grids/Line Grid Builder")]
	public class LineTileGridBuilder : TileGridBuilder<LinePoint>
	{
		#region Types

		public enum Shape
		{
			Segment,
			Custom
		}

		public enum MapType
		{
			Line,
			ArchimedeanSpiral,
			//PointList,
			Custom
		}

		#endregion

		[SerializeField]
		[Tooltip("The shape that the grid will be built in.")]
		private Shape shape = Shape.Segment;

		[SerializeField]
		[Tooltip("The map to use with your grid.")]
		private MapType mapType = MapType.ArchimedeanSpiral;

		protected override void InitGrid()
		{
			switch (shape)
			{
				case Shape.Segment:
					Grid = LineGrid<TileCell>.Segment(size);
					break;
				case Shape.Custom:
					var shapeBuilder = GetComponent<CustomGridBuilder>();
					Grid = shapeBuilder.MakeGrid<TileCell, LinePoint>();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected override WindowedMap<LinePoint> CreateWindowedMap()
		{
			float cellWidth;
			float cellHeight;
			Vector2 cellDimensions;

			switch (mapType)
			{
				case MapType.Line:
					cellWidth = CellPrefab.Dimensions.x;
					cellHeight = CellPrefab.Dimensions.y;
					cellDimensions = new Vector2(cellWidth, cellHeight);

					return new LineMap(cellDimensions.HadamardMul(CellSpacingFactor))
						.WithWindow(CenterRect);

				case MapType.ArchimedeanSpiral:
				default:
					cellWidth = CellPrefab.Dimensions.x;
					cellHeight = CellPrefab.Dimensions.y;
					cellDimensions = new Vector2(cellWidth, cellHeight);

					return new ArchimedeanSpiralMap(new Vector2(30, 30), Grid)
						.Scale(cellWidth/30, cellHeight/30)
						.Scale(CellSpacingFactor)
						.WithWindow(CenterRect);

				case MapType.Custom:
					return GetCustomMap();
			}
		}

		protected override Func<LinePoint, int> GetColorFunc(int x0, int x1, int y1)
		{
			return point => GLMathf.Mod(point, x0);
		}
	}
}