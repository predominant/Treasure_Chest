// <author>Alexander Murauer</author>
// <email>alexander.murauer@kolmich.at</email>
// <date>2014-05-12</date>

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(KGFTouch))]
public class KGFTouchEditor : KGFEditor
{
	// display event sender
	bool itsOptions = false;
	// display disabled areas
	bool itsDisabled = false;
	
	// event swipe
	bool itsOptionsSwipe = false;
	int itsSwipeArea = KGFTouch.AreaIDFullScreen;
	Vector2 itsSwipePosition = Vector2.zero;
	KGFTouch.eSwipeType itsSwipeType = KGFTouch.eSwipeType.None;
	
	// event pan
	bool itsOptionsPan = false;
	int itsPanArea = KGFTouch.AreaIDFullScreen;
	int itsPanFingerCount = 1;
	Vector2[] itsPanFingerDiffs = new Vector2[0];
	
	// event pinch
	bool itsOptionsPinch = false;
	int itsPinchArea = KGFTouch.AreaIDFullScreen;
	Vector2 itsPinchCenter = Vector2.zero;
	Vector2 itsPinchDelta1 = Vector2.zero;
	Vector2 itsPinchDelta2 = Vector2.zero;
	float itsPinchDeltaValue = 0;
	
	KGFTouch itsModuleTouch;
	
	protected override void CustomGui()
	{
		if (Application.isPlaying)
		{
			itsModuleTouch = (KGFTouch)target;
			
			if (itsModuleTouch.itsDebugMode)
			{
				StartBlock();
				itsOptions = EditorGUILayout.Foldout(itsOptions,"Debug Events");
				if (itsOptions)
				{
					DisplayOptions();
				}
				EndBlock();
				
				StartBlock();
				itsDisabled = EditorGUILayout.Foldout(itsDisabled,"Disabled Areas");
				if (itsDisabled)
				{
					DisplayDisabledAreas();
				}
				EndBlock();
			}
		}
	}
	
	void DisplayDisabledAreas()
	{
		StartBlock();
		int [] aListAreaIDs = itsModuleTouch.GetRegisteredTouchAreaIDs();
		for (int i=0;i<aListAreaIDs.Length;i++)
		{
			int anAreaID = aListAreaIDs[i];
			if (!itsModuleTouch.GetTouchAreaActive(anAreaID))
			{
				EditorGUILayout.LabelField(itsModuleTouch.GetRegisteredAreaName(anAreaID));
			}
		}
		EndBlock();
	}
	
	void DisplayOptions()
	{
		StartBlock();
		// event swipe
		itsOptionsSwipe = EditorGUILayout.Foldout(itsOptionsSwipe,"OnEventSwipe");
		if (itsOptionsSwipe)
		{
			StartBlock();
			itsSwipeArea = DisplayAreaPopup(itsSwipeArea);
			itsSwipePosition = EditorGUILayout.Vector2Field("ScreenPosition",itsSwipePosition);
			itsSwipeType = (KGFTouch.eSwipeType)EditorGUILayout.EnumPopup("SwipeType",itsSwipeType);
			
			if (GUILayout.Button("Send"))
				itsModuleTouch.OnEventSwipe(new KGFTouch.SwipeArgs(itsSwipeArea,itsSwipePosition,itsSwipeType));
			EndBlock();
		}
		
		// event pan
		itsOptionsPan = EditorGUILayout.Foldout(itsOptionsPan,"OnEventPan");
		if (itsOptionsPan)
		{
			StartBlock();
			itsPanArea = DisplayAreaPopup(itsPanArea);
			itsPanFingerCount = EditorGUILayout.IntSlider("FingerCount",itsPanFingerCount,1,10);
			if (itsPanFingerDiffs.Length != itsPanFingerCount)
				Array.Resize(ref itsPanFingerDiffs,itsPanFingerCount);
			
			for (int i=0;i<itsPanFingerCount;i++)
				itsPanFingerDiffs[i] = EditorGUILayout.Vector2Field("FingerDiff:"+i,itsPanFingerDiffs[i]);
			
			if (GUILayout.Button("Send"))
				itsModuleTouch.OnEventPan(new KGFTouch.PanArgs(itsPanArea,itsPanFingerDiffs));
			EndBlock();
		}
		
		// event pinch
		itsOptionsPinch = EditorGUILayout.Foldout(itsOptionsPinch,"OnEventPinch");
		if (itsOptionsPinch)
		{
			StartBlock();
			itsPinchArea = DisplayAreaPopup(itsPinchArea);
			itsPinchCenter = EditorGUILayout.Vector2Field("PointCenter",itsPinchCenter);
			itsPinchDelta1 = EditorGUILayout.Vector2Field("Touch1Delta",itsPinchDelta1);
			itsPinchDelta2 = EditorGUILayout.Vector2Field("Touch2Delta",itsPinchDelta2);
			itsPinchDeltaValue = EditorGUILayout.FloatField("TouchDistance",itsPinchDeltaValue);
			
			if (GUILayout.Button("Send"))
				itsModuleTouch.OnEventPinch(new KGFTouch.PinchArgs(itsPinchArea,itsPinchCenter,itsPinchDelta1,itsPinchDelta2,itsPinchDeltaValue));
			EndBlock();
		}
		EndBlock();
	}
	
	int DisplayAreaPopup(int theAreaID)
	{
		List<int> aListIDs = new List<int>(itsModuleTouch.GetRegisteredTouchAreaIDs());
		List<string> aListNames = new List<string>();
		
		foreach (int anID in aListIDs)
		{
			aListNames.Add(itsModuleTouch.GetRegisteredAreaName(anID));
		}
		
		// add item for fullscreen
		aListIDs.Add(KGFTouch.AreaIDFullScreen);
		aListNames.Add("FullScreen");
		
		Color aColorSave = GUI.color;
		GUI.color = itsModuleTouch.GetRegisteredAreaColor(theAreaID);
		theAreaID = EditorGUILayout.IntPopup("AreaName",theAreaID,aListNames.ToArray(),aListIDs.ToArray());
		GUI.color = aColorSave;
		
		return theAreaID;
	}
	
	void StartBlock()
	{
		EditorGUILayout.BeginHorizontal();
		GUILayout.Space(10);
		EditorGUILayout.BeginVertical();
	}
	
	void EndBlock()
	{
		EditorGUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();
	}
}
