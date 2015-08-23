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
		A general implementation of Wrapped grids, that use arbitrary 
		internal grids and point wrappers.

		A "true point" of a wrapped grid is a point that stays the same after the wrapping.

		- Contains returns true only for true points.
		- GetAllNeighbors return true points (that is,the neighbors are already wrapped)
		- the index operator takes all points, and wrap them before access
		- Iterators iterate over true points

		
		@version1_7
	*/
	public class WrappedGrid<TCell, TPoint> : IGrid<TCell, TPoint> 
		where TPoint:IGridPoint<TPoint>
	{
		private readonly IGrid<TCell, TPoint> grid;
		private readonly IPointWrapper<TPoint> wrapper;

		public WrappedGrid(IGrid<TCell, TPoint> grid, IPointWrapper<TPoint> wrapper)
		{
			this.grid = grid;
			this.wrapper = wrapper;
		}


		public TPoint Wrap(TPoint point)
		{
			return wrapper.Wrap(point);
		}

		/**
			This method returns whether the grid contains the unwrapped point or not.
		*/
		public bool Contains(TPoint point)
		{
			return grid.Contains(point);
		}

		public IEnumerator<TPoint> GetEnumerator()
		{
			return grid.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/**
			Points are wrapped before the queries are performed.
		*/
		public TCell this[TPoint point]
		{
			get { return grid[wrapper.Wrap(point)]; }
			set { grid[wrapper.Wrap(point)] = value; }
		}

		/**
			Points are wrapped before the queries are performed.
		*/
		object IGrid<TPoint>.this[TPoint point]
		{
			get { return this[point]; }
			set { this[point] = (TCell)value; }
		}

		public IGrid<TNewCell, TPoint> CloneStructure<TNewCell>()
		{
			return new WrappedGrid<TNewCell, TPoint>(grid.CloneStructure<TNewCell>(), wrapper);
		}

		/**
			Returns all neighbors in this grid as wrapped points.
		*/
		public IEnumerable<TPoint> GetAllNeighbors(TPoint point)
		{
			return grid.GetAllNeighbors(wrapper.Wrap(point)).Select(p => wrapper.Wrap(p));
		}

		public IEnumerable<TCell> Values
		{
			get { return this.Select(p => this[p]); }
		}

		IEnumerable IGrid<TPoint>.Values
		{
			get { return Values; }
		}

		public IEnumerable<TPoint> GetLargeSet(int n)
		{
			return grid.GetLargeSet(n);
		}

		public IEnumerable<TPoint> GetStoragePoints()
		{
			return grid.GetStoragePoints();
		}
	}
}
