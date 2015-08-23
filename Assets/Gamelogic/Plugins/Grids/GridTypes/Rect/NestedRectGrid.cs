//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gamelogic.Grids
{

	/**
		Represents a composite grid, where each cell contains a grid.

		Big points access the "big" cells (which contain grids).
		Small points access the cells of the grids in the big cells.

		The grid can also be accessed with "normal" points. For example:
		suppose the grid is a 3x3 grid, where each cell is a 2x2 grid.
		Then the "normal" point [7, 2] corresponds to "big" point [3, 1] and 
		"small" point [1, 1].

		@note This class is likely to be redesigned when other nested grids
		are introduced, and could possibly be renamed.

		(This class was indeed renamed in 1.8, and replaces SuperRectGrid 
		that was introduced in 1.6.)

		@version1_8
		@ingroup Grids
	*/
	[Experimental]
	public class NestedRectGrid<TCell> : IGrid<TCell, RectPoint>
	{
		private readonly RectGrid<RectGrid<TCell>> bigGrid;
		private readonly RectPoint bigDimensions;
		private readonly RectPoint smallDimensions;

		/**
			Constructs a new NestedRectGrid.

			@param bigDimensions How wide and high this grid is (how many grids per row and how many grids per column).
			@param smallDimensions How wide and high each small grid is (how many cells in each row and columns).
		*/

		public NestedRectGrid(RectPoint bigDimensions, RectPoint smallDimensions)
		{
			this.bigDimensions = bigDimensions;
			this.smallDimensions = smallDimensions;

			bigGrid = new RectGrid<RectGrid<TCell>>(bigDimensions.X, bigDimensions.Y);

			foreach (var bigPoint in bigGrid)
			{
				bigGrid[bigPoint] = new RectGrid<TCell>(smallDimensions.X, smallDimensions.Y);
			}
		}

		public bool Contains(RectPoint point)
		{
			var bigPoint = GetBigPoint(point);
			var smallPoint = GetSmallPoint(point);

			return bigGrid[bigPoint].Contains(smallPoint);
		}

		/**
			Gets the big point that corresponds to the given normal point. 
		*/

		public RectPoint GetBigPoint(RectPoint point)
		{
			return point.Div(smallDimensions);
		}

		/**
			Gets the small point that corresponds to the given normal point.
		*/

		public RectPoint GetSmallPoint(RectPoint point)
		{
			return point.Mod(smallDimensions);
		}

		/**
			Get the value at the given big point and small point within the cell 
			at the big point.
		*/

		public TCell GetValue(RectPoint bigPoint, RectPoint smallPoint)
		{
			return bigGrid[bigPoint][smallPoint];
		}

		public IEnumerator<RectPoint> GetEnumerator()
		{
			return (
				from bigPoint in bigGrid
				from smallPoint in bigGrid[bigPoint]
				select CombinePoints(bigPoint, smallPoint)).GetEnumerator();
		}

		/**
			Gets the normal point that corresponds with the given big point and small point.
		*/

		public RectPoint CombinePoints(RectPoint bigPoint, RectPoint smallPoint)
		{
			return bigPoint.Mul(smallDimensions) + smallPoint;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/**
			Gets and sets the cell at the given normal point.
		*/
		public TCell this[RectPoint point]
		{
			get
			{
				var bigPoint = GetBigPoint(point);
				var smallPoint = GetSmallPoint(point);

				return GetValue(bigPoint, smallPoint);
			}

			set
			{
				var bigPoint = GetBigPoint(point);
				var smallPoint = GetSmallPoint(point);

				bigGrid[bigPoint][smallPoint] = value;
			}
		}

		object IGrid<RectPoint>.this[RectPoint point]
		{
			get { return this[point]; }
			set { this[point] = (TCell)value; }
		}

		/**
			Gets the cell at the point that corresponds with the given big point and small point.
		*/
		public TCell this[RectPoint bigPoint, RectPoint smallPoint]
		{
			get
			{
				return GetValue(bigPoint, smallPoint);
			}

			set
			{
				bigGrid[bigPoint][smallPoint] = value;
			}
		}

		public IGrid<TNewCell, RectPoint> CloneStructure<TNewCell>()
		{
			return new NestedRectGrid<TNewCell>(bigDimensions, smallDimensions);
		}

		public IEnumerable<RectPoint> GetAllNeighbors(RectPoint point)
		{
			yield return point + RectPoint.North;
			yield return point + RectPoint.East;
			yield return point + RectPoint.South;
			yield return point + RectPoint.West;
		}

		public IEnumerable<TCell> Values
		{
			get { return bigGrid.SelectMany(p => bigGrid[p].Values); }
		}

		IEnumerable IGrid<RectPoint>.Values
		{
			get { return Values; }
		}


		public IGridSpace<RectPoint> BaseGrid
		{
			get
			{
				return bigGrid;
			}
		}

		public RectGrid<TCell> GetSmallGrid(RectPoint bigPoint)
		{
			return bigGrid[bigPoint];
		}

		public IEnumerable<RectPoint> GetLargeSet(int n)
		{
			return bigGrid.GetLargeSet(n);
		}

		public IEnumerable<RectPoint> GetStoragePoints()
		{
			return bigGrid.SelectMany(bigPoint => bigGrid[bigPoint].GetStoragePoints());
		}
	}
}
