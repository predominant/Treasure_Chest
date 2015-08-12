// <copyright>
// Copyright (c) 2010 All Right Reserved, http://www.kolmich.at/
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// </copyright>
// <author>Michal Kolasinski</author>
// <email>michal.kolasinski@kolmich.at</email>
// <date>2010-05-28</date>
// <summary>KGF utils</summary>

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using UnityEngine;

public static class KGFUtility
{
	static System.Random itsRandom = new System.Random();
	
	#region Extension methods for: MonoBehaviour
	/// <summary>
	/// Alternative for GetComponents() if you want to use interfaces
	/// </summary>
	/// <param name="theMonobehaviour"></param>
	/// <returns></returns>
	public static T[] GetComponentsInterface<T>(this MonoBehaviour theMonobehaviour) where T : class
	{
		List<T> aList = new List<T>();
		
		foreach (MonoBehaviour aMonobehaviour in theMonobehaviour.GetComponents<MonoBehaviour>())
		{
			T aT = aMonobehaviour as T;
			if (aT != null)
			{
				aList.Add(aT);
			}
		}
		
		return aList.ToArray();
	}
	
	/// <summary>
	/// Alternative for GetComponent() if you want to use interfaces
	/// </summary>
	/// <param name="theMonobehaviour"></param>
	/// <returns></returns>
	public static T GetComponentInterface<T>(this MonoBehaviour theMonobehaviour) where T : class
	{
		T[] anArray = theMonobehaviour.GetComponentsInterface<T>();
		if (anArray.Length > 0)
			return anArray[0];
		return null;
	}
	#endregion
	
	#region Extension methods for: List<T>
	/// <summary>
	/// Randomize a list
	/// </summary>
	/// <param name="theList"></param>
	public static void Shuffle<T>(this IList<T> theList)
	{
		int aCurrentItem = theList.Count;
		while (aCurrentItem > 1) {
			aCurrentItem--;
			int aRandomItem = itsRandom.Next(aCurrentItem + 1);
			T aValue = theList[aRandomItem];
			theList[aRandomItem] = theList[aCurrentItem];
			theList[aCurrentItem] = aValue;
		}
	}
	
	/// <summary>
	/// Sorted list
	/// </summary>
	/// <param name="theList"></param>
	/// <returns></returns>
	public static List<T> Sorted<T>(this List<T> theList)
	{
		List<T> aList = new List<T>(theList);
		aList.Sort();
		return aList;
	}
	#endregion
	
	#region Extension methods for: IEnumerable
	/// <summary>
	/// Check if item is in collection
	/// </summary>
	/// <param name="theList"></param>
	/// <param name="theNeedle"></param>
	/// <returns></returns>
	public static bool ContainsItem<T>(this IEnumerable<T> theList,T theNeedle) where T : class
	{
		foreach (T anElement in theList)
		{
			if (theNeedle.Equals(anElement))
			{
				return true;
			}
		}
		return false;
	}
	
	/// <summary>
	/// Join to string with separator
	/// </summary>
	/// <param name="theList"></param>
	/// <param name="theSeparator"></param>
	/// <returns></returns>
	public static string JoinToString<T>(this IEnumerable<T> theList,string theSeparator)
	{
		if (theList == null)
			return "";
		List<string> aListStrings = new List<string>();
		foreach (T anElement in theList)
		{
			aListStrings.Add(anElement.ToString());
		}
		return string.Join(theSeparator,aListStrings.ToArray());
	}
	
	/// <summary>
	/// Insert item at position
	/// </summary>
	/// <param name="theList"></param>
	/// <param name="theItem"></param>
	/// <param name="thePosition"></param>
	/// <returns></returns>
	public static IEnumerable<T> InsertItem<T>(this IEnumerable<T> theList,T theItem,int thePosition)
	{
		int i=0;
		bool anInserted = false;
		foreach (T anElement in theList)
		{
			if (i == thePosition)
			{
				yield return theItem;
				anInserted = true;
			}
			yield return anElement;
			i++;
		}
		
		if (!anInserted)
		{
			yield return theItem;
		}
	}
	
	/// <summary>
	/// Append a new item
	/// </summary>
	/// <param name="theList"></param>
	/// <param name="theItem"></param>
	/// <returns></returns>
	public static IEnumerable<T> AppendItem<T>(this IEnumerable<T> theList,T theItem)
	{
		foreach (T anElement in theList)
		{
			yield return anElement;
		}
		yield return theItem;
	}
	
	/// <summary>
	/// Remove doubles from IEnumerable
	/// </summary>
	/// <param name="theList"></param>
	/// <returns></returns>
	public static IEnumerable<T> Distinct<T>(this IEnumerable<T> theList)
	{
		List<T> aDistinctList = new List<T>();
		
		foreach (T anElement in theList)
		{
			if (!aDistinctList.Contains(anElement))
			{
				aDistinctList.Add(anElement);
				yield return anElement;
			}
		}
		
		yield break;
	}
	
	/// <summary>
	/// Only return first list without elements of the second list
	/// </summary>
	/// <param name="theMainList"></param>
	/// <param name="theListToRemove"></param>
	/// <returns></returns>
	public static IEnumerable<T> Removed<T>(this IEnumerable<T> theMainList, params T[] theListToRemove)
	{
		List<T> aListToRemove = new List<T>(theListToRemove);
		
		foreach (T anElement in theMainList)
		{
			if (!aListToRemove.Contains(anElement))
				yield return anElement;
		}
		yield break;
	}
	
	/// <summary>
	/// Sorted list
	/// </summary>
	/// <param name="theList"></param>
	/// <returns></returns>
	public static IEnumerable<T> Sorted<T>(this IEnumerable<T> theList)
	{
		List<T> aList = new List<T>(theList);
		aList.Sort();
		foreach (T aT in aList)
			yield return aT;
		yield break;
	}
	
	/// <summary>
	/// Sorted list
	/// </summary>
	/// <param name="theList"></param>
	/// <returns></returns>
	public static IEnumerable<T> Sorted<T>(this IEnumerable<T> theList, Comparison<T> theComparison)
	{
		List<T> aList = new List<T>(theList);
		aList.Sort(theComparison);
		foreach (T aT in aList)
			yield return aT;
		yield break;
	}
	
	/// <summary>
	/// Convert to generic list
	/// </summary>
	/// <returns></returns>
	public static List<T> ToDynList<T>(this IEnumerable<T> theList)
	{
		return new List<T>(theList);
	}
	#endregion

	#region Extension methods for: Transform
	/// <summary>
	/// Sets the scale recursively
	/// </summary>
	/// <param name="theTransform"></param>
	/// <param name="theScale"></param>
	public static void SetScaleRecursively(this Transform theTransform, Vector3 theScale)
	{
		foreach (Transform aChild in theTransform)
		{
			SetScaleRecursively(aChild,theScale);
		}
		theTransform.localScale = theScale;
	}
	
	/// <summary>
	/// Set position x
	/// </summary>
	public static void SetPX(this Transform theTransform, float theX)
	{
		theTransform.position = new Vector3(
			theX,
			theTransform.position.y,
			theTransform.position.z
		);
	}
	/// <summary>
	/// Set position y
	/// </summary>
	public static void SetPY(this Transform theTransform, float theY)
	{
		theTransform.position = new Vector3(
			theTransform.position.x,
			theY,
			theTransform.position.z
		);
	}
	/// <summary>
	/// Set position z
	/// </summary>
	public static void SetPZ(this Transform theTransform, float theZ)
	{
		theTransform.position = new Vector3(
			theTransform.position.x,
			theTransform.position.y,
			theZ
		);
	}
	
	/// <summary>
	/// Set local position x
	/// </summary>
	public static void SetLPX(this Transform theTransform, float theX)
	{
		theTransform.localPosition = new Vector3(
			theX,
			theTransform.localPosition.y,
			theTransform.localPosition.z
		);
	}
	/// <summary>
	/// Set local position y
	/// </summary>
	public static void SetLPY(this Transform theTransform, float theY)
	{
		theTransform.localPosition = new Vector3(
			theTransform.localPosition.x,
			theY,
			theTransform.localPosition.z
		);
	}
	/// <summary>
	/// Set local position z
	/// </summary>
	public static void SetLPZ(this Transform theTransform, float theZ)
	{
		theTransform.localPosition = new Vector3(
			theTransform.localPosition.x,
			theTransform.localPosition.y,
			theZ
		);
	}
	
	/// <summary>
	/// Set local scale x
	/// </summary>
	public static void SetSX(this Transform theTransform, float theX)
	{
		theTransform.localScale = new Vector3(
			theX,
			theTransform.localScale.y,
			theTransform.localScale.z
		);
	}
	/// <summary>
	/// Set local scale y
	/// </summary>
	public static void SetSY(this Transform theTransform, float theY)
	{
		theTransform.localScale = new Vector3(
			theTransform.localScale.x,
			theY,
			theTransform.localScale.z
		);
	}
	/// <summary>
	/// Set local scale z
	/// </summary>
	public static void SetSZ(this Transform theTransform, float theZ)
	{
		theTransform.localScale = new Vector3(
			theTransform.localScale.x,
			theTransform.localScale.y,
			theZ
		);
	}
	#endregion

	#region Extension methods for: GameObject
	/// <summary>
	/// SetActive for all children without self
	/// </summary>
	public static void SetChildrenActive(this GameObject theGameObject, bool theActive)
	{
		foreach (Transform aChildTransform in theGameObject.transform)
		{
			aChildTransform.gameObject.SetActive(theActive);
		}
	}
	
	/// <summary>
	/// SetActive for all children without self (recursive)
	/// </summary>
	public static void SetChildrenActiveRecursively(this GameObject theGameObject, bool theActive)
	{
		foreach (Transform aChildTransform in theGameObject.transform)
		{
			aChildTransform.gameObject.SetActive(theActive);
			aChildTransform.gameObject.SetChildrenActiveRecursively(theActive);
		}
	}

	/// <summary>
	/// Sets the layer of gameobjects recursively.
	/// </summary>
	public static void SetLayerRecursively(this GameObject theGameObject, int theLayer)
	{
		theGameObject.layer = theLayer;
		foreach(Transform aTransform in theGameObject.transform)
		{
			GameObject aGameObject = aTransform.gameObject;
			aGameObject.SetLayerRecursively(theLayer);
		}
	}
	
	/// <summary>
	/// Sets the layer of all gameobjects recursively.
	/// </summary>
	/// <param name='theLayer'>
	/// The layer.
	/// </param>
	public static void SetLayerRecursively(this GameObject theGameObject, string theLayer)
	{
		theGameObject.SetLayerRecursively(LayerMask.NameToLayer(theLayer));
	}
	#endregion

	#region Extension methods for: DateTime
	/// <summary>
	/// converts a datetime to unix time
	/// </summary>
	/// <param name="theDate"></param>
	/// <returns></returns>
	public static long DateToUnix(this DateTime theDate)
	{
		TimeSpan t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
		return (long)t.TotalSeconds;
	}
	#endregion
	
	#region Extension methods for: string
	/// <summary>
	/// Shorten the string to max length, add '..' if shortened at the end
	/// </summary>
	public static string Shortened(this string theString, int theMaxLength)
	{
		if (theString.Length > theMaxLength)
			return theString.Substring(0,theMaxLength-2)+"..";
		return theString;
	}
	
	/// <summary>
	/// Join an array of string to one string with a separator
	/// This method has the params feature of C# which is not available in string.Join
	/// </summary>
	public static string Join(this string theSeparator, params string[] theItems)
	{
		return string.Join(theSeparator,theItems);
	}
	
	/// <summary>
	/// Joins a IEnummerable of strings to a single string with an separator
	/// </summary>
	public static string Join(this string theSeparator, IEnumerable<string> theItems)
	{
		return string.Join(theSeparator,new List<string>(theItems).ToArray());
	}
	
	/// <summary>
	/// Removes the right most item in a single string of separator divided items
	/// </summary>
	public static string RemoveRight(this string theString, char theSeparator)
	{
		string aStringCopy = ""+theString;
		
		while (aStringCopy.Length > 0 && aStringCopy[aStringCopy.Length-1] != theSeparator)
		{
			aStringCopy = aStringCopy.Remove(aStringCopy.Length-1);
		}
		return aStringCopy;
	}
	
	/// <summary>
	/// Returns the last item of a single string of separator divided items
	/// </summary>
	public static string GetLastPart(this string theString, char theSeparator)
	{
		string[] aSplit = theString.Split(theSeparator);
		return aSplit[aSplit.Length-1];
	}
	#endregion
	
	#region Extension methods for: Quaternion
	/// <summary>
	/// Makes sure all parameters are passed correctly to SetLookRotation, prevents Warning log and performance kill
	/// </summary>
	/// <param name="theUpVector">up vector</param>
	/// <param name="theLookRotation">the look rotation, used if not 0</param>
	/// <param name="theAlternativeLookDirection">the alternative look direction, used if theLookRotation is 0. Must never be 0.</param>
	public static Quaternion SetLookRotationSafe(this Quaternion theQuaternion, Vector3 theUpVector, Vector3 theLookRotation, Vector3 theAlternativeLookDirection)
	{
		if(theAlternativeLookDirection.magnitude == 0.0f)
		{
			throw new Exception("Alternative look vector can never be 0!");
		}
		else
		{
			if (theLookRotation.magnitude != 0.0f)
			{
				theQuaternion.SetLookRotation (theLookRotation, theUpVector);
			}
			else
			{
				theQuaternion.SetLookRotation (theAlternativeLookDirection, theUpVector);
			}
		}
		return theQuaternion;
	}
	#endregion
	
	#region Extension methods for: Color
	public static Color SetA(this Color theColor, float theAlpha)
	{
		theColor.a = theAlpha;
		return theColor;
	}
	#endregion
	
	#region Extension methods for Rect
	/// <summary>
	/// Lerp for rects
	/// </summary>
	public static Rect LerpRect(Rect theFrom, Rect theTo, float theTime)
	{
		return new Rect(
			Mathf.Lerp(theFrom.x,theTo.x,theTime),
			Mathf.Lerp(theFrom.y,theTo.y,theTime),
			Mathf.Lerp(theFrom.width,theTo.width,theTime),
			Mathf.Lerp(theFrom.height,theTo.height,theTime)
		);
	}
	#endregion
	
	#region 3D Stuff
	/// <summary>
	/// Create a plane mesh with given width and height
	/// </summary>
	public static Mesh CreatePlaneMesh(int theWidth,int theHeight)
	{
		Mesh aMesh = new Mesh();
		
		Vector3[] aVertices = new Vector3[(theWidth+1)*(theHeight+1)];
		
		for (int y=0;y<=theHeight;y++)
		{
			for (int x=0;x<=theWidth;x++)
			{
				aVertices[y*(theWidth+1)+x] = new Vector3(x,y,0);
			}
		}
		
		Vector3[] aNormals = new Vector3[aVertices.Length];
		for (int i=0;i<aNormals.Length;i++)
		{
			aNormals[i] = new Vector3(0.0f,0.0f,1.0f);
		}
		
		int[] aTriangles = new int[aVertices.Length * 2 * 3];
		int aTriangleCurrent = 0;
		
		for (int y=0;y<theHeight;y++)
		{
			for (int x=0;x<theWidth;x++)
			{
				int index = y*(theWidth+1) + x;
				
				if (index%2 == 0)
				{
					aTriangles[aTriangleCurrent++] = index;
					aTriangles[aTriangleCurrent++] = index+(theWidth+1);
					aTriangles[aTriangleCurrent++] = index+(theWidth+2);
					
					aTriangles[aTriangleCurrent++] = index;
					aTriangles[aTriangleCurrent++] = index+theWidth+2;
					aTriangles[aTriangleCurrent++] = index+1;
				}else
				{
					aTriangles[aTriangleCurrent++] = index;
					aTriangles[aTriangleCurrent++] = index+(theWidth+1);
					aTriangles[aTriangleCurrent++] = index+1;
					
					aTriangles[aTriangleCurrent++] = index+1;
					aTriangles[aTriangleCurrent++] = index+(theWidth+1);
					aTriangles[aTriangleCurrent++] = index+theWidth+2;
				}
			}
		}
		
		Vector2[] anUVs = new Vector2[aVertices.Length];
		for (int i=0;i<anUVs.Length;i++)
		{
			anUVs[i] = new Vector2(aVertices[i].x / theWidth, aVertices[i].y / theHeight);
		}
		
		aMesh.vertices = aVertices;
		aMesh.normals = aNormals;
		aMesh.uv = anUVs;
		aMesh.triangles = aTriangles;
		
		return aMesh;
	}
	#endregion
	
	#region filesystem stuff
	/// <summary>
	/// Converts a path from platform specific to unity style
	/// </summary>
	public static string ConvertPathToUnity(string thePlatformPath)
	{
		return thePlatformPath.Replace(Path.DirectorySeparatorChar,'/');
	}
	
	/// <summary>
	/// Converts a path from unity style to platform specific
	/// </summary>
	public static string ConvertPathToPlatformSpecific(string theUnityPath)
	{
		return theUnityPath.Replace('/',Path.DirectorySeparatorChar);
	}
	#endregion
	
	#region windows mouse related
	#if UNITY_STANDALONE_WIN
	public struct Point{
		public int X;
		public int Y;
	};
	//--------------------------------------------------
	// GetCursorPos
	[DllImport( "user32.dll" )]
	[return: MarshalAs( UnmanagedType.Bool )]
	public static extern bool GetCursorPos( out Point _point );
	//--------------------------------------------------
	// SetCursorPos
	[DllImport( "user32.dll" )]
	[return: MarshalAs( UnmanagedType.Bool )]
	public static extern bool SetCursorPos( int _x, int _y );
	
	[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true )]
	[return: MarshalAs( UnmanagedType.Bool )]
	public static extern bool ClipCursor( ref RECT rcClip );
	[DllImport("user32.dll" )]
	[return: MarshalAs( UnmanagedType.Bool )]
	public static extern bool GetClipCursor( out RECT rcClip );
	[DllImport("user32.dll" )]
	static extern int GetForegroundWindow( );
	[DllImport("user32.dll")]
	[return: MarshalAs( UnmanagedType.Bool )]
	static extern bool GetWindowRect( int hWnd, ref RECT lpRect );
	
	[StructLayout( LayoutKind.Sequential )]
	public struct RECT
	{
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;
		public RECT( int left, int top, int right, int bottom )
		{
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}
	}
	
	static bool itsMouseRectActive = false;
	static RECT itsOriginalClippingRect;
	#endif
	
	/// <summary>
	/// Confine the mouse to an area on the screen
	/// </summary>
	/// <param name="theRect"></param>
	public static void SetMouseRect(Rect theRect)
	{
		#if UNITY_STANDALONE_WIN
		RECT aClippingRect = new RECT((int)theRect.x,(int)theRect.y,(int)theRect.xMax,(int)theRect.yMax);
		if (itsMouseRectActive)
			ClearMouseRect();
		
		itsOriginalClippingRect = new RECT( );
		GetClipCursor( out itsOriginalClippingRect );
		ClipCursor( ref aClippingRect);
		itsMouseRectActive = true;
		#endif
	}
	
	/// <summary>
	/// Clear the mouse confinement
	/// </summary>
	public static void ClearMouseRect()
	{
		#if UNITY_STANDALONE_WIN
		if (itsMouseRectActive)
		{
			ClipCursor( ref itsOriginalClippingRect );
			itsMouseRectActive = false;
		}
		#endif
	}
	
	/// <summary>
	/// Get the rect of the current foreground window
	/// </summary>
	/// <returns></returns>
	public static Rect GetWindowRect()
	{
		#if UNITY_STANDALONE_WIN
		int aWindowHandle = GetForegroundWindow( );
		RECT aCurrentClippingRect = new RECT();
		
		GetWindowRect( aWindowHandle, ref aCurrentClippingRect );
		return new Rect(aCurrentClippingRect.Left,aCurrentClippingRect.Top,aCurrentClippingRect.Right,aCurrentClippingRect.Bottom);
		#else
		return new Rect(0,0,0,0);
		#endif
	}
	#endregion
	
	#region value lerp stuff
	/// <summary>
	/// Mathf.PingPong with additional ping staytime, pong staytime and transition time
	/// </summary>
	/// <param name="theTime">In most cases this should be Time.time</param>
	/// <param name="theMaxValue">Lerp between 0 and this value</param>
	/// <param name="thePingStayTime">Stay time in seconds on the value 0</param>
	/// <param name="thePongStayTime">Stay time in seconds on the value theMaxValue</param>
	/// <param name="theTransitionTime">Time for transition between 0 and theMaxValue</param>
	/// <returns>current lerped value</returns>
	public static float PingPong(float theTime,float theMaxValue, float thePingStayTime, float thePongStayTime, float theTransitionTime)
	{
		float aTimeSum = thePingStayTime+thePongStayTime+2*theTransitionTime;
		float aNumber = theTime % aTimeSum;
		
		if (aNumber < thePingStayTime)
			return 0;
		if (aNumber < thePingStayTime + theTransitionTime)
			return (aNumber-thePingStayTime)*theMaxValue/theTransitionTime;
		if (aNumber < thePingStayTime + theTransitionTime + thePongStayTime)
			return theMaxValue;
		
		return theMaxValue - ((aNumber - (thePingStayTime + theTransitionTime + thePongStayTime))*theMaxValue/theTransitionTime);
	}
	#endregion
	
	#region 2D stuff
	// should be faster than 2D, but not ready yet
	static Color32[] BlockBlur1D(Color32 []thePixels, int theWidth,int theHeight, int theBlurRadius)
	{
		Color32[] thePixelsResult = new Color32[thePixels.Length];
		
		for (int anY = 0;anY < theHeight;anY++)
		{
			for (int anX = 0;anX < theWidth;anX++)
			{
				int aR,aG,aB;
				aR=aG=aB=0;
//				int aStartX=anX-theBlurRadius>=0?anX-theBlurRadius:0;
//				int aStartY=anY-theBlurRadius>=0?anY-theBlurRadius:0;
				int aPixelCount = 0;
				
				for (int aBlockX=anX-theBlurRadius;aBlockX<=anX+theBlurRadius;aBlockX++)
				{
					Color32 aPixelBlock = thePixels[Mathf.Clamp(aBlockX,0,theWidth-1)+anY*theWidth];
					aR+=aPixelBlock.r;
					aG+=aPixelBlock.g;
					aB+=aPixelBlock.b;
					aPixelCount++;
				}
				
				Color32 aPixel = thePixels[anX+anY*theWidth];
				aPixel.r = (byte)(aR/aPixelCount);
				aPixel.g = (byte)(aG/aPixelCount);
				aPixel.b = (byte)(aB/aPixelCount);
//				thePixelsResult[anY+anX*theHeight] = aPixel;
				thePixelsResult[anX+anY*theWidth] = aPixel;
			}
		}
		
		return thePixelsResult;
	}
	
	/// <summary>
	/// Simple block blur implementation
	/// </summary>
	static Color32[] BlockBlur2D(Color32 []thePixels, int theWidth,int theHeight, int theBlurRadiusX, int theBlurRadiusY)
	{
		Color32[] thePixelsResult = new Color32[thePixels.Length];
		
		for (int anY = 0;anY < theHeight;anY++)
		{
			for (int anX = 0;anX < theWidth;anX++)
			{
				int aR,aG,aB;
				aR=aG=aB=0;
				int aStartX=anX-theBlurRadiusX>=0?anX-theBlurRadiusX:0;
				int aStartY=anY-theBlurRadiusY>=0?anY-theBlurRadiusY:0;
				int aPixelCount = 0;
				for (int aBlockY=aStartY;(aBlockY<theHeight && aBlockY<=anY+theBlurRadiusY);aBlockY++)
				{
					for (int aBlockX=aStartX;(aBlockX<theWidth && aBlockX<=anX+theBlurRadiusX);aBlockX++)
					{
						Color32 aPixelBlock = thePixels[aBlockX+aBlockY*theWidth];
						aR+=aPixelBlock.r;
						aG+=aPixelBlock.g;
						aB+=aPixelBlock.b;
						aPixelCount++;
					}
				}
				Color32 aPixel = thePixels[anX+anY*theWidth];
				aPixel.r = (byte)(aR/aPixelCount);
				aPixel.g = (byte)(aG/aPixelCount);
				aPixel.b = (byte)(aB/aPixelCount);
				thePixelsResult[anX+anY*theWidth] = aPixel;
			}
		}
		
		return thePixelsResult;
	}
	#endregion
	
	#region date related
	/// <summary>
	/// converts a unix time to a c# datetime
	/// </summary>
	public static DateTime DateFromUnix(long theSeconds)
	{
		DateTime aUnixStartTime = new DateTime(1970, 1, 1);
		return aUnixStartTime.AddSeconds(theSeconds);
	}
	#endregion
	
	#region string related
	/// <summary>
	/// Converts byte array to hex string
	/// </summary>
	public static string ToHexString(byte []buffer)
	{
		string aHexString = string.Empty;
		foreach (byte aByte in buffer)
		{
			aHexString += string.Format("{0:x02}",aByte);
		}
		return aHexString;
	}
	#endregion
	
	#region file related
	#if !UNITY_WP8
	/// <summary>
	/// Get MD5 hash sum of file
	/// </summary>
	public static string GetHashMD5OfFile(string theFilePath)
	{
		if (File.Exists(theFilePath))
		{
			MD5CryptoServiceProvider aMD5Provider = new MD5CryptoServiceProvider();
			
			FileStream aFile = File.Open(theFilePath,FileMode.Open);
			byte[] aHash = aMD5Provider.ComputeHash(aFile);
			aFile.Close();
			return ToHexString(aHash);
		}
		return null;
	}
	#endif
	#endregion
	
	#region mail stuff
	#if UNITY_EDITOR
	static string itsSMTP = "smtp.example.com";
	static string itsFrom = "from@example.com";
	static string itsUser = "from";
	static string itsPass = "secret";
	#endif
	
	/// <summary>
	/// Set send mail server settings
	/// </summary>
	public static void SetSettingsMail(string theSMTPServer, string theFrom, string theUser, string thePassword)
	{
		#if UNITY_EDITOR
		itsSMTP = theSMTPServer;
		itsFrom = theFrom;
		itsUser = theUser;
		itsPass = thePassword;
		#endif
	}
	
	/// <summary>
	/// Send a mail
	/// </summary>
	public static void SendMail (string theSubject,string theMessage,params string[] theTo)
	{
		#if UNITY_EDITOR && !UNITY_WEBPLAYER
		
		System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
		for (int i=0;i<theTo.Length;i++)
			message.To.Add(theTo[i]);
		message.Subject = theSubject;
		message.From = new System.Net.Mail.MailAddress(itsFrom);
		message.Body = theMessage;
		message.IsBodyHtml = true;
		System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(itsSMTP);
		smtp.Credentials = new System.Net.NetworkCredential(itsUser, itsPass) as ICredentialsByHost;
		smtp.EnableSsl = true;
		ServicePointManager.ServerCertificateValidationCallback =
			delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{ return true; };
		smtp.Send(message);
		#endif
	}
	#endregion
	
	#region ftp stuff
	public static void DeleteFileFTP(string theHostRemote, string thePathRemote, string theUser="anonymous", string thePassword="test@example.com")
	{
		#if UNITY_EDITOR
		FtpWebRequest requestFileDelete = (FtpWebRequest)WebRequest.Create(string.Format("ftp://{0}/{1}",theHostRemote,thePathRemote));
		requestFileDelete.Credentials = new NetworkCredential(theUser,thePassword);
//		requestFileDelete.Method = WebRequestMethods.Ftp.DeleteFile;
		FtpWebResponse responseFileDelete = (FtpWebResponse)requestFileDelete.GetResponse();
		
		Debug.LogWarning(string.Format("Delete File Complete, status {0}", responseFileDelete.StatusDescription));
		#endif
	}
	
	/// <summary>
	/// Upload a single file to ftp
	/// </summary>
	/// <param name="thePathLocal">full filename path</param>
	/// <param name="theHostRemote">remote hostname</param>
	/// <param name="thePathRemote">remote relative path without filename</param>
	/// <param name="theUser">ftp server login user</param>
	/// <param name="thePassword">ftp server login password</param>
	/// <param name="theProgressUpdateMethod">update method called on each submitted chunk</param>
	/// <param name="theChunkSize">size of read/write buffer</param>
	public static void UploadFileFTP(string thePathLocal, string theHostRemote, string thePathRemote, string theUser="anonymous", string thePassword="test@example.com", Func<float,string,string,bool> theProgressUpdateMethod=null, int theChunkSize=4096)
	{
		#if UNITY_EDITOR && !UNITY_WEBPLAYER
		if (!File.Exists(thePathLocal))
			return;
		
		// Get the object used to communicate with the server.
		string aPathRemote = string.Format("ftp://{0}/{1}",theHostRemote,thePathRemote);
		if (theProgressUpdateMethod != null)
			theProgressUpdateMethod(0,thePathLocal,aPathRemote);
		
		try{
			Debug.LogWarning("Remote:"+aPathRemote);
			FtpWebRequest request = (FtpWebRequest)WebRequest.Create(aPathRemote);
//		request.Method = WebRequestMethods.Ftp.UploadFile;
			request.Credentials = new NetworkCredential (theUser,thePassword);
			
			// Copy the contents of the file to the request stream.
			using (FileStream sourceStream = new FileStream(thePathLocal,FileMode.Open))
			{
				request.ContentLength = sourceStream.Length;
				request.Method = WebRequestMethods.Ftp.UploadFile;
				
				using (Stream requestStream = request.GetRequestStream())
				{
					long aTransmited = 0;
					byte [] fileContents = new byte[theChunkSize];
					while (true)
					{
						int aReadCount = sourceStream.Read(fileContents,0,theChunkSize);
						aTransmited += aReadCount;
						if (aReadCount == 0)
							break;
						
						requestStream.Write(fileContents, 0, aReadCount);
						if (theProgressUpdateMethod != null)
						{
							if (theProgressUpdateMethod(((float)aTransmited) / ((float)sourceStream.Length),thePathLocal,aPathRemote))
								break;
						}
					}
					
					requestStream.Close();
				}
			}
			if (theProgressUpdateMethod != null)
				theProgressUpdateMethod(1,thePathLocal,aPathRemote);
			
			FtpWebResponse response = (FtpWebResponse)request.GetResponse();
			Debug.LogWarning(string.Format("Upload File Complete, status {0}", response.StatusDescription));
			response.Close();
		}
		catch (Exception e)
		{
			Debug.LogError("Error on upload:"+e);
		}
		#endif
	}
	#endregion
	
	#region spline stuff
	/// <summary>
	/// Simple version of catmull rom for float arrays
	/// </summary>
	public static bool CatmullRomEasy(float[] inCoordinates, out float[] outCoordinates, int samples)
	{
		if (inCoordinates.Length < 2)
		{
			outCoordinates = null;
			return false;
		}
		
		if (samples == 0)
		{
			outCoordinates = inCoordinates;
			return true;
		}
		
		List<float> aList = new List<float>(inCoordinates);
		aList.Insert(0,inCoordinates[0]);
		aList.Add(inCoordinates[inCoordinates.Length-1]);
		return CatmullRom(aList.ToArray(),out outCoordinates,samples);
	}
	
	/// <summary>
	/// Simple version of catmull rom for Vector3 arrays
	/// </summary>
	public static bool CatmullRomEasy(Vector3[] inCoordinates, out Vector3[] outCoordinates, int samples, bool theNormalized=false)
	{
		if (inCoordinates.Length < 2)
		{
			outCoordinates = null;
			return false;
		}
		
		if (samples == 0)
		{
			outCoordinates = inCoordinates;
			return true;
		}
		
		List<Vector3> aList = new List<Vector3>(inCoordinates);
		aList.Insert(0,inCoordinates[0]);
		aList.Add(inCoordinates[inCoordinates.Length-1]);
		return CatmullRom(aList.ToArray(),out outCoordinates,samples,theNormalized);
	}
	
	/// <summary>
	/// Takes an array of input coordinates used to define a Catmull-Rom spline, and then
	/// samples the resulting spline according to the specified sample count (per span),
	/// populating the output array with the newly sampled coordinates. The returned boolean
	/// indicates whether the operation was successful (true) or not (false).
	/// NOTE: The first and last points specified are used to describe curvature and will be dropped
	/// from the resulting spline. Duplicate them if you wish to include them in the curve.
	/// </summary>
	static bool CatmullRom(float[] inCoordinates, out float[] outCoordinates, int samples)
	{
		if (inCoordinates.Length < 4)
		{
			outCoordinates = null;
			return false;
		}
		
		if (samples == 0)
		{
			outCoordinates = inCoordinates;
			return true;
		}
		
		List<float> results = new List<float>();
		
		for (int n = 1; n < inCoordinates.Length - 2; n++)
			for (int i = 0; i < samples; i++)
				results.Add(CatmullPointOnCurve(inCoordinates[n - 1], inCoordinates[n], inCoordinates[n + 1], inCoordinates[n + 2], (1f / samples) * i ));
		
		results.Add(inCoordinates[inCoordinates.Length - 2]);
		
		outCoordinates = results.ToArray();
		return true;
	}
	
	/// <summary>
	/// Catmull rom for vector3 arrays
	/// </summary>
	static bool CatmullRom(Vector3[] inCoordinates, out Vector3[] outCoordinates, int samples, bool theNormalized=false)
	{
		if (inCoordinates.Length < 4)
		{
			outCoordinates = null;
			return false;
		}
		
		if (samples == 0)
		{
			outCoordinates = inCoordinates;
			return true;
		}
		
		List<Vector3> results = new List<Vector3>();
		
		for (int n = 1; n < inCoordinates.Length - 2; n++)
			for (int i = 0; i < samples; i++)
				results.Add(CatmullPointOnCurve(inCoordinates[n - 1], inCoordinates[n], inCoordinates[n + 1], inCoordinates[n + 2], (1f / samples) * i ,theNormalized));
		
		results.Add(inCoordinates[inCoordinates.Length - 2]);
		
		outCoordinates = results.ToArray();
		return true;
	}
	
	/// <summary>
	/// Return a point on the curve between P1 and P2 with P0 and P3 describing curvature, at
	/// the normalized distance t.
	/// </summary>
	public static float CatmullPointOnCurve(float p0, float p1, float p2, float p3, float t)
	{
		float result = 0;
		const float aParameter = 0.5f;
		
		float t0 = ((-t + 2f) * t - 1f) * t * aParameter;
		float t1 = (((3f * t - 5f) * t) * t + 2f) * aParameter;
		float t2 = ((-3f * t + 4f) * t + 1f) * t * aParameter;
		float t3 = ((t - 1f) * t * t) * aParameter;
		
		result = p0 * t0 + p1 * t1 + p2 * t2 + p3 * t3;
		
		return result;
	}
	
	/// <summary>
	/// Return a point on the curve between P1 and P2 with P0 and P3 describing curvature, at
	/// the normalized distance t.
	/// </summary>
	public static Vector3 CatmullPointOnCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t, bool theNormalized=false)
	{
//		if (theNormalized)
//		{
//			if (Vector3.Distance(p0,p1) > Vector3.Distance(p1,p2))
//				p0 = p1 + (p0-p1).normalized * Vector3.Distance(p1,p2);
//			if (Vector3.Distance(p2,p3) > Vector3.Distance(p1,p2))
//				p3 = p2 + (p2-p3).normalized * Vector3.Distance(p1,p2);
//		}
		
		var result = new Vector3();
		const float aParameter = 0.5f;
		
		float t0 = ((-t + 2f) * t - 1f) * t * aParameter;
		float t1 = (((3f * t - 5f) * t) * t + 2f) * aParameter;
		float t2 = ((-3f * t + 4f) * t + 1f) * t * aParameter;
		float t3 = ((t - 1f) * t * t) * aParameter;
		
		result.x = p0.x * t0 + p1.x * t1 + p2.x * t2 + p3.x * t3;
		result.y = p0.y * t0 + p1.y * t1 + p2.y * t2 + p3.y * t3;
		result.z = p0.z * t0 + p1.z * t1 + p2.z * t2 + p3.z * t3;
		
		return result;
	}
	
	public static Vector3 Bezier3(Vector3 theP1,Vector3 theP1Help,Vector3 theP2Help,Vector3 theP2,float t)
	{
		return (((-theP1 + 3*(theP1Help-theP2Help) + theP2)* t + (3*(theP1+theP2Help) - 6*theP1Help))* t + 3*(theP1Help-theP1))* t + theP1;
	}
	
	/// <summary>
	/// Creates an additional point in a quadratic spline, defined by the to main points P1 and P2 and the helper point at the given percentage theTime
	/// </summary>
	public static Vector3 QuadraticSplinePoint(Vector3 theP1,Vector3 theP2, Vector3 thePHelper,float theTime)
	{
		Vector3 aC1 = theP1 + (thePHelper - theP1) * theTime;
		Vector3 aC2 = thePHelper + (theP2 - thePHelper) * theTime;
		Vector3 aCM = aC1 + (aC2 - aC1) * theTime;
		return aCM;
	}
	
	/// <summary>
	/// Approximate circle with quadratic spline
	/// </summary>
	public static Vector3 CirclePoint(Vector3 theP1,Vector3 theP2,Vector3 theCenter,float theTime)
	{
		Vector3 aV1 = theP1 - theCenter;
		Vector3 aV2 = theP2 - theCenter;
		// (x-x0)^2 + (y-y0)^2 = r^2
		Vector3 aVNew = Vector3.RotateTowards(aV1,aV2,Vector3.Angle(aV1,aV2) * Mathf.Deg2Rad * theTime,0);
		return theCenter + aVNew;
	}
	#endregion

	#region color stuff
	public static Color RGB_to_HSL(Color c)
	{
		Color h = new Color();
		float R, G, B;
		float lMax, lMin;
		float q;
		R = (c.r*255);
		G = (c.g*255);
		B = (c.b*255);

		if(R>G)
		{
			lMax=R;
			lMin=G;
		}
		else
		{
			lMax=G;
			lMin=R;
		}
		if (B>lMax)
		{
			lMax=B;
		}
		else if (B<lMin)
		{
			lMin=B;
		}

		h.b=(lMax*100.0f/255.0f);

		if (lMax>lMin)
		{
			h.g=((lMax-lMin)*100.0f/lMax);
			q=(60.0f / (lMax-lMin));
			if (lMax==R)
			{
				if (B>G)
				{
					h.r=(q*(G-B) + 360);
				}
				else
				{
					h.r=(q*(G-B));
				}
			}
			else if (lMax==G)
				h.r=(q*(B-R) + 120);
			else if (lMax==B)
				h.r=(q*(R-G) + 240);
		}
		else
		{
			h.g=0;
			h.r=0;
		}

		h.g /= 100f;
		h.b /= 100f;
		return h;
	}

	public static Color HSL_to_RGB(Color h)
	{
		h = new Color(h.r,h.g*100,h.b*100);

		Color c = new Color();
		float Hue, Saturation, Luminance;
		float lMax, lMid, lMin;
		float q;
		Hue = h.r;
		Saturation = h.g;
		Luminance = h.b;

		lMax = (Luminance * 255.0f) / 100.0f;
		lMin = (100.0f - Saturation) * lMax / 100.0f;
		q = (lMax - lMin) / 60.0f;

		if (Hue>=0 && Hue<=60)
		{
			lMid = (Hue - 0) * q + lMin;
			c.r = lMax;
			c.g = lMid;
			c.b = lMin;
		}
		else if (Hue>60 && Hue<=120)
		{
			lMid = -(Hue - 120) * q + lMin;
			c.r = lMid;
			c.g = lMax;
			c.b = lMin;
		}
		else if (Hue>120 && Hue<=180)
		{
			lMid = (Hue - 120) * q + lMin;
			c.r = lMin;
			c.g = lMax;
			c.b = lMid;
		}
		else if (Hue>180 && Hue<=240)
		{
			lMid = -(Hue - 240) * q + lMin;
			c.r = lMin;
			c.g = lMid;
			c.b = lMax;
		}
		else if (Hue>240 && Hue<=300)
		{
			lMid = (Hue - 240) * q + lMin;
			c.r = lMid;
			c.g = lMin;
			c.b = lMax;
		}
		else if (Hue>300)
		{
			lMid = -(Hue - 360) * q + lMin;
			c.r = lMax;
			c.g = lMin;
			c.b = lMid;
		}
		c.r/=255.0f;
		c.g/=255.0f;
		c.b/=255.0f;
		return c;
	}
	/*
	public static Color HSL_to_RGB2(Color h)
	{
		Color c;
		float R,G,B;
		float Hue, Saturation, Luminance;
		float nH, nS, nL;
		float nF, nP, nQ, nT;
		int lH;
		Hue=h.hue;
		Saturation=h.sat;
		Luminance=h.lum;
		if (Saturation > 0)
		{
			nH = Hue / 60.0;
			nL = Luminance / 100.0;
			nS = Saturation / 100.0;
			lH = floor(nH);
			nF = nH - lH;
			nP = nL * (1 - nS);
			nQ = nL * (1 - nS * nF);
			nT = nL * (1 - nS * (1 - nF));
			if (lH==0)
			{
				R = nL * 255;
				G = nT * 255;
				B = nP * 255;
			}
			else if (lH==1)
			{
				R = nQ * 255;
				G = nL * 255;
				B = nP * 255;
			}
			else if (lH==2)
			{
				R = nP * 255;
				G = nL * 255;
				B = nT * 255;
			}
			else if (lH==3)
			{
				R = nP * 255;
				G = nQ * 255;
				B = nL * 255;
			}
			else if (lH==4)
			{
				R = nT * 255;
				G = nP * 255;
				B = nL * 255;
			}
			else if (lH==5)
			{
				R = nL * 255;
				G = nP * 255;
				B = nQ * 255;
			}
		}
		else
		{
			R = (Luminance * 255.0) / 100.0;
			G = R;
			B = R;
		}
		c.r=R/255.0;
		c.g=G/255.0;
		c.b=B/255.0;
		return c;
	}
	*/

//	public static Color GetRGBByIndex (Color theColor, Vector2 theCoordinates)
//	{
//		Color aColor = theColor;
//		
//		aColor = theColor;
//		
//		Color aC2 = RGB_to_HSL(aColor);
//		aC2.g = theCoordinates.x;
//		aC2.b = theCoordinates.y;
//		aColor = HSL_to_RGB(aC2);
//		
//		aColor.a = theColor.a;
//		
//		return aColor;
//	}
	#endregion

	#region mesh stuff
	/// <summary>
	/// Merges meshes with same material.
	/// </summary>
	public static Mesh MergeMeshesSameMaterial(params Mesh [] theListMeshes)
	{
		int aVertexCount = 0;
		int aTriangleCount = 0;
		for (int i = 0; i < theListMeshes.Length; i++)
		{
			aVertexCount += theListMeshes[i].vertexCount;
			aTriangleCount += theListMeshes[i].triangles.Length;
		}

		Vector3 []aListVertices = new Vector3[aVertexCount];
		int []aListTriangles = new int[aTriangleCount];
		Color []aListColors = new Color[aVertexCount];
		Vector2 []aListUV = new Vector2[aVertexCount];

		int aVertexIndex = 0;
		int aTriangleIndex = 0;
		for (int i = 0; i < theListMeshes.Length; i++)
		{
			Mesh aMesh = theListMeshes[i];

			Array.Copy(aMesh.vertices, 0, aListVertices, aVertexIndex, aMesh.vertexCount);
			Array.Copy(aMesh.colors, 0, aListColors, aVertexIndex, aMesh.vertexCount);
			Array.Copy(aMesh.uv, 0, aListUV, aVertexIndex, aMesh.vertexCount);

			//TODO: find faster solution
			int[] aSourceTriangles = aMesh.triangles;
			for (int j = 0; j < aSourceTriangles.Length; j++)
			{
				aListTriangles[aTriangleIndex + j] = aSourceTriangles[j] + aVertexIndex;
			}

			aVertexIndex += aMesh.vertexCount;
			aTriangleIndex += aSourceTriangles.Length;
		}

		Mesh aMeshMerged = new Mesh();
		aMeshMerged.vertices = aListVertices;
		aMeshMerged.triangles = aListTriangles;
		aMeshMerged.colors = aListColors;
		aMeshMerged.uv = aListUV;
		aMeshMerged.Optimize();
		aMeshMerged.RecalculateNormals();
		aMeshMerged.RecalculateBounds();
		return aMeshMerged;
	}
	#endregion
}
