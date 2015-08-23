using System;
using UnityEngine;

namespace Gamelogic.Grids
{
	[Experimental]
	[Version(1, 14)]
	[AddComponentMenu("Gamelogic/GridBuilders/Mesh Grids/FlatHex Grid Builder")]
	
	public class FlatHexMeshGridBuilder : MeshGridBuilder<FlatHexPoint>
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
			Custom,
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

		#region Implementation
		protected override void InitGrid()
		{
			VectorPoint rectDimensions = Dimensions;

			switch (shape)
			{
				case Shape.Rectangle:
					Grid = FlatHexGrid<MeshCell>.Rectangle(rectDimensions.X, rectDimensions.Y);
					break;

				case Shape.Parallelogram:
					Grid = FlatHexGrid<MeshCell>.Parallelogram(rectDimensions.X, rectDimensions.Y);
					break;

				case Shape.FatRectangle:
					Grid = FlatHexGrid<MeshCell>.FatRectangle(rectDimensions.X, rectDimensions.Y);
					break;

				case Shape.ThinRectangle:
					Grid = FlatHexGrid<MeshCell>.ThinRectangle(rectDimensions.X, rectDimensions.Y);
					break;

				case Shape.Hexagon:
					Grid = FlatHexGrid<MeshCell>.Hexagon(Size);
					break;

				case Shape.LeftTriangle:
					Grid = FlatHexGrid<MeshCell>.LeftTriangle(Size);
					break;

				case Shape.RightTriangle:
					Grid = FlatHexGrid<MeshCell>.RightTriangle(Size);
					break;

				case Shape.Diamond:
					Grid = FlatHexGrid<MeshCell>.Diamond(Size);
					break;

				case Shape.Single:
					Grid = FlatHexGrid<MeshCell>.Single();
					break;

				case Shape.Custom:
					Grid = GetCustomGrid();
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected override IMap3D<FlatHexPoint> CreateMap()
		{
			switch (mapType)
			{
				case MapType.Hex:
				{
					var windowedMap = new FlatHexMap(cellDimensions.HadamardMul(CellSpacingFactor))
						.WithWindow(CenterRect);

					return GetAlignedMap(windowedMap);
				}
				case MapType.Brick:
				{
					var windowedMap = new FlatBrickMap(cellDimensions.HadamardMul(CellSpacingFactor))
						.WithWindow(CenterRect);

					return GetAlignedMap(windowedMap);
				}
				case MapType.Custom:
				{
					var map = GetCustomMap3D();

					return map;
				}

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected override IMeshMap<FlatHexPoint> CreateMeshMap()
		{
			switch (mapType)
			{
				case MapType.Hex:
					return new FlatHexMeshMap(cellDimensions);
				case MapType.Brick:
					return new FlatBrickMeshMap(cellDimensions);
				default:
				case MapType.Custom:
					return GetCustomMeshMap();
			}
		}

		protected override Func<FlatHexPoint, int> GetColorFunc(int x0, int x1, int y1)
		{
			return (point => point.GetColor(x0, x1, y1));
		}
		#endregion
	}
}