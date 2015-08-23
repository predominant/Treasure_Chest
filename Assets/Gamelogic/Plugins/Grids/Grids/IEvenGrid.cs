//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System.Collections.Generic;

namespace Gamelogic.Grids
{
	/**
		A grid where cells have an even number of neighbors. In an even grid each neighbor has an opposite neighbor.

		
		
		@version1_0

		@ingroup Interface
	*/
	public interface IEvenGrid<TCell, TPoint, TBasePoint> : IVectorGrid<TCell, TPoint, TBasePoint>
		where TPoint : ISplicedVectorPoint<TPoint, TBasePoint>, IGridPoint<TPoint>
		where TBasePoint : IVectorPoint<TBasePoint>, IGridPoint<TBasePoint>
	{
		/**
			This is the set of neighbor directions so that it contains only one of 
			the neighbor directions of a pair of opposites.
		*/
		IEnumerable<TPoint> GetPrincipleNeighborDirections();
	}
}