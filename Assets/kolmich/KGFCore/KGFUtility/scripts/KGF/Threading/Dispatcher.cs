// <author>Alexander Murauer</author>
// <email>alexander.murauer@kolmich.at</email>
// <date>2014-02-10</date>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KGF.Threading
{
	/// <summary>
	/// Simple delegate dispatcher. It can be used to queue execution of a given code in another thread.
	/// </summary>
	public class Dispatcher
	{
		List<System.Action> itsList = new List<System.Action>();
		
		/// <summary>
		/// Add delegate to the execution queue
		/// </summary>
		public void Dispatch(System.Action theAction)
		{
			// add action to list
			lock(itsList)
			{
				itsList.Add(theAction);
			}
		}
		
		/// <summary>
		/// Has to be called in the thread where the queue should be executed
		/// </summary>
		public void Process()
		{
			while (itsList.Count > 0)
			{
				System.Action anAction = null;
				lock(itsList)
				{
					anAction = itsList[0];
					itsList.RemoveAt(0);
				}
				
				anAction();
			}
		}
	}
}
