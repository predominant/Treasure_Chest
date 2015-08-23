//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//



using System;

namespace Gamelogic.Grids
{
	/**
		A rhombille grid in the flat orientation, that is, there are rhombusses with horizontal edges.
	
		
		
		@version1_0

		@ingroup Grids
	*/
	[Serializable]
	public partial class FlatRhombGrid<TCell> : AbstractSplicedGrid<TCell, FlatRhombPoint, FlatHexPoint>
	{
		#region Implementation
		protected override FlatRhombPoint MakePoint(FlatHexPoint basePoint, int index)
		{
			return new FlatRhombPoint(basePoint.X, basePoint.Y, index);
		}

		protected override IGrid<TCell[], FlatHexPoint> MakeUnderlyingGrid(int width, int height)
		{
			return new FlatHexGrid<TCell[]>(width, height);
		}
		#endregion

		#region StorageInfo
		public static FlatHexPoint GridPointFromArrayPoint(ArrayPoint point)
		{
			return FlatHexGrid<TCell>.GridPointFromArrayPoint(point);
		}

		public static ArrayPoint ArrayPointFromGridPoint(FlatHexPoint point)
		{
			return FlatHexGrid<TCell>.ArrayPointFromGridPoint(point);
		}
		#endregion
	}
}