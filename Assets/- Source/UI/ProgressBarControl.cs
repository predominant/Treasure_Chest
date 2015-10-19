using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

[ExecuteInEditMode]
public class ProgressBarControl : MonoBehaviour
{

	#region Enums
	public enum Orientation
	{
		Horizontal = 0,
		Vertical,
	}
	#endregion

	#region Properties
	public float _Scale { get { return _scale; } }
	public float _ValueRatio { get { return _zeroAdjustedValue * _inverseRangeRatio; } }
	public Orientation _Orientation { get { return _orientation; } set { } }
	#endregion

	#region Member Variables
	public RectTransform _fillBar = null;
	public Orientation _orientation = Orientation.Horizontal;
	public Ease _easeType = Ease.Linear;
	public float _easeTime = 0f;
	protected float _scale = 0f;
	public RectTransform _track = null;
	public float _minWidth = 0f;
	public float _maxWidth = 0f;
	public float _minHeight = 0f;
	public float _maxHeight = 0f;
	protected Tween _scalingTween = null;
	public float _valueCur = 0f;
	public float _valueMin = 0f;
	public float _valueMax = 1f;
	protected float _zeroAdjustedValue = 0f;
	protected float _inverseRangeRatio = 0f;

	#region Editor-Only Values
	public bool _useMarginWidth = false;
	public bool _useMarginHeight = false;
	public float _marginWidthMin = 0f;
	public float _marginWidthMax = 0f;
	public float _marginHeightMin = 0f;
	public float _marginHeightMax = 0f;
	#endregion
	#endregion

	#region Unity Events
	public void Awake()
	{
		//if (null != _fillBar)
		//{
		//	if (_maxWidth == 0f)
		//		_maxWidth = _fillBar.rect.width;

		//	if (_maxHeight == 0f)
		//		_maxHeight = _fillBar.rect.height;
		//}
	}

#if UNITY_EDITOR
	void Update()
	{
		SetValueRange(_valueMin, _valueMax);
		SetValue(_valueCur, true);
	}
#endif
	#endregion

	#region Interface
	/// <summary>
	/// Set the new value range
	/// </summary>
	/// <param name="minVal">New target minimal value ( < max value )</param>
	/// <param name="maxVal">New target maximal value ( > min value )</param>
	public void SetValueRange(float minVal, float maxVal)
	{
		// Assign new values
		_valueMin = minVal;
		_valueMax = maxVal;

		// Run our standard set-value tests - clamping & update zero adjusted value
		SetValue(_valueCur);

		// Cache our inverse range so we can do quick mults later
		float range = _valueMax - _valueMin;
		_inverseRangeRatio = 1 / (range);
	}

	/// <summary>
	/// Change the value of the progress bar (contrained by range)
	/// </summary>
	/// <param name="newValue">The new target value</param>
	/// <param name="immediate">If true, will skip tweening</param>
	public void SetValue(float newValue, bool immediate = false)
	{
		_valueCur = newValue;
		_valueCur = Mathf.Clamp(_valueCur, _valueMin, _valueMax);
		_zeroAdjustedValue = _valueCur - _valueMin;
		UpdateScale(_ValueRatio, immediate);
	}
	#endregion

	#region Internal
	protected void UpdateOrientation(Orientation orientation)
	{
		if (orientation == _orientation) { return; }
		_orientation = orientation;

		UpdateScale(_scale, true, true);
	}

	protected void UpdateScale(float newScale, bool immediate = false, bool forceUpdate = false)
	{
		if (forceUpdate == false && newScale == _scale)
		{
			return;
		}

		_scale = newScale;

		// Generate our target scale based on orientation
		Vector3 targetSize = new Vector2(_maxWidth, _maxHeight);
		switch (_orientation)
		{
			case Orientation.Horizontal:
				targetSize.x = _minWidth + _scale * (_maxWidth - _minWidth);
				break;

			case Orientation.Vertical:
				targetSize.y = _minHeight + _scale * (_maxHeight - _minHeight);
				break;
		}

		// If we have a tween in effect, stop it
		if (_scalingTween != null)
		{
			_scalingTween.Complete();
			_scalingTween = null;
		}

		if (immediate)
		{
			_fillBar.sizeDelta = targetSize;
		}
		else
		{
			_scalingTween = DOTween.To(() => _fillBar.sizeDelta, x => _fillBar.sizeDelta = x, targetSize, _easeTime)
									.SetEase(_easeType);
		}
	}
	#endregion
}
