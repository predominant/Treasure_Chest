using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using System;

public class SlideToggleControl : MonoBehaviour {

	#region Event Dispatchers
	[SerializeField]
	//public UnityEngine.Events.UnityEvent<bool> _onToggle; // Generic unity event not found as a serializedproperty in the editor
	public event Action<bool> _onToggle;
	#endregion

	#region Member Variables
	public Button _toggleButton = null;	// Required
	public Image _toggleImage = null;	// Required

	public TMPro.TextMeshProUGUI _textProxy = null;
	public string _onText = "On";
	public string _offText = "Off";

	public Sprite _onToggleSprite = null;
	public Sprite _offToggleSprite = null;

	public Vector3 _onPosition = Vector3.zero;
	public Vector3 _offPosition = Vector3.zero;

	public Ease _slideEase = Ease.Linear;
	public float _slideTime = 0.5f;

	public bool _startOn = true;

	protected bool _isOn = true;
	protected Tweener _currentTween = null;
	#endregion

	#region Unity Events
	void Start() {
		SetState( _startOn );
		BindButton( _toggleButton );
	}
	#endregion

	#region Interface
	public void BindButton(Button button) {
		UnbindButtonEvents();
		_toggleButton = button;
		BindButtonEvents();
	}

	public void SetState( bool on ) {
		// Cache state-dependant variables
		Vector3 targetLoc = on ? _onPosition : _offPosition;
		Sprite targetSprite = on ? _onToggleSprite : _offToggleSprite;
		string targetText = on ? _onText : _offText;

		// Stop any existing tweens
		if (_currentTween != null) { _currentTween.Complete(); }

		// Start moving our button
		_currentTween = _toggleButton.transform.DOLocalMove( targetLoc, _slideTime )
												.SetEase( _slideEase )
												.OnComplete( () => { 
														_toggleImage.sprite = targetSprite;								// Update our button sprite 
														_currentTween = null;
														if (_textProxy != null) { _textProxy.text = targetText; }		// Update our proxy text
														if (_isOn != on && _onToggle != null) { _onToggle.Invoke(on); }	// Fire a toggle event
														_isOn = on;
													} );

	}
	#endregion

	#region Internal
	protected void BindButtonEvents() {
		_toggleButton.onClick.AddListener(OnButtonClicked);
	}

	protected void UnbindButtonEvents() {
		if (_toggleButton == null) { return; }
		_toggleButton.onClick.RemoveListener(OnButtonClicked);
	}

	protected void OnButtonClicked() {	
		SetState( !_isOn );
	}
	#endregion
}