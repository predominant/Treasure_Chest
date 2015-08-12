//please uncomment the following line if you own the PlayMaker package
//#define PLAYMAKER

using UnityEngine;
using System.Collections;
using System;

#if PLAYMAKER

using HutongGames.PlayMaker;

#region KGFCutscene
[ActionCategory("KGFCutscene")]
public class KGFCutsceneStart : FsmStateAction
{
	[RequiredField]
	public KGFCutscene CutScene;
	public override void Reset ()
	{
		CutScene = null;
	}
	public override void OnEnter ()
	{
		if (CutScene != null)
		{
			CutScene.StartCutscene();
		}
		Finish();
	}
}

[ActionCategory("KGFCutscene")]
public class KGFCutsceneStop : FsmStateAction
{
	[RequiredField]
	public KGFCutscene CutScene;
	public override void Reset ()
	{
		CutScene = null;
	}
	public override void OnEnter ()
	{
		if (CutScene != null)
		{
			CutScene.StopCutscene();
		}
		Finish();
	}
}

[ActionCategory("KGFCutscene")]
public class KGFCutsceneSetColor : FsmStateAction
{
	[RequiredField]
	public KGFCutscene CutScene;
	public Color CutsceneColor;
	public override void Reset ()
	{
		CutScene = null;
	}
	public override void OnEnter ()
	{
		if (CutScene != null)
		{
			CutScene.SetColor(CutsceneColor);
		}
		Finish();
	}
}
#endregion
#endif