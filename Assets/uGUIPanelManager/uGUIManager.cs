using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace uGUIPanelManager
{

	public class uGUIManager : MonoBehaviour
	{

		//SETTINGS
		private uGUISettings settings;
		public uGUISettings Settings
		{
			get
			{
				if (settings == null)
				{
					settings = Resources.Load<uGUISettings>("uGUISettings");
				}
				return settings;
			}
		}
		//

		public uGUIManagedPanel[] managedPanels = new uGUIManagedPanel[0];

		public List<uGUIMovingPanel> movingPanels = new List<uGUIMovingPanel>();

		private static uGUIManager _instance;
		public static uGUIManager instance
		{
			get
			{
				#if !UNITY_EDITOR		
				if (_instance == null)
				{
				#endif
				_instance = (uGUIManager)FindObjectOfType(typeof(uGUIManager));
				if (_instance == null)
				{
					_instance = new GameObject("PanelManager").AddComponent<uGUIManager>();
				}
				#if !UNITY_EDITOR
				}
				#endif
				return _instance;
			}
		}

		/// <summary>
		/// Triggers the instance to be searched again on OnDisable()
		/// </summary>
		public void OnDisable()
		{
			_instance = null;
		}
		
		/// <summary>
		/// Triggers the instance to be searched again on OnLevelWasLoaded() 
		/// </summary>
		void OnLevelWasLoaded(int level)
		{
			_instance = null;
		}

		// Use this for initialization
		void Awake()
		{
			ShowMainPanel();
		}
		
		// Update is called once per frame
		void Update()
		{
			MovePanels();
		}

		void MovePanels()
		{
			bool atleastonepanelismoving = false;
			int i, length = movingPanels.Count;
			uGUIMovingPanel mp;

			for (i = 0; i < length; i++)
			{
				mp = movingPanels [i];
				if (mp.queued && atleastonepanelismoving)
				{
					continue;
				}

				if (mp.panel.CachedAnimator != null && !mp.instant)
				{

					//has animator
					if (!mp.isAnimating)
					{
						if (mp.panel.isMoving)
						{
							atleastonepanelismoving = true;
							continue;
						}

						if (mp.panel.panelState == mp.targetState)
						{
							movingPanels.Remove(mp);
							return;
						}
						else
						{

							if (!mp.additional && mp.targetState != PanelState.Hide)
							{
								//hide other panels
								HideAllPanels(mp.panel.name, i);
								mp.additional = true; //kindof dirty but works for now
								return;
							}

							mp.panel.CachedGameObject.SetActive(true);
							mp.panel.stateTransform [(int)mp.panel.panelState].callBeforeLeaveState.Invoke();
							mp.panel.stateTransform [(int)mp.targetState].callBeforeEnterState.Invoke();
							if (mp.toggle)
							{
								mp.panel.SetState(GetToggleState(mp));
							}
							else
							{
								mp.panel.SetState(mp.targetState);
							}
							mp.isAnimating = true;
							mp.panel.isMoving = true;
							atleastonepanelismoving = true;
						}

					}
					else
					{
						if (mp.panel.CachedAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !mp.panel.CachedAnimator.IsInTransition(0))
						{
							mp.panel.stateTransform [(int)mp.panel.panelState].callAfterLeaveState.Invoke();
							mp.panel.panelState = mp.targetState;
							mp.panel.stateTransform [(int)mp.panel.panelState].callAfterEnterState.Invoke();

							if (mp.targetState == PanelState.Hide)
							{
								mp.panel.CachedGameObject.SetActive(false);
							}

							mp.panel.isMoving = false;
							mp.panel.time = 0;
							movingPanels.Remove(mp);

							return;
						}
						else
						{
							atleastonepanelismoving = true;
						}
					}
				}
				else
				{

					//has NO animator or flag instant is set
					if (mp.instant || mp.panel.HasFinishedAnimation())
					{

						mp.panel.CachedGameObject.SetActive(true);

						if (mp.instant)
						{
							mp.panel.stateTransform [(int)mp.panel.panelState].callBeforeLeaveState.Invoke();
						}
						mp.panel.stateTransform [(int)mp.panel.panelState].callAfterLeaveState.Invoke();

						if (GetToggleState(mp) == PanelState.Hide)
						{

							mp.panel.CachedGameObject.SetActive(false);
						}
						mp.panel.CachedRectTransform.anchoredPosition = mp.panel.stateTransform [(int)GetToggleState(mp)].pos;
						mp.panel.CachedRectTransform.localRotation = Quaternion.Euler(mp.panel.stateTransform [(int)GetToggleState(mp)].rot);
						mp.panel.CachedRectTransform.localScale = mp.panel.stateTransform [(int)GetToggleState(mp)].scale;



						mp.panel.panelState = GetToggleState(mp);
						if (mp.instant)
						{
							mp.panel.stateTransform [(int)mp.panel.panelState].callBeforeEnterState.Invoke();
						}
						mp.panel.stateTransform [(int)mp.panel.panelState].callAfterEnterState.Invoke();

						mp.panel.isMoving = false;
						mp.panel.time = 0;
						movingPanels.Remove(mp);
						return;
					}
					else
					{

						if (!mp.additional && mp.targetState != PanelState.Hide)
						{
							//hide other panels
							HideAllPanels(mp.panel.name, i);
							mp.additional = true; //kindof dirty but works for now
							return;
						}

							
						if (!mp.isAnimating)
						{
							mp.panel.CachedGameObject.SetActive(true);
							mp.isAnimating = true;

							if (GetToggleState(mp) == mp.targetState)
							{
								mp.panel.stateTransform [(int)mp.panel.panelState].callBeforeLeaveState.Invoke();
//								CallMethod(mp.panel.stateTransform [(int)mp.panel.panelState].target, mp.panel.stateTransform [(int)mp.panel.panelState].callBeforeLeave);
								mp.panel.stateTransform [(int)mp.targetState].callBeforeEnterState.Invoke();
//								CallMethod(mp.panel.stateTransform [(int)mp.targetState].target, mp.panel.stateTransform [(int)mp.targetState].callBeforeEnter);
							}
							else
							{
								mp.panel.stateTransform [(int)mp.toggleState].callBeforeEnterState.Invoke();
//								CallMethod(mp.panel.stateTransform [(int)mp.toggleState].target, mp.panel.stateTransform [(int)mp.toggleState].callBeforeEnter);
								mp.panel.stateTransform [(int)mp.targetState].callAfterLeaveState.Invoke();
//								CallMethod(mp.panel.stateTransform [(int)mp.targetState].target, mp.panel.stateTransform [(int)mp.targetState].callBeforeLeave);
							}
						}

						atleastonepanelismoving = true;
						float evaluate = mp.panel.curve.Evaluate(mp.panel.time / mp.panel.duration);

						mp.panel.CachedRectTransform.anchoredPosition = Vector2.Lerp(mp.panel.stateTransform [(int)mp.panel.panelState].pos,
						                                                             mp.panel.stateTransform [(int)GetToggleState(mp)].pos,
						                                                             evaluate);
						mp.panel.CachedRectTransform.localRotation = Quaternion.Lerp(Quaternion.Euler(mp.panel.stateTransform [(int)mp.panel.panelState].rot),
						                                                             Quaternion.Euler(mp.panel.stateTransform [(int)GetToggleState(mp)].rot),
						                                                             evaluate);
						mp.panel.CachedRectTransform.localScale = Vector3.Lerp(mp.panel.stateTransform [(int)mp.panel.panelState].scale,
						                                                       mp.panel.stateTransform [(int)GetToggleState(mp)].scale,
						                                                       evaluate);
						if (this.Settings.useFixedDeltaTime)
						{
							mp.panel.time += Time.fixedDeltaTime;
						}
						else
						{
							mp.panel.time += Time.deltaTime;
						}
					}
				}
			}
		}

		public void SearchPaneles()
		{
			//Searching first for canvases and then for Managed Panels is just a workaround to find also deactivated Panels in the scene hierarchy
			List<uGUIManagedPanel> managedPanelsInScene = new List<uGUIManagedPanel>();
			Canvas[] canvases = FindObjectsOfType<Canvas>();

			foreach (Canvas C in canvases)
			{
				managedPanelsInScene.AddRange(C.GetComponentsInChildren<uGUIManagedPanel>(true));
			}


			managedPanels = managedPanelsInScene.ToArray();

		}

		public void HideAllPanels(string exception = "", int insertpos = -1)
		{
			foreach (uGUIManagedPanel mp in managedPanels)
			{
				if (mp.name != exception)
				{
					HidePanelInst(mp.name, insertpos: insertpos);
				}
			}

		}

		/// <summary>
		/// Show Main Panel
		/// </summary>
		public void ShowMainPanel()
		{

			foreach (uGUIManagedPanel mp in managedPanels)
			{
				if (!mp)
					;

				else if (mp.isMainPanel)
					SetPanelStateInst(mp.name, PanelState.Show, false, false, this.Settings.showMainPanelInstantOnSceneLoad);

				else
					HidePanelInst(mp.name, false, true);
			}
		}

//Sample Calls
//		uGUIManager.SetPanelState("MainMenu",PanelState.Show,additional: false,queued: false, instant: false);
//		uGUIManager.SetPanelState("MainMenu",PanelState.Hide);
//		uGUIManager.TogglePanelState("SideBar",targetState: PanelState.ToggleIn, 
//		                             toggleState: PanelState.ToggleOut,additional: true,queued: true);
//		uGUIManager.ShowPanel("Options",additional: true, queued: false, instant:false);
//		uGUIManager.HidePanel("Credits",queued: false, instant:false);
//
	
		public static void SetPanelState(string name, PanelState targetState, bool additional = false, bool queued = false, bool instant = false)
		{
			uGUIManager.instance.SetPanelStateInst(name, targetState, additional, queued, instant);
		}

		public void SetPanelStateInst(string name, PanelState targetState, bool additional = false, bool queued = false, bool instant = false)
		{
			uGUIManagedPanel mp = FindPanelInst(name);
			if (mp.panelState != targetState)
			{
				movingPanels.Add(new uGUIMovingPanel(mp, targetState, queued, instant, additional));
			}
		}

		public static void TogglePanelState(string name, PanelState targetState, PanelState toggleState, bool additional = false, bool queued = false, bool instant = false)
		{
			uGUIManager.instance.TogglePanelStateInst(name, targetState, toggleState, additional, queued, instant);
		}

		public void TogglePanelStateInst(string name, PanelState targetState, PanelState toggleState, bool additional = false, bool queued = false, bool instant = false)
		{
			uGUIManagedPanel mp = FindPanelInst(name);

			bool add = false;
			
			uGUIMovingPanel currMovingPanel = DeleteAllMovingPanelsExceptCurrent(mp);
			if (currMovingPanel != null)
			{
				if (currMovingPanel.targetState != GetToggleState(currMovingPanel))
				{
					add = true;
					queued = true;
				}
			}
			else
			{
				add = true;
			}
			
			if (add)
			{
				movingPanels.Add(new uGUIMovingPanel(mp, targetState, toggleState, queued, instant, additional));
			}
		}

		public static void ShowPanel(string name, bool additional = false, bool queued = false, bool instant = false)
		{
			uGUIManager.instance.ShowPanelInst(name, additional, queued, instant);
		}

		public void ShowPanelInst(string name, bool additional = false, bool queued = false, bool instant = false)
		{
			uGUIManagedPanel mp = FindPanelInst(name);

			bool add = false;
			
			uGUIMovingPanel currMovingPanel = DeleteAllMovingPanelsExceptCurrent(mp);
			if (currMovingPanel != null)
			{
				if (currMovingPanel.targetState != PanelState.Show)
				{
					add = true;
					queued = true;
				}
			}
			else
			{
				if (mp.panelState != PanelState.Show)
				{
					add = true;
				}
			}

			if (add)
			{
				movingPanels.Add(new uGUIMovingPanel(mp, true, false, queued, instant, additional));
			}
		}

		public static void HidePanel(string name, bool queued = false, bool instant = false)
		{
			uGUIManager.instance.HidePanelInst(name, queued, instant);
		}

		public void HidePanelInst(string name, bool queued = false, bool instant = false, int insertpos = -1)
		{
			uGUIManagedPanel mp = FindPanelInst(name);
			bool add = false;

			uGUIMovingPanel currMovingPanel = DeleteAllMovingPanelsExceptCurrent(mp);
			if (currMovingPanel != null)
			{
				if (currMovingPanel.targetState != PanelState.Hide)
				{
					add = true;
					queued = true;
				}
			}
			else
			{
				if (mp.panelState != PanelState.Hide)
				{
					add = true;
				}
			}



			if (add)
			{
				if (insertpos == -1)
				{
					movingPanels.Add(new uGUIMovingPanel(mp, false, true, queued, instant, false));
				}
				else
				{
					movingPanels.Insert(insertpos, new uGUIMovingPanel(mp, false, true, queued, instant, false));
				}
			}

		}

		public static uGUIManagedPanel FindPanel(string name)
		{
			return uGUIManager.instance.FindPanelInst(name);
		}

		public uGUIManagedPanel FindPanelInst(string name)
		{
			foreach (uGUIManagedPanel mp in managedPanels)
			{
				if (mp && mp.name == name)
					return mp;
			}
			Debug.LogError("Panel with name: " + name + " does not exist!");
			return null;
		}

		private void CallMethod(GameObject target, string methodName)
		{
			if (target != null && methodName != "")
			{
				target.SendMessage(methodName, null, SendMessageOptions.RequireReceiver);
			}
		}

		private PanelState GetToggleState(uGUIMovingPanel movingPanel)
		{
			if (movingPanel.panel.panelState != movingPanel.targetState && movingPanel.panel.panelState != movingPanel.toggleState)
			{
				return movingPanel.targetState;
			}
			else
			if (movingPanel.panel.panelState != movingPanel.targetState && movingPanel.panel.panelState == movingPanel.toggleState)
			{
				return movingPanel.targetState;
			}
			else
			if (movingPanel.panel.panelState == movingPanel.targetState && movingPanel.panel.panelState != movingPanel.toggleState)
			{
				return movingPanel.toggleState;
			}
			else
			{
				return movingPanel.targetState;
			}
		}

		private uGUIMovingPanel DeleteAllMovingPanelsExceptCurrent(uGUIManagedPanel panel)
		{
			int i, length = movingPanels.Count;
			uGUIMovingPanel current = null;

			for (i = length -1; i >= 0; i--)
			{
				if (movingPanels [i].panel == panel)
				{
					if (current != null)
					{
						movingPanels.Remove(current);
					}
					current = movingPanels [i];
				}

			}

			return current;
		}
	}
}
