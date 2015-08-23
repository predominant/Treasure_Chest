//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if Trial
using GameLogic.DLLUtil;
#endif

namespace Gamelogic.Grids
{
	/**
		A spliced grid represents a simple grid where all cells has been sliced 
		in an equal numbers of cells. 
	
		This class implements the common features of tri and rhomb grids. 
		It is the most probable class to use to define your own grid.
	
		@version1_0

		@ingroup Scaffolding
	*/
	[Serializable]
	abstract public class AbstractSplicedGrid<TCell, TPoint, TBasePoint> : 
		IGrid<TCell, TPoint>
		where TPoint : ISplicedPoint<TPoint, TBasePoint>
		where TBasePoint : IGridPoint<TBasePoint>, IVectorPoint<TBasePoint>
	{
		#region Fields
		protected int width;
		protected int height;
		protected IEnumerable<TPoint>[] neighborDirections;
		
		[NonSerialized]
		protected Func<TPoint, bool> contains;

		private int cellDivisionCount;
		private IGrid<TCell[], TBasePoint> baseGrid;
		private Func<TPoint, TPoint> gridPointTransform;
		private Func<TPoint, TPoint> inverseGridPointTransform;
		#endregion

		#region Properties
		public TCell this[TPoint point]
		{
			get
			{
				var newPoint = InversePointTransform(point);
				return baseGrid[newPoint.BasePoint][newPoint.I];
			}

			set
			{
				var newPoint = InversePointTransform(point);
				baseGrid[newPoint.BasePoint][newPoint.I] = value;
			}
		}

		object IGrid<TPoint>.this[TPoint point]
		{
			get { return this[point]; }
			set { this[point] = (TCell) value; }
		}

		protected Func<TPoint, TPoint> PointTransform
		{
			get { return gridPointTransform; }
		}

		protected Func<TPoint, TPoint> InversePointTransform
		{
			get { return inverseGridPointTransform; }
		}

		public IEnumerable<TCell> Values
		{
			get { return this.Select(point => this[point]); }
		}

		IEnumerable IGrid<TPoint>.Values
		{
			get { return Values; }
		}

		/**
			Gives the Zero point as transform by this grids transforms.
		*/
		abstract protected TPoint GridOrigin
		{
			get;
		}

		public IEnumerable<TPoint>[] NeighborDirections
		{
			get { return neighborDirections; }
			set
			{
				neighborDirections = new IEnumerable<TPoint>[value.Length];

				for (int i = 0; i < value.Length; i++)
				{
					neighborDirections[i] = value[i].ToList();
				}
			}
		}
		#endregion

		#region Abstract interface
		abstract protected IGrid<TCell[], TBasePoint> MakeUnderlyingGrid(int width, int height);
		abstract protected TPoint MakePoint(TBasePoint basePoint, int index);
		#endregion

		#region Constructors
		protected AbstractSplicedGrid(
			int width, 
			int height, 
			int cellDivisionCount, 
			Func<TPoint, bool> isInsideTest, 
			Func<TPoint, TPoint> gridPointTransform, 
			Func<TPoint, TPoint> inverseGridPointTransform,
			IEnumerable<TPoint>[] neighborDirections)
		{

#if Trial
			DLLUtil.CheckTrial();
#endif
			InitFields(width, height, cellDivisionCount);
			NeighborDirections = neighborDirections;

			contains = isInsideTest;
			SetGridPointTransforms(gridPointTransform, inverseGridPointTransform);
		}
		#endregion

		#region Public Methods
		public IEnumerator<TPoint> GetEnumerator()
		{
			foreach (var basePoint in baseGrid)
			{
				for (int i = 0; i < cellDivisionCount; i++)
				{
					var point = MakePoint(basePoint, i);

					if (contains(point))
					{
						yield return PointTransform(point);
					}
				}
			}
		}

		/**
			This method returns all points that can be contained by
			the storage rectangle for this grid.

			This is useful for debugging shape functions.

			@version1_1
		*/
		public IEnumerable<TPoint> GetStoragePoints()
		{
			foreach (var basePoint in baseGrid.GetStoragePoints())
			{
				for (int i = 0; i < cellDivisionCount; i++)
				{
					var point = MakePoint(basePoint, i);
					
					yield return PointTransform(point);
				}
			}
		}

		/**
			This functions returns a large number of points around the origin.

			This is useful (when used with big enough n) to determine 
			whether a grid that is missing points is doing so becuase of
			an incorrect test function, or an incorrect storage rectangle.

			Use for debugging.

			@version1_1
		*/
		public IEnumerable<TPoint> GetLargeSet(int n)
		{
			foreach (var basePoint in baseGrid.GetLargeSet(n))
			{
				for (int i = 0; i < cellDivisionCount; i++)
				{
					yield return MakePoint(basePoint, i);
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool Contains(TPoint point)
		{
			return contains(InversePointTransform(point));
		}

		

		/**
			\param gridPointTransform
				Points returned by tis grid are transformed forst with this delagate.

			\param inverseGridPointTransform
				This must be the inverse of the gridPointTransform function.
				Together, these functions make it possible to do things such as flip axes.
		*/
		public void SetGridPointTransforms(
			Func<TPoint, TPoint> gridPointTransform,
			Func<TPoint, TPoint> inverseGridPointTransform)
		{
			this.gridPointTransform = gridPointTransform;
			this.inverseGridPointTransform = inverseGridPointTransform;
		}
		#endregion

		#region Helpers
		private void InitFields(int width, int height, int cellDivisionCount)
		{
			this.width = width;
			this.height = height;
			this.cellDivisionCount = cellDivisionCount;
	
			baseGrid = MakeUnderlyingGrid(width, height);

			foreach (var point in baseGrid)
			{
				baseGrid[point] = new TCell[cellDivisionCount];
			}
		}
		#endregion

		#region Neighbors
		public IEnumerable<TPoint> GetAllNeighbors(TPoint point)
		{
			return from direction in GetNeighborDirections(point.I)
				   select point.MoveBy(direction);
		}

		public IEnumerable<TPoint> GetNeighborDirections(int cellIndex)
		{
			return neighborDirections[cellIndex];
		}
		#endregion

		#region Implementation
		abstract public IGrid<TNewCell, TPoint> CloneStructure<TNewCell>();
		#endregion
	}
}