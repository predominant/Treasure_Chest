//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System.Collections.Generic;

namespace Gamelogic.Grids
{
	/**
		IVectorGrids are build on (spliced) vector points. They are regular and uniform.

		
		
		@version1_0

		@ingroup Interface
	*/
	public interface IVectorGrid<TCell, TPoint, TBasePoint> : IGrid<TCell, TPoint>
		where TPoint : ISplicedVectorPoint<TPoint, TBasePoint>, IGridPoint<TPoint>
		where TBasePoint : IVectorPoint<TBasePoint>, IGridPoint<TBasePoint>
	{
		IEnumerable<TPoint> GetNeighborDirections(int cellIndex);
	}
}