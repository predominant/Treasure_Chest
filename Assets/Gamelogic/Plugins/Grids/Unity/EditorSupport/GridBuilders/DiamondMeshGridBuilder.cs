using System;
using UnityEngine;

namespace Gamelogic.Grids
{
	[Experimental]
	[Version(1, 14)]
	[AddComponentMenu("Gamelogic/GridBuilders/Mesh Grids/Diamond Grid Builder")]
	public class DiamondMeshGridBuilder : MeshGridBuilder<DiamondPoint>
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

		public new IMap3D<DiamondPoint> Map
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
					base.Grid = DiamondGrid<MeshCell>.Rectangle(rectDimensions.X, rectDimensions.Y);
					break;

				case Shape.Parallelogram:
					base.Grid = DiamondGrid<MeshCell>.Parallelogram(rectDimensions.X, rectDimensions.Y);
					break;

				case Shape.FatRectangle:
					base.Grid = DiamondGrid<MeshCell>.FatRectangle(rectDimensions.X, rectDimensions.Y);
					break;

				case Shape.ThinRectangle:
					base.Grid = DiamondGrid<MeshCell>.ThinRectangle(rectDimensions.X, rectDimensions.Y);
					break;

				case Shape.Diamond:
					base.Grid = DiamondGrid<MeshCell>.Diamond(Size);
					break;

				case Shape.Single:
					base.Grid = DiamondGrid<MeshCell>.Single();
					break;

				case Shape.Custom:
					var shapeBuilder = GetComponent<CustomGridBuilder>();
					base.Grid = shapeBuilder.MakeGrid<MeshCell, DiamondPoint>();
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			switch (neighborSetup)
			{
				case RectNeighborType.Main:
					((DiamondGrid<MeshCell>)base.Grid).SetNeighborsMain();
					break;
				case RectNeighborType.Diagonals:
					((DiamondGrid<MeshCell>)base.Grid).SetNeighborsDiagonals();
					break;
				case RectNeighborType.MainAndDiagonals:
					((DiamondGrid<MeshCell>)base.Grid).SetNeighborsMainAndDiagonals();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected override IMap3D<DiamondPoint> CreateMap()
		{
			switch (mapType)
			{
				case MapType.Rect:
				{
					var windowedMap = new DiamondMap(cellDimensions.HadamardMul(CellSpacingFactor))
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

		protected override IMeshMap<DiamondPoint> CreateMeshMap()
		{
			switch (mapType)
			{
				case MapType.Rect:
					return new DiamondMeshMap(cellDimensions);
				default:
				case MapType.Custom:
					return GetCustomMeshMap();
			}
		}

		protected override Func<DiamondPoint, int> GetColorFunc(int x0, int x1, int y1)
		{
			return (point => point.GetColor(x0, x1, y1));
		}
		#endregion
	}
}