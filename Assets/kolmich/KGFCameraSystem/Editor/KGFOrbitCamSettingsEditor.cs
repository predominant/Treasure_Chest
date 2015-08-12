using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(KGFOrbitCamSettings))]
public class KGFOrbitCamSettingsEditor : KGFEditor
{
	public static KGFMessageList ValidateKGFOrbitCamSettingsEditor(UnityEngine.Object theTarget)
	{
		KGFMessageList aMessageList = KGFEditor.ValidateKGFEditor(theTarget);
		return aMessageList;
	}

	protected override void CustomGui ()
	{
		KGFOrbitCamSettings itsOrbitCamPosition = (KGFOrbitCamSettings)target;
		base.CustomGui ();
		if (GUILayout.Button ("Apply Settings"))
		{
			if (itsOrbitCamPosition.itsGameStarted)
			{
				itsOrbitCamPosition.Apply ();
			}
		}

//		if (itsOrbitCamPosition.itsRoot.itsRoot == null)
//		{
//			itsOrbitCamPosition.itsRoot.itsRoot = itsOrbitCamPosition.gameObject;
//			EditorUtility.SetDirty(itsOrbitCamPosition);
//		}
		
		if(!Application.isPlaying)
		{
			KGFOrbitCamSettings anOrbitCamSetting = (KGFOrbitCamSettings)target;
			anOrbitCamSetting.itsPreviewCamera = anOrbitCamSetting.gameObject.GetComponent<Camera>();
			if(anOrbitCamSetting.itsPreviewCamera == null)
			{
				anOrbitCamSetting.itsPreviewCamera = anOrbitCamSetting.gameObject.AddComponent<Camera>();
				anOrbitCamSetting.itsPreviewCamera.hideFlags = HideFlags.HideAndDontSave;
			}
			else
			{
				anOrbitCamSetting.itsPreviewCamera.hideFlags = HideFlags.HideAndDontSave;
			}
		}
	}
}
