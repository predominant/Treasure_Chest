using UnityEngine;
using System.Collections;




public class KGFOrbitCamSettings : KGFObject, KGFIValidator
{
	[HideInInspector]
	public Camera itsPreviewCamera = null; //preview camera used in editor time to preview settings
	
	[System.Serializable]
	public class camera_event_orbiter_settings
	{
		public KGFOrbitCam itsOrbitCam = null;
	}

	[System.Serializable]
	public class camera_event_transform_target_settings
	{
		public GameObject itsTransformTarget = null;
	}
	
	public camera_event_orbiter_settings itsOrbitCam = new camera_event_orbiter_settings();
//	public camera_event_transform_target_settings itsTransformTarget = new camera_event_transform_target_settings();

	public KGFOrbitCam.camera_target_settings itsTarget = new KGFOrbitCam.camera_target_settings();
	public KGFOrbitCam.camera_zoom_settings itsZoom = new KGFOrbitCam.camera_zoom_settings();
	public KGFOrbitCam.camera_rotation_settings itsRotation = new KGFOrbitCam.camera_rotation_settings();
	public KGFOrbitCam.camera_panning_settings itsPanning = new KGFOrbitCam.camera_panning_settings();
	public KGFOrbitCam.camera_terrain_settings itsEnvironment = new KGFOrbitCam.camera_terrain_settings();
	public KGFOrbitCam.camera_lookat_settings itsLookat = new KGFOrbitCam.camera_lookat_settings();
	public KGFOrbitCam.camera_settings itsCamera = new KGFOrbitCam.camera_settings();
	public KGFOrbitCam.camera_global_settings itsGlobalSettings = new KGFOrbitCam.camera_global_settings();

	[HideInInspector]
	public bool itsGameStarted = false;

	protected override void Awake()
	{
		base.Awake();
		Camera aPreviewCamera = gameObject.GetComponent<Camera>();
		if(aPreviewCamera != null)
		{			
			Destroy(aPreviewCamera);
		}
	}
	
	public void Start ()
	{
		itsGameStarted = true;
	}

	public void Update ()
	{
	}


	public void Apply ()
	{
		float aTransformTargetAngleX = 0;
		float aTransformTargetAngleY = 0;
		float aZoom = 0;

//		if(itsTransformTarget.itsTransformTarget != null && itsTarget.itsTarget != null)
//		{
//			aTransformTargetVector = -1 * (itsTransformTarget.itsTransformTarget.transform.position - itsTarget.itsTarget.transform.position).normalized;
//			aTransformTargetRotation = Quaternion.LookRotation (aTransformTargetVector);
//			aTransformTargetAngleX = aTransformTargetRotation.eulerAngles.x;
//			aTransformTargetAngleY = -aTransformTargetRotation.eulerAngles.y;
//			aZoom = Vector3.Distance(itsTransformTarget.itsTransformTarget.transform.position, itsTarget.itsTarget.transform.position);
//		}

		if (itsOrbitCam.itsOrbitCam != null)
		{
//			itsOrbitCam.itsOrbitCam.SetLinkTargetRotationForCorrectQuaternionLerping();
			itsOrbitCam.itsOrbitCam.SetCameraFieldOfView(itsCamera.itsFieldOfView);
			
			//target
			itsOrbitCam.itsOrbitCam.SetTargetFollowPosition (itsTarget.itsFollowPosition);
			itsOrbitCam.itsOrbitCam.SetTargetFollowRotation (itsTarget.itsFollowRotation);
			itsOrbitCam.itsOrbitCam.SetTargetFollowPositionSpeed (itsTarget.itsPositionSpeed);
			itsOrbitCam.itsOrbitCam.SetTargetFollowRotationSpeed (itsTarget.itsRotationSpeed);
			
			//zoom
			itsOrbitCam.itsOrbitCam.SetZoomMinLimit (itsZoom.itsMinLimit);
			itsOrbitCam.itsOrbitCam.SetZoomMaxLimit (itsZoom.itsMaxLimit);
			if(aZoom != 0)
			{
				itsOrbitCam.itsOrbitCam.SetZoomStartValue(aZoom);
				itsOrbitCam.itsOrbitCam.SetZoom (aZoom);
			}
			else
			{
				itsOrbitCam.itsOrbitCam.SetZoomStartValue(itsZoom.itsStartValue);
				itsOrbitCam.itsOrbitCam.SetZoom (itsZoom.itsStartValue);
			}
			itsOrbitCam.itsOrbitCam.SetZoomEnable(itsZoom.itsEnable);
			itsOrbitCam.itsOrbitCam.SetZoomUseLimits (itsZoom.itsUseLimits);
			itsOrbitCam.itsOrbitCam.SetZoomSpeed (itsZoom.itsZoomSpeed);
			
			if(itsZoom.itsControls != null)	//only override controls if defined
			{
				if(itsZoom.itsControls.Length != 0)	//only override controls if defined
				{
					itsOrbitCam.itsOrbitCam.SetZoomControls(itsZoom.itsControls);
				}
			}			
			itsOrbitCam.itsOrbitCam.SetZoomControlsTouch(itsZoom.itsControlTouch);
			
			//rotation horizontal
			itsOrbitCam.itsOrbitCam.SetRotationHorizontalLeftLimit (itsRotation.itsHorizontal.itsLeftLimit);
			itsOrbitCam.itsOrbitCam.SetRotationHorizontalRightLimit (itsRotation.itsHorizontal.itsRightLimit);
			itsOrbitCam.itsOrbitCam.SetRotationHorizontalStartValue(itsRotation.itsHorizontal.itsStartValue - aTransformTargetAngleY);
			itsOrbitCam.itsOrbitCam.SetRotationHorizontal (itsRotation.itsHorizontal.itsStartValue - aTransformTargetAngleY);
			itsOrbitCam.itsOrbitCam.SetRotationHorizontalEnable (itsRotation.itsHorizontal.itsEnable);
			itsOrbitCam.itsOrbitCam.SetRotationHorizontalUseLimits (itsRotation.itsHorizontal.itsUseLimits);
			if(itsRotation.itsHorizontal.itsControls != null)	//only override controls if defined
			{
				if(itsRotation.itsHorizontal.itsControls.Length != 0)	//only override controls if defined
				{
					itsOrbitCam.itsOrbitCam.SetRotationHorizontalControls(itsRotation.itsHorizontal.itsControls);
				}
			}
			itsOrbitCam.itsOrbitCam.SetRotationHorizontalControlTouch(itsRotation.itsHorizontal.itsControlTouch);

			//rotation vertical
			itsOrbitCam.itsOrbitCam.SetRotationVerticalDownLimit (itsRotation.itsVertical.itsDownLimit);
			itsOrbitCam.itsOrbitCam.SetRotationVerticalUpLimit(itsRotation.itsVertical.itsUpLimit);
			itsOrbitCam.itsOrbitCam.SetRotationVerticalStartValue(itsRotation.itsVertical.itsStartValue + aTransformTargetAngleX);
			itsOrbitCam.itsOrbitCam.SetRotationVertical (itsRotation.itsVertical.itsStartValue + aTransformTargetAngleX);
			itsOrbitCam.itsOrbitCam.SetRotationVerticalEnable (itsRotation.itsVertical.itsEnable);
			itsOrbitCam.itsOrbitCam.SetRotationVerticalUseLimits (itsRotation.itsVertical.itsUseLimits);
			if(itsRotation.itsVertical.itsControls != null)
			{
				if(itsRotation.itsVertical.itsControls.Length != 0)	//only override controls if defined
				{
					itsOrbitCam.itsOrbitCam.SetRotationVerticalControls(itsRotation.itsVertical.itsControls);
				}
			}
			itsOrbitCam.itsOrbitCam.SetRotationVerticalControlTouch(itsRotation.itsVertical.itsControlTouch);
			
			//panning
			itsOrbitCam.itsOrbitCam.SetPanningBounds(itsPanning.itsBounds);
			itsOrbitCam.itsOrbitCam.SetPanningSpeed(itsPanning.itsSpeed);
			itsOrbitCam.itsOrbitCam.SetPanningUseCameraSpace(itsPanning.itsUseCameraSpace);
			itsOrbitCam.itsOrbitCam.SetPanningLeftRightEnable(itsPanning.itsLeftRight.itsEnable);
			itsOrbitCam.itsOrbitCam.SetPanningForwardBackwardEnable(itsPanning.itsForwardBackward.itsEnable);
			itsOrbitCam.itsOrbitCam.SetPanningUpDownEnable(itsPanning.itsUpDown.itsEnable);
			if(itsPanning.itsLeftRight.itsControls != null)	//only override controls if defined
			{
				if(itsPanning.itsLeftRight.itsControls.Length != 0)	//only override controls if defined
				{
					itsOrbitCam.itsOrbitCam.SetPanningLeftRightControls(itsPanning.itsLeftRight.itsControls);
				}
			}
			itsOrbitCam.itsOrbitCam.SetPanningLeftRightControlTouch(itsPanning.itsLeftRight.itsControlTouch);
			
			if(itsPanning.itsForwardBackward.itsControls != null)	//only override controls if defined
			{
				if(itsPanning.itsForwardBackward.itsControls.Length != 0)	//only override controls if defined
				{
					itsOrbitCam.itsOrbitCam.SetPanningForwardBackwardControls(itsPanning.itsForwardBackward.itsControls);
				}
			}
			itsOrbitCam.itsOrbitCam.SetPanningForwardBackwardControlTouch(itsPanning.itsForwardBackward.itsControlTouch);
			
			if(itsPanning.itsUpDown.itsControls != null)	//only override controls if defined
			{
				if(itsPanning.itsUpDown.itsControls.Length != 0)	//only override controls if defined
				{
					itsOrbitCam.itsOrbitCam.SetPanningUpDownControls(itsPanning.itsUpDown.itsControls);
				}
			}
			itsOrbitCam.itsOrbitCam.SetPanningUpDownControlTouch(itsPanning.itsUpDown.itsControlTouch);

			//enviroment
			itsOrbitCam.itsOrbitCam.SetEnviromentTestCollisionsEnable (itsEnvironment.itsTestCollisions);
			itsOrbitCam.itsOrbitCam.SetEnviromentCollisionLayers (itsEnvironment.itsCollisionLayer);
			itsOrbitCam.itsOrbitCam.SetEnviromentCollisionOffset (itsEnvironment.itsCollisionOffset);
			itsOrbitCam.itsOrbitCam.SetEnviromentFollowGroundEnable (itsEnvironment.itsFollowGround);
//			itsOrbitCam.itsOrbitCam.UseBoxCollision(itsEnvironment.itsUseBoxCollision);
//			itsOrbitCam.itsOrbitCam.UseRayCollision(itsEnvironment.itsUseRayCollision);

			itsOrbitCam.itsOrbitCam.SetLookatEnable(itsLookat.itsEnable);
			itsOrbitCam.itsOrbitCam.SetLookatTarget (itsLookat.itsLookatTarget);
			itsOrbitCam.itsOrbitCam.SetLookatSpeed (itsLookat.itsLookatSpeed);
			itsOrbitCam.itsOrbitCam.SetLookatUpVectorSource (itsLookat.itsUpVectorSource);
			itsOrbitCam.itsOrbitCam.SetLookatHardLinkToTarget(itsLookat.itsHardLinkToTarget);
			
			itsOrbitCam.itsOrbitCam.SetTarget(itsTarget.itsTarget);
		}
	}

	private void PositionPreviewCamera(float theZoom, float theHorizontalRotation, float theVerticalRotation, Vector3 theTargetPosition, Quaternion theTargetRotation)
	{
		if(itsTarget.itsTarget == null)
			return;
		
		float aTranformPointHorizontalAngle = (theHorizontalRotation) * Mathf.PI / 180f;
		float aTranformPointVerticalAngle = (theVerticalRotation) * Mathf.PI / 180f;
		
		Vector3 aTransformPoint = new Vector3 ();
		aTransformPoint.z = theZoom * Mathf.Sin (aTranformPointVerticalAngle) * Mathf.Cos (aTranformPointHorizontalAngle);
		aTransformPoint.x = theZoom * Mathf.Sin (aTranformPointVerticalAngle) * Mathf.Sin (aTranformPointHorizontalAngle);
		aTransformPoint.y = theZoom * Mathf.Cos (aTranformPointVerticalAngle);
		aTransformPoint = theTargetPosition + theTargetRotation * aTransformPoint;
		transform.position = aTransformPoint;
		
		Vector3 aForwardVector = Vector3.zero;
		if(itsLookat.itsEnable && itsLookat.itsLookatTarget != null && itsTarget.itsTarget != null)
		{
			aForwardVector = itsLookat.itsLookatTarget.transform.position - transform.position;
			transform.rotation = Quaternion.LookRotation(aForwardVector.normalized,itsTarget.itsTarget.transform.up);
		}
		else
		{
			aForwardVector = theTargetPosition - transform.position;
			transform.rotation = Quaternion.LookRotation(aForwardVector.normalized,itsTarget.itsTarget.transform.up);
		}
		if(itsPreviewCamera != null)
			itsPreviewCamera.fieldOfView = itsCamera.itsFieldOfView;
		
	}
	
	void OnDrawGizmosSelected ()//DrawDebugRotation ()
	{
		if(itsTarget.itsTarget == null)
			return;
		
		float itsCurrentZoom = itsZoom.itsStartValue;
		float itsRotationLeftRight = itsRotation.itsHorizontal.itsStartValue;
		float itsRotationUpDown = itsRotation.itsVertical.itsStartValue;



		if (itsOrbitCam.itsOrbitCam != null)
		{
			itsCurrentZoom = itsZoom.itsStartValue;
			itsRotationLeftRight = itsRotation.itsHorizontal.itsStartValue;
			itsRotationUpDown = itsRotation.itsVertical.itsStartValue;
		}

		
		//draw target item to lookat target, or if not available to root target
		Vector3 aTargetGizmoPosition = Vector3.zero;
		if (itsLookat.itsLookatTarget != null/* && itsLookat.itsLookatTarget != itsRoot.itsRoot  && itsLookat.itsEnable*/)
		{
			aTargetGizmoPosition = itsLookat.itsLookatTarget.transform.position;
			Gizmos.DrawIcon (aTargetGizmoPosition, "eye.png", true);
		}
		if (itsTarget.itsTarget != null)
		{
			aTargetGizmoPosition = itsTarget.itsTarget.transform.position;
			Gizmos.DrawIcon (aTargetGizmoPosition, "target_64.png", true);
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
		if (itsTarget.itsTarget != null)
		{
			aTargetPosition = itsTarget.itsTarget.transform.position;
		}
		else
		{
			aTargetPosition = Vector3.zero;
		}
		
		
		if (itsTarget.itsTarget != null)
		{
			aTargetRotation = itsTarget.itsTarget.transform.localRotation;
		}
		
		PositionPreviewCamera(itsCurrentZoom,itsRotationLeftRight + aTargetHorizontalAngle,(itsRotation.itsVertical.itsStartValue) + 270f,aTargetPosition,aTargetRotation);
		
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
				aHorizontalAngle = (itsRotationLeftRight + aTargetHorizontalAngle) * Mathf.PI / 180f;
				
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
			aHorizontalAngle = (itsRotationLeftRight + aTargetHorizontalAngle) * Mathf.PI / 180f;
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
				aVerticalAngle = (90 - itsRotationUpDown + aTargetVerticalAngle) * Mathf.PI / 180f;
				
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
			aVerticalAngle = (90 - itsRotationUpDown) * Mathf.PI / 180f;
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
			aTargetVector = (itsTarget.itsTarget.transform.position - transform.position).normalized;
			Vector3 aStartPoint = aTargetPosition + itsZoom.itsMinLimit * aTargetVector;
			Vector3 aEndPoint = aTargetPosition + itsZoom.itsMaxLimit * aTargetVector;
			Gizmos.color = new Color (0, 0, 1);
			
			
			aVerticalAngle = (90f - itsRotationUpDown + aTargetVerticalAngle) * Mathf.PI / 180f;
			aHorizontalAngle = (itsRotationLeftRight + 180f + aTargetHorizontalAngle) * Mathf.PI / 180f;
			
			aPoint = new Vector3 ();
			aPoint.z = itsCurrentZoom * Mathf.Sin (aVerticalAngle) * Mathf.Cos (aHorizontalAngle);
			aPoint.x = itsCurrentZoom * Mathf.Sin (aVerticalAngle) * Mathf.Sin (aHorizontalAngle);
			aPoint.y = itsCurrentZoom * Mathf.Cos (aVerticalAngle);
			
			
			Quaternion aTargetRotation2 = Quaternion.Euler (aVerticalAngle, 0f, 0f);
			Quaternion aFinalRotation = aTargetRotation;
			aPoint = aTargetPosition + aFinalRotation * aPoint;
			
			aTargetVector = (aPoint - aTargetPosition).normalized;
			aStartPoint = aTargetPosition + itsZoom.itsMinLimit * aTargetVector;
			aEndPoint = aTargetPosition + itsZoom.itsStartValue * aTargetVector;
			//Gizmos.DrawIcon (aEndPoint, "camera_small.png", true);

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
			
			
			if (!itsZoom.itsUseLimits)
			{
				//Calculate normals for zoom line
				aVerticalAngle = (itsRotationUpDown + aTargetVerticalAngle) * Mathf.PI / 180f;
				aHorizontalAngle = (itsRotationLeftRight + aTargetHorizontalAngle) * Mathf.PI / 180f;
				
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
			
			
			aVerticalAngle = (90f - itsRotationUpDown + aTargetVerticalAngle) * Mathf.PI / 180f;
			aHorizontalAngle = (itsRotationLeftRight + 180f + aTargetHorizontalAngle) * Mathf.PI / 180f;
			
			aPoint = new Vector3 ();
			aPoint.z = itsCurrentZoom * Mathf.Sin (aVerticalAngle) * Mathf.Cos (aHorizontalAngle);
			aPoint.x = itsCurrentZoom * Mathf.Sin (aVerticalAngle) * Mathf.Sin (aHorizontalAngle);
			aPoint.y = itsCurrentZoom * Mathf.Cos (aVerticalAngle);
			
			Quaternion aFinalRotation = aTargetRotation;
			aPoint = aTargetPosition + aFinalRotation * aPoint;
			
			aTargetVector = (aPoint - itsLookat.itsLookatTarget.transform.position).normalized;
			aStartPoint = itsLookat.itsLookatTarget.transform.position;
			aEndPoint = aPoint;
			Gizmos.color = new Color(1,1,0);
			Gizmos.DrawLine (aStartPoint, aEndPoint);
		}
	}

	#region validate
	
	public KGFMessageList Validate ()
	{
		KGFMessageList aMessageList = new KGFMessageList ();

		//Orbiter
		if (itsOrbitCam.itsOrbitCam == null)
		{
			aMessageList.AddError ("itsOrbitCam should not be empty");
		}

		//Root
		if (itsTarget.itsTarget == null)
		{
			aMessageList.AddError ("itsTarget should not be empty");
		}
		if (itsTarget.itsTarget == this.gameObject)
		{
			aMessageList.AddError ("itsTarget cannot be th KGFOrbitCamSetting itself");
		}
		if (itsTarget.itsPositionSpeed <= 0)
		{
			aMessageList.AddError ("itsLinkedTargetPositionSpeed has invalid value, has to be > 0");
		}
		if (itsTarget.itsRotationSpeed <= 0)
		{
			aMessageList.AddError ("itsLinkedTargetRotationSpeed has invalid value, has to be > 0");
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
			aMessageList.AddError ("itsMaxZoon should not be smaller than itsMinZoom");
		}
		if (itsZoom.itsMinLimit < 0)
		{
			aMessageList.AddError ("itsMinZoon should not be smaller than 0");
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
		
		aMessageList.AddMessages(KGFOrbitCam.ValidateControls(itsRotation.itsHorizontal.itsControls,"itsRotation.itsHorizontal.itsControls").GetAllMessagesArray());
		aMessageList.AddMessages(KGFOrbitCam.ValidateControls(itsRotation.itsVertical.itsControls,"itsRotation.itsVertical.itsControls").GetAllMessagesArray());
		aMessageList.AddMessages(KGFOrbitCam.ValidateControls(itsZoom.itsControls,"itsZoom.itsControls").GetAllMessagesArray());
		aMessageList.AddMessages(KGFOrbitCam.ValidateControls(itsPanning.itsForwardBackward.itsControls,"itsPanning.itsForwardBackward.itsControls").GetAllMessagesArray());
		aMessageList.AddMessages(KGFOrbitCam.ValidateControls(itsPanning.itsLeftRight.itsControls,"itsPanning.itsLeftRight.itsControls").GetAllMessagesArray());
		aMessageList.AddMessages(KGFOrbitCam.ValidateControls(itsPanning.itsUpDown.itsControls,"itsPanning.itsUpDown.itsControls").GetAllMessagesArray());
		
		return aMessageList;
	}
	
	#endregion
}
