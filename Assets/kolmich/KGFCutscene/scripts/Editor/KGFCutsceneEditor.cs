// <author>Alexander Murauer</author>
// <email>alexander.murauer@kolmich.at</email>
// <date>2013-08-05</date>

using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(KGFCutscene))]
public class KGFCutsceneEditor : KGFEditor
{
	protected override void CustomGui()
	{
		base.CustomGui();
		
		KGFCutscene aTarget = (KGFCutscene)target;
		
		if (Application.isPlaying)
		{
			if (GUILayout.Button("Start"))
				aTarget.StartCutscene();
			if (GUILayout.Button("Stop"))
				aTarget.StopCutscene();
		}
		
		// error checking
		if (aTarget.itsSize < 0)
		{
			aTarget.itsSize = 0;
			EditorUtility.SetDirty(aTarget);
		}
		if (aTarget.itsSize > 0.5f)
		{
			aTarget.itsSize = 0.5f;
			EditorUtility.SetDirty(aTarget);
		}
	}
	
	#region validation
	public static KGFMessageList ValidateKGFCutsceneEditor(object theObject)
	{
		return new KGFMessageList();
	}
	#endregion
}
