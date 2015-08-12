// <author>Alexander Murauer</author>
// <email>alexander.murauer@kolmich.at</email>
// <date>2014-03-11</date>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KGFTouch : KGFObject
{
	public enum eSwipeType{
		None,Left,Right,Up,Down
	};
	
	#region parameters
	/// <summary>
	/// Emulate touches with mouse. (to test on pc)
	/// </summary>
	public bool itsEmulateTouchWithMouse = true;
	/// <summary>
	/// Minimal touch movement in pixels, before swipe is recognized
	/// </summary>
	public int itsMinSwipeDistance = 75;
	/// <summary>
	/// Trigger swipe event only on touch release or trigger them instantly after min swipe distance met
	/// </summary>
	public bool itsSwipeWaitForTouchRelease = false;
	/// <summary>
	/// Display debug information
	/// </summary>
	public bool itsDebugMode = false;
	#endregion
	
	#region contstants
	public const int AreaIDFullScreen = -1;
	#endregion
	
	#region internal members
	static Transform itsTempTransform;
	
	Vector2 itsMousePositionOld;
	Vector2 itsMousePosition;
	Dictionary<int,AreaInfo> itsTouchAreas = new Dictionary<int, AreaInfo>();
	int itsTouchCountFullScreen = 0;
	Dictionary<int,int> itsFingerToAreaID = new Dictionary<int, int>();
	Dictionary<int,Vector2> itsSwipeSumByFingerID = new Dictionary<int, Vector2>();
	Dictionary<int,bool> itsSwipeAlreadyDetected = new Dictionary<int, bool>();
	
	class AreaInfo
	{
		public Rect itsRect;
		public int itsTouchCount = 0;
		public int itsPriority = 0;
		public Color itsColor;
		public string itsName = "";
		public bool itsActive = true;
		
		public AreaInfo(Rect theRect, int thePriority, string theName)
		{
			itsRect = theRect;
			itsPriority = thePriority;
			itsColor = new Color(Random.value,Random.value,Random.value);
			itsName = theName;
		}
	}
	
	public class GenericTouch
	{
		public readonly Vector2 itsPosition;
		public readonly Vector2 itsDeltaPosition;
		public readonly int itsAreaID;
		
		public GenericTouch(Vector2 thePosition, Vector2 theDeltaPosition, int theAreaID)
		{
			itsPosition = thePosition;
			itsDeltaPosition = theDeltaPosition;
			itsAreaID = theAreaID;
		}
	}
	#endregion
	
	#region events
	/// <summary>
	/// Event triggered on pinch with 2 fingers
	/// </summary>
	public event System.Action<object,PinchArgs> EventPinch;
	/// <summary>
	/// Event triggered on pan with 1 or 2 fingers
	/// </summary>
	public event System.Action<object,PanArgs> EventPan;
	/// <summary>
	/// Event triggered on touch count changed in area
	/// </summary>
	public event System.Action<object,TouchCountChangedArgs> EventTouchCountChanged;
	/// <summary>
	/// Event triggered on swipe
	/// </summary>
	public event System.Action<object,SwipeArgs> EventSwipe;
	/// <summary>
	/// Event triggered on touch started
	/// </summary>
	public event System.Action<object,TouchArgs> EventTouchStarted;
	/// <summary>
	/// Event triggered on touch moved
	/// </summary>
	public event System.Action<object,TouchArgs> EventTouchMoved;
	/// <summary>
	/// Event triggered on touch ended
	/// </summary>
	public event System.Action<object,TouchArgs> EventTouchEnded;
	
	public class TouchArgs : System.EventArgs
	{
		public readonly int itsAreaID;
		public readonly Vector2 itsPosition;
		public readonly Vector2 itsDelta;
		public readonly int itsFingerID;
		public TouchArgs(int theAreaID,Vector2 thePosition, Vector2 theDelta, int theFingerID)
		{
			itsAreaID = theAreaID;
			itsPosition = thePosition;
			itsDelta = theDelta;
			itsFingerID = theFingerID;
		}
	}
	
	public class PinchArgs : System.EventArgs
	{
		public readonly int itsAreaID;
		public readonly Vector2 itsPointCenter;
		public readonly Vector2 itsLengthOld;
		public readonly Vector2 itsLengthNew;
		public readonly float itsDelta;
		public PinchArgs(int theAreaID,Vector2 thePointCenter, Vector2 theLengthOld, Vector2 theLengthNew, float theDelta)
		{
			itsAreaID = theAreaID;
			itsPointCenter = thePointCenter;
			itsDelta = theDelta;
			itsLengthNew = theLengthNew;
			itsLengthOld = theLengthOld;
		}
	}
	
	public class PanArgs : System.EventArgs
	{
		public readonly int itsAreaID;
		public readonly Vector2 itsDeltaMin;
		public readonly Vector2 itsDeltaMax;
		public readonly Vector2 itsDeltaAvg;
		public readonly Vector2 itsDeltaSum;
		public readonly int itsFingerCount;
		public PanArgs(int theAreaID,params Vector2[] theFingerDiff)
		{
			itsAreaID = theAreaID;
			itsDeltaMin = Vector2.zero;
			itsDeltaMax = Vector2.zero;
			itsDeltaAvg = Vector2.zero;
			itsDeltaSum = Vector2.zero;
			itsFingerCount = theFingerDiff.Length;
			
			if (theFingerDiff.Length > 0)
			{
				// sum
				for (int i=0;i<theFingerDiff.Length;i++)
					itsDeltaSum += theFingerDiff[i];
				
				// avg
				itsDeltaAvg = itsDeltaSum/theFingerDiff.Length;
				
				// min, max
				itsDeltaMin = itsDeltaMax = theFingerDiff[0];
				for (int i=1;i<theFingerDiff.Length;i++)
				{
					if (theFingerDiff[i].magnitude < itsDeltaMin.magnitude)
						itsDeltaMin = theFingerDiff[i];
					if (theFingerDiff[i].magnitude > itsDeltaMax.magnitude)
						itsDeltaMax = theFingerDiff[i];
				}
			}
		}
	}
	
	public class TouchCountChangedArgs : System.EventArgs
	{
		public readonly int itsAreaID;
		public readonly int itsTouchCountOld;
		public readonly int itsTouchCountNew;
		public TouchCountChangedArgs(int theAreaID, int theTouchCountOld, int theTouchCountNew)
		{
			itsAreaID = theAreaID;
			itsTouchCountOld = theTouchCountOld;
			itsTouchCountNew = theTouchCountNew;
		}
	}
	
	public class SwipeArgs : System.EventArgs
	{
		public readonly int itsAreaID;
		public readonly Vector2 itsDelta;
		public readonly eSwipeType itsType;
		public SwipeArgs(int theAreaID,Vector2 theDelta,eSwipeType theType)
		{
			itsAreaID = theAreaID;
			itsDelta = theDelta;
			itsType = theType;
		}
	}
	#endregion
	
	#region event trigger methods
	/// <summary>
	/// Called on touch count changed event
	/// </summary>
	public virtual void OnEventTouchCountChanged(KGFTouch.TouchCountChangedArgs theArgs)
	{
		if (EventTouchCountChanged != null)
			EventTouchCountChanged(this,theArgs);
	}
	
	/// <summary>
	/// Called on swipe event
	/// </summary>
	public virtual void OnEventSwipe(KGFTouch.SwipeArgs theArgs)
	{
		if (EventSwipe != null)
			EventSwipe(this,theArgs);
	}
	
	/// <summary>
	/// Called on pinch event
	/// </summary>
	public virtual void OnEventPinch(KGFTouch.PinchArgs theArgs)
	{
		if (EventPinch != null)
			EventPinch(this,theArgs);
	}
	
	/// <summary>
	/// Called on pan event
	/// </summary>
	public virtual void OnEventPan(KGFTouch.PanArgs theArgs)
	{
		if (EventPan != null)
			EventPan(this,theArgs);
	}
	
	/// <summary>
	/// Called on touch started
	/// </summary>
	public virtual void OnEventTouchStarted(KGFTouch.TouchArgs theArgs)
	{
		if (EventTouchStarted != null)
			EventTouchStarted(this,theArgs);
	}
	
	/// <summary>
	/// Called on touch moved
	/// </summary>
	public virtual void OnEventTouchMoved(KGFTouch.TouchArgs theArgs)
	{
		if (EventTouchMoved != null)
			EventTouchMoved(this,theArgs);
	}
	
	/// <summary>
	/// Called on touch ended
	/// </summary>
	public virtual void OnEventTouchEnded(KGFTouch.TouchArgs theArgs)
	{
		if (EventTouchEnded != null)
			EventTouchEnded(this,theArgs);
	}
	#endregion
	
	#region public interface
	/// <summary>
	/// Do a pinch with a camera.
	/// </summary>
	public static void PinchCameraOrtho(Camera theCamera, PinchArgs theArgs)
	{
		if (itsTempTransform == null)
			itsTempTransform = new GameObject("TempTransform").transform;

		float aFactor = theArgs.itsLengthNew.magnitude / theArgs.itsLengthOld.magnitude;
		itsTempTransform.position = theCamera.ScreenToWorldPoint(new Vector3(Screen.width - theArgs.itsPointCenter.x, Screen.height - theArgs.itsPointCenter.y,10));
		itsTempTransform.localScale = Vector3.one;
		theCamera.transform.parent = itsTempTransform;
		itsTempTransform.transform.localScale *= aFactor;
		theCamera.orthographicSize /= aFactor;
		theCamera.transform.parent = null;
	}

	/// <summary>
	/// Get current touch count
	/// </summary>
	public int GetTouchCount()
	{
		#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
		if (itsEmulateTouchWithMouse)
		{
			if (Input.GetMouseButton(0))
				return 1;
		}
		#endif
		return Input.touchCount;
	}
	
	/// <summary>
	/// Get touch by finger id
	/// </summary>
	public GenericTouch GetTouchByFingerID (int theFingerID)
	{
		#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
		if (itsEmulateTouchWithMouse)
		{
			if (theFingerID == 0)
			{
				if (Input.GetMouseButton(0))
				{
					int anAreaIDMouse = itsFingerToAreaID.ContainsKey(0) ? itsFingerToAreaID[0] : AreaIDFullScreen;
					return new GenericTouch(itsMousePosition, itsMousePosition - itsMousePositionOld, anAreaIDMouse);
				}
			}
		}
		#endif
		for (int i=0;i<Input.touchCount;i++)
		{
			var aTouch = Input.GetTouch(i);
			if (aTouch.fingerId == theFingerID)
			{
				if (aTouch.phase == TouchPhase.Canceled)
					return null;
				if (aTouch.phase == TouchPhase.Ended)
					return null;
				return new GenericTouch(aTouch.position,aTouch.deltaPosition, itsFingerToAreaID[aTouch.fingerId]);
			}
		}
		return null;
	}
	
	/// <summary>
	/// Get touch by internal id. can be iteraded from 0 to touchcount-1
	/// </summary>
	public GenericTouch GetTouch (int theID)
	{
		#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
		if (itsEmulateTouchWithMouse)
		{
			if (theID == 0)
			{
				if (Input.GetMouseButton(0))
				{
					int anAreaIDMouse = itsFingerToAreaID.ContainsKey(0) ? itsFingerToAreaID[0] : AreaIDFullScreen;
					return new GenericTouch(itsMousePosition, itsMousePosition - itsMousePositionOld, anAreaIDMouse);
				}
			}
		}
		#endif
		Touch aTouch = Input.GetTouch(theID);
		return new GenericTouch(aTouch.position,aTouch.deltaPosition, itsFingerToAreaID[aTouch.fingerId]);
	}
	
	/// <summary>
	/// Touch areas can be used to check only a special area for touch activities. Gestures will be checked for each area independently.
	/// If areas overlap then the area with the highest value of priority will be choosen for a touch
	/// </summary>
	public int AddTouchArea(Rect theRect, int thePriority = 0, string theName = "Unnamed")
	{
		// get max id
		int aMaxID = -1;
		foreach (int aKey in itsTouchAreas.Keys)
		{
			if (aKey > aMaxID)
				aMaxID = aKey;
		}
		
		int aNewKey = aMaxID + 1;
		itsTouchAreas[aNewKey] = new AreaInfo(theRect, thePriority,theName);
		
		return aNewKey;
	}

	/// <summary>
	/// Change 2 finger area by id
	/// </summary>
	public void ChangeTouchArea (int theID, Rect theRect)
	{
		if (itsTouchAreas.ContainsKey (theID))
		{
			itsTouchAreas[theID].itsRect = theRect;
		}
	}
	
	/// <summary>
	/// Enable/disable checking for a touch area
	/// </summary>
	public void SetTouchAreaActive(int theID, bool theActive)
	{
		if (itsTouchAreas.ContainsKey(theID))
		{
			itsTouchAreas[theID].itsActive = theActive;
		}
	}
	
	/// <summary>
	/// Get enabled/disabled state of touch area
	/// </summary>
	public bool GetTouchAreaActive(int theID)
	{
		if (itsTouchAreas.ContainsKey(theID))
		{
			return itsTouchAreas[theID].itsActive;
		}
		return false;
	}
	
	/// <summary>
	/// Remove a 2 finger area
	/// </summary>
	public void RemoveTouchArea (int theID)
	{
		if (itsTouchAreas.ContainsKey(theID))
		{
			itsTouchAreas.Remove(theID);
		}
	}
	
	/// <summary>
	/// Get dictionary of all registered areas
	/// </summary>
	public int[] GetRegisteredTouchAreaIDs()
	{
		List<int> aList = new List<int>(itsTouchAreas.Keys);
		return aList.ToArray();
	}
	
	/// <summary>
	/// Get name of registered area
	/// </summary>
	public string GetRegisteredAreaName(int theID)
	{
		if (itsTouchAreas.ContainsKey(theID))
			return itsTouchAreas[theID].itsName;
		return null;
	}
	
	/// <summary>
	/// Get color of registered area
	/// </summary>
	public Color GetRegisteredAreaColor(int theID)
	{
		if (itsTouchAreas.ContainsKey(theID))
			return itsTouchAreas[theID].itsColor;
		return Color.white;
	}
	
	/// <summary>
	/// Check for active touches in area
	/// </summary>
	public int GetTouchCountInArea(Rect theArea)
	{
		int aCount = 0;
		foreach (Touch aTouch in Input.touches)
		{
			if (theArea.Contains(aTouch.position))
			{
				aCount++;
			}
		}
		return aCount;
	}
	
	/// <summary>
	/// Check for active touches that started in area by id
	/// </summary>
	public int GetTouchCountInArea(int theAreaKey)
	{
		int aCount = 0;
		foreach (Touch aTouch in Input.touches)
		{
			if (itsFingerToAreaID.ContainsKey(aTouch.fingerId))
			{
				if (itsFingerToAreaID[aTouch.fingerId] == theAreaKey)
				{
					aCount++;
				}
			}
		}
		return aCount;
	}
	#endregion
	
	#region unity handlers
	protected override void KGFAwake()
	{
		// activate multitouch
		Input.multiTouchEnabled = true;
	}
	
	void Update()
	{
		// save old mouse position
		itsMousePositionOld = itsMousePosition;
		itsMousePosition = Input.mousePosition;
		
		UpdateTouchToAreaID();
		CheckTouches();
		CheckGestures();
		CheckTouchCounts();
		CheckSwipeGestures();
	}
	
	void OnGUI()
	{
		if (itsDebugMode)
		{
			#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
			if (Input.GetMouseButton(0))
			{
				int anAreaID = itsFingerToAreaID.ContainsKey(0) ? itsFingerToAreaID[0] : AreaIDFullScreen;
				GUI.Button(new Rect(Input.mousePosition.x+50,Screen.height-Input.mousePosition.y,100,50),"I=M"+"/F="+0+"/A="+anAreaID);
			}
			#endif

			for (int i=0;i<Input.touchCount;i++)
			{
				Touch aTouch = Input.GetTouch(i);
				int anAreaID = itsFingerToAreaID.ContainsKey(aTouch.fingerId) ? itsFingerToAreaID[aTouch.fingerId] : AreaIDFullScreen;
				GUI.Button(new Rect(aTouch.position.x+50,Screen.height-aTouch.position.y,100,50),"I="+i+"/F="+aTouch.fingerId+"/A="+anAreaID);
			}

			foreach (int anAreaKey in itsTouchAreas.Keys)
			{
				if (!itsTouchAreas[anAreaKey].itsActive)
					continue;
				
				Rect aRect = itsTouchAreas[anAreaKey].itsRect;
				Rect aGUIRect = new Rect(aRect.x,Screen.height - (aRect.y + aRect.height),aRect.width,aRect.height);

				GUI.color = itsTouchAreas[anAreaKey].itsColor;
				GUI.Button(aGUIRect,itsTouchAreas[anAreaKey].itsName);
			}
		}
	}
	#endregion
	
	#region internal methods
	/// <summary>
	/// Check touches for begin, end and move
	/// </summary>
	void CheckTouches()
	{
		#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
		if (itsEmulateTouchWithMouse)
		{
			if (Input.GetMouseButtonDown(0))
			{
				TouchArgs anArgs = new TouchArgs(itsFingerToAreaID[0],itsMousePosition,itsMousePosition-itsMousePositionOld,0);
				OnEventTouchStarted(anArgs);
			}
			else if (Input.GetMouseButtonUp(0))
			{
				TouchArgs anArgs = new TouchArgs(itsFingerToAreaID[0],itsMousePosition,itsMousePosition-itsMousePositionOld,0);
				OnEventTouchEnded(anArgs);
			}
			else if (Input.GetMouseButton(0))
			{
				TouchArgs anArgs = new TouchArgs(itsFingerToAreaID[0],itsMousePosition,itsMousePosition-itsMousePositionOld,0);
				OnEventTouchMoved(anArgs);
			}
		}
		#endif
		
		for (int i=0;i<Input.touchCount;i++)
		{
			Touch aTouch = Input.GetTouch(i);
			TouchArgs anArgs = new TouchArgs(itsFingerToAreaID[aTouch.fingerId],aTouch.position,aTouch.deltaPosition,aTouch.fingerId);
			
			if (aTouch.phase == TouchPhase.Began)
			{
				OnEventTouchStarted(anArgs);
			}
			else if ((aTouch.phase == TouchPhase.Ended) || (aTouch.phase == TouchPhase.Canceled))
			{
				OnEventTouchEnded(anArgs);
			}
			else if (aTouch.phase == TouchPhase.Moved)
			{
				OnEventTouchStarted(anArgs);
			}
		}
	}
	
	/// <summary>
	/// Check for swipe movement
	/// </summary>
	void CheckSwipeGestures()
	{
		if (EventSwipe == null)
			return;
		
		#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
		if (itsEmulateTouchWithMouse)
		{
			if (Input.GetMouseButtonDown(0))
			{
				// init sum on new touch
				itsSwipeSumByFingerID[0] = Vector2.zero;
				itsSwipeAlreadyDetected[0] = false;
			}
			else if (Input.GetMouseButtonUp(0) || !itsSwipeWaitForTouchRelease)
			{
				if (itsSwipeAlreadyDetected.ContainsKey(0))
				{
					if (!itsSwipeAlreadyDetected[0])
					{
						// detect swipe on touch end
						eSwipeType aType = DetectSwipeType(itsSwipeSumByFingerID[0]);
						
						if (aType != eSwipeType.None)
						{
							// don't send swipe events again if already sent for this gesture
							itsSwipeAlreadyDetected[0] = true;
							
							if (itsFingerToAreaID.ContainsKey(0))
								OnEventSwipe(new SwipeArgs(itsFingerToAreaID[0],itsSwipeSumByFingerID[0],aType));
							else
								OnEventSwipe(new SwipeArgs(AreaIDFullScreen,itsSwipeSumByFingerID[0],aType));
							
							// init sum on new touch
							itsSwipeSumByFingerID[0] = Vector2.zero;
						}
					}
				}
			}
			if (Input.GetMouseButton(0))
			{
				// else sum delta
				itsSwipeSumByFingerID[0] += (itsMousePosition - itsMousePositionOld);
			}
		}
		#endif
		
		for (int i=0;i<Input.touchCount;i++)
		{
			Touch aTouch = Input.GetTouch(i);
			if (aTouch.phase == TouchPhase.Began)
			{
				// init sum on new touch
				itsSwipeSumByFingerID[aTouch.fingerId] = Vector2.zero;
				itsSwipeAlreadyDetected[aTouch.fingerId] = false;
			}
			else if ((aTouch.phase == TouchPhase.Ended) || !itsSwipeWaitForTouchRelease)
			{
				if (!itsSwipeAlreadyDetected[aTouch.fingerId])
				{
					// detect swipe on touch end
					eSwipeType aType = DetectSwipeType(itsSwipeSumByFingerID[aTouch.fingerId]);
					
					if (aType != eSwipeType.None)
					{
						// don't send swipe events again if already sent for this gesture
						itsSwipeAlreadyDetected[aTouch.fingerId] = true;
						
						if (itsFingerToAreaID.ContainsKey(aTouch.fingerId))
							OnEventSwipe(new SwipeArgs(itsFingerToAreaID[aTouch.fingerId],itsSwipeSumByFingerID[aTouch.fingerId],aType));
						else
							OnEventSwipe(new SwipeArgs(AreaIDFullScreen,itsSwipeSumByFingerID[aTouch.fingerId],aType));
						
						// init sum on new touch
						itsSwipeSumByFingerID[aTouch.fingerId] = Vector2.zero;
					}
				}
			}
			{
				// else sum delta
				itsSwipeSumByFingerID[aTouch.fingerId] += aTouch.deltaPosition;
			}
		}
	}
	
	/// <summary>
	/// Detect swipe type in a movement
	/// </summary>
	eSwipeType DetectSwipeType(Vector2 theMovement)
	{
		eSwipeType aType = eSwipeType.None;
		if (theMovement.x > itsMinSwipeDistance)
			aType = eSwipeType.Right;
		else if (theMovement.x < -itsMinSwipeDistance)
			aType = eSwipeType.Left;
		else if (theMovement.y > itsMinSwipeDistance)
			aType = eSwipeType.Up;
		else if (theMovement.y < -itsMinSwipeDistance)
			aType = eSwipeType.Down;
		return aType;
	}
	
	/// <summary>
	/// Bind new touches to areas or remove old bindings if touches ended or got canceled
	/// </summary>
	void UpdateTouchToAreaID()
	{
		#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
		if (itsEmulateTouchWithMouse)
		{
			if (Input.GetMouseButtonDown(0))
			{
				// reset area id to fullscreen
				itsFingerToAreaID[0] = AreaIDFullScreen;

				int? aFoundArea = null;

				// search for area
				foreach (int anAreaID in itsTouchAreas.Keys)
				{
					if (!itsTouchAreas[anAreaID].itsActive)
						continue;
					
					if (itsTouchAreas[anAreaID].itsRect.Contains(Input.mousePosition))
					{
						if (aFoundArea.HasValue)
						{
							if (itsTouchAreas[aFoundArea.Value].itsPriority < itsTouchAreas[anAreaID].itsPriority)
								aFoundArea = anAreaID;
						}
						else
						{
							aFoundArea = anAreaID;
						}
					}
				}

				if (aFoundArea.HasValue)
					itsFingerToAreaID[0] = aFoundArea.Value;
			}
		}
		#endif

		for (int i=0;i<Input.touchCount;i++)
		{
			Touch aTouch = Input.GetTouch(i);
			
			// Add new touches
			if (aTouch.phase == TouchPhase.Began)
			{
				// reset area id to fullscreen
				itsFingerToAreaID[aTouch.fingerId] = AreaIDFullScreen;

				int? aFoundArea = null;

				// search for area
				foreach (int anAreaID in itsTouchAreas.Keys)
				{
					if (!itsTouchAreas[anAreaID].itsActive)
						continue;
					
					if (itsTouchAreas[anAreaID].itsRect.Contains(aTouch.position))
					{
						if (aFoundArea.HasValue)
						{
							if (itsTouchAreas[aFoundArea.Value].itsPriority < itsTouchAreas[anAreaID].itsPriority)
								aFoundArea = anAreaID;
						}
						else
						{
							aFoundArea = anAreaID;
						}
					}
				}

				if (aFoundArea.HasValue)
					itsFingerToAreaID[aTouch.fingerId] = aFoundArea.Value;
			}
		}
	}
	
	/// <summary>
	/// Check for touch count changes
	/// </summary>
	void CheckTouchCounts()
	{
		#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
		if (itsEmulateTouchWithMouse)
		{
			if (Input.GetMouseButtonDown(0))
			{
				TouchCountChangedArgs anArgs = new TouchCountChangedArgs(AreaIDFullScreen,itsTouchCountFullScreen,Input.touchCount);
				itsTouchCountFullScreen = 1;
				OnEventTouchCountChanged(anArgs);
			}
			
			if (Input.GetMouseButtonUp(0))
			{
				TouchCountChangedArgs anArgs = new TouchCountChangedArgs(AreaIDFullScreen,itsTouchCountFullScreen,Input.touchCount);
				itsTouchCountFullScreen = 0;
				OnEventTouchCountChanged(anArgs);
			}
		}
		#endif
		
		// check for whole screen
		if (itsTouchCountFullScreen != Input.touchCount)
		{
			TouchCountChangedArgs anArgs = new TouchCountChangedArgs(AreaIDFullScreen,itsTouchCountFullScreen,Input.touchCount);
			itsTouchCountFullScreen = Input.touchCount;
			OnEventTouchCountChanged(anArgs);
		}
		
		// check for each registered area
		foreach (int anAreaKey in itsTouchAreas.Keys)
		{
			int aCountNew = 0;
			if (!itsTouchAreas[anAreaKey].itsActive)
				continue;
			
			#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
			if (itsEmulateTouchWithMouse)
			{
				if (Input.GetMouseButton(0))
				{
					if (itsFingerToAreaID.ContainsKey(0))
					{
						if (itsFingerToAreaID[0] == anAreaKey)
						{
							aCountNew++;
						}
					}
				}
			}
			#endif
			
			for (int i=0;i<Input.touchCount;i++)
			{
				Touch aTouch = Input.GetTouch(i);
//				if (anArea.Contains(aTouch.position))
				if (itsFingerToAreaID.ContainsKey(aTouch.fingerId))
				{
					if (itsFingerToAreaID[aTouch.fingerId] == anAreaKey)
						aCountNew++;
				}
			}
			
			if (aCountNew != itsTouchAreas[anAreaKey].itsTouchCount)
			{
				TouchCountChangedArgs anArgs = new TouchCountChangedArgs(anAreaKey,itsTouchAreas[anAreaKey].itsTouchCount,aCountNew);
				itsTouchAreas[anAreaKey].itsTouchCount = aCountNew;
				OnEventTouchCountChanged(anArgs);
			}
		}
	}
	
	/// <summary>
	/// Check for 2 finger gestures
	/// </summary>
	void CheckGestures()
	{
		List<Touch> aListFoundTouches = new List<Touch>();
		// check whole display
		aListFoundTouches.AddRange(Input.touches);
		CheckGestures(AreaIDFullScreen,aListFoundTouches);
		aListFoundTouches.Clear();
		
		#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
		if (itsEmulateTouchWithMouse)
		{
			if (Input.GetMouseButton(0))
			{
				if (EventPan != null)
					EventPan(this,new PanArgs(AreaIDFullScreen,itsMousePosition - itsMousePositionOld));
			}
		}
		#endif
		
		// check for each registered area
		foreach (int anAreaKey in itsTouchAreas.Keys)
		{
			if (!itsTouchAreas[anAreaKey].itsActive)
				continue;
			
			#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
			if (itsEmulateTouchWithMouse)
			{
				if (Input.GetMouseButton(0))
				{
					if (itsFingerToAreaID.ContainsKey(0))
					{
						if (itsFingerToAreaID[0] == anAreaKey)
						{
							if (EventPan != null)
								EventPan(this,new PanArgs(anAreaKey,itsMousePosition - itsMousePositionOld));
						}
					}
				}
			}
			#endif
			
			for (int i=0;i<Input.touchCount;i++)
			{
				Touch aTouch = Input.GetTouch(i);
				if (itsFingerToAreaID.ContainsKey(aTouch.fingerId))
				{
					if (itsFingerToAreaID[aTouch.fingerId] == anAreaKey)
					{
						aListFoundTouches.Add(aTouch);
					}
				}
			}
			
			CheckGestures(anAreaKey,aListFoundTouches);
			aListFoundTouches.Clear();
		}
	}
	
	/// <summary>
	/// Check for 2 finger gesture in an specified area
	/// </summary>
	void CheckGestures(int theAreaID, List<Touch> theListFoundTouches)
	{
		// check for pinch action (possible with 2 fingers)
		if (theListFoundTouches.Count == 2)
		{
			Vector2 []aPointsOld = new Vector2[]{
				theListFoundTouches[0].position - theListFoundTouches[0].deltaPosition,
				theListFoundTouches[1].position - theListFoundTouches[1].deltaPosition
			};
			Vector2 aVOld = aPointsOld[0] - aPointsOld[1];
			Vector2 aPointsOldCenter = (aPointsOld[0] + aPointsOld[1])/2;
			Vector2 aVNew = theListFoundTouches[0].position - theListFoundTouches[1].position;
			Vector2 aPointsNewCenter = (theListFoundTouches[0].position + theListFoundTouches[1].position)/2;
			Vector2 aPointCenter = (aPointsOldCenter + aPointsNewCenter) / 2;
			float aDistanceOld = aVOld.magnitude;
			float aDistanceNew = aVNew.magnitude;
			float aPinchDistance = (theListFoundTouches[0].deltaPosition - theListFoundTouches[1].deltaPosition).magnitude;
			if (aDistanceNew > aDistanceOld)
				aPinchDistance *= -1;
			
			OnEventPinch(new PinchArgs(theAreaID,aPointCenter,aVOld,aVNew,aPinchDistance));
		}
		// check for pan action (possible if >= 1 touches found)
		if (theListFoundTouches.Count > 0)
		{
			if (EventPan != null)
			{
				List<Vector2> aListDiffs = new List<Vector2>();
				foreach (Touch aTouchFound in theListFoundTouches)
				{
					aListDiffs.Add(aTouchFound.deltaPosition);
				}
				
				OnEventPan(new PanArgs(theAreaID,aListDiffs.ToArray()));
			}
		}
	}
	#endregion
}
