
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Gamelogic.Grids
{
	/**
		Class for building a point list grid in the Unity editor. 

		This component should (generally) not be accessed directly. Instead, add your 
		own component that inherits from GridBebaviour, and access the grid and map
		through there.

		@version1_8

		@ingroup UnityComponents
	*/
	[AddComponentMenu("Gamelogic/GridBuilders/Tile Grids/Point List Grid Builder")]
	public class PointListTileGridBuilder : TileGridBuilder<LinePoint>
	{
		#region Types

		public enum Shape
		{
			List,
			Poisson
		}

		public enum MapType
		{
			List,
			Custom
		}

		#endregion

		[SerializeField]
		[Tooltip("The shape that the grid will be built in.")]
		private Shape shape = Shape.List;

		[SerializeField]
		[Tooltip("The map to use with your grid.")]
		private MapType mapType = MapType.List;

		[SerializeField] private List<Vector2> pointList;

		[SerializeField] private PoissonDiskProperties poissonDiskProperties = new PoissonDiskProperties()
		{
			minimumDistance = 5,
			pointCount = 5,
			range = new SerializableRect
			{
				left = 0, top = 0, width = 1, height = 1
			}
		};

		protected override void InitGrid()
		{
			switch (shape)
			{
				case Shape.List:
					Grid = LineGrid<TileCell>.Segment(pointList.Count);
					break;
				case Shape.Poisson:
					pointList = PoissonDisk.GeneratePoisson(
						poissonDiskProperties.range.ToRect(),
						poissonDiskProperties.minimumDistance,
						poissonDiskProperties.pointCount);
					Grid = LineGrid<TileCell>.Segment(pointList.Count);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected override WindowedMap<LinePoint> CreateWindowedMap()
		{
			switch (mapType)
			{
				case MapType.List:
					return new VoronoiMap<LinePoint>(grid, new PointListMap(pointList))
						.WithWindow(CenterRect);

				case MapType.Custom:
					return GetCustomMap();

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected override Func<LinePoint, int> GetColorFunc(int x0, int x1, int y1)
		{
			return point => GLMathf.Mod(point, x0);
		}
	}
}