// <author>Alexander Murauer</author>
// <email>alexander.murauer@kolmich.at</email>
// <date>2014-02-10</date>

using System.Collections.Generic;
using System.Threading;

namespace KGF.Threading
{
	/// <summary>
	/// Producer/Consumer queue with waiting on dequeue
	/// </summary>
	public class ProducerConsumerQueue<T> where T : class
	{
		Queue<T> itsQueue = new Queue<T>();
		Semaphore itsQueueCounter = null;
		
		/// <summary>
		/// Constructor
		/// </summary>
		public ProducerConsumerQueue(int theMaxItems)
		{
			itsQueueCounter = new Semaphore(0,theMaxItems);
		}
		
		/// <summary>
		/// Enqueue new item
		/// </summary>
		public void Enqueue(T theItem)
		{
			lock(itsQueue)
			{
				itsQueue.Enqueue(theItem);
				itsQueueCounter.Release();
			}
		}
		
		/// <summary>
		/// Dequeue item
		/// </summary>
		public T Dequeue()
		{
			itsQueueCounter.WaitOne();
			
			T anItem = default(T);
			lock(itsQueue)
			{
				anItem = itsQueue.Dequeue();
			}
			return anItem;
		}
		
		/// <summary>
		/// Convert queue to array
		/// </summary>
		/// <returns></returns>
		public T[]ToArray()
		{
			T[] aList = null;
			lock(itsQueue)
			{
				aList = itsQueue.ToArray();
			}
			return aList;
		}
		
		/// <summary>
		/// Clear queue
		/// </summary>
		public void Clear()
		{
			lock(itsQueue)
			{
				itsQueue.Clear();
			}
		}
	}
}