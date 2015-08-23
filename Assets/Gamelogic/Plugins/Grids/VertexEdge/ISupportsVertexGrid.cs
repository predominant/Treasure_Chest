//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

namespace Gamelogic.Grids
{
	/**
		Indicates that a grid supports an vertex grid.

		
		
		@version1_1
	*/
	public interface ISupportsVertexGrid<TPoint>
		where TPoint : IGridPoint<TPoint>
	{
		/**
			Makes a grid that corresponds to the vertices of this grid.

			If point is inside this grid, then all of point.GetVertices() 
			are in the grid returned by this method.
		*/
		IGrid<TNewCell, TPoint> MakeVertexGrid<TNewCell>();
	}
}