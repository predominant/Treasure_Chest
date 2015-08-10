using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;


namespace uGUIPanelManager
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(CanvasRenderer))]
    [System.Serializable]
    public class uGUIManagedPanel : MonoBehaviour
    {


        public bool isMainPanel = false;
        public bool isMoving = false;

        //Animation without animator
        public float duration = 1;
        public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [HideInInspector]
        [SerializeField]
        private PanelStateSettings[]
            _stateTransform;

        public PanelStateSettings[] stateTransform
        {
            get
            {
                if (_stateTransform == null || _stateTransform.Length < System.Enum.GetNames(typeof(PanelState)).Length)
                {
                    PanelStateSettings[] oldstateTransform = _stateTransform;

                    _stateTransform = new PanelStateSettings[System.Enum.GetNames(typeof(PanelState)).Length];
                    for (int i = 0; i < _stateTransform.Length; i++)
                    {
                        if (oldstateTransform != null && oldstateTransform.Length > i)
                        {
                            _stateTransform [i] = oldstateTransform [i];
                        }
                        else
                        {
                            _stateTransform [i] = new PanelStateSettings();
                        }
                    }
                }

                return _stateTransform;
            }
        }
       
        public PanelStateSettings currstateTransform;
        

			

        //temps for aniamtion without animator
        public float time = 0;


        public PanelState panelState = PanelState.Show;

        private GameObject cachedGameObject;
        public GameObject CachedGameObject
        {
            get
            {
                if (cachedGameObject == null)
                {
                    cachedGameObject = this.gameObject;
                }
                return cachedGameObject;
            }
        }

        private Animator cachedAnimator;
        public Animator CachedAnimator
        {
            get
            {
                if (cachedAnimator == null)
                {
                    cachedAnimator = this.GetComponent<Animator>();
                }
                return cachedAnimator;
            }
        }

        private RectTransform cachedRectTransform;
        public RectTransform CachedRectTransform
        {
            get
            {
                if (cachedRectTransform == null)
                {
                    cachedRectTransform = this.GetComponent<RectTransform>();
                }
                return cachedRectTransform;
            }
        }

        private CanvasGroup cachedCanvasGroup;
        public CanvasGroup CachedCanvasGroup
        {
            get
            {
                if (cachedCanvasGroup == null)
                {
                    cachedCanvasGroup = this.GetComponent<CanvasGroup>();
                    if (cachedCanvasGroup == null)
                    {
                        cachedCanvasGroup = CachedGameObject.AddComponent<CanvasGroup>();
                    }
                }
                return cachedCanvasGroup;
            }
        }

#if UNITY_EDITOR

        void Start()
        {
            uGUIManager.instance.SearchPaneles();
        }
#endif

        public void SetState(PanelState targetState)
        {
            if (CachedAnimator != null)
            {
                CachedAnimator.SetTrigger(stateTransform [(int)targetState].propertyname);
            }
        }

        /// <summary>
        /// Not ment for direct calling please call uGUIManager.instance.ShowPanel
        /// </summary>
        public void Show()
        {

            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                CachedGameObject.SetActive(true);
                panelState = PanelState.Show;
            }
            #endif
            #if UNITY_EDITOR
            if (Application.isPlaying)
            {
                #endif
                if (CachedAnimator != null)
                {
                    CachedAnimator.SetTrigger(stateTransform [(int)PanelState.Show].propertyname);

                }
            }
            #if UNITY_EDITOR
        }
	#endif

        /// <summary>
        /// Not ment for direct calling please call uGUIManager.instance.HidePanel
        /// </summary>
        public void Hide()
        {
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                CachedGameObject.SetActive(false);
                panelState = PanelState.Hide;
            }
            #endif
            #if UNITY_EDITOR
            if (Application.isPlaying)
            {
                #endif
                if (CachedAnimator != null)
                {
                    CachedAnimator.SetTrigger(stateTransform [(int)PanelState.Hide].propertyname);
                }
                #if UNITY_EDITOR
            }
            #endif
        }

        public bool HasFinishedAnimation()
        {
            if (time >= duration)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [System.Serializable]
    public class PanelStateSettings
    {
        //Basic Animation
        public Vector2 pos = Vector2.zero;
        public Vector3 rot = Vector3.zero;
        public Vector3 scale = Vector3.one;
        public float alpha = 1f;
        //Animator TriggerName
        public string propertyname = "";
        //Call Method
        public UnityEvent callBeforeEnterState;
        public UnityEvent callAfterEnterState;
        public UnityEvent callBeforeLeaveState;
        public UnityEvent callAfterLeaveState;

    }

}