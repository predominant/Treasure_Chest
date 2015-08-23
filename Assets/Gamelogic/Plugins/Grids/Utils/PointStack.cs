using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace Gamelogic.Grids
{
    /**
		An implementation of a stack for grid points that is 
		safe to use with the AOT compiler.

		@version1_10
		@ingroup Utilities
	*/
    [Serializable]
    public class PointStack<TPoint> : IEnumerable<TPoint>, ICollection, IEnumerable
    {
        private readonly Stack<TPoint> points;

        public int Count
        {
            get { return points.Count; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public PointStack()
        {
            points = new Stack<TPoint>();
        }

        public PointStack(IEnumerable<TPoint> collection)
        {
            points = new Stack<TPoint>(collection);
        }

        public PointStack(int capacity)
        {
            points = new Stack<TPoint>(capacity);
        }

        public IEnumerator<TPoint> GetEnumerator()
        {
            return points.GetEnumerator();
        }

        public TPoint Peek()
        {
            return points.Peek();
        }

        public void Push(TPoint point)
        {
            points.Push(point);
        }

        public TPoint Pop()
        {
            return points.Pop();
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

        public void CopyTo(Array array, int index)
        {
            ((ICollection) points).CopyTo(array, index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) points).GetEnumerator();
        }

        public TPoint[] ToArray()
        {
            return points.ToArray();
        }

        public void TrimExcess()
        {
            points.TrimExcess();
        }
    }
}