using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// The KGFOrbitCam class. This class implements an orbiter that can be used to control a unity3d camera.
/// </summary>
public class KGFOrbitCam : KGFObject, KGFIValidator
{
	[System.Serializable]
	public class control_axis
	{
		public enum eMouseButton
		{
			eNone = -10,
			eLeft = 0,
			eRight = 1,
			eMiddle = 2
		}
		
		public string itsAxisName = "";
		public float itsAxisSensitivity = 1.0f;
		public bool itsInvertAxis = false;
		public KeyCode itsAdditionalKey;
		public eMouseButton itsAdditionalMouseButton = eMouseButton.eNone;
		public bool itsResetRotationOnRelease = false;
	}
	
	[System.Serializable]
	public class touch_axis
	{
		public enum eNumberOfTouches
		{
			eNone = 0,
			eOne = 1,
			eTwo = 2,
			eThree
		}
		
		public eNumberOfTouches itsNumberOfTouches = eNumberOfTouches.eNone;
		public float itsSensitivity = 1.0f;
		public bool itsInvertAxis = false;
	}
	
	[System.Serializable]
	public class touch_axis_zoom
	{
		public float itsSensitivity = 1.0f;
		public bool itsInvertAxis = false;
	}
	
	#region data classes
	[System.Serializable]
	public class camera_target_settings
	{
		public GameObject itsTarget;
		public bool itsFollowPosition = true;
		public float itsPositionSpeed = 2.0f;
		public bool itsFollowRotation = true;
		public float itsRotationSpeed = 2.0f;
	}
	
	[System.Serializable]
	public class camera_zoom_settings
	{
		public bool itsEnable = true;
		public float itsStartValue = 50.0f;
		public bool itsUseLimits = true;
		public float itsMinLimit = 30.0f;
		public float itsMaxLimit = 100.0f;
		public float itsZoomSpeed = 2.0f;
		public control_axis[] itsControls;
		public touch_axis_zoom itsControlTouch;
	}
	
	[System.Serializable]
	public class camera_rotation_settings
	{
		[System.Serializable]
		public class up_down_settings
		{
			public bool itsEnable = true;
			public float itsStartValue = 0.0f;
			public bool itsUseLimits = true;
			public float itsUpLimit = 30.0f;
			public float itsDownLimit = 30.0f;
			public control_axis[] itsControls;
			public touch_axis itsControlTouch;
		}

		[System.Serializable]
		public class left_right_settings
		{
			public bool itsEnable = true;
			public float itsStartValue = 0.0f;
			public bool itsUseLimits = true;
			public float itsLeftLimit = 45.0f;
			public float itsRightLimit = 45.0f;
			public control_axis[] itsControls;
			public touch_axis itsControlTouch;
		}

		public left_right_settings itsHorizontal = new left_right_settings();
		public up_down_settings itsVertical = new up_down_settings();
	}
	
	[System.Serializable]
	public class camera_panning_settings
	{
		public Collider itsBounds = null;
		public float itsSpeed = 10.0f;
		public bool itsUseCameraSpace = false;
		
		[System.Serializable]
		public class up_down_settings
		{
			public bool itsEnable = false;
			public control_axis[] itsControls;
			public touch_axis itsControlTouch;
		}
		[System.Serializable]
		public class left_right_settings
		{
			public bool itsEnable = false;
			public control_axis[] itsControls;
			public touch_axis itsControlTouch;
		}
		
		[System.Serializable]
		public class forward_backward_settings
		{
			public bool itsEnable = false;
			public control_axis[] itsControls;
			public touch_axis itsControlTouch;
		}
		public left_right_settings itsLeftRight = new left_right_settings();
		public forward_backward_settings itsForwardBackward= new forward_backward_settings();
		public up_down_settings itsUpDown = new up_down_settings();
	}
	
	[System.Serializable]
	public class camera_terrain_settings
	{
		public bool itsFollowGround;
		public bool itsTestCollisions;
		public LayerMask itsCollisionLayer;
		public float itsCollisionOffset = 0.1f;
		//private bool itsUseRayCollision = true;
		//public bool itsUseBoxCollision = true;
	}
	
	[System.Serializable]
	public class camera_global_settings
	{
		public bool itsSoftLimitsEnable = false;
		public float itsSoftLimitCorrectionSpeed = 1.0f;
	}
	
	[System.Serializable]
	public class camera_lookat_settings
	{
		public bool itsEnable = false;
		public GameObject itsLookatTarget;
		public GameObject itsUpVectorSource;
		public float itsLookatSpeed = 2.0f;
		public bool itsHardLinkToTarget = false;
	}
	
	[System.Serializable]
	public class camera_settings
	{
		[HideInInspector]
		public Camera itsCamera = null;
		public float itsFieldOfView = 60.0f;
	}
	#endregion

	#region public events
	/// <summary>
	/// This event will get triggered immediately
	/// </summary>
	public KGFDelegate EventTargetChanged = new KGFDelegate();
	
	/// <summary>
	/// This event will get triggered if the root changed and the orbit cam reached the new root
	/// If the new root is moving and the orbit cam is not fast enough to follow its new root it may never reach it. So it
	/// is possible that this event will never be triggered while trying to reach the root.
	/// </summary>
	public KGFDelegate EventTargetReached = new KGFDelegate();
	
	/// <summary>
	/// This event gets triggered if an objects starts or stops intersecting the line of sight between orbiter root and the camera.
	/// It can be used for example to make objects transparant that will block the line of sight.
	/// </summary>
	public KGFDelegate EventIntersectorsChanged = new KGFDelegate();
	#endregion
	
	#region public members
	public camera_target_settings itsTarget = new camera_target_settings();
	public camera_zoom_settings itsZoom = new camera_zoom_settings();
	public camera_rotation_settings itsRotation = new camera_rotation_settings();
	public camera_panning_settings itsPanning = new camera_panning_settings();
	public camera_terrain_settings itsEnvironment = new camera_terrain_settings();
	public camera_lookat_settings itsLookat = new camera_lookat_settings();
	public camera_settings itsCamera = new camera_settings();
	public camera_global_settings itsGlobalSettings = new camera_global_settings();
	#endregion

	#region private members

	//lerp velocity values
	private float itsFieldOfViewVelocity;
	private float itsGlobalSettingsRotationLeftRightVelocity;
	private float itsGlobalSettingsRotationUpDownVelocity;
	private Vector3 itsLinkPositionVelocity = new Vector3();
	private Vector3 itsPanningOffsetVelocity = new Vector3();
	private Vector3 itsLinkLookatVelocity = new Vector3();
	private float itsTargetRotationLeftRightVelocityPos;
	private float itsTargetRotationLeftRightVelocityNeg;
	private float itsTargetRotationUpDownVelocityPos;
	private float itsTargetRotationUpDownVelocityNeg;
	private float itsTargetZoomVelocity;
	private Vector3 itsPanningVelocity;
	
	//hidden root object to look at during transitions
	private Vector3 itsCurrentLookatPosition;
	
	//rotation & zoom influenced by input
	private float itsLocalRotationHorizontalInput;
	private float itsLocalRotationVerticalInput;
	
	private float itsCurrentZoom;
	private float itsTargetZoom;
	private float itsCollisionZoom;

	//gyroscope
	private Vector3 itsTargetGroundNormal;
	private Vector3 itsCurrentGroundNormal;

	//link position
	private Vector3 itsCurrentLinkPosition = new Vector3();

	//Quaternions to calculate rotation from euler angles
	private Quaternion itsRotationQuaternion;

	//input rotation
	private float itsLocalRotationHorizontalCurrent;
	private float itsLocalRotationVerticalCurrent;
	
	//input panning
	private Vector3 itsPanningOffset;
	private Vector3 itsCurrentPanningOffset;

	//target link position
	private Vector3 itsTargetStartPosition;
	//target rotation
	private Quaternion itsLinkRotationQuaternion;
	private Quaternion itsGlobalSettingsQuaternion;


	//collider
	private GameObject itsColliderObject;
	private bool itsCollision = false;
	
	
	
	private Transform itsTargetTranform = null;
	
	//this member always stores the old root when it gets changed to a new one.
	private GameObject itsTargetOld;
	private Transform itsTargetOldTransform = null;
	
	/// <summary>
	/// This vector is used to interpolate the up vector when changing between roots.
	/// </summary>
	private Vector3 itsTargetUp;
	
	/// <summary>
	/// Used for lerping the up vector while transiting
	/// </summary>
	private Vector3 itsitsLinkUpVelocity;
	
	
	private Transform itsLookatTransform = null;
	
	private float itsLastCollisionZoom;


	//check if in Editor or game mode for gizmo drawing
	private bool itsEditor = true;

	//normal vectors to add to collision
	private Vector3 itsNormalVector = Vector3.zero;
	private Vector3 itsOriginalPosition;

	private Quaternion itsCurrentLookatRotation;
	private Quaternion itsTargetLookatRotation;
	
	/// <summary>
	/// This member is used to store gameobjects that are blocking the line of sight
	/// </summary>
	private RaycastHit[] itsObjectInLineOfSight = null;
	
	/// <summary>
	/// class for touch interface
	/// </summary>
	
	#if (UNITY_IPHONE || UNITY_ANDROID || UNITY_BLACKBERRY || UNITY_WP8) && !UNITY_EDITOR
	private KGFTouch itsKGFTouch = null;
	#endif

	#endregion

	protected override void KGFAwake()
	{
		base.KGFAwake();
		if(itsLookat.itsLookatTarget != null)
		{
			itsLookatTransform = itsLookat.itsLookatTarget.transform;
		}
		if(itsCamera.itsCamera == null)
		{
			itsCamera.itsCamera = GetComponentInChildren<Camera>();
			itsCamera.itsFieldOfView = itsCamera.itsCamera.fieldOfView;
		}
		
		if(itsTarget.itsTarget != null)
		{
			itsTargetTranform = itsTarget.itsTarget.transform;
		}
	}

	/// <summary>
	/// Default unity3d start method. Used for initialisation
	/// </summary>
	public void Start ()
	{
		#if (UNITY_IPHONE || UNITY_ANDROID || UNITY_BLACKBERRY || UNITY_WP8) && !UNITY_EDITOR
		itsKGFTouch = KGFAccessor.GetObject<KGFTouch>();
		itsKGFTouch.EventPan += OnTouchPan;
		itsKGFTouch.EventPinch += OnTouchPinch;
		#endif
		
		
		itsEditor = false;
		ApplyStartValues();
		if (itsTarget.itsTarget != null)
		{
			SetTarget(itsTarget.itsTarget);
		}
		else
		{
			itsTargetStartPosition = new Vector3 (0, 0, 0);
		}
		if (itsLookat.itsEnable)
		{
			if(itsLookatTransform != null)
			{
				itsCurrentLookatPosition = itsLookatTransform.position;
			}
			if(itsLookat.itsUpVectorSource != null)
			{
				itsTargetLookatRotation = Quaternion.LookRotation ((itsCurrentLookatPosition - transform.position).normalized, itsLookat.itsUpVectorSource.transform.up);
			}
		}
		else
		{
			if(itsTargetTranform != null)
			{
				itsCurrentLookatPosition = itsTargetTranform.position;
				itsTargetLookatRotation = Quaternion.LookRotation ((itsCurrentLookatPosition - transform.position).normalized, itsTargetTranform.up);
			}
		}
		itsCurrentLookatRotation = itsTargetLookatRotation;
		LateUpdate();
		HandleLookatTarget(true);
	}
	
	#if (UNITY_IPHONE || UNITY_ANDROID || UNITY_BLACKBERRY || UNITY_WP8) && !UNITY_EDITOR
	/// <summary>
	/// react on panning
	/// </summary>
	/// <param name="theSender"></param>
	/// <param name="theEventArgs"></param>
	void OnTouchPan(object theSender, EventArgs theEventArgs)
	{
		KGFTouch.PanArgs aPanArgs = theEventArgs as KGFTouch.PanArgs;
		float aDampedInputValue = 1.0f;
		
		if (itsRotation.itsHorizontal.itsEnable && (int)itsRotation.itsHorizontal.itsControlTouch.itsNumberOfTouches == aPanArgs.itsFingerCount)
		{
			float anInvertFactor = 1.0f;
			if(itsRotation.itsHorizontal.itsControlTouch.itsInvertAxis)
				anInvertFactor = -1.0f;
			
			float anInput = (aPanArgs.itsDeltaAvg.x/(float)Screen.width) * 360.0f * itsRotation.itsHorizontal.itsControlTouch.itsSensitivity * anInvertFactor;
			if (itsRotation.itsHorizontal.itsUseLimits)	//make input constant and slow when limits reached
			{
				if (itsLocalRotationHorizontalInput > itsRotation.itsHorizontal.itsStartValue + itsRotation.itsHorizontal.itsLeftLimit && anInput > 0.0f && anInput > aDampedInputValue)
				{
					anInput = aDampedInputValue;
				}
				else if (itsLocalRotationHorizontalInput < itsRotation.itsHorizontal.itsStartValue - itsRotation.itsHorizontal.itsRightLimit && anInput < 0.0f && anInput < -aDampedInputValue)
				{
					anInput = -aDampedInputValue;
				}
			}
			itsLocalRotationHorizontalInput += anInput;
		}
		
		if (itsRotation.itsVertical.itsEnable && (int)itsRotation.itsVertical.itsControlTouch.itsNumberOfTouches == aPanArgs.itsFingerCount)
		{
			float anInvertFactor = 1.0f;
			if(itsRotation.itsVertical.itsControlTouch.itsInvertAxis)
				anInvertFactor = -1.0f;
			float anInput = (aPanArgs.itsDeltaAvg.y/(float)Screen.height) * 360.0f * itsRotation.itsVertical.itsControlTouch.itsSensitivity * anInvertFactor;
			
			if (itsRotation.itsVertical.itsUseLimits)	//make input constant and slow when limits reached
			{
				
				if (itsLocalRotationVerticalInput > itsRotation.itsVertical.itsStartValue + itsRotation.itsVertical.itsUpLimit && anInput > 0.0f && anInput > aDampedInputValue)
				{
					anInput = aDampedInputValue;
				}
				else if (itsLocalRotationVerticalInput < itsRotation.itsVertical.itsStartValue - itsRotation.itsVertical.itsDownLimit && anInput < 0.0f && anInput < -aDampedInputValue)
				{
					anInput = -aDampedInputValue;
				}
			}
			itsLocalRotationVerticalInput += anInput;
		}
		
		//the next few lines of code try to calculate a good start sensitivity for panning based on the camera clipping volume
		float anAngleInRadiants = (itsCamera.itsFieldOfView/2.0f)*Mathf.Deg2Rad;
		float aHalfClipDistance = Mathf.Abs(itsCamera.itsCamera.farClipPlane-itsCamera.itsCamera.nearClipPlane)/2.0f;
		float aHeight = Mathf.Tan(anAngleInRadiants)*aHalfClipDistance;
		float aWidth = aHeight*itsCamera.itsCamera.aspect;
		
		if(itsPanning.itsLeftRight.itsEnable && (int)itsPanning.itsLeftRight.itsControlTouch.itsNumberOfTouches == aPanArgs.itsFingerCount)
		{
			float anInvertFactor = 1.0f;
			if(itsPanning.itsLeftRight.itsControlTouch.itsInvertAxis)
				anInvertFactor = -1.0f;
			
			float aLeftRightInput = (aPanArgs.itsDeltaAvg.x/(float)Screen.width) * aWidth * itsPanning.itsLeftRight.itsControlTouch.itsSensitivity * anInvertFactor;
			Vector3 aDelta= transform.right.normalized * aLeftRightInput;
			itsPanningOffset += aDelta;
		}
		
		if(itsPanning.itsForwardBackward.itsEnable && (int)itsPanning.itsForwardBackward.itsControlTouch.itsNumberOfTouches == aPanArgs.itsFingerCount)
		{
			float anInvertFactor = 1.0f;
			if(itsPanning.itsForwardBackward.itsControlTouch.itsInvertAxis)
				anInvertFactor = -1.0f;
			float aForwardBackwardInput = (aPanArgs.itsDeltaAvg.y/(float)Screen.height) * aHeight* itsPanning.itsForwardBackward.itsControlTouch.itsSensitivity * anInvertFactor;
			Vector3 aDelta= transform.forward.normalized;
			if(!itsPanning.itsUseCameraSpace)
			{
				aDelta.y = 0.0f;
				aDelta.Normalize();
			}
			aDelta *= aForwardBackwardInput;
			itsPanningOffset += aDelta;
		}

		if(itsPanning.itsUpDown.itsEnable && (int)itsPanning.itsUpDown.itsControlTouch.itsNumberOfTouches == aPanArgs.itsFingerCount)
		{
			float anInvertFactor = 1.0f;
			if(itsPanning.itsUpDown.itsControlTouch.itsInvertAxis)
				anInvertFactor = -1.0f;
			float anUpDownInput = (aPanArgs.itsDeltaAvg.y/(float)Screen.height) * aHeight * itsPanning.itsUpDown.itsControlTouch.itsSensitivity * anInvertFactor;
			Vector3 aDelta= transform.up.normalized;
			if(!itsPanning.itsUseCameraSpace)
			{
				aDelta.z = 0.0f;
				aDelta.Normalize();
			}
			aDelta *= anUpDownInput;
			itsPanningOffset += aDelta;
		}
		
		DampPanningDirection();
	}
	
	/// <summary>
	/// react on pinch
	/// </summary>
	/// <param name="theSender"></param>
	/// <param name="theEventArgs"></param>
	void OnTouchPinch(object theSender, EventArgs theEventArgs)
	{
		KGFTouch.PinchArgs aPinchArgs = theEventArgs as KGFTouch.PinchArgs;
		if(itsZoom.itsEnable)
		{
			float anInvertFactor = 1.0f;
			if(itsZoom.itsControlTouch.itsInvertAxis)
				anInvertFactor = -1.0f;
			float aTotalPossibleZoomDistance = Mathf.Abs(itsZoom.itsMinLimit - itsZoom.itsMaxLimit);
			
			float aDelta = aPinchArgs.itsDelta;
			
//			Debug.LogError("aZoomDelta: "+aDelta);
			
			itsTargetZoom += (aDelta/((float)Screen.height/2.0f)) * itsZoom.itsControlTouch.itsSensitivity * aTotalPossibleZoomDistance * anInvertFactor;
			HandleZoomLimits();
		}
	}
	
	#endif
	
	private void HandleZoomLimits()
	{
		if (itsZoom.itsUseLimits)
		{
			if (itsTargetZoom > itsZoom.itsMaxLimit)
			{
				float aDifference = Math.Abs(itsTargetZoom - itsZoom.itsMaxLimit);
				itsTargetZoom = Mathf.SmoothDamp(itsTargetZoom, itsZoom.itsMaxLimit, ref itsTargetZoomVelocity,0.1f/aDifference);
			}
			if (itsTargetZoom < itsZoom.itsMinLimit)
			{
				float aDifference = Math.Abs(itsTargetZoom - itsZoom.itsMinLimit);
				itsTargetZoom = Mathf.SmoothDamp(itsTargetZoom, itsZoom.itsMinLimit, ref itsTargetZoomVelocity,0.1f/aDifference);
			}
		}
	}
	
	/// <summary>
	/// sets the horizontal rotation input value
	/// </summary>
	/// <param name="theHorizontalRotation"></param>
	private void SetLocalRotationHorizontalInput(float theHorizontalRotation)
	{
		itsLocalRotationHorizontalInput = theHorizontalRotation;
	}
	
	/// <summary>
	/// sets the vertical rotation input value
	/// </summary>
	/// <param name="theHorizontalRotation"></param>
	private void SetLocalRotationVerticalInput(float theVerticalRotation)
	{
		itsLocalRotationVerticalInput = theVerticalRotation;
	}
	
	private void ApplyStartValues()
	{
		//initialize rotation and zoom, set target to start values
		itsLocalRotationHorizontalCurrent = itsRotation.itsHorizontal.itsStartValue;
		SetLocalRotationHorizontalInput(itsRotation.itsHorizontal.itsStartValue);
		itsLocalRotationVerticalCurrent = itsRotation.itsVertical.itsStartValue;
		SetLocalRotationVerticalInput(itsRotation.itsVertical.itsStartValue);
		
		itsPanningOffset = Vector3.zero;
		itsCurrentPanningOffset = itsPanningOffset;
		
		itsCurrentZoom = itsZoom.itsStartValue;
		itsTargetZoom = itsZoom.itsStartValue;
		itsCollisionZoom = itsZoom.itsStartValue;
		if(itsTarget.itsTarget != null)
		{
			itsCurrentLinkPosition = itsTarget.itsTarget.transform.position;
		}
		
		if(itsCamera.itsCamera != null)
		{
			if(Application.isPlaying)
				itsCamera.itsFieldOfView = itsCamera.itsCamera.fieldOfView;
			else
				itsCamera.itsCamera.fieldOfView = itsCamera.itsFieldOfView;	//in editor mode show target fov immediately.
		}
	}
	
	/// <summary>
	/// This method is checking if the new root target is reached by the orbiter.
	/// If so the old root is set to the current root and the event for reaching the new root is triggered.
	/// </summary>
	private void CheckDistanceToRoot()
	{
		if(itsTarget.itsTarget == null)
			return;
		if(Vector3.Distance(itsCurrentLinkPosition,itsTarget.itsTarget.transform.position) < 0.01f && itsTargetOld != itsTarget.itsTarget)
		{
			itsTargetOld = itsTarget.itsTarget;
			itsTargetOldTransform = itsTargetOld.transform;
			if(EventTargetReached != null)
			{
				EventTargetReached.Trigger(this);
			}
		}
	}
	
	public bool GetTargetIsChanging()
	{
		return (itsTargetOld != itsTarget.itsTarget);
	}
	
	/// <summary>
	/// This methods checks if the input is not overshooting the given limits.
	/// If so there is a smoothdamping enabled that will correct the overshoot again.
	/// </summary>
	private void HandleInputLimits()
	{
		//prevent overshoot
		if (itsRotation.itsHorizontal.itsUseLimits)
		{
			if (itsLocalRotationHorizontalInput > itsRotation.itsHorizontal.itsStartValue + itsRotation.itsHorizontal.itsLeftLimit)
			{
				if(itsGlobalSettings.itsSoftLimitsEnable)
				{
					float aDifference = Math.Abs(itsLocalRotationHorizontalInput - (itsRotation.itsHorizontal.itsStartValue + itsRotation.itsHorizontal.itsLeftLimit));
					itsLocalRotationHorizontalInput = Mathf.SmoothDamp(itsLocalRotationHorizontalInput, itsRotation.itsHorizontal.itsStartValue + itsRotation.itsHorizontal.itsLeftLimit, ref itsTargetRotationLeftRightVelocityPos,1.0f/(itsGlobalSettings.itsSoftLimitCorrectionSpeed*aDifference));
				}
				else
				{
					itsLocalRotationHorizontalInput = itsRotation.itsHorizontal.itsStartValue + itsRotation.itsHorizontal.itsLeftLimit;
				}
			}
			if (itsLocalRotationHorizontalInput < itsRotation.itsHorizontal.itsStartValue - itsRotation.itsHorizontal.itsRightLimit)
			{
				if(itsGlobalSettings.itsSoftLimitsEnable)
				{
					float aDifference = Math.Abs(itsLocalRotationHorizontalInput - (itsRotation.itsHorizontal.itsStartValue - itsRotation.itsHorizontal.itsRightLimit));
					itsLocalRotationHorizontalInput = Mathf.SmoothDamp(itsLocalRotationHorizontalInput, itsRotation.itsHorizontal.itsStartValue - itsRotation.itsHorizontal.itsRightLimit, ref itsTargetRotationLeftRightVelocityNeg,1.0f/(itsGlobalSettings.itsSoftLimitCorrectionSpeed*aDifference));
				}
				else
				{
					itsLocalRotationHorizontalInput = itsRotation.itsHorizontal.itsStartValue - itsRotation.itsHorizontal.itsRightLimit;
				}
			}
		}
		if (itsRotation.itsVertical.itsUseLimits)
		{
			if (itsLocalRotationVerticalInput > itsRotation.itsVertical.itsStartValue + itsRotation.itsVertical.itsUpLimit)
			{
				if(itsGlobalSettings.itsSoftLimitsEnable)
				{
					float aDifference = Math.Abs(itsLocalRotationVerticalInput - (itsRotation.itsVertical.itsStartValue + itsRotation.itsVertical.itsUpLimit));
					itsLocalRotationVerticalInput = Mathf.SmoothDamp(itsLocalRotationVerticalInput, itsRotation.itsVertical.itsStartValue + itsRotation.itsVertical.itsUpLimit, ref itsTargetRotationUpDownVelocityPos,1.0f/(itsGlobalSettings.itsSoftLimitCorrectionSpeed*aDifference));
				}
				else
				{
					itsLocalRotationVerticalInput = itsRotation.itsVertical.itsStartValue + itsRotation.itsVertical.itsUpLimit;
				}
			}
			if (itsLocalRotationVerticalInput < itsRotation.itsVertical.itsStartValue - itsRotation.itsVertical.itsDownLimit)
			{
				if(itsGlobalSettings.itsSoftLimitsEnable)
				{
					float aDifference = Math.Abs(itsLocalRotationVerticalInput - (itsRotation.itsVertical.itsStartValue - itsRotation.itsVertical.itsDownLimit));
					itsLocalRotationVerticalInput = Mathf.SmoothDamp(itsLocalRotationVerticalInput, itsRotation.itsVertical.itsStartValue - itsRotation.itsVertical.itsDownLimit, ref itsTargetRotationUpDownVelocityNeg,1.0f/(itsGlobalSettings.itsSoftLimitCorrectionSpeed*aDifference));
				}
				else
				{
					itsLocalRotationVerticalInput = itsRotation.itsVertical.itsStartValue - itsRotation.itsVertical.itsDownLimit;
				}
			}
		}
		HandleZoomLimits();
	}
	
	/// <summary>
	/// This method is doing all the transformation calculations
	/// </summary>
	public virtual void LateUpdate()
	{
		CheckDistanceToRoot();
		#if (!UNITY_IPHONE && !UNITY_ANDROID && !UNITY_BLACKBERRY && !UNITY_WP8) || UNITY_EDITOR
		ReactToInput();
		#endif
		
		#if (!UNITY_IPHONE && !UNITY_ANDROID && !UNITY_BLACKBERRY && !UNITY_WP8) || UNITY_EDITOR
		ReactToInputPanning();
		#endif
		
		HandleInputLimits();
		
		if (itsTarget.itsTarget != null)
		{
			//Get ground vector angle
			float aGroundOffsetAngle = 0;
			if (itsEnvironment.itsFollowGround)
			{
				GetGroundVector ();
				Vector3 aForward = transform.forward;
				aForward.y = 0;
				aGroundOffsetAngle = Vector3.Dot (aForward, itsCurrentGroundNormal) * 90.0f;
			}
			
			HandleTargetPosition();
			//adapt field of view
			if(itsCamera.itsCamera != null)
				itsCamera.itsCamera.fieldOfView = Mathf.SmoothDamp(itsCamera.itsCamera.fieldOfView,itsCamera.itsFieldOfView, ref itsFieldOfViewVelocity, 1.0f /5.0f);
			
			HandleRotation(aGroundOffsetAngle);

			/// detect collision & calculate new position
			itsCollisionZoom = itsTargetZoom;

			//Check for Ray Collision
			Vector3 aNormalVector = Vector3.zero;
			if (itsTarget.itsTarget != null && itsEnvironment.itsTestCollisions)
			{
				RaycastHit aCollisionHit;
				
				//Collision detected
				if (Physics.Linecast (itsTargetTranform.position, itsOriginalPosition, out aCollisionHit, itsEnvironment.itsCollisionLayer))
				{
					Debug.DrawLine (itsTargetTranform.position, itsOriginalPosition, new Color (1, 0.0f, 0.0f));

					//get distance to collision object + a little overhead
					float aDistance = Vector3.Distance (itsTargetTranform.position, aCollisionHit.point + itsEnvironment.itsCollisionOffset*transform.forward);
					
//					if (itsZoom.itsUseLimits)	//disable this. Allow zooming nearer than min zoom if collision
//					{
//						if (aDistance < itsZoom.itsMinLimit)
//							aDistance = itsZoom.itsMinLimit;
//					}

					if (itsTargetZoom > aDistance)
					{
						//only set collision distance if smaller than current distance - prevent zooming away
						itsCollisionZoom = aDistance;
					}
					itsCollision = true;
					aNormalVector = aCollisionHit.normal * 1;

				}
				else
				{
					Debug.DrawLine (itsTargetTranform.position, itsOriginalPosition, new Color (0.5f, 0.5f, 0.5f));
				}
			}
			
			float aCollisionZoomSpeed = 100.0f;
			//Ray collision found -> do following
			if (itsCollision)
			{
				if (aNormalVector != Vector3.zero)
				{
					//Lerp to ray cast normal
					itsNormalVector.x = Mathf.Lerp (itsNormalVector.x, aNormalVector.x, aCollisionZoomSpeed * Time.deltaTime);
					itsNormalVector.y = Mathf.Lerp (itsNormalVector.y, aNormalVector.y, aCollisionZoomSpeed * Time.deltaTime);
					itsNormalVector.z = Mathf.Lerp (itsNormalVector.z, aNormalVector.z, aCollisionZoomSpeed * Time.deltaTime);
				}
			}
			else
			{
				//Remove offset if no collision
				itsNormalVector.x = Mathf.Lerp (itsNormalVector.x, 0, aCollisionZoomSpeed * Time.deltaTime);
				itsNormalVector.y = Mathf.Lerp (itsNormalVector.y, 0, aCollisionZoomSpeed * Time.deltaTime);
				itsNormalVector.z = Mathf.Lerp (itsNormalVector.z, 0, aCollisionZoomSpeed * Time.deltaTime);

			}
			if(itsCollisionZoom < itsTargetZoom)
				itsCurrentZoom = Mathf.Lerp (itsCurrentZoom, itsCollisionZoom, 100* Time.deltaTime);//itsCollisionZoom;
			else
				itsCurrentZoom = Mathf.Lerp (itsCurrentZoom, itsCollisionZoom, itsZoom.itsZoomSpeed * Time.deltaTime);
			
			//Original Position without offset -> for ray cast
			//itsOriginalPosition = itsCurrentLinkPosition + itsRotationQuaternion * (new Vector3 (0f, 0f, -itsCurrentZoom) + itsCurrentBoxCollisionNormal);
			itsOriginalPosition = (itsCurrentLinkPosition + itsCurrentPanningOffset) + itsRotationQuaternion * (new Vector3 (0f, 0f, -itsTargetZoom));
			//
			//final position of orbiter
			//transform.position = itsCurrentLinkPosition + itsRotationQuaternion * (new Vector3 (0f, 0f, -itsCurrentZoom) + itsCurrentBoxCollisionNormal);
			transform.position = (itsCurrentLinkPosition + itsCurrentPanningOffset) + itsRotationQuaternion * (new Vector3 (0f, 0f, -itsCurrentZoom));
			
			itsCollision = false;
		}
		HandleLookatTarget (false);
		HandleObjectsInLineOfSight();
	}
	
	private void HandleTargetPosition()
	{
		if (itsTarget.itsFollowPosition && itsTarget.itsTarget != null && itsTargetTranform != null)
		{
			itsCurrentLinkPosition = Vector3.SmoothDamp(itsCurrentLinkPosition, itsTargetTranform.position, ref itsLinkPositionVelocity, 1.0f / itsTarget.itsPositionSpeed);
		}
		else
		{
			itsCurrentLinkPosition = Vector3.SmoothDamp (itsCurrentLinkPosition, itsTargetStartPosition, ref itsLinkPositionVelocity, 1.0f / itsTarget.itsPositionSpeed);
		}
		itsCurrentPanningOffset = Vector3.SmoothDamp (itsCurrentPanningOffset, itsPanningOffset, ref itsPanningOffsetVelocity, 1.0f / itsPanning.itsSpeed);
	}
	
	/// <summary>
	/// Calculates the camera rotation based on quaternions
	/// </summary>
	/// <param name="theGroundOffsetAngle"></param>
	private void HandleRotation(float theGroundOffsetAngle)
	{
		Quaternion aRotationHorizontalCurrent = Quaternion.Euler (0f, itsLocalRotationHorizontalInput, 0f);
		Quaternion aRotationVerticalCurrent = Quaternion.Euler (itsLocalRotationVerticalInput - theGroundOffsetAngle, 0f, 0f);
		Quaternion aRotationInputFinal = aRotationHorizontalCurrent * aRotationVerticalCurrent;
		
		Quaternion aTargetQuaternion = Quaternion.identity;
		if (itsTarget.itsFollowRotation && itsTarget.itsTarget != null && itsTargetTranform != null)
		{
			aTargetQuaternion = itsTargetTranform.rotation * aRotationInputFinal;
		}
		else
		{
			aTargetQuaternion = aRotationInputFinal;
		}
		itsRotationQuaternion = Quaternion.Slerp (itsRotationQuaternion, aTargetQuaternion, Time.deltaTime * itsTarget.itsRotationSpeed);
	}
	
	/// <summary>
	/// This method collects all objects in the line of sight between the root and the camera
	/// </summary>
	private void HandleObjectsInLineOfSight()
	{
		if(itsTargetTranform == null)
			return;
		
		Vector3 aDirection = itsOriginalPosition-itsTargetTranform.position;
		aDirection.Normalize();
		float aDistance = Vector3.Distance(itsTargetTranform.position,itsOriginalPosition);
		int aNumberOfHits = 0;
		if(itsObjectInLineOfSight != null)
			aNumberOfHits = itsObjectInLineOfSight.Length;
		itsObjectInLineOfSight = Physics.RaycastAll(itsTargetTranform.position,aDirection,aDistance,itsEnvironment.itsCollisionLayer);
		
		if(itsObjectInLineOfSight != null)
		{
			if(itsObjectInLineOfSight.Length != aNumberOfHits)
			{
				if(EventIntersectorsChanged != null)
					EventIntersectorsChanged.Trigger(this);
			}
		}
	}

	/// <summary>
	/// Handles lookat target camera rotation
	/// </summary>
	/// <param name="theInit"></param>
	private void HandleLookatTarget (bool theInit)
	{
		if(itsTargetOldTransform == null || itsTargetTranform == null)
			return;
		
		if(itsLookatTransform == null && itsLookat.itsLookatTarget != null)
			itsLookatTransform = itsLookat.itsLookatTarget.transform;
		
		float aDistanceFromOrigin = Vector3.Distance(itsCurrentLinkPosition,itsTargetOldTransform.position);
		float aTotalDistance = Vector3.Distance(itsTargetOldTransform.position,itsTargetTranform.position);
		if(itsTargetTranform == itsTargetOldTransform)
			aTotalDistance = 1;
		
		if (!itsLookat.itsEnable)
		{
			if (itsTarget.itsTarget != null)
			{
				itsCurrentLookatPosition = Vector3.SmoothDamp (itsCurrentLookatPosition, itsTargetTranform.position, ref itsLinkLookatVelocity, 1.0f / itsTarget.itsPositionSpeed);
				if(itsTarget.itsFollowRotation)
				{
					itsTargetUp = Vector3.Slerp(itsTargetOldTransform.up,itsTargetTranform.up,aDistanceFromOrigin/aTotalDistance);
				}
				else
				{
					itsTargetUp = Vector3.Slerp(itsTargetOldTransform.up,Vector3.up,aDistanceFromOrigin/aTotalDistance);
				}
				Vector3 aForwardVector = ((itsCurrentLookatPosition + itsCurrentPanningOffset) - transform.position).normalized;
				Quaternion aLookRotation = Quaternion.LookRotation(aForwardVector,itsTargetUp);
				transform.rotation = aLookRotation;
			}
		}
		else
		{
			if(theInit)
			{
				itsCurrentLookatPosition = itsLookatTransform.position;

				if(itsLookat.itsUpVectorSource != null)
				{
					itsTargetLookatRotation = Quaternion.LookRotation(((itsCurrentLookatPosition + itsCurrentPanningOffset)- transform.position).normalized, itsLookat.itsUpVectorSource.transform.up);
					itsCurrentLookatRotation = itsTargetLookatRotation;
				}
			}
			else
			{

				if(itsLookat.itsHardLinkToTarget)
				{
					itsCurrentLookatPosition = itsLookatTransform.position;
					itsCurrentLookatRotation = Quaternion.LookRotation(((itsCurrentLookatPosition  + itsCurrentPanningOffset)- transform.position).normalized, itsLookat.itsUpVectorSource.transform.up);
				}
				else
				{
					
					itsCurrentLookatPosition = Vector3.SmoothDamp (itsCurrentLookatPosition, itsLookatTransform.position, ref itsLinkLookatVelocity, 1.0f / itsLookat.itsLookatSpeed);
					itsTargetUp = Vector3.Slerp(itsTargetOldTransform.up,itsTargetTranform.up,aDistanceFromOrigin/aTotalDistance);
					Vector3 aForwardVector = ((itsCurrentLookatPosition  + itsCurrentPanningOffset)- transform.position).normalized;
					itsTargetLookatRotation = Quaternion.LookRotation(aForwardVector,itsTargetUp);
					itsCurrentLookatRotation = itsTargetLookatRotation;
				}
				transform.rotation = itsCurrentLookatRotation;
			}
			transform.rotation = itsCurrentLookatRotation;
		}
	}
	
	/// <summary>
	/// 
	/// </summary>
	private void GetGroundVector ()
	{
		if (itsTarget.itsTarget != null)
		{
			RaycastHit aCollisionHit;
			if (Physics.Linecast (itsTargetTranform.position, itsTargetTranform.position - Vector3.up * 1000.0f, out aCollisionHit))//, itsDataSpatialOrbiter.itsCollisionLayers.value))
			{
				Debug.DrawLine (itsTargetTranform.position, aCollisionHit.point, Color.yellow);
				itsTargetGroundNormal = -aCollisionHit.normal;
				itsTargetGroundNormal.Normalize ();
				itsCurrentGroundNormal = Vector3.Lerp (itsCurrentGroundNormal, itsTargetGroundNormal, Time.deltaTime * 1.0f);
				itsCurrentGroundNormal.Normalize ();
			}
			else
			{
				Debug.DrawLine (transform.position, transform.position - Vector3.up * 1000.0f, Color.yellow);
			}
		}
	}
	
	/// <summary>
	/// 
	/// </summary>
	private void ReactToInputPanning ()
	{
		bool theReset = false;
		float anInputPanLeftRight = HandleControlAxis(itsPanning.itsLeftRight.itsControls, out theReset);
		if (itsPanning.itsLeftRight.itsEnable)
		{
			itsPanningOffset += transform.right * anInputPanLeftRight;
		}
		if(theReset)
		{
			itsPanningOffset.x = 0.0f;
		}
		
		float anInputUpDown = HandleControlAxis(itsPanning.itsUpDown.itsControls, out theReset);
		if (itsPanning.itsUpDown.itsEnable)
		{
			Vector3 aDelta = transform.up;
			if(!itsPanning.itsUseCameraSpace)
			{
				aDelta.z = 0.0f;
				aDelta.Normalize();
			}
			aDelta *= anInputUpDown;
			itsPanningOffset += aDelta;
		}
		if(theReset)
		{
			itsPanningOffset.y = 0.0f;
		}
		
		float anInputForwardBackward = HandleControlAxis(itsPanning.itsForwardBackward.itsControls, out theReset);
		if (itsPanning.itsForwardBackward.itsEnable)
		{
			Vector3 aDelta = transform.forward;
			if(!itsPanning.itsUseCameraSpace)
			{
				aDelta.y = 0.0f;
				aDelta.Normalize();
			}
			aDelta *=  anInputForwardBackward;
			itsPanningOffset += aDelta;
		}
		if(theReset)
		{
			itsPanningOffset.z = 0.0f;
		}
		
		DampPanningDirection();
	}
	
	/// <summary>
	/// This method checks if moving into this direction is possible. If not a damping will be applied.
	/// </summary>
	/// <param name="theDirection"></param>
	/// <returns></returns>
	private void DampPanningDirection()
	{
		if(itsPanning.itsBounds != null && itsPanningOffset != Vector3.zero)
		{
			float aRayCastLength = itsPanningOffset.magnitude;
//			RaycastHit aRayCastHit;
//			Ray aRay = new Ray(itsTargetTranform.position+itsPanningOffset,itsTargetTranform.position);	//this code would be much faster but for some reason collider.raycast is not working?
//			bool aHit = itsPanning.itsBounds.Raycast(aRay,out aRayCastHit, 1000.0f);
			
			Debug.DrawLine(itsTargetTranform.position+itsPanningOffset,itsTargetTranform.position,Color.red);
			RaycastHit[] aHits = Physics.RaycastAll(itsTargetTranform.position+itsPanningOffset,-itsPanningOffset, aRayCastLength);
			foreach(RaycastHit aHit in aHits)
			{
				if(aHit.collider == itsPanning.itsBounds.GetComponent<Collider>())
				{
					itsPanningOffset = aHit.point - itsTargetTranform.position;
				}
			}
		}
	}
	
	/// <summary>
	/// handles all assigned controls for one axis
	/// </summary>
	/// <param name="theControls"></param>
	/// <returns></returns>
	float HandleControlAxis(control_axis[] theControls, out bool theReset)
	{
		theReset = false;
		float aValue = 0.0f;
		for(int i = 0; i< theControls.Length; i++)
		{
			control_axis aControlAxis = theControls[i];
			float aSign = 1.0f;
			if (aControlAxis.itsInvertAxis)
			{
				aSign = -1.0f;
			}
			if(aControlAxis.itsAdditionalKey == KeyCode.None && aControlAxis.itsAdditionalMouseButton == control_axis.eMouseButton.eNone)
			{
				float anAxis = 0.0f;
				try
				{
					anAxis = Input.GetAxis(aControlAxis.itsAxisName);
				}
				catch(Exception theException)
				{
					Debug.LogError("KGFOrbitcam exception: "+theException);
					return 0.0f;
				}
				aValue += anAxis*aControlAxis.itsAxisSensitivity*aSign;
			}
			else
			{
				if(Input.GetKey(aControlAxis.itsAdditionalKey) || Input.GetMouseButton((int)aControlAxis.itsAdditionalMouseButton))
				{
					aValue += Input.GetAxis(aControlAxis.itsAxisName)*aControlAxis.itsAxisSensitivity*aSign;
				}
				else
				{
					if(aControlAxis.itsResetRotationOnRelease)
					{
						theReset = true;
					}
				}
			}
		}
		return aValue;
	}
	
	/// <summary>
	/// 
	/// </summary>
	private void ReactToInput ()
	{
		float aDampedInputValue = 1.0f;
		
		bool theReset = false;
		//horizontal rotation
		float anInputLeftRight = HandleControlAxis(itsRotation.itsHorizontal.itsControls, out theReset);
		
		if (itsRotation.itsHorizontal.itsEnable)
		{
			if (itsRotation.itsHorizontal.itsUseLimits)	//make input constant and slow when limits reached
			{
				
				if (itsLocalRotationHorizontalInput > itsRotation.itsHorizontal.itsStartValue + itsRotation.itsHorizontal.itsLeftLimit && anInputLeftRight > 0.0f && anInputLeftRight > aDampedInputValue)
				{
					anInputLeftRight = aDampedInputValue;
				}
				else if (itsLocalRotationHorizontalInput < itsRotation.itsHorizontal.itsStartValue - itsRotation.itsHorizontal.itsRightLimit && anInputLeftRight < 0.0f && anInputLeftRight < -aDampedInputValue)
				{
					anInputLeftRight = -aDampedInputValue;
				}
			}
			itsLocalRotationHorizontalInput += anInputLeftRight;
			if(theReset)
			{
				SetRotationHorizontal(GetRotationHorizontalStartValue());
			}
		}
		
		//vertical rotation
		float anInputUpDown = HandleControlAxis(itsRotation.itsVertical.itsControls, out theReset);
		if (itsRotation.itsVertical.itsEnable)
		{
			if (itsRotation.itsVertical.itsUseLimits)
			{
				if (itsLocalRotationVerticalInput > itsRotation.itsVertical.itsStartValue + itsRotation.itsVertical.itsUpLimit && anInputUpDown > 0.0f && anInputUpDown > aDampedInputValue)
				{
					anInputUpDown = aDampedInputValue;
				}
				else if (itsLocalRotationVerticalInput < itsRotation.itsVertical.itsStartValue - itsRotation.itsVertical.itsDownLimit && anInputUpDown < 0.0f && anInputUpDown < -aDampedInputValue)
				{
					anInputUpDown = -aDampedInputValue;
				}
			}
			itsLocalRotationVerticalInput += anInputUpDown;
			if(theReset)
			{
				SetRotationVertical(GetRotationVerticalStartValue());
			}
		}
		
		//zoom
		if (itsZoom.itsEnable)
		{
			itsTargetZoom += HandleControlAxis(itsZoom.itsControls, out theReset);
			if(theReset)
			{
				SetZoom(GetZoomStartValue());
			}
		}
	}
	
	/// <summary>
	/// 
	/// </summary>
	void OnDrawGizmosSelected()
	{
		if(itsTarget.itsTarget == null)
			return;
		
		itsTargetTranform =  itsTarget.itsTarget.transform;
		if(itsLookat.itsLookatTarget != null)
		{
			itsLookatTransform = itsLookat.itsLookatTarget.transform;
		}

		//draw target item to lookat target, or if not available to root target
		Vector3 aTargetGizmoPosition = Vector3.zero;
		if (itsLookat.itsLookatTarget != null /*&& itsLookat.itsLookatTarget != itsRoot.itsRoot  && itsLookat.itsEnable*/)
		{
			aTargetGizmoPosition = itsLookatTransform.position;
			Gizmos.DrawIcon (aTargetGizmoPosition, "eye.png", true);
		}
		if (itsTarget.itsTarget != null)
		{
			aTargetGizmoPosition = itsTargetTranform.position;
			Gizmos.DrawIcon (aTargetGizmoPosition, "target_64.png", true);
		}


		//if game not running initialize variables
		if (itsEditor)
		{
			itsCurrentZoom = itsZoom.itsStartValue;
			itsLocalRotationHorizontalCurrent = itsRotation.itsHorizontal.itsStartValue;
			itsLocalRotationVerticalCurrent = itsRotation.itsVertical.itsStartValue;
		}
		
		//Calculate number of points per circle
		int aVerticalPointNumber = (int)(((itsRotation.itsVertical.itsStartValue + itsRotation.itsVertical.itsUpLimit) - (itsRotation.itsVertical.itsStartValue - itsRotation.itsVertical.itsDownLimit)) / 5f);
		int aHorizontalPointNumber = (int)(((itsRotation.itsHorizontal.itsStartValue + itsRotation.itsHorizontal.itsLeftLimit) - (itsRotation.itsHorizontal.itsStartValue - itsRotation.itsHorizontal.itsRightLimit)) / 5f);

		if (!itsRotation.itsVertical.itsUseLimits)
		{
			aVerticalPointNumber = 36;
		}

		if (!itsRotation.itsHorizontal.itsUseLimits)
		{
			aHorizontalPointNumber = 36;
		}

		Vector3[] aVerticalPointsArray = new Vector3[aVerticalPointNumber + 1];
		Vector3[] aHorizontalPointsArray = new Vector3[aHorizontalPointNumber + 1];

		float aTargetVerticalAngle = 0f;
		float aTargetHorizontalAngle = 0f;
		Vector3 aTargetPosition = new Vector3 ();
		Quaternion aTargetRotation = Quaternion.identity;
		float aVerticalAngle = 0;
		float aHorizontalAngle = 0;
		Vector3 aPoint = new Vector3 ();
		float aLineLength = 1f;
		Vector3 aTargetVector = Vector3.zero;
		Vector3 aTargetPoint = Vector3.zero;

		//if link target enabled, add rotation and position to calculation
		if (itsTarget.itsFollowPosition && itsTarget.itsTarget != null)
		{
			aTargetPosition = itsTargetTranform.position;
		}
		else
		{
			aTargetPosition = Vector3.zero;
		}

		if (itsTarget.itsFollowRotation && itsTarget.itsTarget != null)
		{
			aTargetRotation = itsTargetTranform.rotation;
		}

		//only draw circle if vertical rotation allowed
		if (itsRotation.itsVertical.itsEnable)
		{
			for (int i = 0; i < aVerticalPointNumber+1; i++)
			{
				if (itsRotation.itsVertical.itsUseLimits)
				{
					//calculate angles
					aVerticalAngle = ((itsRotation.itsVertical.itsStartValue - itsRotation.itsVertical.itsDownLimit) + 270f + ((itsRotation.itsVertical.itsStartValue + itsRotation.itsVertical.itsUpLimit) - (itsRotation.itsVertical.itsStartValue - itsRotation.itsVertical.itsDownLimit)) / ((float)aVerticalPointNumber) * i) * Mathf.PI / 180f;
				}
				else
				{
					aVerticalAngle = (0 + 270f + (360) / ((float)aVerticalPointNumber) * i) * Mathf.PI / 180f;
				}
				//horizontal angle is current horizontal rotation
				aHorizontalAngle = (itsLocalRotationHorizontalCurrent + aTargetHorizontalAngle) * Mathf.PI / 180f;

				//Calcualte point on sphere around target with zoom as radius
				aPoint = new Vector3 ();
				aPoint.z = itsCurrentZoom * Mathf.Sin (aVerticalAngle) * Mathf.Cos (aHorizontalAngle);
				aPoint.x = itsCurrentZoom * Mathf.Sin (aVerticalAngle) * Mathf.Sin (aHorizontalAngle);
				aPoint.y = itsCurrentZoom * Mathf.Cos (aVerticalAngle);

				aPoint = aTargetPosition + aTargetRotation * aPoint;

				aVerticalPointsArray [i] = aPoint;
			}

			//Draw circle
			for (int i = 0; i < aVerticalPointNumber -0; i++)
			{
				Gizmos.color = new Color (1f, 0f, 0f);
				Gizmos.DrawLine (aVerticalPointsArray [i], aVerticalPointsArray [i + 1]);

			}

			//Draw small line from circle start to target
			aTargetVector = (aTargetPosition - aVerticalPointsArray [0]).normalized;
			aTargetPoint = aVerticalPointsArray [0] + aTargetVector * aLineLength;
			Gizmos.DrawLine (aVerticalPointsArray [0], aTargetPoint);

			//Draw small line from circle end to target
			aTargetVector = (aTargetPosition - aVerticalPointsArray [aVerticalPointNumber]).normalized;
			aTargetPoint = aVerticalPointsArray [aVerticalPointNumber] + aTargetVector * aLineLength;
			Gizmos.DrawLine (aVerticalPointsArray [aVerticalPointNumber], aTargetPoint);


			//Calculate point on the middle of the circle
			aHorizontalAngle = (itsLocalRotationHorizontalCurrent + aTargetHorizontalAngle) * Mathf.PI / 180f;
			aPoint.z = itsCurrentZoom * Mathf.Sin ((itsRotation.itsVertical.itsStartValue + 270) / 180f * Mathf.PI) * Mathf.Cos (aHorizontalAngle);
			aPoint.x = itsCurrentZoom * Mathf.Sin ((itsRotation.itsVertical.itsStartValue + 270) / 180f * Mathf.PI) * Mathf.Sin (aHorizontalAngle);
			aPoint.y = itsCurrentZoom * Mathf.Cos ((itsRotation.itsVertical.itsStartValue + 270) / 180f * Mathf.PI);
			
			aPoint = aTargetPosition + aTargetRotation * aPoint;

			//Draw small line from circle center to target
			aTargetVector = (aTargetPosition - aPoint).normalized;
			aTargetPoint = aPoint + aTargetVector * aLineLength;
			Gizmos.DrawLine (aPoint, aTargetPoint);
		}



		//only draw circle if horizontal rotation allowed
		if (itsRotation.itsHorizontal.itsEnable)
		{

			for (int i = 0; i < aHorizontalPointNumber+1; i++)
			{
				//Vertical angle is current vertical rotation
				aVerticalAngle = (90 - itsLocalRotationVerticalCurrent + aTargetVerticalAngle) * Mathf.PI / 180f;

				if (itsRotation.itsHorizontal.itsUseLimits)
				{
					//calculate angles
					aHorizontalAngle = (270 - ((itsRotation.itsHorizontal.itsStartValue - itsRotation.itsHorizontal.itsRightLimit) + ((itsRotation.itsHorizontal.itsStartValue + itsRotation.itsHorizontal.itsLeftLimit) - (itsRotation.itsHorizontal.itsStartValue - itsRotation.itsHorizontal.itsRightLimit)) / ((float)aHorizontalPointNumber) * i)) * Mathf.PI / 180f;
				}
				else
				{
					aHorizontalAngle = (270 - (360) / ((float)aHorizontalPointNumber) * i) * Mathf.PI / 180f;
				}

				//Calculate point on sphere wieh current zoom as radius
				aPoint = new Vector3 ();

				aPoint.x = itsCurrentZoom * Mathf.Sin (aVerticalAngle) * Mathf.Cos (aHorizontalAngle);
				aPoint.z = itsCurrentZoom * Mathf.Sin (aVerticalAngle) * Mathf.Sin (aHorizontalAngle);
				aPoint.y = itsCurrentZoom * Mathf.Cos (aVerticalAngle);
				
				aPoint = aTargetPosition + aTargetRotation * aPoint;

				aHorizontalPointsArray [i] = aPoint;
			}

			//Draw cirlce
			for (int i = 0; i < aHorizontalPointNumber - 0; i++)
			{
				Gizmos.color = new Color (0f, 1f, 0f);
				Gizmos.DrawLine (aHorizontalPointsArray [i], aHorizontalPointsArray [i + 1]);
			}

			//Draw Line from start point of circle to target
			aTargetVector = (aTargetPosition - aHorizontalPointsArray [0]).normalized;
			aTargetPoint = aHorizontalPointsArray [0] + aTargetVector * aLineLength;
			Gizmos.DrawLine (aHorizontalPointsArray [0], aTargetPoint);

			//Draw Line from end point of circle to target
			aTargetVector = (aTargetPosition - aHorizontalPointsArray [aHorizontalPointNumber]).normalized;
			aTargetPoint = aHorizontalPointsArray [aHorizontalPointNumber] + aTargetVector * aLineLength;
			Gizmos.DrawLine (aHorizontalPointsArray [aHorizontalPointNumber], aTargetPoint);

			//Calculate point on center of circle
			aVerticalAngle = (90 - itsLocalRotationVerticalCurrent) * Mathf.PI / 180f;
			aPoint.x = itsCurrentZoom * Mathf.Sin (aVerticalAngle) * Mathf.Cos ((270 - itsRotation.itsHorizontal.itsStartValue) / 180f * Mathf.PI);
			aPoint.z = itsCurrentZoom * Mathf.Sin (aVerticalAngle) * Mathf.Sin ((270 - itsRotation.itsHorizontal.itsStartValue) / 180f * Mathf.PI);
			aPoint.y = itsCurrentZoom * Mathf.Cos (aVerticalAngle);
			
			aPoint = aTargetPosition + aTargetRotation * aPoint;

			//Draw line from center of cirlce to target
			aTargetVector = (aTargetPosition - aPoint).normalized;
			aTargetPoint = aPoint + aTargetVector * aLineLength;
			Gizmos.DrawLine (aPoint, aTargetPoint);

		}

		
		//Only draw line if zoom enabled
		if (itsZoom.itsEnable)
		{
			aTargetVector = (transform.position - aTargetPosition).normalized;
			Vector3 aStartPoint = aTargetPosition + itsZoom.itsMinLimit * aTargetVector;
			Vector3 aEndPoint = aTargetPosition + itsZoom.itsMaxLimit * aTargetVector;
			Gizmos.color = new Color (0, 0, 1);


			aVerticalAngle = (90f - itsLocalRotationVerticalCurrent + aTargetVerticalAngle) * Mathf.PI / 180f;
			aHorizontalAngle = (itsLocalRotationHorizontalCurrent + 180f + aTargetHorizontalAngle) * Mathf.PI / 180f;
			
			aPoint = new Vector3 ();
			aPoint.z = itsCurrentZoom * Mathf.Sin (aVerticalAngle) * Mathf.Cos (aHorizontalAngle);
			aPoint.x = itsCurrentZoom * Mathf.Sin (aVerticalAngle) * Mathf.Sin (aHorizontalAngle);
			aPoint.y = itsCurrentZoom * Mathf.Cos (aVerticalAngle);
			

			Quaternion aTargetRotation2 = Quaternion.Euler (aVerticalAngle, 0f, 0f);
			Quaternion aFinalRotation = aTargetRotation;
			aPoint = aTargetPosition + aFinalRotation * aPoint;


			if (itsZoom.itsUseLimits)
			{
				//if limits enabled draw line from zoom min to zoom max
				//aPoint = transform.position;
				aTargetVector = (aPoint - aTargetPosition).normalized;
				aStartPoint = aTargetPosition + itsZoom.itsMinLimit * aTargetVector;
				aEndPoint = aTargetPosition + itsZoom.itsMaxLimit * aTargetVector;
			}
			else
			{
				//Draw "infinite" line if limits disabled
				aTargetVector = (aPoint - aTargetPosition).normalized;
				aStartPoint = aTargetPosition;
				aEndPoint = aTargetPosition + 1000 * aTargetVector;
			}

			Gizmos.DrawLine (aStartPoint, aEndPoint);


			if (itsZoom.itsUseLimits)
			{
				//Calculate normals for zoom line
				aVerticalAngle = (itsLocalRotationVerticalCurrent + aTargetVerticalAngle) * Mathf.PI / 180f;
				aHorizontalAngle = (itsLocalRotationHorizontalCurrent + aTargetHorizontalAngle) * Mathf.PI / 180f;
				
				aPoint = new Vector3 ();
				aPoint.z = itsCurrentZoom * Mathf.Sin (aVerticalAngle) * Mathf.Cos (aHorizontalAngle);
				aPoint.x = itsCurrentZoom * Mathf.Sin (aVerticalAngle) * Mathf.Sin (aHorizontalAngle);
				aPoint.y = itsCurrentZoom * Mathf.Cos (aVerticalAngle);

				//rotate line around vertical angle
				aTargetRotation2 = Quaternion.Euler (aVerticalAngle, 0f, 0f);
				aFinalRotation = aTargetRotation * aTargetRotation2;
				aPoint = aTargetPosition + aFinalRotation * aPoint;

				//calculate final normal vector
				aTargetVector = (aPoint - aTargetPosition).normalized;

				//Draw normal line at min zoom point
				Vector3 aLineStartPoint = aStartPoint + aLineLength * aTargetVector * 0.5f;
				Vector3 aLineEndPoint = aStartPoint - aLineLength * aTargetVector * 0.5f;
				Gizmos.DrawLine (aLineStartPoint, aLineEndPoint);

				//Draw normal line at max zoom point
				aLineStartPoint = aEndPoint + aLineLength * aTargetVector * 0.5f;
				aLineEndPoint = aEndPoint - aLineLength * aTargetVector * 0.5f;
				Gizmos.DrawLine (aLineStartPoint, aLineEndPoint);
			}

		}


		//Draw line to lookat target
		if (itsLookat.itsLookatTarget != null/* && itsLookat.itsEnable*/)
		{
			aTargetVector = (transform.position - aTargetPosition).normalized;
			Vector3 aStartPoint = aTargetPosition + itsZoom.itsMinLimit * aTargetVector;
			Vector3 aEndPoint = aTargetPosition + itsZoom.itsMaxLimit * aTargetVector;
			Gizmos.color = new Color (0, 0, 1);
			
			
			aVerticalAngle = (90f - itsLocalRotationVerticalCurrent + aTargetVerticalAngle) * Mathf.PI / 180f;
			aHorizontalAngle = (itsLocalRotationHorizontalCurrent + 180f + aTargetHorizontalAngle) * Mathf.PI / 180f;
			
			aPoint = new Vector3 ();
			aPoint.z = itsCurrentZoom * Mathf.Sin (aVerticalAngle) * Mathf.Cos (aHorizontalAngle);
			aPoint.x = itsCurrentZoom * Mathf.Sin (aVerticalAngle) * Mathf.Sin (aHorizontalAngle);
			aPoint.y = itsCurrentZoom * Mathf.Cos (aVerticalAngle);

			Quaternion aFinalRotation = aTargetRotation;
			aPoint = aTargetPosition + aFinalRotation * aPoint;

			aTargetVector = (aPoint - itsLookatTransform.position).normalized;
			aStartPoint = itsLookatTransform.position;
			aEndPoint = aPoint;
			Gizmos.color = new Color(1,1,0);
			Gizmos.DrawLine (aStartPoint, aEndPoint);
		}
	}

	#region exposed methods

	#region target
	
	/// <summary>
	/// Changes the target of the KGFOrbitCam
	/// </summary>
	/// <param name="theObject"></param>
	[KGFEventExpose]
	public void SetTarget(GameObject theObject)
	{
		if(itsTarget.itsTarget == theObject)	//do nothing if same target
			return;
		
		itsTargetOld = itsTarget.itsTarget;
		itsTargetOldTransform = itsTargetOld.transform;
		itsTarget.itsTarget = theObject;
		itsTargetTranform = itsTarget.itsTarget.transform;
		if(EventTargetChanged != null)
			EventTargetChanged.Trigger(this);
		
		itsTargetStartPosition = itsTargetTranform.position;
		itsPanningOffset = Vector3.zero;
	}
	
	/// <summary>
	/// Gets the current target of the KGFOrbitCam
	/// </summary>
	/// <returns></returns>
	public GameObject GetTarget()
	{
		return itsTarget.itsTarget;
	}
	
	/// <summary>
	/// Defines if the KGFOrbitCam should follow the target position. If true it will move after the target using the itsPositionSpeed
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetTargetFollowPosition(bool theValue)
	{
		itsTarget.itsFollowPosition = theValue;
	}

	/// <summary>
	/// Returns if the KGFOrbitCam is following the target position.
	/// </summary>
	/// <returns></returns>
	public bool GetTargetFollowPosition()
	{
		return itsTarget.itsFollowPosition;
	}
	
	/// <summary>
	/// Defines how fast the KGFOrbitCam will follow the target position. Using low values will result in a nice following delay.
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetTargetFollowPositionSpeed(float theValue)
	{
		itsTarget.itsPositionSpeed = theValue;
	}
	
	/// <summary>
	/// Returns the target following speed
	/// </summary>
	/// <returns></returns>
	public float GetTargetFollowPositionSpeed()
	{
		return itsTarget.itsPositionSpeed;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetTargetFollowRotation(bool theValue)
	{
		itsTarget.itsFollowRotation = theValue;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public bool GetTargetFollowRotation()
	{
		return itsTarget.itsFollowRotation;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetTargetFollowRotationSpeed(float theValue)
	{
		itsTarget.itsRotationSpeed = theValue;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public float GetTargetFollowRotationSpeed()
	{
		return itsTarget.itsRotationSpeed;
	}
	#endregion
	
	
	#region zoom
	/// <summary>
	/// 
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetZoomEnable(bool theValue)
	{
		itsZoom.itsEnable = theValue;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public bool GetZoomEnable()
	{
		return itsZoom.itsEnable;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetZoomStartValue(float theValue)
	{
		itsZoom.itsStartValue = theValue;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public float GetZoomStartValue()
	{
		return itsZoom.itsStartValue;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetZoomUseLimits(bool theValue)
	{
		itsZoom.itsUseLimits = theValue;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public bool GetZoomUseLimits()
	{
		return itsZoom.itsUseLimits;
	}
	
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetZoomMaxLimit(float theValue)
	{
		itsZoom.itsMaxLimit = theValue;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public float GetZoomMaxLimit()
	{
		return itsZoom.itsMaxLimit;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetZoomMinLimit(float theValue)
	{
		itsZoom.itsMinLimit = theValue;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public float GetZoomMinLimit()
	{
		return itsZoom.itsMinLimit;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetZoom(float theValue)
	{
		itsTargetZoom = theValue;
		itsCollisionZoom = theValue;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public float GetZoom()
	{
		return itsTargetZoom;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetZoomCurrent(float theValue)
	{
		itsCurrentZoom = theValue;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public float GetZoomCurrent()
	{
		return itsCurrentZoom;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetZoomSpeed(float theValue)
	{
		itsZoom.itsZoomSpeed = theValue;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public float GetZoomSpeed()
	{
		return itsZoom.itsZoomSpeed;
	}
	
	/// <summary>
	/// Returns array of zoom controls
	/// </summary>
	/// <returns></returns>
	public control_axis[] GetZoomControls()
	{
		return itsZoom.itsControls;
	}
	
	/// <summary>
	/// Sets the horizontal controls
	/// </summary>
	/// <returns></returns>
	public void SetZoomControls(control_axis[] theControls)
	{
		itsZoom.itsControls = theControls;
	}
	
	/// <summary>
	/// Sets the horizontal controls
	/// </summary>
	/// <returns></returns>
	public void SetZoomControlsTouch(touch_axis_zoom theControlsTouch)
	{
		itsZoom.itsControlTouch = theControlsTouch;
	}
	
	
	
	
	#endregion
	/// <summary>
	/// 
	/// </summary>
	/// <param name="theValue"></param>
	#region rotation
	
	
	/// <summary>
	/// Use this method to enable or disable horizontal and vertical rotation at once
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetRotationEnable(bool theValue)
	{
		SetRotationHorizontalEnable(theValue);
		SetRotationVerticalEnable(theValue);
	}
	
	/// <summary>
	/// If true the user can manipulate horizontal rotation of the orbiter by the input defined in itsRotation.itsHorizontal.itsAxisName
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetRotationHorizontalEnable(bool theValue)
	{
		itsRotation.itsHorizontal.itsEnable = theValue;
	}
	
	/// <summary>
	/// Returns if the user may manipulate the horizontal rotation of the orbiter
	/// </summary>
	/// <returns></returns>
	public bool GetRotationHorizontalEnable()
	{
		return itsRotation.itsHorizontal.itsEnable;
	}
	
	/// <summary>
	/// Sets the initial horizontal rotation value of the orbiter. (If itsTarget.itsFollowRotation is true this rotation is relative to itsTarget, else it is relative to the world)
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetRotationHorizontalStartValue(float theValue)
	{
		itsRotation.itsHorizontal.itsStartValue = theValue;
	}
	
	/// <summary>
	/// Returns the initial horizontal rotation of the orbiter
	/// </summary>
	/// <returns></returns>
	public float GetRotationHorizontalStartValue()
	{
		return itsRotation.itsHorizontal.itsStartValue;
	}
	
	/// <summary>
	/// Enables min and max rotation limits for horizontal orbiter rotation
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetRotationHorizontalUseLimits(bool theValue)
	{
		itsRotation.itsHorizontal.itsUseLimits = theValue;
	}
	
	/// <summary>
	/// Returns if limits for the horizontal orbiter rotation are enabled
	/// </summary>
	/// <returns></returns>
	public bool GetRotationHorizontalUseLimits()
	{
		return itsRotation.itsHorizontal.itsUseLimits;
	}
	
	/// <summary>
	/// Sets the right limit for the horizontal orbiter rotation
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetRotationHorizontalRightLimit(float theValue)
	{
		itsRotation.itsHorizontal.itsRightLimit = theValue;
	}
	
	/// <summary>
	/// Returns the right limit for the horizontal orbiter rotation
	/// </summary>
	/// <returns></returns>
	public float GetRotationHorizontalRightLimit()
	{
		return itsRotation.itsHorizontal.itsRightLimit;
	}

	/// <summary>
	/// Sets the left limit for the horizontal orbiter rotation
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetRotationHorizontalLeftLimit(float theValue)
	{
		itsRotation.itsHorizontal.itsLeftLimit = theValue;
	}
	
	/// <summary>
	/// Returns the left limit for the horizontal orbiter rotation
	/// </summary>
	/// <returns></returns>
	public float GetRotationHorizontalLeftLimit()
	{
		return itsRotation.itsHorizontal.itsLeftLimit;
	}

	/// <summary>
	/// Sets the horizontal rotation the orbiter should reach. After setting this value the orbiter will move smoothly (using the rotation speed until it reaches this value)
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetRotationHorizontal(float theValue)
	{
		itsLocalRotationHorizontalInput = theValue;
	}
	
	/// <summary>
	/// Gets the horizontal rotation the orbiter should reach.
	/// </summary>
	/// <returns></returns>
	public float GetRotationHorizontal()
	{
		return itsLocalRotationVerticalInput;
	}

	/// <summary>
	/// Sets the current value for the horizontal orbiter rotation. Use this to reach a rotation immediately.
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetRotationHorizontalCurrent(float theValue)
	{
		itsLocalRotationHorizontalCurrent = theValue;
	}
	
	/// <summary>
	/// Returns the current horizontal orbiter rotation.
	/// </summary>
	/// <returns></returns>
	public float GetRotationHorizontalCurrent()
	{
		return itsLocalRotationHorizontalCurrent;
	}
	
	/// <summary>
	/// Sets the horizontal controls
	/// </summary>
	/// <returns></returns>
	public void SetRotationHorizontalControls(control_axis[] theControls)
	{
		itsRotation.itsHorizontal.itsControls = theControls;
	}
	
	/// <summary>
	/// Returns array of horizontal controls
	/// </summary>
	/// <returns></returns>
	public control_axis[] GetRotationHorizontalControls()
	{
		return itsRotation.itsHorizontal.itsControls;
	}
	
	/// <summary>
	/// Sets the horizontal touch control
	/// </summary>
	/// <param name="theControl"></param>
	public void SetRotationHorizontalControlTouch(touch_axis theControl)
	{
		itsRotation.itsHorizontal.itsControlTouch = theControl;
	}
	
	/// <summary>
	/// Returns horitzonal touch control
	/// </summary>
	/// <returns></returns>
	public touch_axis GetRotationHorizontalControlTouch()
	{
		return itsRotation.itsHorizontal.itsControlTouch;
	}
	
	/// <summary>
	/// Sets the vertical touch controls
	/// </summary>
	/// <param name="theControl"></param>
	public void SetRotationVerticalControlTouch(touch_axis theControl)
	{
		itsRotation.itsVertical.itsControlTouch = theControl;
	}
	
	/// <summary>
	/// Returns vertical touch control
	/// </summary>
	/// <returns></returns>
	public touch_axis GetRotationVerticalControlTouch()
	{
		return itsRotation.itsVertical.itsControlTouch;
	}
	
	/// <summary>
	/// If true the user can manipulate vertical rotation of the orbiter by the input defined in itsRotation.itsVertical.itsAxisName
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetRotationVerticalEnable(bool theValue)
	{
		itsRotation.itsVertical.itsEnable = theValue;
	}
	
	/// <summary>
	/// Returns if the user may manipulate the horizontal rotation of the orbiter
	/// </summary>
	/// <returns></returns>
	public bool GetRotationVerticalEnable()
	{
		return itsRotation.itsVertical.itsEnable;
	}
	
	/// <summary>
	/// Sets the initial vertical rotation value of the orbiter. (If itsTarget.itsFollowRotation is true this rotation is relative to itsTarget, else it is relative to the world)
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetRotationVerticalStartValue(float theValue)
	{
		itsRotation.itsVertical.itsStartValue = theValue;
	}
	
	/// <summary>
	/// Returns the initial vertical rotation of the orbiter
	/// </summary>
	/// <returns></returns>
	public float GetRotationVerticalStartValue()
	{
		return itsRotation.itsVertical.itsStartValue;
	}
	
	/// <summary>
	/// Enables min and max rotation limits for vertical orbiter rotation
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetRotationVerticalUseLimits(bool theValue)
	{
		itsRotation.itsVertical.itsUseLimits = theValue;
	}
	
	/// <summary>
	/// Returns if limits for the vertical orbiter rotation are enabled
	/// </summary>
	/// <returns></returns>
	public bool GetRotationVerticalUseLimits()
	{
		return itsRotation.itsVertical.itsUseLimits;
	}
	
	/// <summary>
	/// Sets the up limit for the vertical orbiter rotation
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetRotationVerticalUpLimit(float theValue)
	{
		itsRotation.itsVertical.itsUpLimit = theValue;
	}
	
	/// <summary>
	/// Returns the up limit for the vertical orbiter rotation
	/// </summary>
	/// <returns></returns>
	public float GetRotationVerticalUpLimit()
	{
		return itsRotation.itsVertical.itsUpLimit;
	}

	/// <summary>
	/// Sets the down limit for the vertical orbiter rotation
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetRotationVerticalDownLimit(float theValue)
	{
		itsRotation.itsVertical.itsDownLimit = theValue;
	}
	
	/// <summary>
	/// Returns the down limit for the vertical orbiter rotation
	/// </summary>
	/// <returns></returns>
	public float GetRotationVerticalDownLimit()
	{
		return itsRotation.itsVertical.itsDownLimit;
	}

	/// <summary>
	/// Sets the vertical rotation the orbiter should reach. After setting this value the orbiter will move smoothly (using the rotation speed until it reaches this value)
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetRotationVertical(float theValue)
	{
		itsLocalRotationVerticalInput = theValue;
	}
	
	/// <summary>
	/// Gets the vertical rotation the orbiter should reach.
	/// </summary>
	/// <returns></returns>
	public float GetRotationVertical()
	{
		return itsLocalRotationVerticalInput;
	}

	/// <summary>
	/// Sets the current value for the vertical orbiter rotation. Use this to reach a rotation immediately.
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetRotationVerticalCurrent(float theValue)
	{
		itsLocalRotationVerticalCurrent = theValue;
	}
	
	/// <summary>
	/// Returns the current vertical orbiter rotation.
	/// </summary>
	/// <returns></returns>
	public float GetRotationVerticalCurrent()
	{
		return itsLocalRotationVerticalCurrent;
	}
	
	/// <summary>
	/// Sets the vertical controls
	/// </summary>
	/// <returns></returns>
	public void SetRotationVerticalControls(control_axis[] theControls)
	{
		itsRotation.itsVertical.itsControls = theControls;
	}
	
	/// <summary>
	/// Returns array of horizontal controls
	/// </summary>
	/// <returns></returns>
	public control_axis[] GetRotationVerticalControls()
	{
		return itsRotation.itsVertical.itsControls;
	}
	#endregion

	
	#region panning
	/// <summary>
	/// Use this method to enable or disable horizontal and vertical rotation at once
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetPanningEnable(bool theValue)
	{
		SetPanningLeftRightEnable(theValue);
		SetPanningForwardBackwardEnable(theValue);
		SetPanningUpDownEnable(theValue);
	}
	
	/// <summary>
	/// User can set a collider to act as panning bounds. If not null the user will not be able to pan out of this boundaries..
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetPanningBounds(Collider theCollider)
	{
		itsPanning.itsBounds = theCollider;
	}
	
	/// <summary>
	/// User can change the panning speed by changing this value
	/// </summary>
	/// <param name="theSpeed"></param>
	public void SetPanningSpeed(float theSpeed)
	{
		itsPanning.itsSpeed = theSpeed;
	}
	
	/// <summary>
	/// returns the current panning speed
	/// </summary>
	/// <returns></returns>
	public float GetPanningSpeed()
	{
		return itsPanning.itsSpeed;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="theCameraSpacePanning"></param>
	public void SetPanningUseCameraSpace(bool theUseCameraSpace)
	{
		itsPanning.itsUseCameraSpace = theUseCameraSpace;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public bool GetPanningUseCameraSpace()
	{
		return itsPanning.itsUseCameraSpace;
	}
	
	/// <summary>
	/// Returns if limits for the horizontal orbiter rotation are enabled
	/// </summary>
	/// <returns></returns>
	public bool GetPanningHasBounds()
	{
		return itsPanning.itsBounds != null;
	}
	
	/// <summary>
	/// If true the user can manipulate left right panning of the orbiter by the input defined in itsPanning.itsLeftRight.itsControls
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetPanningLeftRightEnable(bool theValue)
	{
		itsPanning.itsLeftRight.itsEnable = theValue;
	}
	
	/// <summary>
	/// Returns if the user may manipulate the left right panning of the orbiter
	/// </summary>
	/// <returns></returns>
	public bool GetPanningLeftRightEnable()
	{
		return itsPanning.itsLeftRight.itsEnable;
	}
	
	/// <summary>
	/// Returns array of lef tright controls
	/// </summary>
	/// <returns></returns>
	public control_axis[] GetPanningLeftRightControls()
	{
		return itsPanning.itsLeftRight.itsControls;
	}
	
	/// <summary>
	/// Sets the left right controls
	/// </summary>
	/// <returns></returns>
	public void SetPanningLeftRightControls(control_axis[] theControls)
	{
		itsPanning.itsLeftRight.itsControls = theControls;
	}
	
	/// <summary>
	/// Sets the left right touch control
	/// </summary>
	/// <param name="theControl"></param>
	public void SetPanningLeftRightControlTouch(touch_axis theControl)
	{
		itsPanning.itsLeftRight.itsControlTouch = theControl;
	}
	
	/// <summary>
	/// Returns left right touch control
	/// </summary>
	/// <returns></returns>
	public touch_axis GetPanningLeftRightControlTouch()
	{
		return itsPanning.itsLeftRight.itsControlTouch;
	}
	
	/// <summary>
	/// If true the user can manipulate forward backward panning of the orbiter by the input defined in itsPanning.itsForwardBackward.itsControls
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetPanningForwardBackwardEnable(bool theValue)
	{
		itsPanning.itsForwardBackward.itsEnable = theValue;
	}
	
	/// <summary>
	/// Returns if the user may manipulate the forward backward panning of the orbiter
	/// </summary>
	/// <returns></returns>
	public bool GetPanningForwardBackwardEnable()
	{
		return itsPanning.itsForwardBackward.itsEnable;
	}
	
	/// <summary>
	/// Returns array of forward backward controls
	/// </summary>
	/// <returns></returns>
	public control_axis[] GetPanningForwardBackwardControls()
	{
		return itsPanning.itsForwardBackward.itsControls;
	}
	
	/// <summary>
	/// Sets the Forward Backward controls
	/// </summary>
	/// <returns></returns>
	public void SetPanningForwardBackwardControls(control_axis[] theControls)
	{
		itsPanning.itsForwardBackward.itsControls = theControls;
	}
	
	/// <summary>
	/// Sets the forward backward touch control
	/// </summary>
	/// <param name="theControl"></param>
	public void SetPanningForwardBackwardControlTouch(touch_axis theControl)
	{
		itsPanning.itsForwardBackward.itsControlTouch = theControl;
	}
	
	/// <summary>
	/// Returns forward backward touch control
	/// </summary>
	/// <returns></returns>
	public touch_axis GetPanningForwardBackwardControlTouch()
	{
		return itsPanning.itsForwardBackward.itsControlTouch;
	}
	
	/// <summary>
	/// If true the user can manipulate up down panning of the orbiter by the input defined in itsPanning.itsUpDown.itsControls
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetPanningUpDownEnable(bool theValue)
	{
		itsPanning.itsUpDown.itsEnable = theValue;
	}
	
	/// <summary>
	/// Returns if the user may manipulate the Up Down panning of the orbiter
	/// </summary>
	/// <returns></returns>
	public bool GetPanningUpDownEnable()
	{
		return itsPanning.itsUpDown.itsEnable;
	}
	
	/// <summary>
	/// Returns array of Up Downcontrols
	/// </summary>
	/// <returns></returns>
	public control_axis[] GetPanningUpDownControls()
	{
		return itsPanning.itsUpDown.itsControls;
	}
	
	/// <summary>
	/// Sets the Up Down controls
	/// </summary>
	/// <returns></returns>
	public void SetPanningUpDownControls(control_axis[] theControls)
	{
		itsPanning.itsUpDown.itsControls = theControls;
	}
	
	/// <summary>
	/// Sets the up down touch control
	/// </summary>
	/// <param name="theControl"></param>
	public void SetPanningUpDownControlTouch(touch_axis theControl)
	{
		itsPanning.itsUpDown.itsControlTouch = theControl;
	}
	
	/// <summary>
	/// Returns up down touch control
	/// </summary>
	/// <returns></returns>
	public touch_axis GetPanningUpDownControlTouch()
	{
		return itsPanning.itsUpDown.itsControlTouch;
	}
	#endregion
	
	
	#region camera
	/// <summary>
	/// 
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetCameraFieldOfView(float theValue)
	{
		itsCamera.itsFieldOfView = theValue;
	}
	
	
	/// <summary>
	/// Gets the target field of view angle of the KGFOrbitcam
	/// </summary>
	/// <remarks>
	/// Use this method to get the target field of view angle of the KGFOrbitcam. The target field of view angle is not the current field of view angle but the
	/// angle the camera tries to reach with the smoot field of view animation
	/// </remarks>
	/// <example>
	/// How to create a new map icon dynamically at runtime
	/// <code>
	/// using UnityEngine;
	/// using System.Collections;
	/// 
	/// public class MyCameraController : MonoBehaviour
	/// {
	/// 	private KGFOrbitCam itsKGFOrbitCam;	//reference to the orbitcam
	///
	/// 	public void Start()
	/// 	{
	/// 		itsKGFOrbitCam = KGFAccessor.GetObject<KGFOrbitCam>();	//get the KGFOrbitcam by using the KGFAccessor class
	///
	/// 		if(itsKGFOrbitCam != null)
	/// 		{
	/// 			float aTargetFieldOfView = itsKGFOrbitCam.GetCameraFieldOfView(); //get the target field of view of the camera
	/// 			Debug.Log("field of view of the camera is: "+aTargetFieldOfView);
	/// 		}
	/// 	}
	/// }
	/// </code>
	/// </example>
	/// <returns>float: The field of view angle in degrees</returns>
	[KGFEventExpose]
	public float GetCameraFieldOfView()
	{
		return itsCamera.itsFieldOfView;
	}
	#endregion

	#region control
	[KGFEventExpose]
	public void SetRotationToStartValues()
	{
		SetLocalRotationHorizontalInput(itsRotation.itsHorizontal.itsStartValue);
		SetLocalRotationVerticalInput(itsRotation.itsVertical.itsStartValue);
	}
	
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="theString"></param>
	[KGFEventExpose]
	public void SetRotationHorizontalAxisName(int theControlIndex, string theString)
	{
		itsRotation.itsHorizontal.itsControls[theControlIndex].itsAxisName = theString;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public string GetRotationHorizontalAxisName(int theControlIndex)
	{
		return itsRotation.itsHorizontal.itsControls[theControlIndex].itsAxisName;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetRotationHoritzontalAxisInvert(int theControlIndex, bool theValue)
	{
		itsRotation.itsHorizontal.itsControls[theControlIndex].itsInvertAxis = theValue;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public bool GetRotationHorizontalAxisInvert(int theControlIndex)
	{
		return itsRotation.itsHorizontal.itsControls[theControlIndex].itsInvertAxis;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetRotationHoritzontalAxisSensitivity(int theControlIndex, float theValue)
	{
		itsRotation.itsHorizontal.itsControls[theControlIndex].itsAxisSensitivity = theValue;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public float GetRotationHoritzontalAxisSensitivity(int theControlIndex)
	{
		return itsRotation.itsHorizontal.itsControls[theControlIndex].itsAxisSensitivity;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="theString"></param>
	[KGFEventExpose]
	public void SetRotationVerticalAxisName(int theControlIndex, string theString)
	{
		itsRotation.itsVertical.itsControls[theControlIndex].itsAxisName = theString;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public string GetRotationVerticalAxisName(int theControlIndex)
	{
		return itsRotation.itsVertical.itsControls[theControlIndex].itsAxisName;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetRotationVerticalAxisInvert(int theControlIndex, bool theValue)
	{
		itsRotation.itsVertical.itsControls[theControlIndex].itsInvertAxis = theValue;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public bool GetRotationVerticalAxisInvert(int theControlIndex)
	{
		return itsRotation.itsVertical.itsControls[theControlIndex].itsInvertAxis;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetRotationVerticalAxisSensitivity(int theControlIndex, float theValue)
	{
		itsRotation.itsVertical.itsControls[theControlIndex].itsAxisSensitivity = theValue;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public float GetRotationVerticalAxisSensitivity(int theControlIndex)
	{
		return itsRotation.itsVertical.itsControls[theControlIndex].itsAxisSensitivity;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="theString"></param>
	[KGFEventExpose]
	public void SetZoomAxisName(int theControlIndex, string theString)
	{
		itsZoom.itsControls[theControlIndex].itsAxisName = theString;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public string GetZoomAxisName(int theControlIndex)
	{
		return itsZoom.itsControls[theControlIndex].itsAxisName;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetZoomAxisSensitivity(int theControlIndex, float theValue)
	{
		itsZoom.itsControls[theControlIndex].itsAxisSensitivity = theValue;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public float GetZoomAxisSensitivity(int theControlIndex)
	{
		return itsZoom.itsControls[theControlIndex].itsAxisSensitivity;
	}
	#endregion

	#region terrain
	/// <summary>
	/// 
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetEnviromentFollowGroundEnable(bool theValue)
	{
		itsEnvironment.itsFollowGround = theValue;
	}
	
	/// <summary>
	/// Gets the information if ground following of the camera is enabled or not.
	/// </summary>
	/// <remarks>
	/// Use this method to get the information if ground following of the KGFOrbitcam is enabled.
	/// </remarks>
	/// <example>
	/// How to check if camera is following ground at runtime
	/// <code>
	/// using UnityEngine;
	/// using System.Collections;
	/// 
	/// public class MyCameraController : MonoBehaviour
	/// {
	/// 	private KGFOrbitCam itsKGFOrbitCam;	//reference to the orbitcam
	///
	/// 	public void Start()
	/// 	{
	/// 		itsKGFOrbitCam = KGFAccessor.GetObject<KGFOrbitCam>();	//get the KGFOrbitcam by using the KGFAccessor class
	///
	/// 		if(itsKGFOrbitCam != null)
	/// 		{
	/// 			bool anIsFollowingGround = itsKGFOrbitCam.GetEnviromentFollowGroundEnabled(); //check if ground following is enabled
	/// 			if(anIsFollowingGround)
	/// 				Debug.Log("camera is following ground");
	/// 			else
	/// 				Debug.Log("camera is not following ground");
	/// 		}
	/// 	}
	/// }
	/// </code>
	/// </example>
	/// <returns>bool: true if following false if not</returns>
	public bool GetEnviromentFollowGroundEnabled()
	{
		return itsEnvironment.itsFollowGround;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetEnviromentTestCollisionsEnable(bool theValue)
	{
		itsEnvironment.itsTestCollisions = theValue;
	}
	
	/// <summary>
	/// Gets the information if collision testing is enabled
	/// </summary>
	/// <remarks>
	/// Use this method to find out if collision testing is enabled
	/// </remarks>
	/// <example>
	/// How to check if collision testing is enabled at runtime
	/// <code>
	/// using UnityEngine;
	/// using System.Collections;
	/// 
	/// public class MyCameraController : MonoBehaviour
	/// {
	/// 	private KGFOrbitCam itsKGFOrbitCam;	//reference to the orbitcam
	///
	/// 	public void Start()
	/// 	{
	/// 		itsKGFOrbitCam = KGFAccessor.GetObject<KGFOrbitCam>();	//get the KGFOrbitcam by using the KGFAccessor class
	///
	/// 		if(itsKGFOrbitCam != null)
	/// 		{
	/// 			bool aCollisionTestingIsEnabled = itsKGFOrbitCam.GetEnviromentTestCollisionsEnabled(); //check if collision testing is enabled
	/// 			Debug.Log("camera is testing collisions: "+aCollisionTestingIsEnabled);
	/// 		}
	/// 	}
	/// }
	/// </code>
	/// </example>
	/// <returns>bool: true if testing is enabled else false.</returns>
	public bool GetEnviromentTestCollisionsEnabled()
	{
		return itsEnvironment.itsTestCollisions;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="theLayer"></param>
	[KGFEventExpose]
	public void SetEnviromentCollisionLayers(LayerMask theLayer)
	{
		itsEnvironment.itsCollisionLayer = theLayer;
	}
	
	/// <summary>
	/// Gets the LayerMask describing the layers where collisions will be tested if collision testing is enabled.
	/// </summary>
	/// <remarks>
	/// Use this method to get the LayerMask used for collision testing of the KGFOrbitcam.
	/// </remarks>
	/// <example>
	/// How to get the collision layers at runtime
	/// <code>
	/// using UnityEngine;
	/// using System.Collections;
	/// 
	/// public class MyCameraController : MonoBehaviour
	/// {
	/// 	private KGFOrbitCam itsKGFOrbitCam;	//reference to the orbitcam
	///
	/// 	public void Start()
	/// 	{
	/// 		itsKGFOrbitCam = KGFAccessor.GetObject<KGFOrbitCam>();	//get the KGFOrbitcam by using the KGFAccessor class
	///
	/// 		if(itsKGFOrbitCam != null)
	/// 		{
	/// 			LayerMask aLayerMask = itsKGFOrbitCam.GetEnviromentCollisionLayers(); //get the layermask for collisions
	/// 			Debug.Log("camera will test collisions on the following layers: "+aLayerMask);
	/// 		}
	/// 	}
	/// }
	/// </code>
	/// </example>
	/// <returns>LayerMask: The LayerMask used for collision testing</returns>
	public LayerMask GetEnviromentCollisionLayers()
	{
		return itsEnvironment.itsCollisionLayer;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="theLayer"></param>
	public void SetEnviromentCollisionOffset(float theCollisionOffset)
	{
		itsEnvironment.itsCollisionOffset = theCollisionOffset;
	}
	
	/// <summary>
	/// Gets the distance the camera gets transformed during collision in look direction. So the new camera position will be collision hitpoint + camera transform.forward * EnviromentCollisionOffset
	/// </summary>
	/// <remarks>
	/// Use this method to get the distance the camera gets transformed in look direction during a collision
	/// </remarks>
	/// <example>
	/// How to get the collision offset at runtime
	/// <code>
	/// using UnityEngine;
	/// using System.Collections;
	/// 
	/// public class MyCameraController : MonoBehaviour
	/// {
	/// 	private KGFOrbitCam itsKGFOrbitCam;	//reference to the orbitcam
	///
	/// 	public void Start()
	/// 	{
	/// 		itsKGFOrbitCam = KGFAccessor.GetObject<KGFOrbitCam>();	//get the KGFOrbitcam by using the KGFAccessor class
	///
	/// 		if(itsKGFOrbitCam != null)
	/// 		{
	/// 			float anOffset = itsKGFOrbitCam.GetEnviromentCollisionOffset(); //get collision offset
	/// 			Debug.Log("camera is located: "+anOffset+" units in front of the collision");
	/// 		}
	/// 	}
	/// }
	/// </code>
	/// </example>
	/// <returns>float: the offset</returns>
	public float GetEnviromentCollisionOffset()
	{
		return itsEnvironment.itsCollisionOffset;
	}
	#endregion

	#region lookat target
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetLookatEnable(bool theValue)
	{
		itsLookat.itsEnable = theValue;
	}
	
	/// <summary>
	/// Gets the information if lookat is enabled
	/// </summary>
	/// <remarks>
	/// Use this method to find out if lookat is enabled
	/// </remarks>
	/// <example>
	/// How to check if lookat is enabled at runtime
	/// <code>
	/// using UnityEngine;
	/// using System.Collections;
	/// 
	/// public class MyCameraController : MonoBehaviour
	/// {
	/// 	private KGFOrbitCam itsKGFOrbitCam;	//reference to the orbitcam
	///
	/// 	public void Start()
	/// 	{
	/// 		itsKGFOrbitCam = KGFAccessor.GetObject<KGFOrbitCam>();	//get the KGFOrbitcam by using the KGFAccessor class
	///
	/// 		if(itsKGFOrbitCam != null)
	/// 		{
	/// 			bool aLookatIsEnabled = itsKGFOrbitCam.GetLookatEnabled(); //check if lookat is enabled
	/// 			Debug.Log("lookat is enabled: "+aLookatIsEnabled);
	/// 		}
	/// 	}
	/// }
	/// </code>
	/// </example>
	/// <returns>bool: true if testing is enabled else false.</returns>
	public bool GetLookatEnabled()
	{
		return itsLookat.itsEnable;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="theObject"></param>
	[KGFEventExpose]
	public void SetLookatTarget(GameObject theObject)
	{
		itsLookat.itsLookatTarget = theObject;
		if(itsLookat.itsLookatTarget != null)
		{
			itsLookatTransform = itsLookat.itsLookatTarget.transform;
		}
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public GameObject GetLookatTarget()
	{
		return itsLookat.itsLookatTarget;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="theObject"></param>
	[KGFEventExpose]
	public void SetLookatUpVectorSource(GameObject theObject)
	{
		itsLookat.itsUpVectorSource = theObject;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public GameObject GetLookatUpVectorSource()
	{
		return itsLookat.itsUpVectorSource;
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="theValue"></param>
	[KGFEventExpose]
	public void SetLookatSpeed(float theValue)
	{
		itsLookat.itsLookatSpeed = theValue;
	}
	
	/// <summary>
	/// Gets the speed the camera is using to follow the lookat target if lookat is enabled
	/// </summary>
	/// <remarks>
	/// Use this method to find out how fast the camera is following the target if lookat is enabled and hardlink is disabled.
	/// </remarks>
	/// <example>
	/// How to check how fast the camera is following its lookat target at runtime (hard link must be disabled)
	/// <code>
	/// using UnityEngine;
	/// using System.Collections;
	/// 
	/// public class MyCameraController : MonoBehaviour
	/// {
	/// 	private KGFOrbitCam itsKGFOrbitCam;	//reference to the orbitcam
	///
	/// 	public void Start()
	/// 	{
	/// 		itsKGFOrbitCam = KGFAccessor.GetObject<KGFOrbitCam>();	//get the KGFOrbitcam by using the KGFAccessor class
	///
	/// 		if(itsKGFOrbitCam != null)
	/// 		{
	/// 			float aLookatSpeed = itsKGFOrbitCam.GetLookatSpeed(); //check how fast the camera is following its lookat target
	/// 			Debug.Log("canera lookat speed is: "+aLookatSpeed);
	/// 		}
	/// 	}
	/// }
	/// </code>
	/// </example>
	/// <returns>float: the speed the camera is following its lookat target.</returns>
	public float GetLookatSpeed()
	{
		return itsLookat.itsLookatSpeed;
	}
	
	[KGFEventExpose]
	public void SetLookatHardLinkToTarget(bool theValue)
	{
		itsLookat.itsHardLinkToTarget = theValue;
	}
	
	/// <summary>
	/// Gets the information if lookat is following the lookat target with or without smooth damping
	/// </summary>
	/// <remarks>
	/// Use this method to find out if the camera is following the lookat target smoothly or not.
	/// </remarks>
	/// <example>
	/// How to check if the camera is following the lookat target smoothly or not at runtime
	/// <code>
	/// using UnityEngine;
	/// using System.Collections;
	/// 
	/// public class MyCameraController : MonoBehaviour
	/// {
	/// 	private KGFOrbitCam itsKGFOrbitCam;	//reference to the orbitcam
	///
	/// 	public void Start()
	/// 	{
	/// 		itsKGFOrbitCam = KGFAccessor.GetObject<KGFOrbitCam>();	//get the KGFOrbitcam by using the KGFAccessor class
	///
	/// 		if(itsKGFOrbitCam != null)
	/// 		{
	/// 			bool anIsHardLinked = itsKGFOrbitCam.GetLookatHardLinkToTarget(); //check if lookat is hard linked to lookat target
	/// 			Debug.Log("lookat is hard linked: "+anIsHardLinked);
	/// 		}
	/// 	}
	/// }
	/// </code>
	/// </example>
	/// <returns>bool: true if camera is hard linked to lookat target (no smooth damping), else false.</returns>
	public bool GetLookatHardLinkToTarget()
	{
		return itsLookat.itsHardLinkToTarget;
	}
	
	#endregion

	#endregion
	
	/// <summary>
	/// Gets the position the camera is looking at if lookat is enabled
	/// </summary>
	/// <remarks>
	/// Use this method to find out the position the camera is looking at if lookat is enabled.
	/// </remarks>
	/// <example>
	/// How to check at which positinon the camera is looking if lookat is enabled at runtime
	/// <code>
	/// using UnityEngine;
	/// using System.Collections;
	/// 
	/// public class MyCameraController : MonoBehaviour
	/// {
	/// 	private KGFOrbitCam itsKGFOrbitCam;	//reference to the orbitcam
	///
	/// 	public void Start()
	/// 	{
	/// 		itsKGFOrbitCam = KGFAccessor.GetObject<KGFOrbitCam>();	//get the KGFOrbitcam by using the KGFAccessor class
	///
	/// 		if(itsKGFOrbitCam != null)
	/// 		{
	/// 			Vector3 aLookatPosition = itsKGFOrbitCam.GetLookatPositionCurrent(); //check the position the camere is currently looking at
	/// 			Debug.Log("canera is looking at position: "+aLookatPosition);
	/// 		}
	/// 	}
	/// }
	/// </code>
	/// </example>
	/// <returns>Vector3: the position where the camera is looking at in world space.</returns>
	public Vector3 GetLookatPositionCurrent()
	{
		return itsCurrentLookatPosition;
	}
	
	public void SetLookatPositionCurrent(Vector3 thePosition)
	{
		itsCurrentLookatPosition = thePosition;
	}
	
	
	#region save / load
	
	public KGFOrbitCamData SaveSettings()
	{
		KGFOrbitCamData aData = new KGFOrbitCamData();
		
		aData.itsRoot.itsTarget = itsTarget.itsTarget;
		aData.itsRoot.itsFollowPosition = itsTarget.itsFollowPosition;
		aData.itsRoot.itsPositionSpeed = itsTarget.itsPositionSpeed;
		aData.itsRoot.itsFollowRotation = itsTarget.itsFollowRotation;
		aData.itsRoot.itsRotationSpeed = itsTarget.itsRotationSpeed;
		
		aData.itsZoom.itsMinLimit = itsZoom.itsMinLimit;
		aData.itsZoom.itsMaxLimit = itsZoom.itsMaxLimit;
		aData.itsZoom.itsEnable = itsZoom.itsEnable;
		aData.itsZoom.itsStartValue = itsZoom.itsStartValue;
		aData.itsZoom.itsUseLimits = itsZoom.itsUseLimits;
		aData.itsZoom.itsZoomSpeed = itsZoom.itsZoomSpeed;
		
		aData.itsRotation.itsHorizontal.itsLeftLimit = itsRotation.itsHorizontal.itsLeftLimit;
		aData.itsRotation.itsHorizontal.itsRightLimit = itsRotation.itsHorizontal.itsRightLimit;
		aData.itsRotation.itsHorizontal.itsStartValue = itsRotation.itsHorizontal.itsStartValue;
		aData.itsRotation.itsHorizontal.itsEnable = itsRotation.itsHorizontal.itsEnable;
		aData.itsRotation.itsHorizontal.itsUseLimits = itsRotation.itsHorizontal.itsUseLimits;
		
		if(itsRotation.itsHorizontal.itsControls != null)
		{
			if(itsRotation.itsHorizontal.itsControls.Length != 0)
			{
				aData.itsRotation.itsHorizontal.itsControls = itsRotation.itsHorizontal.itsControls;
			}
		}
		
		aData.itsRotation.itsVertical.itsDownLimit = itsRotation.itsVertical.itsDownLimit;
		aData.itsRotation.itsVertical.itsUpLimit = itsRotation.itsVertical.itsUpLimit;
		aData.itsRotation.itsVertical.itsStartValue = itsRotation.itsVertical.itsStartValue;
		aData.itsRotation.itsVertical.itsEnable = itsRotation.itsVertical.itsEnable;
		aData.itsRotation.itsVertical.itsUseLimits = itsRotation.itsVertical.itsUseLimits;
		
		if(itsRotation.itsVertical.itsControls != null)
		{
			if(itsRotation.itsVertical.itsControls.Length != 0)
			{
				aData.itsRotation.itsVertical.itsControls = itsRotation.itsVertical.itsControls;
			}
		}
		
		aData.itsPanning.itsLeftRight.itsEnable = itsPanning.itsLeftRight.itsEnable;
		if(itsPanning.itsLeftRight.itsControls != null)
		{
			if(itsPanning.itsLeftRight.itsControls.Length != 0)
			{
				aData.itsPanning.itsLeftRight.itsControls = itsPanning.itsLeftRight.itsControls;
			}
		}
		aData.itsPanning.itsForwardBackward.itsEnable = itsPanning.itsForwardBackward.itsEnable;
		if(itsPanning.itsForwardBackward.itsControls != null)
		{
			if(itsPanning.itsForwardBackward.itsControls.Length != 0)
			{
				aData.itsPanning.itsForwardBackward.itsControls = itsPanning.itsForwardBackward.itsControls;
			}
		}
		aData.itsPanning.itsUpDown.itsEnable = itsPanning.itsUpDown.itsEnable;
		if(itsPanning.itsUpDown.itsControls != null)
		{
			if(itsPanning.itsUpDown.itsControls.Length != 0)
			{
				aData.itsPanning.itsUpDown.itsControls = itsPanning.itsUpDown.itsControls;
			}
		}
		
		aData.itsEnvironment.itsTestCollisions = itsEnvironment.itsTestCollisions;
		aData.itsEnvironment.itsCollisionLayer = itsEnvironment.itsCollisionLayer;
		aData.itsEnvironment.itsCollisionOffset = itsEnvironment.itsCollisionOffset;
		aData.itsEnvironment.itsFollowGround = itsEnvironment.itsFollowGround;
		
		aData.itsLookat.itsEnable = itsLookat.itsEnable;
		aData.itsLookat.itsLookatTarget = itsLookat.itsLookatTarget;
		aData.itsLookat.itsLookatSpeed = itsLookat.itsLookatSpeed;
		aData.itsLookat.itsUpVectorSource = itsLookat.itsUpVectorSource;
		aData.itsLookat.itsHardLinkToTarget = itsLookat.itsHardLinkToTarget;
		
		aData.itsCurrentZoom = GetZoomCurrent();
		aData.itsTargetZoom = GetZoom();
		aData.itsCurrentRotationLeftRight = GetRotationHorizontalCurrent();
		aData.itsTargetRotationLeftRight = GetRotationHorizontal();
		aData.itsCurrentRotationUpDown = GetRotationVerticalCurrent();
		aData.itsTargetRotationUpDown = GetRotationVertical();
		
		aData.itsCurrentLookatPosition = GetLookatPositionCurrent();
		
		return aData;
	}
	
	public void LoadSettings(KGFOrbitCamData theData)
	{
		SetTarget(theData.itsRoot.itsTarget);
		SetTargetFollowPosition (theData.itsRoot.itsFollowPosition);
		SetTargetFollowRotation (theData.itsRoot.itsFollowRotation);
		SetTargetFollowPositionSpeed (theData.itsRoot.itsPositionSpeed);
		SetTargetFollowRotationSpeed (theData.itsRoot.itsRotationSpeed);
		
		SetZoomMinLimit (theData.itsZoom.itsMinLimit);
		SetZoomMaxLimit (theData.itsZoom.itsMaxLimit);
		SetZoom (theData.itsTargetZoom);
		SetZoomEnable(theData.itsZoom.itsEnable);
		SetZoomUseLimits (theData.itsZoom.itsUseLimits);
		SetZoomSpeed (theData.itsZoom.itsZoomSpeed);
		SetZoomStartValue(theData.itsZoom.itsStartValue);
		SetZoomControls(theData.itsZoom.itsControls);

		SetRotationHorizontalLeftLimit (theData.itsRotation.itsHorizontal.itsLeftLimit);
		SetRotationHorizontalRightLimit (theData.itsRotation.itsHorizontal.itsRightLimit);
		SetRotationHorizontal (theData.itsTargetRotationLeftRight);
		SetRotationHorizontalEnable (theData.itsRotation.itsHorizontal.itsEnable);
		SetRotationHorizontalUseLimits (theData.itsRotation.itsHorizontal.itsUseLimits);
		SetRotationHorizontalStartValue(theData.itsRotation.itsHorizontal.itsStartValue);
		SetRotationHorizontalControls(theData.itsRotation.itsHorizontal.itsControls);

		SetRotationVerticalDownLimit (theData.itsRotation.itsVertical.itsDownLimit);
		SetRotationVerticalUpLimit(theData.itsRotation.itsVertical.itsUpLimit);
		SetRotationVertical (theData.itsTargetRotationUpDown);
		SetRotationVerticalEnable (theData.itsRotation.itsVertical.itsEnable);
		SetRotationVerticalUseLimits(theData.itsRotation.itsVertical.itsUseLimits);
		SetRotationVerticalStartValue(theData.itsRotation.itsVertical.itsStartValue);
		SetRotationVerticalControls(theData.itsRotation.itsVertical.itsControls);
		
		SetPanningLeftRightEnable(theData.itsPanning.itsLeftRight.itsEnable);
		SetPanningLeftRightControls(theData.itsPanning.itsLeftRight.itsControls);
		SetPanningForwardBackwardEnable(theData.itsPanning.itsForwardBackward.itsEnable);
		SetPanningForwardBackwardControls(theData.itsPanning.itsForwardBackward.itsControls);
		SetPanningUpDownEnable(theData.itsPanning.itsUpDown.itsEnable);
		SetPanningUpDownControls(theData.itsPanning.itsUpDown.itsControls);
		
		SetEnviromentTestCollisionsEnable (theData.itsEnvironment.itsTestCollisions);
		SetEnviromentCollisionLayers (theData.itsEnvironment.itsCollisionLayer);
		SetEnviromentCollisionOffset (theData.itsEnvironment.itsCollisionOffset);
		SetEnviromentFollowGroundEnable (theData.itsEnvironment.itsFollowGround);
		
//		SetHoritzontalAxisInvert (theData.itsRotation.itsHorizontal.itsInvertAxis);
//		SetVerticalAxisInvert(theData.itsRotation.itsVertical.itsInvertAxis);
//		SetHorizontalAxisName(theData.itsRotation.itsHorizontal.itsAxisName);
//		SetVerticalAxisName(theData.itsRotation.itsVertical.itsAxisName);
//		SetZoomAxisName(theData.itsZoom.itsAxisName);
//		SetHoritzontalAxisSensitivity(theData.itsRotation.itsHorizontal.itsAxisSensitivity);
//		SetVerticalAxisSensitivity(theData.itsRotation.itsVertical.itsAxisSensitivity);
//		SetXAxisSensitivityTouch(theData.itsRotation.itsHorizontal.itsAxisSensitivityTouch);
//		SetYAxisSensitivityTouch(theData.itsRotation.itsVertical.itsAxisSensitivityTouch);
//		SetTwoFingerRotateTouch(theData.itsGlobalSettings.itsTwoFingerRotateTouch);
//		SetZoomAxisSensitivity(theData.itsZoom.itsAxisSensitivity);

		SetLookatEnable(theData.itsLookat.itsEnable);
		SetLookatTarget (theData.itsLookat.itsLookatTarget);
		SetLookatSpeed (theData.itsLookat.itsLookatSpeed);
		SetLookatUpVectorSource (theData.itsLookat.itsUpVectorSource);
		SetLookatHardLinkToTarget(theData.itsLookat.itsHardLinkToTarget);
	}
	
	#endregion
	
	
	
	

	#region validate

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public KGFMessageList Validate ()
	{
		KGFMessageList aMessageList = new KGFMessageList ();

		//target
		if (itsTarget.itsTarget == null)
		{
			aMessageList.AddError ("itsTarget should not be empty");
		}
		if (itsTarget.itsPositionSpeed <= 0)
		{
			aMessageList.AddError ("itsLinkedTargetPositionSpeed has invalid value, has to be > 0");
		}
		if (itsTarget.itsRotationSpeed <= 0)
		{
			aMessageList.AddError ("itsLinkedTargetRotationSpeed has invalid value, has to be > 0");
		}
		
		//Zoom
		if (itsZoom.itsZoomSpeed <= 0)
		{
			aMessageList.AddError ("itsZoomSpeed has invalid value, has to be > 0");
		}
		if (itsZoom.itsStartValue < itsZoom.itsMinLimit)
		{
			aMessageList.AddError ("itsStartZoom should not be smaller than itsMinZoom");
		}
		if (itsZoom.itsStartValue > itsZoom.itsMaxLimit)
		{
			aMessageList.AddError ("itsStartZoom should not be bigger than itsMaxZoom");
		}
		if (itsZoom.itsMinLimit > itsZoom.itsMaxLimit)
		{
			aMessageList.AddError ("itsMinZoom should not be bigger than itsMaxZoom");
		}
		if (itsZoom.itsMaxLimit < itsZoom.itsMinLimit)
		{
			aMessageList.AddError ("itsMaxZoom should not be smaller than itsMinZoom");
		}
		if (itsZoom.itsMinLimit < 0)
		{
			aMessageList.AddError ("itsMinZoom should not be smaller than 0");
		}

		//Rotation Up Down
		if (itsRotation.itsVertical.itsDownLimit < 0)
		{
			aMessageList.AddError ("itsRotation itsUpDown itsDownLimit should not be smaller than 0");
		}
		if (itsRotation.itsVertical.itsUpLimit < 0)
		{
			aMessageList.AddError ("itsRotation itsUpDown itsUpLimit should not be smaller than 0");
		}
		

		//Rotation Left Right
		if (itsRotation.itsHorizontal.itsLeftLimit < 0)
		{
			aMessageList.AddError ("itsRotation itsLeftRight itsLeftLimit should not be smaller than 0");
		}
		if (itsRotation.itsHorizontal.itsRightLimit < 0)
		{
			aMessageList.AddError ("itsRotation itsLeftRight itsRightLimit should not be smaller than 0");
		}
		
		
		
		//Lookat
		if (itsLookat.itsLookatSpeed <= 0)
		{
			aMessageList.AddError ("itsLookatSpeed has invalid value, has to be > 0");
		}
		if (itsLookat.itsEnable)
		{
			if(itsLookat.itsLookatTarget == null)
			{
				aMessageList.AddError ("itsLookat is Enabled but Lookat target has not been assigned");
			}
			if(itsLookat.itsUpVectorSource == null)
			{
				aMessageList.AddError ("itsLookat is Enabled but up vector source has not been assigned");
			}
		}


		aMessageList.AddMessages(KGFOrbitCam.ValidateControls(itsRotation.itsHorizontal.itsControls,"itsRotation.itsHorizontal.itsControls").GetAllMessagesArray());
		aMessageList.AddMessages(KGFOrbitCam.ValidateControls(itsRotation.itsVertical.itsControls,"itsRotation.itsVertical.itsControls").GetAllMessagesArray());
		aMessageList.AddMessages(KGFOrbitCam.ValidateControls(itsZoom.itsControls,"itsZoom.itsControls").GetAllMessagesArray());
		aMessageList.AddMessages(KGFOrbitCam.ValidateControls(itsPanning.itsForwardBackward.itsControls,"itsPanning.itsForwardBackward.itsControls").GetAllMessagesArray());
		aMessageList.AddMessages(KGFOrbitCam.ValidateControls(itsPanning.itsLeftRight.itsControls,"itsPanning.itsLeftRight.itsControls").GetAllMessagesArray());
		aMessageList.AddMessages(KGFOrbitCam.ValidateControls(itsPanning.itsUpDown.itsControls,"itsPanning.itsUpDown.itsControls").GetAllMessagesArray());

		return aMessageList;
	}
	
	/// <summary>
	/// validate controls
	/// </summary>
	/// <param name="theControlAxis"></param>
	/// <returns></returns>
	public static KGFMessageList ValidateControls(control_axis[] theControlAxis, string theDebugPrefix)
	{
		KGFMessageList aMessageList = new KGFMessageList();
		for(int i = 0; i< theControlAxis.Length; i++)
		{
			control_axis aControlAxis = theControlAxis[i];
			if(aControlAxis.itsAxisSensitivity < 0)
			{
				aMessageList.AddError ("control axis sensitivity cannot be smaller than 0!");
			}
			
			bool anException = false;
			try
			{
				Input.GetAxis(aControlAxis.itsAxisName);
			}
			catch(Exception theException)
			{
				Debug.LogError("KGFOrbitcamException: "+theException);
				anException = true;
			}
			if(anException)
			{
				aMessageList.AddError(theDebugPrefix+"["+i+"].itsAxisName: '"+aControlAxis.itsAxisName+"' is not defined in unity input settings");
			}
		}
		return aMessageList;
	}

	#endregion
}
