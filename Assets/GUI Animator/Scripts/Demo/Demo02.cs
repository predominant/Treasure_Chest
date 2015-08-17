// GUI Animator for NGUI version: 0.8.3
// Author: Gold Experience Team (http://www.ge-team.com/pages/)
// Support: geteamdev@gmail.com
// Please direct any bugs/comments/suggestions to geteamdev@gmail.com

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion

/**************
* Demo02 class
* This class handles Demo02 scene.
* It does animate all GameObjects when scene starts and ends.
* It also responds to user mouse click or tap on buttons.
**************/

public class Demo02 : MonoBehaviour
{
	
	#region Variables
	
	// GEAnim objects of Title text
	public GEAnim m_Title1;
	public GEAnim m_Title2;
	
	// GEAnim objects of Top and bottom
	public GEAnim m_TopBar;
	public GEAnim m_BottomBar;
	
	// GEAnim object of Dialog
	public GEAnim m_Dialog;
	
	#endregion
	
	// ######################################################################
	// MonoBehaviour Functions
	// ######################################################################
	
	#region MonoBehaviour
	
	// Use this for initialization
	void Awake ()
	{
		#if UNITY_EDITOR
		// Prevent GEAnimSystem from running MoveIn() or MoveOut in the same time with this script.
		// If you want to try test mode by GEAnimSystem, follow these steps.
		//	1. In Hierachy tab, select -SceneController- game object.
		//	2. Uncceck -SceneController- in Inspector tab to disable it.
		//	3. Select GUIAnimSystem in Hierachy tab.
		//	4. Enable Test Move In and Text Move Out of GUIAnimSystem.
		if(GEAnimSystem.Instance.m_TestMoveIn==true)
			GEAnimSystem.Instance.m_TestMoveIn = false;
		if(GEAnimSystem.Instance.m_TestMoveOut==true)
			GEAnimSystem.Instance.m_TestMoveOut = false;
		#endif
	}

	// Use this for initialization
	void Start ()
	{
		// MoveIn m_TopBar and m_BottomBar
		m_TopBar.MoveIn(eGUIMove.SelfAndChildren);
		m_BottomBar.MoveIn(eGUIMove.SelfAndChildren);
		
		// MoveIn m_Title1 and m_Title2
		StartCoroutine(MoveInTitleGameObjects());
		
		// Disable all scene switch buttons
		EnableAllDemoButtons(false);
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
	
	#endregion
	
	// ######################################################################
	// MoveIn/MoveOut functions
	// ######################################################################
	
	#region MoveIn/MoveOut
	
	// MoveIn m_Title1 and m_Title2
	IEnumerator MoveInTitleGameObjects()
	{
		yield return new WaitForSeconds(1.0f);
		
		// MoveIn m_Title1 and m_Title2
		m_Title1.MoveIn(eGUIMove.Self);
		m_Title2.MoveIn(eGUIMove.Self);
		
		// MoveIn m_Dialog
		StartCoroutine(ShowDialog());
	}
	
	// MoveIn m_Dialog
	IEnumerator ShowDialog()
	{
		yield return new WaitForSeconds(1.0f);
		
		// MoveIn m_Dialog
		m_Dialog.MoveIn(eGUIMove.SelfAndChildren);
		
		// Enable all scene switch buttons
		StartCoroutine(EnableAllDemoButtons());
	}
	
	// MoveOut m_Dialog
	public void HideAllGUIs()
	{
		// MoveOut m_Dialog
		m_Dialog.MoveOut(eGUIMove.SelfAndChildren);
		
		// MoveOut m_Title1 and m_Title2
		StartCoroutine(HideTitleTextMeshes());
	}
	
	// MoveOut m_Title1 and m_Title2
	IEnumerator HideTitleTextMeshes()
	{
		yield return new WaitForSeconds(1.0f);
		
		// MoveOut m_Title1 and m_Title2
		m_Title1.MoveOut(eGUIMove.Self);
		m_Title2.MoveOut(eGUIMove.Self);
		
		// MoveOut m_TopBar and m_BottomBar
		m_TopBar.MoveOut(eGUIMove.SelfAndChildren);
		m_BottomBar.MoveOut(eGUIMove.SelfAndChildren);
	}
	
	#endregion
	
	// ######################################################################
	// Enable/Disable button functions
	// ######################################################################
	
	#region Enable/Disable buttons
	
	// Enable/Disable all scene switch Coroutine
	IEnumerator EnableAllDemoButtons()
	{
		yield return new WaitForSeconds(1.0f);
		
		// Enable all scene switch buttons
		EnableAllDemoButtons(true);
	}
	
	// Enable/Disable all scene switch buttons
	void EnableAllDemoButtons(bool IsEnable)
	{
		for(int i=1;i<=8;i++)
		{
			// Find all demo scene switch buttons
			GameObject pGameObject = GameObject.Find("ButtonDemoscene"+i.ToString()+" (TextMesh)");
			if(pGameObject!=null)
			{
				GEAnimSystem.Instance.EnableButton(pGameObject.transform, IsEnable);
			}
		}
	}
	
	// Disable all buttons for a few seconds
	IEnumerator DisableAllButtonsForSeconds(float DisableTime)
	{
		// Disable all buttons
		GEAnimSystem.Instance.EnableButton(false);
		
		yield return new WaitForSeconds(DisableTime);
		
		// Enable all buttons
		GEAnimSystem.Instance.EnableButton(true);
	}
	
	#endregion
	
	// ######################################################################
	// Button handler functions
	// ######################################################################
	
	#region Button Handler

	public void OnButton_UpperEdge()
	{
		// MoveOut m_Dialog
		m_Dialog.MoveOut(eGUIMove.SelfAndChildren);

		// MoveIn m_Dialog from top
		StartCoroutine(DialogMoveIn(ePosMoveIn.From_UpperScreenEdge));
	}
	
	public void OnButton_LeftEdge()
	{
		// MoveOut m_Dialog
		m_Dialog.MoveOut(eGUIMove.SelfAndChildren);
		
		// MoveIn m_Dialog from left
		StartCoroutine(DialogMoveIn(ePosMoveIn.From_LeftScreenEdge));
	}
	
	public void OnButton_RightEdge()
	{
		// MoveOut m_Dialog
		m_Dialog.MoveOut(eGUIMove.SelfAndChildren);
		
		// Disable all buttons for a few seconds
		StartCoroutine(DisableAllButtonsForSeconds(2.0f));
		
		// MoveIn m_Dialog from right
		StartCoroutine(DialogMoveIn(ePosMoveIn.From_RightScreenEdge));
	}
	
	public void OnButton_BottomEdge()
	{
		// MoveOut m_Dialog
		m_Dialog.MoveOut(eGUIMove.SelfAndChildren);
		
		// Disable all buttons for a few seconds
		StartCoroutine(DisableAllButtonsForSeconds(2.0f));
		
		// MoveIn m_Dialog from bottom
		StartCoroutine(DialogMoveIn(ePosMoveIn.From_BottomScreenEdge));
	}
	
	public void OnButton_UpperLeft()
	{
		// MoveOut m_Dialog
		m_Dialog.MoveOut(eGUIMove.SelfAndChildren);
		
		// Disable all buttons for a few seconds
		StartCoroutine(DisableAllButtonsForSeconds(2.0f));
		
		// MoveIn m_Dialog from upper left
		StartCoroutine(DialogMoveIn(ePosMoveIn.From_UpperLeft));
	}
	
	public void OnButton_UpperRight()
	{
		// MoveOut m_Dialog
		m_Dialog.MoveOut(eGUIMove.SelfAndChildren);
		
		// Disable all buttons for a few seconds
		StartCoroutine(DisableAllButtonsForSeconds(2.0f));
		
		// MoveIn m_Dialog from upper right
		StartCoroutine(DialogMoveIn(ePosMoveIn.From_UpperRight));
	}
	
	public void OnButton_BottomLeft()
	{
		// MoveOut m_Dialog
		m_Dialog.MoveOut(eGUIMove.SelfAndChildren);
		
		// Disable all buttons for a few seconds
		StartCoroutine(DisableAllButtonsForSeconds(2.0f));
		
		// MoveIn m_Dialog from bottom left
		StartCoroutine(DialogMoveIn(ePosMoveIn.From_BottomLeft));
	}
	
	public void OnButton_BottomRight()
	{
		// MoveOut m_Dialog
		m_Dialog.MoveOut(eGUIMove.SelfAndChildren);
		
		// Disable all buttons for a few seconds
		StartCoroutine(DisableAllButtonsForSeconds(2.0f));
		
		// MoveIn m_Dialog from bottom right
		StartCoroutine(DialogMoveIn(ePosMoveIn.From_BottomRight));
	}
	
	public void OnButton_Center()
	{
		// MoveOut m_Dialog
		m_Dialog.MoveOut(eGUIMove.SelfAndChildren);
		
		// Disable all buttons for a few seconds
		StartCoroutine(DisableAllButtonsForSeconds(2.0f));
		
		// MoveIn m_Dialog from center of screen
		StartCoroutine(DialogMoveIn(ePosMoveIn.From_Center));
	}
	
	#endregion
	
	// ######################################################################
	// Move dialog functions
	// ######################################################################
	
	#region Move Dialog

	// MoveIn m_Dialog by position
	IEnumerator DialogMoveIn(ePosMoveIn PosMoveIn)
	{
		yield return new WaitForSeconds(1.5f);
		
		//Debug.Log("PosMoveIn="+PosMoveIn);
		switch(PosMoveIn)
		{
			// Set m_Dialog to move in from upper
			case ePosMoveIn.From_UpperScreenEdge:
				m_Dialog.m_MoveIn.MoveFrom = ePosMoveIn.From_UpperScreenEdge;
				m_Dialog.m_MoveOut.MoveTo = ePosMoveOut.To_Center;
				break;
			// Set m_Dialog to move in from left
			case ePosMoveIn.From_LeftScreenEdge:
				m_Dialog.m_MoveIn.MoveFrom = ePosMoveIn.From_LeftScreenEdge;
				m_Dialog.m_MoveOut.MoveTo = ePosMoveOut.To_Center;
				break;
			// Set m_Dialog to move in from right
			case ePosMoveIn.From_RightScreenEdge:
				m_Dialog.m_MoveIn.MoveFrom = ePosMoveIn.From_RightScreenEdge;
				m_Dialog.m_MoveOut.MoveTo = ePosMoveOut.To_Center;
				break;
			// Set m_Dialog to move in from bottom
			case ePosMoveIn.From_BottomScreenEdge:
				m_Dialog.m_MoveIn.MoveFrom = ePosMoveIn.From_BottomScreenEdge;
				m_Dialog.m_MoveOut.MoveTo = ePosMoveOut.To_Center;
				break;
			// Set m_Dialog to move in from upper left
			case ePosMoveIn.From_UpperLeft:	
				m_Dialog.m_MoveIn.MoveFrom = ePosMoveIn.From_UpperLeft;
				m_Dialog.m_MoveOut.MoveTo = ePosMoveOut.To_Center;
				break;
			// Set m_Dialog to move in from upper right
			case ePosMoveIn.From_UpperRight:
				m_Dialog.m_MoveIn.MoveFrom = ePosMoveIn.From_UpperRight;
				m_Dialog.m_MoveOut.MoveTo = ePosMoveOut.To_Center;
				break;
			// Set m_Dialog to move in from bottom left
			case ePosMoveIn.From_BottomLeft:
				m_Dialog.m_MoveIn.MoveFrom = ePosMoveIn.From_BottomLeft;
				m_Dialog.m_MoveOut.MoveTo = ePosMoveOut.To_Center;
				break;
			// Set m_Dialog to move in from bottom right
			case ePosMoveIn.From_BottomRight:
				m_Dialog.m_MoveIn.MoveFrom = ePosMoveIn.From_BottomRight;
				m_Dialog.m_MoveOut.MoveTo = ePosMoveOut.To_Center;
				break;
			// Set m_Dialog to move in from center
			case ePosMoveIn.From_Center:
			default:
				m_Dialog.m_MoveIn.MoveFrom = ePosMoveIn.From_Center;
				m_Dialog.m_MoveOut.MoveTo = ePosMoveOut.To_Center;
				break;
		}

		// Reset m_Dialog
		m_Dialog.Reset();

		// MoveIn m_Dialog by position
		m_Dialog.MoveIn(eGUIMove.SelfAndChildren);
	}
	
	#endregion
}