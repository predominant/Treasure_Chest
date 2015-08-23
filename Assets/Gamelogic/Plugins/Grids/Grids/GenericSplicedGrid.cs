//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2014 Gamelogic (Pty) Ltd       //
//----------------------------------------------//
using System.Collections;
using System.Collections.Generic;

using System.Linq;

namespace Gamelogic.Grids
{
	/**
		A point that can be use that access a generic SplicedGrid. It has two components, a base point (which is another grid point), and a splice index.
		
		Note that for these points to be truly immutable, the base point on which they are based must also be immutable.

		@ingroup 1_8		
		@immutable
		
	*/
	[Immutable]
	public struct SplicedPoint<TBasePoint> : IGridPoint<SplicedPoint<TBasePoint>>
		where TBasePoint : IGridPoint<TBasePoint>
	{
		private readonly TBasePoint basePoint;
		private readonly int index;
		private readonly int spliceCount;
		
		public TBasePoint BasePoint 
		{
			get
			{
				return basePoint;
			}
		}

		public int I 
		{
			get
			{
				return index;
			}
		}

		public int SpliceIndex
		{
			get
			{
				return index;
			}
		}

		public int SpliceCount
		{
			get
			{
				return spliceCount;
			}
		}

		public SplicedPoint(TBasePoint basePoint, int index, int spliceCount)
		{
			this.basePoint = basePoint;
			this.index = index;
			this.spliceCount = spliceCount;
		}

		public bool Equals(SplicedPoint<TBasePoint> other)
		{
			return (index == other.index) && basePoint.Equals(other.basePoint);
		}

		/**
			This method is not implemented for generic grids.

			You have to override this method in a base class, and provide
			a suitable implementation for your grid.
		*/
		public int DistanceFrom(SplicedPoint<TBasePoint> other)
		{
			throw new System.NotImplementedException();
		}

		public override string ToString()
		{
			return "[" + basePoint.ToString() + " | " + index + "]";
		}
	}

	/**
		A SplicedGrid is a grid formed from another grid, where each cell is "spliced" in a certain number of cells. For example, a
		triangular grid can be implemented as a diamond grid where each diamond cell is spliced into two triangles.
	*/
	public class SplicedGrid<TCell, TBasePoint> : IGrid<TCell, SplicedPoint<TBasePoint>> where TBasePoint: IGridPoint<TBasePoint>
	{
		private readonly int spliceCount;
		private readonly IGrid<TCell[], TBasePoint> baseGrid;

		/**
			Constructs a new spliced grid from the given base grid, where each cell is divided into
			the number of splices given.

			
		*/
		public SplicedGrid(IGrid<TBasePoint> model, int spliceCount)
		{
			this.spliceCount = spliceCount;

			baseGrid = model.CloneStructure<TCell[]>();

			foreach (var point in baseGrid)
			{
				baseGrid[point] = new TCell[spliceCount];
			}
		}

		public bool Contains(SplicedPoint<TBasePoint> point)
		{
			return baseGrid.Contains(point.BasePoint) && point.I >= 0 && point.I < spliceCount;
		}

		public IEnumerator<SplicedPoint<TBasePoint>> GetEnumerator()
		{
			foreach (var point in baseGrid)
			{
				for (int i = 0; i < spliceCount; i++)
				{
					yield return new SplicedPoint<TBasePoint>(point, i, spliceCount);
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public TCell this[SplicedPoint<TBasePoint> point]
		{
			get
			{
				return baseGrid[point.BasePoint][point.I];
			}

			set
			{
				baseGrid[point.BasePoint][point.I] = value;
			}
		}

		object IGrid<SplicedPoint<TBasePoint>>.this[SplicedPoint<TBasePoint> point]
		{
			get { return this[point]; }
			set { this[point] = (TCell)value; }
		}

		public IGrid<TNewCell, SplicedPoint<TBasePoint>> CloneStructure<TNewCell>()
		{
			return new SplicedGrid<TNewCell, TBasePoint>(baseGrid.CloneStructure<bool>(), spliceCount);
		}

		public IEnumerable<SplicedPoint<TBasePoint>> GetAllNeighbors(SplicedPoint<TBasePoint> point)
		{
			throw new System.NotImplementedException();
		}

		public IEnumerable<TCell> Values
		{
			get
			{
				return this.Select(p => this[p]);
			}
		}

		IEnumerable IGrid<SplicedPoint<TBasePoint>>.Values
		{
			get { return Values; }
		}

		public IEnumerable<SplicedPoint<TBasePoint>> GetLargeSet(int n)
		{
			return 
				from basePoint in baseGrid.GetLargeSet(n)
				from i in Enumerable.Range(0, spliceCount)
				select new SplicedPoint<TBasePoint>(basePoint, i, spliceCount);

		}

		public IEnumerable<SplicedPoint<TBasePoint>> GetStoragePoints()
		{
			return
				from basePoint in baseGrid.GetStoragePoints()
				from i in Enumerable.Range(0, spliceCount)
				select new SplicedPoint<TBasePoint>(basePoint, i, spliceCount);
		}
	}
}