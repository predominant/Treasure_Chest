using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamelogic.Grids
{
	/**
		A one-way map that can be used to make maps for arbitrary spliced grids, where each point is mapped to 
		the base point location, plus an offset, where each index has a different offset.

		For example, if the spliced grid is a diamond grid, with each cell spliced into a top and bottom 
		triangle, then the two offsets will be above and below the origin (for example, (0, 1) and (0, -1)).

		@version1_8
		@ingroup Maps
	*/
	public class SplicedMap<TBasePoint> : IGridToWorldMap<SplicedPoint<TBasePoint>> where TBasePoint : IGridPoint<TBasePoint>
	{
		private IGridToWorldMap<TBasePoint> baseMap;
		private Vector2[] offsets;

		public SplicedMap(IGridToWorldMap<TBasePoint> baseMap, IEnumerable<Vector2> offsets)
		{
			this.baseMap = baseMap;
			this.offsets = offsets.ToArray();
		}

		public Vector2 this[SplicedPoint<TBasePoint> point]
		{
			get
			{
				return baseMap[point.BasePoint] + offsets[point.I];
			}
		}
	}
}