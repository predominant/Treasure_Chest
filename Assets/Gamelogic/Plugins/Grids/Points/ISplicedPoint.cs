//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

namespace Gamelogic.Grids
{
	/**
		Interface for working with compound points.

		Spliced points are used for periodic grids where not all
		faces are identical (see AbstractSplicedGrid). 

		Every spliced grid can be seen as a uniform regular grid, 
		where each cell has been divided. Therefore, a spliced point
		consists of a coordinate on the base grid, and an index denoting 
		the particular cell.

		@tparam TPoint the type that implements this interface
		@tparam TBasePoint the type of the refular grid that underlies this grid. For example, 
			a hexagonal grid underlies regular triangular grids

		
		
		@version1_0

		@ingroup Interface
	*/
	public interface ISplicedPoint<TPoint, TBasePoint> :
		IGridPoint<TPoint>,
		ISplicedVectorPoint<TPoint, TBasePoint>

		where TPoint : ISplicedVectorPoint<TPoint, TBasePoint>, IGridPoint<TPoint>
		where TBasePoint : IVectorPoint<TBasePoint>, IGridPoint<TBasePoint>
	{
		/*	
			Why is this public?
				- Convenience
				- Algorithm Design
			Otherwise the user will just make a new 
			basepoint in any case, and perhaps make a mistake.
		*/
		TBasePoint BasePoint { get; }

		/**
			Returns the X-coordinate of this point.

			@version1_2
		*/
		int X { get; }

		/**
			Returns the Y-coordinate of this point.

			@version1_2
		*/
		int Y { get; }

		//What is a better name for this proeprty?
		/**
			Returns the splice idnex for this point.
		*/
		int I { get; }

		TPoint IncIndex(int n);
		TPoint DecIndex(int n);
		TPoint InvertIndex();
	}
}