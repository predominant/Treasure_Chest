using System;
using System.Collections.Generic;

namespace Gamelogic.Grids
{
	/**
		This class provides extensions that implement obsolete methods.
		
		These methods will be removed in a future version of this library.

		
		
		@version1_3

		@ingroup Scaffolding
	*/
	public static class IMapObsoleteExtensions
	{
		[Obsolete("Use AnchorCellMiddleRight instead. Will be removed in next version.")]
		public static IMap<TPoint> AnchorCellMiddelRight<TPoint>(this IMap<TPoint> map) 
			where TPoint:IGridPoint<TPoint>
		{
			return map.AnchorCellMiddleRight();
		}

		[Obsolete("Use AnchorCellMiddleLeft instead. Will be removed in next version.")]
		public static IMap<TPoint> AnchorCellMiddelLeft<TPoint>(this IMap<TPoint> map)
			where TPoint : IGridPoint<TPoint>

		{
			return map.AnchorCellMiddleLeft();
		}

		[Obsolete("Use AnchorCellMiddleCenter instead. Will be removed in next version.")]
		public static IMap<TPoint> AnchorCellMiddelCenter<TPoint>(this IMap<TPoint> map)
			where TPoint:IGridPoint<TPoint>
		{
			return map.AnchorCellMiddleCenter();
		}
	}

	/**
		This class provides extensions that implement obsolete methods.
		
		These methods will be removed in a future version of this library.

		
		
		@version1_3

		@ingroup Scaffolding
	*/
	public static class WindowedMapObsoleteExtensions
	{
		[Obsolete("Use AlignMiddle instead. Will be removed in a future version.")]
		public static IMap<TPoint> AlignMiddel<TPoint>(this WindowedMap<TPoint> map, IGridSpace<TPoint> grid)
			where TPoint: IGridPoint<TPoint>
		{
			return map.AlignMiddle(grid);
		}

		[Obsolete("Use AlignMiddleLeft instead. Will be removed in a future version.")]
		public static IMap<TPoint> AlignMiddelLeft<TPoint>(WindowedMap<TPoint> map, IGridSpace<TPoint> grid)
			where TPoint : IGridPoint<TPoint>
		{
			return map.AlignMiddleLeft(grid);
		}

		[Obsolete("Use AlignMiddleCenter instead. Will be removed in a future version.")]
		public static IMap<TPoint> AlignMiddelCenter<TPoint>(this WindowedMap<TPoint> map, IGridSpace<TPoint> grid)
			where TPoint : IGridPoint<TPoint>
		{
			return map.AlignMiddleCenter(grid);
		}
	}

	public static class AbstractUniformGridObsoleteExtensions
	{
		[Obsolete("Use the property NeighborDirections instead")]
		public static IEnumerable<TPoint> GetNeighborDirections<TCell, TPoint>(this AbstractUniformGrid<TCell, TPoint> grid)
			where TPoint : IGridPoint<TPoint>, IVectorPoint<TPoint>
		{
			return grid.NeighborDirections;
		}
	}
}
