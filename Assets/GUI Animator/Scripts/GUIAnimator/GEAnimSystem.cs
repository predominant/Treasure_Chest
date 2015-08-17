// GUI Animator for NGUI version: 0.8.3
// Author: Gold Experience Team (http://www.ge-team.com/pages/)
// Support: geteamdev@gmail.com
// Please direct any bugs/comments/suggestions to geteamdev@gmail.com

#region Namespaces

using UnityEngine;
using System.Collections;

using UnityEngine.UI;

#endregion

#region EaseType

// Common ease type for iTween, Hotween, LeanTween
// Hotween EaseType: http://www.holoville.com/hotween/hotweenAPI/namespace_holoville_1_1_h_o_tween.html#ab8f6c428f087160deca07d7d402c4934
// LeanTween EaseType: http://dentedpixel.com/LeanTweenDocumentation/classes/LeanTweenType.html
// iTween EaseType: http://itween.pixelplacement.com/documentation.php

// Here is a good reference for what you can expect from each ease type:
// http://www.robertpenner.com/easing/easing_demo.html
public enum eEaseType
{
	InQuad,
	OutQuad,
	InOutQuad,
	InCubic,
	OutCubic,
	InOutCubic,
	InQuart,
	OutQuart,
	InOutQuart,
	InQuint,
	OutQuint,
	InOutQuint,
	InSine,
	OutSine,
	InOutSine,
	InExpo,
	OutExpo,
	InOutExpo,
	InCirc,
	OutCirc,
	InOutCirc,
	linear,
	spring,
	InBounce,
	OutBounce,
	InOutBounce,
	InBack,
	OutBack,
	InOutBack,
	InElastic,
	OutElastic,
	InOutElastic
}

#endregion

#region Alignment

public enum eAlignment
{
	Current,
	TopLeft,
	TopCenter,
	TopRight,
	LeftCenter,
	Center,
	RightCenter,
	BottomLeft,
	BottomCenter,
	BottomRight
}

#endregion

#region Move In/Out

public enum ePosMoveIn
{
	From_ParentPosition,
	From_SpecificPosition,
	
	From_UpperScreenEdge,
	From_LeftScreenEdge,
	From_RightScreenEdge,
	From_BottomScreenEdge,
	
	From_UpperLeft,
	From_UpperCenter,
	From_UpperRight,
	From_LeftCenter,
	From_Center,
	From_RightCenter,
	From_BottomLeft,
	From_BottomCenter,
	From_BottomRight,
	
	From_CurrentPos
}

public enum ePosMoveOut
{
	To_ParentPosition,
	To_SpecificPosition,
	
	To_UpperScreenEdge,
	To_LeftScreenEdge,
	To_RightScreenEdge,
	To_BottomScreenEdge,
	
	To_UpperLeft,
	To_UpperCenter,
	To_UpperRight,
	To_LeftCenter,
	To_Center,
	To_RightCenter,
	To_BottomLeft,
	To_BottomCenter,
	To_BottomRight,
	
	To_CurrentPos
}

// Move-In information
[System.Serializable]
public class cMoveIn
{
	public bool Enable = false;
	
	[HideInInspector]
	public bool Began = false;
	[HideInInspector]
	public bool Done = false;
	public ePosMoveIn MoveFrom = ePosMoveIn.From_UpperScreenEdge;
	//[HideInInspector]
	public Vector3 Position;
	public eEaseType EaseType = eEaseType.OutBack;
	public float Time = 1.0f;
	public float Delay;
}

// Move-Out information
[System.Serializable]
public class cMoveOut
{
	public bool Enable = false;
	[HideInInspector]
	public bool Began = false;
	[HideInInspector]
	public bool Done = false;
	public ePosMoveOut MoveTo = ePosMoveOut.To_UpperScreenEdge;
	//[HideInInspector]
	public Vector3 Position;
	public eEaseType EaseType = eEaseType.InBack;
	public float Time = 1.0f;
	public float Delay;
}

#endregion

#region Scale In/Out

// Scale-In information
[System.Serializable]
public class cScaleIn
{
	public bool Enable = false;
	[HideInInspector]
	public bool Began = false;
	[HideInInspector]
	public bool Done = false;
	public Vector3 ScaleBegin = new Vector3(0,0,0);
	public eEaseType EaseType = eEaseType.OutBack;
	public float Time = 1.0f;
	public float Delay;
}

// Scale-Out information
[System.Serializable]
public class cScaleOut
{
	public bool Enable = false;
	[HideInInspector]
	public bool Began = false;
	[HideInInspector]
	public bool Done = false;
	public Vector3 ScaleEnd = new Vector3(0,0,0);
	public eEaseType EaseType = eEaseType.InBack;
	public float Time = 1.0f;
	public float Delay;
}

#endregion

#region Fade In/Out

// Fade In/Out information
[System.Serializable]
public class cFade
{
	public bool Enable = false;
	[HideInInspector]
	public bool Began = false;
	[HideInInspector]
	public bool Done = false;
	[HideInInspector]
	public float Fade = 0;
	public eEaseType EaseType = eEaseType.linear;
	public float Time = 1.0f;
	public float Delay;
	public bool FadeChildren = false;
}

#endregion

#region Scale Loop

// Scale-Loop information
[System.Serializable]
public class cPingPongScale
{
	public bool Enable = false;
	[HideInInspector]
	public bool Began = false;
	[HideInInspector]
	public bool Done = false;
	public Vector3 Min = new Vector3(1,1,1);
	public Vector3 Max = new Vector3(1.05f,1.05f,1.05f);
	public eEaseType EaseType = eEaseType.linear;
	public float Time = 2.0f;
	public float Delay;
}

#endregion

#region Fade Loop

// Fade-Loop information
[System.Serializable]
public class cPingPongFade
{
	public bool Enable = false;
	[HideInInspector]
	public bool Began = false;
	[HideInInspector]
	public bool Done = false;
	public float Min = 0.0f;
	public float Max = 1.0f;
	public eEaseType EaseType = eEaseType.linear;
	public float Time = 2.0f;
	public float Delay;
	public bool FadeChildren = false;
}

#endregion

#region Move Hierachy

public enum eGUIMove
{
	Self,
	Children,
	SelfAndChildren
}

#endregion

/**************
* GEAnimSystem class
* This is Singleton class. It is handle GUI speed for all GEAnim, 
* test mode in Unity Editor, delay for loading new level, recursive move-in/out function and Camera utilities.
**************/

#region GEAnimSystem

public class GEAnimSystem : MonoBehaviour
{
	#region Variables

	// Private reference which can be accessed by this class only
	private static GEAnimSystem instance;

	// Public static reference that can be accesd from anywhere
	public static GEAnimSystem Instance
	{
		get
		{
			// Check if instance has not been set yet and set it it is not set already
			// This takes place only on the first time usage of this reference
			if(instance==null)
			{
				instance = GameObject.FindObjectOfType<GEAnimSystem>();
				DontDestroyOnLoad(instance.gameObject);
			}
			return instance;
		}
	}

	// Public property which can be accessed using the instance
	[Range(0.5f,8.0f)]
	public float m_GUISpeed = 1.0f;

	#if UNITY_EDITOR
	public bool m_TestMoveIn = true;	
	public float m_TestIdleTime = 8.0f;		// Amount of time to idle before run MoveOut. This value will be used when m_TestMoveIn and m_TestMoveOut are true.
	public bool m_TestMoveOut = true;
	#endif
	
	#endregion
	
	// ######################################################################
	// MonoBehaviour Functions
	// ######################################################################
	
	#region MonoBehaviour
	
	// Awake is called when the script instance is being loaded.
	void Awake()
	{		
		if(instance == null)
		{
			// Make the current instance as the singleton
			instance = this;

			// Make it persistent  
			DontDestroyOnLoad(this);
		}
		else
		{
			// If more than one singleton exists in the scene find the existing reference from the scene and destroy it
			if(this != instance)
			{
				Destroy(this.gameObject);
			}
		}
	}
	
	// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
	void Start ()
	{
		
	}
	
	// Update is called every frame, if the MonoBehaviour is enabled.
	void Update ()
	{
		
	}
	
	#endregion
	
	// ######################################################################
	// Load Level Functions
	// ######################################################################
	
	#region Load Level

	// Load Level with a delay time
	public void LoadLevel(string LevelName, float delay)
	{
		if(delay<=0)
		{
			Application.LoadLevel(LevelName);
		}
		else
		{
			StartCoroutine(LoadLevelDelay(LevelName, delay));
		}
	}
	
	// Load Level with a delay time
	IEnumerator LoadLevelDelay(string LevelName, float delay)
	{
		yield return new WaitForSeconds(delay);
		
		Application.LoadLevel(LevelName);
	}
	
	#endregion
	
	// ######################################################################
	// Move In/Out Functions
	// ######################################################################
	
	#region Move In/Out

	// Recursive Move-In function
	public void MoveIn(Transform trans, bool IsRecursive)
	{
		GEAnim pGUIAnim = trans.gameObject.GetComponent<GEAnim>();
		if(pGUIAnim!=null)
		{
			pGUIAnim.MoveIn();
		}
		
		Button pButton = trans.gameObject.GetComponent<Button>();
		if(pButton!=null)
		{
			pButton.enabled = true;
		}

		if(IsRecursive==true)
		{
			foreach(Transform child in trans)
			{
				MoveIn(child, IsRecursive);
			}
		}
	}
	
	// Recursive Move-Out function
	public void MoveOut(Transform trans, bool IsRecursive)
	{
		GEAnim pGUIAnim = trans.gameObject.GetComponent<GEAnim>();
		if(pGUIAnim!=null)
		{
			pGUIAnim.MoveOut();
		}

		Button pButton = trans.gameObject.GetComponent<Button>();
		if(pButton!=null)
		{
			pButton.enabled = false;
		}

		if(IsRecursive==true)
		{
			foreach(Transform child in trans)
			{
				MoveOut(child, IsRecursive);
			}
		}
	}
	
	#endregion
	
	// ######################################################################
	// Enable/Disable Button Functions
	// ######################################################################
	
	#region Enable/Disable Button
	
	// Recursive enable/disable all buttons
	public void EnableButton(bool Enable)
	{
		Button[] ButtonList = GameObject.FindObjectsOfType<Button>();
		foreach(Button pButton in ButtonList)
		{
			pButton.enabled = Enable;
		}
	}

	// Recursive enable buttons
	public void EnableButton(Transform trans, bool Enable)
	{		
		Button pButton = trans.gameObject.GetComponent<Button>();
		if(pButton!=null)
		{
			pButton.enabled = Enable;
		}

		foreach(Transform child in trans)
		{
			EnableButton(child, Enable);
		}
	}
	
	#endregion
	
	// ######################################################################
	// Utilities Functions
	// ######################################################################
	
	#region Utilities

	// Find parent Camera of any GameObject
	public Camera GetParent_Camera(Transform trans)
	{
		Transform ParentObj = trans.parent;
		while(ParentObj!=null)
		{
			Camera pCamera = ParentObj.gameObject.GetComponent<Camera>();
			if(pCamera!=null)
			{
				return pCamera;
			}
			ParentObj = ParentObj.parent;
		}
		
		return null;
	}
	
	#endregion
	
	// ######################################################################
	// UNITY UI utilities Functions
	// ######################################################################
	
	#region UNITY_UI Utilities

	// Find parent Canvas of any GameObject
	public Canvas GetParent_Canvas(Transform trans)
	{
		Transform ParentObj = trans.parent;
		while(ParentObj!=null)
		{
			Canvas pCanvas = ParentObj.gameObject.GetComponent<Canvas>();
			if(pCanvas!=null)
			{
				return pCanvas;
			}
			ParentObj = ParentObj.parent;
		}
		
		return null;
	}
	
	#endregion

}

#endregion
