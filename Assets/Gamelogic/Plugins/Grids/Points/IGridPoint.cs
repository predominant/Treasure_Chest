//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System;

namespace Gamelogic.Grids
{
	public interface IGridPoint
	{
		 
	}

	/**
		Represents a "point" that is used to access a cell in a Grid. 

		For built-in 2D grids, these points are often 2D integer vectors, or spliced vectors, and hence they implement 
		additional interfaces such as IVectorPoint, ISplicedPoint, andISplicedVectorPoint. These points supports 
		arithmetic, [colorings](http://gamelogic.co.za/2013/12/18/what-are-grid-colorings/), and some other geometric operations. 

		In general, points do not "know" their neighbors. Use the grid methods IGrid<TCell, TPoint>.GetNeighbors and 
		IGrid<TCell, TPoint>.GetAllNeighbors to make queries about a point's neighbors.

		@implementers GridPoint base classes must be immutable for many of the algorithms to work correctly. In particular, 
		GridPoints are used as keys in dictionaries and sets.
	
		@implementers It is also a good idea to overload the `==` and `!=` operators.		
		
		@version1_0

		@ingroup Interface
	*/
	public interface IGridPoint<TPoint> : IEquatable<TPoint>, IGridPoint
		where TPoint : IGridPoint<TPoint>
	{
		/**
			The lattice distance between two points.
		
			@implementers Two points should have a distance of 1 if and only if they are neighbors.
		*/
		int DistanceFrom(TPoint other);

		/** 
			For spliced grids, this is the index of the splice.

			For Uniform grids, this is always 0.
		*/
		int SpliceIndex
		{
			get;
		}

		/** 
			For spliced grids, this is the number of slices for all points.

			For Uniform grids, this is always 1.
		*/
		int SpliceCount
		{
			get;
		}
	}
}