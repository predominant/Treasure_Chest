//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//



using System;
using System.Collections.Generic;

namespace Gamelogic.Grids
{
	/**
		A grid for flat hexagons, that is, hexagons with two horizontal edges.
	
		
		
		@version1_0

		@ingroup Grids
	*/
	[Serializable]
	public partial class FlatHexGrid<TCell> : AbstractUniformGrid<TCell, FlatHexPoint>, IEvenGrid<TCell, FlatHexPoint, FlatHexPoint>
	{
		#region Storage
		override protected FlatHexPoint PointFromArrayPoint(int aX, int aY)
		{
			return GridPointFromArrayPoint(new ArrayPoint(aX, aY));
		}

		override protected ArrayPoint ArrayPointFromPoint(int hX, int hY)
		{
			return ArrayPointFromGridPoint(new FlatHexPoint(hX, hY));
		}

		override protected ArrayPoint ArrayPointFromPoint(FlatHexPoint hexPoint)
		{
			return ArrayPointFromGridPoint(hexPoint);
		}

		public static FlatHexPoint GridPointFromArrayPoint(ArrayPoint point)
		{
			return new FlatHexPoint(point.X, point.Y);
		}

		public static ArrayPoint ArrayPointFromGridPoint(FlatHexPoint point)
		{
			return new ArrayPoint(point.X, point.Y);
		}

		public IEnumerable<FlatHexPoint> GetPrincipleNeighborDirections()
		{
			return NeighborDirections.TakeHalf();
		}
		#endregion

		#region Wrapped Grids
		/**
			Returns a grid wrapped horizontally along a parallelogram.

			@version1_7
		*/
		public static WrappedGrid<TCell, FlatHexPoint> HorizontallyWrappedRectangle(int width, int height)
		{
			var grid = Rectangle(width, height);
			var wrapper = new FlatHexHorizontalWrapper(width);
			var wrappedGrid = new WrappedGrid<TCell, FlatHexPoint>(grid, wrapper);

			return wrappedGrid;
		}
		#endregion
	}
}