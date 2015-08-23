//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2014 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gamelogic.Grids
{
	/**
		Represents a one dimensional grid; essentially an array that can 
		work with maps.

		LinePoints are automatically convertible to integers.

		@version1_8

		@ingroup Grids
	*/
	[Experimental]
	public partial class LineGrid<TCell> : IVectorGrid<TCell, LinePoint, LinePoint>
	{
		protected static readonly LinePoint[] DefaultNeighborDirections =
		{
			LinePoint.Left, LinePoint.Right
		};

		private readonly LinePoint[] NeighborDirections;

		protected Func<LinePoint, bool> contains;
		private readonly TCell[] cells;
		private Func<LinePoint, LinePoint> gridPointTransform;
		private Func<LinePoint, LinePoint> inverseGridPointTransform;

		protected Func<LinePoint, LinePoint> PointTransform
		{
			get
			{
				return gridPointTransform;
			}
		}

		protected Func<LinePoint, LinePoint> InversePointTransform
		{
			get
			{
				return inverseGridPointTransform;
			}
		}

		public LineGrid(int size) : this(size, x => (0 <= x && x < size), x => x, x => x)
		{
			NeighborDirections = DefaultNeighborDirections;
		}

		//Used by auto code generators
		public LineGrid(int size, int ignored)
			: this(size, x => (0 <= x && x < size), x => x, x => x)
		{ }

		public LineGrid(
			int size,
			Func<LinePoint, bool> isInsideTest,
			Func<LinePoint, LinePoint> gridPointTransform,
			Func<LinePoint, LinePoint> inverseGridPointTransform)
		{
			cells = new TCell[size];
			contains = isInsideTest;
			SetGridPointTransforms(gridPointTransform, inverseGridPointTransform);
		}

		public LineGrid(
			int size,
			int dummy,
			Func<LinePoint, bool> isInsideTest,
			Func<LinePoint, LinePoint> gridPointTransform,
			Func<LinePoint, LinePoint> inverseGridPointTransform,
			IEnumerable<LinePoint> neighborDirections):
			this(size, isInsideTest, gridPointTransform, inverseGridPointTransform)
		{
		}

		/**
			\param gridPointTransform
				Points returned by tis grid are transformed first with this delagate.

			\param inverseGridPointTransform
				This must be the inverse of the gridPointTransform function.
				Together, these functions make it possible to do things such as flip axes.
		*/

		public void SetGridPointTransforms(Func<LinePoint, LinePoint> gridPointTransform,
			Func<LinePoint, LinePoint> inverseGridPointTransform)
		{
			this.gridPointTransform = gridPointTransform;
			this.inverseGridPointTransform = inverseGridPointTransform;
		}

		public bool Contains(LinePoint point)
		{
			return contains(InversePointTransform(point));
		}

		public IEnumerable<LinePoint> GetNeighborDirections(int cellIndex)
		{
			return NeighborDirections;
		}

		public IEnumerator<LinePoint> GetEnumerator()
		{
			for (int i = 0; i < cells.Length; i++)
			{
				if (Contains(i))
				{
					yield return i;
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public TCell this[LinePoint point]
		{
			get
			{
				return cells[inverseGridPointTransform(point)];
			}

			set
			{
				cells[inverseGridPointTransform(point)] = value;
			}
		}

		object IGrid<LinePoint>.this[LinePoint point]
		{
			get { return this[point]; }
			set { this[point] = (TCell) value; }
		}

		public IGrid<TNewCell, LinePoint> CloneStructure<TNewCell>()
		{
			return new LineGrid<TNewCell>(cells.Length, contains, gridPointTransform, inverseGridPointTransform);
		}

		public IEnumerable<LinePoint> GetAllNeighbors(LinePoint point)
		{
			return NeighborDirections.Select<LinePoint, LinePoint>(point.Translate);
		}

		public IEnumerable<TCell> Values
		{
			get { return cells; }
		}

		IEnumerable IGrid<LinePoint>.Values
		{
			get { return Values; }
		}

		public IEnumerable<LinePoint> GetLargeSet(int n)
		{
			for (int i = -n; i <= n; i++)
			{
				yield return i;
			}
		}

		public IEnumerable<LinePoint> GetStoragePoints()
		{
			for (int i = 0; i < cells.Length; i++)
			{
				yield return gridPointTransform(i);
			}
		}

		public static bool DefaultContains(LinePoint point, int width, int height)
		{
			return
				point.X >= 0 &&
				point.Y < width;
		}

		public static LineOp<TCell> BeginShape()
		{
			return new LineOp<TCell>();
		}
	}
}