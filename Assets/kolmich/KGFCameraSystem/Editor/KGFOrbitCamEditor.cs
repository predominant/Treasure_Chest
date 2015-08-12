using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(KGFOrbitCam))]
public class KGFOrbitCamEditor : KGFEditor
{
	public static KGFMessageList ValidateKGFOrbitCamEditor(UnityEngine.Object theTarget)
	{
		KGFMessageList aMessageList = KGFEditor.ValidateKGFEditor(theTarget);
		return aMessageList;
	}
	
	protected override void CustomGui()
	{
		base.CustomGui();
		if(!Application.isPlaying)
		{
			KGFOrbitCam anOrbitCam = (KGFOrbitCam)target;
			anOrbitCam.Start();
		}
	}
	
	void OnEnable()
	{
		KGFOrbitCam anOrbitCam = target as KGFOrbitCam;
		if(anOrbitCam.itsRotation.itsHorizontal.itsControls.Length == 0)
		{
			anOrbitCam.itsRotation.itsHorizontal.itsControls = new KGFOrbitCam.control_axis[1];
			anOrbitCam.itsRotation.itsHorizontal.itsControls[0] = new KGFOrbitCam.control_axis();
			anOrbitCam.itsRotation.itsHorizontal.itsControls[0].itsAxisName = "Mouse X";
			anOrbitCam.itsRotation.itsHorizontal.itsControls[0].itsAxisSensitivity = 1.0f;
			anOrbitCam.itsRotation.itsHorizontal.itsControls[0].itsInvertAxis = false;
			anOrbitCam.itsRotation.itsHorizontal.itsControls[0].itsAdditionalKey = KeyCode.None;
			anOrbitCam.itsRotation.itsHorizontal.itsControls[0].itsResetRotationOnRelease = false;
		}
		if(anOrbitCam.itsRotation.itsVertical.itsControls.Length == 0)
		{
			anOrbitCam.itsRotation.itsVertical.itsControls = new KGFOrbitCam.control_axis[1];
			anOrbitCam.itsRotation.itsVertical.itsControls[0] = new KGFOrbitCam.control_axis();
			anOrbitCam.itsRotation.itsVertical.itsControls[0].itsAxisName = "Mouse Y";
			anOrbitCam.itsRotation.itsVertical.itsControls[0].itsAxisSensitivity = 1.0f;
			anOrbitCam.itsRotation.itsVertical.itsControls[0].itsInvertAxis = false;
			anOrbitCam.itsRotation.itsVertical.itsControls[0].itsAdditionalKey = KeyCode.None;
			anOrbitCam.itsRotation.itsVertical.itsControls[0].itsResetRotationOnRelease = false;
		}
		if(anOrbitCam.itsZoom.itsControls.Length == 0)
		{
			anOrbitCam.itsZoom.itsControls = new KGFOrbitCam.control_axis[1];
			anOrbitCam.itsZoom.itsControls[0] = new KGFOrbitCam.control_axis();
			anOrbitCam.itsZoom.itsControls[0].itsAxisName = "Mouse ScrollWheel";
			anOrbitCam.itsZoom.itsControls[0].itsAxisSensitivity = 1.0f;
			anOrbitCam.itsZoom.itsControls[0].itsInvertAxis = false;
			anOrbitCam.itsZoom.itsControls[0].itsAdditionalKey = KeyCode.None;
			anOrbitCam.itsZoom.itsControls[0].itsResetRotationOnRelease = false;
		}
	}
}

