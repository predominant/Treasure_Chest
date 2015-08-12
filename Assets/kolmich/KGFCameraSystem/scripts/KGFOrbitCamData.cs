using UnityEngine;
using System.Collections;

public class KGFOrbitCamData {
	
	[HideInInspector]
	public KGFOrbitCam.camera_target_settings itsRoot = new KGFOrbitCam.camera_target_settings();
	[HideInInspector]
	public KGFOrbitCam.camera_zoom_settings itsZoom = new KGFOrbitCam.camera_zoom_settings();
	[HideInInspector]
	public KGFOrbitCam.camera_rotation_settings itsRotation = new KGFOrbitCam.camera_rotation_settings();
	[HideInInspector]
	public KGFOrbitCam.camera_panning_settings itsPanning = new KGFOrbitCam.camera_panning_settings();
	[HideInInspector]
	public KGFOrbitCam.camera_terrain_settings itsEnvironment = new KGFOrbitCam.camera_terrain_settings();
	[HideInInspector]
	public KGFOrbitCam.camera_lookat_settings itsLookat = new KGFOrbitCam.camera_lookat_settings();
	[HideInInspector]
	public KGFOrbitCam.camera_global_settings itsGlobalSettings = new KGFOrbitCam.camera_global_settings();
	[HideInInspector]
	public Vector3 itsCurrentLookatPosition;

	[HideInInspector]
	public float itsCurrentRotationLeftRight;
	[HideInInspector]
	public float itsTargetRotationLeftRight;
	[HideInInspector]
	public float itsCurrentRotationUpDown;
	[HideInInspector]
	public float itsTargetRotationUpDown;
	[HideInInspector]
	public float itsCurrentZoom;
	[HideInInspector]
	public float itsTargetZoom;
}
