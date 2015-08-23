//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

namespace Gamelogic.Grids
{
	/**
		A CiaroGrid is a grid where all the cells are pentagons (so each cell has five neighbors).
		
		See http://en.wikipedia.org/wiki/Cairo_pentagonal_tiling.

		
		
		@version1_2
		
		@ingroup Grids
	*/
	[Experimental]
	public partial class CairoGrid<TCell> : AbstractSplicedGrid<TCell, CairoPoint, PointyHexPoint>
	{
		

		#region Implementation

		protected override CairoPoint MakePoint(PointyHexPoint basePoint, int index)
		{
			return new CairoPoint(basePoint.X, basePoint.Y, index);
		}

		protected override IGrid<TCell[], PointyHexPoint> MakeUnderlyingGrid(int width, int height)
		{
			return new PointyHexGrid<TCell[]>(width, height);
		}
		#endregion

		#region StorageInfo
		public static PointyHexPoint GridPointFromArrayPoint(ArrayPoint point)
		{
			return PointyHexGrid<TCell>.GridPointFromArrayPoint(point);
		}

		public static ArrayPoint ArrayPointFromGridPoint(PointyHexPoint point)
		{
			return PointyHexGrid<TCell>.ArrayPointFromGridPoint(point);
		}
		#endregion
	}
}