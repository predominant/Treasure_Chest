using System;
using UnityEngine;

namespace Gamelogic.Grids
{

	[Experimental]
	[Version(1, 14)]
	[AddComponentMenu("Gamelogic/GridBuilders/Mesh Grids/Pointy Hex Grid Builder")]
	public class PointyHexMeshGridBuilder : MeshGridBuilder<PointyHexPoint>
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
					Grid = PointyHexGrid<MeshCell>.Rectangle(rectDimensions.X, rectDimensions.Y);
					break;

				case Shape.Parallelogram:
					Grid = PointyHexGrid<MeshCell>.Parallelogram(rectDimensions.X, rectDimensions.Y);
					break;

				case Shape.FatRectangle:
					Grid = PointyHexGrid<MeshCell>.FatRectangle(rectDimensions.X, rectDimensions.Y);
					break;

				case Shape.ThinRectangle:
					Grid = PointyHexGrid<MeshCell>.ThinRectangle(rectDimensions.X, rectDimensions.Y);
					break;

				case Shape.Hexagon:
					Grid = PointyHexGrid<MeshCell>.Hexagon(Size);
					break;

				case Shape.UpTriangle:
					Grid = PointyHexGrid<MeshCell>.UpTriangle(Size);
					break;

				case Shape.DownTriangle:
					Grid = PointyHexGrid<MeshCell>.DownTriangle(Size);
					break;

				case Shape.Diamond:
					Grid = PointyHexGrid<MeshCell>.Diamond(Size);
					break;

				case Shape.Single:
					Grid = PointyHexGrid<MeshCell>.Single();
					break;

				case Shape.Custom:
					Grid = GetCustomGrid();
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected override IMap3D<PointyHexPoint> CreateMap()
		{
			switch (mapType)
			{
				case MapType.Hex:
				{
					var windowedMap = new PointyHexMap(cellDimensions.HadamardMul(CellSpacingFactor))
						.WithWindow(CenterRect);

					return GetAlignedMap(windowedMap);
				}
				case MapType.Brick:
				{
					var windowedMap = new PointyBrickMap(cellDimensions.HadamardMul(CellSpacingFactor))
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

		protected override IMeshMap<PointyHexPoint> CreateMeshMap()
		{
			switch (mapType)
			{
				case MapType.Hex:
					return new PointyHexMeshMap(cellDimensions, new PointyHexMap(cellDimensions));
				case MapType.Brick:
					return new PointyBrickMeshMap(cellDimensions);
				default:
				case MapType.Custom:
					return GetCustomMeshMap();
			}
		}

		protected override Func<PointyHexPoint, int> GetColorFunc(int x0, int x1, int y1)
		{
			return (point => point.GetColor(x0, x1, y1));
		}
		#endregion
	}
}