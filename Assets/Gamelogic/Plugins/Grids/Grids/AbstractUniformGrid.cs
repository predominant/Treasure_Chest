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
		This is the base class for grids that are not spliced; in other words, grids 
		that have identical cells, with identical orientation, and in which grid points
		behave as (integer) vectors.

		
		
		@version1_0

		@ingroup Scaffolding
	*/
	[Serializable]
	abstract public class AbstractUniformGrid<TCell, TPoint> : IVectorGrid<TCell, TPoint, TPoint>
		where TPoint : IGridPoint<TPoint>, IVectorPoint<TPoint>
	{
		#region Fields
		private IEnumerable<TPoint> neighborDirections;
		protected int width;
		protected int height;

		[NonSerialized]
		protected Func<TPoint, bool> contains;

		//--- Yes, even subclasses should not touch these, to make sure offsets always work as expected!
		//private TPoint offset;
		private readonly TCell[,] cells;
		
		[NonSerialized]
		private Func<TPoint, TPoint> gridPointTransform;
		private Func<TPoint, TPoint> inverseGridPointTransform;

		//---
		#endregion

		#region Properties
		/**
			Gets the cell at the specified point.
		*/
		public TCell this[TPoint point]
		{
			get
			{
				ArrayPoint aPoint = ArrayPointFromPoint(inverseGridPointTransform(point));

				return cells[aPoint.X, aPoint.Y];
			}

			set
			{
				ArrayPoint aPoint = ArrayPointFromPoint(inverseGridPointTransform(point));

				cells[aPoint.X, aPoint.Y] = value;
			}
		}

		object IGrid<TPoint>.this[TPoint point]
		{
			get { return this[point]; }
			set { this[point] = (TCell)value; }
		}

		/// <summary>
		/// Gives the Zero point as transform by this grids transforms.
		/// </summary>
		abstract protected TPoint GridOrigin
		{
			get;
		}

		public IEnumerable<TCell> Values
		{
			get
			{
				return this.Select(point => this[point]);
			}
		}

		IEnumerable IGrid<TPoint>.Values
		{
			get { return Values; }
		}

		protected Func<TPoint, TPoint> PointTransform
		{
			get
			{
				return gridPointTransform;
			}
		}

		protected Func<TPoint, TPoint> InversePointTransform
		{
			get
			{
				return inverseGridPointTransform;
			}
		}

		public IEnumerable<TPoint> NeighborDirections 
		{
			get { return neighborDirections; }
			set { neighborDirections = value.ToList(); }
		}
		#endregion

		#region Construction
		/**
		Constructs a new hex grid with a shape determined by the IsInsider shape.
		
		\param width
			The with of the hex rectangle that will contain the grid.
		
		\param height
			The height of the hex rectangle that will contain the grid.
		
		\param isInsideTest 
			A function that tests whether a given point is inside 
			the grid. This function should not rely on the points given to it to be limited
			in any way by the specified with and height.
		
		\param gridPointTransform
			Points returned by tis grid are transformed first with this delagate.

		\param inverseGridPointTransform
			This must be the inverse of the gridPointTransform function.
			Together, these functions make it possible to do things such as flip axes.

	*/
		protected AbstractUniformGrid(int width, int height, Func<TPoint, bool> isInsideTest, 
			Func<TPoint, TPoint> gridPointTransform,
			Func<TPoint, TPoint> inverseGridPointTransform, 
			IEnumerable<TPoint> neighborDirections)
		{
#if Trial
			DLLUtil.CheckTrial();
#elif EXPERIMENTAL
			ExperimentalUtil.CreateGUI();
#endif

			this.width = width;
			this.height = height;

			cells = new TCell[width, height];

			contains = isInsideTest;
			SetGridPointTransforms(gridPointTransform, inverseGridPointTransform);

			//Copy the neighbors to prevent external and unexpected manipulation 
			NeighborDirections = neighborDirections;

			//TODO: replace; this is just for now
			//InitNeighbors();
		}

		/**
			\param gridPointTransform
				Points returned by tis grid are transformed first with this delagate.

			\param inverseGridPointTransform
				This must be the inverse of the gridPointTransform function.
				Together, these functions make it possible to do things such as flip axes.
		*/
		public void SetGridPointTransforms(Func<TPoint, TPoint> gridPointTransform, Func<TPoint, TPoint> inverseGridPointTransform)
		{
			this.gridPointTransform = gridPointTransform;
			this.inverseGridPointTransform = inverseGridPointTransform;
		}
		#endregion

		#region Access
		public IEnumerator<TPoint> GetEnumerator()
		{
			for (int aY = 0; aY < height; aY++)
			{
				for (int aX = 0; aX < width; aX++)
				{
					TPoint gridPoint = PointFromArrayPoint(aX, aY);

					if (contains(gridPoint))
					{
						yield return gridPointTransform(gridPoint);
					}
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/**
			This method returns all points that can be contined by
			the storage rectangle for this grid.

			This is useful for debuggong shape functions.

			@version1_1
		*/
		public IEnumerable<TPoint> GetStoragePoints()
		{
			for (int aY = 0; aY < height; aY++)
			{
				for (int aX = 0; aX < width; aX++)
				{
					TPoint gridPoint = PointFromArrayPoint(aX, aY);

					yield return gridPointTransform(gridPoint);
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
			for (int i = -n; i < n; i++)
			{
				for (int j = -n; j < n; j++)
				{
					/*
						Do not worry that array points are negative
						this function is used for debugging, not actual 
						indexing.
					*/
					yield return PointFromArrayPoint(i, j);
				}
			}
		}
		#endregion

		#region Membership
		/**
		Checks whether the given point is inside the grid, or not.
		
		This function must be consistent with the enumerator that is returned
		with GetEnumerator (all points returned by the  enumerator must be Inside,
		and all points that are inside must be returned by the enumerator).
	*/
		public bool Contains(TPoint point)
		{
			return contains(InversePointTransform(point));
		}
		#endregion

		#region Neighbors
		public IEnumerable<TPoint> GetAllNeighbors(TPoint point)
		{
			return from direction in NeighborDirections
				   select point.Translate(direction);
		}

		//The only legal cellIndex to pass to this methid is 0
		public IEnumerable<TPoint> GetNeighborDirections(int cellIndex)
		{
			return NeighborDirections;
		}
		#endregion

		#region Implementation
		abstract public IGrid<TNewCell, TPoint> CloneStructure<TNewCell>();
		abstract protected TPoint PointFromArrayPoint(int aX, int aY);
		abstract protected ArrayPoint ArrayPointFromPoint(int hX, int hY);
		abstract protected ArrayPoint ArrayPointFromPoint(TPoint point);
		#endregion
	}
}
