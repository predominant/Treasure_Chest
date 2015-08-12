
//please uncomment the following line if you own the PlayMaker package
//#define PLAYMAKER


using UnityEngine;
using System.Collections;
using System;




#if PLAYMAKER

using HutongGames.PlayMaker;

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSettingsApply : FsmStateAction
{
	public KGFOrbitCamSettings itsOrbitCamSettings;
	public override void Reset ()
	{
		itsOrbitCamSettings = null;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCamSettings == null)
		{
			LogError("OrbitCamSettings is null");
		}
		else
		{
			itsOrbitCamSettings.Apply();
		}
		Finish();
	}
}

#region target
[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetTarget : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmOwnerDefault itsTarget;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsTarget = null;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else if(itsTarget.GameObject.Value == null)
		{
			LogError("itsTarget is null");
		}
		else
		{
			itsOrbitCam.SetTarget(itsTarget.GameObject.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetTarget : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmOwnerDefault itsTarget;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsTarget = null;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsTarget.GameObject.Value = itsOrbitCam.GetTarget();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetTargetFollowPosition : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmBool itsFollowTargetPosition = false;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsFollowTargetPosition = false;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetTargetFollowPosition(itsFollowTargetPosition.Value);
		}
		Finish ();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetTargetFollowPosition : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmBool itsFollowTargetPosition = false;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsFollowTargetPosition = false;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsFollowTargetPosition.Value = itsOrbitCam.GetTargetFollowPosition();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetTargetFollowPositionSpeed : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsTargetFollowPositionSpeed;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsTargetFollowPositionSpeed = 1;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetTargetFollowPositionSpeed(itsTargetFollowPositionSpeed.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetTargetFollowPositionSpeed : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsTargetFollowPositionSpeed;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsTargetFollowPositionSpeed = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsTargetFollowPositionSpeed.Value = itsOrbitCam.GetTargetFollowPositionSpeed();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetTargetFollowRotation : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmBool itsTargetFollowRotation = false;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsTargetFollowRotation = false;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetTargetFollowRotation(itsTargetFollowRotation.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetTargetFollowRotation : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmBool itsTargetFollowRotation = false;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsTargetFollowRotation = false;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsTargetFollowRotation.Value = itsOrbitCam.GetTargetFollowRotation();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetTargetFollowRotationSpeed : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsTargetFollowRotationSpeed;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsTargetFollowRotationSpeed = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetTargetFollowRotationSpeed(itsTargetFollowRotationSpeed.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetTargetFollowRotationSpeed : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsTargetFollowRotationSpeed;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsTargetFollowRotationSpeed = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsTargetFollowRotationSpeed.Value = itsOrbitCam.GetTargetFollowRotationSpeed();
		}
		Finish();
	}
}
#endregion

#region zoom
[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetZoom : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsZoom;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsZoom = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetZoom(itsZoom.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetZoom : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsZoom;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsZoom = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsZoom.Value = itsOrbitCam.GetZoom();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetZoomCurrent : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsZoomCurrent;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsZoomCurrent = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetZoomCurrent(itsZoomCurrent.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetZoomCurrent : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsZoomCurrent;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsZoomCurrent = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsZoomCurrent.Value = itsOrbitCam.GetZoomCurrent();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetZoomEnable : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmBool itsZoomEnable;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsZoomEnable = false;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetZoomEnable(itsZoomEnable.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetZoomEnable : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmBool itsZoomEnable;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsZoomEnable = false;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsZoomEnable.Value = itsOrbitCam.GetZoomEnable();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetZoomMaxLimit : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsZoomMaxLimit;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsZoomMaxLimit = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetZoomMaxLimit(itsZoomMaxLimit.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetZoomMaxLimit : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsZoomMaxLimit;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsZoomMaxLimit = 0.0f;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsZoomMaxLimit.Value = itsOrbitCam.GetZoomMaxLimit();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetZoomMinLimit : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsZoomMinLimit;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsZoomMinLimit = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetZoomMinLimit(itsZoomMinLimit.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetZoomMinLimit : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsZoomMinLimit;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsZoomMinLimit = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsZoomMinLimit.Value = itsOrbitCam.GetZoomMinLimit();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetZoomSpeed : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsZoomSpeed;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsZoomSpeed = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetZoomSpeed(itsZoomSpeed.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetZoomSpeed : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsZoomSpeed;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsZoomSpeed = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsZoomSpeed.Value = itsOrbitCam.GetZoomSpeed();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetZoomStartValue : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsZoomStartValue;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsZoomStartValue = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetZoomStartValue(itsZoomStartValue.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetZoomStartValue : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsZoomStartValue;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsZoomStartValue = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsZoomStartValue.Value = itsOrbitCam.GetZoomStartValue();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetZoomUseLimits : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmBool itsZoomUseLimits;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsZoomUseLimits = false;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsZoomUseLimits.Value = itsOrbitCam.GetZoomUseLimits();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetZoomUseLimits : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmBool itZoomUseLimits;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itZoomUseLimits = false;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetZoomUseLimits(itZoomUseLimits.Value);
		}
		Finish();
	}
}
#endregion

#region enviroment
[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetEnviromentCollisionOffset : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsCollisionOffset;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsCollisionOffset = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetEnviromentCollisionOffset(itsCollisionOffset.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetEnviromentCollisionOffset : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsCollisionOffset;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsCollisionOffset = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsCollisionOffset.Value = itsOrbitCam.GetEnviromentCollisionOffset();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetEnviromentFollowGroundEnabled : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmBool itsFollowGroundEnabled;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsFollowGroundEnabled = false;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetEnviromentFollowGroundEnable(itsFollowGroundEnabled.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetEnviromentFollowGroundEnabled : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmBool itsFollowGroundEnabled;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsFollowGroundEnabled = false;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsFollowGroundEnabled.Value = itsOrbitCam.GetEnviromentFollowGroundEnabled();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetEnviromentTestCollisionsEnabled : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmBool itsCollisionEnabled;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsCollisionEnabled = false;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetEnviromentTestCollisionsEnable(itsCollisionEnabled.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetEnviromentTestCollisionsEnabled : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmBool itsCollisionEnabled;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsCollisionEnabled = false;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsCollisionEnabled.Value = itsOrbitCam.GetEnviromentTestCollisionsEnabled();
		}
		Finish();
	}
}
#endregion

#region look at
[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetLookatEnabled : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmBool itsLookatEnabled;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsLookatEnabled = false;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetLookatEnable(itsLookatEnabled.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetLookatEnabled : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmBool itsLookatEnabled;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsLookatEnabled = false;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsLookatEnabled.Value = itsOrbitCam.GetLookatEnabled();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetLookatHardLinkToTarget : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmBool itsLookatHardlinkEnabled;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsLookatHardlinkEnabled = false;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetLookatHardLinkToTarget(itsLookatHardlinkEnabled.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetLookatHardLinkToTarget : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmBool itsLookatHardlinkEnabled;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsLookatHardlinkEnabled = false;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsLookatHardlinkEnabled.Value = itsOrbitCam.GetLookatHardLinkToTarget();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetLookatSpeed : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsLookatSpeed;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsLookatSpeed = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetLookatSpeed(itsLookatSpeed.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetLookatSpeed : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsLookatSpeed;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsLookatSpeed = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsLookatSpeed.Value = itsOrbitCam.GetLookatSpeed();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetLookatTarget : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmOwnerDefault itsLookatTarget;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsLookatTarget = null;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetLookatTarget(itsLookatTarget.GameObject.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetLookatTarget : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmOwnerDefault itsLookatTarget;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsLookatTarget = null;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else if(itsLookatTarget.GameObject.Value == null)
		{
			LogError("itsLookatTarget is null");
		}
		else
		{
			itsLookatTarget.GameObject.Value = itsOrbitCam.GetLookatTarget();
		}
		Finish ();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetLookatUpVectorSource : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmOwnerDefault itsUpVectorSource;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsUpVectorSource = null;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else if(itsUpVectorSource.GameObject.Value == null)
		{
			LogError("itsUpVectorSource is null");
		}
		else
		{
			itsOrbitCam.SetLookatUpVectorSource(itsUpVectorSource.GameObject.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetLookatUpVectorSource : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmOwnerDefault itsUpVectorSource;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsUpVectorSource = null;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsUpVectorSource.GameObject.Value = itsOrbitCam.GetLookatUpVectorSource();
		}
		Finish();
	}
}
#endregion

#region events
[ActionCategory("KGFCameraSystem")]
public class KGFCameraSystemEventTargetReached : FsmStateAction
{
	[RequiredField]
	public KGFOrbitCam itsOrbitCam;
	public FsmEvent EventOnTargetReached;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
	}
	
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.EventTargetReached += OnTargetReached;
		}
	}
	
	public override void OnExit()
	{
		if (itsOrbitCam != null)
		{
			itsOrbitCam.EventTargetReached -= OnTargetReached;
		}
	}
	
	void OnTargetReached(object theSender, EventArgs theArgs)
	{
		Fsm.Event(EventOnTargetReached);
		Finish();
	}
	
	public override string ErrorCheck()
	{
		if (itsOrbitCam == null)
		{
			return "itsOrbitCam has to be filled in";
		}
		return null;
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFCameraSystemEventTargetChanged : FsmStateAction
{
	[RequiredField]
	public KGFOrbitCam itsOrbitCam;
	public FsmEvent EventOnTargetChanged;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
	}
	
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.EventTargetChanged += OnTargetChanged;
		}
	}
	
	public override void OnExit()
	{
		if (itsOrbitCam != null)
		{
			itsOrbitCam.EventTargetChanged -= OnTargetChanged;
		}
	}
	
	void OnTargetChanged(object theSender, EventArgs theArgs)
	{
		Fsm.Event(EventOnTargetChanged);
		Finish();
	}
	
	public override string ErrorCheck()
	{
		if (itsOrbitCam == null)
		{
			return "itsOrbitCam has to be filled in";
		}
		return null;
	}
}
#endregion

#region rotation vertical
[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetRotationVerticalUpLimit : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsRotationVerticalUpLimit;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsRotationVerticalUpLimit = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsRotationVerticalUpLimit.Value = itsOrbitCam.GetRotationVerticalUpLimit();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetRotationVerticalUpLimit : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsRotationVerticalUpLimit;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsRotationVerticalUpLimit = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetRotationVerticalUpLimit(itsRotationVerticalUpLimit.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetRotationVerticalDownLimit : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsRotationVerticalDownLimit;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsRotationVerticalDownLimit = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsRotationVerticalDownLimit.Value = itsOrbitCam.GetRotationVerticalDownLimit();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetRotationVerticalDownLimit : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsRotationVerticalDownLimit;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsRotationVerticalDownLimit = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetRotationVerticalDownLimit(itsRotationVerticalDownLimit.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetRotationVertical : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsRotationVertical;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsRotationVertical = 0.0f;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsRotationVertical.Value = itsOrbitCam.GetRotationVertical();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetRotationVertical : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsRotationVertical;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsRotationVertical = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetRotationVertical(itsRotationVertical.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetRotationVerticalCurrent : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsRotationVerticalCurrent;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsRotationVerticalCurrent = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsRotationVerticalCurrent.Value = itsOrbitCam.GetRotationVerticalCurrent();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetRotationVerticalCurrent : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsRotationVerticalCurrent;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsRotationVerticalCurrent = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetRotationVerticalCurrent(itsRotationVerticalCurrent.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetRotationVerticalStartValue : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsRotationVerticalStartValue;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsRotationVerticalStartValue = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsRotationVerticalStartValue.Value = itsOrbitCam.GetRotationVerticalStartValue();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetRotationVerticalStartValue : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsRotationVerticalStartValue;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsRotationVerticalStartValue = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetRotationVerticalStartValue(itsRotationVerticalStartValue.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetRotationVerticalEnable : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmBool itsRotationVerticalEnable;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsRotationVerticalEnable = false;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsRotationVerticalEnable.Value = itsOrbitCam.GetRotationVerticalEnable();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetRotationVerticalEnable : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmBool itsRotationVerticalEnable;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsRotationVerticalEnable = false;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetRotationVerticalEnable(itsRotationVerticalEnable.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetRotationVerticalUseLimits : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmBool itsRotationVerticalUseLimits;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsRotationVerticalUseLimits = false;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsRotationVerticalUseLimits.Value = itsOrbitCam.GetRotationVerticalUseLimits();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetRotationVerticalUseLimits : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmBool itsRotationVerticalUseLimits;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsRotationVerticalUseLimits = false;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetRotationVerticalUseLimits(itsRotationVerticalUseLimits.Value);
		}
		Finish();
	}
}
#endregion

#region rotation Horizontal
[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetRotationHorizontalLeftLimit : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsRotationHorizontalLeftLimit;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsRotationHorizontalLeftLimit = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsRotationHorizontalLeftLimit.Value = itsOrbitCam.GetRotationHorizontalLeftLimit();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetRotationHorizontalLeftLimit : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsRotationHorizontalLeftLimit;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsRotationHorizontalLeftLimit = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetRotationHorizontalLeftLimit(itsRotationHorizontalLeftLimit.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetRotationHorizontalRightLimit : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsRotationHorizontalRightLimit;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsRotationHorizontalRightLimit = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsRotationHorizontalRightLimit.Value = itsOrbitCam.GetRotationHorizontalRightLimit();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetRotationHorizontalRightLimit : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsRotationHorizontalRightLimit;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsRotationHorizontalRightLimit = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetRotationHorizontalRightLimit(itsRotationHorizontalRightLimit.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetRotationHorizontal : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsRotationHorizontal;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsRotationHorizontal = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsRotationHorizontal.Value = itsOrbitCam.GetRotationHorizontal();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetRotationHorizontal : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsRotationHorizontal;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsRotationHorizontal = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetRotationHorizontal(itsRotationHorizontal.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetRotationHorizontalCurrent : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsRotationHorizontalCurrent;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsRotationHorizontalCurrent = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsRotationHorizontalCurrent.Value = itsOrbitCam.GetRotationHorizontalCurrent();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetRotationHorizontalCurrent : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsRotationHorizontalCurrent;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsRotationHorizontalCurrent = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetRotationHorizontalCurrent(itsRotationHorizontalCurrent.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetRotationHorizontalStartValue : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsRotationHorizontalStartValue;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsRotationHorizontalStartValue = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsRotationHorizontalStartValue.Value = itsOrbitCam.GetRotationHorizontalStartValue();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetRotationHorizontalStartValue : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsRotationHorizontalStartValue;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsRotationHorizontalStartValue = 0;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetRotationHorizontalStartValue(itsRotationHorizontalStartValue.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetRotationHorizontalEnable : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmBool itsRotationHorizontalEnable;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsRotationHorizontalEnable = false;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsRotationHorizontalEnable.Value = itsOrbitCam.GetRotationHorizontalEnable();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetRotationHorizontalEnable : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmBool itsRotationHorizontalEnable;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsRotationHorizontalEnable = false;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetRotationHorizontalEnable(itsRotationHorizontalEnable.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetRotationHorizontalUseLimits : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmBool itsRotationHorizontalUseLimits;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsRotationHorizontalUseLimits = false;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsRotationHorizontalUseLimits.Value = itsOrbitCam.GetRotationHorizontalUseLimits();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetRotationHorizontalUseLimits : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmBool itsRotationHorizontalUseLimits;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsRotationHorizontalUseLimits = false;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetRotationHorizontalUseLimits(itsRotationHorizontalUseLimits.Value);
		}
		Finish();
	}
}
#endregion

#region panning
[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetPanningLeftRightEnable : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmBool itsPanningLeftRightEnable;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsPanningLeftRightEnable = false;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetPanningLeftRightEnable(itsPanningLeftRightEnable.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetPanningLeftRightEnable : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmBool itsPanningLeftRightEnable;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsPanningLeftRightEnable = false;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsPanningLeftRightEnable.Value = itsOrbitCam.GetPanningLeftRightEnable();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetPanningUpDownEnable : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmBool itsPanningUpDownEnable;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsPanningUpDownEnable = false;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetPanningUpDownEnable(itsPanningUpDownEnable.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetPanningUpDownEnable : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmBool itsPanningUpDownEnable;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsPanningUpDownEnable = false;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsPanningUpDownEnable.Value = itsOrbitCam.GetPanningUpDownEnable();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetPanningForwardBackwardEnable : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmBool itsPanningForwardBackwardEnable;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsPanningForwardBackwardEnable = false;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetPanningForwardBackwardEnable(itsPanningForwardBackwardEnable.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetPanningForwardBackwardEnable : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmBool itsPanningForwardBackwardEnable;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsPanningForwardBackwardEnable = false;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsPanningForwardBackwardEnable.Value = itsOrbitCam.GetPanningForwardBackwardEnable();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetPanningHasBounds : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmBool itsPanningHasBounds;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsPanningHasBounds = false;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsPanningHasBounds.Value = itsOrbitCam.GetPanningHasBounds();
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamSetPanningSpeed : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsPanningSpeed;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsPanningSpeed = 0.0f;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsOrbitCam.SetPanningSpeed(itsPanningSpeed.Value);
		}
		Finish();
	}
}

[ActionCategory("KGFCameraSystem")]
public class KGFOrbitCamGetPanningSpeed : FsmStateAction
{
	public KGFOrbitCam itsOrbitCam;
	public FsmFloat itsPanningSpeed;
	
	public override void Reset ()
	{
		itsOrbitCam = null;
		itsPanningSpeed = 0.0f;
	}
	public override void OnEnter ()
	{
		if(itsOrbitCam == null)
		{
			LogError("itsOrbitCam is null");
		}
		else
		{
			itsPanningSpeed.Value = itsOrbitCam.GetPanningSpeed();
		}
		Finish();
	}
}
#endregion

#endif