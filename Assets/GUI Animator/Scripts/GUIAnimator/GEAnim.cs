// GUI Animator for NGUI version: 0.8.3
// Author: Gold Experience Team (http://www.ge-team.com/pages/)
// Support: geteamdev@gmail.com
// Please direct any bugs/comments/suggestions to geteamdev@gmail.com

#region Namespaces

using UnityEngine;
using System.Collections;

using UnityEngine.UI;

#if HOTWEEN
// HOTween: https://www.assetstore.unity3d.com/#/content/3311 Documentation:  http://www.holoville.com/hotween/documentation.html
// LeanTween: https://www.assetstore.unity3d.com/#/content/3595 Documentation: http://dentedpixel.com/LeanTweenDocumentation/classes/LeanTween.html
// iTween: https://www.assetstore.unity3d.com/#/content/84 Documentation: http://itween.pixelplacement.com/documentation.php
using Holoville.HOTween;
#endif

#endregion

/**************
* GEAnim class
* This class controls move, scale and fading animation using iTween/HOTween/LeanTween tweeners.
* It also add AudioSource to GameObject when m_SoundMoveIn or m_SoundMoveOut is not null.
**************/

public class GEAnim : MonoBehaviour
{
	
	#region Variables
	
	// Required parent objects

		Camera					m_Parent_Camera = null;

			Canvas				m_Parent_Canvas = null;
	
	// Camera world dimension
		[HideInInspector]
		public Vector3 m_CameraArea;
		[HideInInspector]
		public float m_CameraLeftEdge;
		[HideInInspector]
		public float m_CameraRightEdge;
		[HideInInspector]
		public float m_CameraTopEdge;
		[HideInInspector]
		public float m_CameraBottomEdge;
	
	// Total bounds of this GameObject

		[HideInInspector]
		public Bounds m_TotalBounds;
	
	// Audio
		public AudioClip m_SoundMoveIn				= null;
		public AudioClip m_SoundMoveOut				= null;
	
	// Move

		[HideInInspector]
		public Vector3 m_MoveOriginal;
		public cMoveIn m_MoveIn;
		public cMoveOut m_MoveOut;
	
	// Scale

		[HideInInspector]
		public Vector3 m_ScaleOriginal;
		[HideInInspector]
		public Vector3 m_ScaleLast;
		[HideInInspector]
		public bool m_ScaleIsOverriding	= false;
		public cScaleIn m_ScaleIn;
		public cScaleOut m_ScaleOut;
	
	// Fade

		[HideInInspector]
		public float m_FadeOriginal;
		public cFade m_FadeIn;
		public cFade m_FadeOut;
	
	// Scale loop

		public cPingPongScale m_ScaleLoop;
	
	// Fade loop

		public cPingPongFade m_FadeLoop;
	
	// Animation variable

		[HideInInspector]
		public float m_MoveVariable		= 0.0f;
		[HideInInspector]
		public float m_ScaleVariable	= 0.0f;
		[HideInInspector]
		public float m_FadeVariable		= 0.0f;

	// Initial flag

		bool m_InitialDone				= false;

	
	// Buttons component
			Image			m_Image							= null;
			Toggle			m_Toggle						= null;
			Text			m_Text							= null;
			RawImage		m_RawImage						= null;
			Slider			m_Slider						= null;
			CanvasRenderer	m_CanvasRenderer				= null;
			RectTransform	m_ParentCanvasRectTransform		= null;
	
	#endregion	
	
	// ######################################################################
	// MonoBehaviour Functions
	// ######################################################################
	
	#region MonoBehaviour

	// Awake is called when the script instance is being loaded.
	void Awake()
	{
		
		#if HOTWEEN
		#elif ITWEEN
		#else
		// HOTween: https://www.assetstore.unity3d.com/#/content/3311 Documentation:  http://www.holoville.com/hotween/documentation.html
		// LeanTween: https://www.assetstore.unity3d.com/#/content/3595 Documentation: http://dentedpixel.com/LeanTweenDocumentation/classes/LeanTween.html
		// iTween: https://www.assetstore.unity3d.com/#/content/84 Documentation: http://itween.pixelplacement.com/documentation.php
		
		// LEANTWEEN INITIALIZATION
		LeanTween.init(3200); // This line is optional. Here you can specify the maximum number of tweens you will use (the default is 400).  This must be called before any use of LeanTween is made for it to be effective.
		#endif

		// Keep Original position and scale
		m_MoveOriginal = transform.position;
		m_ScaleOriginal = transform.localScale;
		m_FadeOriginal = 1.0f;

		m_Image = this.gameObject.GetComponent<Image>();
		m_Toggle = this.gameObject.GetComponent<Toggle>();
		m_Text = this.gameObject.GetComponent<Text>();
		m_RawImage = this.gameObject.GetComponent<RawImage>();
		m_Slider = this.gameObject.GetComponent<Slider>();
		m_CanvasRenderer = this.gameObject.GetComponent<CanvasRenderer>();
		
		// Set begin alpha
		if(this.gameObject.GetComponent<Renderer>()!=null)
		{
			if(m_Image)
			{
				m_FadeOriginal = m_Image.color.a;
			}
			if(m_Toggle)
			{
				m_FadeOriginal = m_Toggle.GetComponentInChildren<Image>().color.a;
			}
			if(m_Text)
			{
				m_FadeOriginal = m_Text.color.a;
			}
			if(m_RawImage)
			{
				m_FadeOriginal = m_RawImage.color.a;
			}
			if(m_Slider)
			{
				m_FadeOriginal = m_Slider.colors.normalColor.a;
			}
		}
	}
	
	// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
	void Start () {
		
		#if HOTWEEN
		// HOTween: https://www.assetstore.unity3d.com/#/content/3311 Documentation:  http://www.holoville.com/hotween/documentation.html
		// LeanTween: https://www.assetstore.unity3d.com/#/content/3595 Documentation: http://dentedpixel.com/LeanTweenDocumentation/classes/LeanTween.html
		// iTween: https://www.assetstore.unity3d.com/#/content/84 Documentation: http://itween.pixelplacement.com/documentation.php
		
		// HOTWEEN INITIALIZATION
		// Must be done only once, before the creation of your first tween
		// (you can skip this if you want, and HOTween will be initialized automatically
		// when you create your first tween - using default values)
		HOTween.Init(true, true, true);
		#endif	
	}
	
	// Update is called every frame, if the MonoBehaviour is enabled.
	void Update () {

		// Apply animator to first update
		if(m_InitialDone==false)
		{
			m_InitialDone = true;
			bool ShouldCallReset = true;
			
			#if UNITY_EDITOR
			// Auto run MoveIn animation when GEAnimSystem.m_TestMoveIn is true
			if(GEAnimSystem.Instance.m_TestMoveIn==true)
			{
				MoveIn();
			}
			else if(GEAnimSystem.Instance.m_TestMoveOut==true)
			{
				ShouldCallReset = false;
				InitMoveOut();
			}
			#endif
			
			if(ShouldCallReset==true)
			{
				// Init m_MoveIn, m_MoveOut, m_ScaleIn, m_ScaleOut, m_FadeIn, m_FadeOut after finished Start function
				Reset();
			}
			
			#if UNITY_EDITOR
			// Auto run MoveOut animation when GEAnimSystem.m_TestMoveOut is true
			if(GEAnimSystem.Instance.m_TestMoveOut==true)
			{
				float DelayTime = 1.0f;
				
				// Waits for a limited time then test auto run MoveOut animation
				if(GEAnimSystem.Instance.m_TestMoveIn==true)
					DelayTime = GEAnimSystem.Instance.m_TestIdleTime;
				
				StartCoroutine(CoroutineMoveOut(DelayTime));
			}
			#endif
		}
	}
	
	#endregion
	
	// ######################################################################
	// Reset Functions
	// ######################################################################
	
	#region Reset
	
	// Init m_MoveIn, m_MoveOut, m_ScaleIn, m_ScaleOut, m_FadeIn, m_FadeOut
	public void Reset()
	{
		// Init m_MoveIn and m_MoveOut
		InitMoveIn();
		InitMoveOut();
		
		// Init m_ScaleIn and m_ScaleOut
		InitScaleIn();
		
		// Init m_FadeIn and m_FadeOut
		InitFadeIn();
	}
	
	// Init m_MoveIn, m_MoveOut, m_ScaleIn, m_ScaleOut, m_FadeIn, m_FadeOut for all children
	public void ResetAllChildren()
	{		

		// Init m_MoveIn and m_MoveOut
		InitMoveIn();
		InitMoveOut();
		
		// Init m_ScaleIn and m_ScaleOut
		InitScaleIn();
		
		// Init m_FadeIn and m_FadeOut
		InitFadeIn();
		
		// make a recursive call to this GameObject's children
		foreach(Transform child in this.transform)
		{
			GEAnim pGEAnim = child.gameObject.GetComponent<GEAnim>();
			if(pGEAnim!=null)
			{
				pGEAnim.ResetAllChildren();
			}
		}
	}
	
	#endregion
	
	// ######################################################################
	// Initial Functions
	// ######################################################################
	
	#region Initial
	
	// Find camera view size in 3d space dimention
	void CalculateCameraArea()
	{
		// Find parent Camera
		m_Parent_Camera = GEAnimSystem.Instance.GetParent_Camera(this.transform);
		// Calculate Camera area
		if(m_Parent_Camera!=null)
		{
			m_CameraArea = m_Parent_Camera.ScreenToWorldPoint( new Vector3(Screen.width, Screen.height, 0));
			m_CameraRightEdge = m_CameraArea.x - m_Parent_Camera.transform.position.x;
			m_CameraLeftEdge = m_Parent_Camera.transform.position.x - m_CameraRightEdge;
			m_CameraTopEdge = m_CameraArea.y - m_Parent_Camera.transform.position.y;
			m_CameraBottomEdge = m_Parent_Camera.transform.position.y - m_CameraTopEdge;
		}

		bool HasAnchor = false;

		if(HasAnchor==false)
		{
			HasAnchor = true;
			// Find parent Canvas
			m_Parent_Canvas = GEAnimSystem.Instance.GetParent_Canvas(this.transform);
			
			// Display warning messages if this object is not under hierarchy of Canvas
			if(m_Image!=null || m_Toggle!=null || m_Slider!=null || m_CanvasRenderer!=null)
			{
				if(m_Parent_Canvas==null)
				{
					Debug.LogWarning(this.name + " has to be a child of Unity UI Canvas.");
				}
			}
		}
		
		// Calculate Camera area
		if(m_Parent_Canvas!=null)
		{
			m_ParentCanvasRectTransform = m_Parent_Canvas.GetComponent<RectTransform>();

			if(m_Parent_Canvas.renderMode==RenderMode.ScreenSpaceOverlay)
			{
				m_CameraRightEdge = m_ParentCanvasRectTransform.rect.width;
				m_CameraLeftEdge = 0;
				m_CameraTopEdge = m_ParentCanvasRectTransform.rect.height;
				m_CameraBottomEdge = 0;
			}
			else
			{
				m_CameraRightEdge = (m_ParentCanvasRectTransform.rect.width * m_Parent_Canvas.transform.localScale.x)/2;
				m_CameraLeftEdge = -m_CameraRightEdge;
				m_CameraTopEdge = (m_ParentCanvasRectTransform.rect.height * m_Parent_Canvas.transform.localScale.y)/2;
				m_CameraBottomEdge = -m_CameraTopEdge;

				if(m_Parent_Canvas.worldCamera==null)
				{
					Debug.LogWarning(m_Parent_Canvas.name+" Canvas type "+m_Parent_Canvas.renderMode+" requires a camera. GEAnim may work not properly.");
				}
			}

		}

	}
	
	// Init Move-In
	void InitMoveIn()
	{

		if(m_MoveIn.Enable == true && m_MoveIn.Done == false)
		{
			// Find camera view size in 3d space dimention
			CalculateCameraArea();
			
			// Find total bounds
			CalculateTotalBounds();

			// update m_MoveIn.Position
			switch(m_MoveIn.MoveFrom)
			{
			case ePosMoveIn.From_ParentPosition:
				
				if(transform.parent!=null)
				{
					m_MoveIn.Position = new Vector3(transform.parent.transform.position.x,transform.parent.transform.position.y,m_MoveOriginal.z);;
				}
				else
				{
					m_MoveIn.Position = new Vector3(m_CameraRightEdge/2,m_CameraTopEdge/2,m_MoveOriginal.z);
				}
				break;
			case ePosMoveIn.From_SpecificPosition:
				// Set position to m_MoveIn
				if(m_Parent_Canvas.renderMode==RenderMode.ScreenSpaceOverlay)
				{
					m_MoveIn.Position = new Vector3(m_MoveIn.Position.x,m_MoveIn.Position.y,m_MoveOriginal.z);
				}
				else
				{
					m_MoveIn.Position = new Vector3(m_MoveIn.Position.x,m_MoveIn.Position.y,m_MoveOriginal.z);
				}
				break;
				
			case ePosMoveIn.From_UpperScreenEdge:
				m_MoveIn.Position = new Vector3(m_MoveOriginal.x,
				                                m_CameraTopEdge+m_TotalBounds.size.y,
				                                m_MoveOriginal.z);
				break;
			case ePosMoveIn.From_LeftScreenEdge:
				m_MoveIn.Position = new Vector3(m_CameraLeftEdge-m_TotalBounds.size.x,
				                                m_MoveOriginal.y,
				                                m_MoveOriginal.z);
				break;
			case ePosMoveIn.From_RightScreenEdge:
				m_MoveIn.Position = new Vector3(m_CameraRightEdge+m_TotalBounds.size.x,
				                                m_MoveOriginal.y,
				                                m_MoveOriginal.z);
				break;
			case ePosMoveIn.From_BottomScreenEdge:
				m_MoveIn.Position = new Vector3(m_MoveOriginal.x,
				                                m_CameraBottomEdge-m_TotalBounds.size.y,
				                                m_MoveOriginal.z);
				break;
				
			case ePosMoveIn.From_UpperLeft:
				m_MoveIn.Position = new Vector3(m_CameraLeftEdge-m_TotalBounds.size.x,
				                                m_CameraTopEdge+m_TotalBounds.size.y,
				                                m_MoveOriginal.z);	
				break;
			case ePosMoveIn.From_UpperCenter:
				if(m_Parent_Canvas.renderMode==RenderMode.ScreenSpaceOverlay)
				{
					m_MoveIn.Position = new Vector3(m_CameraRightEdge/2,
					                                m_CameraTopEdge+m_TotalBounds.size.y,
					                                m_MoveOriginal.z);	
				}
				else
				{
					m_MoveIn.Position = new Vector3(0,
					                                m_CameraTopEdge+m_TotalBounds.size.y,
					                                m_MoveOriginal.z);	
				}
				break;
			case ePosMoveIn.From_UpperRight:
				m_MoveIn.Position = new Vector3(m_CameraRightEdge+m_TotalBounds.size.x,
				                                m_CameraTopEdge+m_TotalBounds.size.y,
				                                m_MoveOriginal.z);	
				break;
			case ePosMoveIn.From_LeftCenter:
				if(m_Parent_Canvas.renderMode==RenderMode.ScreenSpaceOverlay)
				{
					m_MoveIn.Position = new Vector3(m_CameraLeftEdge-m_TotalBounds.size.x,
					                                m_CameraTopEdge/2,
					                                m_MoveOriginal.z);
				}
				else
				{
					m_MoveIn.Position = new Vector3(m_CameraLeftEdge-m_TotalBounds.size.x,
					                                0,
					                                m_MoveOriginal.z);
				}	
				break;
			case ePosMoveIn.From_Center:
				if(m_Parent_Canvas.renderMode==RenderMode.ScreenSpaceOverlay)
				{
					m_MoveIn.Position = new Vector3(m_CameraRightEdge/2,
					                                m_CameraTopEdge/2,
					                                m_MoveOriginal.z);
				}
				else
				{
					m_MoveIn.Position = new Vector3(0,
					                                0,
					                                m_MoveOriginal.z);
				}
				break;
			case ePosMoveIn.From_RightCenter:
				if(m_Parent_Canvas.renderMode==RenderMode.ScreenSpaceOverlay)
				{
					m_MoveIn.Position = new Vector3(m_CameraRightEdge+m_TotalBounds.size.x,
					                                m_CameraTopEdge/2,
					                                m_MoveOriginal.z);
				}
				else
				{
					m_MoveIn.Position = new Vector3(m_CameraRightEdge+m_TotalBounds.size.x,
					                                0,
					                                m_MoveOriginal.z);
				}
				break;
			case ePosMoveIn.From_BottomLeft:
				m_MoveIn.Position = new Vector3(m_CameraLeftEdge-m_TotalBounds.size.x,
				                                m_CameraBottomEdge-m_TotalBounds.size.y,
				                                m_MoveOriginal.z);
				break;
			case ePosMoveIn.From_BottomCenter:
				if(m_Parent_Canvas.renderMode==RenderMode.ScreenSpaceOverlay)
				{
					m_MoveIn.Position = new Vector3(m_CameraRightEdge/2,
					                                m_CameraBottomEdge-m_TotalBounds.size.y,
					                                m_MoveOriginal.z);
				}
				else
				{
					m_MoveIn.Position = new Vector3(0,
					                                m_CameraBottomEdge-m_TotalBounds.size.y,
					                                m_MoveOriginal.z);
				}
				break;
			case ePosMoveIn.From_BottomRight:
				m_MoveIn.Position = new Vector3(m_CameraRightEdge+m_TotalBounds.size.x,
				                                m_CameraBottomEdge-m_TotalBounds.size.y,
				                                m_MoveOriginal.z);
				break;
				
			case ePosMoveIn.From_CurrentPos:
				m_MoveIn.Position = m_MoveOriginal;
				break;
			}
			
			// update position with m_MoveIn.Position
			transform.position = m_MoveIn.Position;
		}
	}
	
	// Init Move-Out
	void InitMoveOut()
	{
		if(m_MoveOut.Enable == true)
		{
			// Find camera view size in 3d space dimention
			CalculateCameraArea();
			
			// Find total bounds
			CalculateTotalBounds();

			// update m_MoveOut.Position
			switch(m_MoveOut.MoveTo)
			{
			case ePosMoveOut.To_ParentPosition:
				if(transform.parent!=null)
				{
					m_MoveOut.Position = new Vector3(transform.parent.transform.position.x,transform.parent.transform.position.y,m_MoveOriginal.z);;
				}
				else
				{
					m_MoveOut.Position = new Vector3(m_CameraRightEdge/2,m_CameraTopEdge/2,m_MoveOriginal.z);
				}
				break;
			case ePosMoveOut.To_SpecificPosition:
				m_MoveOut.Position = new Vector3(m_MoveOut.Position.x,m_MoveOut.Position.y,m_MoveOriginal.z);
				break;
				
			case ePosMoveOut.To_UpperScreenEdge:
				m_MoveOut.Position = new Vector3(m_MoveOriginal.x,
				                                 m_CameraTopEdge+m_TotalBounds.size.y,
				                                 m_MoveOriginal.z);
				break;
			case ePosMoveOut.To_LeftScreenEdge:
				m_MoveOut.Position = new Vector3(m_CameraLeftEdge-m_TotalBounds.size.x,
				                                 m_MoveOriginal.y,
				                                 m_MoveOriginal.z);
				break;
			case ePosMoveOut.To_RightScreenEdge:
				m_MoveOut.Position = new Vector3(m_CameraRightEdge+m_TotalBounds.size.x,
				                                 m_MoveOriginal.y,
				                                 m_MoveOriginal.z);
				break;
			case ePosMoveOut.To_BottomScreenEdge:
				m_MoveOut.Position = new Vector3(m_MoveOriginal.x,
				                                 m_CameraBottomEdge-m_TotalBounds.size.y,
				                                 m_MoveOriginal.z);
				break;
				
			case ePosMoveOut.To_UpperLeft:
				m_MoveOut.Position = new Vector3(m_CameraLeftEdge-m_TotalBounds.size.x,
				                                 m_CameraTopEdge+m_TotalBounds.size.y,
				                                 m_MoveOriginal.z);	
				break;
			case ePosMoveOut.To_UpperCenter:
				if(m_Parent_Canvas.renderMode==RenderMode.ScreenSpaceOverlay)
				{
					m_MoveOut.Position = new Vector3(m_CameraRightEdge/2,
					                                 m_CameraTopEdge+m_TotalBounds.size.y,
					                                 m_MoveOriginal.z);	
				}
				else
				{
					m_MoveOut.Position = new Vector3(0,
					                                 m_CameraTopEdge+m_TotalBounds.size.y,
					                                 m_MoveOriginal.z);	
				}
				break;
			case ePosMoveOut.To_UpperRight:
				m_MoveOut.Position = new Vector3(m_CameraRightEdge+m_TotalBounds.size.x,
				                                 m_CameraTopEdge+m_TotalBounds.size.y,
				                                 m_MoveOriginal.z);	
				break;
			case ePosMoveOut.To_LeftCenter:
				if(m_Parent_Canvas.renderMode==RenderMode.ScreenSpaceOverlay)
				{
					m_MoveOut.Position = new Vector3(m_CameraLeftEdge-m_TotalBounds.size.x,
					                                 m_CameraTopEdge/2,
					                                 m_MoveOriginal.z);	
				}
				else
				{
					m_MoveOut.Position = new Vector3(m_CameraLeftEdge-m_TotalBounds.size.x,
					                                 0,
					                                 m_MoveOriginal.z);	
				}
				break;
			case ePosMoveOut.To_Center:
				if(m_Parent_Canvas.renderMode==RenderMode.ScreenSpaceOverlay)
				{
					m_MoveOut.Position = new Vector3(m_CameraRightEdge/2,
					                                 m_CameraTopEdge/2,
					                                 m_MoveOriginal.z);
				}
				else
				{
					m_MoveOut.Position = new Vector3(0,
					                                 0,
					                                 m_MoveOriginal.z);
				}
				break;
			case ePosMoveOut.To_RightCenter:
				if(m_Parent_Canvas.renderMode==RenderMode.ScreenSpaceOverlay)
				{
					m_MoveOut.Position = new Vector3(m_CameraRightEdge+m_TotalBounds.size.x,
					                                 m_CameraTopEdge/2,
					                                 m_MoveOriginal.z);	
				}
				else
				{
					m_MoveOut.Position = new Vector3(m_CameraRightEdge+m_TotalBounds.size.x,
					                                 0,
					                                 m_MoveOriginal.z);	
				}
				break;
			case ePosMoveOut.To_BottomLeft:
				m_MoveOut.Position = new Vector3(m_CameraLeftEdge-m_TotalBounds.size.x,
				                                 m_CameraBottomEdge-m_TotalBounds.size.y,
				                                 m_MoveOriginal.z);
				break;
			case ePosMoveOut.To_BottomCenter:
				if(m_Parent_Canvas.renderMode==RenderMode.ScreenSpaceOverlay)
				{
					m_MoveOut.Position = new Vector3(m_CameraRightEdge/2,
					                                 m_CameraBottomEdge-m_TotalBounds.size.y,
					                                 m_MoveOriginal.z);
				}
				else
				{
					m_MoveOut.Position = new Vector3(0,
					                                 m_CameraBottomEdge-m_TotalBounds.size.y,
					                                 m_MoveOriginal.z);
				}
				break;
			case ePosMoveOut.To_BottomRight:
				m_MoveOut.Position = new Vector3(m_CameraRightEdge+m_TotalBounds.size.x,
				                                 m_CameraBottomEdge-m_TotalBounds.size.y,
				                                 m_MoveOriginal.z);
				break;
				
			case ePosMoveOut.To_CurrentPos:
				m_MoveOut.Position = m_MoveOriginal;
				break;
			}
		}
	}
	
	// Init Scale
	void InitScaleIn()
	{
		if(m_ScaleIn.Enable == true)
		{
			transform.localScale = m_ScaleIn.ScaleBegin;
		}
	}
	
	// Init Fade
	void InitFadeIn()
	{
		if(m_FadeIn.Enable == true)
		{
			RecursiveFade(this.transform, m_FadeIn.Fade, m_FadeIn.FadeChildren);
		}
	}
	
	#endregion
	
	// ######################################################################
	// Move In Functions
	// ######################################################################
	
	#region Move In
	
	// Moves in this object
	public void MoveIn()
	{
		MoveIn(eGUIMove.SelfAndChildren);
	}
	
	// Moves this object in according to eGUIMove type
	public void MoveIn(eGUIMove GUIMoveType)
	{
		// GUIMoveType is Self or SelfAndChildren
		if(GUIMoveType==eGUIMove.Self || GUIMoveType==eGUIMove.SelfAndChildren)
		{
			// Check is move-in enable
			if(m_MoveIn.Enable)
			{
				#if HOTWEEN
				m_MoveVariable = 0.0f;
				// HOTween: https://www.assetstore.unity3d.com/#/content/3311 Documentation:  http://www.holoville.com/hotween/documentation.html
				HOTween.To(this, m_MoveIn.Time/GEAnimSystem.Instance.m_GUISpeed, new TweenParms()
				           .Prop("m_MoveVariable", 1.0f, false)
				           .Delay(m_MoveIn.Delay/GEAnimSystem.Instance.m_GUISpeed)
				           .Ease(HOTweenEaseType(m_MoveIn.EaseType))
				           .OnUpdate(AnimIn_MoveUpdate)
				           .OnStepComplete(AnimIn_MoveComplete)
				           .UpdateType(UpdateType.TimeScaleIndependentUpdate)
				           );
				
				m_MoveIn.Began = true;
				#elif ITWEEN
				// iTween: https://www.assetstore.unity3d.com/#/content/84 Documentation: http://itween.pixelplacement.com/documentation.php
				iTween.ValueTo(this.gameObject, iTween.Hash("from", 0.0f,
				                                            "to", 1.0f,
				                                            "time", m_MoveIn.Time/GEAnimSystem.Instance.m_GUISpeed,
				                                            "ignoretimescale", true,
				                                            "delay",m_MoveIn.Delay/GEAnimSystem.Instance.m_GUISpeed,
				                                            "easeType", iTweenEaseType(m_MoveIn.EaseType),
				                                            "onupdate", "AnimIn_MoveUpdateByValue",
				                                            "onupdatetarget", this.gameObject,
				                                            "oncomplete", "AnimIn_MoveComplete"));
				
				m_MoveIn.Began = true;
				#else
				// LeanTween: https://www.assetstore.unity3d.com/#/content/3595 Documentation: http://dentedpixel.com/LeanTweenDocumentation/classes/LeanTween.html
				LeanTween.value(this.gameObject, AnimIn_MoveUpdateByValue, 0.0f, 1.0f, m_MoveIn.Time/GEAnimSystem.Instance.m_GUISpeed)
					.setDelay(m_MoveIn.Delay/GEAnimSystem.Instance.m_GUISpeed)
						.setEase(LeanTweenEaseType(m_MoveIn.EaseType))
						.setOnComplete(AnimIn_MoveComplete)
						.setUseEstimatedTime( true );
				#endif
				
				m_MoveIn.Began = true;
				m_MoveIn.Done = false;
			}
			
			// Check is scale-in enable
			if(m_ScaleIn.Enable)
			{
				#if HOTWEEN
				m_ScaleVariable = 0.0f;
				// HOTween: https://www.assetstore.unity3d.com/#/content/3311 Documentation:  http://www.holoville.com/hotween/documentation.html
				HOTween.To(this, m_ScaleIn.Time/GEAnimSystem.Instance.m_GUISpeed, new TweenParms()
				           .Prop("m_ScaleVariable", 1.0f, false)
				           .Delay(m_ScaleIn.Delay/GEAnimSystem.Instance.m_GUISpeed)
				           .Ease(HOTweenEaseType(m_ScaleIn.EaseType))
				           .OnUpdate(AnimIn_ScaleUpdate)
				           .OnStepComplete(AnimIn_ScaleComplete)
				           .UpdateType(UpdateType.TimeScaleIndependentUpdate)
				           );
				
				m_ScaleIn.Began = true;
				#elif ITWEEN
				// iTween: https://www.assetstore.unity3d.com/#/content/84 Documentation: http://itween.pixelplacement.com/documentation.php
				iTween.ValueTo(this.gameObject, iTween.Hash("from", 0,
				                                            "to", 1.0f,
				                                            "time", m_ScaleIn.Time/GEAnimSystem.Instance.m_GUISpeed,
				                                            "ignoretimescale", true,
				                                            "delay",m_ScaleIn.Delay/GEAnimSystem.Instance.m_GUISpeed,
				                                            "easeType", iTweenEaseType(m_ScaleIn.EaseType),
				                                            "onupdate", "AnimIn_ScaleUpdateByValue",
				                                            "onupdatetarget", this.gameObject,
				                                            "oncomplete", "AnimIn_ScaleComplete"));
				
				m_ScaleIn.Began = true;
				#else
				// LeanTween: https://www.assetstore.unity3d.com/#/content/3595 Documentation: http://dentedpixel.com/LeanTweenDocumentation/classes/LeanTween.html
				LeanTween.value(this.gameObject, AnimIn_ScaleUpdateByValue, 0, 1.0f, m_ScaleIn.Time/GEAnimSystem.Instance.m_GUISpeed)
					.setDelay(m_ScaleIn.Delay/GEAnimSystem.Instance.m_GUISpeed)
						.setEase(LeanTweenEaseType(m_ScaleIn.EaseType))
						.setOnComplete(AnimIn_ScaleComplete)
						.setUseEstimatedTime( true );
				#endif
				
				m_ScaleIn.Began = true;
				m_ScaleIn.Done = false;
				
			}
			
			// Check is fade-in enable
			if(m_FadeIn.Enable)
			{
				#if HOTWEEN
				m_FadeVariable = 0.0f;
				// HOTween: https://www.assetstore.unity3d.com/#/content/3311 Documentation:  http://www.holoville.com/hotween/documentation.html
				HOTween.To(this, m_FadeIn.Time/GEAnimSystem.Instance.m_GUISpeed, new TweenParms()
				           .Prop("m_FadeVariable", 1.0f, false)
				           .Delay(m_FadeIn.Delay/GEAnimSystem.Instance.m_GUISpeed)
				           .Ease(HOTweenEaseType(m_FadeIn.EaseType))
				           .OnUpdate(AnimIn_FadeUpdate)
				           .OnStepComplete(AnimIn_FadeComplete)
				           .UpdateType(UpdateType.TimeScaleIndependentUpdate)
				           );
				#elif ITWEEN
				// iTween: https://www.assetstore.unity3d.com/#/content/84 Documentation: http://itween.pixelplacement.com/documentation.php
				iTween.ValueTo(this.gameObject, iTween.Hash("from", 0,
				                                            "to", 1.0f,
				                                            "time", m_FadeIn.Time/GEAnimSystem.Instance.m_GUISpeed,
				                                            "ignoretimescale", true,
				                                            "delay",m_FadeIn.Delay/GEAnimSystem.Instance.m_GUISpeed,
				                                            "easeType", iTweenEaseType(m_FadeIn.EaseType),
				                                            "onupdate", "AnimIn_FadeUpdateByValue",
				                                            "onupdatetarget", this.gameObject,
				                                            "oncomplete", "AnimIn_FadeComplete"));
				#else
				// LeanTween: https://www.assetstore.unity3d.com/#/content/3595 Documentation: http://dentedpixel.com/LeanTweenDocumentation/classes/LeanTween.html
				LeanTween.value(this.gameObject, AnimIn_FadeUpdateByValue, 0, 1.0f, m_FadeIn.Time/GEAnimSystem.Instance.m_GUISpeed)
					.setDelay(m_FadeIn.Delay/GEAnimSystem.Instance.m_GUISpeed)
						.setEase(LeanTweenEaseType(m_FadeIn.EaseType))
						.setOnComplete(AnimIn_FadeComplete)
						.setUseEstimatedTime( true );
				#endif
				
				m_FadeIn.Began = true;
				m_FadeIn.Done = false;
			}
			
			// Start scale loop
			if(m_ScaleLoop.Enable)
			{
				ScaleLoopStart(m_ScaleLoop.Delay);
			}
			
			// Start fade loop
			if(m_FadeLoop.Enable)
			{
				FadeLoopStart(m_FadeLoop.Delay);
			}
			
		}
		
		// Check if there is m_SoundMoveIn, play sound once
		if(m_SoundMoveIn!=null)
		{
			PlaySoundOneShot(m_SoundMoveIn);
		}
		
		// Check if GUIMoveType is Children or SelfAndChildren then call children's Move-In
		if(GUIMoveType==eGUIMove.Children || GUIMoveType==eGUIMove.SelfAndChildren)
		{
			foreach(Transform child in this.transform)
			{
				GEAnim pGEAnim = child.gameObject.GetComponent<GEAnim>();
				if(pGEAnim!=null)
				{
					pGEAnim.MoveIn(eGUIMove.SelfAndChildren);
				}
			}
		}
	}
	
	#endregion
	
	// ######################################################################
	// Move Out Functions
	// ######################################################################
	
	#region Move Out
	
	// Moves out this object
	public void MoveOut()
	{
		MoveOut(eGUIMove.SelfAndChildren);
	}
	
	#if UNITY_EDITOR
	IEnumerator CoroutineMoveOut(float Delay)
	{
		yield return new WaitForSeconds(Delay);
		MoveOut();
	}
	#endif
	
	// Moves this object out according to eGUIMove type
	public void MoveOut(eGUIMove GUIMoveType)
	{
		// GUIMoveType is Self or SelfAndChildren
		if(GUIMoveType==eGUIMove.Self || GUIMoveType==eGUIMove.SelfAndChildren)
		{
			// Check is move-out enable
			if(m_MoveOut.Enable)
			{
				#if HOTWEEN
				m_MoveVariable = 0.0f;
				// HOTween: https://www.assetstore.unity3d.com/#/content/3311 Documentation:  http://www.holoville.com/hotween/documentation.html
				HOTween.To(this, m_MoveOut.Time/GEAnimSystem.Instance.m_GUISpeed, new TweenParms()
				           .Prop("m_MoveVariable", 1.0f, false)
				           .Delay(m_MoveOut.Delay/GEAnimSystem.Instance.m_GUISpeed)
				           .Ease(HOTweenEaseType(m_MoveOut.EaseType))
				           .OnUpdate(AnimOut_MoveUpdate)
				           .OnStepComplete(AnimOut_MoveComplete)
				           .UpdateType(UpdateType.TimeScaleIndependentUpdate)
				           );
				
				m_MoveOut.Began = true;
				#elif ITWEEN
				// iTween: https://www.assetstore.unity3d.com/#/content/84 Documentation: http://itween.pixelplacement.com/documentation.php
				iTween.ValueTo(this.gameObject, iTween.Hash("from", 0.0f,
				                                            "to", 1.0f,
				                                            "time", m_MoveOut.Time/GEAnimSystem.Instance.m_GUISpeed,
				                                            "ignoretimescale", true,
				                                            "delay",m_MoveOut.Delay/GEAnimSystem.Instance.m_GUISpeed,
				                                            "easeType", iTweenEaseType(m_MoveOut.EaseType),
				                                            "onupdate", "AnimOut_MoveUpdateByValue",
				                                            "onupdatetarget", this.gameObject,
				                                            "oncomplete", "AnimOut_MoveComplete"));
				
				m_MoveOut.Began = true;
				#else
				// LeanTween: https://www.assetstore.unity3d.com/#/content/3595 Documentation: http://dentedpixel.com/LeanTweenDocumentation/classes/LeanTween.html
				LeanTween.value(this.gameObject, AnimOut_MoveUpdateByValue, 0.0f, 1.0f, m_MoveOut.Time/GEAnimSystem.Instance.m_GUISpeed)
					.setDelay(m_MoveOut.Delay/GEAnimSystem.Instance.m_GUISpeed)
						.setEase(LeanTweenEaseType(m_MoveOut.EaseType))
						.setOnComplete(AnimOut_MoveComplete)
						.setUseEstimatedTime( true );
				#endif
				
				m_MoveOut.Began = true;
				m_MoveOut.Done = false;
			}
			
			// Check is scale-out enable
			if(m_ScaleOut.Enable)
			{
				#if HOTWEEN
				m_ScaleVariable = 0.0f;
				// HOTween: https://www.assetstore.unity3d.com/#/content/3311 Documentation:  http://www.holoville.com/hotween/documentation.html
				HOTween.To(this, m_ScaleOut.Time/GEAnimSystem.Instance.m_GUISpeed, new TweenParms()
				           .Prop("m_ScaleVariable", 1.0f, false)
				           .Delay(m_ScaleOut.Delay/GEAnimSystem.Instance.m_GUISpeed)
				           .Ease(HOTweenEaseType(m_ScaleOut.EaseType))
				           .OnUpdate(AnimOut_ScaleUpdate)
				           .OnStepComplete(AnimOut_ScaleComplete)
				           .UpdateType(UpdateType.TimeScaleIndependentUpdate)
				           );
				
				m_ScaleOut.Began = true;
				#elif ITWEEN
				// iTween: https://www.assetstore.unity3d.com/#/content/84 Documentation: http://itween.pixelplacement.com/documentation.php
				iTween.ValueTo(this.gameObject, iTween.Hash("from", 0.0f,
				                                            "to", 1.0f,
				                                            "time", m_ScaleOut.Time/GEAnimSystem.Instance.m_GUISpeed,
				                                            "ignoretimescale", true,
				                                            "delay",m_ScaleOut.Delay/GEAnimSystem.Instance.m_GUISpeed,
				                                            "easeType", iTweenEaseType(m_ScaleOut.EaseType),
				                                            "onupdate", "AnimOut_ScaleUpdateByValue",
				                                            "onupdatetarget", this.gameObject,
				                                            "oncomplete", "AnimOut_ScaleComplete"));
				
				m_ScaleOut.Began = true;
				#else
				// LeanTween: https://www.assetstore.unity3d.com/#/content/3595 Documentation: http://dentedpixel.com/LeanTweenDocumentation/classes/LeanTween.html
				LeanTween.value(this.gameObject, AnimOut_ScaleUpdateByValue, 0.0f, 1.0f, m_ScaleOut.Time/GEAnimSystem.Instance.m_GUISpeed)
					.setDelay(m_ScaleOut.Delay/GEAnimSystem.Instance.m_GUISpeed)
						.setEase(LeanTweenEaseType(m_ScaleOut.EaseType))
						.setOnComplete(AnimOut_ScaleComplete)							
						.setUseEstimatedTime( true );
				#endif
				
				m_ScaleOut.Began = true;
				m_ScaleOut.Done = false;

			}
			
			// Check is fade-out enable
			if(m_FadeOut.Enable)
			{
				#if HOTWEEN
				m_FadeVariable = 0.0f;
				// HOTween: https://www.assetstore.unity3d.com/#/content/3311 Documentation:  http://www.holoville.com/hotween/documentation.html
				HOTween.To(this, m_FadeOut.Time/GEAnimSystem.Instance.m_GUISpeed, new TweenParms()
				           .Prop("m_FadeVariable", 1.0f, false)
				           .Delay(m_FadeOut.Delay/GEAnimSystem.Instance.m_GUISpeed)
				           .Ease(HOTweenEaseType(m_FadeOut.EaseType))
				           .OnUpdate(AnimOut_FadeUpdate)
				           .OnStepComplete(AnimOut_FadeComplete)
				           .UpdateType(UpdateType.TimeScaleIndependentUpdate)
				           );
				#elif ITWEEN
				// iTween: https://www.assetstore.unity3d.com/#/content/84 Documentation: http://itween.pixelplacement.com/documentation.php
				iTween.ValueTo(this.gameObject, iTween.Hash("from", 0,
				                                            "to", 1.0f,
				                                            "time", m_FadeOut.Time/GEAnimSystem.Instance.m_GUISpeed,
				                                            "ignoretimescale", true,
				                                            "delay",m_FadeOut.Delay/GEAnimSystem.Instance.m_GUISpeed,
				                                            "easeType", iTweenEaseType(m_FadeOut.EaseType),
				                                            "onupdate", "AnimOut_FadeUpdateByValue",
				                                            "onupdatetarget", this.gameObject,
				                                            "oncomplete", "AnimOut_FadeComplete"));
				#else
				// LeanTween: https://www.assetstore.unity3d.com/#/content/3595 Documentation: http://dentedpixel.com/LeanTweenDocumentation/classes/LeanTween.html
				LeanTween.value(this.gameObject, AnimOut_FadeUpdateByValue, 0, 1.0f, m_FadeOut.Time/GEAnimSystem.Instance.m_GUISpeed)
					.setDelay(m_FadeOut.Delay/GEAnimSystem.Instance.m_GUISpeed)
						.setEase(LeanTweenEaseType(m_FadeOut.EaseType))
						.setOnComplete(AnimOut_FadeComplete)							
						.setUseEstimatedTime( true );
				#endif
				
				m_FadeOut.Began = true;
				m_FadeOut.Done = false;
			}
			
			// Stop scale loop
			if(m_ScaleLoop.Enable)
			{
				m_ScaleLoop.Began = false;
				m_ScaleLoop.Done = true;
			}
			
			// Stop fade loop
			if(m_FadeLoop.Enable)
			{
				m_FadeLoop.Began = false;
				m_FadeLoop.Done = true;
			}
		}
		
		// Check if there is m_SoundMoveOut, play sound once
		if(m_SoundMoveOut!=null)
		{
			PlaySoundOneShot(m_SoundMoveOut);
		}
		
		// Check if GUIMoveType is Children or SelfAndChildren then call children's Move-Out
		if(GUIMoveType==eGUIMove.Children || GUIMoveType==eGUIMove.SelfAndChildren)
		{
			foreach(Transform child in this.transform)			
			{
				GEAnim pGEAnim = child.gameObject.GetComponent<GEAnim>();
				if(pGEAnim!=null)
				{
					pGEAnim.MoveOut(eGUIMove.SelfAndChildren);
				}
			}
		}
	}

	// Play m_SoundMoveIn or m_SoundMoveOut
	void PlaySoundOneShot(AudioClip pAudioClip)
	{
		// Check if we have an AudioListener in the scene
		AudioListener pAudioListener = GameObject.FindObjectOfType<AudioListener>();
		if(pAudioListener!=null)
		{
			bool IsPlaySuccess = false;
			AudioSource[] pAudioSourceList = pAudioListener.gameObject.GetComponents<AudioSource>();
			if(pAudioSourceList.Length>0)
			{
				for(int i=0;i<pAudioSourceList.Length;i++)
				{
					if(pAudioSourceList[i].isPlaying==false)
					{
						IsPlaySuccess = true;
						pAudioSourceList[i].PlayOneShot(pAudioClip);
						break;
					}
				}
			}
			
			if(IsPlaySuccess==false)
			{
				// Add AudioSource to AudioListener game object if there is no AudioSource
				AudioSource pAudioSource = pAudioListener.gameObject.AddComponent<AudioSource>();
				pAudioSource.rolloffMode = AudioRolloffMode.Linear;
				pAudioSource.playOnAwake = false;
				pAudioSource.PlayOneShot(pAudioClip);
			}
		}
	}
	
	#endregion
	
	// ######################################################################
	// Move In/Out Animation by Tweeners
	// ######################################################################
	
	#region Move In/Out Animation by Tweeners
	
	#if HOTWEEN
	// Animate Move-In by m_MoveVariable
	// This function is used by HOTween.To()
	void AnimIn_MoveUpdate()
	{
		AnimIn_MoveUpdateByValue(m_MoveVariable);
	}
	#endif

	// This function is used by iTween.ValueTo() and LeanTween.value()
	void AnimIn_MoveUpdateByValue(float Value)
	{
		this.transform.position = new Vector3(m_MoveIn.Position.x + ((m_MoveOriginal.x-m_MoveIn.Position.x) * Value),
		                                      m_MoveIn.Position.y + ((m_MoveOriginal.y-m_MoveIn.Position.y) * Value),
		                                      m_MoveIn.Position.z + ((m_MoveOriginal.z-m_MoveIn.Position.z) * Value));
	}

	// Move-In animate completed
	void AnimIn_MoveComplete()
	{
		m_MoveIn.Began = false;
		m_MoveIn.Done = true;
		
		m_MoveOut.Began = false;
		m_MoveOut.Done = false;
	}


	#if HOTWEEN
	// Animate Move-Out by m_MoveVariable
	// This function is used by HOTween.To()
	void AnimOut_MoveUpdate()
	{
		AnimOut_MoveUpdateByValue(m_MoveVariable);
	}
	#endif

	// This function is used by iTween.ValueTo() and LeanTween.value()
	void AnimOut_MoveUpdateByValue(float Value)
	{
		this.transform.position = new Vector3(m_MoveOriginal.x + ((m_MoveOut.Position.x-m_MoveOriginal.x) * Value),
		                                      m_MoveOriginal.y + ((m_MoveOut.Position.y-m_MoveOriginal.y) * Value),
		                                      m_MoveOriginal.z + ((m_MoveOut.Position.z-m_MoveOriginal.z) * Value));
	}
	
	// Move-Out animate completed
	void AnimOut_MoveComplete()
	{		
		m_MoveIn.Began = false;
		m_MoveIn.Done = false;

		m_MoveOut.Began = false;
		m_MoveOut.Done = true;
	}
	
	#endregion
	
	// ######################################################################
	// Scale In/Out Animation by Tweeners
	// ######################################################################
	
	#region Scale In/Out Animation by Tweeners
	
	#if HOTWEEN
	// Animate Scale-In by m_ScaleVariable
	// Used by HOTween.To()
	void AnimIn_ScaleUpdate()
	{
		AnimIn_ScaleUpdateByValue(m_ScaleVariable);
	}
	#else
	// This function is used by iTween.ValueTo() and LeanTween.value()
	void AnimIn_ScaleUpdateByValue(float Value)
	{
		transform.localScale = new Vector3(m_ScaleIn.ScaleBegin.x + ((m_ScaleOriginal.x-m_ScaleIn.ScaleBegin.x) * Value),
		                                   m_ScaleIn.ScaleBegin.y + ((m_ScaleOriginal.y-m_ScaleIn.ScaleBegin.y) * Value),
		                                   m_ScaleIn.ScaleBegin.z + ((m_ScaleOriginal.z-m_ScaleIn.ScaleBegin.z) * Value));
	}
	#endif

	// Animate Scale-In animate completed
	void AnimIn_ScaleComplete()
	{
		m_ScaleIn.Began = false;
		m_ScaleIn.Done = true;
		
		m_ScaleOut.Began = false;
		m_ScaleOut.Done = false;

		AnimIn_ScaleUpdateByValue(1.0f);
	}
	
	#if HOTWEEN
	// Animate Scale-Out by m_ScaleVariable
	// This function is used by HOTween.To()
	void AnimOut_ScaleUpdate()
	{
		AnimOut_ScaleUpdateByValue(m_ScaleVariable);
	}
	#endif
	
	// This function is used by iTween.ValueTo() and LeanTween.value()
	void AnimOut_ScaleUpdateByValue(float Value)
	{
		transform.localScale = new Vector3(m_ScaleOriginal.x + ((m_ScaleOut.ScaleEnd.x-m_ScaleOriginal.x) * Value),
		                                   m_ScaleOriginal.y + ((m_ScaleOut.ScaleEnd.y-m_ScaleOriginal.y) * Value),
		                                   m_ScaleOriginal.z + ((m_ScaleOut.ScaleEnd.z-m_ScaleOriginal.z) * Value));
	}
	
	// Animate Scale-In animate completed
	void AnimOut_ScaleComplete()
	{
		m_ScaleIn.Began = false;
		m_ScaleIn.Done = false;

		m_ScaleOut.Began = false;
		m_ScaleOut.Done = true;

		AnimOut_ScaleUpdateByValue(1.0f);
	}
	
	#endregion
	
	// ######################################################################
	// Fade In/Out Animation by Tweeners
	// ######################################################################
	
	#region Fade In/Out Animation by Tweeners

	#if HOTWEEN
	// Animate Move-In by m_FadeVariable
	// This function is used by HOTween.To()
	void AnimIn_FadeUpdate()
	{
		AnimIn_FadeUpdateByValue(m_FadeVariable);
	}
	#endif

	// Used by iTween.ValueTo() and LeanTween.value()
	void AnimIn_FadeUpdateByValue(float Value)
	{
		RecursiveFade(this.transform, m_FadeIn.Fade + ((m_FadeOriginal-m_FadeIn.Fade)*Value), m_FadeIn.FadeChildren);
	}


	
	// FadeIn animate completed
	void AnimIn_FadeComplete()
	{
		m_FadeIn.Began = false;
		m_FadeIn.Done = true;

		m_FadeOut.Began = false;
		m_FadeOut.Done = false;

		AnimIn_FadeUpdateByValue(1.0f);
	}
	
	#if HOTWEEN
	// Animate FadeIn by m_FadeVariable
	// This function is used by HOTween.To()
	void AnimOut_FadeUpdate()
	{
		AnimOut_FadeUpdateByValue(m_FadeVariable);
	}
	#endif

	// Used by iTween.ValueTo() and LeanTween.value()
	void AnimOut_FadeUpdateByValue(float Value)
	{
		RecursiveFade(this.transform, m_FadeOriginal + ((m_FadeOut.Fade-m_FadeOriginal)*Value), m_FadeOut.FadeChildren);
	}

	
	// FadeOut animate completed
	void AnimOut_FadeComplete()
	{
		m_FadeIn.Began = false;
		m_FadeIn.Done = false;

		m_FadeOut.Began = false;
		m_FadeOut.Done = true;

		AnimOut_FadeUpdateByValue(1.0f);
	}
	
	#endregion
	
	// ######################################################################
	// Scale Loop In/Out Animation by Tweeners
	// ######################################################################
	
	#region Scale Loop In/Out Animation by Tweeners
	
	// Begin Scale-loop
	void ScaleLoopStart(float delay)
	{
		#if HOTWEEN
		m_ScaleVariable = 0.0f;
		// HOTween: https://www.assetstore.unity3d.com/#/content/3311 Documentation:  http://www.holoville.com/hotween/documentation.html
		HOTween.To(this, (m_ScaleLoop.Time)/2.0f, new TweenParms()
		           .Prop("m_BreathVariable", 1.0f, false)
		           .Delay(delay)
		           .Ease(HOTweenEaseType(m_ScaleLoop.EaseType))
		           .OnUpdate(ScaleLoopUpdate)
		           .UpdateType(UpdateType.TimeScaleIndependentUpdate)
		           );
		HOTween.To(this, (m_ScaleLoop.Time)/2.0f, new TweenParms()
		           .Prop("m_BreathVariable", 0.0f, false)
		           .Delay(delay+((m_FadeLoop.Time)/2.0f))
		           .Ease(HOTweenEaseType(m_ScaleLoop.EaseType))
		           .OnUpdate(ScaleLoopUpdate)
		           .OnStepComplete(ScaleLoopComplete)
		           .UpdateType(UpdateType.TimeScaleIndependentUpdate)
		           );
		#elif ITWEEN
		// iTween: https://www.assetstore.unity3d.com/#/content/84 Documentation: http://itween.pixelplacement.com/documentation.php
		iTween.ValueTo(this.gameObject, iTween.Hash("from", 0.0f,
		                                            "to", 1.0f,
		                                            "time", (m_ScaleLoop.Time)/2.0f,
		                                            "ignoretimescale", true,
		                                            "delay",delay,
		                                            "easeType", iTweenEaseType(m_ScaleLoop.EaseType),
		                                            "onupdate", "BreathLoopUpdateByValue",
		                                            "onupdatetarget", this.gameObject
		                                            ));
		iTween.ValueTo(this.gameObject, iTween.Hash("from", 1.0f,
		                                            "to", 0.0f,
		                                            "time", (m_ScaleLoop.Time)/2.0f,
		                                            "ignoretimescale", true,
		                                            "delay",delay+((m_FadeLoop.Time)/2.0f),
		                                            "easeType", iTweenEaseType(m_ScaleLoop.EaseType),
		                                            "onupdate", "BreathLoopUpdateByValue",
		                                            "onupdatetarget", this.gameObject,
		                                            "oncomplete", "BreathLoopComplete"
		                                            ));
		#else
		// LeanTween: https://www.assetstore.unity3d.com/#/content/3595 Documentation: http://dentedpixel.com/LeanTweenDocumentation/classes/LeanTween.html
		LeanTween.value(this.gameObject, ScaleLoopUpdateByValue, 0.0f, 1.0f, (m_ScaleLoop.Time)/2.0f)
			.setDelay(delay)
				.setEase(LeanTweenEaseType(m_ScaleLoop.EaseType))
				.setUseEstimatedTime( true );
		LeanTween.value(this.gameObject, ScaleLoopUpdateByValue, 1.0f, 0.0f, (m_ScaleLoop.Time)/2.0f)
			.setDelay(delay+((m_FadeLoop.Time)/2.0f))
				.setEase(LeanTweenEaseType(m_ScaleLoop.EaseType))
				.setOnComplete(ScaleLoopComplete)
				.setUseEstimatedTime( true );
		#endif
		

		
		m_ScaleIsOverriding	= false;
		m_ScaleLast = transform.localScale;

		m_ScaleLoop.Began = true;
		m_ScaleLoop.Done = false;
	}
	
	#if HOTWEEN
	// This function is used by HOTween.To()
	void ScaleLoopUpdate()
	{
		ScaleLoopUpdateByValue(m_ScaleVariable);
	}
	#endif

	// This function is used by iTween.ValueTo() and LeanTween.value()
	void ScaleLoopUpdateByValue(float Value)
	{
		
		if(m_ScaleIsOverriding == true)
		{
			m_ScaleIsOverriding = true;
			return;
		}
		
		// Return when scale is overriding from outside
		if(m_ScaleLast != transform.localScale)
		{
			m_ScaleIsOverriding = true;
			return;
		}
		
		// Pause scale-loop when scale in/out is animating
		if((m_ScaleIn.Enable && m_ScaleIn.Began==true && m_ScaleIn.Done==false) || 
		   (m_ScaleOut.Enable && m_ScaleOut.Began==true && m_ScaleOut.Done==false))
		{
			return;
		}
		else
		{
			// Update scale
			transform.localScale = new Vector3(m_ScaleLoop.Min.x + ((m_ScaleLoop.Max.x-m_ScaleLoop.Min.x) * Value),
			                                   m_ScaleLoop.Min.y + ((m_ScaleLoop.Max.y-m_ScaleLoop.Min.y) * Value),
			                                   m_ScaleLoop.Min.z + ((m_ScaleLoop.Max.z-m_ScaleLoop.Min.z) * Value));
			
		}
		
		m_ScaleLast = transform.localScale;
	}
	
	// Scale-Loop animate completed
	void ScaleLoopComplete()
	{
		// Return when scale is overriding from outside
		if(m_ScaleIsOverriding == true)
		{
			float difX = transform.localScale.x-m_ScaleOriginal.x;
			float difY = transform.localScale.y-m_ScaleOriginal.y;

			float checkDifX = 0.05f;
			float checkDifY = 0.05f;
            
			if(difX<checkDifX && difY<checkDifY)
			{
				if(m_ScaleLast.x==transform.localScale.x && m_ScaleLast.y==transform.localScale.y)
				{
					m_ScaleIsOverriding = false;

					transform.localScale = m_ScaleOriginal;
				}
			}

			m_ScaleLast = transform.localScale;
			StartCoroutine(CoRoutineScaleLoopComplete());
		}
		else
		{
			m_ScaleIsOverriding = false;
			m_ScaleLast = transform.localScale;

			if(m_ScaleLoop.Done==true)
			{
			}
			else
			{
				ScaleLoopStart(0);
			}
		}
	}

	IEnumerator CoRoutineScaleLoopComplete()
	{
		yield return new WaitForSeconds(0.01f);

		ScaleLoopComplete();
	}
	
	#endregion
	
	// ######################################################################
	// Fade Loop Animation by Tweeners
	// ######################################################################
	
	#region Fade Loop Animation by Tweeners
	
	// Begin Fade-loop
	void FadeLoopStart(float delay)
	{
		#if HOTWEEN
		m_FadeVariable = 0.0f;
		// HOTween: https://www.assetstore.unity3d.com/#/content/3311 Documentation:  http://www.holoville.com/hotween/documentation.html
		HOTween.To(this, (m_FadeLoop.Time)/2.0f, new TweenParms()
		           .Prop("m_FadeVariable", 1.0f, false)
		           .Delay(delay)
		           .Ease(HOTweenEaseType(m_FadeLoop.EaseType))
		           .OnUpdate(FadeLoopUpdate)
		           .UpdateType(UpdateType.TimeScaleIndependentUpdate)
		           );
		HOTween.To(this, (m_FadeLoop.Time)/2.0f, new TweenParms()
		           .Prop("m_FadeVariable", 0.0f, false)
		           .Delay(delay+((m_FadeLoop.Time)/2.0f))
		           .Ease(HOTweenEaseType(m_FadeLoop.EaseType))
		           .OnUpdate(FadeLoopUpdate)
		           .OnStepComplete(FadeLoopComplete)
		           .UpdateType(UpdateType.TimeScaleIndependentUpdate)
		           );
		#elif ITWEEN
		// iTween: https://www.assetstore.unity3d.com/#/content/84 Documentation: http://itween.pixelplacement.com/documentation.php
		iTween.ValueTo(this.gameObject, iTween.Hash("from", 0.0f,
		                                            "to", 1.0f,
		                                            "time", (m_FadeLoop.Time)/2.0f,
		                                            "ignoretimescale", true,
		                                            "delay",delay,
		                                            "easeType", iTweenEaseType(m_FadeLoop.EaseType),
		                                            "onupdate", "FadeLoopUpdateByValue",
		                                            "onupdatetarget", this.gameObject
		                                            ));
		iTween.ValueTo(this.gameObject, iTween.Hash("from", 1.0f,
		                                            "to", 0.0f,
		                                            "time", (m_FadeLoop.Time)/2.0f,
		                                            "ignoretimescale", true,
		                                            "delay",delay+((m_FadeLoop.Time)/2.0f),
		                                            "easeType", iTweenEaseType(m_FadeLoop.EaseType),
		                                            "onupdate", "FadeLoopUpdateByValue",
		                                            "onupdatetarget", this.gameObject,
		                                            "oncomplete", "FadeLoopComplete"
		                                            ));
		#else
		// LeanTween: https://www.assetstore.unity3d.com/#/content/3595 Documentation: http://dentedpixel.com/LeanTweenDocumentation/classes/LeanTween.html
		LeanTween.value(this.gameObject, FadeLoopUpdateByValue, 0.0f, 1.0f, (m_FadeLoop.Time)/2.0f)
			.setDelay(delay)
				.setEase(LeanTweenEaseType(m_FadeLoop.EaseType))
				.setUseEstimatedTime( true );
		LeanTween.value(this.gameObject, FadeLoopUpdateByValue, 1.0f, 0.0f, (m_FadeLoop.Time)/2.0f)
			.setDelay(delay+((m_FadeLoop.Time)/2.0f))
				.setEase(LeanTweenEaseType(m_FadeLoop.EaseType))
				.setOnComplete(FadeLoopComplete)
				.setUseEstimatedTime( true );
		#endif
		
		m_FadeLoop.Began = true;
		m_FadeLoop.Done = false;
	}
	
	#if HOTWEEN
	// This function is used by HOTween.To()
	void FadeLoopUpdate()
	{
		FadeLoopUpdateByValue(m_FadeVariable);
	}
	#endif

	// This function is used by iTween.ValueTo() and LeanTween.value()
	void FadeLoopUpdateByValue(float Value)
	{
		// Pause fade-loop when fade in/out is animating
		if((m_FadeIn.Enable && m_FadeIn.Began==true && m_FadeIn.Done==false) || 
		   (m_FadeOut.Enable && m_FadeOut.Began==true && m_FadeOut.Done==false))
		{
			return;
		}
		else
		{			
			RecursiveFade(this.transform, m_FadeLoop.Min + ((m_FadeLoop.Max-m_FadeLoop.Min) * Value), m_FadeLoop.FadeChildren);			
		}
	}

	// Fade-Loop animate completed
	void FadeLoopComplete()
	{
		if(m_FadeLoop.Done==true)
		{
		}
		else
		{
			FadeLoopStart(0);
		}
	}
	
	#endregion
	
	// ######################################################################
	// Utilities Functions
	// ######################################################################
	
	#region Utilities
	
	// Find total bounds size
	void CalculateTotalBounds()
	{
		m_TotalBounds = new Bounds(Vector3.zero, Vector3.zero);

		if(m_Slider!=null || m_Toggle!=null || m_CanvasRenderer!=null)
		{
			m_ParentCanvasRectTransform = m_Parent_Canvas.GetComponent<RectTransform>();
			RectTransform pRectTransform = this.gameObject.GetComponent<RectTransform>();
			m_TotalBounds.size = new Vector3(pRectTransform.rect.width * m_ParentCanvasRectTransform.localScale.x, pRectTransform.rect.height * m_ParentCanvasRectTransform.localScale.y, 0);
		}

	}
	
	// Reset position when screen resolution is changed
	public void ScreenResolutionChange()
	{
		InitMoveIn();
		
		InitMoveOut();
	}
	
	public bool IsAnimating()
	{
		if(m_MoveIn.Began == true || m_MoveOut.Began == true ||
		   m_ScaleIn.Began == true || m_ScaleOut.Began == true ||
		   m_FadeIn.Began == true || m_FadeOut.Began == true)
		{
			return true;
		}
		
		return false;
	}
	
	// Recursive funtion for fading
	void RecursiveFade(Transform trans, float Fade, bool IsFadeChildren)
	{
		bool SkipFade = false;
		if(this.transform != trans)
		{
			GEAnim pGEAnim = trans.GetComponent<GEAnim>();
			if(pGEAnim!=null)
			{
				if(pGEAnim.m_FadeIn.Enable==true && pGEAnim.m_FadeIn.Began==true)
				{
					SkipFade = true;
				}
				else if(pGEAnim.m_FadeOut.Enable==true && pGEAnim.m_FadeOut.Began==true)
				{
					SkipFade = true;
				}
			}
		}

		if(SkipFade==false)
		{
			Image pImage = trans.gameObject.GetComponent<Image>();
			if(pImage!=null)
			{
				pImage.color = new Color(pImage.color.r, pImage.color.g, pImage.color.b, Fade);
			}
			Text pText = trans.gameObject.GetComponent<Text>();
			if(pText!=null)
			{
				pText.color = new Color(pText.color.r, pText.color.g, pText.color.b, Fade);
			}
			RawImage pRawImage = trans.gameObject.GetComponent<RawImage>();
			if(pRawImage!=null)
			{
				pRawImage.color = new Color(pRawImage.color.r, pRawImage.color.g, pRawImage.color.b, Fade);
			}
		}
		
		// Fade children
		if(IsFadeChildren==true)
		{
			foreach(Transform child in trans)
			{
				RecursiveFade(child.transform, Fade, IsFadeChildren);
			}
		}
	}
	
	#endregion
	
	// ######################################################################
	// EaseType Converter for HOTween/LeanTween/iTween
	// ######################################################################
	
	#region EaseType Converter
	
	#if HOTWEEN
	// HOTween: https://www.assetstore.unity3d.com/#/content/3311 Documentation:  http://www.holoville.com/hotween/documentation.html
	public Holoville.HOTween.EaseType HOTweenEaseType(eEaseType easeType)
	{
		Holoville.HOTween.EaseType result = Holoville.HOTween.EaseType.Linear;
		switch (easeType)
		{
		case eEaseType.InQuad:			result = Holoville.HOTween.EaseType.EaseInQuad; break;
		case eEaseType.OutQuad:			result = Holoville.HOTween.EaseType.EaseOutQuad; break;
		case eEaseType.InOutQuad:		result = Holoville.HOTween.EaseType.EaseInOutQuad; break;
		case eEaseType.InCubic:			result = Holoville.HOTween.EaseType.EaseOutCubic; break;
		case eEaseType.OutCubic:		result = Holoville.HOTween.EaseType.EaseOutCubic; break;
		case eEaseType.InOutCubic:		result = Holoville.HOTween.EaseType.EaseInOutCubic; break;
		case eEaseType.InQuart:			result = Holoville.HOTween.EaseType.EaseInQuart; break;
		case eEaseType.OutQuart:		result = Holoville.HOTween.EaseType.EaseOutQuart; break;
		case eEaseType.InOutQuart:		result = Holoville.HOTween.EaseType.EaseInOutQuart; break;
		case eEaseType.InQuint:			result = Holoville.HOTween.EaseType.EaseInQuint; break;
		case eEaseType.OutQuint:		result = Holoville.HOTween.EaseType.EaseOutQuint; break;
		case eEaseType.InOutQuint:		result = Holoville.HOTween.EaseType.EaseInOutQuint; break;
		case eEaseType.InSine:			result = Holoville.HOTween.EaseType.EaseInSine; break;
		case eEaseType.OutSine:			result = Holoville.HOTween.EaseType.EaseOutSine; break;
		case eEaseType.InOutSine:		result = Holoville.HOTween.EaseType.EaseInOutSine; break;
		case eEaseType.InExpo:			result = Holoville.HOTween.EaseType.EaseInExpo; break;
		case eEaseType.OutExpo:			result = Holoville.HOTween.EaseType.EaseOutExpo; break;
		case eEaseType.InOutExpo:		result = Holoville.HOTween.EaseType.EaseInOutExpo; break;
		case eEaseType.InCirc:			result = Holoville.HOTween.EaseType.EaseInCirc; break;
		case eEaseType.OutCirc:			result = Holoville.HOTween.EaseType.EaseOutCirc; break;
		case eEaseType.InOutCirc:		result = Holoville.HOTween.EaseType.EaseInOutCirc; break;
		case eEaseType.linear:			result = Holoville.HOTween.EaseType.Linear; break;
		case eEaseType.InBounce:		result = Holoville.HOTween.EaseType.EaseInBounce; break;
		case eEaseType.OutBounce:		result = Holoville.HOTween.EaseType.EaseOutBounce; break;
		case eEaseType.InOutBounce:		result = Holoville.HOTween.EaseType.EaseInOutBounce; break;
		case eEaseType.InBack:			result = Holoville.HOTween.EaseType.EaseInBack; break;
		case eEaseType.OutBack:			result = Holoville.HOTween.EaseType.EaseOutBack; break;
		case eEaseType.InOutBack:		result = Holoville.HOTween.EaseType.EaseInOutBack; break;
		case eEaseType.InElastic:		result = Holoville.HOTween.EaseType.EaseInElastic; break;
		case eEaseType.OutElastic:		result = Holoville.HOTween.EaseType.EaseOutElastic; break;
		case eEaseType.InOutElastic:	result = Holoville.HOTween.EaseType.EaseInOutElastic; break;
		default:						result = Holoville.HOTween.EaseType.Linear; break;
		}
		return result;
	}
	#elif ITWEEN
	// iTween: https://www.assetstore.unity3d.com/#/content/84 Documentation: http://itween.pixelplacement.com/documentation.php
	public string iTweenEaseType(eEaseType easeType)
	{
		string result = "linear";
		switch (easeType)
		{
		case eEaseType.InQuad:			result = "easeInQuad"; break;
		case eEaseType.OutQuad:			result = "easeOutQuad"; break;
		case eEaseType.InOutQuad:		result = "easeInOutQuad"; break;
		case eEaseType.InCubic:			result = "easeOutCubic"; break;
		case eEaseType.OutCubic:		result = "easeOutCubic"; break;
		case eEaseType.InOutCubic:		result = "easeInOutCubic"; break;
		case eEaseType.InQuart:			result = "easeInQuart"; break;
		case eEaseType.OutQuart:		result = "easeOutQuart"; break;
		case eEaseType.InOutQuart:		result = "easeInOutQuart"; break;
		case eEaseType.InQuint:			result = "easeInQuint"; break;
		case eEaseType.OutQuint:		result = "easeOutQuint"; break;
		case eEaseType.InOutQuint:		result = "easeInOutQuint"; break;
		case eEaseType.InSine:			result = "easeInSine"; break;
		case eEaseType.OutSine:			result = "easeOutSine"; break;
		case eEaseType.InOutSine:		result = "easeInOutSine"; break;
		case eEaseType.InExpo:			result = "easeInExpo"; break;
		case eEaseType.OutExpo:			result = "easeOutExpo"; break;
		case eEaseType.InOutExpo:		result = "easeInOutExpo"; break;
		case eEaseType.InCirc:			result = "easeInCirc"; break;
		case eEaseType.OutCirc:			result = "easeOutCirc"; break;
		case eEaseType.InOutCirc:		result = "easeInOutCirc"; break;
		case eEaseType.linear:			result = "linear"; break;
		case eEaseType.InBounce:		result = "easeInBounce"; break;
		case eEaseType.OutBounce:		result = "easeOutBounce"; break;
		case eEaseType.InOutBounce:		result = "easeInOutBounce"; break;
		case eEaseType.InBack:			result = "easeInBack"; break;
		case eEaseType.OutBack:			result = "easeOutBack"; break;
		case eEaseType.InOutBack:		result = "easeInOutBack"; break;
		case eEaseType.InElastic:		result = "easeInElastic"; break;
		case eEaseType.OutElastic:		result = "easeOutElastic"; break;
		case eEaseType.InOutElastic:	result = "easeInOutElastic"; break;
		default:						result = "linear"; break;
		}
		return result;
	}
	#else
	// LeanTween: https://www.assetstore.unity3d.com/#/content/3595 Documentation: http://dentedpixel.com/LeanTweenDocumentation/classes/LeanTween.html
	public LeanTweenType LeanTweenEaseType(eEaseType easeType)
	{
		LeanTweenType result = LeanTweenType.linear;
		switch (easeType)
		{
		case eEaseType.InQuad:			result = LeanTweenType.easeInQuad; break;
		case eEaseType.OutQuad:			result = LeanTweenType.easeOutQuad; break;
		case eEaseType.InOutQuad:		result = LeanTweenType.easeInOutQuad; break;
		case eEaseType.InCubic:			result = LeanTweenType.easeOutCubic; break;
		case eEaseType.OutCubic:		result = LeanTweenType.easeOutCubic; break;
		case eEaseType.InOutCubic:		result = LeanTweenType.easeInOutCubic; break;
		case eEaseType.InQuart:			result = LeanTweenType.easeInQuart; break;
		case eEaseType.OutQuart:		result = LeanTweenType.easeOutQuart; break;
		case eEaseType.InOutQuart:		result = LeanTweenType.easeInOutQuart; break;
		case eEaseType.InQuint:			result = LeanTweenType.easeInQuint; break;
		case eEaseType.OutQuint:		result = LeanTweenType.easeOutQuint; break;
		case eEaseType.InOutQuint:		result = LeanTweenType.easeInOutQuint; break;
		case eEaseType.InSine:			result = LeanTweenType.easeInSine; break;
		case eEaseType.OutSine:			result = LeanTweenType.easeOutSine; break;
		case eEaseType.InOutSine:		result = LeanTweenType.easeInOutSine; break;
		case eEaseType.InExpo:			result = LeanTweenType.easeInExpo; break;
		case eEaseType.OutExpo:			result = LeanTweenType.easeOutExpo; break;
		case eEaseType.InOutExpo:		result = LeanTweenType.easeInOutExpo; break;
		case eEaseType.InCirc:			result = LeanTweenType.easeInCirc; break;
		case eEaseType.OutCirc:			result = LeanTweenType.easeOutCirc; break;
		case eEaseType.InOutCirc:		result = LeanTweenType.easeInOutCirc; break;
		case eEaseType.linear:			result = LeanTweenType.linear; break;
		case eEaseType.InBounce:		result = LeanTweenType.easeInBounce; break;
		case eEaseType.OutBounce:		result = LeanTweenType.easeOutBounce; break;
		case eEaseType.InOutBounce:		result = LeanTweenType.easeInOutBounce; break;
		case eEaseType.InBack:			result = LeanTweenType.easeInBack; break;
		case eEaseType.OutBack:			result = LeanTweenType.easeOutBack; break;
		case eEaseType.InOutBack:		result = LeanTweenType.easeInOutBack; break;
		case eEaseType.InElastic:		result = LeanTweenType.easeInElastic; break;
		case eEaseType.OutElastic:		result = LeanTweenType.easeOutElastic; break;
		case eEaseType.InOutElastic:	result = LeanTweenType.easeInOutElastic; break;
		default:						result = LeanTweenType.linear; break;
		}
		return result;
	}
	#endif


	
	#endregion
}