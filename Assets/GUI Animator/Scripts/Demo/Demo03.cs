// GUI Animator for NGUI version: 0.8.3
// Author: Gold Experience Team (http://www.ge-team.com/pages/)
// Support: geteamdev@gmail.com
// Please direct any bugs/comments/suggestions to geteamdev@gmail.com

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion

/**************
* Demo03 class
* This class handles Demo03 scene.
* It does animate all GameObjects when scene starts and ends.
* It also responds to user mouse click or tap on buttons.
**************/

public class Demo03 : MonoBehaviour
{
	
	#region Variables
	
	// GEAnim objects of title text
	public GEAnim m_Title1;
	public GEAnim m_Title2;
	
	// GEAnim objects of top and bottom bars
	public GEAnim m_TopBar;
	public GEAnim m_BottomBar;
	
	// GEAnim object of dialogs
	public GEAnim m_Dialog1;
	public GEAnim m_Dialog2;
	public GEAnim m_Dialog3;
	public GEAnim m_Dialog4;
	
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

	// Move In m_Title1 and m_Title2
	IEnumerator MoveInTitleGameObjects()
	{
		yield return new WaitForSeconds(1.0f);
		
		// Move In m_Title1 and m_Title2
		m_Title1.MoveIn(eGUIMove.Self);
		m_Title2.MoveIn(eGUIMove.Self);
		
		// MoveIn dialogs
		StartCoroutine(ShowDialogs());
	}

	// MoveIn dialogs
	IEnumerator ShowDialogs()
	{
		yield return new WaitForSeconds(1.0f);
		
		// MoveIn dialogs
		m_Dialog1.MoveIn(eGUIMove.SelfAndChildren);		
		m_Dialog2.MoveIn(eGUIMove.SelfAndChildren);		
		m_Dialog3.MoveIn(eGUIMove.SelfAndChildren);		
		m_Dialog4.MoveIn(eGUIMove.SelfAndChildren);
		
		// Enable all scene switch buttons
		StartCoroutine(EnableAllDemoButtons());
	}
	
	// MoveOut dialogs
	public void HideAllGUIs()
	{
		// MoveOut dialogs
		m_Dialog1.MoveOut(eGUIMove.SelfAndChildren);
		m_Dialog2.MoveOut(eGUIMove.SelfAndChildren);
		m_Dialog3.MoveOut(eGUIMove.SelfAndChildren);
		m_Dialog4.MoveOut(eGUIMove.SelfAndChildren);
		
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
	IEnumerator DisableButtonForSeconds(GameObject GO, float DisableTime)
	{
		// Disable all buttons
		GEAnimSystem.Instance.EnableButton(GO.transform, false);
		
		yield return new WaitForSeconds(DisableTime);
		
		// Enable all buttons
		GEAnimSystem.Instance.EnableButton(GO.transform, true);
	}
	
	#endregion
	
	// ######################################################################
	// Button handler functions
	// ######################################################################
	
	#region Button Handler

	public void OnButton_Dialog1()
	{		
		// Disable m_Dialog1 for a few seconds
		StartCoroutine(DisableButtonForSeconds(m_Dialog1.gameObject, 1.0f));

		// MoveOut m_Dialog1
		m_Dialog1.MoveOut(eGUIMove.SelfAndChildren);
		
		// Moves m_Dialog1 back to screen
		StartCoroutine(Dialog1_MoveIn());
	}
	
	public void OnButton_Dialog2()
	{
		// Disable m_Dialog2 for a few seconds
		StartCoroutine(DisableButtonForSeconds(m_Dialog2.gameObject, 1.0f));

		// MoveOut m_Dialog2
		m_Dialog2.MoveOut(eGUIMove.SelfAndChildren);
		
		// Moves m_Dialog2 back to screen
		StartCoroutine(Dialog2_MoveIn());
	}
	
	public void OnButton_Dialog3()
	{
		// Disable m_Dialog3 for a few seconds
		StartCoroutine(DisableButtonForSeconds(m_Dialog3.gameObject, 1.0f));

		// MoveOut m_Dialog3
		m_Dialog3.MoveOut(eGUIMove.SelfAndChildren);
		
		// Moves m_Dialog3 back to screen
		StartCoroutine(Dialog3_MoveIn());
	}
	
	public void OnButton_Dialog4()
	{
		// Disable m_Dialog4 for a few seconds
		StartCoroutine(DisableButtonForSeconds(m_Dialog4.gameObject, 1.0f));

		// MoveOut m_Dialog4
		m_Dialog4.MoveOut(eGUIMove.SelfAndChildren);
		
		// Moves m_Dialog4 back to screen
		StartCoroutine(Dialog4_MoveIn());
	}
	
	// MoveOut dialogs
	public void OnButton_MoveOutAllDialogs()
	{
		// MoveOut dialogs
		if(m_Dialog1.m_MoveOut.Began==false && m_Dialog1.m_MoveOut.Done==false)
		{
			m_Dialog1.MoveOut(eGUIMove.SelfAndChildren);
			// Move m_Dialog1 back to screen with Coroutines
			StartCoroutine(Dialog1_MoveIn());
		}
		if(m_Dialog2.m_MoveOut.Began==false && m_Dialog2.m_MoveOut.Done==false)
		{
			m_Dialog2.MoveOut(eGUIMove.SelfAndChildren);
			// Move m_Dialog2 back to screen with Coroutines
			StartCoroutine(Dialog2_MoveIn());
		}
		if(m_Dialog3.m_MoveOut.Began==false && m_Dialog3.m_MoveOut.Done==false)
		{
			m_Dialog3.MoveOut(eGUIMove.SelfAndChildren);
			// Move m_Dialog3 back to screen with Coroutines
			StartCoroutine(Dialog3_MoveIn());
		}
		if(m_Dialog4.m_MoveOut.Began==false && m_Dialog4.m_MoveOut.Done==false)
		{
			m_Dialog4.MoveOut(eGUIMove.SelfAndChildren);
			// Move m_Dialog4 back to screen with Coroutines
			StartCoroutine(Dialog4_MoveIn());
		}
	}
	
	#endregion
	
	// ######################################################################
	// Move dialog coroutine functions
	// ######################################################################
	
	#region Move Dialog
	
	// MoveIn m_Dialog1
	IEnumerator Dialog1_MoveIn()
	{
		yield return new WaitForSeconds(1.5f);
		
		// Reset m_Dialog1
		m_Dialog1.Reset();

		// Moves m_Dialog1 back to screen
		m_Dialog1.MoveIn(eGUIMove.SelfAndChildren);
	}
	
	// MoveIn m_Dialog2
	IEnumerator Dialog2_MoveIn()
	{
		yield return new WaitForSeconds(1.5f);
		
		// Reset m_Dialog2
		m_Dialog2.Reset();

		// Moves m_Dialog2 back to screen
		m_Dialog2.MoveIn(eGUIMove.SelfAndChildren);
	}
	
	// MoveIn m_Dialog3
	IEnumerator Dialog3_MoveIn()
	{
		yield return new WaitForSeconds(1.5f);
		
		// Reset m_Dialog3
		m_Dialog3.Reset();

		// Moves m_Dialog3 back to screen
		m_Dialog3.MoveIn(eGUIMove.SelfAndChildren);
	}
	
	// MoveIn m_Dialog4
	IEnumerator Dialog4_MoveIn()
	{
		yield return new WaitForSeconds(1.5f);
		
		// Reset m_Dialog4
		m_Dialog4.Reset();

		// Moves m_Dialog4 back to screen
		m_Dialog4.MoveIn(eGUIMove.SelfAndChildren);
	}
	
	#endregion
}