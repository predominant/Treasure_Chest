using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gamelogic.Grids
{
	/**
		An implementation of IList for grid points that is 
		safe to use with the AOT compiler.

		@version1_6
		@ingroup Utilities
	*/
	public class PointList<TPoint> : IList<TPoint>
		//where TPoint : IGridPoint<TPoint>
	{
		private readonly List<TPoint> points;

		public PointList()
		{
			points = new List<TPoint>();
		}

		public PointList(IEnumerable<TPoint> collection)
		{
			points = new List<TPoint>(collection);
		}

		public PointList(int capacity)
		{
			points = new List<TPoint>(capacity);
		}
 
		public IEnumerator<TPoint> GetEnumerator()
		{
			return points.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return points.GetEnumerator();
		}

		public void Add(TPoint point)
		{
			points.Add(point);
		}

		public void Clear()
		{
			points.Clear();
		}

		public bool Contains(TPoint point)
		{
			return points.Any(x => x.Equals(point));
		}

		public void CopyTo(TPoint[] array, int arrayIndex)
		{
			points.CopyTo(array, arrayIndex);
		}

		public bool Remove(TPoint point)
		{
			int index = points.FindIndex(x => x.Equals(point));

			if (index >= 0)
			{
				points.RemoveAt(index);
				return true;
			}

			return false;
		}

		public int Count
		{
			get
			{
				return points.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}
		
		public int IndexOf(TPoint point)
		{
			return points.FindIndex(x => x.Equals(point));
		}

		public int LastIndexOf(TPoint point)
		{
			return points.FindLastIndex(x => x.Equals(point));
		}

		public void Insert(int index, TPoint point)
		{
			points.Insert(index, point);
		}

		public void RemoveAt(int index)
		{
			points.RemoveAt(index);
		}

		public TPoint this[int index]
		{
			get
			{
				return points[index];
			}
			set
			{
				points[index] = value;
			}
		}

		/**
			Removes all the elements in the list that does not satisfy the predicate.
		*/
		public void RemoveAllBut(Predicate<TPoint> match)
		{
			Predicate<TPoint> nomatch = point => !match(point);

			points.RemoveAll(nomatch);
		}

		public void RemoveAll(Predicate<TPoint> match)
		{
			for (int i = points.Count - 1; i >= 0; i--)
			{
				if (match(points[i]))
				{
					points.RemoveAt(i);
				}
			}
		}
	}

	/**
		Extensions for IEnumerable.
	*/
	public static class IEnumerableExtensions
	{
		/**
			This method performs the same function as ToList, but returns a PointList instead.
		*/
		public static PointList<TPoint> ToPointList<TPoint>(this IEnumerable<TPoint> list)
			where TPoint : IGridPoint<TPoint>
		{
			return new PointList<TPoint>(list);
		}
	}
}
