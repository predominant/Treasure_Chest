using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Devdog.InventorySystem.UI
{
    using UnityEngine.Serialization;

    [RequireComponent(typeof(Animator))]
    public partial class UIWindow : MonoBehaviour
    {
        #region Events & Delegates

        public delegate void WindowShowHide();

        /// <summary>
        /// Event is fired when the window is hidden.
        /// </summary>
        public event WindowShowHide OnHide;

        /// <summary>
        /// Event is fired when the window becomes visible.
        /// </summary>
        public event WindowShowHide OnShow;

        #endregion


        #region Variables 


        [Header("Behavior")]
        public string windowName = "MyWindow";
        
        /// <summary>
        /// Should the window be hidden when the game starts?
        /// </summary>
        public bool hideOnStart = true;

        /// <summary>
        /// Set the position to 0,0 when the game starts
        /// </summary>
        public bool resetPositionOnStart = true;

        /// <summary>
        /// The animation played when showing the window, if null the item will be shown without animation.
        /// </summary>
        [Header("Audio & Visuals")]
        [SerializeField]
        [FormerlySerializedAs("showAnimation")]
        private AnimationClip _showAnimation;
        public int showAnimationHash { get; protected set; }

        /// <summary>
        /// The animation played when hiding the window, if null the item will be hidden without animation. 
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("hideAnimation")]
        private AnimationClip _hideAnimation;
        public int hideAnimationHash { get; protected set; }

        public AudioClip showAudioClip;
        public AudioClip hideAudioClip;

        
        /// <summary>
        /// Is the window visible or not? Used for toggling.
        /// </summary>
        public bool isVisible { get; protected set; }

        private IEnumerator _animationCoroutine;


        private List<UIWindowPage> _pages;
        public List<UIWindowPage> pages
        {
            get
            {
                if (_pages == null)
                    _pages = new List<UIWindowPage>();

                return _pages;
            }
            protected set
            {
                _pages = value;
            }
        }



        public UIWindowPage currentPage
        {
            get;
            set;
        }

        private Animator _animator;
        public Animator animator
        {
            get
            {
                if (_animator == null)
                    _animator = GetComponent<Animator>();

                return _animator;
            }
        }

        private RectTransform _rectTransform;
        protected RectTransform rectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();

                return _rectTransform;
            }
        }



        #endregion




        public virtual void Awake()
        {
            if(_showAnimation != null)
                showAnimationHash = Animator.StringToHash(_showAnimation.name);

            if(_hideAnimation != null)
                hideAnimationHash = Animator.StringToHash(_hideAnimation.name);

            LevelStart();
        }
        
        public void OnLevelWasLoaded(int level)
        {
            LevelStart();
        }

        protected virtual void LevelStart()
        {
            if (hideOnStart)
                HideFirst();
            else
            {
                isVisible = true;
            }
        }


        #region Notifies

        /// <summary>
        /// One of our children pages has been shown
        /// </summary>
        public void NotifyChildShown(UIWindowPage page)
        {
            HideOtherPages(page);

            if (isVisible == false)
                Show(true);
        }

        public void NotifyWindowHidden()
        {
            if (OnHide != null)
                OnHide();

        }

        public void NotifyWindowShown()
        {
            if (OnShow != null)
                OnShow();

        }

        #endregion



        public void HideOtherPages(UIWindowPage page, float waitTime = 0.0f)
        {
            foreach (var item in pages)
            {
                if (item.isVisible && item != page)
                {
                    item.NotifyWindowHidden();
                    StartCoroutine(item._WaitAndStartHide(waitTime, false));
                }
            }
        }

        protected virtual void SetChildrenActive(bool active)
        {
            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(active);
            }

            var img = gameObject.GetComponent<UnityEngine.UI.Image>();
            if (img != null)
                img.enabled = active;
        }


        private void PlayAnimation(AnimationClip clip, int hash, Action callback)
        {
            if (clip != null)
            {
                if (_animationCoroutine != null)
                {
                    StopCoroutine(_animationCoroutine);
                }

                _animationCoroutine = _PlayAnimationAndDisableAnimator(clip.length + 0.1f, hash, callback);
                StartCoroutine(_animationCoroutine);
            }
            else
            {
                animator.enabled = false;
                if (callback != null)
                    callback();

            }
        }

        public virtual void Toggle()
        {
            if (isVisible)
                Hide();
            else
                Show();
        }

        public virtual void Show()
        {
            Show(true);
        }

        public virtual void Show(bool fireEvents)
        {
            if (isVisible)
                return;

            isVisible = true;
            SetChildrenActive(true);
            PlayAnimation(_showAnimation, showAnimationHash, null);

            if (showAudioClip != null)
            {
                InventoryUtility.AudioPlayOneShot(showAudioClip);
            }

            if (fireEvents)
                NotifyWindowShown();
        }

        public void Show(float waitTime)
        {
            Show(waitTime, true);
        }

        public void Show(float waitTime, bool fireEvents)
        {
            if (waitTime > 0.0f)
                StartCoroutine(_Show(waitTime, fireEvents));
            else
                Show(fireEvents);
        }

        protected virtual IEnumerator _Show(float waitTime, bool fireEvents = true)
        {
            yield return StartCoroutine(CustomWait(waitTime));
            Show(fireEvents);
        }


        public virtual void HideFirst()
        {
            isVisible = false;
            animator.enabled = false;

            SetChildrenActive(false);

            if (resetPositionOnStart)
                rectTransform.anchoredPosition = Vector2.zero;
        }

        /// <summary>
        /// Convenience method for easy upgrading...
        /// </summary>
        public virtual void Hide()
        {
            Hide(true);
        }

        public virtual void Hide(bool fireEvents)
        {
            if (isVisible == false)
                return;

            isVisible = false;
            PlayAnimation(_hideAnimation, hideAnimationHash, () =>
                {
                    // Still invisible? Maybe it got shown while we waited.
                    if (isVisible == false)
                        SetChildrenActive(false);

                });

            if (hideAudioClip != null)
            {
                InventoryUtility.AudioPlayOneShot(hideAudioClip);
            }

            if (fireEvents)
                NotifyWindowHidden();
        }

        public void Hide(float waitTime)
        {
            Hide(waitTime, true);
        }

        public void Hide(float waitTime, bool fireEvents)
        {
            if (waitTime > 0.0f)
                StartCoroutine(_WaitAndStartHide(waitTime, fireEvents));
            else
                Hide(fireEvents);
        }

        protected virtual IEnumerator _WaitAndStartHide(float waitTime, bool fireEvents = true)
        {
            yield return StartCoroutine(CustomWait(waitTime));
            Hide(fireEvents);
        }


        /// <summary>
        /// Hides object after animation is completed.
        /// </summary>
        /// <param name="animation"></param>
        /// <returns></returns>
        protected virtual IEnumerator _PlayAnimationAndDisableAnimator(float waitTime, int hash, Action callback)
        {
            yield return null; // Needed for some reason, Unity bug??

            var before = _animationCoroutine;
            animator.enabled = true;
            animator.Play(hash);

            yield return StartCoroutine(CustomWait(waitTime));

            // If action completed without any other actions overriding isVisible should be true. It could be hidden before the coroutine finished.
            animator.enabled = false;
            if (callback != null)
                callback();

            if (before == _animationCoroutine)
            {
                // Didn't change curing coroutine
                _animationCoroutine = null;
            }
        }

        protected IEnumerator CustomWait(float waitTime)
        {
            float start = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup < start + waitTime)
            {
                yield return null;
            }
        }
    }
}
