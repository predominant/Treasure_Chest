//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//



using System;

namespace Gamelogic.Grids
{
	/**
		A rhombille grid in the pointy orientation, that is, there are rhombusses with vertical edges.
	
		
		
		@version1_0

		@ingroup Grids
	*/
	[Serializable]
	public partial class PointyRhombGrid<TCell> :
		AbstractSplicedGrid<TCell, PointyRhombPoint, PointyHexPoint>
	{
		#region Implementation

		protected override PointyRhombPoint MakePoint(PointyHexPoint basePoint, int index)
		{
			return new PointyRhombPoint(basePoint.X, basePoint.Y, index);
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