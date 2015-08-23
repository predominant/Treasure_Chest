using System;
using UnityEngine;

namespace Gamelogic.Grids
{
	[Experimental]
	[Version(1, 14)]
	[AddComponentMenu("Gamelogic/GridBuilders/Mesh Grids/Rect Grid Builder")]
	public class RectMeshGridBuilder : MeshGridBuilder<RectPoint>
	{

		#region Types
		[Serializable]
		public enum Shape
		{
			Rectangle,
			Parallelogram,
			CheckerBoard,
			Circle,
			Custom,
		}

		[Serializable]
		public enum MapType
		{
			Rect,
			Custom
		}
		#endregion

		#region Fields
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
					base.Grid = RectGrid<MeshCell>.Rectangle(rectDimensions.X, rectDimensions.Y);
					break;
				case Shape.Parallelogram:
					base.Grid = RectGrid<MeshCell>.Parallelogram(rectDimensions.X, rectDimensions.Y);
					break;
				case Shape.CheckerBoard:
					base.Grid = RectGrid<MeshCell>.CheckerBoard(rectDimensions.X, rectDimensions.Y);
					break;
				case Shape.Circle:
					base.Grid = RectGrid<MeshCell>.BeginShape().Circle(size).EndShape();
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
					((RectGrid<MeshCell>)base.Grid).SetNeighborsMain();
					break;
				case RectNeighborType.Diagonals:
					((RectGrid<MeshCell>)base.Grid).SetNeighborsDiagonals();
					break;
				case RectNeighborType.MainAndDiagonals:
					((RectGrid<MeshCell>)base.Grid).SetNeighborsMainAndDiagonals();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected override IMap3D<RectPoint> CreateMap()
		{
			switch (mapType)
			{
				case MapType.Rect:
				{
					var windowedMap = new RectMap(cellDimensions.HadamardMul(CellSpacingFactor))
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

		protected override IMeshMap<RectPoint> CreateMeshMap()
		{
			switch (mapType)
			{
				case MapType.Rect:
					return new RectMeshMap(cellDimensions);
				default:
				case MapType.Custom:
					return GetCustomMeshMap();
			}
		}

		protected override Func<RectPoint, int> GetColorFunc(int x0, int x1, int y1)
		{
			return (point => point.GetColor(x0, x1, y1));
		}
		#endregion
	}
}