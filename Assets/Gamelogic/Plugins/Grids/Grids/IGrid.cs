//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System.Collections;
using System.Collections.Generic;

namespace Gamelogic.Grids
{
	/**
		A grid interface that is not generic in the cell type. This is useful if you do not 
		care about the cell type, and for implementing casting of grid contents.
		
		@version1_8
	*/
	public interface IGrid<TPoint> : IGridSpace<TPoint>
		where TPoint : IGridPoint<TPoint>
	{
		/**
			Accesses a cell in the given point.
		*/
		object this[TPoint point]
		{
			get;
			set;
		}

		/**
			Returns the neighbors of this point, 
			regardless of whether they are in the grid or not.
		*/
		IEnumerable<TPoint> GetAllNeighbors(TPoint point);

		/**
			Returns a grid with exactly the same structure, but potentially holding
			elements of a different type.
		
		*/
		IGrid<TNewCell, TPoint> CloneStructure<TNewCell>();

		/**
			This functions returns a large number of points around the origin.

			This is useful (when used with big enough n) to determine 
			whether a grid that is missing points is doing so because of
			an incorrect test function, or an incorrect storage rectangle.

			Use for debugging.

			@version1_1
		*/
		IEnumerable<TPoint> GetLargeSet(int n);

		/**
			This method returns all points that can be contained by
			the storage rectangle for this grid.

			This is useful for debugging shape functions.

			@version1_1
		*/
		IEnumerable<TPoint> GetStoragePoints();

		/**
			A enumerable containing all the values of this grid.

			For example, the following two pieces of code do the same:

			@code
			foreach(var point in grid)
			{
				Debug.Log(grid[point]);
			}

			foreach(var value in grid.Values)
			{
				Debug.Log(value);
			}
			@endcode
		*/

		IEnumerable Values
		{
			get;
		}
	}

	/**
		The base class of all types of grids. 
	
		Grids are similar to 2D arrays. Elements in the grid are called _cells_. Grids support random access to cells through 
		grid points (IGridPoint), using square bracket syntax.

			Cell cell = squareGrid[squarePoint];

		Most grids support enumeration of points, making it possible to use [LINQ](http://msdn.microsoft.com/en-us/library/bb397926.aspx)
		on grids as well.

			foreach(var point in grid) doSomethingWithCell(grid[point]);
	
			var pointsThatSatsifyPointPredicate = grid.Where(pointPredicate);

		
		General algorithms are provided in Algorithms.

		If you want to implement your own grid, you can implement this interface to have your grid work with 
		many grid algorithms.
		
		@version1_0

		@ingroup Interface	
	*/
	public interface IGrid<TCell, TPoint> : IGrid<TPoint>
		where TPoint : IGridPoint<TPoint>
	{
		/**
			Accesses a cell in the given point.
		*/
		new TCell this[TPoint point]
		{
			get;
			set;
		}

		/**
			A enumerable containing all the values of this grid.

			For example, the following two pieces of code do the same:

			@code
			foreach(var point in grid)
			{
				Debug.Log(grid[point]);
			}

			foreach(var value in grid.Values)
			{
				Debug.Log(value);
			}
			@endcode
		*/
		new IEnumerable<TCell> Values
		{
			get;
		}
	}
}