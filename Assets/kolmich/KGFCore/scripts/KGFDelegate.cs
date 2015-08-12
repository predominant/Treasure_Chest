// <author>Alexander Murauer</author>
// <email>alexander.murauer@kolmich.at</email>
// <date>2011-07-26</date>
// <summary>Delegate for better event handling in unity. it automatically removes destroyed gameobjects from call list.</summary>

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Use this instead of C# events to ensure no destroyed unity gameobjects are called
/// Delegates of destroyed gameobjects are removed automatically.
/// </summary>
public class KGFDelegate
{
	/// <summary>
	/// internal list of all registered delegates
	/// </summary>
	List<System.Action<object,System.EventArgs>> itsDelegateList = new List<System.Action<object,System.EventArgs>>();
	
	/// <summary>
	/// Operator +, to add new delegates to the list
	/// </summary>
	/// <param name="theMyDelegate"></param>
	/// <param name="theDelegate"></param>
	/// <returns></returns>
	public static KGFDelegate operator+(KGFDelegate theMyDelegate, System.Action<object,System.EventArgs> theDelegate)
	{
		theMyDelegate.itsDelegateList.Add(theDelegate);
		return theMyDelegate;
	}
	
	/// <summary>
	/// Operator -, to remove delegates from list
	/// </summary>
	/// <param name="theMyDelegate"></param>
	/// <param name="theDelegate"></param>
	/// <returns></returns>
	public static KGFDelegate operator-(KGFDelegate theMyDelegate, System.Action<object,System.EventArgs> theDelegate)
	{
		theMyDelegate.itsDelegateList.Remove(theDelegate);
		return theMyDelegate;
	}
	
	/// <summary>
	/// Trigger event
	/// </summary>
	/// <param name="theSender"></param>
	public void Trigger(object theSender)
	{
		Trigger(theSender,null);
	}
	
	/// <summary>
	/// Trigger event and pass args
	/// </summary>
	/// <param name="theSender"></param>
	public void Trigger(object theSender,System.EventArgs theArgs)
	{
		for (int i=itsDelegateList.Count-1;i>=0;i--)
		{
			System.Action<object,System.EventArgs> aDelegate = itsDelegateList[i];
			
			if (aDelegate == null)
			{
				itsDelegateList.RemoveAt(i);
				continue;
			}
			if (aDelegate.Target == null)
			{
				itsDelegateList.RemoveAt(i);
				continue;
			}
			if (aDelegate.Target is MonoBehaviour)
			{
				if (((MonoBehaviour)aDelegate.Target) == null)
				{
					itsDelegateList.RemoveAt(i);
					continue;
				}
			}
			
			aDelegate(theSender,theArgs);
		}
	}
	
	/// <summary>
	/// Remove all registered event handlers
	/// </summary>
	public void Clear()
	{
		itsDelegateList.Clear();
	}
}